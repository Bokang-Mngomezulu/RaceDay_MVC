using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Helpers;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ApiClient _api;

        public HomeController(ApiClient api)
        {
            _api = api;
        }

        public IActionResult Index() => View();

        public IActionResult Error(int? code)
        {
            var status = code ?? 500;
            Response.StatusCode = status;
            ViewBag.Code = status;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EventDetails(int id)
        {
            var result = await _api.GetEventAsync(id);
            if (!result.Success || result.Data == null)
            {
                SetError("We couldn't find that event.");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // --- NEW: Handle participant enrolment into a category ---
        [HttpPost, ValidateAntiForgeryToken]
        [RoleAuthorize(UserSession.Participant)]
        public async Task<IActionResult> Enrol(int eventId, int eventCategoryId)
        {
            var result = await _api.EnrolAsync(eventCategoryId);
            if (!result.Success)
            {
                SetError(result.Message);
            }
            else
            {
                SetSuccess("You have successfully enrolled in this event!");
            }

            return RedirectToAction(nameof(EventDetails), new { id = eventId });
        }
    }
}

