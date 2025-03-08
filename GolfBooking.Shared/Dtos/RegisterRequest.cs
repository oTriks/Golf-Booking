namespace GolfBooking.Shared.Dtos
{
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public int? GolfClubId { get; set; }
    }
}
