using BMS.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.DeviceSchedules.Commands
{
    public class UpdateDeviceScheduleCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; } // Id جدول DeviceSchedules
        public int StartDay { get; init; }
        public TimeSpan? StartTime { get; init; }
        public int EndDay { get; init; }
        public TimeSpan? EndTime { get; init; }
    }

}
