using PackageSync.Domain;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Blazored.LocalStorage;

namespace PackageSyncWASM.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        public bool isLoggedIn;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<string> Register(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/register", user);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return "Invalid input(s).";
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Whoops, something went wrong.";
            }

            return "User registered successfully.";
        }

        public async Task<string> Login(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/login", user);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Invalid username or password.";
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Whoops, something went wrong.";
            }

            try
            {
                using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = jsonDocument.RootElement;
                var token = root.GetProperty("token").GetString();
                await _localStorageService.SetItemAsync("authToken", token);
                isLoggedIn = true;
                return "Login successful.";
            }
            catch (JsonException)
            {
                return "An error occurred while parsing the response.";
            }
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            isLoggedIn = false;
        }

        public async Task CheckLoginStatus()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            isLoggedIn = !string.IsNullOrEmpty(token);
        }
    }
}
