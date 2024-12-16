using PackageSync.Domain;

namespace PackageSyncWebAPI.Services
{
    public interface IPackageService
    {
        public Task<IEnumerable<Package>> GetAll();

        public Task<Package> GetById(Guid id);

        public Task<Package> Add(Package package);

        public Task<Package> Update(Guid id, Package package);

        public Task<Package> Delete(Guid id);
    }
}
