using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Users.Dtos;
using BMS.Application.Users.Queries;
using System.Linq;

namespace BMS.Application.Users.Handlers;

public sealed class GetUsersListQueryHandler
    : IRequestHandler<GetUsersListQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersListQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(
        GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository
            .GetAllWithRolesAsync(cancellationToken);

        return users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                IsActive = u.IsActive,
                RoleIds = u.UserRoles
                    .Select(r => r.RoleId)
                    .ToList()
            })
            .ToList();
    }
}
