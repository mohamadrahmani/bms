using MediatR;

namespace BMS.Application.Controllers.Commands
{
    public class UpdateControllerCommand : IRequest
    {
        public Guid ControllerId { get; set; }

        public string Name { get; init; } = default!;
        public int TimeoutMs { get; init; }
        public int RetryCount { get; init; }
        public int ScanIntervalMs { get; init; }
        public string? Description { get; init; }
    }
}
