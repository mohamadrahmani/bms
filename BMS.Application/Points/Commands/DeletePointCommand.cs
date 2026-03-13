using MediatR;

namespace BMS.Application.Points.Commands;

public class DeletePointCommand : IRequest
{
    public Guid Id { get; set; }

    public DeletePointCommand(Guid id)
    {
        Id = id;
    }
}
