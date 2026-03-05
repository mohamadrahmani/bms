using MediatR;

public sealed class UpdateUserCommand : IRequest
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }

    public IReadOnlyCollection<int> RoleIds { get; set; }
        = Array.Empty<int>();
}
