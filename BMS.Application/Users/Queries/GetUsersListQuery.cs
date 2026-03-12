using MediatR;
using BMS.Application.Users.Dtos;
using BMS.Application.Common.Pagination;


namespace BMS.Application.Users.Queries;

public sealed class GetUsersListQuery : PagedRequest, IRequest<PagedResult<UserDto>>
{
}
