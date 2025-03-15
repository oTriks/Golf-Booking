namespace GolfBookingAPI.Models
{
    public class GolfClub
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }

        public string Type { get; set; } = "Club";

        public ICollection<GolfCourse> Courses { get; set; } = new List<GolfCourse>();
    }
}
