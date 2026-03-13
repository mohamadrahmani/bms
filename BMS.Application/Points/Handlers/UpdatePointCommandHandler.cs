using BMS.Application.Common.Interfaces;
using BMS.Application.Points.Commands;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class UpdatePointCommandHandler : IRequestHandler<UpdatePointCommand>
{
    private readonly IPointRepository _repository;

    public UpdatePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdatePointCommand request, CancellationToken cancellationToken)
    {
        var point = await _repository.GetByIdAsync(request.Id);

        if (point == null)
            throw new Exception("Point not found");

        var dto = request.Dto;

        point.Update(
            dto.Title,
            dto.Kind,
            dto.DataType,
            dto.Address
        );

        await _repository.UpdateAsync(point);

        return Unit.Value;
    }
}
