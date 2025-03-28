namespace GolfBooking.Shared.Dtos
{
    public class ClubRead
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public string Type { get; set; } = "Club";

        public IEnumerable<CourseRead>? Courses { get; set; }
    }
}