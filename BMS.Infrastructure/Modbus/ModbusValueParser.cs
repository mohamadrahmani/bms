using BMS.Application.Enum;
using BMS.Application.Models;

namespace BMS.Infrastructure.Modbus;

public static class ModbusValueParser
{
    // -------------------------------
    // READ → Raw → Engineering
    // -------------------------------
    public static double Parse(ushort[] registers, PointConfig config)
    {
        double raw = config.DataType switch
        {
            ModbusDataType.UInt16 =>
                registers[0],

            ModbusDataType.Int16 =>
                (short)registers[0],

            ModbusDataType.UInt32 =>
                (uint)((registers[0] << 16) | registers[1]),

            ModbusDataType.Int32 =>
                (int)((registers[0] << 16) | registers[1]),

            ModbusDataType.Float32 =>
                ParseFloat(registers),

            ModbusDataType.Boolean =>
                registers[0] == 1 ? 1 : 0,

            _ => throw new NotSupportedException(
                $"DataType {config.DataType} not supported")
        };

        // 🔵 Engineering conversion
        return (raw * config.Scale) + config.Offset;
    }

    private static float ParseFloat(ushort[] registers)
    {
        var bytes = new byte[4];

        // Word swap (common for Modbus)
        bytes[0] = (byte)(registers[1] >> 8);
        bytes[1] = (byte)(registers[1] & 0xFF);
        bytes[2] = (byte)(registers[0] >> 8);
        bytes[3] = (byte)(registers[0] & 0xFF);

        return BitConverter.ToSingle(bytes, 0);
    }

    // -------------------------------
    // WRITE → Engineering → Raw
    // -------------------------------
    public static ushort[] BuildWriteRegisters(
        double engineeringValue,
        PointConfig config)
    {
        var rawValue =
            (engineeringValue - config.Offset) / config.Scale;

        return config.DataType switch
        {
            ModbusDataType.UInt16 =>
                new[] { (ushort)rawValue },

            ModbusDataType.Int16 =>
                new[] { (ushort)(short)rawValue },

            ModbusDataType.UInt32 =>
                new[]
                {
                    (ushort)((uint)rawValue >> 16),
                    (ushort)((uint)rawValue & 0xFFFF)
                },

            ModbusDataType.Int32 =>
                new[]
                {
                    (ushort)((int)rawValue >> 16),
                    (ushort)((int)rawValue & 0xFFFF)
                },

            ModbusDataType.Float32 =>
                BuildFloatRegisters((float)engineeringValue),

            ModbusDataType.Boolean =>
                new[] { engineeringValue > 0 ? (ushort)1 : (ushort)0 },

            _ => throw new NotSupportedException(
                $"Write not supported for {config.DataType}")
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