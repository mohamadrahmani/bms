using WebApi.Realtime.Models;

namespace WebApi.Realtime.Services;

public interface ITwinRealtimePublisher
{
    Task PublishAsync(RealtimeUpdate update);

    Task PublishToDeviceAsync(string deviceId, RealtimeUpdate update);
}
