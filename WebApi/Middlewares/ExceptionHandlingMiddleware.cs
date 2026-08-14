using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
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

        catch (NotFoundException ex)
        {
            await HandleError(context, HttpStatusCode.NotFound, ex.Message);
        }

        catch (ConflictException ex)
        {
            await HandleError(context, HttpStatusCode.Conflict, ex.Message);
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
            await HandleBadRequest(context, "A database constraint violation occurred.");
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

    private static async Task HandleError(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new
            {
                errors = new[] { message }
            }));
    }

    private async Task HandleUnhandledException(HttpContext context, Exception ex)
    {
        var errorId = Guid.NewGuid();

        _logger.LogError(ex,
            "Unhandled exception occurred. ErrorId: {ErrorId}",
            errorId);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        if (_env.IsDevelopment())
        {
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errorId,
                    message = ex.Message,
                    //stackTrace = ex.StackTrace,
                    //innerException = ex.InnerException?.Message
                })
            );
        }
        else
        {
            // todo:
            //await context.Response.WriteAsync(
            //    JsonSerializer.Serialize(new
            //    {
            //        errorId,
            //        errors = new[]
            //        {
            //            "An unexpected error occurred. Please contact support with the provided error id."
            //        }
            //    })
            //);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errorId,
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                })
            );
        }
    }
}
