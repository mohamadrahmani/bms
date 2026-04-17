using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;

public sealed class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPermissionRepository _permissionRepository;

    public AssignPermissionCommandHandler(IUserRepository userRepository, IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<Unit> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("کاربر یافت نشد");

        await _permissionRepository.AssignPermissionAsync(request.UserId, request.PermissionId);

        return Unit.Value;
    }
}
