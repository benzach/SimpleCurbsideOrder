using Dat.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dat.Data
{
    public class DatContext:DbContext
    {
        public DbSet<UserProfile> UserProfile { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = DatData");

             }
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<UserProfile>()
        //                .HasMany(x => x.Claims)
        //                .WithOne(x => x.UserProfile)
        //                .OnDelete(DeleteBehavior.Cascade);
                        

        //}
    }
}
