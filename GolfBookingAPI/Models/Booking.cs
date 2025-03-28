namespace GolfBookingAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public DateTime TeeTime { get; set; }
        public int GolfCourseId { get; set; }
        public required GolfCourse GolfCourse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}