using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;

using MediatR;

namespace BMS.Application.Controllers.Commands
{
    [Audit(EventType.AddData, "Controllers")]
    public class CreateControllerCommand : IRequest<ApiResponse<Guid>>
    {
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public ControllerProtocol Protocol { get; init; }
        public string IpAddress { get; init; } = default!;
        public int Port { get; init; }
        public byte UnitId { get; init; }
        public int TimeoutMs { get; init; }
        public int RetryCount { get; init; }
        public int ScanIntervalMs { get; init; }
        public string? Description { get; init; }

        public string? FirmwareVersion { get; init; }
        public ControllerHealthStatus? HealthStatus { get; init; }

        public bool IsActive { get; init; } = true;

        public Guid? SiteId { get; init; }
        public Guid? BuildingId { get; init; }
        public Guid? FloorId { get; init; }
        public Guid? WardId { get; init; }
        public Guid? RoomId { get; init; }
    }
}
