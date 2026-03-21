using MediatR;

namespace BMS.Application.Location.Floors.Commands;

public class DeleteFloorCommand : IRequest
{
    public Guid Id { get; set; }
}
