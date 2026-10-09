using BMS.Domain.Entities.BMS;

namespace BMS.Application.Points.Dtos;

public class PointCategoryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public DeviceType? DeviceType { get; set; }
    public int SortOrder { get; set; }
}
