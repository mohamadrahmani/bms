using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Files.Commands;

public sealed record DeleteFileAttachmentCommand(Guid AttachmentId)
    : IRequest<ApiResponse<bool>>;
