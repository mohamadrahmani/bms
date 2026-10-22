using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using MediatR;

namespace BMS.Application.DeviceSchedules.Commands
{
    [Audit(EventType.AddData, "DeviceSchedules")]
    public class CreateDeviceScheduleCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid DeviceId { get; set; }
        public bool IsActive { get; set; }
        public int StartDay { get; init; }
        public TimeSpan? StartTime { get; init; }
        public int EndDay { get; init; }
        public TimeSpan? EndTime { get; init; }
        public int RegisterIndex { get; init; }

        [JsonIgnore]
        public string? Name { get; set; }

        [JsonIgnore]
        public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

    }
}
