using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Golf_Booking.Models;
// using Golf_Booking.Dtos;
using GolfBooking.Shared.Dtos;

[ApiController]
[Route("api/[controller]")]
public class GolfClubController : ControllerBase
{
    private readonly BookingContext _context;

    public GolfClubController(BookingContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClubRead>>> GetClubs()
    {
        var clubs = await _context.GolfClubs.Include(c => c.Courses).ToListAsync();

        var clubDtos = clubs.Select(c => new ClubRead
        {
            Id = c.Id,
            Name = c.Name,
            Location = c.Location,
            Type = c.Type,
            Courses = c.Courses.Select(course => new CourseRead
            {
                Id = course.Id,
                Name = course.Name,
                GolfClubId = course.GolfClubId,
                GolfClubName = c.Name,
                Type = course.Type
            }).ToList()
        }).ToList();

        return clubDtos;
    }



    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GolfClub>> CreateClub([FromBody] ClubCreate clubDto)
    {
        var club = new GolfClub
        {
            Name = clubDto.Name,
            Location = clubDto.Location,
            Type = clubDto.Type  // This will be "Club" by default if not provided by the client.
        };

        _context.GolfClubs.Add(club);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClubs), new { id = club.Id }, club);
    }



    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        // Include courses if you want to ensure they are loaded before deletion
        var club = await _context.GolfClubs.Include(c => c.Courses)
                                           .FirstOrDefaultAsync(c => c.Id == id);
        if (club == null)
        {
            return NotFound("Golf club not found.");
        }

        // Optionally, check if there are any courses and handle accordingly
        // For example, you might want to prevent deletion if courses exist,
        // or you may allow cascade deletion.

        _context.GolfClubs.Remove(club);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
