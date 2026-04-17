using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Controllers.Commands
{

    [Audit(EventType.DeleteData, "Controllers")]
    public class DeleteControllerCommand : IRequest
    {
        public Guid ControllerId { get; set; }
    }
}
