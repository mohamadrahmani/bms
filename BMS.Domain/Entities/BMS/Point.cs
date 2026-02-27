using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Domain.Enums;
using System.Globalization;

namespace BMS.Domain.Entities.BMS
{
    public class Point :BaseEntity<Guid>
    {
        // سازنده خصوصی برای EF Core
        public Point() { } // سازنده خصوصی برای EF (Entity Framework)

        // سازنده اصلی برای ایجاد یک Point جدید
        public Point(
            Guid controllerId,        // شناسه کنترلری که این نقطه به آن تعلق دارد
            PointKind kind,           // نوع نقطه (DI, DO, AI, AO, TI …)
            ushort? address,           // آدرس نقطه (مثلاً X0, Y0, CH1 ...)
            //string tag,               // برچسب مختصر و یکتا
            string? title,             // عنوان توصیفی نقطه
            PointDataType dataType,   // نوع داده نقطه (Boolean, Int32, Float32 …)
            string? unit = null)      // واحد اندازه‌گیری (اختیاری)
        {
            // اعتبارسنجی پارامترها
            if (controllerId == Guid.Empty) throw new ArgumentException("ControllerId is required.");
            if (address == null) throw new ArgumentException("Address is required.");
            //if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Tag is required.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");

            ControllerId = controllerId;
            Kind = kind;
            Address = address;
            //Tag = tag.Trim();
            Title = title.Trim();
            DataType = dataType;
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(); // حذف فاصله اضافی یا null

            // آیا نقطه قابل نوشتن است؟ فقط DO و AO قابل نوشتن هستند
            IsWritable = kind is PointKind.DO or PointKind.AO;

            // کیفیت اولیه نقطه
            Quality = PointQuality.Unknown;
        }

      
        public string Code { get; set; } = default!;

      
        public ushort Length { get; set; } = 1;

        public double Scale { get; set; } = 1;
        public double Offset { get; set; } = 0;

        public ushort? CommandAddress { get; set; }
        public ushort? FeedbackAddress { get; set; }

        public int ValidationRetryCount { get; set; } = 3;
        public int ValidationDelayMs { get; set; } = 200;

        // شناسه کنترلر و شیء کنترلر مرتبط
        public Guid ControllerId { get; private set; }
        public Controller Controller { get; private set; } = default!;

        public PointKind Kind { get; private set; }

        // اطلاعات PLC/Excel-friendly
        public ushort? Address { get; set; } = default!;  // X0 / Y0 / CH1 ...
        public string Tag { get; private set; } = default!;
        public string? Title { get; private set; } = default!;

        public string? Unit { get; private set; }       // واحد اندازه‌گیری (مثلاً °C یا m³/h)
        public PointDataType DataType { get; set; } // نوع داده
        public bool IsWritable { get; set; }    // آیا قابل نوشتن است؟

        // اطلاعات Mapping صنعتی (برای ارتباط با Modbus یا PLC)
        public RegisterType? RegisterType { get; private set; }   // نوع رجیستر
        public int? RegisterAddress { get; private set; }         // آدرس رجیستر
        public int? BitIndex { get; private set; }                // بیت مرتبط (برای Coil یا DO)
        public ByteOrder? ByteOrder { get; private set; }         // ترتیب بایت برای داده‌های چند بایتی

        public object? Value { get; set; }
        // آخرین مقدار ذخیره‌شده (Snapshot / Realtime)
        public double? LastNumericValue { get; private set; }     // برای عددی‌ها
        public bool? LastBooleanValue { get; private set; }       // برای بولی‌ها
        public string? LastTextValue { get; private set; }        // برای رشته‌ها

        public string? LastRawValue { get; private set; }         // مقدار خام برای UI یا دیباگ
        public DateTime? LastUpdatedAtUtc { get; private set; } = DateTime.UtcNow;  // زمان آخرین بروزرسانی
        public PointQuality Quality { get; private set; }         // کیفیت داده (Good, Bad, Unknown…)

        // مقیاس‌بندی برای AI/AO/TI (مثلاً تبدیل raw → engineering unit)
        //public PointScaling? Scaling { get; private set; }

        // تنظیم Mapping صنعتی
        public void SetMapping(RegisterType registerType, int registerAddress, int? bitIndex = null, ByteOrder? byteOrder = null)
        {
            if (registerAddress < 0) throw new ArgumentException("RegisterAddress is invalid.");
            if (bitIndex is < 0 or > 15) throw new ArgumentException("BitIndex must be between 0 and 15.");

            RegisterType = registerType;
            RegisterAddress = registerAddress;
            BitIndex = bitIndex;
            ByteOrder = byteOrder;
            SetUpdated(); // علامت‌گذاری تغییر برای EF
        }

        // پاک کردن Mapping
        public void ClearMapping()
        {
            RegisterType = null;
            RegisterAddress = null;
            BitIndex = null;
            ByteOrder = null;
            SetUpdated();
        }

        // تنظیم Scaling برای AI/AO/TI
        //public void SetScaling(PointScaling? scaling)
        //{
        //    if (scaling is not null && Kind is PointKind.DI or PointKind.DO)
        //        throw new BmsDomainException("Scaling is not allowed for DI/DO points.");

        //    Scaling = scaling;
        //    SetUpdated();
        //}

        // بروزرسانی Snapshot/مقدار واقعی نقطه
        public void UpdateSnapshot(string? rawValue, PointQuality quality, DateTime utcNow)
        {
            LastRawValue = rawValue;
            Quality = quality;
            LastUpdatedAtUtc = utcNow;

            // پاک کردن مقادیر قبلی
            LastNumericValue = null;
            LastBooleanValue = null;
            LastTextValue = null;

            if (rawValue is null)
            {
                SetUpdated();
                return;
            }

            // بسته به نوع داده، مقدار را پارس و ذخیره کن
            switch (DataType)
            {
                case PointDataType.Boolean:
                    LastBooleanValue = ParseBool(rawValue);
                    break;

                case PointDataType.Int32:
                case PointDataType.Float32:
                case PointDataType.Float64:
                    LastNumericValue = ParseDouble(rawValue);
                    break;

                case PointDataType.String:
                default:
                    LastTextValue = rawValue;
                    break;
            }

            SetUpdated();
        }

        // تغییر عنوان نقطه
        public void Rename(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
            Title = title.Trim();
            SetUpdated();
        }

        // تبدیل رشته به bool
        private static bool? ParseBool(string s)
        {
            s = s.Trim();

            if (bool.TryParse(s, out var b))  // تبدیل استاندارد "true"/"false"
                return b;

            if (s == "1") return true;       // تبدیل 1 → true
            if (s == "0") return false;      // تبدیل 0 → false

            return null;                     // اگر قابل تبدیل نبود
        }

        // تبدیل رشته به double
        private static double? ParseDouble(string s)
        {
            s = s.Trim();

            // تلاش با استاندارد invariant (نقطه اعشار)
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                return d;

            // تلاش با فرمت منطقه‌ای سیستم
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                return d;

            return null; // اگر قابل تبدیل نبود
        }
    }
}
