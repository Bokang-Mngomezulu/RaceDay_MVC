using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay_MVC.Helpers
{
    /// <summary>
    /// Protects a controller or action. Not logged in: sent to the login page.
    /// Wrong role: sent to the access-denied page. Use [RoleAuthorize] for "any logged-in user",
    /// or [RoleAuthorize(UserSession.Organiser)] / [RoleAuthorize(UserSession.Participant)].
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? _role;

        public RoleAuthorizeAttribute(string? role = null)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;

            if (!session.IsLoggedIn())
            {
                context.Result = new RedirectToActionResult("Login", "Account",
                    new { returnUrl = context.HttpContext.Request.GetEncodedPathAndQuery() });
                return;
            }

            if (_role != null && !string.Equals(session.Role(), _role, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
