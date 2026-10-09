using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using PointCategoryCode = BMS.Domain.Enums.PointCategory;

namespace BMS.Application.Points;

public static class PointCategoryAssignment
{
    public static async Task<int> ValidateAsync(
        int? requestedCategoryId,
        Guid deviceId,
        IPointCategoryRepository categories,
        IDeviceRepository devices,
        CancellationToken cancellationToken)
    {
        var categoryId = requestedCategoryId.GetValueOrDefault() == 0
            ? (int)PointCategoryCode.General
            : requestedCategoryId.Value;

        var deviceType = await devices.Devices
            .Where(device => device.Id == deviceId)
            .Select(device => (DeviceType?)device.Type)
            .FirstOrDefaultAsync(cancellationToken);
        if (!deviceType.HasValue)
            throw new ArgumentException("Device not found.");

        var category = await categories.Categories
            .Where(item => item.Id == categoryId)
            .Select(item => new { item.Code, item.DeviceType })
            .FirstOrDefaultAsync(cancellationToken);
        if (category == null || category.DeviceType != null && category.DeviceType != deviceType.Value)
            throw new ArgumentException("Point category does not apply to this device type.");

        if (global::System.Enum.GetName(typeof(PointCategoryCode), categoryId) != category.Code)
            throw new ArgumentException("Point category code and ID must match the PointCategory enum.");

        return categoryId;
    }
}
