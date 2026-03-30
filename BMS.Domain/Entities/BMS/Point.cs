using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BMS.Domain.Enums;
using System.Globalization;
using System.Xml.Linq;
using BMS.Domain.Entities.Location;

namespace BMS.Domain.Entities.BMS;
public class Point : BaseEntity<Guid>
{
    // سازنده خصوصی برای EF Core
    public Point() { } // سازنده خصوصی برای EF (Entity Framework)

    // سازنده اصلی برای ایجاد یک Point جدید
    public Point(
        Guid deviceId,        // شناسه کنترلری که این نقطه به آن تعلق دارد
        PointKind kind,           // نوع نقطه (DI, DO, AI, AO, TI …)
		ushort? address,           // آدرس نقطه (مثلاً X0, Y0, CH1 ...)
       string tag,               // برچسب مختصر و یکتا
        string? title,             // عنوان توصیفی نقطه
        PointDataType dataType,   // نوع داده نقطه (Boolean, Int32, Float32 …)
        PointType pointType,
        string? unit = null,
        LocationReference? location = null,
    string? code = null,
    ushort length = 1,
    double scale = 1,
    double offset = 0,
    ushort? feedbackAddress = null,
    int validationRetryCount = 3,
    int validationDelayMs = 200,
    bool isWritable = false)      // واحد اندازه‌گیری (اختیاری)
    {
        // اعتبارسنجی پارامترها
        if (deviceId == Guid.Empty) throw new ArgumentException("DeviceId is required.");
        if (address == null) throw new ArgumentException("Address is required.");
        if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Tag is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");

        DeviceId = deviceId;
        Kind = kind;
        Address = address;
        Tag = tag.Trim();
        Title = title.Trim();
        DataType = dataType;
        PointType = pointType;
        Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(); // حذف فاصله اضافی یا null
        Location = location;
        // آیا نقطه قابل نوشتن است؟ فقط DO و AO قابل نوشتن هستند
        IsWritable = kind is PointKind.DO or PointKind.AO;

        // کیفیت اولیه نقطه
        Quality = PointQuality.Unknown;
        Code = code;
        Length = length;
        Scale = scale;
        Offset = offset;

        FeedbackAddress = feedbackAddress;

        ValidationRetryCount = validationRetryCount;
        ValidationDelayMs = validationDelayMs;

        IsWritable = isWritable;

        Quality = PointQuality.Unknown;
    }

    public LocationReference? Location { get; set; }
    // کد یکتا برای پوینت
    public string? Code { get; set; }

    // طول داده در رجیستر
    public ushort Length { get; set; } = 1;
    // ضریب مقیاس برای تبدیل مقدار خام PLC به مقدار مهندسی
    public double Scale { get; set; } = 1;
    // آفست برای اصلاح مقدار بعد از Scale
    public double Offset { get; set; } = 0;
    // آدرس ارسال فرمان
    // آدرس دریافت فیدبک از PLC (مثلاً وضعیت واقعی خروجی)
    public ushort? FeedbackAddress { get; set; }
    // تعداد دفعات تلاش مجدد برای اعتبارسنجی فرمان
    public int ValidationRetryCount { get; set; } = 3;
    // فاصله زمانی بین تلاش‌های اعتبارسنجی (میلی‌ثانیه)
    public int ValidationDelayMs { get; set; } = 200;

    // شناسه کنترلر و شیء کنترلر مرتبط
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = default!;
    // نوع پوینت در سیستم BMS
    // DI = Digital Input
    // DO = Digital Output
    // AI = Analog Input
    // AO = Analog Output
    public PointKind Kind { get; set; }

    // اطلاعات PLC/Excel-friendly
    // آدرس منطقی پوینت در PLC
    public ushort? Address { get; set; } = default!;  // X0 / Y0 / CH1 ...
                                                      // نام کوتاه و یکتای پوینت (Tag مهندسی)
    public string Tag { get; set; } = default!;
    // عنوان قابل نمایش برای کاربر
    public string? Title { get; set; } = default!;
    // واحد اندازه‌گیری مقدار
    // مثال: °C ، bar ، m³/h
    public string? Unit { get; set; }       // واحد اندازه‌گیری (مثلاً °C یا m³/h)
                                                    // نوع داده پوینت
                                                    // Boolean / Int32 / Float32 / Float64 / String
    public PointDataType DataType { get; set; } // نوع داده
    public PointType PointType { get; set; } // نوع داده
                                                // مشخص می‌کند این پوینت قابل نوشتن است یا فقط خواندنی
                                                // معمولاً DO و AO قابل نوشتن هستند
    public bool IsWritable { get; set; }    // آیا قابل نوشتن است؟

