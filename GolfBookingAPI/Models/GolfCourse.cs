namespace Golf_Booking.Models
{
    public class GolfCourse
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Mark as required
        public int GolfClubId { get; set; } // Foreign Key
        public required GolfClub GolfClub { get; set; } // Navigation property
        public string Type { get; set; } = "Course";

    }
}