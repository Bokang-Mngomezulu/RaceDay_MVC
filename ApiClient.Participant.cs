using RaceDay_MVC.Models;
using System.Globalization;

namespace RaceDay_MVC.Services
{
    public partial class ApiClient
    {
        public Task<ApiResult<List<MyEnrolmentDto>>> GetMyEnrolmentsAsync() =>
            GetAsync<List<MyEnrolmentDto>>("api/enrolments/my-enrolments");

        public Task<ApiResult<List<MyResultDto>>> GetMyResultsAsync() =>
            GetAsync<List<MyResultDto>>("api/results/my-results");

        public Task<ApiResult<List<PublicEventDto>>> GetPublicEventsAsync() =>
            GetAsync<List<PublicEventDto>>("api/events");

        public Task<ApiResult<MessageDto>> EnrolAsync(int eventCategoryId) =>
            SendFormAsync<MessageDto>(HttpMethod.Post, "api/enrolments", new Dictionary<string, string?>
            {
                ["EventCategoryID"] = eventCategoryId.ToString(CultureInfo.InvariantCulture)
            });
    }
}