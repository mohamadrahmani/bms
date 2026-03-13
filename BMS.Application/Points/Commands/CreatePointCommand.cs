using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Commands;

public class CreatePointCommand : IRequest<Guid>
{
    public CreatePointDto Dto { get; set; }

    public CreatePointCommand(CreatePointDto dto)
    {
        Dto = dto;
    }
}
