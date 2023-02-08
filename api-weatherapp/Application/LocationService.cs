using GoogleMaps;
using GoogleMaps.Models;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOfWrapper.Types;

namespace Application;

public interface ILocationService
{
    public Task<OneOf<Success<string>, BadRequest>> GetCityName(double longitude, double latitude, CancellationToken token);
    public Task<OneOf<Success<List<PredictionModel>>, BadRequest>> GetCityPredictions(string incompleteCityName, CancellationToken token);
    public Task<OneOf<Success<CoordinateModel>, BadRequest>> GetCityCoordinates(string PlaceId, CancellationToken token);
}

public class LocationService : ILocationService
{
    private readonly IGeocodeService _geocodeService;
    private readonly IPlaceService _placeService;

    public LocationService(IGeocodeService geocodeService, IPlaceService placeService, ILogger<LocationService> logger)
    {
        _geocodeService = geocodeService;
        _placeService = placeService;
    }

    public async Task<OneOf<Success<CoordinateModel>, BadRequest>> GetCityCoordinates(string PlaceId, CancellationToken token)
    {
        var result = await _geocodeService.GetCoordinates(PlaceId, token);

        if (result is null)
        {
            return new BadRequest("Failed getting city coordinates from google maps");
        }

        return new Success<CoordinateModel>(result);
    }

    public async Task<OneOf<Success<string>, BadRequest>> GetCityName(double longitude, double latitude, CancellationToken token)
    {
        var result = await _geocodeService.GetCityName(longitude, latitude, token);

        if (result is null)
        {
            return new BadRequest("Failed getting city name from google maps");
        }

        return new Success<string>(result);
    }

    public async Task<OneOf<Success<List<PredictionModel>>, BadRequest>> GetCityPredictions(string incompleteCityName, CancellationToken token)
    {
        var result = await _placeService.GetCityAutoCompleteResults(incompleteCityName, token);

        if (result is null)
        {
            return new BadRequest("Failed getting city predictions from google maps");
        }

        return new Success<List<PredictionModel>>(result);
    }
}
