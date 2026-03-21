namespace BMS.Application.Location.Rooms.Dtos;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public Guid WardId { get; set; }
    public string Name { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public string? Type { get; set; }
    public double Area { get; set; }
}
