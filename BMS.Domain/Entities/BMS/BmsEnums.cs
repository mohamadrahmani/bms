using System;

namespace BMS.Domain.Entities.BMS
{
    // پروتکل‌های ارتباطی کنترلرها
    public enum ControllerProtocol
    {
        ModbusTcp = 1,   // TCP/IP با Modbus
        ModbusRtu = 2,   // RTU (Serial) با Modbus
        Bacnet = 3       // پروتکل BACnet برای ساختمان‌ها
    }

    // وضعیت سلامت ارتباط/کنترلر
    public enum ControllerHealthStatus
    {
        Unknown = 0,     // وضعیت نامشخص
        Online = 1,      // آنلاین و سالم
        Offline = 2,     // آفلاین یا ارتباط قطع
        Degraded = 3     // آنلاین اما با مشکل (مثلاً Timeout یا خطاهای ارتباطی)
    }

    // انواع تجهیزات یا Deviceهایی که در BMS کنترل می‌شوند
    public enum DeviceType
    {
        Other = 0,        // سایر تجهیزات
        ExhaustFan = 1,   // فن اگزاست
        AHU = 2,          // Air Handling Unit
        Chiller = 3,      // چیلر
        Boiler = 4,       // بویلر
        Pump = 5,         // پمپ
        Tank = 6,         // مخزن/منبع
        HeatExchanger = 7,// مبدل حرارتی
        CoilUnit = 8,      // یونیت کویلی (کویل سرمایش/گرمایش)
        Solar = 9 // پنل خورشیدی
    }

    public enum PointType
    {
        Sensor = 1,
        Command = 2,
        SetPoint = 3,
        Status = 4
    }

    // نوع نقطه/Point در کنترلر
    public enum PointKind
    {
        DI = 1, // Digital Input
        DO = 2, // Digital Output
        AI = 3, // Analog Input
        AO = 4, // Analog Output
        TI = 5  // Temperature Input (PT100/PT1000)
    }

    // نوع داده هر Point


    public enum PointDataType
    {
        UInt16=1,
        Int16=2,
        UInt32=3,
        Int32=4,
        Float32=5,
        Boolean=6,
        Float64=7,
        String=8
    }

    // وضعیت کیفیت داده (Quality) برای Point
    public enum PointQuality
    {
        Unknown = 0,   // کیفیت نامشخص
        Good = 1,      // داده معتبر و سالم
        Bad = 2,       // داده نامعتبر
        Uncertain = 3  // داده با تردید / ممکن است خطا داشته باشد
    }

   

    // ترتیب بایت برای داده‌های چندبایتی (Endianness)
    public enum ByteOrder
    {
        ABCD = 1,   // Big-Endian استاندارد
        BADC = 2,   // بعضی کنترلرها
        CDAB = 3,   // ترکیب Swap
        DCBA = 4    // Little-Endian معکوس
    }

    // نوع آلارم برای هر Point
    public enum AlarmMode
    {
        None = 0,              // بدون آلارم
        Binary = 1,            // Binary Alarm برای DI (مثلاً Trip / Fault / Fire)
        AnalogThreshold = 2    // Threshold آلارم برای AI/TI/AO (مثلاً High/Low/Deadband)
    }

    public enum CommandType
    {
        Toggle = 1,      // On/Off
        StartStop = 2,   // Start/Stop
        OpenClose = 3,   // Damper/Valve
        EnableDisable = 4,
        AutoManual = 5,
        SetValue = 6,    // SetPoint
        Pulse = 7        // Momentary command
    }
}