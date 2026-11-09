using Microsoft.AspNetCore.Mvc;
using RaceDay_MVC.Helpers;
using RaceDay_MVC.Models;
using RaceDay_MVC.Services;

namespace RaceDay_MVC.Controllers
{
    [RoleAuthorize(UserSession.Organiser)]
    public class OrganiserController : BaseController
    {
        private readonly ApiClient _api;

        public OrganiserController(ApiClient api)
        {
            _api = api;
        }

        // ===================== DASHBOARD =====================

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _api.GetMyEventsAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;
            if (!result.Success) SetError(result.Message);

            var events = result.Data ?? new List<OrganiserEventDto>();
            var now = DateTime.Now;

            var model = new DashboardViewModel
            {
                TotalEvents = events.Count,
                TotalEnrolments = events.Sum(e => e.EnrolmentCount),
                Upcoming = events.Where(e => e.EventDate >= now).OrderBy(e => e.EventDate).ToList(),
                Past = events.Where(e => e.EventDate < now).OrderByDescending(e => e.EventDate).ToList()
            };

            return View(model);
        }

        // ===================== EVENTS =====================

        [HttpGet]
        public async Task<IActionResult> Events()
        {
            var result = await _api.GetMyEventsAsync();
            var expired = SessionExpired(result);
            if (expired != null) return expired;
            if (!result.Success) SetError(result.Message);

            return View(result.Data ?? new List<OrganiserEventDto>());
        }

        [HttpGet]
        public IActionResult CreateEvent() => View("EventForm", new EventFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventFormViewModel model)
        {
            ImageIsValid(model.Banner, nameof(model.Banner));
            if (!ModelState.IsValid) return View("EventForm", model);

            var result = await _api.CreateEventAsync(model);
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("EventForm", model);
            }

            SetSuccess("Event created. Now add the categories people can enter.");

            if (result.Data == null) return RedirectToAction(nameof(Events));
            return RedirectToAction(nameof(Categories), new { id = result.Data.EventId });
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var (ev, redirect) = await LoadEventAsync(id);
            if (redirect != null) return redirect;

            return View("EventForm", new EventFormViewModel
            {
                EventId = ev!.EventId,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance ?? string.Empty,
                SportType = ev.EventType,
                Information = ev.Information,
                LiveWeather = ev.LiveWeather,
                ExistingBannerUrl = ev.BannerUrl
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, EventFormViewModel model)
        {
            model.EventId = id;

            ImageIsValid(model.Banner, nameof(model.Banner));
            if (!ModelState.IsValid) return View("EventForm", model);

            var result = await _api.UpdateEventAsync(id, model);
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("EventForm", model);
            }

            SetSuccess("Event updated.");
            return RedirectToAction(nameof(Events));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _api.DeleteEventAsync(id);
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (result.Success) SetSuccess("Event deleted.");
            else SetError(result.Message);

            return RedirectToAction(nameof(Events));
        }

        // ===================== CATEGORIES =====================

        [HttpGet]
        public async Task<IActionResult> Categories(int id)
        {
            var (ev, redirect) = await LoadEventAsync(id);
            if (redirect != null) return redirect;

            return View(new CategoriesViewModel { Event = ev! });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(int id, [Bind(Prefix = "Form")] CategoryFormViewModel form)
        {
            if (string.IsNullOrWhiteSpace(form.AgeGroup) && string.IsNullOrWhiteSpace(form.Distance))
                ModelState.AddModelError(string.Empty, "Please enter an age group, a distance, or both.");

            if (ModelState.IsValid)
            {
                var result = await _api.AddCategoryAsync(id, form);
                var expired = SessionExpired(result);
                if (expired != null) return expired;

                if (result.Success)
                {
                    SetSuccess("Category added.");
                    return RedirectToAction(nameof(Categories), new { id });
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            var (ev, redirect) = await LoadEventAsync(id);
            if (redirect != null) return redirect;

            return View("Categories", new CategoriesViewModel { Event = ev!, Form = form });
        }

        // ===================== ENROLMENTS =====================

        [HttpGet]
        public async Task<IActionResult> Enrolments(int id)
        {
            var (ev, redirect) = await LoadEventAsync(id);
            if (redirect != null) return redirect;

            var result = await _api.GetEventEnrolmentsAsync(id);
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (!result.Success)
            {
                SetError(result.Message);
                return RedirectToAction(nameof(Events));
            }

            return View(new EnrolmentsViewModel { Event = ev!, Enrolments = result.Data ?? new List<EventEnrolmentDto>() });
        }

        // ===================== RESULTS =====================

        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var (ev, redirect) = await LoadEventAsync(id);
            if (redirect != null) return redirect;

            var enrolments = await _api.GetEventEnrolmentsAsync(id);
            var expired = SessionExpired(enrolments);
            if (expired != null) return expired;

            if (!enrolments.Success)
            {
                SetError(enrolments.Message);
                return RedirectToAction(nameof(Events));
            }

            var published = await _api.GetEventResultsAsync(id);

            return View(new ResultsViewModel
            {
                Event = ev!,
                Concluded = ev!.EventDate <= DateTime.Now,
                Pending = (enrolments.Data ?? new List<EventEnrolmentDto>()).Where(e => !e.HasResult).ToList(),
                Published = published.Data ?? new List<EventResultDto>()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CaptureResult(ResultFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));

                SetError(message ?? "Please check the finish time and position.");
                return RedirectToAction(nameof(Results), new { id = form.EventId });
            }

            var result = await _api.CaptureResultAsync(form);
            var expired = SessionExpired(result);
            if (expired != null) return expired;

            if (result.Success) SetSuccess("Result saved and published.");
            else SetError(result.Message);

            return RedirectToAction(nameof(Results), new { id = form.EventId });
        }

        // ===================== helpers =====================

        private async Task<(EventDetailDto? Event, IActionResult? Redirect)> LoadEventAsync(int id)
        {
            var result = await _api.GetEventAsync(id);

            var expired = SessionExpired(result);
            if (expired != null) return (null, expired);

            if (!result.Success || result.Data == null)
            {
                SetError(result.IsNotFound ? "That event could not be found." : result.Message);
                return (null, RedirectToAction(nameof(Events)));
            }

            return (result.Data, null);
        }
    }
}
