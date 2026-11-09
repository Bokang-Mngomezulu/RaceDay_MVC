using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Helpers;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RaceDay_MVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApiClient _api;

        public AccountController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.IsLoggedIn()) return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _api.RegisterAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            SetSuccess("Your account has been created. Please log in.");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.IsLoggedIn()) return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            /// Start from a clean session so no old API cookie or user details linger.
            HttpContext.Session.Clear();

            var login = await _api.LoginAsync(model);
            if (!login.Success)
            {
                ModelState.AddModelError(string.Empty,
                    login.IsUnauthorized ? "Invalid email or password." : login.Message);
                return View(model);
            }

            var profile = await _api.GetProfileAsync();
            if (!profile.Success || profile.Data == null)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError(string.Empty, "We couldn't load your account. Please try again.");
                return View(model);
            }

            HttpContext.Session.SignIn(profile.Data.UserId, profile.Data.Role,
                profile.Data.FullName, profile.Data.ProfilePictureUrl);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            
            /// Role-based post-login redirection
           
            if (string.Equals(profile.Data.Role, UserSession.Participant, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Enrolments", "Participant");
            }
            else if (string.Equals(profile.Data.Role, UserSession.Organiser, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _api.LogoutAsync();            
            HttpContext.Session.Clear();

            SetSuccess("You have been logged out.");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
