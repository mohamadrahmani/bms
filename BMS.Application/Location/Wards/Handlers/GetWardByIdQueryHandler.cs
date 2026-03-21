using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Wards.Dtos;
using BMS.Application.Location.Wards.Queries;
using MediatR;

namespace BMS.Application.Location.Wards.Handlers;

public class GetWardByIdQueryHandler : IRequestHandler<GetWardByIdQuery, WardDto?>
{
    private readonly IWardRepository _repository;

    public GetWardByIdQueryHandler(IWardRepository repository)
    {
        _repository = repository;
    }

    public async Task<WardDto?> Handle(GetWardByIdQuery request, CancellationToken cancellationToken)
    {
        var ward = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (ward == null)
            return null;

        return new WardDto
        {
            Id = ward.Id,
            FloorId = ward.FloorId,
            Name = ward.Name,
            Type = ward.Type,
            Description = ward.Description
        };
    }
}
