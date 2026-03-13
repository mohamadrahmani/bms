using System;
using System.Collections.Generic;
using System.Linq;
using BMS.Domain.Events;
using BMS.Domain.Enums;
using System.Net;
using System.Net.NetworkInformation;
using BMS.Domain.Entities.Location;

namespace BMS.Domain.Entities.BMS
{
    public class Device :BaseEntity<Guid>
    {
        private readonly object _sync = new();

        //private readonly Dictionary<string, DataPoint> _points =
        //    new(StringComparer.OrdinalIgnoreCase);

        //public string Id { get; }

        public Device(Guid id)
        {
            //if (string.IsNullOrWhiteSpace(id))
                //throw new ArgumentException("Device id cannot be empty.");

            Id = id;
        }

        private readonly List<Point> _devicePoints = new();

        public Device() { } // سازنده خصوصی برای EF Core

        // سازنده اصلی برای ایجاد دستگاه جدید
        public Device(Guid controllerId, string code, string name, DeviceType type, LocationReference location)
        {
            // اعتبارسنجی ورودی‌ها
            if (controllerId == Guid.Empty) throw new ArgumentException("ControllerId is required.");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");

            ControllerId = controllerId;   // کنترلری که این دستگاه به آن متصل است
            Code = code.Trim();            // کد یکتای دستگاه (مثلاً AHU-01)
            Name = name.Trim();            // نام نمایشی دستگاه
            Type = type;                   // نوع دستگاه (Fan, Pump, AHU ...)
            Location = location;
            // تنظیمات پیش‌فرض
            IsActive = true;               // دستگاه فعال است
            EnableAlarming = true;         // آلارم فعال است
            EnableTrending = true;         // ثبت ترند فعال است
        }

        // شناسه کنترلر مربوطه
        public Guid ControllerId { get; private set; }
        public void UpdateLocation(LocationReference location)
        {
            Location = location;
            SetUpdated();
        }

        // ناوبری به شیء کنترلر (رابطه EF)
        public Controller Controller { get; private set; } = default!;

        // اگر این دستگاه زیرمجموعه دستگاه دیگری باشد
        //public Guid? ParentDeviceId { get; private set; }
        //public Device? ParentDevice { get; private set; }

        // مشخصات اصلی دستگاه
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public DeviceType Type { get; private set; }

        public LocationReference Location { get; private set; } = default!;

        public string? Description { get; private set; }   // توضیح اضافی

        // کلید صفحه گرافیکی UI
        // مشخص می‌کند وقتی کاربر روی این دستگاه کلیک کرد
        // کدام صفحه گرافیکی در نرم‌افزار باز شود
        //public string? GraphicPageKey { get; private set; }

        // تنظیمات سیستم
        public bool EnableAlarming { get; private set; }   // آیا آلارم این دستگاه فعال است؟
        public bool EnableTrending { get; private set; }   // آیا داده‌های این دستگاه ذخیره ترند شوند؟

        public bool IsActive { get; private set; }         // آیا دستگاه فعال است؟

        // دسترسی فقط خواندنی به نقاط دستگاه
        public IReadOnlyCollection<Point> DevicePoints => _devicePoints.AsReadOnly();

        // تعیین دستگاه والد
        //public void SetParent(Guid? parentDeviceId)
        //{
        //    ParentDeviceId = parentDeviceId;
        //    SetUpdated(); // ثبت تغییر
        //}

        // تنظیم محل نصب
        //public void SetLocation(string? location)
        //{
        //    Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        //    SetUpdated();
        //}

        // تنظیم توضیحات
        public void SetDescription(string? description)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            SetUpdated();
        }

        // تعیین صفحه گرافیکی مرتبط در UI
        //public void SetGraphicPageKey(string? key)
        //{
        //    GraphicPageKey = string.IsNullOrWhiteSpace(key) ? null : key.Trim();
        //    SetUpdated();
        //}

        // فعال/غیرفعال کردن آلارم
        public void SetAlarming(bool enabled)
        {
            EnableAlarming = enabled;
            SetUpdated();
        }

        // فعال/غیرفعال کردن ترندینگ
        public void SetTrending(bool enabled)
        {
            EnableTrending = enabled;
            SetUpdated();
        }

        // تغییر نام دستگاه
        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");

            Name = name.Trim();
            SetUpdated();
        }

        // غیرفعال کردن دستگاه
        public void Disable()
        {
            IsActive = false;

            // رفتار امن:
            // وقتی دستگاه غیرفعال شود،
            // نقاط آن هم در UI مخفی می‌شوند تا استفاده اشتباه نشود
            //foreach (var dp in _devicePoints)
            //    dp.SetVisible(false);

            SetUpdated();
        }

        // فعال کردن مجدد دستگاه
        public void Enable()
        {
            IsActive = true;
            SetUpdated();
        }

        // -----------------------------
        // Register
        // -----------------------------

        public void RegisterPoint(
            Guid pointId,
            PointDataType type,
            object? initialValue = null)
        {
            lock (_sync)
            {
                if (_devicePoints.Any(p => p.Id == pointId))
                    //throw new InvalidOperationException($"Point '{pointId}' already exists.");
                    return;

                Point p = new Point();
                p.Id = pointId;
                p.DataType = type;
                
                //new Point(pointId, type, initialValue)

                _devicePoints.Add(p);
            }
        }

        // -----------------------------
        // Update
        // -----------------------------

        public DataPointUpdatedDomainEvent? UpdatePoint(
            Guid pointId,
            string? value)
        {
            lock (_sync)
            {
                var point = _devicePoints.FirstOrDefault(p => p.Id == pointId);

                if (point == null)
                    throw new KeyNotFoundException($"Point '{pointId}' not found.");

                if (Equals(point.Value, value))
                    return null;

                //point.Update(value);
                point.Value = value;

                return new DataPointUpdatedDomainEvent(
                    Id,
                    pointId,
                    value,
                    point.LastUpdatedAtUtc.Value);
            }
        }
        public void UpdateLocation(
    Guid siteId,
    Guid buildingId,
    Guid floorId,
    Guid wardId,
    Guid roomId)
        {
            Location = new LocationReference(
                siteId,
                buildingId,
                floorId,
                wardId,
                roomId);
        }

        public GetDeviceStateDomainEvent? GetDeviceState(Guid deviceId)
        {
            lock (_sync)
            {
                
                //var device = _devicePoints.FirstOrDefault(p => p.Id == deviceId);

                //if (device == null)
                    //throw new KeyNotFoundException($"Point '{deviceId}' not found.");


                return new GetDeviceStateDomainEvent(Id, this);
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
                return _devicePoints
                    .Select(p =>
                        new DataPointSnapshot(
                            p.Id,
                            p.Value,
                            p.LastUpdatedAtUtc.Value))
                    .ToList();
            }
        }

        public bool HasPoint(Guid pointId)
        {
            lock (_sync)
                return _devicePoints.Any(p=> p.Id == pointId);
        }
    }

    public record DataPointSnapshot(
        Guid PointId,
        object? Value,
        DateTime LastUpdatedUtc);
}
