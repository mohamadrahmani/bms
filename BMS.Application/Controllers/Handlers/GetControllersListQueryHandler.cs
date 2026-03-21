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

            var query = controllers.Select(c => new ControllerDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                UnitId = c.UnitId,
                IpAddress = c.IpAddress,
                Port = c.Port,
                Protocol = c.Protocol,
                Description = c.Description
            });

            return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        }
    }
}
