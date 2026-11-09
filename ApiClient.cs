using RaceDay_MVC.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RaceDay_MVC.Services
{
    /// <summary>
    /// The only class that talks to the RaceDay API. Controllers never use HttpClient directly.
    /// It is split over several files: this one holds the shared plumbing,
    /// the other files hold the endpoint methods.
    /// </summary>
    public partial class ApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient http, ILogger<ApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ---------- shared helpers ----------

        private Task<ApiResult<T>> GetAsync<T>(string url) =>
            SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, url));

        private Task<ApiResult<T>> DeleteAsync<T>(string url) =>
            SendAsync<T>(new HttpRequestMessage(HttpMethod.Delete, url));

        /// <summary>Sends form fields (the API shows these as textboxes in Swagger). Null values are left out.</summary>
        private Task<ApiResult<T>> SendFormAsync<T>(HttpMethod method, string url, IDictionary<string, string?> fields)
        {
            var pairs = fields
                .Where(f => f.Value != null)
                .Select(f => new KeyValuePair<string, string>(f.Key, f.Value!));

            return SendAsync<T>(new HttpRequestMessage(method, url) { Content = new FormUrlEncodedContent(pairs) });
        }

        /// <summary>Sends form fields plus an optional file (used for banner and profile picture uploads).</summary>
        private Task<ApiResult<T>> SendMultipartAsync<T>(HttpMethod method, string url,
            IDictionary<string, string?> fields, string? fileField = null, IFormFile? file = null)
        {
            var content = new MultipartFormDataContent();

            foreach (var field in fields.Where(f => f.Value != null))
                content.Add(new StringContent(field.Value!), field.Key);

            if (file != null && file.Length > 0 && fileField != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
                content.Add(fileContent, fileField, Path.GetFileName(file.FileName));
            }

            return SendAsync<T>(new HttpRequestMessage(method, url) { Content = content });
        }

        private async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request)
        {
            using (request)
            {
                try
                {
                    using var response = await _http.SendAsync(request);
                    var body = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        T? data = default;
                        if (!string.IsNullOrWhiteSpace(body))
                            data = JsonSerializer.Deserialize<T>(body, JsonOptions);

                        return new ApiResult<T> { Success = true, StatusCode = response.StatusCode, Data = data };
                    }

                    return new ApiResult<T>
                    {
                        Success = false,
                        StatusCode = response.StatusCode,
                        Message = ExtractMessage(response.StatusCode, body)
                    };
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
                {
                    _logger.LogWarning(ex, "API call failed: {Method} {Url}", request.Method, request.RequestUri);

                    return new ApiResult<T>
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.ServiceUnavailable,
                        Message = "We couldn't reach the RaceDay service. Please try again in a moment."
                    };
                }
            }
        }

        /// <summary>Turns an API error into a short, friendly sentence. Raw errors are never shown to users.</summary>
        private static string ExtractMessage(HttpStatusCode status, string body)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
                    {
                        var text = message.GetString();
                        if (!string.IsNullOrWhiteSpace(text)) return text;
                    }

                    if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var property in errors.EnumerateObject())
                        {
                            if (property.Value.ValueKind != JsonValueKind.Array) continue;

                            foreach (var item in property.Value.EnumerateArray())
                            {
                                var text = item.GetString();
                                if (!string.IsNullOrWhiteSpace(text)) return text;
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON: fall through to the generic messages below.
            }

            return (int)status switch
            {
                401 => "Please log in to continue.",
                403 => "You don't have permission to do that.",
                404 => "We couldn't find what you were looking for.",
                409 => "That conflicts with something that already exists.",
                >= 500 => "Something went wrong on our side. Please try again.",
                _ => "That request couldn't be completed. Please check your details and try again."
            };
        }
    }
}
