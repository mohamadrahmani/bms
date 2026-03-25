using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Buildings.Commands;

public sealed class DeleteBuildingCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
