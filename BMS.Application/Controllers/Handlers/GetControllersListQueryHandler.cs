using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;
using BMS.Application.Controllers.Queries;
using MediatR;

namespace BMS.Application.Controllers.Handlers
{
    public class GetControllersListQueryHandler
        : IRequestHandler<GetControllersListQuery, PagedResult<ControllerDto>>
    {
        private readonly IControllerRepository _controllerRepository;

        public GetControllersListQueryHandler(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        public async Task<PagedResult<ControllerDto>> Handle(
            GetControllersListQuery request,
            CancellationToken cancellationToken)
        {
            var controllers = _controllerRepository.Controllers;
            // فیلترهای مکانی
            if (request.SiteId.HasValue)
                controllers = controllers.Where(c => c.Location.SiteId == request.SiteId);

            if (request.BuildingId.HasValue)
                controllers = controllers.Where(c => c.Location.BuildingId == request.BuildingId);

            if (request.FloorId.HasValue)
                controllers = controllers.Where(c => c.Location.FloorId == request.FloorId);

            if (request.WardId.HasValue)
                controllers = controllers.Where(c => c.Location.WardId == request.WardId);

            if (request.RoomId.HasValue)
                controllers = controllers.Where(c => c.Location.RoomId == request.RoomId);
            // سرچ فقط روی Name
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                controllers = controllers.Where(c =>
                    c.Name.Contains(request.Search));
            }

            var query = controllers.Select(c => new ControllerDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                UnitId = c.UnitId,
                IpAddress = c.IpAddress,
                Port = c.Port,
                Protocol = c.Protocol,
                Description = c.Description,
                TimeoutMs = c.TimeoutMs,
                RetryCount = c.RetryCount,
                ScanIntervalMs = c.ScanIntervalMs,
                FirmwareVersion = c.FirmwareVersion,
                HealthStatus = c.HealthStatus,
                IsActive = c.IsActive,
                SiteId = c.Location.SiteId,
                BuildingId = c.Location.BuildingId,
                FloorId = c.Location.FloorId,
                WardId = c.Location.WardId,
                RoomId = c.Location.RoomId
            });

            return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        }
    }
}
