namespace BMS.Domain.Entities.BMS;

public class PointCategory : BaseEntity<int>
{
    public string Code { get; set; } = default!;
    public string Title { get; set; } = default!;
    public DeviceType? DeviceType { get; set; }
    public int SortOrder { get; set; }
    public ICollection<Point> Points { get; set; } = new List<Point>();
}
