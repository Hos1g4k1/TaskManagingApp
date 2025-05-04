using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorWebApp.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}