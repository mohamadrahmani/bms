using BMS.Domain.Entities;

namespace BMS.Domain.Entities.Logs;

/// <summary>
/// Stores details about an application or HTTP request error.
/// </summary>
public sealed class SystemErrorLog : BaseEntity<Guid>
{
    public Guid ErrorId { get; set; } = Guid.NewGuid();
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }

    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
    public string? QueryString { get; set; }
    public string? RequestBody { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? StatusCode { get; set; }
    public Guid? UserId { get; set; }
    public string? EnvironmentName { get; set; }

    public bool IsResolved { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public Guid? ResolvedByUserId { get; set; }
    public string? ResolutionNote { get; set; }

    // Populated for display; not persisted as part of the error record.
    public string? UserName { get; set; }
}
