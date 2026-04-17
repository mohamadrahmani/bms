using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;

[Audit(EventType.UpdateData, "Users")]
public sealed class ChangePasswordCommand : IRequest
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
