using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Persons.Commands
{
    [Audit(EventType.AddData, "Persons")]
    public sealed record CreatePersonCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Mobile
) : IRequest<Guid>;// IRequest<ApiResponse<Guid>>;

}
