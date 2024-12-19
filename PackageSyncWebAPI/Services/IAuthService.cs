using Microsoft.AspNetCore.Identity;
using PackageSync.Domain;

namespace PackageSyncWebAPI.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> Register(User user);

        public Task<string> Login(User user);
    }
}
