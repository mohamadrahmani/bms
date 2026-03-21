using MediatR;

namespace BMS.Application.Location.Buildings.Commands;

public sealed class DeleteBuildingCommand : IRequest
{
    public Guid Id { get; set; }
}
