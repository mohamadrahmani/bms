using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;

namespace BMS.Application.Users.Commands
{
    public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
                throw new NotFoundException("کاربر پیدا نشد.");
            //throw new NotFoundException("User not found.");

            await _userRepository.DeleteAsync(user);

            return Unit.Value;
        }
    }
}
