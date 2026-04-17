using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace BMS.Application.Controllers.Commands
{

    [Audit(EventType.UpdateData, "Controllers")]
    public class UpdateControllerCommand : IRequest<ApiResponse<bool>>
    {
        public Guid ControllerId { get; set; }

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

        public bool IsActive { get; init; }

        public Guid? SiteId { get; init; }
        public Guid? BuildingId { get; init; }
        public Guid? FloorId { get; init; }
        public Guid? WardId { get; init; }
        public Guid? RoomId { get; init; }

        [JsonIgnore]
        public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

    }
}
