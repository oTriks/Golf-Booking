namespace GolfBookingAPI.Models
{
    public class GolfCourse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int GolfClubId { get; set; }
        public required GolfClub GolfClub { get; set; }
        public string Type { get; set; } = "Course";

    }
}