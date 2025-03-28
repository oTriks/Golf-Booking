using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GolfBookingAPI.Models;
using GolfBooking.Shared.Dtos;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
public class WaitingListController : ControllerBase
{
    private readonly BookingContext _context;

    public WaitingListController(BookingContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<WaitingListRead>> AddToWaitingList([FromBody] WaitingListCreate dto)
    {
        var exists = await _context.WaitingLists.AnyAsync(w =>
            w.GolfCourseId == dto.GolfCourseId &&
            w.TeeTime == dto.TeeTime &&
            w.UserId == dto.UserId);
        if (exists)
            return BadRequest("You are already on the waiting list for this time slot.");

        var entry = new WaitingList
        {
            UserId = dto.UserId,
            GolfCourseId = dto.GolfCourseId,
            TeeTime = dto.TeeTime,
            CreatedAt = DateTime.UtcNow
        };

        _context.WaitingLists.Add(entry);
        await _context.SaveChangesAsync();

        var readDto = new WaitingListRead
        {
            Id = entry.Id,
            UserId = entry.UserId,
            GolfCourseId = entry.GolfCourseId,
            TeeTime = entry.TeeTime,
            CreatedAt = entry.CreatedAt
        };

        return CreatedAtAction(nameof(GetWaitingList), new { id = entry.Id }, readDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WaitingListRead>> GetWaitingList(int id)
    {
        var entry = await _context.WaitingLists.FindAsync(id);
        if (entry == null)
            return NotFound();
        var readDto = new WaitingListRead
        {
            Id = entry.Id,
            UserId = entry.UserId,
            GolfCourseId = entry.GolfCourseId,
            TeeTime = entry.TeeTime,
            CreatedAt = entry.CreatedAt
        };
        return readDto;
    }

    [HttpGet("availability/{courseId}/{teeTime}")]
    public async Task<ActionResult<IEnumerable<WaitingListRead>>> GetWaitingListForSlot(int courseId, DateTime teeTime)
    {
        var entries = await _context.WaitingLists
            .Where(w => w.GolfCourseId == courseId && w.TeeTime == teeTime)
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();

        var result = entries.Select(e => new WaitingListRead
        {
            Id = e.Id,
            UserId = e.UserId,
            GolfCourseId = e.GolfCourseId,
            TeeTime = e.TeeTime,
            CreatedAt = e.CreatedAt
        });

        return Ok(result);
    }

    [HttpGet("myentries")]
    public async Task<ActionResult<IEnumerable<WaitingListRead>>> GetMyWaitingListEntries()
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var entries = await _context.WaitingLists
            .Where(w => w.UserId == username)
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();

        var result = entries.Select(e => new WaitingListRead
        {
            Id = e.Id,
            UserId = e.UserId,
            GolfCourseId = e.GolfCourseId,
            TeeTime = e.TeeTime,
            CreatedAt = e.CreatedAt
        });

        return Ok(result);
    }

}
