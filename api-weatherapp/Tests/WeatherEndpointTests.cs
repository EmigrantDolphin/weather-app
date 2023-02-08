using api_weatherapp;
using Application;
using HttpResults = Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneOfWrapper.Types;
using Shouldly;
using OpenWeather.Models;

namespace Tests;

public class WeatherEndpointTests
{
    [Fact]
    public async Task GetWeather_ReturnsWeather()
    {
        //Arrange
        var placeId = "1";
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

        var weatherServiceMock = new Mock<IWeatherService>();
        weatherServiceMock.Setup(x => x.GetTodaysWeather(placeId, CancellationToken.None)).ReturnsAsync(new Success<WeatherModel>(weatherModel));

        //Act
        var response = (HttpResults.Ok<WeatherModel>) await WeatherEndpoints.GetTodaysWeather(weatherServiceMock.Object, placeId, CancellationToken.None);

        //Assert
        response.Value.ShouldNotBeNull();
        response.Value.ShouldBe(weatherModel);
        weatherServiceMock.Verify(x => x.GetTodaysWeather(placeId, CancellationToken.None), Times.Once);
    }
}
