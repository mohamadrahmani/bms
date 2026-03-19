namespace BMS.Application.Location.Sites.Dtos;

public sealed class SiteDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string? Address { get; init; }

    public string? Description { get; init; }
}
