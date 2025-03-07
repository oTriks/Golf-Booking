using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Golf_Booking.Models;
using Golf_Booking.Dtos;


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
    public async Task<ActionResult<IEnumerable<GolfBooking>>> GetBookings()
    {
        return await _context.GolfBookings.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GolfBooking>> GetBooking(int id)
    {
        var booking = await _context.GolfBookings.FindAsync(id);
        if (booking == null) return NotFound();
        return booking;
    }

    [HttpPost]
    public async Task<ActionResult<GolfBooking>> CreateBooking(GolfBooking booking)
    {
        var course = await _context.GolfCourses.FindAsync(booking.GolfCourseId);
        if (course == null) return BadRequest("Invalid Course ID");

        _context.GolfBookings.Add(booking);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, GolfBooking booking)
    {
        if (id != booking.Id) return BadRequest();
        _context.Entry(booking).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.GolfBookings.FindAsync(id);
        if (booking == null)
            return NotFound("Booking not found.");

        // 1) Get the current user's role from the JWT
        var userRole = User.FindFirst("role")?.Value;

        // Allow Admin or Personal to delete any booking
        if (userRole == "Admin" || userRole == "Personal")
        {
            _context.GolfBookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 2) If the user is a Player, only allow if they own the booking
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return Forbid("User identifier not found.");
        }
        var currentUsername = claim.Value;
        // Ensure that booking.UserId (the stored username) matches the current user's username
        if (userRole == "Player" && booking.UserId == currentUsername)
        {
            _context.GolfBookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 3) Otherwise, not authorized
        return Forbid("You are not authorized to delete this booking.");
    }

}
