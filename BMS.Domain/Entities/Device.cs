using System;
using System.Collections.Generic;
using System.Linq;
using BMS.Domain.Entities;
using BMS.Domain.Events;
using BMS.Domain.Enums;
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
                throw new ArgumentException("Device id cannot be empty.");

            Id = id;
        }

        // -----------------------------
        // Register
        // -----------------------------

        public void RegisterPoint(
            string pointId,
            DataType type,
            object? initialValue = null)
        {
            lock (_sync)
            {
                if (_points.ContainsKey(pointId))
                    throw new InvalidOperationException(
                        $"Point '{pointId}' already exists.");

                _points.Add(pointId,
                    new DataPoint(pointId, type, initialValue));
            }
        }

        // -----------------------------
        // Update
        // -----------------------------

        public DataPointUpdatedDomainEvent? UpdatePoint(
            string pointId,
            object? value)
        {
            lock (_sync)
            {
                if (!_points.TryGetValue(pointId, out var point))
                    throw new KeyNotFoundException(
                        $"Point '{pointId}' not found.");

                if (Equals(point.Value, value))
                    return null;

                point.Update(value);

                return new DataPointUpdatedDomainEvent(
                    Id,
                    pointId,
                    value,
                    point.LastUpdatedUtc);
            }
        }

        // -----------------------------
        // Command
        // -----------------------------

        public DeviceCommandExecutedDomainEvent ExecuteCommand(
            string commandName,
            object? payload = null)
        {
            lock (_sync)
            {
                return new DeviceCommandExecutedDomainEvent(
                    Id,
                    commandName,
                    payload,
                    DateTime.UtcNow);
            }
        }

        // -----------------------------
        // Snapshot
        // -----------------------------

        public IReadOnlyCollection<DataPointSnapshot> GetSnapshot()
        {
            lock (_sync)
            {
                return _points.Values
                    .Select(p =>
                        new DataPointSnapshot(
                            p.Id,
                            p.Value,
                            p.LastUpdatedUtc))
                    .ToList();
            }
        }

        public bool HasPoint(string pointId)
        {
            lock (_sync)
                return _points.ContainsKey(pointId);
        }
    }

    public record DataPointSnapshot(
        string PointId,
        object? Value,
        DateTime LastUpdatedUtc);
}
