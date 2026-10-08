using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class GetPointByIdQueryHandler : IRequestHandler<GetPointByIdQuery, PointDto?>
{
    private readonly IPointRepository _repository;

    public GetPointByIdQueryHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<PointDto?> Handle(GetPointByIdQuery request, CancellationToken cancellationToken)
    {
        var point = await _repository.GetByIdAsync(request.Id);

        if (point == null)
            return null;

        //return new PointDto
        //{
        //    Id = point.Id,
        //    DeviceId = point.DeviceId,
        //    Title = point.Title,
        //    Kind = point.Kind,
        //    DataType = point.DataType,
        //    Address = point.Address,
        //    Value = point.Value
        //};
        return new PointDto
        {
            Id = point.Id,
            DeviceId = point.DeviceId,
            Tag = point.Tag,
            Title = point.Title,
            Kind = point.Kind,
            DataType = point.DataType,
            Address = point.Address,
            Unit = point.Unit,
            Code = point.Code,
            Length = point.Length,
            Scale = point.Scale,
            Offset = point.Offset,
            IsWritable = point.IsWritable,
            StoreHistory = point.StoreHistory,
            RegisterType = point.RegisterType,
            BitIndex = point.BitIndex,
            ByteOrder = point.ByteOrder,
            SiteId = point.Location?.SiteId,
            BuildingId = point.Location?.BuildingId,
            FloorId = point.Location?.FloorId,
            WardId = point.Location?.WardId,
            RoomId = point.Location?.RoomId,
            Value = point.Value,
            Quality = point.Quality.ToString(),
            LastUpdatedAtUtc = point.LastUpdatedAtUtc
        };

    }
}
