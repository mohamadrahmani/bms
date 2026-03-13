using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class GetPointsQueryHandler : IRequestHandler<GetPointsQuery, List<PointDto>>
{
    private readonly IPointRepository _pointRepository;

    public GetPointsQueryHandler(IPointRepository pointRepository)
    {
        _pointRepository = pointRepository;
    }

    public async Task<List<PointDto>> Handle(GetPointsQuery request, CancellationToken cancellationToken)
    {
        var points = await _pointRepository.GetAllAsync(cancellationToken);

        return points.Select(p => new PointDto
        {
            Id = p.Id,
            DeviceId = p.DeviceId,
            Title = p.Title,
            Kind = p.Kind,
            DataType = p.DataType,
            Address = p.Address,
            Unit = p.Unit,
            Value = p.Value,
            Quality = p.Quality.ToString(),
            LastUpdatedAtUtc = p.LastUpdatedAtUtc
        }).ToList();
    }
}
