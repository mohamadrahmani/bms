using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Commands;

public class UpdatePointCommand : IRequest
{
    public Guid Id { get; set; }

    public UpdatePointDto Dto { get; set; }

    public UpdatePointCommand(Guid id, UpdatePointDto dto)
    {
        Id = id;
        Dto = dto;
    }
}
