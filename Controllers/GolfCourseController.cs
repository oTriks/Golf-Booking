using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Golf_Booking.Models;
using Golf_Booking.Dtos;

[ApiController]
[Route("api/[controller]")]
public class GolfCourseController : ControllerBase
{
    private readonly BookingContext _context;

    public GolfCourseController(BookingContext context)
    {
        _context = context;
    }

    // GET: api/GolfCourse
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
            Type = c.Type  // Map the stored type from your domain model
        }).ToList();

        return courseDtos;
    }

    // POST: api/GolfCourse
    [Authorize(Roles = "Personal,Admin")]
    [HttpPost]
    public async Task<ActionResult<CourseRead>> CreateCourse([FromBody] CourseCreate courseDto)
    {
        // Check if the referenced golf club exists
        var club = await _context.GolfClubs.FindAsync(courseDto.GolfClubId);
        if (club == null)
        {
            return BadRequest("Invalid Golf Club ID.");
        }

        var course = new GolfCourse
        {
            Name = courseDto.Name,
            GolfClubId = courseDto.GolfClubId,
            GolfClub = club,  // Set the navigation property
            Type = courseDto.Type
        };

        _context.GolfCourses.Add(course);
        await _context.SaveChangesAsync();

        // Map to DTO to avoid cycles in serialization:
        var courseReadDto = new CourseRead
        {
            Id = course.Id,
            Name = course.Name,
            GolfClubId = course.GolfClubId,
            GolfClubName = course.GolfClub?.Name,
            Type = course.Type  // Include the Type property here

        };

        return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, courseReadDto);
    }


    [Authorize(Roles = "Admin")]
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


}
