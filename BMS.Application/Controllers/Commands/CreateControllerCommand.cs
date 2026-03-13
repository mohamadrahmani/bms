using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Controllers.Commands
{
    public class CreateControllerCommand : IRequest<Guid>
    {
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public ControllerProtocol Protocol { get; init; }
        public string IpAddress { get; init; } = default!;
        public int Port { get; init; }
        public byte UnitId { get; init; }
        public int TimeoutMs { get; init; }
        public int RetryCount { get; init; }
        public int ScanIntervalMs { get; init; }
        public string? Description { get; init; }
    }
}
