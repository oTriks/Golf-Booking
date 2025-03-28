namespace GolfBooking.Shared.Dtos
{
    public class UserRead
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
    }
}
