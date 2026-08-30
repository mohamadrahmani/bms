using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Files.Commands;

public sealed record UpdateFileDescriptionRequest(string? Description);

public sealed record UpdateFileDescriptionCommand(
    Guid FileId,
    string? Description) : IRequest<ApiResponse<bool>>;
