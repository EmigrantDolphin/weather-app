using api_weatherapp;
using Application;
using HttpResults = Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OneOfWrapper.Types;
using Shouldly;
using GoogleMaps.Models;

namespace Tests;

public class LocationEndpointTests
{
    [Fact]
    public async Task GetCityName_ReturnsCityName()
    {
        //Arrange
        var longitude = 1L;
        var latitude = 2L;
        var cityName = "Kaunas";

        var locationServiceMock = new Mock<ILocationService>();
        locationServiceMock.Setup(x => x.GetCityName(longitude, latitude, CancellationToken.None)).ReturnsAsync(new Success<string>(cityName));

        //Act
        var response = (HttpResults.Ok<string>) await LocationEndpoints.GetCityName(locationServiceMock.Object, longitude, latitude, CancellationToken.None);

        //Assert
        response.StatusCode.ShouldBe(200); // this is from microsoft documentation but this makes no sense.
                                           // This assertion is always true, because if it's not true, the cast to Ok<string> in 'act' section will throw an exception and the execution won't reach this line of code.
                                           // I've been told that microsoft only hires gifted people and it seems that it's true.
                                           // Link to documentation if you are interested https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-7.0

        response.Value.ShouldBe(cityName);
        locationServiceMock.Verify(x => x.GetCityName(longitude, latitude, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCityName_ReturnsBadRequest()
    {
        //Arrange
        var longitude = 1L;
        var latitude = 2L;
        var message = "Failed to get city name";

        var locationServiceMock = new Mock<ILocationService>();
        locationServiceMock.Setup(x => x.GetCityName(longitude, latitude, CancellationToken.None)).ReturnsAsync(new BadRequest(message));

        //Act
        var response = (HttpResults.BadRequest<string>) await LocationEndpoints.GetCityName(locationServiceMock.Object, longitude, latitude, CancellationToken.None);

        //Assert
        response.Value.ShouldBe(message);
        locationServiceMock.Verify(x => x.GetCityName(longitude, latitude, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCityPredictions_ReturnsPredictions()
    {
        //Arrange
        var incompleteCityName = "Ka";
        var predictionModels = new List<PredictionModel>
        {
            new PredictionModel("1", "Kaunas"),
            new PredictionModel("2", "Kaunas")
        };

        var locationServiceMock = new Mock<ILocationService>();
        locationServiceMock.Setup(x => x.GetCityPredictions(incompleteCityName, CancellationToken.None)).ReturnsAsync(new Success<List<PredictionModel>>(predictionModels));

        //Act
        var response = (HttpResults.Ok<List<PredictionModel>>) await LocationEndpoints.GetCityPredictions(locationServiceMock.Object, incompleteCityName, CancellationToken.None);

        //Assert
        response.Value.ShouldNotBeNull();
        response.Value.Count.ShouldBe(2);
        locationServiceMock.Verify(x => x.GetCityPredictions(incompleteCityName, CancellationToken.None), Times.Once);
    }
}
