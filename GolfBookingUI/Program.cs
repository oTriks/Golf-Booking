using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using GolfBookingUI; // Your project's root namespace
using GolfBookingUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5210") });
builder.Services.AddScoped<GolfClubService>();

await builder.Build().RunAsync();
