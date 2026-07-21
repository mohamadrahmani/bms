using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;
using BMS.Domain.Entities.Logs;
using System.Text.Json;

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
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IAuditLogger _auditLogger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider,
            IUnitOfWork unitOfWork,
            IPermissionResolver permissionResolver,
            ILogger<LoginCommandHandler> logger,
            IAuditLogger auditLogger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _unitOfWork = unitOfWork;
            _permissionResolver = permissionResolver;
            _logger = logger;
            _auditLogger = auditLogger;
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
            {
                _logger.LogWarning("Login failed. User not found: {UserName}", request.UserName);

                _auditLogger.Add(new Log
                {
                    UserId = null,
                    EventType = EventType.Login,
                    ObjectName = "Users",
                    //ObjectId = request.UserName,
                    ObjectId = normalizedUserName,
                    Result = OperationResult.Failed,
                    ResultMessage = "تلاش ناموفق برای ورود - نام کاربری یافت نشد",
                    LogDate = DateTime.UtcNow,
                    IpAddress = request.IpAddress,
                    Source = nameof(LoginCommandHandler),
                    RequestBody = JsonSerializer.Serialize(new { Username = normalizedUserName, Password = "***" }),

                });
                throw new BusinessRuleException("نام کاربری یا رمز ورود نامعتبر می باشد");
            }

            try
            {
                // بررسی وضعیت اکانت (Active + Lock)
                user.EnsureCanLogin();

                var isValid = _passwordHasher
                    .Verify(request.Password, user.PasswordHash);

                // در صورت اشتباه بودن پسورد Exception می‌اندازد
                user.RegisterLoginResult(isValid);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (!isValid)
                {
                    _logger.LogWarning("Login failed. Wrong password for user {UserName}", user.UserName);
                    _auditLogger.Add(new Log
                    {
                        UserId = user.Id,
                        EventType = EventType.Login,
                        Result = OperationResult.Failed,
                        ResultMessage = "تلاش ناموفق برای ورود - رمز عبور اشتباه است",
                        IpAddress = request.IpAddress,
                        //Source = "LoginCommandHandler",
                        Source = nameof(LoginCommandHandler),
                        LogDate = DateTime.UtcNow,
                        ObjectName = "Users",
                        ObjectId = user.Id.ToString(),
                        RequestBody = JsonSerializer.Serialize(new { Username = normalizedUserName, Password = "***" })
                    });

                    throw new BusinessRuleException("نام کاربری یا رمز ورود نامعتبر می باشد");
                }
            }
            catch (UserDomainException ex)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogError(ex, "Login error for user {UserName}", request.UserName);

                _auditLogger.Add(new Log
                {
                    UserId = user?.Id,
                    EventType = EventType.Login,
                    Result = OperationResult.Error,
                    ResultMessage = $"خطا در فرآیند ورود کاربر: {ex.Message}",
                    IpAddress = request.IpAddress,
                    //Source = "LoginCommandHandler",
                    Source = nameof(LoginCommandHandler),
                    LogDate = DateTime.UtcNow,
                    ObjectName = "Users",
                    ObjectId = user?.Id.ToString(),
                    RequestBody = JsonSerializer.Serialize(new { Username = normalizedUserName, Password = "***" }) // Storing sanitized request body
                });

                throw;
            }

            var permissions = await _permissionResolver
                .ResolveAsync(user.Id, cancellationToken);

            var (token, expiresAt) = _jwtProvider.GenerateToken(
                user,
                permissions,
                user.PermissionVersion);

            _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

            _auditLogger.Add(new Log
            {
                UserId = user.Id,
                EventType = EventType.Login,
                Result = OperationResult.Success,
                ResultMessage = "ورود کاربر با موفقیت انجام شد",
                IpAddress = request.IpAddress,
                //Source = "LoginCommandHandler",
                Source = nameof(LoginCommandHandler),
                LogDate = DateTime.UtcNow,
                ObjectName = "Users",
                ObjectId = user.Id.ToString()

            });

            return new LoginResponse(
                user.Id,
                user.Person.FirstName + " " + user.Person.LastName,
                user.UserName,
                token,
                expiresAt,
                permissions);
        }
    }
}
