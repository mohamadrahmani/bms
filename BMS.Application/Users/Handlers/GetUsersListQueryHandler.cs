using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Users.Dtos;
using BMS.Application.Users.Queries;
using System.Linq;
using BMS.Application.Common.Pagination;

namespace BMS.Application.Users.Handlers;

public sealed class GetUsersListQueryHandler
    : IRequestHandler<GetUsersListQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersListQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> Handle(
        GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var users = _userRepository.Users;

        var query = users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                IsActive = u.IsActive,
                PersonId = u.PersonId,
                PersonFullName = u.Person.FirstName + " " + u.Person.LastName,
                RoleIds = u.UserRoles
                    .Select(r => r.RoleId)
                    .ToList()
            });

        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

    }
}