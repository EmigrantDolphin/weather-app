using GoogleMaps.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace GoogleMaps;

public interface IPlaceService
{
	public Task<List<PredictionModel>> GetCityAutoCompleteResults(string incompleteCityName, CancellationToken token);
}

public class PlaceService : IPlaceService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlaceService> _logger;

    public PlaceService(ILogger<PlaceService> logger, HttpClient httpClient, IConfiguration configuration)
	{
        _apiKey = configuration["GoogleMapsApiKey"] ?? "";
        _httpClient = httpClient;
        _logger = logger;
	}

    public async Task<List<PredictionModel>> GetCityAutoCompleteResults(string incompleteCityName, CancellationToken token)
    {
        var url = $"autocomplete/json?input={incompleteCityName}&types=(cities)&key={_apiKey}";

        try
        {
            var result = await _httpClient.GetAsync(url, token);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Failed getting city predictions from google maps api. StatusCode: {statusCode}, reasonPhrase: {reasonPhrase}", result.StatusCode, result.ReasonPhrase);
                return new List<PredictionModel>();
            }

            var predictions = await result.Content.ReadFromJsonAsync<AutocompleteModel>(cancellationToken: token);
            if (predictions is null)
            {
                return new List<PredictionModel>();
            }

            return predictions.ToPredictionModels();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed getting city predictions from google maps api");
            return new List<PredictionModel>();
        }
    }
}
