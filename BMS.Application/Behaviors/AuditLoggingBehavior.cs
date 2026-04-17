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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLoggingBehavior(
        IAuditLogger auditLogger,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditLogger = auditLogger;
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


        try
        {
            var response = await next();

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
                ObjectId = ExtractObjectId(request),

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
                            ObjectId = ExtractObjectId(request),

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
}
