
namespace GolfBookingUI.Models
{
    public class ClubReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string Type { get; set; } = "Club";
        // You can add a Courses property if you plan to display courses
    }
}
