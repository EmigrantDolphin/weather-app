using System.Text.Json.Serialization;

namespace GoogleMaps.Models;

internal class CityNameModel
{
    [JsonPropertyName("plus_code")]
    public PlusCode? PlusCode { get; init; }

    [JsonPropertyName("results")]
    public Results[]? Results { get; init; }

    public string? GetCityName()
    {
        if (Results is null)
        {
            return PlusCode?.CompoundCode;
        }

        var addressComponents = Results.Where(x => x.AddressComponents is not null).SelectMany(x => x.AddressComponents!).ToList();
        var cityName = addressComponents.Where(x => x.IsLocality()).FirstOrDefault()?.LongName;

        if (cityName is not null)
        {
            return cityName;
        }

        return PlusCode?.CompoundCode;
    }
}

internal record PlusCode
{
    [JsonPropertyName("compound_code")]
    public string? CompoundCode { get; init; }
}

internal record Results
{
    [JsonPropertyName("address_components")]
    public AddressComponents[]? AddressComponents { get; init; }
}

internal record AddressComponents
{
    [JsonPropertyName("long_name")]
    public string? LongName { get; init; }

    [JsonPropertyName("types")]
    public string[]? Types { get; init; }

    public bool IsLocality() => Types != null && Types.Contains("locality");
}
