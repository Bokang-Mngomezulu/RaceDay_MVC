using RaceDay_MVC.Models;

namespace RaceDay_MVC.Services
{
    public partial class ApiClient
    {
        public Task<ApiResult<RegisterResultDto>> RegisterAsync(RegisterViewModel model) =>
            SendFormAsync<RegisterResultDto>(HttpMethod.Post, "api/auth/register", new Dictionary<string, string?>
            {
                ["FullName"] = model.FullName.Trim(),
                ["Email"] = model.Email.Trim(),
                ["Password"] = model.Password,
                ["RoleName"] = model.RoleName,
                ["Phone"] = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim()
            });

        public Task<ApiResult<LoginResultDto>> LoginAsync(LoginViewModel model) =>
            SendFormAsync<LoginResultDto>(HttpMethod.Post, "api/auth/login", new Dictionary<string, string?>
            {
                ["Email"] = model.Email.Trim(),
                ["Password"] = model.Password
            });

        public Task<ApiResult<MessageDto>> LogoutAsync() =>
            SendFormAsync<MessageDto>(HttpMethod.Post, "api/auth/logout", new Dictionary<string, string?>());

        public Task<ApiResult<ProfileDto>> GetProfileAsync() =>
            GetAsync<ProfileDto>("api/profile");
    }
}
