using Bms.Infrastructure.Seeds;
using BMS.Application.PreventiveMaintenance.Commands;
using BMS.Application.PreventiveMaintenance.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api")]
public sealed class PreventiveMaintenanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public PreventiveMaintenanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RequirePermission(PermissionKeys.PM.View)]
    [HttpGet("devices/{deviceId:guid}/pm/active")]
    public async Task<IActionResult> GetActive(
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetActivePmByDeviceQuery(deviceId),
            cancellationToken);
        return Ok(result);
    }

    [RequirePermission(PermissionKeys.PM.Manage)]
    [HttpPost("devices/{deviceId:guid}/pm")]
    public async Task<IActionResult> Create(
        Guid deviceId,
        [FromBody] CreatePmScheduleCommand command,
        CancellationToken cancellationToken)
    {
        command.DeviceId = deviceId;
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetActive), new { deviceId }, new { id });
    }

    [RequirePermission(PermissionKeys.PM.Manage)]
    [HttpPut("pm/{pmScheduleId:guid}")]
    public async Task<IActionResult> Update(
        Guid pmScheduleId,
        [FromBody] UpdatePmScheduleCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = pmScheduleId;
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [RequirePermission(PermissionKeys.PM.Manage)]
    [HttpPost("pm/{pmScheduleId:guid}/finalize")]
    public async Task<IActionResult> Finalize(
        Guid pmScheduleId,
        [FromBody] FinalizePmScheduleCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = pmScheduleId;
        var historyId = await _mediator.Send(command, cancellationToken);
        return Ok(new { historyId });
    }

    [RequirePermission(PermissionKeys.PM.View)]
    [HttpGet("devices/{deviceId:guid}/pm/history")]
    public async Task<IActionResult> GetHistory(
        Guid deviceId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPmHistoryByDeviceQuery
        {
            DeviceId = deviceId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [RequirePermission(PermissionKeys.PM.Manage)]
    [HttpPost("pm/{pmScheduleId:guid}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddScheduleAttachment(
        Guid pmScheduleId,
        [FromForm] PmAttachmentUploadRequest request,
        CancellationToken cancellationToken)
    {
        var content = await ReadFileAsync(request.File, cancellationToken);
        var id = await _mediator.Send(new AddPmScheduleAttachmentCommand
        {
            PmScheduleId = pmScheduleId,
            FileName = Path.GetFileName(request.File.FileName),
            ContentType = request.File.ContentType,
            FileContent = content,
            Description = request.Description
        }, cancellationToken);

        return CreatedAtAction(nameof(DownloadAttachment), new { attachmentId = id }, new { id });
    }

    [RequirePermission(PermissionKeys.PM.Manage)]
    [HttpPost("pm-history/{historyId:guid}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddHistoryAttachment(
        Guid historyId,
        [FromForm] PmAttachmentUploadRequest request,
        CancellationToken cancellationToken)
    {
        var content = await ReadFileAsync(request.File, cancellationToken);
        var id = await _mediator.Send(new AddPmHistoryAttachmentCommand
        {
            PmServiceHistoryId = historyId,
            FileName = Path.GetFileName(request.File.FileName),
            ContentType = request.File.ContentType,
            FileContent = content,
            Description = request.Description
        }, cancellationToken);

        return CreatedAtAction(nameof(DownloadAttachment), new { attachmentId = id }, new { id });
    }

    [RequirePermission(PermissionKeys.PM.View)]
    [HttpGet("pm-attachments/{attachmentId:guid}/download")]
    public async Task<IActionResult> DownloadAttachment(
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DownloadPmAttachmentQuery(attachmentId),
            cancellationToken);

        return File(result.Content, result.ContentType, result.FileName);
    }

    private static async Task<byte[]> ReadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Array.Empty<byte>();

        await using var stream = new MemoryStream((int)file.Length);
        await file.CopyToAsync(stream, cancellationToken);
        return stream.ToArray();
    }
}

public sealed class PmAttachmentUploadRequest
{
    public IFormFile File { get; set; } = default!;
    public string? Description { get; set; }
}
