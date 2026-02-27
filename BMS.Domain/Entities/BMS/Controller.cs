using System;
using System.Collections.Generic;

namespace BMS.Domain.Entities.BMS
{
    // Controller: نماینده یک کنترلر (PLC / دستگاه مرکزی) در سیستم BMS
    public class Controller : BaseEntity<Guid>
    {
        // لیست نقاط (Point) مرتبط با این کنترلر
        //private readonly List<Point> _points = new();
        // لیست دستگاه‌ها (Device) مرتبط با این کنترلر
        private readonly List<Device> _devices = new();

        // سازنده خصوصی برای EF Core (Entity Framework)
        public Controller() { }

        // سازنده اصلی برای ایجاد یک کنترلر جدید
        public Controller(
            string code,               // کد یکتا کنترلر
            string name,               // نام کنترلر
            ControllerProtocol protocol, // نوع پروتکل (ModbusTCP, RTU, Bacnet)
            string ipAddress,          // IP کنترلر
            int port,                  // پورت شبکه
            byte unitId,               // شناسه داخلی دستگاه (Modbus Unit ID)
            int timeoutMs,             // تایم‌اوت ارتباط با دستگاه به میلی‌ثانیه
            int retryCount,            // تعداد تلاش دوباره در صورت شکست ارتباط
            int scanIntervalMs,        // فاصله زمانی خواندن نقاط از PLC به میلی‌ثانیه
            string? description = null) // توضیحات اختیاری
        {
            // اعتبارسنجی ورودی‌ها
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(ipAddress)) throw new ArgumentException("IpAddress is required.");
            if (port <= 0) throw new ArgumentException("Port is invalid.");
            if (timeoutMs <= 0) throw new ArgumentException("TimeoutMs is invalid.");
            if (retryCount < 0) throw new ArgumentException("RetryCount is invalid.");
            if (scanIntervalMs <= 0) throw new ArgumentException("ScanIntervalMs is invalid.");

            // مقداردهی به پراپرتی‌ها
            Code = code.Trim();
            Name = name.Trim();
            Protocol = protocol;
            IpAddress = ipAddress.Trim();
            Port = port;
            UnitId = unitId; // UnitId برای شناسایی یکتای PLC در شبکه
            TimeoutMs = timeoutMs;
            RetryCount = retryCount;
            ScanIntervalMs = scanIntervalMs;

            FirmwareVersion = null;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            // مقداردهی اولیه وضعیت‌ها
            IsActive = true;
            HealthStatus = ControllerHealthStatus.Unknown;
        }

        // اطلاعات پایه کنترلر
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public ControllerProtocol Protocol { get; private set; }
        public string IpAddress { get; private set; } = default!;
        public int Port { get; private set; }
        public byte UnitId { get; private set; } // شناسه Modbus

        // جزئیات ارتباط و وضعیت
        public string? FirmwareVersion { get; private set; }
        public int TimeoutMs { get; private set; }
        public int RetryCount { get; private set; }
        public int ScanIntervalMs { get; private set; }
        public string? Description { get; private set; }

        public bool IsActive { get; private set; } // فعال/غیرفعال بودن کنترلر
        public ControllerHealthStatus HealthStatus { get; private set; } // وضعیت سلامت کنترلر
        public DateTime? LastSeenAtUtc { get; private set; } // آخرین زمان پاسخ‌دهی

        // دسترسی به لیست نقاط و دستگاه‌ها بصورت فقط‌خواندنی
        //public IReadOnlyCollection<Point> Points => _points.AsReadOnly();
        public IReadOnlyCollection<Device> Devices => _devices.AsReadOnly();

        // علامت‌گذاری کنترلر به عنوان آنلاین
        public void MarkSeen(DateTime utcNow)
        {
            LastSeenAtUtc = utcNow;
            HealthStatus = ControllerHealthStatus.Online;
            SetUpdated(); // بروزرسانی تاریخ آخرین تغییرات
        }

        // علامت‌گذاری کنترلر به عنوان آفلاین
        public void MarkOffline()
        {
            HealthStatus = ControllerHealthStatus.Offline;
            SetUpdated();
        }

        // علامت‌گذاری کنترلر به عنوان وضعیت خراب یا degraded
        public void MarkDegraded()
        {
            HealthStatus = ControllerHealthStatus.Degraded;
            SetUpdated();
        }

        // بروزرسانی نسخه Firmware
        public void SetFirmwareVersion(string? version)
        {
            FirmwareVersion = string.IsNullOrWhiteSpace(version) ? null : version.Trim();
            SetUpdated();
        }

        // تغییر نام کنترلر
        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            Name = name.Trim();
            SetUpdated();
        }

        // بروزرسانی تنظیمات ارتباط (Timeout, Retry, Scan Interval)
        public void UpdateCommunication(int timeoutMs, int retryCount, int scanIntervalMs)
        {
            if (timeoutMs <= 0) throw new ArgumentException("TimeoutMs is invalid.");
            if (retryCount < 0) throw new ArgumentException("RetryCount is invalid.");
            if (scanIntervalMs <= 0) throw new ArgumentException("ScanIntervalMs is invalid.");

            TimeoutMs = timeoutMs;
            RetryCount = retryCount;
            ScanIntervalMs = scanIntervalMs;
            SetUpdated();
        }

        // غیرفعال کردن کنترلر
        public void Disable() { IsActive = false; SetUpdated(); }

        // فعال کردن کنترلر
        public void Enable() { IsActive = true; SetUpdated(); }
    }
}