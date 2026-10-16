using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Points.Commands;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;
using PointCategoryCode = BMS.Domain.Enums.PointCategory;

namespace BMS.Application.Points.Handlers;

public class CreatePointCommandHandler : IRequestHandler<CreatePointCommand, ApiResponse<Guid>>
{
    private readonly IPointRepository _repository;
    private readonly IPointCategoryRepository _categories;
    private readonly IDeviceRepository _devices;

    public CreatePointCommandHandler(IPointRepository repository, IPointCategoryRepository categories, IDeviceRepository devices)
    {
        _repository = repository;
        _categories = categories;
        _devices = devices;
    }

    public async Task<ApiResponse<Guid>> Handle(CreatePointCommand request, CancellationToken cancellationToken)
    {
        var scale = request.Scale;
        var offset = request.Offset;
        if (request.DataType != PointDataType.Boolean)
        {
            var scaler = new BMS.Application.Common.Settings.LinearScaler(
                request.MinDisplayValue!.Value,
                request.MaxDisplayValue!.Value,
                request.MinRawValue!.Value,
                request.MaxRawValue!.Value);
            scale = scaler.Scale;
            offset = scaler.Offset;
        }

        var categoryId = await PointCategoryAssignment.ValidateAsync(
            request.PointCategoryId, request.DeviceId, _categories, _devices, cancellationToken);
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
           request.RegisterType,
           request.Unit,
           location,
        request.Code,
        request.Length,
        scale,
        offset,
        request.FeedbackAddress,
        request.ValidationRetryCount,
        request.ValidationDelayMs,
        request.IsWritable,
        request.CommandDefinitionId,
        request.StoreHistory,
        (PointCategoryCode)categoryId,
        request.MinRawValue,
        request.MaxRawValue,
        request.MinDisplayValue,
        request.MaxDisplayValue
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
