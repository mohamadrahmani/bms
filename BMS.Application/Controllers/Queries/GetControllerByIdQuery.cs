using MediatR;
using BMS.Application.Controllers.Dtos;

namespace BMS.Application.Controllers.Queries
{
    public class GetControllerByIdQuery : IRequest<ControllerDto?>
    {
        public Guid ControllerId { get; init; }
    }
}
