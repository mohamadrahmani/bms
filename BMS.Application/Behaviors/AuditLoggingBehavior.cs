using System.Diagnostics;
using System.Text.Json;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using BMS.Domain.Entities.Logs;
using BMS.Application.Attributes;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Logging;


public class AuditLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditLogger _auditLogger;
    private readonly ILogRepository _logRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLoggingBehavior(
        IAuditLogger auditLogger,
        ILogRepository logRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditLogger = auditLogger;
        _logRepository = logRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var auditAttr = typeof(TRequest)
            .GetCustomAttributes(typeof(AuditAttribute), false)
            .FirstOrDefault() as AuditAttribute;

        if (auditAttr == null)
            return await next();

        var http = _httpContextAccessor.HttpContext;
        var stopwatch = Stopwatch.StartNew();


        Guid? userId = null;

        var userIdStr =
            http?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? http?.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdStr, out var parsed))
            userId = parsed;

        var ip =
            http?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        var userAgent =
            http?.Request?.Headers["User-Agent"].ToString();

        var rawBody = JsonSerializer.Serialize(request);
        var requestBody = LogSanitizer.Sanitize(rawBody);

        var objectId = ExtractObjectId(request);
        string? deletedObjectName = null;
        if (auditAttr.EventType == EventType.DeleteData && Guid.TryParse(objectId, out var deletedId))
            deletedObjectName = await _logRepository.GetObjectDisplayNameAsync(auditAttr.ObjectName, deletedId, cancellationToken);


        try
        {
            var response = await next();

            if (auditAttr.EventType == EventType.AddData)
            {
                var createdId = response is Guid guid ? guid : response?.GetType().GetProperty("Data")?.GetValue(response);
                if (createdId is Guid id && id != Guid.Empty)
                    objectId = id.ToString();
            }

            var objectDisplayName = deletedObjectName ?? ExtractObjectDisplayName(request);
            if (string.IsNullOrWhiteSpace(objectDisplayName) && Guid.TryParse(objectId, out var recordId))
                objectDisplayName = await _logRepository.GetObjectDisplayNameAsync(auditAttr.ObjectName, recordId, cancellationToken);
            objectDisplayName = TruncateDisplayName(objectDisplayName);

            stopwatch.Stop();

            var statusCode = http?.Response?.StatusCode;
            //****************

            // =============================
            // 1) ایجاد Parent Log
            // =============================
            var parentLog = new Log
            {
                UserId = userId,
                EventType = auditAttr.EventType,
                ObjectName = auditAttr.ObjectName,
                ObjectId = objectId,
                ObjectDisplayName = objectDisplayName,

                Result = OperationResult.Success,
                ResultMessage = "عملیات با موفقیت انجام شد",
                LogDate = DateTime.UtcNow,
                IpAddress = ip,
                Source = "API",

                RequestBody = requestBody,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                UserAgent = userAgent,
                StatusCode = statusCode
            };

            // ثبت لاگ اصلی
            await _auditLogger.Add(parentLog);

            // =============================
            // 2) ثبت Child Logs (تغییر فیلدها)
            // =============================
            var changesProp = request.GetType().GetProperty("Changes");

            if (changesProp != null)
            {
                var changes = changesProp.GetValue(request)
                    as IEnumerable<(string Field, string? OldValue, string? NewValue)>;

                if (changes != null && changes.Any())
                {
                    foreach (var change in changes)
                    {
                        var childLog = new Log
                        {
                            ParentLogId = parentLog.Id,

                            UserId = userId,
                            EventType = auditAttr.EventType,
                            ObjectName = auditAttr.ObjectName,
                            ObjectId = objectId,
                            ObjectDisplayName = objectDisplayName,

                            FieldName = change.Field,
                            OldValue = change.OldValue,
                            NewValue = change.NewValue,

                            Result = OperationResult.Success,
                            ResultMessage = "عملیات با موفقیت انجام شد",
                            LogDate = DateTime.UtcNow,
                            IpAddress = ip,
                            Source = "API",
                            UserAgent = userAgent,
                            StatusCode = statusCode
                        };

                        await _auditLogger.Add(childLog);
                    }
                }
            }
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var statusCode = http?.Response?.StatusCode;
            // خطای Parent Log
            await _auditLogger.Add(new Log
            {
                UserId = userId,
                EventType = auditAttr.EventType,
                ObjectName = auditAttr.ObjectName,
                ObjectId = ExtractObjectId(request),
                ObjectDisplayName = TruncateDisplayName(deletedObjectName ?? ExtractObjectDisplayName(request)),
                Result = OperationResult.Error,
                ResultMessage = $"خطا: {ex.Message}",
                LogDate = DateTime.UtcNow,
                IpAddress = ip,
                Source = "API",

                RequestBody = requestBody,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                UserAgent = userAgent,
                StatusCode = statusCode
            });

            throw;
        }
    }

    private string ExtractObjectId(TRequest request)
    {

        var type = request.GetType();

        var prop =
            type.GetProperty("Id") ??
            type.GetProperty("UserId") ??
            type.GetProperty("ControllerId") ??
            type.GetProperty("DeviceId") ??
            type.GetProperty("PointId") ??
            type.GetProperty("SiteId") ??
            type.GetProperty("BuildingId") ??
            type.GetProperty("FloorId") ??
            type.GetProperty("WardId") ??
            type.GetProperty("RoomId");


        var value = prop?.GetValue(request);

        return value?.ToString();
    }

    private static string? ExtractObjectDisplayName(TRequest request)
    {
        var type = request.GetType();
        var name = type.GetProperty("Name")?.GetValue(request)?.ToString()
            ?? type.GetProperty("Title")?.GetValue(request)?.ToString()
            ?? type.GetProperty("UserName")?.GetValue(request)?.ToString();

        if (string.IsNullOrWhiteSpace(name))
        {
            var firstName = type.GetProperty("FirstName")?.GetValue(request)?.ToString();
            var lastName = type.GetProperty("LastName")?.GetValue(request)?.ToString();
            name = string.Join(" ", new[] { firstName, lastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        return TruncateDisplayName(name);
    }

    private static string? TruncateDisplayName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        name = name.Trim();
        return name.Length > 300 ? name[..300] : name;
    }
}
