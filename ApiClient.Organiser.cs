using System.Globalization;
using RaceDay_MVC.Models;

namespace RaceDay_MVC.Services
{
    public partial class ApiClient
    {
        public Task<ApiResult<List<OrganiserEventDto>>> GetMyEventsAsync() =>
            GetAsync<List<OrganiserEventDto>>("api/events/mine");

        public Task<ApiResult<EventDetailDto>> GetEventAsync(int id) =>
            GetAsync<EventDetailDto>($"api/events/{id}");

        public Task<ApiResult<List<EventEnrolmentDto>>> GetEventEnrolmentsAsync(int eventId) =>
            GetAsync<List<EventEnrolmentDto>>($"api/events/{eventId}/enrolments");

        public Task<ApiResult<List<EventResultDto>>> GetEventResultsAsync(int eventId) =>
            GetAsync<List<EventResultDto>>($"api/events/{eventId}/results");

        public Task<ApiResult<EventCreatedDto>> CreateEventAsync(EventFormViewModel model) =>
            SendMultipartAsync<EventCreatedDto>(HttpMethod.Post, "api/events", EventFields(model), "Banner", model.Banner);

        public Task<ApiResult<MessageDto>> UpdateEventAsync(int id, EventFormViewModel model) =>
            SendMultipartAsync<MessageDto>(HttpMethod.Put, $"api/events/{id}", EventFields(model), "Banner", model.Banner);

        public Task<ApiResult<MessageDto>> DeleteEventAsync(int id) =>
            DeleteAsync<MessageDto>($"api/events/{id}");

        public Task<ApiResult<EventCategoryCreatedDto>> AddCategoryAsync(int eventId, CategoryFormViewModel model) =>
            SendFormAsync<EventCategoryCreatedDto>(HttpMethod.Post, $"api/events/{eventId}/categories",
                new Dictionary<string, string?>
                {
                    ["CategoryName"] = model.CategoryName.Trim(),
                    ["AgeGroup"] = Blank(model.AgeGroup),
                    ["Distance"] = Blank(model.Distance),
                    ["EntryFee"] = model.EntryFee?.ToString(CultureInfo.InvariantCulture),
                    ["MaxCapacity"] = model.MaxCapacity?.ToString(CultureInfo.InvariantCulture)
                });

        public Task<ApiResult<ResultCreatedDto>> CaptureResultAsync(ResultFormViewModel model) =>
            SendFormAsync<ResultCreatedDto>(HttpMethod.Post, "api/results", new Dictionary<string, string?>
            {
                ["EnrolmentID"] = model.EnrolmentId.ToString(CultureInfo.InvariantCulture),
                ["Time"] = model.Time?.ToString(CultureInfo.InvariantCulture),
                ["OverallPosition"] = model.OverallPosition?.ToString(CultureInfo.InvariantCulture)
            });

        // ---------- helpers ----------

        private static string? Blank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static Dictionary<string, string?> EventFields(EventFormViewModel m) => new()
        {
            ["Name"] = m.Name.Trim(),
            ["Description"] = Blank(m.Description),
            ["EventDate"] = m.EventDate?.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture),
            ["Location"] = m.Location.Trim(),
            ["Distance"] = m.Distance.Trim(),
            ["SportType"] = m.SportType,
            ["Information"] = Blank(m.Information),
            ["LiveWeather"] = Blank(m.LiveWeather)
        };
    }
}
