namespace WebApi.Domain.Twin.Entities;

public class DigitalTwin
{
    public string Id { get; }

    private readonly Dictionary<string, DataPoint> _dataPoints = new();

    public DigitalTwin(string id)
    {
        Id = id;
    }

    public IReadOnlyCollection<DataPoint> DataPoints => _dataPoints.Values;

    public DataPoint GetOrCreate(string dpId)
    {
        if (!_dataPoints.ContainsKey(dpId))
        {
            _dataPoints[dpId] = new DataPoint(dpId);
        }

        return _dataPoints[dpId];
    }
}
