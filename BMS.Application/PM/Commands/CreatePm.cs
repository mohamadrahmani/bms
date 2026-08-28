using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Pm.Commands
{
    public record CreatePmCommand(
        Guid DeviceId,
        string Title,
        string? Tag,
        string? Description,
        DateOnly DueDate,
        int WarningDays
    ) : IRequest<ApiResponse<Guid>>;
}