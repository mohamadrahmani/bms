using System;

namespace BMS.Infrastructure.Persistence.Entities
{
    public class DataPointHistoryEntity
    {
        public long Id { get; set; } // Identity (Clustered)

        public Guid DeviceId { get; set; } = default!;
        public Guid PointId { get; set; } = default!;

        public string? ValueString { get; set; } // universal storage

        public DateTime TimestampUtc { get; set; }
    }
}