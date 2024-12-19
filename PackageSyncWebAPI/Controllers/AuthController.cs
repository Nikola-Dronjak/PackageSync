using Microsoft.AspNetCore.Mvc;
using PackageSync.Domain;
using PackageSyncWebAPI.Services;

namespace PackageSyncWebAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to create a new account by providing the necessary details in the request body.
        /// </remarks>
        /// <param name="user">The username and password of the account.</param>
        /// <response code="200">User registered successfully. Returns a success message.</response>
        /// <response code="400">Validation failed. Returns a list of validation errors.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpPost("/api/register")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            try
            {
                var result = await _authService.Register(user);
                if (result.Succeeded)
                {
                    return Ok(new { message = "User registered successfully." });
                }

                return BadRequest(new
                {
                    title = "Invalid request",
                    details = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Signs in an existing user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to sign in by providing the necessary details in the request body.
        /// </remarks>
        /// <param name="user">The username and password of the account.</param>
        /// <response code="200">Login successful. Returns an authentication token.</response>
        /// <response code="401">Unauthorized. Returns an error message.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpPost("/api/login")]
        public async Task<IActionResult> LoginUser(User user)
        {
            try
            {
                var token = await _authService.Login(user);
                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                return Ok(new
                {
                    token = token,
                    message = "Login successful."
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }
    }
}
