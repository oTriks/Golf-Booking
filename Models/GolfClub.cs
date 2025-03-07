namespace Golf_Booking.Models
{
    public class GolfClub
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }

        // New property for type with default value "Club"
        public string Type { get; set; } = "Club";

        public ICollection<GolfCourse> Courses { get; set; } = new List<GolfCourse>();
    }
}
