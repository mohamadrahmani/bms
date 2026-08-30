using BMS.Application.Files.Models;
using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Files.Queries;

public sealed record GetEntityFilesQuery(Guid EntityTypeId, Guid EntityId)
    : IRequest<ApiResponse<IReadOnlyList<FileDto>>>;
