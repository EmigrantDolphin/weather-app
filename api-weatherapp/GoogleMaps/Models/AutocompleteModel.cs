using System.Text.Json.Serialization;

namespace GoogleMaps.Models;

internal record AutocompleteModel
{
    [JsonPropertyName("predictions")]
    public List<Prediction>? Predictions { get; init; }

    public List<PredictionModel> ToPredictionModels()
    {
        if (Predictions is null)
        {
            return new List<PredictionModel>();
        }

        var validPredictions = Predictions
            .Where(x => x.Types is not null && x.Formatting is not null)
            .Where(x => x.PlaceId is not null && x.Formatting!.MainText is not null)
            .ToList();

        if (!validPredictions.Any() )
        {
            return new List<PredictionModel>();
        }

        var cityPredictions = validPredictions.Select(x => new PredictionModel(x.PlaceId!, x.Formatting!.MainText!)).ToList();

        return cityPredictions;
    }
}

internal record Prediction
{
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; init; }

    [JsonPropertyName("structured_formatting")]
    public StructuredFormatting? Formatting { get; init; }

    [JsonPropertyName("types")]
    public string[]? Types { get; init; }
}

internal record StructuredFormatting
{
    [JsonPropertyName("main_text")]
    public string? MainText { get; init; }
}
