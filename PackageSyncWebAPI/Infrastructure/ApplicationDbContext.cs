using Microsoft.EntityFrameworkCore;
using PackageSyncWebAPI.Models;

namespace PackageSyncWebAPI.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Package> Packages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
