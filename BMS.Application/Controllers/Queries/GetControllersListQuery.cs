using MediatR;
using BMS.Application.Controllers.Dtos;
using System.Collections.Generic;

namespace BMS.Application.Controllers.Queries
{
    public record GetControllersListQuery : IRequest<List<ControllerDto>>
    {
    }
}
