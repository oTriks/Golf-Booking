using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Lägg till Key Vault som konfigurationskälla
var kvUri = "https://golfbookingvault.vault.azure.net";
builder.Configuration.AddAzureKeyVault(new Uri(kvUri), new DefaultAzureCredential());

// Skapa en Key Vault client (om du behöver hämta andra hemligheter, t.ex. connection string)
var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

// Exempel: Hämta connection string (du kan även ha denna i appsettings.json eller i Key Vault)
KeyVaultSecret secret = client.GetSecret("ConnectDefault");
var connectionString = secret.Value;
Console.WriteLine($"Connection string successfully retrieved from Key Vault: {connectionString}");

// Configure DbContext with the retrieved connection string
builder.Services.AddDbContext<BookingContext>(options =>
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information));

// Registrera controllers
builder.Services.AddControllers();

// Konfigurera JWT-autentisering med värden hämtade från Key Vault via builder.Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["jwt-issuer"],
        ValidAudience = builder.Configuration["jwt-audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt-key"]))
    };
});
Console.WriteLine($"jwt-issuer: {builder.Configuration["jwt-issuer"]}");
Console.WriteLine($"jwt-audience: {builder.Configuration["jwt-audience"]}");
Console.WriteLine($"jwt-key: {builder.Configuration["jwt-key"]}");


// Lägg till Authorization
builder.Services.AddAuthorization();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Lägg till Authentication och Authorization i middleware-pipelinen
app.UseAuthentication();
app.UseAuthorization();

// Mappa controllers
app.MapControllers();

// Exempel WeatherForecast API Endpoint
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/", () => Results.Ok("Välkommen till Golf Booking API"));
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Testa databaskopplingen innan applikationen körs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingContext>();
    try
    {
        Console.WriteLine("Testing database connection...");
        var canConnect = dbContext.Database.CanConnect();
        Console.WriteLine(canConnect ? "Database connection successful!" : "Failed to connect to the database.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    }
}

app.Run();

// Define a WeatherForecast record for demonstration purposes
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
