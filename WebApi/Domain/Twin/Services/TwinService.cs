using WebApi.Infrastructure.Twin;
using WebApi.Realtime.Models;
using WebApi.Realtime.Services;

namespace WebApi.Domain.Twin.Services;

public class TwinService : ITwinService
{
    private readonly ITwinRepository _repository;
    private readonly ITwinRealtimePublisher _publisher;

    public TwinService(
        ITwinRepository repository,
        ITwinRealtimePublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task UpdateAsync(
        string twinId,
        string dataPointId,
        object? value)
    {
        var twin = _repository.GetOrCreate(twinId);

        var dp = twin.GetOrCreate(dataPointId);

        var changed = dp.Update(value);

        if (!changed)
            return;

        // todo:
        //await _publisher.PublishToDeviceAsync(
        await _publisher.PublishAsync(
            new RealtimeUpdate
            {
                DatapointId = dataPointId,
                Value = value,
                Timestamp = dp.Timestamp
            });
    }
}
