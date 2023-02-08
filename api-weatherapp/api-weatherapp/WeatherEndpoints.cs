using Application;
using Microsoft.AspNetCore.Mvc;
using OneOfWrapper;

namespace api_weatherapp;

public static class WeatherEndpoints
{
    public static RouteGroupBuilder MapWeatherApi(this RouteGroupBuilder builder)
    {
        builder.MapGet("/today", GetTodaysWeather);
        return builder;
    }

    public static async Task<IResult> GetTodaysWeather(IWeatherService weatherService, [FromQuery] string placeId, CancellationToken token)
    {
        var result = await weatherService.GetTodaysWeather(placeId, token);

        if (result.IsSuccess())
        {
            return Results.Ok(result.AsSuccess().Value);
        }

        return Results.BadRequest(result.AsError().Message);
    }
}
