
using BMS.Application.Common.Interfaces;
using BMS.Application.Controllers.Dtos;
using BMS.Application.Controllers.Queries;
using MediatR;

namespace BMS.Application.Controllers.Handlers
{
    public class GetControllerByIdQueryHandler
        : IRequestHandler<GetControllerByIdQuery, ControllerDto?>
    {
        private readonly IControllerRepository _controllerRepository;

        public GetControllerByIdQueryHandler(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        public async Task<ControllerDto?> Handle(
            GetControllerByIdQuery request,
            CancellationToken cancellationToken)
        {
            var controller = await _controllerRepository
                .GetByIdAsync(request.ControllerId, cancellationToken);

            if (controller == null)
                return null;

            return new ControllerDto
            {
                Id = controller.Id,
                Code = controller.Code,
                Name = controller.Name,
                UnitId = controller.UnitId,
                IpAddress = controller.IpAddress,
                Port = controller.Port,
                Protocol = controller.Protocol
            };
        }
    }
}
