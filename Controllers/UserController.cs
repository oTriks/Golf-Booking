using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Golf_Booking.Models;
using Golf_Booking.Dtos;
using Microsoft.AspNetCore.Authorization;

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
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null)
        {
            return BadRequest("Username already in use.");
        }

        // For a real app, store a hashed password instead of plain text
        var newUser = new AppUser
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role
        };

        // If the user is not an admin, require a club membership
        if (request.Role.ToLower() != "admin")
        {
            if (!request.GolfClubId.HasValue)
            {
                return BadRequest("Club membership is required for players and personals.");
            }

            // Check if the provided club exists
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


    // 2) Login: Return JWT (similar to your existing AccountController)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return Unauthorized("Invalid credentials");

        // Compare passwords (for production: use a proper hashing check)
        if (user.Password != request.Password)
            return Unauthorized("Invalid credentials");

        // Create JWT with user-specific role
        var tokenString = JwtHelper.GenerateJwtToken(
            user.Username,
            user.Role,
            _configuration
        );

        return Ok(new { token = tokenString });
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

}


