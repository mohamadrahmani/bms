using BMS.Application.Common.Interfaces;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Application.Points.Commands;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Location;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BMS.Application.Points.Handlers;

public sealed class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand, ApiResponse<bool>>
{
    private readonly IPointRepository _repository;
    private readonly IMemoryCache _memoryCache;

    public UpdatePointCommandHandler(
        IPointRepository repository,
        IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public async Task<ApiResponse<bool>> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
    {
        var point = await _repository.GetByIdAsync(request.Id);

        if (point == null)
            throw new Exception("Point not found");
        //************************
        void TrackChange(string field, object? oldVal, object? newVal)
        {
            if ((oldVal?.ToString() ?? "") != (newVal?.ToString() ?? ""))
            {
                request.Changes.Add((field, oldVal?.ToString(), newVal?.ToString()));
            }
        }
        var location = new LocationReference(
           request.SiteId,
           request.BuildingId,
           request.FloorId,
           request.WardId,
           request.RoomId
       );
        var storeHistory = request.StoreHistory ?? point.StoreHistory;
        TrackChange(nameof(point.Tag), point.Tag, request.Tag);
        TrackChange(nameof(point.Title), point.Title, request.Title);
        TrackChange(nameof(point.Kind), point.Kind, request.Kind);
        TrackChange(nameof(point.DataType), point.DataType, request.DataType);
        TrackChange(nameof(point.Address), point.Address, request.Address);
        TrackChange(nameof(point.Unit), point.Unit, request.Unit);
        TrackChange(nameof(point.Code), point.Code, request.Code);
        TrackChange(nameof(point.Length), point.Length, request.Length);
        TrackChange(nameof(point.Scale), point.Scale, request.Scale);
        TrackChange(nameof(point.Offset), point.Offset, request.Offset);
        TrackChange(nameof(point.FeedbackAddress), point.FeedbackAddress, request.FeedbackAddress);
        TrackChange(nameof(point.ValidationRetryCount), point.ValidationRetryCount, request.ValidationRetryCount);
        TrackChange(nameof(point.ValidationDelayMs), point.ValidationDelayMs, request.ValidationDelayMs);
        TrackChange(nameof(point.IsWritable), point.IsWritable, request.IsWritable);
        TrackChange(nameof(point.StoreHistory), point.StoreHistory, storeHistory);

        //*************************
        //var location = new LocationReference(
        //    request.SiteId,
        //    request.BuildingId,
        //    request.FloorId,
        //    request.WardId,
        //    request.RoomId
        //);

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
            location,
            request.CommandDefinitionId,
            storeHistory
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
        _memoryCache.Remove($"point_{point.Id}");

        var dto = new PointDto
        {
            Id = point.Id,
        };

        return ApiResponse<bool>.SuccessResponse(true, "Device updated successfully");
    }
}
