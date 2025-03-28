namespace GolfBooking.Shared.Dtos
{
    public class BookingRead
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime TeeTime { get; set; }
        public string GolfCourseName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
