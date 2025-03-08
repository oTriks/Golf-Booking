using System.ComponentModel;

namespace GolfBooking.Shared.Dtos
{
    public class CourseCreate
    {
        public required string Name { get; set; }
        public required int GolfClubId { get; set; }
        [DefaultValue("Course")]

        public string Type { get; set; } = "Course";

    }
}