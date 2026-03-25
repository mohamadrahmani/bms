using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Persons.Commands;

public sealed record UpdatePersonCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Mobile
) : IRequest<ApiResponse<bool>>;
