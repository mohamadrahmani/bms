using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Persons.Dtos;
using BMS.Application.Common.Pagination;
using BMS.Application.Users.Dtos;
using BMS.Domain.Entities;

namespace BMS.Application.Persons.Handlers;

public sealed class GetPersonsListQueryHandler
    : IRequestHandler<GetPersonsListQuery, PagedResult<PersonDto>>
{
    private readonly IPersonRepository _PersonRepository;

    public GetPersonsListQueryHandler(IPersonRepository PersonRepository)
    {
        _PersonRepository = PersonRepository;
    }

    public async Task<PagedResult<PersonDto>> Handle(
        GetPersonsListQuery request,
        CancellationToken cancellationToken)
    {
        //var Persons = await _PersonRepository
        //    .GetAll(cancellationToken);

        //return Persons
        //    .Select(u => new PersonDto
        //    {
        //        Id = u.Id,
        //        FirstName = u.FirstName,
        //        LastName = u.LastName
        //    })
        //    .ToList();


        var query = _PersonRepository.Persons
            .Select(u => new PersonDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Mobile = u.Mobile,
                IsActive = u.IsActive
            });

        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
