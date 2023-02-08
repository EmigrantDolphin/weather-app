using GoogleMaps;
using GoogleMaps.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;

namespace Tests.GoogleMaps;

public class GeocodeServiceTests
{
    [Fact]
    public async Task GetCityName_ReturnsCityName()
    {
        //Arrange
        var longitude = 1L;
        var latitude = 2L;
        var cityName = "Kaunas";
        var apiKey = "apiKey";

        var baseUrl = "https://maps.googleapis.com/maps/api/geocode/";
        var url = $"{baseUrl}json?latlng={latitude},{longitude}&key={apiKey}";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Respond("application/json", GetCityNameResponse());
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);

        var settings = new Dictionary<string, string?> { { "GoogleMapsApiKey", apiKey } };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var sut = new GeocodeService(httpClient, configuration, Helpers.GetLoggerMock<GeocodeService>());

        //Act
        var result = await sut.GetCityName(longitude, latitude, CancellationToken.None);

        //Assert
        result.ShouldBe(cityName);
    }

    private string GetCityNameResponse()
    {
        var response = new { plus_code = new { compound_code = "Kaunas" } };

        return JsonConvert.SerializeObject(response);
    }

    [Fact]
    public async Task GetCityName_WhenThrowsException_ReturnsNull()
    {
        //Arrange
        var longitude = 1L;
        var latitude = 2L;
        var apiKey = "apiKey";

        var mockHttp = new MockHttpMessageHandler();
        var baseUrl = "https://maps.googleapis.com/maps/api/geocode/";
        var url = $"{baseUrl}json?latlng={latitude},{longitude}&key={apiKey}";
        mockHttp.When(url).Throw(new Exception());

        var settings = new Dictionary<string, string?>
        {
            { "GoogleMapsApiKey", apiKey }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);
        var sut = new GeocodeService(httpClient, configuration, Helpers.GetLoggerMock<GeocodeService>());

        //Act
        var result = await sut.GetCityName(longitude, latitude, CancellationToken.None);

        //Assert
        result.ShouldBe(null);
    }

    [Fact]
    public async Task GetCityCoordinate_ReturnsCityCoordinate()
    {
        //Arrange
        var placeId = "placeId";
        var apiKey = "apiKey";
        var coordinateModel = new CoordinateModel(1L, 2L);

        var baseUrl = "https://maps.googleapis.com/maps/api/geocode/";
        var url = $"{baseUrl}json?place_id={placeId}&key={apiKey}";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Respond("application/json", GetCityCoordinateResponse(coordinateModel));
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);

        var settings = new Dictionary<string, string?> { { "GoogleMapsApiKey", apiKey } };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var sut = new GeocodeService(httpClient, configuration, Helpers.GetLoggerMock<GeocodeService>());

        //Act
        var result = await sut.GetCoordinates(placeId, CancellationToken.None);

        //Assert
        result.ShouldBe(coordinateModel);
    }

    private string GetCityCoordinateResponse(CoordinateModel model)
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    geometry = new
                    {
                        location = new
                        {
                            lat = model.Latitude,
                            lng = model.Longitude
                        }
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(response);
    }
}
