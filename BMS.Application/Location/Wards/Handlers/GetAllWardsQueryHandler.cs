using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Location.Wards.Dtos;
using BMS.Application.Location.Wards.Queries;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BMS.Application.Location.Wards.Handlers;

public class GetAllWardsQueryHandler : IRequestHandler<GetAllWardsQuery, PagedResult<WardDto>>
{
    private readonly IWardRepository _repository;

    public GetAllWardsQueryHandler(IWardRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<WardDto>> Handle(GetAllWardsQuery request, CancellationToken cancellationToken)
    {
        var wards = _repository.Wards;
        // filter by floor
        if (request.FloorId.HasValue)
        {
            wards = wards.Where(w => w.FloorId == request.FloorId);
        }

        // filter by type
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            wards = wards.Where(w => w.Type == request.Type);
        }

        // search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            wards = wards.Where(w =>
                w.Name.Contains(request.Search) ||
                (w.Type != null && w.Type.Contains(request.Search)));
        }

        var query =wards.Select(w => new WardDto
        {
            Id = w.Id,
            FloorId = w.FloorId,
            Name = w.Name,
            Type = w.Type,
            Description = w.Description

        });
        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);


    }
}
