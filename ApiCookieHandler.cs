namespace RaceDay_MVC.Helpers
{
    /// <summary>
    /// Carries the API's session cookie for each MVC user: saves it from login responses
    /// and attaches it to every later API request made for that user.
    /// </summary>
    public class ApiCookieHandler : DelegatingHandler
    {
        private const string SessionKey = "ApiCookie";
        private const string ApiCookieName = ".AspNetCore.Session=";

        private readonly IHttpContextAccessor _accessor;

        public ApiCookieHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var session = _accessor.HttpContext?.Session;

            var cookie = session?.GetString(SessionKey);
            if (!string.IsNullOrEmpty(cookie))
                request.Headers.TryAddWithoutValidation("Cookie", cookie);

            var response = await base.SendAsync(request, cancellationToken);

            if (session != null && response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                foreach (var header in setCookies)
                {
                    var pair = header.Split(';')[0];
                    if (pair.StartsWith(ApiCookieName, StringComparison.Ordinal))
                        session.SetString(SessionKey, pair);
                }
            }

            return response;
        }
    }
}
