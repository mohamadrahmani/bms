//using BMS.Application.Common.Interfaces;
//using BMS.Application.Points.Commands;
//using MediatR;

//namespace BMS.Application.Points.Handlers;

//public class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand>
//{
//    private readonly IPointRepository _repository;

//    public UpdatePointCommandHandler(IPointRepository repository)
//    {
//        _repository = repository;
//    }

//    public async Task<Unit> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
//    {
//        var point = await _repository.GetByIdAsync(request.Id);

//        if (point == null)
//            throw new Exception("Point not found");

//        var dto = request.Dto;

//        point.Update(
//            dto.Title,
//            dto.Kind,
//            dto.DataType,
//            dto.Address
//        );

//        await _repository.UpdateAsync(point);

//        return Unit.Value;
//    }
//}
using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Commands;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand>
{
    private readonly IPointRepository _repository;

    public UpdatePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
    {
        var point = await _repository.GetByIdAsync(request.Id);

        if (point == null)
            throw new Exception("Point not found");

        var location = new LocationReference(
            request.SiteId,
            request.BuildingId,
            request.FloorId,
            request.WardId,
            request.RoomId
        );

        point.Update(
            request.Tag,
            request.Title,
            request.Kind,
            request.DataType,
            request.Address,
            request.Unit,
            request.Code,
            request.Length,
            request.Scale,
            request.Offset,
            request.CommandAddress,
            request.FeedbackAddress,
            request.ValidationRetryCount,
            request.ValidationDelayMs,
            request.IsWritable,
            location
        );

        if (request.RegisterType.HasValue && request.RegisterAddress.HasValue)
        {
            point.SetMapping(
                request.RegisterType.Value,
                (int)request.RegisterAddress.Value,
                request.BitIndex,
                request.ByteOrder
            );
        }
        else
        {
            point.ClearMapping();
        }

        await _repository.UpdateAsync(point);

        return Unit.Value;
    }
}
