using Golf_Booking.Dtos;

namespace Golf_Booking.Dtos
{
    public class ClubRead
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public string Type { get; set; } = "Club";  // Include the type for the response.

        // Include courses using a read DTO to avoid cycles
        public IEnumerable<CourseRead>? Courses { get; set; }
    }
}