using Microsoft.AspNetCore.Identity;

namespace PackageSyncWebAPI.Infrastructure
{
    public class UserSeeder
    {
        public static async Task SeedUsers(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = new IdentityUser()
                {
                    Id = "1",
                    UserName = "Admin"
                };
                await userManager.CreateAsync(user, "Admin123!");
            }
        }
    }
}
