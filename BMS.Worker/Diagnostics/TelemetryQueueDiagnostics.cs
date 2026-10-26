using BMS.Application.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace BMS.Worker.Diagnostics;

/// <summary>
/// Worker-only telemetry queue diagnostics. Does not alter queue delivery behavior.
/// </summary>
public static class TelemetryQueueDiagnostics
{
    private static readonly ConcurrentDictionary<TelemetryMessage, long> EnqueuedAt =
        new(ReferenceEqualityComparer.Instance);

    private static long _droppedOldest;

    public static long DroppedOldest => Interlocked.Read(ref _droppedOldest);

    public static void TrackEnqueued(TelemetryMessage message)
    {
        EnqueuedAt[message] = Stopwatch.GetTimestamp();
    }

    public static void TrackWriteFailed(TelemetryMessage message)
    {
        EnqueuedAt.TryRemove(message, out _);
    }

    public static void OnDropped(TelemetryMessage message)
    {
        EnqueuedAt.TryRemove(message, out _);
        Interlocked.Increment(ref _droppedOldest);
    }

    public static double? TrackDequeuedAndGetAgeMs(TelemetryMessage message)
    {
        if (!EnqueuedAt.TryRemove(message, out var timestamp))
            return null;

        return Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
    }

    public static int PendingTracked => EnqueuedAt.Count;
}
