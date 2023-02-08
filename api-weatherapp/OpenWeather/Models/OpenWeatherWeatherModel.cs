using System.Text.Json.Serialization;

namespace OpenWeather.Models;

internal record OpenWeatherWeatherModel
{
    [JsonPropertyName("weather")]
    public Weather[] Weather { get; set; } = Array.Empty<Weather>();

    [JsonPropertyName("main")]
    public Main? Main { get; init; }

    [JsonPropertyName("wind")]
    public Wind? Wind { get; init; }

    public WeatherModel ToWeatherModel()
    {
        return new WeatherModel(
            MainName: Weather.Any() ? Weather[0].Main! : "",
            Description: Weather.Any() ? Weather[0].Description! : "",
            TemperatureKelvin: Main?.Temp ?? 0,
            TemperatureKelvinFeelsLike: Main?.FeelsLike ?? 0,
            Pressure: Main?.Pressure ?? 0,
            Humidity: Main?.Humidity ?? 0,
            WindSpeed: Wind?.Speed ?? 0
            );
    }
}

internal record Weather
{
    [JsonPropertyName("main")]
    public string? Main { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

internal record Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; init; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; init; }

    [JsonPropertyName("pressure")]
    public double Pressure { get; init; }

    [JsonPropertyName("humidity")]
    public double Humidity { get; init; }
}

internal record Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; init; }
}
