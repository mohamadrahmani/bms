using BMS.Application.Models;
using MediatR;

namespace BMS.Application.DeviceSchedules.Commands
{
    public class SetDeviceActivationScheduleCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; } // Id جدول DeviceSchedules
        public int StartDay { get; init; }
        public TimeSpan? StartTime { get; init; }
        public int EndDay { get; init; }
        public TimeSpan? EndTime { get; init; }
    }

}
