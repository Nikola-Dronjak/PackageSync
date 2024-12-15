using Microsoft.EntityFrameworkCore;
using PackageSyncWebAPI.Infrastructure;
using PackageSyncWebAPI.Models;

namespace PackageSyncWebAPI.Services
{
    public class PackageService : IPackageService
    {
        private readonly ApplicationDbContext _dbContext;

        public PackageService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Package>> GetAll()
        {
            return await _dbContext.Packages.ToListAsync();
        }

        public async Task<Package> GetById(Guid id)
        {
            Package? packageFromDb = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
            if (packageFromDb == null)
                throw new KeyNotFoundException($"There is no package with the id of {id}.");

            return packageFromDb;
        }

        public async Task<Package> Add(Package package)
        {
            package.Id = Guid.NewGuid();
            package.Status = PackageStatus.InWarehouse;
            package.DateOfCreation = DateTime.Now;

            await _dbContext.Packages.AddAsync(package);
            await _dbContext.SaveChangesAsync();
            return package;
        }

        public async Task<Package> Update(Guid id, Package package)
        {
            Package? packageFromDb = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
            if (packageFromDb == null)
                throw new KeyNotFoundException($"There is no package with the id of {id}");

            if (packageFromDb.Status == PackageStatus.Delivered)
                throw new InvalidOperationException("You cannot update package details for a package that has already been delivered.");

            if (package.DateOfDelivery != null && package.DateOfDelivery < package.DateOfCreation)
                throw new ArgumentException("The package cannot be delivered before an order for it is placed.");

            packageFromDb.Name = package.Name;
            if(package.Status == PackageStatus.Delivered)
            {
                if(packageFromDb.DateOfDelivery == null || (packageFromDb.DateOfDelivery != null && packageFromDb.DateOfDelivery > DateTime.Now))
                {
                    throw new ArgumentException($"The package status cannot be set to {PackageStatus.Delivered} if the package hasn't been delivered yet.");
                }
                else
                {
                    packageFromDb.Status = PackageStatus.Delivered;
                }
            }
            else
            {
                packageFromDb.Status = package.Status;
            }
            packageFromDb.DateOfDelivery = package.DateOfDelivery;

            _dbContext.Packages.Update(packageFromDb);
            await _dbContext.SaveChangesAsync();
            return packageFromDb;
        }

        public async Task<Package> Delete(Guid id)
        {
            Package? packageFromDb = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
            if (packageFromDb == null)
                throw new KeyNotFoundException($"There is no package with the id of {id}");

            _dbContext.Packages.Remove(packageFromDb);
            await _dbContext.SaveChangesAsync();
            return packageFromDb;
        }
    }
}
