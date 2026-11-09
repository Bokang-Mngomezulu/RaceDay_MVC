using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Models;

namespace RaceDay_MVC.Controllers
{
    /// <summary>Shared helpers for friendly messages, expired sessions and image checks.</summary>
    public abstract class BaseController : Controller
    {
        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        protected void SetSuccess(string message) => TempData["Success"] = message;

        protected void SetError(string message) => TempData["Error"] = message;

        /// <summary>
        /// If the API says the user is no longer logged in (for example the API restarted),
        /// clear the MVC session and send them to the login page. Otherwise returns null.
        /// </summary>
        protected IActionResult? SessionExpired(ApiResult result)
        {
            if (!result.IsUnauthorized) return null;

            HttpContext.Session.Clear();
            SetError("Your session has expired. Please log in again.");
            return RedirectToAction("Login", "Account");
        }

        /// <summary>Checks an optional image before it is sent to the API. Adds a friendly error if it is not valid.</summary>
        protected bool ImageIsValid(IFormFile? file, string fieldName)
        {
            if (file == null || file.Length == 0) return true;

            if (file.Length > MaxImageBytes)
            {
                ModelState.AddModelError(fieldName, "The image must be 5 MB or smaller.");
                return false;
            }

            if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                ModelState.AddModelError(fieldName, "Only JPG, PNG, WEBP or GIF images are allowed.");
                return false;
            }

            return true;
        }
    }
}