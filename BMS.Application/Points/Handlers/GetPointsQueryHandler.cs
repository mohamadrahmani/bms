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
        var points = _pointRepository.Points;//.Include(p=> p.Device);
        // Device filter
        if (request.DeviceId.HasValue)
            points = points.Where(p => p.DeviceId == request.DeviceId.Value);

        // Controller filter
        if (request.ControllerId.HasValue)
            points = points.Where(p => p.Device != null && p.Device.ControllerId == request.ControllerId);

        // Location filters
        if (request.SiteId.HasValue)
            points = points.Where(p => p.Location != null && p.Location.SiteId == request.SiteId);

        if (request.BuildingId.HasValue)
            points = points.Where(p => p.Location != null && p.Location.BuildingId == request.BuildingId);

        if (request.FloorId.HasValue)
            points = points.Where(p => p.Location != null && p.Location.FloorId == request.FloorId);

        if (request.WardId.HasValue)
            points = points.Where(p => p.Location != null && p.Location.WardId == request.WardId);

        if (request.RoomId.HasValue)
            points = points.Where(p => p.Location != null && p.Location.RoomId == request.RoomId);

        // Kind
        if (request.Kind.HasValue)
            points = points.Where(p => p.Kind == request.Kind);

        // DataType
        if (request.DataType.HasValue)
            points = points.Where(p => p.DataType == request.DataType);

        // فیلترهای داینامیک
        points = points.ApplyDynamicFilters(request.Filters);

        // Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            points = points.Where(p =>
                p.Tag.Contains(request.Search)
                || p.Title.Contains(request.Search)
                || p.Code.Contains(request.Search)
                || p.Address.ToString() == request.Search
                || p.Device.Name.Contains(request.Search));
        }
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
            StoreHistory = p.StoreHistory,
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
            LastUpdatedAtUtc = p.LastUpdatedAtUtc,
            CommandDefinitionId = p.CommandDefinitionId
        });
        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
