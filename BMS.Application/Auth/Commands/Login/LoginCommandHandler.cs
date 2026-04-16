using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;

namespace BMS.Application.Auth.Commands.Login
{
    public sealed class LoginCommandHandler
        : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionResolver _permissionResolver;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            IUnitOfWork unitOfWork,
            IPermissionResolver permissionResolver)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
            _permissionResolver = permissionResolver;
        }

        public async Task<LoginResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var normalizedUserName = request.UserName
                .Trim()
                .ToLowerInvariant();

            var hashedPassword = _passwordHasher.Hash(request.Password);

            //return new LoginResponse(
            //    Guid.Empty,
            //    request.UserName,
            //    "token",
            //    DateTime.Now.AddMinutes(60));


            var user = await _userRepository
                .GetActiveByUserNameAsync(normalizedUserName, cancellationToken);

            if (user is null)
                throw new BusinessRuleException("نام کاربری یا رمز ورود نامعتبر می باشد");

            try
            {
                // بررسی وضعیت اکانت (Active + Lock)
                user.EnsureCanLogin();

                var isValid = _passwordHasher
                    .Verify(request.Password, user.PasswordHash);

                // در صورت اشتباه بودن پسورد Exception می‌اندازد
                user.RegisterLoginResult(isValid);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (UserDomainException)
            {
                // حتی در صورت خطا باید state ذخیره شود
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw;
            }

            // 🔐 Resolve permissions (از Infrastructure)
            var permissions = await _permissionResolver
                .ResolveAsync(user.Id, cancellationToken);

            // 🎟 Generate JWT with permissions + PermissionVersion
            var (token, expiresAt) = _jwtProvider.GenerateToken(
                user,
                permissions,
                user.PermissionVersion);

            return new LoginResponse(
                user.Id,
                user.UserName,
                token,
                expiresAt);
        }
    }
}
