using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Points.Dtos;
using BMS.Application.Points.Queries;
using BMS.Domain.Entities.BMS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PointCategoryCode = BMS.Domain.Enums.PointCategory;

namespace BMS.Application.Points.Handlers;

public class GetPointCategoriesQueryHandler : IRequestHandler<GetPointCategoriesQuery, PagedResult<PointCategoryDto>>
{
    private readonly IPointCategoryRepository _categories;
    private readonly IDeviceRepository _devices;

    public GetPointCategoriesQueryHandler(IPointCategoryRepository categories, IDeviceRepository devices)
    {
        _categories = categories;
        _devices = devices;
    }

    public async Task<PagedResult<PointCategoryDto>> Handle(GetPointCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = _categories.Categories.AsNoTracking();
        if (request.DeviceId.HasValue)
        {
            var deviceType = await _devices.Devices
                .Where(device => device.Id == request.DeviceId.Value)
                .Select(device => (DeviceType?)device.Type)
                .FirstOrDefaultAsync(cancellationToken);
            if (!deviceType.HasValue)
                throw new ArgumentException("Device not found.");

            categories = categories.Where(category => category.DeviceType == null || category.DeviceType == deviceType.Value);
        }

        var result = await categories
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Title)
            .Select(category => new PointCategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Title = category.Title,
                DeviceType = category.DeviceType,
                SortOrder = category.SortOrder
            })
            .ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);

        if (result.Items.Any(category => global::System.Enum.GetName(typeof(PointCategoryCode), category.Id) != category.Code))
            throw new ArgumentException("Point category codes and IDs must match the PointCategory enum.");

        return result;
    }
}
