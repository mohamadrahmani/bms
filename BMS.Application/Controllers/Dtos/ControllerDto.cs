//using BMS.Domain.Entities.BMS;

//namespace BMS.Application.Controllers.Dtos
//{
//    public class ControllerDto
//    {
//        public Guid Id { get; set; }
//        public string Code { get; set; } = default!;
//        public string Name { get; set; } = default!;
//        public byte UnitId { get; set; }
//        public string IpAddress { get; set; } = default!;
//        public int Port { get; set; }
//        public ControllerProtocol Protocol { get; set; }
//    }
//}
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Controllers.Dtos
{
    public class ControllerDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        public ControllerProtocol Protocol { get; set; }

        public string IpAddress { get; set; } = default!;
        public int Port { get; set; }
        public byte UnitId { get; set; }

        public int TimeoutMs { get; set; }
        public int RetryCount { get; set; }
        public int ScanIntervalMs { get; set; }

        public string? Description { get; set; }

        public string? FirmwareVersion { get; set; }
        public ControllerHealthStatus? HealthStatus { get; set; }

        public bool IsActive { get; set; }

        public Guid? SiteId { get; set; }
        public Guid? BuildingId { get; set; }
        public Guid? FloorId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? RoomId { get; set; }
    }
}
