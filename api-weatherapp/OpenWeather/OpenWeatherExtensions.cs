using Microsoft.Extensions.DependencyInjection;

namespace OpenWeather;

public static class OpenWeatherExtensions
{

    public static IServiceCollection AddOpenWeatherService(this IServiceCollection services)
    {
        services.AddScoped<IOpenWeatherService, OpenWeatherService>(); // check if this is needed
        services.AddHttpClient<IOpenWeatherService, OpenWeatherService>(client => 
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        });

        return services;
    }
}
