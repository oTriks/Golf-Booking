namespace GolfBooking.Shared.Dtos
{
    public class CourseUpdate
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int GolfClubId { get; set; }
    }
}
