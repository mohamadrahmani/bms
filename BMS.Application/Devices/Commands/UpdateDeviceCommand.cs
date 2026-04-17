//using BMS.Application.Devices.DTOs;
//using MediatR;

//namespace BMS.Application.Devices.Commands
//{
//    public class UpdateDeviceCommand : IRequest<DeviceDto>
//    {
//        public UpdateDeviceDto Device { get; set; }

//        public UpdateDeviceCommand(UpdateDeviceDto device)
//        {
//            Device = device;
//        }
//    }
//}
//using BMS.Application.Devices.DTOs;
//using BMS.Domain.Entities.BMS;
//using MediatR;
//using System;

//namespace BMS.Application.Devices.Commands
//{
//    public class UpdateDeviceCommand : IRequest<DeviceDto>
//    {
//        public Guid Id { get; set; }

//        public string Name { get; set; } = default!;

//        public DeviceType Type { get; set; }

//        public bool EnableAlarming { get; set; }

//        public bool EnableTrending { get; set; }

//        public bool IsActive { get; set; }

//        // Location
//        public Guid SiteId { get; set; }
//        public Guid BuildingId { get; set; }
//        public Guid FloorId { get; set; }
//        public Guid WardId { get; set; }
//        public Guid RoomId { get; set; }
//    }
//}
using BMS.Application.Attributes;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Devices.Commands
{
    [Audit(EventType.UpdateData, "Devices")]
    public class UpdateDeviceCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }

        public Guid? ControllerId { get; set; }
        public string? Code { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? Description { get; set; } = default!;

        public DeviceType Type { get; set; }

        public bool? EnableAlarming { get; set; }
        public bool? EnableTrending { get; set; }
        public bool? IsActive { get; set; }

        public Guid? SiteId { get; set; }
        public Guid? BuildingId { get; set; }
        public Guid? FloorId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? RoomId { get; set; }

        [JsonIgnore]
        public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();
    }
}
