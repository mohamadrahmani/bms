namespace BMS.Application.Location.Wards.Dtos;

public class WardDto
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public string Name { get; set; } = default!;
    public string Type { get; set; }
    public string? Description { get; set; }
}
