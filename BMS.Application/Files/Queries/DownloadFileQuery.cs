using BMS.Application.Files.Models;
using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Files.Queries;

public sealed record DownloadFileQuery(Guid FileId)
    : IRequest<ApiResponse<FileDownload?>>;
