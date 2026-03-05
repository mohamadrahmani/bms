using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;


namespace BMS.Application.Users.Commands
{
    public sealed class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdWithRolesAsync(request.UserId, cancellationToken);

            if (user is null)
                throw new NotFoundException(
                    $"User with id '{request.UserId}' not found.");

            var normalizedUserName = request.UserName
                .Trim()
                .ToLowerInvariant();

            var exists = await _userRepository
                .ExistsByUserNameAsync(
                    normalizedUserName,
                    cancellationToken,
                    request.UserId);

            if (exists)
                throw new BusinessRuleException("Username already exists.");

            user.UpdateProfile(request.UserName, request.IsActive);

            // ✅ FIXED
            user.SyncRoles(request.RoleIds ?? Enumerable.Empty<int>());

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
