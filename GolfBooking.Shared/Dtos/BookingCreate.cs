namespace GolfBooking.Shared.Dtos
{
    public class BookingCreate
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime TeeTime { get; set; }
        public int GolfCourseId { get; set; }
    }
}
