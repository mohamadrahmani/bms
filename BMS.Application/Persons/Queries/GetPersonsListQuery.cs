using MediatR;
using BMS.Application.Users.Dtos;
using BMS.Application.Persons.Dtos;
using BMS.Application.Common.Pagination;




public sealed class GetPersonsListQuery : PagedRequest, IRequest<PagedResult<PersonDto>>
{
}
