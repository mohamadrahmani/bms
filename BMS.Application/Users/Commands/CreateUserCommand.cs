using MediatR;
using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using BMS.Application.Models;


namespace BMS.Application.Users.Commands;

/// <summary>
/// دستور (Command) CQRS برای ایجاد حساب کاربری جدید
/// برای یک شخص (Person) موجود در سیستم.
/// </summary>
[Audit(EventType.AddData, "Users")]
public sealed record CreateUserCommand(
    Guid PersonId,
    string UserName,
    string Password,
    IReadOnlyCollection<int>? RoleIds
) : IRequest<ApiResponse<Guid>>;
