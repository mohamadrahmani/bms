using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using System.Diagnostics;

namespace BMS.Infrastructure.Modbus;

public class ModbusPlcClient : IPlcClient
{
    private readonly IModbusConnectionManager _connectionManager;
    private readonly IEnumerable<IDeviceClient> _devices;

    public string Name { get; }
    public ConnectionState State => _connectionManager.State;

    public ModbusPlcClient(
        string name,
        IModbusConnectionManager connectionManager,
        IEnumerable<IDeviceClient> devices)
    {
        Name = name;
        _connectionManager = connectionManager;
        _devices = devices;
    }

    public async Task<bool> TestConnectionAsync(CancellationToken token)
    {
        try
        {
            //await _connectionManager.GetMasterAsync(token);
             return await _connectionManager.PingAsync(token);

           // return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken ct)
    {
        var deviceLookup = _devices.ToDictionary(d => d.DeviceId, d => d);
        var master = await _connectionManager.GetMasterForReadAsync(ct);
        var results = new List<DeviceSnapshotDto>();

        var swTotal = Stopwatch.StartNew();
        int totalBatchesAllDevices = 0;
        int totalPointsAllDevices = 0;

        foreach (var device in _devices)
        {
            var swDevice = Stopwatch.StartNew();
            // ۱. آماده‌سازی نقاط این دستگاه (فیلتر و تبدیل)
            var points = device.Points
                .Where(p => p.RegisterType == RegisterType.Coil || p.RegisterType == RegisterType.HoldingRegister)
                .Select(p =>
                {
                    if (!p.RegisterType.HasValue || !p.Address.HasValue) return null;
                    ushort physical;
                    try
                    {
                        physical = MapLogicalToPhysical(p.RegisterType.Value, p.Address.Value);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return null;
                    }

                    return new PointDto
                    {
                        Id = p.Id,
                        Code = p.Code,
                        DeviceId = p.DeviceId,
                        RegisterType = p.RegisterType,
                        Address = physical,
                        Length = p.Length,
                        DataType = p.DataType,
                        IsWritable = p.IsWritable,
                        FeedbackAddress = p.FeedbackAddress,
                        Scale = p.Scale,
                        Offset = p.Offset
                    };
                })
                .Where(p => p != null)
                .OrderBy(p => p!.RegisterType)   // ابتدا Coil سپس Holding
                .ThenBy(p => p!.Address)
                .ToList();

            if (points.Count == 0) continue;

            Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Points={points.Count}");

            // ۲. ساخت batchهای امن (فقط آدرس‌های موجود)
            var batches = new List<(RegisterType regType, ushort start, ushort len, List<PointDto> batchPoints)>();
            foreach (var group in points.GroupBy(p => p!.RegisterType!.Value))
            {
                var sorted = group.ToList();
                ushort batchStart = sorted[0]!.Address!.Value;
                ushort batchEnd = (ushort)(batchStart + sorted[0]!.Length - 1);
                var currentBatchPoints = new List<PointDto> { sorted[0]! };

                for (int i = 1; i < sorted.Count; i++)
                {
                    var pt = sorted[i]!;
                    ushort ptStart = pt.Address!.Value;
                    ushort ptEnd = (ushort)(ptStart + pt.Length - 1);

                    // اگر فاصله نداشته باشد (gap=0) و با اضافه شدن از maxBatchLength رد نشود
                    if (ptStart <= batchEnd + 5 && ptEnd - batchStart + 1 <= 120)
                    {
                        currentBatchPoints.Add(pt);
                        batchEnd = ptEnd;
                    }
                    else
                    {
                        // پایان batch قبلی
                        batches.Add((group.Key, batchStart, (ushort)(batchEnd - batchStart + 1), currentBatchPoints));
                        // شروع batch جدید
                        batchStart = ptStart;
                        batchEnd = ptEnd;
                        currentBatchPoints = new List<PointDto> { pt };
                    }
                }
                // آخرین batch گروه
                batches.Add((group.Key, batchStart, (ushort)(batchEnd - batchStart + 1), currentBatchPoints));
            }

            totalBatchesAllDevices += batches.Count;
            totalPointsAllDevices += points.Count;
            Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Batches={batches.Count}");

            // ۳. خواندن batchها
            var sensors = new List<SensorValueDto>();
            foreach (var (regType, start, len, batchPoints) in batches)
            {
                var swBatch = Stopwatch.StartNew();
                try
                {
                    bool[]? coilBuffer = null;
                    ushort[]? regBuffer = null;

                    switch (regType)
                    {
                        case RegisterType.Coil:
                            coilBuffer = await _connectionManager.ExecuteReadAsync(
                                () => master.ReadCoilsAsync(1, start, len));
                            break;
                        case RegisterType.HoldingRegister:
                            regBuffer = await _connectionManager.ExecuteReadAsync(
                                () => master.ReadHoldingRegistersAsync(1, start, len));
                            break;
                    }

                    foreach (var point in batchPoints)
                    {
                        int offset = point.Address!.Value - start;
                        int length = point.Length > 0 ? point.Length : 1;

                        object rawSlice = regType switch
                        {
                            RegisterType.Coil => coilBuffer?.Skip(offset).Take(length).ToArray(),
                            RegisterType.HoldingRegister => regBuffer?.Skip(offset).Take(length).ToArray(),
                            _ => null
                        };

                        if (rawSlice == null) continue;

                        if (regType == RegisterType.HoldingRegister && point.DataType == PointDataType.Boolean)
                        {
                            if (rawSlice is ushort[] ushortValues)
                                rawSlice = ushortValues.Select(v => v != 0).ToArray();
                        }

                        double engValue;
                        try
                        {
                            engValue = ModbusValueParser.Parse(rawSlice, point);
                        }
                        catch (Exception parseEx)
                        {
                            Console.WriteLine($"[WARN] Parse error PLC={Name} Device={device.DeviceName} Point {point.Code} (addr {point.Address}): {parseEx.Message}");
                            continue;
                        }

                        sensors.Add(new SensorValueDto
                        {
                            SensorId = point.Id,
                            Name = point.Code,
                            Address = point.Address!.Value,
                            Value = engValue
                        });
                    }
                    swBatch.Stop();

                    Console.WriteLine(
                        $"[DIAG] PLC={Name} Device={device.DeviceName} Batch {start}-{start + len - 1} Time={swBatch.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] PLC={Name} Device={device.DeviceName} Batch Start={start} Len={len} Points=[{string.Join(",", batchPoints.Select(p => p.Address))}] Error: {ex.Message}");
                }
            }
            swDevice.Stop();

            Console.WriteLine(
                $"[DIAG] PLC={Name} Device={device.DeviceName} DeviceTime={swDevice.ElapsedMilliseconds} ms");
            if (sensors.Count > 0)
            {
                results.Add(new DeviceSnapshotDto
                {
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    Timestamp = DateTime.UtcNow,
                    Sensors = sensors
                });
            }
        }

        swTotal.Stop();
        Console.WriteLine($"[DIAG] PLC={Name} GrandTotal: Batches={totalBatchesAllDevices} Points={totalPointsAllDevices} Time={swTotal.ElapsedMilliseconds} ms DevicesSnapshots={results.Count}");
        return results;
    }
    //public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken ct)
    //{
    //    var deviceLookup = _devices.ToDictionary(d => d.DeviceId, d => d);
    //    var master = await _connectionManager.GetMasterAsync(ct);
    //    var results = new List<DeviceSnapshotDto>();

    //    var swTotal = Stopwatch.StartNew();
    //    int totalBatchesAllDevices = 0;
    //    int totalPointsAllDevices = 0;

    //    foreach (var device in _devices)
    //    {
    //        // نقاط فقط این دستگاه
    //        var devicePoints = device.Points
    //            .Where(p => p.RegisterType == RegisterType.Coil || p.RegisterType == RegisterType.HoldingRegister)
    //            .Select(p =>
    //            {
    //                if (!p.RegisterType.HasValue || !p.Address.HasValue) return null;

    //                ushort physical;
    //                try
    //                {
    //                    physical = MapLogicalToPhysical(p.RegisterType.Value, p.Address.Value);
    //                }
    //                catch (ArgumentOutOfRangeException)
    //                {
    //                    return null;
    //                }

    //                return new PointDto
    //                {
    //                    Id = p.Id,
    //                    Code = p.Code,
    //                    DeviceId = p.DeviceId,
    //                    RegisterType = p.RegisterType,
    //                    Address = physical,
    //                    Length = p.Length,
    //                    DataType = p.DataType,
    //                    IsWritable = p.IsWritable,
    //                    FeedbackAddress = p.FeedbackAddress,
    //                    Scale = p.Scale,
    //                    Offset = p.Offset
    //                };
    //            })
    //            .Where(p => p != null)
    //            .ToList();

    //        if (devicePoints.Count == 0) continue;

    //        Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Points={devicePoints.Count}");

    //        // batch builder با gap بزرگ برای کاهش تعداد batch (امتحان کن ببین تا کجا خطا نمیده)
    //        var batches = new ModbusBatchBuilder(maxBatchLength: 120, maxAddressGap: 100)
    //                        .Build(devicePoints);

    //        Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Batches={batches.Count}");

    //        totalBatchesAllDevices += batches.Count;
    //        totalPointsAllDevices += devicePoints.Count;

    //        var sensors = new List<SensorValueDto>();

    //        foreach (var batch in batches)
    //        {
    //            try
    //            {
    //                ushort start = batch.StartAddress;
    //                ushort len = batch.Length;
    //                bool[] coilBuffer = null;
    //                ushort[] regBuffer = null;

    //                switch (batch.RegisterType)
    //                {
    //                    case RegisterType.Coil:
    //                        coilBuffer = await _connectionManager.ExecuteReadAsync(
    //                            () => master.ReadCoilsAsync(1, start, len));
    //                        break;
    //                    case RegisterType.HoldingRegister:
    //                        regBuffer = await _connectionManager.ExecuteReadAsync(
    //                            () => master.ReadHoldingRegistersAsync(1, start, len));
    //                        break;
    //                }

    //                foreach (var point in batch.Points)
    //                {
    //                    if (point.DeviceId == Guid.Empty) continue;
    //                    if (!deviceLookup.TryGetValue(point.DeviceId, out _)) continue;

    //                    int offset = point.Address.Value - batch.StartAddress;
    //                    int length = point.Length > 0 ? point.Length : 1;

    //                    object rawSlice = batch.RegisterType switch
    //                    {
    //                        RegisterType.Coil => coilBuffer?.Skip(offset).Take(length).ToArray(),
    //                        RegisterType.HoldingRegister => regBuffer?.Skip(offset).Take(length).ToArray(),
    //                        _ => null
    //                    };

    //                    if (rawSlice == null) continue;

    //                    if (batch.RegisterType == RegisterType.HoldingRegister && point.DataType == PointDataType.Boolean)
    //                    {
    //                        if (rawSlice is ushort[] ushortValues)
    //                            rawSlice = ushortValues.Select(v => v != 0).ToArray();
    //                    }

    //                    double engValue;
    //                    try
    //                    {
    //                        engValue = ModbusValueParser.Parse(rawSlice, point);
    //                    }
    //                    catch
    //                    {
    //                        continue;
    //                    }

    //                    sensors.Add(new SensorValueDto
    //                    {
    //                        SensorId = point.Id,
    //                        Name = point.Code,
    //                        Address = point.Address!.Value,
    //                        Value = engValue
    //                    });
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine($"[WARN] PLC={Name} Device={device.DeviceName} Batch Start={batch.StartAddress} Len={batch.Length} Points=[{string.Join(",", batch.Points.Select(p => p.Address))}] Error: {ex.Message}");
    //            }
    //            //catch (Exception ex)
    //            //{

    //            //    Console.WriteLine($"[WARN] PLC={Name} Device={device.DeviceName} Batch failed: {ex.Message}");
    //            //    // اگر یک batch به خاطر gap نامعتبر خطا داد، ادامه بده
    //            //}
    //        }

    //        if (sensors.Count > 0)
    //        {
    //            results.Add(new DeviceSnapshotDto
    //            {
    //                DeviceId = device.DeviceId,
    //                DeviceName = device.DeviceName,
    //                Timestamp = DateTime.UtcNow,
    //                Sensors = sensors
    //            });
    //        }
    //    }

    //    swTotal.Stop();
    //    Console.WriteLine($"[DIAG] PLC={Name} GrandTotal: Batches={totalBatchesAllDevices} Points={totalPointsAllDevices} Time={swTotal.ElapsedMilliseconds} ms DevicesSnapshots={results.Count}");
    //    return results;
    //}

    public async Task<bool> WriteAsync(PointDto point, double engineeringValue, CancellationToken token)
    {
        if (point == null)
            throw new InvalidOperationException($"Point {point.Code} not found.");

        if (!point.IsWritable)
            throw new InvalidOperationException($"Point {point.Code} is not writable.");

        //var master = await _connectionManager.GetMasterAsync(token);
        var master = await _connectionManager.GetMasterForReadAsync(token);
        byte slaveId = 1;

        if (!point.RegisterType.HasValue)
            throw new InvalidOperationException("RegisterType is null");

        ushort physAddr = MapLogicalToPhysical(point.RegisterType.Value, point.Address.Value);

        switch (point.RegisterType)
        {
            case RegisterType.Coil:
                if (point.Length == 1)
                {
                    bool valueToWrite = engineeringValue != 0;
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteSingleCoilAsync(slaveId, physAddr, valueToWrite));
                }
                else
                {
                    throw new NotImplementedException(
                        $"Writing multiple coils for point {point.Code} is not implemented.");
                }
                break;

            case RegisterType.HoldingRegister:
                var registersToWrite = ModbusValueParser.BuildWriteRegisters(engineeringValue, point);
                if (registersToWrite.Length == 1)
                {
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteSingleRegisterAsync(slaveId, physAddr, registersToWrite[0]));
                }
                else
                {
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteMultipleRegistersAsync(slaveId, physAddr, registersToWrite));
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    $"Unsupported RegisterType for writing: {point.RegisterType}");
        }

        return true;
    }

    private ushort MapLogicalToPhysical(RegisterType registerType, ushort logicalAddress)
    {
        if (registerType == RegisterType.Coil)
        {
            if (logicalAddress >= 0 && logicalAddress <= 1535)
                return (ushort)(0x800 + (logicalAddress - 0));
            if (logicalAddress >= 1536 && logicalAddress <= 4095)
                return (ushort)(0xB000 + (logicalAddress - 1536));
        }
        else if (registerType == RegisterType.HoldingRegister)
        {
            return logicalAddress;
        }

        throw new ArgumentOutOfRangeException(
            $"No mapping rule for RegisterType={registerType}, LogicalAddress={logicalAddress}");
    }
}
