using System.Net;

namespace RaceDay_MVC.Models
{
    /// <summary>The outcome of an API call, with a friendly message if it failed.</summary>
    public class ApiResult
    {
        public bool Success { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;

        public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;
        public bool IsForbidden => StatusCode == HttpStatusCode.Forbidden;
        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; init; }
    }
}
