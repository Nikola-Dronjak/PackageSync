using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PackageSync.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PackageSyncWebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> Register(User user)
        {
            IdentityUser newUser = new IdentityUser
            {
                UserName = user.Username,
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            return result;
        }

        public async Task<string> Login(User user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return null;
            }

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(jwt);
        }
    }
}