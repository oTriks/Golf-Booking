using System.ComponentModel;

namespace Golf_Booking.Dtos
{
    public class ClubCreate
    {
        public required string Name { get; set; }
        public required string Location { get; set; }

        [DefaultValue("Club")]
        public string Type { get; set; } = "Club";
    }
}
