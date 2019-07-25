using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TimeTracker.Client.Security;

namespace TimeTracker.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenAuthenticationStateProvider _authStateProvider;

        public ApiService(HttpClient httpClient, TokenAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<T> GetAsync<T>(string url, string token = null)
        {
            var response = await SendAuthorizedRequest<T>(HttpMethod.Get, url, default, token);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<T>(
                responseBytes, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        public async Task<bool> CreateAsync<T>(string url, T inputModel)
        {
            var response = await SendAuthorizedRequest(HttpMethod.Post, url, inputModel);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync<T>(string url, T inputModel)
        {
            var response = await SendAuthorizedRequest(HttpMethod.Put, url, inputModel);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var response = await SendAuthorizedRequest<object>(HttpMethod.Delete, url);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> SendAuthorizedRequest<T>(
            HttpMethod method, string url, T content = default, string token = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                token = await _authStateProvider.GetTokenAsync();
            }

            var request = new HttpRequestMessage(method, $"{Config.ApiResourceUrl}{url}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (content != null)
            {
                var json = JsonSerializer.Serialize<object>(
                    content, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await _httpClient.SendAsync(request);
        }
    }
}
