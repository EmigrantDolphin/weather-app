using GoogleMaps.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace GoogleMaps;

public interface IGeocodeService
{
	public Task<string?> GetCityName(double longitude, double latitude, CancellationToken token);
	public Task<CoordinateModel?> GetCoordinates(string placeId, CancellationToken token);
}

public class GeocodeService : IGeocodeService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeocodeService> _logger;

    public GeocodeService(HttpClient httpClient, IConfiguration configuration, ILogger<GeocodeService> logger)
	{
		_httpClient = httpClient;
        _apiKey = configuration["GoogleMapsApiKey"] ?? "";
        _logger = logger;
	}

    public async Task<string?> GetCityName(double longitude, double latitude, CancellationToken token)
    {
        var url = $"json?latlng={latitude},{longitude}&key={_apiKey}";

        try
        {
            var result = await _httpClient.GetAsync(url, token);

            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get city name from google maps. StatusCode: {statusCode}, reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase);
                return null;
            }

            var cityNameModel = await result.Content.ReadFromJsonAsync<CityNameModel>(cancellationToken: token);

            if (cityNameModel is not null)
            {
                return cityNameModel.GetCityName();
            }

            _logger.LogError("Failed to get city name from google maps. StatusCode: {statusCode}, reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase);
            return null;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to get city name from google maps.");
            return null;
        }
    }

    public async Task<CoordinateModel?> GetCoordinates(string placeId, CancellationToken token)
    {
        var url = $"json?place_id={placeId}&key={_apiKey}";

        try
        {
            var result = await _httpClient.GetAsync(url, token);

            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get city coordinate from google maps. StatusCode: {statusCode}, reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase);
                return null;
            }

            var coordinatesResult = await result.Content.ReadFromJsonAsync<GoogleCoordinateModel>(cancellationToken: token);

            if (coordinatesResult is not null)
            {
                return coordinatesResult.ToCoordinateModel();
            }

            _logger.LogError("Failed to get city coordinates from google maps. StatusCode: {statusCode}, reason: {reasonPhrase}", result.StatusCode, result.ReasonPhrase);
            return null;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to get city coordinates from google maps.");
            return null;
        }
    }
}
