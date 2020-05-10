using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Dat.Database.Abstract;
using Homer.Authentication.Services;
using Homer.Authorization.Handlers;
using Homer.Authorization.Interfaces;
using Homer.Authorization.Models;
using Homer.Authorization.Requirements;
using Homer.Authorization.Services;
using Homer.DbContext;
using Homer.Models.Interfaces;
using Homer.Models.Options;
using Homer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Homer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(
                        opt => {
                            opt.Authority = "https://localhost:6001";
                            opt.ApiName = "HomerApi";
                        }
                    );
            services.AddScoped< IAuthorizationHandler, SameCompanyAuthorizationHandler >();
            services.AddScoped<IAuthorizationHandler,SameUserAuthorizationHandler>();
            services.AddScoped<IResourceBasedAuthService, ResourceBasedAuthService>();
            //services.AddAuthentication("basic")
            //        .AddScheme<HomerAuthenticationSchemeOptions, HomerAuthenticationHandler>("basic", null);
            services.AddScoped<ITokenService, TokenService>();
            services.AddAuthorization(
                opt =>
                {
                    opt.AddPolicy("MustBeStoreOwner", policybuilder =>
                    {
                        policybuilder.AddAuthenticationSchemes(new[] { "Bearer" });
                        policybuilder.RequireAuthenticatedUser();
                        policybuilder.RequireRole(new[] { HomerRoleConstants.StoreOwner });
                        policybuilder.RequireClaim(HomerClaimConstants.Company);
                        policybuilder.RequireClaim(HomerClaimConstants.UserId);
                    });
                    opt.AddPolicy("SameCompany", policyBuilder => {
                        policyBuilder.Requirements = new List<IAuthorizationRequirement>( new[] { new SameCompanyRequirement() });
                    });
                    opt.AddPolicy("SameCompanyAndOwner", policyBuilder => {
                        policyBuilder.Requirements = new List<IAuthorizationRequirement>(new IAuthorizationRequirement[] { new SameCompanyRequirement(),new SameUserRequirement() });
                    });

                }
            );
            services.AddSwaggerGen(
            //    c => {
            //    var info = new OpenApiInfo
            //    {
            //        Title = "Homer Store Api",
            //        Version = "v1"
            //    };
            //    var securityScheme = new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.ApiKey,
            //        In = ParameterLocation.Header,
            //        Name = "Authorization",
            //        Description = $"Input \"Bearer {{token}}\" (without quotes)"
            //    };
            //    c.SwaggerDoc(name: "v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Homer Store Api", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", securityScheme);
            //    c.DescribeAllEnumsAsStrings();

            //    var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlpath = Path.Combine(AppContext.BaseDirectory, xmlfile);
            //    c.IncludeXmlComments(xmlpath, true);

            //}
            );
            services.Configure<StoreOptions>(Configuration.GetSection("StoreOptions"));
            services.AddScoped<IMongoClient, MongoClient>(_ => new MongoClient(Configuration.GetValue<string>("StoreOptions:ConnectionString")));
            services.AddScoped(typeof(IBackStoreContext<IMongoDatabase>), typeof(StoreMongoDbContext));
            services.AddScoped(typeof(IStoreRepository<IMongoDatabase>), typeof(StoresMongoRepository));
            var dataModelAssembly = Assembly.Load("Homer.Models");
            services.AddAutoMapper(dataModelAssembly);
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseSwagger();
            //app.UseSwaggerUI(c => {
            //    c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "HomerApi v1");
            //    c.RoutePrefix = string.Empty;
            //})
            //    .UseAuthentication()
            //    .UseAuthorization();

            app.UseHttpsRedirection();

            app.UseRouting()
                .UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
                .UseEndpoints(builder =>builder.MapHealthChecks(new PathString("/healthcheck")))
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(
                c => {
                    string jsonUrl = "/swagger/openapi.json";
                    c.SwaggerEndpoint(jsonUrl, "Homer Store Api");
                    c.RoutePrefix = string.Empty;
                }
                )
                .UseWhen(context=>
                    !context.Request.Path.Equals("/index.html",StringComparison.InvariantCultureIgnoreCase) &&
                    !context.Request.Path.Equals("/swagger/openapi.json", StringComparison.InvariantCultureIgnoreCase) &&
                    !context.Request.Path.StartsWithSegments("/healthcheck",StringComparison.InvariantCultureIgnoreCase) &&
                    !context.Request.Path.StartsWithSegments("/info",StringComparison.InvariantCultureIgnoreCase) ,
                    app1=>app1.UseRouting()
                          .UseAuthentication()
                          .UseAuthorization()
                          .UseEndpoints(builder =>builder.MapControllers()));
            //app.UseAuthentication();
            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
