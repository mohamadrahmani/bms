namespace BMS.Application.SystemErrorLogs.Models;

public sealed class SystemErrorLogListItemDto
{
    public Guid Id { get; init; }
    public Guid ErrorId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public string ExceptionType { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string? RequestPath { get; init; }
    public int? StatusCode { get; init; }
    public Guid? UserId { get; init; }
    public bool IsResolved { get; init; }
}

public sealed class SystemErrorLogDetailsDto
{
    public Guid Id { get; init; }
    public Guid ErrorId { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public string ExceptionType { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string? StackTrace { get; init; }
    public string? InnerException { get; init; }
    public string? RequestPath { get; init; }
    public string? HttpMethod { get; init; }
    public string? QueryString { get; init; }
    public string? RequestBody { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public int? StatusCode { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? EnvironmentName { get; init; }
    public bool IsResolved { get; init; }
    public DateTime? ResolvedAtUtc { get; init; }
    public Guid? ResolvedByUserId { get; init; }
    public string? ResolutionNote { get; init; }
}
