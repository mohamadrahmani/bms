using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;

public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            throw new BusinessRuleException("User not found.");

        var newHash = _passwordHasher.Hash(request.NewPassword);

        user.ChangePassword(newHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
