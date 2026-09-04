using BMS.Application.Files.Commands;
using BMS.Application.Files.Models;
using BMS.Application.Files.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BMS.Api.Controllers;

[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    public FilesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(
        [FromBody] FileValidationRequest request, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new ValidateFileCommand(request), cancellationToken));

    [HttpPost]
    [RequestSizeLimit(2_500_000)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] Guid entityTypeId,
        [FromForm] Guid entityId,
        [FromForm] string? description,
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(
            new UploadFileCommand(file, entityTypeId, entityId, description),
            cancellationToken));

    [HttpGet("entity/{entityTypeId:guid}/{entityId:guid}")]
    public async Task<IActionResult> GetEntityFiles(
        Guid entityTypeId, Guid entityId, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(
            new GetEntityFilesQuery(entityTypeId, entityId), cancellationToken));

    [HttpGet("{fileId:guid}/download")]
    public async Task<IActionResult> Download(
        Guid fileId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new DownloadFileQuery(fileId), cancellationToken);
        if (!response.Success || response.Data is null)
            return NotFound(response);

        return File(response.Data.Content, response.Data.ContentType, response.Data.FileName);
    }

    [HttpDelete("attachments/{attachmentId:guid}")]
    public async Task<IActionResult> Delete(
        Guid attachmentId, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(
            new DeleteFileAttachmentCommand(attachmentId), cancellationToken));

    [HttpPut("{fileId:guid}/description")]
    public async Task<IActionResult> UpdateDescription(
        Guid fileId,
        [FromBody] UpdateFileDescriptionRequest request,
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(
            new UpdateFileDescriptionCommand(fileId, request.Description),
            cancellationToken));
}
