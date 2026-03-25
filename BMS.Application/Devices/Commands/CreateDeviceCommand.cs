
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;
using System;

namespace BMS.Application.Devices.Commands
{
    public sealed class CreateDeviceCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid ControllerId { get; init; }
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public DeviceType Type { get; init; }
        public string? Description { get; init; } = default!;

        public bool IsActive { get; init; } = true;

        public Guid? SiteId { get; init; }
        public Guid? BuildingId { get; init; }
        public Guid? FloorId { get; init; }
        public Guid? WardId { get; init; }
        public Guid? RoomId { get; init; }
    }
}
