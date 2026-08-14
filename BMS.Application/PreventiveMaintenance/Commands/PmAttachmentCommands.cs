using MediatR;

namespace BMS.Application.PreventiveMaintenance.Commands;

public sealed class AddPmScheduleAttachmentCommand : IRequest<Guid>
{
    public Guid PmScheduleId { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string? Description { get; set; }
}

public sealed class AddPmHistoryAttachmentCommand : IRequest<Guid>
{
    public Guid PmServiceHistoryId { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string? Description { get; set; }
}
