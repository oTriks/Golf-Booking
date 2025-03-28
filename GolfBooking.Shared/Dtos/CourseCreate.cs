using System.ComponentModel;

namespace GolfBooking.Shared.Dtos
{
    public class CourseCreate
    {
        public required string Name { get; set; } = string.Empty;
        public required int GolfClubId { get; set; } = 0;
        [DefaultValue("Course")]
        public string Type { get; set; } = "Course";
    }
}
