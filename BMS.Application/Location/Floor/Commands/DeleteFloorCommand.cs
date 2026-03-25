using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Floors.Commands;

public class DeleteFloorCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
