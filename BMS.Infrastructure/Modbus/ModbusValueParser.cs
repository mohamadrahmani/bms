using BMS.Application.Common.Settings;
using BMS.Application.Enum;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;

using static System.Formats.Asn1.AsnWriter;

namespace BMS.Infrastructure.Modbus;

public static class ModbusValueParser
{
    // READ → Raw → Engineering
    public static double Parse(object registers, PointDto config)
    {
        if (registers is bool[] bools)
        {
            return bools[0] ? 1 : 0;
        }

        if (registers is ushort[] ushorts)
        {
            // Boolean مستقیماً
            if (config.DataType == PointDataType.Boolean)
                return ushorts[0] != 0 ? 1 : 0;

            // سایر انواع داده → متد داخلی با PointDto
            return Parse(ushorts, config);
        }

        throw new NotSupportedException("Unsupported register type");
    }

    // تغییر پارامتر از PointConfig به PointDto
    private static double Parse(ushort[] registers, PointDto config)
    {
        double raw = config.DataType switch
        {
            PointDataType.UInt16 => registers[0],
            PointDataType.Int16 => (short)registers[0],
            PointDataType.UInt32 => (uint)((registers[0] << 16) | registers[1]),
            PointDataType.Int32 => (int)((registers[0] << 16) | registers[1]),
            PointDataType.Float32 => ParseFloat(registers),
            _ => throw new NotSupportedException($"DataType {config.DataType} not supported")
        };

        LinearScaler scaler = new LinearScaler(config.Scale, config.Offset);
        return scaler.RawToDisplay(raw);
    }

    private static float ParseFloat(ushort[] registers)
    {
        var bytes = new byte[4];
        bytes[0] = (byte)(registers[1] >> 8);
        bytes[1] = (byte)(registers[1] & 0xFF);
        bytes[2] = (byte)(registers[0] >> 8);
        bytes[3] = (byte)(registers[0] & 0xFF);
        return BitConverter.ToSingle(bytes, 0);
    }

    // WRITE → Engineering → Raw (بدون تغییر)
    public static ushort[] BuildWriteRegisters(double engineeringValue, PointDto config)
    {
        LinearScaler scaler = new LinearScaler(config.Scale, config.Offset);
        var rawValue = scaler.DisplayToRaw(engineeringValue);
        return config.DataType switch
        {
            PointDataType.UInt16 => new[] { (ushort)rawValue },
            PointDataType.Int16 => new[] { (ushort)(short)rawValue },
            PointDataType.Int32 => new[]
            {
                (ushort)((int)rawValue >> 16),
                (ushort)((int)rawValue & 0xFFFF)
            },
            PointDataType.UInt32 => new[]
            {
                (ushort)((uint)rawValue >> 16),
                (ushort)((uint)rawValue & 0xFFFF)
            },
            PointDataType.Float32 => BuildFloatRegisters((float)engineeringValue),
            PointDataType.Boolean => new[] { engineeringValue > 0 ? (ushort)1 : (ushort)0 },
            _ => throw new NotSupportedException($"Write not supported for {config.DataType}")
        };
    }

    private static ushort[] BuildFloatRegisters(float value)
    {
        var bytes = BitConverter.GetBytes(value);
        return new ushort[]
        {
            (ushort)((bytes[2] << 8) | bytes[3]),
            (ushort)((bytes[0] << 8) | bytes[1])
        };
    }
}
//public static class ModbusValueParser
//{
//    // -------------------------------
//    // READ → Raw → Engineering
//    // -------------------------------
//    //public static double Parse(object registers, PointDto config)
//    //{
//    //    if (registers is bool[] bools)
//    //    {
//    //        return bools[0] ? 1 : 0;
//    //    }

//    //    if (registers is ushort[] ushorts)
//    //    {
//    //        return Parse(ushorts, config);
//    //    }

