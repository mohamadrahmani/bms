using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Persons.Commands;
[Audit(EventType.UpdateData, "Persons")]
public sealed record UpdatePersonCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Mobile
  ) : IRequest<ApiResponse<bool>>
{
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; }
        = new();
}
