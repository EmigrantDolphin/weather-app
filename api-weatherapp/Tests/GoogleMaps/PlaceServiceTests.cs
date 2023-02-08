using GoogleMaps;
using GoogleMaps.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Shouldly;

namespace Tests.GoogleMaps;

public class PlaceServiceTests
{
    [Fact]
    public async Task GetCityAutoCompletePredictions_ReturnsListOfCities()
    {
        //Arrange
        var incompleteCityName = "Ka";
        var apiKey = "apiKey";
        var predictions = new List<PredictionModel>()
        {
            new PredictionModel("1","Kaunas"),
            new PredictionModel("2","Kansas")
        };

        var baseUrl = "https://maps.googleapis.com/maps/api/place/";
        var url = $"{baseUrl}autocomplete/json?input={incompleteCityName}&types=(cities)&key={apiKey}";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Respond("application/json", GetAutoCompleteResponse(predictions[0], predictions[1]));
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);

        var settings = new Dictionary<string, string?>
        {
            {"GoogleMapsApiKey", apiKey }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var sut = new PlaceService(Helpers.GetLoggerMock<PlaceService>(), httpClient, configuration);

        //Act
        var result = await sut.GetCityAutoCompleteResults(incompleteCityName, CancellationToken.None);

        //Assert
        result.Count.ShouldBe(predictions.Count);
        result.Where(x => x.PlaceId == predictions[0].PlaceId).First().CityName.ShouldBe(predictions[0].CityName);
        result.Where(x => x.PlaceId == predictions[1].PlaceId).First().CityName.ShouldBe(predictions[1].CityName);
    }

    private string GetAutoCompleteResponse(PredictionModel predictionOne, PredictionModel predictionTwo)
    {
        var response = new
        {
            predictions = new[]
            {
                new { place_id = predictionOne.PlaceId, structured_formatting = new { main_text = predictionOne.CityName }, types = new[]{ "locality" } },
                new { place_id = predictionTwo.PlaceId, structured_formatting = new { main_text = predictionTwo.CityName }, types = new[]{ "locality" } },
            }
        };

        return JsonConvert.SerializeObject(response);
    }
}
