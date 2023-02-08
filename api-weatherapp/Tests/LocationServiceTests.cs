using Application;
using GoogleMaps;
using GoogleMaps.Models;
using Moq;
using OneOfWrapper;
using OneOfWrapper.Types;
using Shouldly;

namespace Tests;

public class LocationServiceTests
{
    [Fact]
    public async Task GetCityNames_ReturnsCityName()
    {
        //Arrange
        var cityName = "Kaunas";
        var longitude = 1L;
        var latitude = 2L;
        var geocodeServiceMock = new Mock<IGeocodeService>();
        var placeServiceMock = new Mock<IPlaceService>();

        geocodeServiceMock.Setup(x => x.GetCityName(longitude, latitude, CancellationToken.None)).ReturnsAsync(cityName);

        var sut = new LocationService(geocodeServiceMock.Object, placeServiceMock.Object, Helpers.GetLoggerMock<LocationService>());

        //Act
        var result = await sut.GetCityName(longitude, latitude, CancellationToken.None);

        //Assert

        result.IsSuccess().ShouldBe(true);
        result.AsSuccess().Value.ShouldBe(cityName);
        geocodeServiceMock.Verify(x => x.GetCityName(longitude, latitude, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCityNames_GoogleServiceReturnsNull_ReturnsBadRequest()
    {
        //Arrange
        string? cityName = null;
        var longitude = 1L;
        var latitude = 2L;
        var geocodeServiceMock = new Mock<IGeocodeService>();
        var placeServiceMock = new Mock<IPlaceService>();

        geocodeServiceMock.Setup(x => x.GetCityName(longitude, latitude, CancellationToken.None)).ReturnsAsync(cityName);

        var sut = new LocationService(geocodeServiceMock.Object, placeServiceMock.Object, Helpers.GetLoggerMock<LocationService>());

        //Act
        var result = await sut.GetCityName(longitude, latitude, CancellationToken.None);

        //Assert

        result.IsSuccess().ShouldBe(false);
        result.IsError().ShouldBe(true);
        result.AsError().ShouldBeOfType<BadRequest>();
        geocodeServiceMock.Verify(x => x.GetCityName(longitude, latitude, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCityPredictions_ReturnsPredictions()
    {
        //Arrange
        var incompleteCityName = "Ka";
        var predictions = new List<PredictionModel>()
        {
            new PredictionModel("1","Kaunas"),
            new PredictionModel("2","Kansas")
        };
        var geocodeServiceMock = new Mock<IGeocodeService>();
        var placeServiceMock = new Mock<IPlaceService>();

        placeServiceMock.Setup(x => x.GetCityAutoCompleteResults(incompleteCityName, CancellationToken.None)).ReturnsAsync(predictions);

        var sut = new LocationService(geocodeServiceMock.Object, placeServiceMock.Object, Helpers.GetLoggerMock<LocationService>());

        //Act
        var result = await sut.GetCityPredictions(incompleteCityName, CancellationToken.None);

        //Assert

        result.IsSuccess().ShouldBe(true);
        result.AsSuccess().Value.Count.ShouldBe(2);
        result.AsSuccess().Value.First(x => x.PlaceId.Equals(predictions[0].PlaceId)).CityName.ShouldBe(predictions[0].CityName);
        result.AsSuccess().Value.First(x => x.PlaceId.Equals(predictions[1].PlaceId)).CityName.ShouldBe(predictions[1].CityName);
        placeServiceMock.Verify(x => x.GetCityAutoCompleteResults(incompleteCityName, CancellationToken.None), Times.Once);
    }


    [Fact]
    public async Task GetCityCoordinates_ReturnsCoordinates()
    {
        //Arrange
        var coordinatesModel = new CoordinateModel(1L, 2L);
        var placeId = "1";
        var geocodeServiceMock = new Mock<IGeocodeService>();
        var placeServiceMock = new Mock<IPlaceService>();

        geocodeServiceMock.Setup(x => x.GetCoordinates(placeId, CancellationToken.None)).ReturnsAsync(coordinatesModel);

        var sut = new LocationService(geocodeServiceMock.Object, placeServiceMock.Object, Helpers.GetLoggerMock<LocationService>());

        //Act
        var result = await sut.GetCityCoordinates(placeId, CancellationToken.None);

        //Assert

        result.IsSuccess().ShouldBe(true);
        result.AsSuccess().Value.ShouldBe(coordinatesModel);
        geocodeServiceMock.Verify(x => x.GetCoordinates(placeId, CancellationToken.None), Times.Once);
    }
}
