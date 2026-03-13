using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class GetPointsByDeviceQueryHandler : IRequestHandler<GetPointsByDeviceQuery, IEnumerable<PointDto>>
{
    private readonly IPointRepository _repository;

    public GetPointsByDeviceQueryHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PointDto>> Handle(GetPointsByDeviceQuery request, CancellationToken cancellationToken)
    {
        var points = await _repository.GetByDeviceIdAsync(request.DeviceId);

        return points.Select(p => new PointDto
        {
            Id = p.Id,
            DeviceId = p.DeviceId,
            Title = p.Title,
            Kind = p.Kind,
            DataType = p.DataType,
            Address = p.Address,
            Value = p.Value
        });
    }
}
