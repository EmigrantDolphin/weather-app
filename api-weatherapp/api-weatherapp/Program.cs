using api_weatherapp;
using GoogleMaps;
using Application;
using OpenWeather;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

const int gmapsLimit = 800;
const int openWeatherLimit = 700;
const string gmapsLimiter = "gmaps";
const string openWeatherLimiter = "oWeather";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddGoogleMaps();
builder.Services.AddOpenWeatherService();
builder.Services.AddCors();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddFixedWindowLimiter(gmapsLimiter, options =>
    {
        options.AutoReplenishment = true;
        options.PermitLimit = gmapsLimit;
        options.Window = TimeSpan.FromDays(30);
    });
    options.AddFixedWindowLimiter(openWeatherLimiter, options =>
    {
        options.AutoReplenishment = true;
        options.PermitLimit = openWeatherLimit;
        options.Window = TimeSpan.FromDays(30);
    });
});

var app = builder.Build();

app.UseRateLimiter();

app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
    x.AllowAnyMethod();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("/api/location")
    .MapLocationApi()
    .RequireRateLimiting(gmapsLimiter);

app.MapGroup("/api/weather")
    .MapWeatherApi()
    .RequireRateLimiting(openWeatherLimiter);

app.Run();
