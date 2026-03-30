using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.Points.Handlers;

public class GetPointsQueryHandler : IRequestHandler<GetPointsQuery, PagedResult<PointDto>>
{
    private readonly IPointRepository _pointRepository;

    public GetPointsQueryHandler(IPointRepository pointRepository)
    {
        _pointRepository = pointRepository;
    }

    public async Task<PagedResult<PointDto>> Handle(GetPointsQuery request, CancellationToken cancellationToken)
    {
        var points = _pointRepository.Points.Include(p=> p.Device);

        var query = points.Select(p => new PointDto
        {
            Id = p.Id,
            DeviceId = p.DeviceId,
            DeviceName = p.Device.Name,
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
            PointType = p.PointType,
            BitIndex = p.BitIndex,
            ByteOrder = p.ByteOrder,
            SiteId = p.Location != null ? p.Location!.SiteId : null,
            BuildingId = p.Location != null ? p.Location!.BuildingId : null,
            FloorId = p.Location != null ? p.Location!.FloorId : null,
            WardId = p.Location != null ? p.Location!.WardId : null,
            RoomId = p.Location != null ? p.Location!.RoomId : null,
            Value = p.Value,
            Quality = p.Quality.ToString(),
            LastUpdatedAtUtc = p.LastUpdatedAtUtc
        });
        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
