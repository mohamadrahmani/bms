using BMS.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Persons.Commands
{
    public sealed record CreatePersonCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Mobile
) : IRequest<ApiResponse<Guid>>;

}
