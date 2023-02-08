using System.Runtime;
using System.Text.Json.Serialization;

namespace GoogleMaps.Models;

internal record GoogleCoordinateModel
{
    public Result[] Results { get; init; } = Array.Empty<Result>();

    public CoordinateModel? ToCoordinateModel()
    {
        var validResults = Results
            .Where(x => x.Geometry is not null && x.Geometry.Location is not null)
            .ToList();

        if (validResults.Any())
        {
            return new CoordinateModel(validResults[0].Geometry!.Location!.Lat, validResults[0].Geometry!.Location!.Lng);
        }

        return null;
    }
}

internal record Result
{
    public Geometry? Geometry { get; init; }
}

internal record Geometry
{
    public Location? Location { get; init; }
}

internal record Location
{
    [JsonPropertyName("lat")]
    public double Lat { get; init; }

    [JsonPropertyName("lng")]
    public double Lng { get; init; }
}
