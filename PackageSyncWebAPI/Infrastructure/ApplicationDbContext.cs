using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PackageSync.Domain;

namespace PackageSyncWebAPI.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Package> Packages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Package>().HasData(
                new Package { Id = Guid.NewGuid(), Name = "Chair", Status = PackageStatus.InWarehouse, DateOfCreation = DateTime.Now, DateOfDelivery = null },
                new Package { Id = Guid.NewGuid(), Name = "Table", Status = PackageStatus.InWarehouse, DateOfCreation = DateTime.Now, DateOfDelivery = null },
                new Package { Id = Guid.NewGuid(), Name = "Bed", Status = PackageStatus.InTransit, DateOfCreation = new DateTime(2024, 12, 14, 11, 50, 00), DateOfDelivery = new DateTime(2024, 12, 20, 14, 30, 0) },
                new Package { Id = Guid.NewGuid(), Name = "Lamp", Status = PackageStatus.InTransit, DateOfCreation = new DateTime(2024, 12, 13, 18, 25, 00), DateOfDelivery = new DateTime(2024, 12, 25, 15, 0, 0) },
                new Package { Id = Guid.NewGuid(), Name = "Book", Status = PackageStatus.Delivered, DateOfCreation = new DateTime(2024, 12, 10, 10, 45, 00), DateOfDelivery = new DateTime(2024, 12, 15, 12, 0, 0) }
            );
        }
    }
}
