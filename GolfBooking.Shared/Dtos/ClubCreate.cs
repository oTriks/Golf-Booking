using System.ComponentModel;

namespace GolfBooking.Shared.Dtos
{
    public class ClubCreate
    {
        public required string Name { get; set; }
        public required string Location { get; set; }

        [DefaultValue("Club")]
        public string Type { get; set; } = "Club";
    }
}
