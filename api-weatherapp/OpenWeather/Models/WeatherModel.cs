namespace OpenWeather.Models;

public record WeatherModel(
    string MainName,
    string Description,
    double TemperatureKelvin,
    double TemperatureKelvinFeelsLike,
    double Pressure,
    double Humidity,
    double WindSpeed
);
