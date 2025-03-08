using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// **1. Retrieve Connection String from Azure Key Vault**
var kvUri = "https://golfbookingvault.vault.azure.net";
builder.Configuration.AddAzureKeyVault(new Uri(kvUri), new DefaultAzureCredential());

var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
KeyVaultSecret secret = client.GetSecret("ConnectDefault");
var connectionString = secret.Value;
Console.WriteLine($"Connection string successfully retrieved from Key Vault.");

// **2. Configure Database Context**
builder.Services.AddDbContext<BookingContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
        sqlServerOptions.EnableRetryOnFailure())
    .LogTo(Console.WriteLine, LogLevel.Information)
);

// **3. Add Controllers**
builder.Services.AddControllers();

// **Add CORS configuration**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5165")
      .AllowAnyHeader()
      .AllowAnyMethod();
    });
});

// **4. Configure Authentication & JWT Token Validation**
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt-key"]!))
    };
});

builder.Services.AddAuthorization();

// **5. Add API Documentation (Swagger)**
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Golf Booking API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}' (without quotes) in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// **6. Enable Swagger in Development Mode**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **7. Middleware Setup**
app.UseHttpsRedirection();

// Use CORS policy BEFORE Authentication/Authorization middleware:
app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

// **8. Map Controllers**
app.MapControllers();

// **9. Health Check & Debugging**
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
