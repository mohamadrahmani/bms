using MediatR;

public sealed class ChangePasswordCommand : IRequest
{
    public Guid UserId { get; set; }

    public string NewPassword { get; set; } = null!;
}
