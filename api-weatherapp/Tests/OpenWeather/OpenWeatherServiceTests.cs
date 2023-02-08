using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenWeather;
using OpenWeather.Models;
using RichardSzalay.MockHttp;
using Shouldly;

namespace Tests.OpenWeather;

public class OpenWeatherServiceTests
{
    [Fact]
    public async Task GetTodaysWeather_ReturnsTodaysWeather()
    {
        //Arrange
        var weatherModel = new WeatherModel
        (
            MainName: "Clouds",
            Description: "broken clouds",
            TemperatureKelvin: 267.34,
            TemperatureKelvinFeelsLike: 262.58,
            Pressure: 1035,
            Humidity: 79,
            WindSpeed: 3.09
        );
        var longitude = 1L;
        var latitude = 2L;
        var apiKey = "apiKey";
        var baseUrl = "https://api.openweathermap.org/data/2.5/";
        var url = $"{baseUrl}weather?lat={latitude}&lon={longitude}&appid={apiKey}";

        var mockHandler = new MockHttpMessageHandler();
        mockHandler.When(url).Respond("application/json", GetTodayWeatherResponse(weatherModel));
        var httpClient = mockHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);

        var settings = new Dictionary<string, string?>
        {
            { "OpenWeatherApiKey", apiKey }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var sut = new OpenWeatherService(Helpers.GetLoggerMock<OpenWeatherService>(), httpClient, configuration);

        //Act
        var result = await sut.GetTodayWeather(longitude, latitude, CancellationToken.None);

        //Assert

        result.ShouldNotBeNull();
        result.ShouldBe(weatherModel);
    }

    private string GetTodayWeatherResponse(WeatherModel weather)
    {
        var weatherResponse = new
        {
            weather = new[]
            {
                new
                {
                    main = weather.MainName,
                    description = weather.Description
                }
            },
            main = new
            {
                temp = weather.TemperatureKelvin,
                feels_like = weather.TemperatureKelvinFeelsLike,
                pressure = weather.Pressure,
                humidity = weather.Humidity
            },
            wind = new
            {
                speed = weather.WindSpeed
            }
        };

        return JsonConvert.SerializeObject(weatherResponse);
    }
}
