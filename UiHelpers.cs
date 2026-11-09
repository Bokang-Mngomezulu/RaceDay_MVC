using System.Globalization;

namespace RaceDay_MVC.Helpers
{
    /// <summary>Display helpers so every page formats dates, money, times and statuses the same way.</summary>
    public static class UiHelpers
    {
        public static string StatusClass(string? status) => (status ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "confirmed" => "rd-status rd-status-confirmed",
            "pending" => "rd-status rd-status-pending",
            "cancelled" or "canceled" => "rd-status rd-status-cancelled",
            _ => "rd-status"
        };

        public static string FormatDate(DateTime date) =>
            date.ToString("ddd, d MMM yyyy HH:mm", CultureInfo.InvariantCulture);

        public static string FormatMoney(decimal amount) =>
            "R" + amount.ToString("N2", CultureInfo.InvariantCulture);

        /// <summary>The API stores finish times in minutes (105.50 = 1:45:30).</summary>
        public static string FormatMinutes(decimal minutes)
        {
            var totalSeconds = (int)Math.Round((double)minutes * 60);
            var h = totalSeconds / 3600;
            var m = totalSeconds % 3600 / 60;
            var s = totalSeconds % 60;
            return $"{h}:{m:00}:{s:00}";
        }
    }
}
