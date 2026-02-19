using WebApi.Domain.Twin.Entities;

namespace WebApi.Infrastructure.Twin;

public interface ITwinRepository
{
    DigitalTwin GetOrCreate(string twinId);
}

public class InMemoryTwinRepository : ITwinRepository
{
    private readonly Dictionary<string, DigitalTwin> _twins = new();

    public DigitalTwin GetOrCreate(string twinId)
    {
        if (!_twins.ContainsKey(twinId))
        {
            _twins[twinId] = new DigitalTwin(twinId);
        }

        return _twins[twinId];
    }
}
