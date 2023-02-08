using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenWeather.Models;
using System.Net.Http.Json;

namespace OpenWeather;

public interface IOpenWeatherService
{
    public Task<WeatherModel?> GetTodayWeather(double longitude, double latitude, CancellationToken token);
}

public class OpenWeatherService : IOpenWeatherService
{
    private readonly ILogger<OpenWeatherService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherService(ILogger<OpenWeatherService> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherApiKey"] ?? "";
    }

    public async Task<WeatherModel?> GetTodayWeather(double longitude, double latitude, CancellationToken token)
    {
        var url = $"weather?lat={latitude}&lon={longitude}&appid={_apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url, token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get a response from open weather. StatusCode: {statusCode}, reasonPhrase: {reasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return null;
            }

            var weatherResponse = await response.Content.ReadFromJsonAsync<OpenWeatherWeatherModel>(cancellationToken: token);

            if (weatherResponse is null)
            {
                _logger.LogError("Failed to get a response from open weather. StatusCode: {statusCode}, reasonPhrase: {reasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return weatherResponse.ToWeatherModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get weather from Open Weather API");
            return null;
        }
    }
}
