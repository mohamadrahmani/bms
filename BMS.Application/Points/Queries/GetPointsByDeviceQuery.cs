using BMS.Application.Points.Dtos;
using MediatR;

namespace BMS.Application.Points.Queries;

public class GetPointsByDeviceQuery : IRequest<IEnumerable<PointDto>>
{
    public Guid DeviceId { get; set; }

    public GetPointsByDeviceQuery(Guid deviceId)
    {
        DeviceId = deviceId;
    }
}
