using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;
using MediatR;
using System.Collections.Generic;

namespace BMS.Application.Controllers.Queries
{
    public sealed class GetControllersListQuery : PagedRequest, IRequest<PagedResult<ControllerDto>>
    {
    }
}
