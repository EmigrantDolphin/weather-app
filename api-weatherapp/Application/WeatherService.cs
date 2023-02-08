using Microsoft.Extensions.Logging;
using OneOf;
using OneOfWrapper;
using OneOfWrapper.Types;
using OpenWeather;
using OpenWeather.Models;

namespace Application;

public interface IWeatherService
{
    public Task<OneOf<Success<WeatherModel>, BadRequest>> GetTodaysWeather(string placeId, CancellationToken token);
}

public class WeatherService : IWeatherService
{
    private readonly ILogger<WeatherService> _logger;
    private readonly ILocationService _locationService;
    private readonly IOpenWeatherService _openWeatherService;

    public WeatherService(ILogger<WeatherService> logger, ILocationService locationService, IOpenWeatherService openWeatherService)
    {
        _logger = logger;
        _locationService = locationService;
        _openWeatherService = openWeatherService;
    }

    public async Task<OneOf<Success<WeatherModel>, BadRequest>> GetTodaysWeather(string placeId, CancellationToken token)
    {
        var coordinatesResult = await _locationService.GetCityCoordinates(placeId, token);
        
        if (coordinatesResult.IsError())
        {
            return new BadRequest(coordinatesResult.AsError().Message);
        }

        var coordinates = coordinatesResult.AsSuccess().Value;

        var weather = await _openWeatherService.GetTodayWeather(coordinates.Longitude, coordinates.Latitude, token);

        if (weather is null)
        {
            return new BadRequest("Failed to get weather from open weather api");
        }

        return new Success<WeatherModel>(weather);
    }
}
