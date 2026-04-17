using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;
[Audit(EventType.DeleteData, "Wards")]
public class DeleteWardCommand : IRequest
{
    public Guid Id { get; set; }
}
