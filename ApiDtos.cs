namespace RaceDay_MVC.Models
{
    // Shapes of the JSON the API returns.
    public record MessageDto(string? Message);
    public record RegisterResultDto(int UserId, string Email, string Role);
    public record LoginResultDto(string Token, string Role, DateTime Date);
    public record ProfileDto(int UserId, string FullName, string Email, string? Phone, string Role, string? ProfilePictureUrl);

    public record OrganiserEventDto(int EventId, string Name, string? Description, DateTime EventDate,
        string Location, string? Distance, string EventType, string? BannerUrl, int EnrolmentCount);

    public record EventCategoryDto(int EventCategoryId, string CategoryName, string? AgeGroup, string? Distance,
        decimal EntryFee, int MaxCapacity);

    public record EventDetailDto(int EventId, string Name, string? Description, DateTime EventDate,
        string Location, string? Distance, string EventType, string? Information, string? LiveWeather,
        string? BannerUrl, List<EventCategoryDto> Categories);

    public record EventCreatedDto(int EventId, string Name, string? BannerUrl);
    public record EventCategoryCreatedDto(int EventCategoryId, string CategoryName);

    public record EventEnrolmentDto(int EnrolmentId, string ParticipantName, string Email, string CategoryName,
        string Status, bool HasResult);

    public record ResultCreatedDto(int ResultId, int EnrolmentId);
    public record EventResultDto(int? OverallPosition, string ParticipantName, decimal Time);

    public record MyEnrolmentDto(int EnrolmentId, string EventTitle, string CategoryName, string Status);
    public record MyResultDto(string EventTitle, string CategoryName, decimal Time, int? OverallPosition);
    public record EnrolmentFormModel(int EventCategoryId);
    public record PublicEventDto(int EventId, string Title, string? Description, DateTime Date, string Location, string Distance, string EventType);
}