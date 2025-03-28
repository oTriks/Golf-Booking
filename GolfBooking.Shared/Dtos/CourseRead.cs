namespace GolfBooking.Shared.Dtos
{
    public class CourseRead
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int GolfClubId { get; set; }
        public string? GolfClubName { get; set; }
        public string Type { get; set; } = "Course";

    }
}