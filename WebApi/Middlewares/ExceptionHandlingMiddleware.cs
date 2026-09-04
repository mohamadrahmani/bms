using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Settings;
using BMS.Domain.Exceptions;
using BMS.Domain.Entities.Logs;
using Microsoft.Extensions.Options;
using System.Security.Claims;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ISystemErrorLogWriter _systemErrorLogWriter;
    private readonly IOptionsMonitor<SystemErrorLogOptions> _options;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<ExceptionHandlingMiddleware> logger,
        ISystemErrorLogWriter systemErrorLogWriter,
        IOptionsMonitor<SystemErrorLogOptions> options)
    {
        _next = next;
        _env = env;
        _logger = logger;
        _systemErrorLogWriter = systemErrorLogWriter;
        _options = options;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        try
        {
            await _next(context);
        }

        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }

        catch (ArgumentException ex)
        {
            await HandleBadRequest(context, ex.Message);
        }

        catch (DomainException ex)
        {
            await HandleBadRequest(context, ex.Message);
        }

        catch (BusinessRuleException ex)
        {
            await HandleBadRequest(context, ex.Message);
        }

        catch (DbUpdateException ex)
        {
            await HandleDatabaseException(context, ex);
        }

        catch (Exception ex)
        {
            await HandleUnhandledException(context, ex);
        }
    }

    private async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { errors })
        );
    }

    private async Task HandleBadRequest(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new
            {
                errors = new[] { message }
            })
        );
    }

    private async Task HandleUnhandledException(HttpContext context, Exception ex)
    {
        var errorId = await PersistSystemErrorAsync(
            context,
            ex,
            (int)HttpStatusCode.InternalServerError);

        _logger.LogError(ex,
            "Unhandled exception occurred. ErrorId: {ErrorId}",
            errorId);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            errorId,
            message = _env.IsDevelopment()
                ? ex.Message
                : "خطای غیرمنتظره‌ای در سرور رخ داده است. شناسه خطا را با پشتیبانی در میان بگذارید."
        }));
    }

    private async Task HandleDatabaseException(
        HttpContext context,
        DbUpdateException ex)
    {
        var errorId = await PersistSystemErrorAsync(
            context,
            ex,
            (int)HttpStatusCode.BadRequest);

        _logger.LogError(ex,
            "Database exception occurred. ErrorId: {ErrorId}",
            errorId);

        await HandleBadRequest(
            context,
            "A database constraint violation occurred.");
    }

    private async Task<Guid> PersistSystemErrorAsync(
        HttpContext context,
        Exception ex,
        int statusCode)
    {
        var errorId = Guid.NewGuid();
        var options = _options.CurrentValue;
        var requestBody = await SystemErrorLogSanitizer.ReadRequestBodyAsync(
            context.Request,
            options,
            CancellationToken.None);

        await _systemErrorLogWriter.WriteAsync(new SystemErrorLog
        {
            ErrorId = errorId,
            OccurredAtUtc = DateTime.UtcNow,
            ExceptionType = SystemErrorLogSanitizer.Truncate(
                ex.GetType().FullName ?? ex.GetType().Name,
                512),
            Message = ex.Message,
            StackTrace = SystemErrorLogSanitizer.Truncate(
                ex.StackTrace,
                options.MaxStackTraceLength),
            InnerException = SystemErrorLogSanitizer.Truncate(
                ex.InnerException?.ToString(),
                options.MaxInnerExceptionLength),
            RequestPath = SystemErrorLogSanitizer.Truncate(
                context.Request.Path.Value,
                2048),
            HttpMethod = SystemErrorLogSanitizer.Truncate(
                context.Request.Method,
                16),
            QueryString = SystemErrorLogSanitizer.SanitizeQueryString(
                context.Request.QueryString.Value,
                options),
            RequestBody = requestBody,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = SystemErrorLogSanitizer.Truncate(
                context.Request.Headers.UserAgent.ToString(),
                1024),
            StatusCode = statusCode,
            UserId = GetUserId(context.User),
            EnvironmentName = _env.EnvironmentName
        }, CancellationToken.None);

        return errorId;
    }

    private static Guid? GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
