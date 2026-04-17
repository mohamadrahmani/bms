namespace BMS.Application.Location.Floors.Dtos;

public class FloorDto
{
    public Guid Id { get; set; }

    public Guid BuildingId { get; set; }

    public string Name { get; set; } = default!;

    public int LevelNumber { get; set; }

    public string? Description { get; set; }
}
