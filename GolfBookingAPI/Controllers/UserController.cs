
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GolfBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using GolfBooking.Shared.Dtos;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly BookingContext _context;
    private readonly IConfiguration _configuration;

    public UserController(BookingContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            return BadRequest("Username already in use.");
        }

        var newUser = new AppUser
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role
        };

        if (request.Role.ToLower() != "admin")
        {
            if (!request.GolfClubId.HasValue)
            {
                return BadRequest("Club membership is required for players and personals.");
            }

            var club = await _context.GolfClubs.FindAsync(request.GolfClubId.Value);
            if (club == null)
            {
                return BadRequest("Invalid Golf Club ID.");
            }

            newUser.GolfClubId = request.GolfClubId.Value;
        }

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || user.Password != request.Password)
            return Unauthorized("Invalid credentials");

        var tokenString = JwtHelper.GenerateJwtToken(
            user.Username,
            user.Role,
            _configuration
        );

        // Return token, role, and username
        return Ok(new
        {
            token = tokenString,
            role = user.Role,
            username = user.Username
        });
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserRead>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        var userDtos = users.Select(u => new UserRead
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role
        }).ToList();

        return userDtos;
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdate request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Username = request.Username;
        user.Role = request.Role;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }


}
