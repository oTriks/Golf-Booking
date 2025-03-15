using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GolfBookingAPI.Models;
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
            Type = clubDto.Type
        };

        _context.GolfClubs.Add(club);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClubs), new { id = club.Id }, club);
    }



    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        var club = await _context.GolfClubs.Include(c => c.Courses)
                                           .FirstOrDefaultAsync(c => c.Id == id);
        if (club == null)
        {
            return NotFound("Golf club not found.");
        }

        _context.GolfClubs.Remove(club);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ClubRead>> GetClub(int id)
    {
        var club = await _context.GolfClubs
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (club == null)
        {
            return NotFound();
        }

        var clubDto = new ClubRead
        {
            Id = club.Id,
            Name = club.Name,
            Location = club.Location,
            Type = club.Type,
            Courses = club.Courses.Select(course => new CourseRead
            {
                Id = course.Id,
                Name = course.Name,
                GolfClubId = course.GolfClubId,
                GolfClubName = club.Name,
                Type = course.Type
            }).ToList()
        };

        return clubDto;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClub(int id, [FromBody] ClubUpdate clubDto)
    {
        var club = await _context.GolfClubs.FindAsync(id);
        if (club == null)
        {
            return NotFound("Golf club not found.");
        }

        club.Name = clubDto.Name;
        club.Location = clubDto.Location;
        club.Type = clubDto.Type;

        await _context.SaveChangesAsync();
        return NoContent();
    }

}
