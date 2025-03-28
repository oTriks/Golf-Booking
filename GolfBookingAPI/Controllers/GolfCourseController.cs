using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GolfBookingAPI.Models;
using GolfBooking.Shared.Dtos;

[ApiController]
[Route("api/[controller]")]
public class GolfCourseController : ControllerBase
{
    private readonly BookingContext _context;

    public GolfCourseController(BookingContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseRead>>> GetCourses()
    {
        var courses = await _context.GolfCourses.Include(c => c.GolfClub).ToListAsync();

        var courseDtos = courses.Select(c => new CourseRead
        {
            Id = c.Id,
            Name = c.Name,
            GolfClubId = c.GolfClubId,
            GolfClubName = c.GolfClub?.Name,
            Type = c.Type
        }).ToList();

        return courseDtos;
    }

    [Authorize(Roles = "Personal,Admin")]
    [HttpPost]
    public async Task<ActionResult<CourseRead>> CreateCourse([FromBody] CourseCreate courseDto)
    {
        var club = await _context.GolfClubs.FindAsync(courseDto.GolfClubId);
        if (club == null)
        {
            return BadRequest("Invalid Golf Club ID.");
        }

        var course = new GolfCourse
        {
            Name = courseDto.Name,
            GolfClubId = courseDto.GolfClubId,
            GolfClub = club,
            Type = courseDto.Type
        };

        _context.GolfCourses.Add(course);
        await _context.SaveChangesAsync();

        var courseReadDto = new CourseRead
        {
            Id = course.Id,
            Name = course.Name,
            GolfClubId = course.GolfClubId,
            GolfClubName = course.GolfClub?.Name,
            Type = course.Type

        };

        return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, courseReadDto);
    }


    [Authorize(Roles = "Admin,Personal")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.GolfCourses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Golf course not found.");
        }

        _context.GolfCourses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Personal,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdate courseDto)
    {
        var course = await _context.GolfCourses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Golf course not found.");
        }

        course.Name = courseDto.Name;
        course.Type = courseDto.Type;
        course.GolfClubId = courseDto.GolfClubId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

}
