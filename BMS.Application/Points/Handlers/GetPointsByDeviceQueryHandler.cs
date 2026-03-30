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

        //return points.Select(p => new PointDto
        //{
        //    Id = p.Id,
        //    DeviceId = p.DeviceId,
        //    Title = p.Title,
        //    Kind = p.Kind,
        //    DataType = p.DataType,
        //    Address = p.Address,
        //    Value = p.Value
        //});
        return points.Select(p => new PointDto
        {
            Id = p.Id,
            DeviceId = p.DeviceId,
            Tag = p.Tag,
            Title = p.Title,
            Kind = p.Kind,
            DataType = p.DataType,
            Address = p.Address,
            Unit = p.Unit,
            Code = p.Code,
            Length = p.Length,
            Scale = p.Scale,
            Offset = p.Offset,
            IsWritable = p.IsWritable,
            RegisterType = p.RegisterType,
            BitIndex = p.BitIndex,
            ByteOrder = p.ByteOrder,
            SiteId = p.Location?.SiteId,
            BuildingId = p.Location?.BuildingId,
            FloorId = p.Location?.FloorId,
            WardId = p.Location?.WardId,
            RoomId = p.Location?.RoomId,
            Value = p.Value,
            Quality = p.Quality.ToString(),
            LastUpdatedAtUtc = p.LastUpdatedAtUtc
        });

    }
}
