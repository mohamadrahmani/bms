using System;
using System.Collections.Generic;
using System.Linq;
using BMS.Domain.Entities;
using BMS.Domain.Events;
using BMS.Domain.Events;

namespace BMS.Domain.Entities
{
    public class Device
    {
        private readonly object _sync = new();

        private readonly Dictionary<string, DataPoint> _points =
            new(StringComparer.OrdinalIgnoreCase);

        public string Id { get; }

        public Device(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Device id cannot be empty.", nameof(id));

            Id = id;
        }

        // -----------------------------
        // DataPoint Registration
        // -----------------------------

        public void RegisterPoint(
            string pointId,
            DataType dataType,
            object? initialValue = null)
        {
            lock (_sync)
            {
                if (_points.ContainsKey(pointId))
                    throw new InvalidOperationException(
                        $"Point '{pointId}' already exists in device '{Id}'.");

                var point = new DataPoint(
                    pointId,
                    dataType,
                    initialValue);

                _points.Add(pointId, point);
            }
        }

        // -----------------------------
        // Update DataPoint
        // -----------------------------

        public DataPointUpdatedDomainEvent UpdatePoint(
            string pointId,
            object? value)
        {
            lock (_sync)
            {
                if (!_points.TryGetValue(pointId, out var point))
                    throw new KeyNotFoundException(
                        $"Point '{pointId}' not found in device '{Id}'.");

                var previous = point.Value;

                // اگر مقدار تغییری نکرده، event تولید نکن
                if (Equals(previous, value))
                {
                    return null!;
                }

                point.Update(value);

                return new DataPointUpdatedDomainEvent(
                    deviceId: Id,
                    pointId: pointId,
                    value: value,
                    timestamp: point.LastUpdatedUtc
                );
            }
        }

        // -----------------------------
        // Snapshot (برای Cold Start UI)
        // -----------------------------

        public IReadOnlyCollection<DataPointSnapshot> GetSnapshot()
        {
            lock (_sync)
            {
                return _points.Values
                    .Select(p => new DataPointSnapshot(
                        p.Id,
                        p.Value,
                        p.LastUpdatedUtc))
                    .ToList();
            }
        }

        // -----------------------------
        // Command Execution (Device-centric)
        // -----------------------------

        public DeviceCommandExecutedDomainEvent ExecuteCommand(
            string commandName,
            object? payload = null)
        {
            lock (_sync)
            {
                // در این نسخه، فقط Domain Event تولید می‌کنیم.
                // Driver واقعی در Application/Infrastructure صدا زده می‌شود.

                return new DeviceCommandExecutedDomainEvent(
                    deviceId: Id,
                    commandName: commandName,
                    payload: payload,
                    executedAtUtc: DateTime.UtcNow
                );
            }
        }

        // -----------------------------
        // Helpers
        // -----------------------------

        public bool HasPoint(string pointId)
        {
            lock (_sync)
            {
                return _points.ContainsKey(pointId);
            }
        }

        public DataPoint? GetPoint(string pointId)
        {
            lock (_sync)
            {
                _points.TryGetValue(pointId, out var point);
                return point;
            }
        }
    }

    // -----------------------------
    // Snapshot DTO (Domain-level)
    // -----------------------------

    public record DataPointSnapshot(
        string PointId,
        object? Value,
        DateTime LastUpdatedUtc);
}
