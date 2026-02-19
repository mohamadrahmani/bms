namespace WebApi.Realtime.Models
{
    public class RealtimeUpdate
    {
        public string DatapointId { get; set; } = default!;

        public object? Value { get; set; }

        public string Quality { get; set; } = "Good";

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
