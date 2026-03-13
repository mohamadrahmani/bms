using BMS.Application.Common.Interfaces;
using BMS.Application.Controllers.Dtos;
using BMS.Application.Controllers.Queries;
using MediatR;

namespace BMS.Application.Controllers.Handlers
{
    public class GetControllersListQueryHandler
        : IRequestHandler<GetControllersListQuery, List<ControllerDto>>
    {
        private readonly IControllerRepository _controllerRepository;

        public GetControllersListQueryHandler(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        public async Task<List<ControllerDto>> Handle(
            GetControllersListQuery request,
            CancellationToken cancellationToken)
        {
            var controllers = await _controllerRepository.GetAllAsync(cancellationToken);

            return controllers.Select(c => new ControllerDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                UnitId = c.UnitId,
                IpAddress = c.IpAddress,
                Port = c.Port,
                Protocol = c.Protocol
            }).ToList();
        }
    }
}
