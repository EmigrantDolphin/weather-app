using Application;
using GoogleMaps.Models;
using Moq;
using OneOfWrapper;
using OneOfWrapper.Types;
using OpenWeather;
using OpenWeather.Models;
using Shouldly;

namespace Tests;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetTodaysWeather_ReturnsWeather()
    {
        //Arrange
        var placeId = "1";
        var coordinate = new CoordinateModel(Latitude: 1L, Longitude: 2L);
        var weather = new WeatherModel
        (
            MainName: "Clouds",
            Description: "broken clouds",
            TemperatureKelvin: 267.34,
            TemperatureKelvinFeelsLike: 262.58,
            Pressure: 1035,
            Humidity: 79,
            WindSpeed: 3.09
        );

        var locationServiceMock = new Mock<ILocationService>();
        locationServiceMock.Setup(x => x.GetCityCoordinates(placeId, CancellationToken.None)).ReturnsAsync(new Success<CoordinateModel>(coordinate));

        var openWeatherMock = new Mock<IOpenWeatherService>();
        openWeatherMock.Setup(x => x.GetTodayWeather(coordinate.Longitude, coordinate.Latitude, CancellationToken.None)).ReturnsAsync(weather);

        var sut = new WeatherService(Helpers.GetLoggerMock<WeatherService>(), locationServiceMock.Object, openWeatherMock.Object);

        //Act

        var result = await sut.GetTodaysWeather(placeId, CancellationToken.None);

        //Assert
        result.IsSuccess().ShouldBeTrue();
        result.AsSuccess().Value.ShouldBe(weather);
        locationServiceMock.Verify(x => x.GetCityCoordinates(placeId, CancellationToken.None), Times.Once);
        openWeatherMock.Verify(x => x.GetTodayWeather(coordinate.Longitude, coordinate.Latitude, CancellationToken.None), Times.Once);
    }
}
