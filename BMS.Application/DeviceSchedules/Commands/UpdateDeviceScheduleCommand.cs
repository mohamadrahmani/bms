using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.DeviceSchedules.Commands
{
    [Audit(EventType.UpdateData, "DeviceSchedules")]
    public class UpdateDeviceScheduleCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public int StartDay { get; init; }
        public TimeSpan? StartTime { get; init; }
        public int EndDay { get; init; }
        public TimeSpan? EndTime { get; init; }

        [JsonIgnore]
        public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

        [JsonIgnore]
        public string? Name { get; set; }
    }

}
