namespace BMS.Application.Common.Settings;

public sealed class SystemErrorLogOptions
{
    public const string SectionName = "SystemErrorLogs";

    public int RetentionDays { get; init; } = 180;
    public bool CleanupEnabled { get; init; } = true;
    public int CleanupHourUtc { get; init; } = 3;
    public int CleanupBatchSize { get; init; } = 1000;
    public int MaxRequestBodyLength { get; init; } = 32 * 1024;
    public int MaxStackTraceLength { get; init; } = 64 * 1024;
    public int MaxInnerExceptionLength { get; init; } = 16 * 1024;
}
