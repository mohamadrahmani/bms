using BMS.Application.Models;
using MediatR;

public sealed class UpdateUserCommand : IRequest<ApiResponse<bool>>
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; }
    public IReadOnlyCollection<int> RoleIds { get; set; }
        = Array.Empty<int>();
}
