using Microsoft.Extensions.DependencyInjection;

namespace GoogleMaps;

public static class GoogleMapsExtensions
{
    public static IServiceCollection AddGoogleMaps(this IServiceCollection services)
    {
        services.AddScoped<IGeocodeService, GeocodeService>(); // check if this is needed
        services.AddHttpClient<IGeocodeService, GeocodeService>(client => 
        {
            client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/geocode/");
        });


        services.AddScoped<IPlaceService, PlaceService>(); // check if this is needed
        services.AddHttpClient<IPlaceService, PlaceService>(client =>
        {
            client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/place/");
        });

        return services;
    }
}
