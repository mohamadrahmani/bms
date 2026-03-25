using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Points.Commands;

public class DeletePointCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }

    public DeletePointCommand(Guid id)
    {
        Id = id;
    }
}
