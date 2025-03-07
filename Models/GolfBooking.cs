namespace Golf_Booking.Models
{
    public class GolfBooking
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Mark as required
        public DateTime TeeTime { get; set; }
        public int GolfCourseId { get; set; }
        public required GolfCourse GolfCourse { get; set; } // Mark as required
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}