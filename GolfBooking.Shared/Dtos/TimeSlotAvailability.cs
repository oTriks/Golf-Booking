namespace GolfBooking.Shared.Dtos
{
    public class TimeSlotAvailability
    {
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
    }
}
