using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Exempel på en POST-inloggningsmetod
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validera användaruppgifter (detta är ett förenklat exempel)
        // I en riktig applikation hämtar du användaren från databasen och verifierar lösenordet.
        if (request.Username != "admin" || request.Password != "password")
        {
            return Unauthorized("Invalid credentials");
        }

        // Skapa claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim("role", "Admin"), // eller "Spelare" beroende på användarens roll
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Hämta nyckel och konfiguration från inställningarna
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt-key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Skapa tokenen
        var token = new JwtSecurityToken(
            issuer: _configuration["jwt-issuer"],
            audience: _configuration["jwt-audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // exempel: token gäller i 30 minuter
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }
}

// Modell för inloggningsförfrågan
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
