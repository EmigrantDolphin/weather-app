using Application;
using Microsoft.AspNetCore.Mvc;
using OneOfWrapper;
using System.Runtime.CompilerServices;

namespace api_weatherapp;

public static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocationApi(this RouteGroupBuilder builder)
    {
        builder.MapGet("/city-name", GetCityName);
        builder.MapGet("/city-predictions", GetCityPredictions);

        return builder;
    }

    public static async Task<IResult> GetCityName(ILocationService locationService, [FromQuery] double longitude, [FromQuery] double latitude, CancellationToken token)
    {
        var cityName = await locationService.GetCityName(longitude, latitude, token);

        if (cityName.IsSuccess())
        {
            return Results.Ok(cityName.AsSuccess().Value);
        }

        return Results.BadRequest(cityName.AsError().Message);
    }

    public static async Task<IResult> GetCityPredictions(ILocationService locationService, [FromQuery] string incompleteCityName, CancellationToken token)
    {
        var result = await locationService.GetCityPredictions(incompleteCityName, token);

        if (result.IsSuccess())
        {
            return Results.Ok(result.AsSuccess().Value);
        }

        return Results.BadRequest(result.AsError().Message);
    }
}
