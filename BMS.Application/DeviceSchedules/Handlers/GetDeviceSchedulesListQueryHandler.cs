using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.DeviceSchedules.Dtos;
using BMS.Application.DeviceSchedules.Queries;
using MediatR;

namespace BMS.Application.DeviceSchedules.Handlers
{
    public class GetDeviceSchedulesListQueryHandler
        : IRequestHandler<GetDeviceSchedulesListQuery, PagedResult<DeviceScheduleDto>>
    {
        private readonly IDeviceScheduleRepository _deviceScheduleRepository;

        public GetDeviceSchedulesListQueryHandler( IDeviceScheduleRepository deviceScheduleRepository)
        {
            _deviceScheduleRepository = deviceScheduleRepository;
        }

        public async Task<PagedResult<DeviceScheduleDto>> Handle(
            GetDeviceSchedulesListQuery request,
            CancellationToken cancellationToken)
        {
            var deviceSchedules = _deviceScheduleRepository.DeviceSchedules;
            //// فیلترهای مکانی
            //if (request.SiteId.HasValue)
            //    DeviceSchedules = DeviceSchedules.Where(c => c.Location.SiteId == request.SiteId);

            //if (request.BuildingId.HasValue)
            //    DeviceSchedules = DeviceSchedules.Where(c => c.Location.BuildingId == request.BuildingId);

            //if (request.FloorId.HasValue)
            //    DeviceSchedules = DeviceSchedules.Where(c => c.Location.FloorId == request.FloorId);

            //if (request.WardId.HasValue)
            //    DeviceSchedules = DeviceSchedules.Where(c => c.Location.WardId == request.WardId);

            //if (request.RoomId.HasValue)
            //    DeviceSchedules = DeviceSchedules.Where(c => c.Location.RoomId == request.RoomId);
            //// سرچ فقط روی Name
            //if (!string.IsNullOrWhiteSpace(request.Search))
            //{
            //    DeviceSchedules = DeviceSchedules.Where(c =>
            //        c.Name.Contains(request.Search));
            //}


            // فیلترهای داینامیک
            deviceSchedules = deviceSchedules.ApplyDynamicFilters(request.Filters);


            var query = deviceSchedules.Select(c => new DeviceScheduleDto
            {
                 Id = c.Id,
                 DeviceId = c.DeviceId,
                 EndDay = c.EndDay,
                 EndTime = c.EndTime,
                 IsActive = c.IsActive,
                 RegisterIndex = c.RegisterIndex,
                 StartDay = c.StartDay,
                 StartTime = c.StartTime
            });

            return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        }
    }
}
