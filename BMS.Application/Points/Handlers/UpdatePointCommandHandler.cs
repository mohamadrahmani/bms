using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Application.Points.Commands;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand, ApiResponse<bool>>
{
    private readonly IPointRepository _repository;

    public UpdatePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
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
            request.PointType,
            request.RegisterType,
            request.DataType,
            request.Address,
            request.Unit,
            request.Code,
            request.Length,
            request.Scale,
            request.Offset,
            request.FeedbackAddress,
            request.ValidationRetryCount,
            request.ValidationDelayMs,
            request.IsWritable,
            location
        );

        //if (request.RegisterType && request.Address)
        {
            point.SetMapping(
                request.RegisterType,
                request.Address,
                request.BitIndex,
                request.ByteOrder
            );
        }
        //else
        //{
        //    point.ClearMapping();
        //}

        await _repository.UpdateAsync(point);

        var dto = new PointDto
        {
            Id = point.Id,
        };

        return ApiResponse<bool>.SuccessResponse(true, "Device updated successfully");
    }
}
