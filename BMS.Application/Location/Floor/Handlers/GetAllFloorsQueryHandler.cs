using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Controllers.Dtos;
using BMS.Application.Location.Floors.Dtos;
using BMS.Application.Location.Floors.Queries;
using MediatR;

public class GetAllFloorsQueryHandler
    :  IRequestHandler<GetAllFloorsQuery, PagedResult<FloorDto>>
{
    private readonly IFloorRepository _repository;

    public GetAllFloorsQueryHandler(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<FloorDto>> Handle(
        GetAllFloorsQuery request,
        CancellationToken cancellationToken)
    {
        var floors =  _repository.Floors;

        var query = floors.Select(x => new FloorDto
        {
            Id = x.Id,
            BuildingId = x.BuildingId,
            Name = x.Name,
            LevelNumber = x.LevelNumber,
            Description = x.Description
        });
        return await query.ToPagedResultAsync(
        request.PageNumber,
        request.PageSize,
        cancellationToken);

    }
}
