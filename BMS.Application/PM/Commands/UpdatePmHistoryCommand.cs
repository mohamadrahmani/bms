using BMS.Application.Models;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.Pm.Commands;

public sealed class UpdatePmHistoryCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public PmServiceStatus Status { get; set; }
    public DateTime ActionDateUtc { get; set; }
    public string? Description { get; set; }
}
