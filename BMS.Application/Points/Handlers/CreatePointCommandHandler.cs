using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Commands;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class CreatePointCommandHandler : IRequestHandler<CreatePointCommand, Guid>
{
    private readonly IPointRepository _repository;

    public CreatePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePointCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var point = new Point(
    dto.DeviceId,
    dto.Kind,
    dto.Address,
    dto.Tag,
    dto.Title,
    dto.DataType,
    dto.Unit
);


        await _repository.AddAsync(point);

        return point.Id;
    }
}
