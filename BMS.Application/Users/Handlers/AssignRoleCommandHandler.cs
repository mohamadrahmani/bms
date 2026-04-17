using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;

public sealed class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public AssignRoleCommandHandler(IUserRepository userRepository, IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException("کاربر یافت نشد");

        await _userRoleRepository.AssignRoleAsync(request.UserId, request.RoleId);

        return Unit.Value;
    }
}
