using BMS.Application.Files.Models;
using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Files.Commands;

public sealed record ValidateFileCommand(FileValidationRequest Request)
    : IRequest<ApiResponse<FileValidationResult>>;
