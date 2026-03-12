using MediatR;


namespace BMS.Application.Users.Commands;

/// <summary>
/// دستور (Command) CQRS برای ایجاد حساب کاربری جدید
/// برای یک شخص (Person) موجود در سیستم.
/// </summary>
public sealed record CreateUserCommand(
    Guid PersonId,
    string UserName,
    string Password,
    IReadOnlyCollection<int>? RoleIds
) : IRequest<Guid>;
