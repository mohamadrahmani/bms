using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Points.Commands;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class CreatePointCommandHandler : IRequestHandler<CreatePointCommand, ApiResponse<Guid>>
{
    private readonly IPointRepository _repository;

    public CreatePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<Guid>> Handle(CreatePointCommand request, CancellationToken cancellationToken)
    {
        LocationReference? location = null;
        if (request.SiteId.HasValue)
        {
            location = new LocationReference(
                request.SiteId,
                request.BuildingId,
                request.FloorId,
                request.WardId,
                request.RoomId
            );
        }
        var point = new Point(
           request.DeviceId,
           request.Kind,
           request.Address,
           request.Tag,
           request.Title,
           request.DataType,
           request.PointType,
           request.Unit,
           location,
        request.Code,
        request.Length,
        request.Scale,
        request.Offset,
        request.FeedbackAddress,
        request.ValidationRetryCount,
        request.ValidationDelayMs,
        request.IsWritable
       );
        // تنظیم Mapping (رجیستر PLC)
        if (request.RegisterType.HasValue && request.RegisterAddress.HasValue)
        {
            point.SetMapping(
                request.RegisterType.Value,
                request.Address,
                request.BitIndex,
                request.ByteOrder
            );
        }
        await _repository.AddAsync(point);
        return ApiResponse<Guid>.SuccessResponse(point.Id, "پوینت ایجاد شد"); ;
    }
}
