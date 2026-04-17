using MediatR;
using BMS.Application.Attributes;
using BMS.Domain.Enums;
using BMS.Domain.Entities.Logs;

[Audit(EventType.DeleteData, "Users")]
public class DeleteUserCommand : IRequest
{
    public Guid Id { get; set; }
}
