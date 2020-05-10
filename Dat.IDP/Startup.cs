// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using Dat.Data;
using Dat.Database.Abstract;
using Dat.Domain;
using Dat.IDP.Services;
using Homer.DbContext;
using Homer.Models.Options;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Dat.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            //var connectionString = Configuration.GetSection("ConfigureDbOptions")["ConnectionString"];
            //services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(authConnectionString));
            services.Configure<StoreOptions>(Configuration.GetSection("StoreOptions"));
            services.AddScoped<IMongoClient, MongoClient>(_ => new MongoClient(Configuration.GetValue<string>("StoreOptions:ConnectionString")));
            services.AddScoped<IBackStoreContext<IMongoDatabase>, StoreMongoDbContext>();
            services.AddScoped<IUserProfileRepository<IMongoDatabase>, UserProfileMongoRepository>();
            services.AddSingleton<Func<string, HashedData>>(val => new HashedData { Value = val.Sha256() });
            var builder = services.AddIdentityServer()
                //.AddInMemoryIdentityResources(Config.Ids)
                //.AddInMemoryApiResources(Config.Apis)
                //.AddInMemoryClients(Config.Clients)                
                //.AddTestUsers(TestUsers.Users);
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
                //.Services.AddSingleton<IUserProfileRepository<IMongoDatabase>, UserProfileMongoRepository>();
            builder.Services.AddScoped<IUserProfileRepository<IMongoDatabase>, UserProfileMongoRepository>();



            // not recommended for production - you need to store your key material somewhere secure
            //builder.AddDeveloperSigningCredential();
            var cert = LoadCertificate();
            builder.AddSigningCredential(cert);

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            //var connectionString = @"Data Source =.\sqlexpress; Initial Catalog = DatllcIDPDb; Integrated Security = True";
            //var connectionString = @"Server=(localhost)\\mssqllocaldb;Database=DatllcIDPDb;Trusted_Connection=true;";

            var connectionString = Configuration.GetSection("ConfigureDbOptions")["ConnectionString"];
            builder.AddConfigurationStore(opt => {
                opt.ConfigureDbContext = dbcontextbuilder => 
                dbcontextbuilder.UseSqlServer(
                    connectionString, //Configuration.Get<ConfigureDbOptions>().ConnectionString,
                    builder =>builder.MigrationsAssembly(migrationAssembly)
                    );
            });

            //var userConnectionString = Configuration.GetSection("UserDbOptions")["ConnectionString"];
            builder.AddOperationalStore(options => {
                options.ConfigureDbContext = dbcontextBuilder =>
                dbcontextBuilder.UseSqlServer(
                    connectionString,
                    builder =>builder.MigrationsAssembly(migrationAssembly)
                    );
            });
            var dataModelAssembly = Assembly.Load("Dat.Domain");
            services.AddAutoMapper(dataModelAssembly);

            
        }
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            InitializeDatabase(app).GetAwaiter().GetResult();
            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        public X509Certificate2 LoadCertificate()
        {
            var path = System.IO.Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, "Certs/datllc.pfx");
            var cert = new X509Certificate2(filePath,"Datllc",X509KeyStorageFlags.Exportable);
            return cert;
        }
        private async Task InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                await PopulateConfigurationDbAsync(serviceScope);
            }
            //static async Task PopulateUserDbAsync(IServiceScope serviceScope)
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            //    context.Database.Migrate();
            //    var hasUsers=await context.
            //}
            static async Task PopulateConfigurationDbAsync(IServiceScope serviceScope)
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();
                var hasClients = await context.Clients.AnyAsync();
                if (!hasClients)
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }
                var hasIdentityResources = await context.IdentityResources.AnyAsync();
                if (!hasIdentityResources)
                {
                    foreach (var idResource in Config.Ids)
                    {
                        context.IdentityResources.Add(idResource.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }
                var hasApis = await context.ApiResources.AnyAsync();
                if (!hasApis)
                {
                    foreach (var api in Config.Apis)
                    {
                        context.ApiResources.Add(api.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
