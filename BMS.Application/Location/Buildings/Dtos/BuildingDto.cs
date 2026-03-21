namespace BMS.Application.Location.Buildings.Dtos;

public class BuildingDto
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }
}
