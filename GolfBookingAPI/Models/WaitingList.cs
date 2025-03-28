namespace GolfBookingAPI.Models
{
    public class WaitingList
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int GolfCourseId { get; set; }
        public DateTime TeeTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