//    //    throw new NotSupportedException("Unsupported register type");
//    //}
//    public static double Parse(object registers, PointDto config)
//    {
//        if (registers is bool[] bools)
//        {
//            return bools[0] ? 1 : 0;
//        }

//        if (registers is ushort[] ushorts)
//        {
//            // اگر DataType برابر Boolean باشد، مستقیماً تبدیل کنیم و برگردیم
//            if (config.DataType == PointDataType.Boolean)
//            {
//                // برای یک نقطه Boolean، فقط اولین عضو آرایه ملاک است
//                return ushorts[0] != 0 ? 1 : 0;
//            }

//            // برای سایر DataTypeها به متد تخصصی دیگر برو (که دیگر Boolean نیست)
//            return Parse(ushorts, config);
//        }

//        throw new NotSupportedException("Unsupported register type");
//    }
//    public static double Parse(ushort[] registers, PointConfig config)
//    {
//        double raw = config.DataType switch
//        {
//            PointDataType.UInt16 =>
//                registers[0],

//            PointDataType.Int16 =>
//                (short)registers[0],

//            PointDataType.UInt32 =>
//                (uint)((registers[0] << 16) | registers[1]),

//            PointDataType.Int32 =>
//                (int)((registers[0] << 16) | registers[1]),

//            PointDataType.Float32 =>
//                ParseFloat(registers),

//            PointDataType.Boolean =>
//                registers[0] != 0 ? 1 : 0,

//            _ => throw new NotSupportedException(
//                $"DataType {config.DataType} not supported")
//        };
//        LinearScaler scaler=new LinearScaler(config.Scale, config.Offset);
//        return scaler.RawToDisplay(raw);
//        // 🔵 Engineering conversion
//        //return raw * config.Scale + config.Offset;// (raw * config.Scale) + config.Offset;

//    }

//    private static float ParseFloat(ushort[] registers)
//    {
//        var bytes = new byte[4];

//        // Word swap (common for Modbus)
//        bytes[0] = (byte)(registers[1] >> 8);
//        bytes[1] = (byte)(registers[1] & 0xFF);
//        bytes[2] = (byte)(registers[0] >> 8);
//        bytes[3] = (byte)(registers[0] & 0xFF);

//        return BitConverter.ToSingle(bytes, 0);
//    }

//    // -------------------------------
//    // WRITE → Engineering → Raw
//    // -------------------------------
//    public static ushort[] BuildWriteRegisters(
//        double engineeringValue,
//        PointDto config)
//    {
//        LinearScaler scaler = new LinearScaler(config.Scale, config.Offset);
//        var rawValue = scaler.DisplayToRaw(engineeringValue);

//        //var rawValue =
//        //    (engineeringValue - config.Offset) / config.Scale;

//        return config.DataType switch
//        {
//            PointDataType.UInt16 =>
//                new[] { (ushort)rawValue },

//            PointDataType.Int16 =>
//                new[] { (ushort)(short)rawValue },


//            PointDataType.Int32 =>
//                new[]
//                {
//                    (ushort)((int)rawValue >> 16),
//                    (ushort)((int)rawValue & 0xFFFF)
//                },
//            PointDataType.UInt32 =>
//                new[]
//                {
//                    (ushort)((uint)rawValue >> 16),
//                    (ushort)((uint)rawValue & 0xFFFF)
//                },

//            PointDataType.Float32 =>
//                BuildFloatRegisters((float)engineeringValue),

//            PointDataType.Boolean =>
//                new[] { engineeringValue > 0 ? (ushort)1 : (ushort)0 },

//            _ => throw new NotSupportedException(
//                $"Write not supported for {config.DataType}")
//        };
//    }

//    private static ushort[] BuildFloatRegisters(float value)
//    {
//        var bytes = BitConverter.GetBytes(value);

//        return new ushort[]
//        {
//            (ushort)((bytes[2] << 8) | bytes[3]),
//            (ushort)((bytes[0] << 8) | bytes[1])
//        };
//    }
//}