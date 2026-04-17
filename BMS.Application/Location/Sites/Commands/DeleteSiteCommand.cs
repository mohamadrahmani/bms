using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Sites.Commands;

[Audit(EventType.DeleteData, "Sites")]
public sealed class DeleteSiteCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
