using BlogMVC.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlogMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // ==========================================================
        // GET
        // ==========================================================

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(endpoint);

            return await HandleResponse<T>(response);
        }

        // ==========================================================
        // POST
        // ==========================================================

        public async Task<T?> PostAsync<T>(
            string endpoint,
            object? data = null)
        {
            AddAuthorizationHeader();

            HttpResponseMessage response;

            if (data == null)
            {
                response = await _httpClient.PostAsync(
                    endpoint,
                    null);
            }
            else
            {
                response = await _httpClient.PostAsJsonAsync(
                    endpoint,
                    data);
            }

            return await HandleResponse<T>(response);
        }

        // ==========================================================
        // PUT
        // ==========================================================

        public async Task<T?> PutAsync<T>(
            string endpoint,
            object data)
        {
            AddAuthorizationHeader();

            var response =
                await _httpClient.PutAsJsonAsync(
                    endpoint,
                    data);

            return await HandleResponse<T>(response);
        }

        // ==========================================================
        // DELETE
        // ==========================================================

        public async Task<T?> DeleteAsync<T>(
            string endpoint)
        {
            AddAuthorizationHeader();

            var response =
                await _httpClient.DeleteAsync(endpoint);

            return await HandleResponse<T>(response);
        }

        // ==========================================================
        // DELETE WITHOUT RESPONSE
        // ==========================================================

        public async Task<bool> DeleteAsync(
            string endpoint)
        {
            AddAuthorizationHeader();

            var response =
                await _httpClient.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }

        // ==========================================================
        // AUTHORIZATION HEADER
        // ==========================================================

        private void AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var token =
                _httpContextAccessor
                    .HttpContext?
                    .Session
                    .GetString(SessionKeys.AccessToken);

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }
        }

        // ==========================================================
        // HANDLE API RESPONSE
        // ==========================================================

        private async Task<T?> HandleResponse<T>(
            HttpResponseMessage response)
        {
            var content =
                await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return default;
                }

                // String response
                if (typeof(T) == typeof(string))
                {
                    object result = content;

                    return (T)result;
                }

                try
                {
                    return JsonSerializer.Deserialize<T>(
                        content,
                        _jsonOptions);
                }
                catch
                {
                    return default;
                }
            }

            var errorMessage =
                ExtractErrorMessage(content);

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode);
        }

        // ==========================================================
        // ERROR MESSAGE
        // ==========================================================

        private string ExtractErrorMessage(
            string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "Something went wrong.";
            }

            try
            {
                using var document =
                    JsonDocument.Parse(content);

                var root =
                    document.RootElement;

                if (root.TryGetProperty(
                        "message",
                        out var message))
                {
                    return message.GetString()
                           ?? "Request failed.";
                }

                if (root.TryGetProperty(
                        "title",
                        out var title))
                {
                    return title.GetString()
                           ?? "Request failed.";
                }
            }
            catch
            {
                // Plain text response
            }

            return content;
        }
    }
}