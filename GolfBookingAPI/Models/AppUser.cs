namespace GolfBookingAPI.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

        public int? GolfClubId { get; set; }
        public GolfClub? GolfClub { get; set; }
    }
}
