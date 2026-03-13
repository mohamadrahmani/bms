using FluentValidation;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Exceptions;
//using BMS.Core.Exceptions;   // ⬅ اینو اضافه کن

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }

        // 🔹 FluentValidation
        catch (ValidationException ex)
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

        catch (ArgumentException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errors = new[] { ex.Message }
                })
            );
        }

        // 🔹 Domain Rules (NEW)
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errors = new[] { ex.Message }
                })
            );
        }

        // 🔹 Business Rules
        catch (BusinessRuleException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errors = new[] { ex.Message }
                })
            );
        }

        // 🔹 Database constraint errors
        catch (DbUpdateException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    errors = new[]
                    {
                        "A database constraint violation occurred."
                    }
                })
            );
        }

        // 🔹 Unhandled errors
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        }
    }
}
