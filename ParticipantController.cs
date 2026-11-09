using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Helpers;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    [RoleAuthorize(UserSession.Participant)]
    public class ParticipantController : BaseController
    {
        private readonly ApiClient _api;

        public ParticipantController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return RedirectToAction(nameof(Events)); // Route dashboard directly to events or enrolments
        }

        // --- NEW: Participant Events Tab ---
        [HttpGet]
        public async Task<IActionResult> Events()
        {
            var result = await _api.GetPublicEventsAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success) SetError(result.Message);

            return View(result.Data ?? new List<PublicEventDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Enrolments()
        {
            var result = await _api.GetMyEnrolmentsAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success) SetError(result.Message);

            return View(result.Data ?? new List<MyEnrolmentDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Results()
        {
            var result = await _api.GetMyResultsAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success) SetError(result.Message);

            return View(result.Data ?? new List<MyResultDto>());
        }
    }
}
