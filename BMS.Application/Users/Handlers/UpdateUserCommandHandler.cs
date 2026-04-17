using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Exceptions;
using MediatR;


namespace BMS.Application.Users.Commands
{
    public sealed class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, ApiResponse<bool>>
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

        public async Task<ApiResponse<bool>> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdWithRolesAsync(request.UserId, cancellationToken);

            if (user is null)
                //throw new NotFoundException($"User with id '{request.UserId}' not found.");
                throw new NotFoundException($"کاربری با شناسه {request.UserId} یافت نشد.");


            var normalizedUserName = request.UserName
                .Trim()
                .ToLowerInvariant();

            var exists = await _userRepository
                .ExistsByUserNameAsync(
                    normalizedUserName,
                    cancellationToken,
                    request.UserId);

            if (exists)
                //throw new BusinessRuleException("Username already exists.");
                throw new BusinessRuleException("نام کاربری موجود است.");

            // بررسی تغییرات
            if (user.UserName != request.UserName)
            {
                request.Changes.Add(("UserName", user.UserName, request.UserName));
            }
            if (user.IsActive != request.IsActive)
            {
                request.Changes.Add(("IsActive", user.IsActive.ToString(), request.IsActive.ToString()));
            }


            user.UpdateProfile(request.UserName, request.IsActive);

            // ✅ FIXED
            user.SyncRoles(request.RoleIds ?? Enumerable.Empty<int>());

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "اطلاعات بروزرسانی شد");
        }
    }
}
