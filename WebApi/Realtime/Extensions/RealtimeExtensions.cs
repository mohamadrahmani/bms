using WebApi.Realtime.Hubs;
using WebApi.Realtime.Services;

namespace WebApi.Realtime.Extensions;

public static class RealtimeExtensions
{
    public static IServiceCollection AddRealtimeInfrastructure(
        this IServiceCollection services)
    {
        services.AddSignalR();

        //services.AddScoped<ITwinRealtimePublisher, TwinRealtimePublisher>();

        return services;
    }

    public static IEndpointRouteBuilder MapRealtimeEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<TwinHub>("/hubs/twin");

        return endpoints;
    }
}
