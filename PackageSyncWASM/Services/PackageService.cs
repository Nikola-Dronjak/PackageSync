using Blazored.LocalStorage;
using PackageSync.Domain;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PackageSyncWASM.Services
{
    public class PackageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public PackageService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<List<Package>> GetAll()
        {
            var response = await _httpClient.GetAsync("/api/packages");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<Package>>(response.Content.ReadAsStringAsync().Result, options);
        }

        public async Task<Package> GetById(Guid id)
        {
            var response = await _httpClient.GetAsync($"/api/packages/{id}");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Package>(response.Content.ReadAsStringAsync().Result, options);
        }

        public async Task<string> Add(Package package)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/packages", package);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return "Invalid input(s).";
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Whoops, something went wrong.";
            }

            return "The package was created successfully.";
        }

        public async Task<string> Update(Guid id, Package package)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/packages/{id}", package);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                try
                {
                    using var jsonDocument = JsonDocument.Parse(errorMessage);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("title", out var titleElement))
                    {
                        var title = titleElement.GetString();

                        if (title == "Validation error")
                        {
                            return "Invalid input(s).";
                        }
                        else if ((title == "Invalid operation" || title == "Invalid request") && root.TryGetProperty("details", out var detailsMessageElement) && detailsMessageElement.ValueKind == JsonValueKind.String)
                        {
                            var message = detailsMessageElement.GetString();
                            return message;
                        }
                        else
                        {
                            return "An unknown error occurred.";
                        }
                    }
                    else
                    {
                        return "An unknown error occurred.";
                    }
                }
                catch (JsonException)
                {
                    return "An error occurred while parsing the response.";
                }
            }
            
            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return "There is no package with the given id.";
            }


            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Whoops, something went wrong.";
            }

            return "The package was updated successfully.";
        }

        public async Task<string> Delete(Guid id)
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/packages/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "There is no package with the given id.";
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Whoops, something went wrong.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = null;
            return "The package was removed successfully.";
        }
    }
}
