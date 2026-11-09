namespace RaceDay_MVC.Helpers
{
    public static class UserSession
    {
        public const string Organiser = "Organiser";
        public const string Participant = "Participant";
    }

    /// <summary>Reads and writes the logged-in user's details in the MVC session.</summary>
    public static class UserSessionExtensions
    {
        public static bool IsLoggedIn(this ISession s) => s.GetInt32("UserID") != null;

        public static int? UserId(this ISession s) => s.GetInt32("UserID");

        public static string? Role(this ISession s) => s.GetString("Role");

        public static string? FullName(this ISession s) => s.GetString("FullName");

        public static string? ProfilePictureUrl(this ISession s) => s.GetString("ProfilePictureUrl");

        public static bool IsOrganiser(this ISession s) =>
            string.Equals(s.Role(), UserSession.Organiser, StringComparison.OrdinalIgnoreCase);

        public static bool IsParticipant(this ISession s) =>
            string.Equals(s.Role(), UserSession.Participant, StringComparison.OrdinalIgnoreCase);

        public static void SignIn(this ISession s, int userId, string role, string fullName, string? pictureUrl)
        {
            s.SetInt32("UserID", userId);
            s.SetString("Role", role);
            s.SetString("FullName", fullName);

            if (pictureUrl != null) s.SetString("ProfilePictureUrl", pictureUrl);
            else s.Remove("ProfilePictureUrl");
        }
    }
}
