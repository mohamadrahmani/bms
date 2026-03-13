using MediatR;

namespace BMS.Application.Controllers.Commands
{
    public class DeleteControllerCommand : IRequest
    {
        public Guid ControllerId { get; set; }
    }
}
