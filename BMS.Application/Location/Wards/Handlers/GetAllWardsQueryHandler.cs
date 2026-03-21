using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Wards.Dtos;
using BMS.Application.Location.Wards.Queries;
using MediatR;

namespace BMS.Application.Location.Wards.Handlers;

public class GetAllWardsQueryHandler : IRequestHandler<GetAllWardsQuery, List<WardDto>>
{
    private readonly IWardRepository _repository;

    public GetAllWardsQueryHandler(IWardRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WardDto>> Handle(GetAllWardsQuery request, CancellationToken cancellationToken)
    {
        var wards = await _repository.GetAllAsync(cancellationToken);

        return wards.Select(w => new WardDto
        {
            Id = w.Id,
            FloorId = w.FloorId,
            Name = w.Name,
            Type = w.Type,
            Description = w.Description

        }).ToList();
    }
}
