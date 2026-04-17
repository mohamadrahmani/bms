using MediatR;
using BMS.Application.Attributes;
using BMS.Domain.Enums;
using BMS.Domain.Entities.Logs;

[Audit(EventType.UpdateData, "Users")]
public class AssignPermissionCommand : IRequest
{
    public Guid UserId { get; set; }

    public int PermissionId { get; set; }
}
