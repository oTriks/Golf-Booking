using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GolfBookingAPI.Models;
using GolfBooking.Shared.Dtos;

[ApiController]
[Route("api/[controller]")]
public class GolfBookingController : ControllerBase
{
    private readonly BookingContext _context;

    public GolfBookingController(BookingContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingRead>>> GetBookings()
    {
        var bookings = await _context.Bookings
            .Include(b => b.GolfCourse)
            .Select(b => new BookingRead
            {
                Id = b.Id,
                UserId = b.UserId,
                TeeTime = b.TeeTime,
                GolfCourseName = b.GolfCourse.Name,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return bookings;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingRead>> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.GolfCourse)
            .Where(b => b.Id == id)
            .Select(b => new BookingRead
            {
                Id = b.Id,
                UserId = b.UserId,
                TeeTime = b.TeeTime,
                GolfCourseName = b.GolfCourse.Name,
                CreatedAt = b.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (booking == null)
            return NotFound();

        return booking;
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingCreate bookingDto)
    {
        var course = await _context.GolfCourses.FindAsync(bookingDto.GolfCourseId);
        if (course == null)
            return BadRequest("Invalid Course ID");

        DateTime teeTimeUtc = bookingDto.TeeTime.ToUniversalTime();

        bool hasExistingBooking = await _context.Bookings.AnyAsync(b =>
            b.GolfCourseId == bookingDto.GolfCourseId &&
            b.TeeTime == teeTimeUtc &&
            b.UserId == bookingDto.UserId);

        if (hasExistingBooking)
            return BadRequest("You already booked this tee time.");

        int existingCount = await _context.Bookings
            .CountAsync(b => b.GolfCourseId == bookingDto.GolfCourseId && b.TeeTime == teeTimeUtc);
        if (existingCount >= 4)
            return BadRequest("This time slot is fully booked.");

        var booking = new Booking
        {
            UserId = bookingDto.UserId,
            TeeTime = teeTimeUtc,
            GolfCourseId = bookingDto.GolfCourseId,
            GolfCourse = course,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, Booking booking)
    {
        if (id != booking.Id)
            return BadRequest();

        _context.Entry(booking).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound("Booking not found.");

        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        var nameClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameClaim))
        {
            return Unauthorized("User identifier not found.");
        }

        if (roleClaim == "Admin" || roleClaim == "Personal" ||
            (roleClaim == "Player" && string.Equals(booking.UserId, nameClaim, System.StringComparison.OrdinalIgnoreCase)))
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            var waitingEntry = await _context.WaitingLists
                .Where(w => w.GolfCourseId == booking.GolfCourseId && w.TeeTime == booking.TeeTime)
                .OrderBy(w => w.CreatedAt)
                .FirstOrDefaultAsync();

            if (waitingEntry != null)
            {
                var course = await _context.GolfCourses.FindAsync(booking.GolfCourseId);

                var newBooking = new Booking
                {
                    UserId = waitingEntry.UserId,
                    TeeTime = waitingEntry.TeeTime,
                    GolfCourseId = waitingEntry.GolfCourseId,
                    GolfCourse = course,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(newBooking);
                _context.WaitingLists.Remove(waitingEntry);

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to delete this booking.");
    }

    [HttpGet("availability/{courseId}/{teeTime}")]
    public async Task<ActionResult<TimeSlotAvailability>> GetAvailabilityForSlot(int courseId, DateTime teeTime)
    {

        int count = await _context.Bookings
            .CountAsync(b => b.GolfCourseId == courseId && b.TeeTime == teeTime);

        var availability = new TimeSlotAvailability
        {
            Date = teeTime.Date,
            TimeSlot = teeTime.ToString("HH:mm"),
            AvailableSlots = Math.Max(0, 4 - count)
        };

        return availability;
    }

    [HttpGet("availability/{courseId}")]
    public async Task<ActionResult<IEnumerable<TimeSlotAvailability>>> GetAvailability(int courseId)
    {
        var course = await _context.GolfCourses.FindAsync(courseId);
        if (course == null)
            return NotFound($"Course with ID {courseId} not found.");

        var availabilityList = new List<TimeSlotAvailability>();

        var timeSlots = new[] { "09:00", "11:00", "13:00", "15:00" };

        var today = DateTime.Today;
        for (int i = 0; i <= 7; i++)
        {
            var date = today.AddDays(i);
            foreach (var slot in timeSlots)
            {
                var parsed = TimeSpan.Parse(slot);
                var slotDateTime = date.Add(parsed);

                int count = await _context.Bookings
                    .CountAsync(b => b.GolfCourseId == courseId && b.TeeTime == slotDateTime);

                availabilityList.Add(new TimeSlotAvailability
                {
                    Date = date,
                    TimeSlot = slot,
                    AvailableSlots = Math.Max(0, 4 - count)
                });
            }
        }

        return availabilityList;
    }
}
