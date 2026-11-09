using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Helpers;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly ApiClient _api;

        public ProfileController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var result = await _api.GetProfileAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success || result.Data == null)
            {
                SetError(result.Message ?? "Could not load your profile.");
                return RedirectToAction("Index", "Home");
            }

            return View(result.Data);
        }
    }
}
