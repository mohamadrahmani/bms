using MediatR;
using BMS.Application.Users.Dtos;


namespace BMS.Application.Users.Queries;

public sealed class GetUsersListQuery : IRequest<List<UserDto>>
{
}
