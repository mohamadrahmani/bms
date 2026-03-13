using BMS.Domain.Entities.BMS;
using System;

namespace BMS.Application.Devices.DTOs
{
    public class UpdateDeviceDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public DeviceType Type { get; set; }

        public bool EnableAlarming { get; set; }

        public bool EnableTrending { get; set; }

        public bool IsActive { get; set; }
        // ✅ Location
        public Guid SiteId { get; set; }
        public Guid BuildingId { get; set; }
        public Guid FloorId { get; set; }
        public Guid WardId { get; set; }
        public Guid RoomId { get; set; }
    }
}
