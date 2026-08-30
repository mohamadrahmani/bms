using System.Security.Cryptography;
using BMS.Application.Common.Interfaces;
using BMS.Application.Files.Commands;
using BMS.Application.Files.Models;
using BMS.Application.Files.Queries;
using BMS.Application.Models;
using BMS.Domain.Entities.Files;
using MediatR;
using FileEntity = BMS.Domain.Entities.Files.File;

namespace BMS.Application.Files.Handlers;

public sealed class ValidateFileCommandHandler
    : IRequestHandler<ValidateFileCommand, ApiResponse<FileValidationResult>>
{
    private readonly IFileRepository _repository;
    public ValidateFileCommandHandler(IFileRepository repository) => _repository = repository;

    public async Task<ApiResponse<FileValidationResult>> Handle(
        ValidateFileCommand request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetTypeWithConfigurationAsync(
            request.Request.EntityTypeId, cancellationToken);
        if (type?.Configuration is null)
            return ApiResponse<FileValidationResult>.SuccessResponse(
                new(false, "FILE_OPERATION_NOT_ALLOWED", "عملیات برای این نوع موجودیت مجاز نیست."));

        var result = FileValidationRules.ValidateMetadata(
            request.Request.FileName, request.Request.FileSize * 1024,
            request.Request.ContentType, request.Request.FileExtension, type.Configuration);
        return ApiResponse<FileValidationResult>.SuccessResponse(
            new(result.Valid, result.Code, result.Message));
    }
}

public sealed class UploadFileCommandHandler
    : IRequestHandler<UploadFileCommand, ApiResponse<FileDto>>
{
    private readonly IFileRepository _repository;
    public UploadFileCommandHandler(IFileRepository repository) => _repository = repository;

    public async Task<ApiResponse<FileDto>> Handle(
        UploadFileCommand request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetTypeWithConfigurationAsync(
            request.EntityTypeId, cancellationToken);
        if (type?.Configuration is null)
            return ApiResponse<FileDto>.FailResponse(
                new() { "FILE_OPERATION_NOT_ALLOWED" }, "عملیات برای این نوع موجودیت مجاز نیست.", 403);

        var metadata = FileValidationRules.ValidateMetadata(
            request.File.FileName, request.File.Length, request.File.ContentType,
            Path.GetExtension(request.File.FileName), type.Configuration);
        if (!metadata.Valid)
            return ApiResponse<FileDto>.FailResponse(
                new() { metadata.Code! }, metadata.Message!);

        var activeCount = await _repository.CountActiveAttachmentsAsync(
            request.EntityTypeId, request.EntityId, cancellationToken);
        if (activeCount >= type.Configuration.MaxFileCount)
            return ApiResponse<FileDto>.FailResponse(
                new() { "FILE_MAX_COUNT_EXCEEDED" }, "تعداد فایل‌های مجاز تکمیل شده است.");

        await using var stream = request.File.OpenReadStream();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken);
        var bytes = memory.ToArray();
        if (bytes.LongLength > (long)type.Configuration.MaxFileSize * 1024 ||
            !FileValidationRules.HasValidSignature(Path.GetExtension(request.File.FileName), bytes))
            return ApiResponse<FileDto>.FailResponse(
                new() { "FILE_SIGNATURE_INVALID" }, "محتوای فایل با فرمت اعلام‌شده مطابقت ندارد.");

        var file = new FileEntity(
            Path.GetFileName(request.File.FileName),
            request.File.ContentType,
            Path.GetExtension(request.File.FileName),
            bytes.LongLength,
            Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
            request.Description);
        var content = new FileContent(file.Id, bytes);
        var attachment = new FileAttachment(file.Id, request.EntityTypeId, request.EntityId);
        return await _repository.ExecuteInTransactionAsync(async () =>
        {
            await _repository.AddAsync(file, content, attachment, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ApiResponse<FileDto>.SuccessResponse(
                new(file.Id, attachment.Id, file.OriginalFileName, file.ContentType, file.FileExtension,
                    file.FileSize, file.Description, file.CreatedAtUtc), "فایل با موفقیت بارگذاری شد.");
        }, cancellationToken);
    }
}

public sealed class DeleteFileAttachmentCommandHandler
    : IRequestHandler<DeleteFileAttachmentCommand, ApiResponse<bool>>
{
    private readonly IFileRepository _repository;
    public DeleteFileAttachmentCommandHandler(IFileRepository repository) => _repository = repository;

    public async Task<ApiResponse<bool>> Handle(
        DeleteFileAttachmentCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAttachmentAsync(request.AttachmentId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "فایل با موفقیت حذف شد.");
    }
}

public sealed class UpdateFileDescriptionCommandHandler
    : IRequestHandler<UpdateFileDescriptionCommand, ApiResponse<bool>>
{
    private readonly IFileRepository _repository;

    public UpdateFileDescriptionCommandHandler(IFileRepository repository) =>
        _repository = repository;

    public async Task<ApiResponse<bool>> Handle(
        UpdateFileDescriptionCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Description?.Length > 2000)
        {
            return ApiResponse<bool>.FailResponse(
                new() { "FILE_DESCRIPTION_TOO_LONG" },
                "توضیحات فایل نمی‌تواند بیشتر از ۲۰۰۰ کاراکتر باشد.");
        }

        var updated = await _repository.UpdateDescriptionAsync(
            request.FileId, request.Description, cancellationToken);

        if (!updated)
        {
            return ApiResponse<bool>.FailResponse(
                new() { "FILE_NOT_FOUND" }, "فایل پیدا نشد.", 404);
        }

        await _repository.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse(true, "توضیحات فایل ذخیره شد.");
    }
}

public sealed class GetEntityFilesQueryHandler
    : IRequestHandler<GetEntityFilesQuery, ApiResponse<IReadOnlyList<FileDto>>>
{
    private readonly IFileRepository _repository;
    public GetEntityFilesQueryHandler(IFileRepository repository) => _repository = repository;

    public async Task<ApiResponse<IReadOnlyList<FileDto>>> Handle(
        GetEntityFilesQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetEntityFilesAsync(
            request.EntityTypeId, request.EntityId, cancellationToken);
        return ApiResponse<IReadOnlyList<FileDto>>.SuccessResponse(
            items.Select(x => new FileDto(x.Id, x.AttachmentId, x.OriginalFileName, x.ContentType,
                x.FileExtension, x.FileSize, x.Description, x.CreatedAtUtc)).ToList());
    }
}

public sealed class DownloadFileQueryHandler
    : IRequestHandler<DownloadFileQuery, ApiResponse<FileDownload?>>
{
    private readonly IFileRepository _repository;
    public DownloadFileQueryHandler(IFileRepository repository) => _repository = repository;

    public async Task<ApiResponse<FileDownload?>> Handle(
        DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetDownloadAsync(request.FileId, cancellationToken);
        if (item is null)
            return ApiResponse<FileDownload?>.FailResponse(new() { "FILE_NOT_FOUND" }, "فایل پیدا نشد.", 404);
        return ApiResponse<FileDownload?>.SuccessResponse(
            new FileDownload(item.FileName, item.ContentType, new MemoryStream(item.Content)));
    }
}