    // اطلاعات Mapping صنعتی (برای ارتباط با Modbus یا PLC)
    // نوع رجیستر در پروتکل Modbus
    // Coil / DiscreteInput / HoldingRegister / InputRegister
    public RegisterType? RegisterType { get; set; }   // نوع رجیستر
                                                              // آدرس رجیستر Modbus
    public int? BitIndex { get; set; }
    // ترتیب بایت برای داده‌های چند بایتی
    // BigEndian / LittleEndian// بیت مرتبط (برای Coil یا DO)
    public ByteOrder? ByteOrder { get; set; }         // ترتیب بایت برای داده‌های چند بایتی

    //public LocationReference Location { get; set; } = default!;

    // مقدار فعلی پوینت به صورت رشته (برای نمایش عمومی یا انتقال)
    public string? Value { get; set; }
    // آخرین مقدار ذخیره‌شده (Snapshot / Realtime)
    // آخرین مقدار عددی پوینت (برای AI/AO)
    public double? LastNumericValue { get; set; }     // برای عددی‌ها
                                                              // آخرین مقدار بولین پوینت (برای DI/DO)
    public bool? LastBooleanValue { get; set; }       // برای بولی‌ها
                                                              // آخرین مقدار متنی پوینت (برای String points)
    public string? LastTextValue { get; set; }        // برای رشته‌ها
                                                              // مقدار خام دریافت‌شده از PLC قبل از تبدیل
    public string? LastRawValue { get; set; }         // مقدار خام برای UI یا دیباگ
    public DateTime? LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;  // زمان آخرین بروزرسانی
                                                                                // کیفیت داده دریافتی
                                                                                // Good / Bad / Unknown
    public PointQuality Quality { get; set; }         // کیفیت داده (Good, Bad, Unknown…)

    // مقیاس‌بندی برای AI/AO/TI (مثلاً تبدیل raw → engineering unit)
    //public PointScaling? Scaling { get; set; }
    public void UpdateLocation(LocationReference location)
    {
        Location = location;
        SetUpdated();
    }
    // تنظیم Mapping صنعتی
    public void SetMapping(RegisterType registerType, ushort address, int? bitIndex = null, ByteOrder? byteOrder = null)
    {
        if (address < 0) throw new ArgumentException("Address is invalid.");
        if (bitIndex is < 0 or > 15) throw new ArgumentException("BitIndex must be between 0 and 15.");

        Address = address;
        RegisterType = registerType;
        BitIndex = bitIndex;
        ByteOrder = byteOrder;
        SetUpdated(); // علامت‌گذاری تغییر برای EF
    }

    // پاک کردن Mapping
    public void ClearMapping()
    {
        RegisterType = null;
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

    public void Update(
    string tag,
    string title,
    PointKind kind,
    PointType pointType,
    RegisterType registerType,
    PointDataType dataType,
    ushort? address,
    string? unit,
    string? code,
    ushort length,
    double scale,
    double offset,
    ushort? feedbackAddress,
    int validationRetryCount,
    int validationDelayMs,
    bool isWritable,
    LocationReference? location
)
    {
        Tag = tag;
        Title = title;
        Kind = kind;
        PointType = pointType;
        DataType = dataType;
        RegisterType = registerType;
        Address = address;
        Unit = unit;

        Code = code;
        Length = length;
        Scale = scale;
        Offset = offset;

        FeedbackAddress = feedbackAddress;

        ValidationRetryCount = validationRetryCount;
        ValidationDelayMs = validationDelayMs;

        IsWritable = isWritable;

        Location = location;

        UpdatedAtUtc = DateTime.UtcNow;
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

// نوع رجیستر در پروتکل Modbus
public enum RegisterType
{
    Coil = 1,           // Coil: Digital Output در Modbus
    DiscreteInput = 2,  // Digital Input
    HoldingRegister = 3, // Analog / Writable
    InputRegister = 4    // Analog / Read-only
}