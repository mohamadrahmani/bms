using BMS.Application.Files.Models;
using BMS.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BMS.Application.Files.Commands;

public sealed record UploadFileCommand(
    IFormFile File,
    Guid EntityTypeId,
    Guid EntityId,
    string? Description)
    : IRequest<ApiResponse<FileDto>>;
