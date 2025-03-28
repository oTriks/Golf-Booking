namespace GolfBooking.Shared.Dtos
{
    public class WaitingListCreate
    {
        public string UserId { get; set; } = string.Empty;
        public int GolfCourseId { get; set; }
        public DateTime TeeTime { get; set; }
    }
}