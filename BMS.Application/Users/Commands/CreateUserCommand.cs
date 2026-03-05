using MediatR;


namespace BMS.Application.Users.Commands;

/// <summary>
/// دستور (Command) CQRS برای ایجاد حساب کاربری جدید
/// برای یک شخص (Person) موجود در سیستم.
/// </summary>
public sealed record CreateUserCommand(
    Guid PersonId,                     // شناسه شخص موجود
    string UserName,                   // نام کاربری
    string Password,                   // رمز عبور خام (در Handler هش می‌شود)
    IReadOnlyCollection<int>? RoleIds   // 🔥 اصلاح شد: int به جای Guid
) : IRequest<Guid>;
