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
             await _connectionManager.PingAsync(token);

            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken ct)
    {
        var deviceLookup = _devices.ToDictionary(d => d.DeviceId, d => d);
        var master = await _connectionManager.GetMasterAsync(ct);
        var results = new List<DeviceSnapshotDto>();

        var swTotal = Stopwatch.StartNew();
        int totalBatchesAllDevices = 0;
        int totalPointsAllDevices = 0;

        foreach (var device in _devices)
        {
            // نقاط فقط این دستگاه
            var devicePoints = device.Points
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
                .ToList();

            if (devicePoints.Count == 0) continue;

            Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Points={devicePoints.Count}");

            // batch builder با gap بزرگ برای کاهش تعداد batch (امتحان کن ببین تا کجا خطا نمیده)
            var batches = new ModbusBatchBuilder(maxBatchLength: 120, maxAddressGap: 100)
                            .Build(devicePoints);

            Console.WriteLine($"[DIAG] PLC={Name} Device={device.DeviceName} Batches={batches.Count}");

            totalBatchesAllDevices += batches.Count;
            totalPointsAllDevices += devicePoints.Count;

            var sensors = new List<SensorValueDto>();

            foreach (var batch in batches)
            {
                try
                {
                    ushort start = batch.StartAddress;
                    ushort len = batch.Length;
                    bool[] coilBuffer = null;
                    ushort[] regBuffer = null;

                    switch (batch.RegisterType)
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

                    foreach (var point in batch.Points)
                    {
                        if (point.DeviceId == Guid.Empty) continue;
                        if (!deviceLookup.TryGetValue(point.DeviceId, out _)) continue;

                        int offset = point.Address.Value - batch.StartAddress;
                        int length = point.Length > 0 ? point.Length : 1;

                        object rawSlice = batch.RegisterType switch
                        {
                            RegisterType.Coil => coilBuffer?.Skip(offset).Take(length).ToArray(),
                            RegisterType.HoldingRegister => regBuffer?.Skip(offset).Take(length).ToArray(),
                            _ => null
                        };

                        if (rawSlice == null) continue;

                        if (batch.RegisterType == RegisterType.HoldingRegister && point.DataType == PointDataType.Boolean)
                        {
                            if (rawSlice is ushort[] ushortValues)
                                rawSlice = ushortValues.Select(v => v != 0).ToArray();
                        }

                        double engValue;
                        try
                        {
                            engValue = ModbusValueParser.Parse(rawSlice, point);
                        }
                        catch
                        {
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] PLC={Name} Device={device.DeviceName} Batch Start={batch.StartAddress} Len={batch.Length} Points=[{string.Join(",", batch.Points.Select(p => p.Address))}] Error: {ex.Message}");
                }
                //catch (Exception ex)
                //{

                //    Console.WriteLine($"[WARN] PLC={Name} Device={device.DeviceName} Batch failed: {ex.Message}");
                //    // اگر یک batch به خاطر gap نامعتبر خطا داد، ادامه بده
                //}
            }

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

    //    var allPoints = _devices
    //        .SelectMany(d => d.Points)
    //        .Where(p => p.RegisterType == RegisterType.Coil || p.RegisterType == RegisterType.HoldingRegister)
    //        .Select(p =>
    //        {
    //            if (!p.RegisterType.HasValue || !p.Address.HasValue)
    //                return null;

    //            ushort physical;
    //            try
    //            {
    //                physical = MapLogicalToPhysical(p.RegisterType.Value, p.Address.Value);
    //            }
    //            catch (ArgumentOutOfRangeException)
    //            {
    //                // Silently skip points that cannot be mapped
    //                return null;
    //            }

    //            return new PointDto
    //            {
    //                Id = p.Id,
    //                Code = p.Code,
    //                DeviceId = p.DeviceId,
    //                RegisterType = p.RegisterType,
    //                Address = physical,
    //                Length = p.Length,
    //                DataType = p.DataType,
    //                IsWritable = p.IsWritable,
    //                FeedbackAddress = p.FeedbackAddress,
    //                Scale = p.Scale,
    //                Offset = p.Offset
    //            };
    //        })
    //        .Where(p => p != null)
    //        .ToList();
    //    // ---------- لاگ تشخیص ----------
    //    var swTotal = Stopwatch.StartNew();
    //    // ساخت batchها
    //    var batches = new ModbusBatchBuilder().Build(allPoints);
    //    Console.WriteLine($"[DIAG] PLC={Name} TotalBatches={batches.Count}, PointsConsidered={allPoints.Count}");
    //    // --------------------------------
    //    //var batches = new ModbusBatchBuilder().Build(allPoints);
    //    var snapshots = new Dictionary<Guid, List<SensorValueDto>>();
    //    var master = await _connectionManager.GetMasterAsync(ct);

    //    foreach (var batch in batches)
    //    {
    //        try
    //        {
    //            ushort start = batch.StartAddress;
    //            ushort len = batch.Length;

    //            bool[] coilBuffer = null;
    //            ushort[] regBuffer = null;

    //            switch (batch.RegisterType)
    //            {
    //                case RegisterType.Coil:
    //                    coilBuffer = await _connectionManager.ExecuteWithRetryAsync(
    //                        () => master.ReadCoilsAsync(1, start, len));
    //                    break;

    //                case RegisterType.HoldingRegister:
    //                    regBuffer = await _connectionManager.ExecuteWithRetryAsync(
    //                        () => master.ReadHoldingRegistersAsync(1, start, len));
    //                    break;
    //            }

    //            foreach (var point in batch.Points)
    //            {
    //                if (point.DeviceId == Guid.Empty)
    //                    continue;

    //                if (!deviceLookup.TryGetValue(point.DeviceId, out _))
    //                    continue;

    //                int offset = point.Address.Value - batch.StartAddress;
    //                int length = point.Length > 0 ? point.Length : 1;

    //                object rawSlice = batch.RegisterType switch
    //                {
    //                    RegisterType.Coil => coilBuffer.Skip(offset).Take(length).ToArray(),
    //                    RegisterType.HoldingRegister => regBuffer.Skip(offset).Take(length).ToArray(),
    //                    _ => null
    //                };

    //                if (rawSlice == null)
    //                    continue;

    //                // Convert HoldingRegister Boolean from ushort[] to bool[] if needed
    //                if (batch.RegisterType == RegisterType.HoldingRegister && point.DataType == PointDataType.Boolean)
    //                {
    //                    if (rawSlice is ushort[] ushortValues)
    //                        rawSlice = ushortValues.Select(v => v != 0).ToArray();
    //                }

    //                double engValue;
    //                try
    //                {
    //                    engValue = ModbusValueParser.Parse(rawSlice, point);
    //                }
    //                catch (Exception)
    //                {
    //                    // If a single point fails, skip it to keep the batch alive
    //                    continue;
    //                }

    //                var sensor = new SensorValueDto
    //                {
    //                    SensorId = point.Id,
    //                    Name = point.Code,
    //                    Address = point.Address.Value,
    //                    Value = engValue
    //                };

    //                if (!snapshots.ContainsKey(point.DeviceId))
    //                    snapshots[point.DeviceId] = new List<SensorValueDto>();

    //                snapshots[point.DeviceId].Add(sensor);
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            // Batch failed – skip it and continue with the next
    //        }
    //    }

    //    var results = new List<DeviceSnapshotDto>();
    //    foreach (var kv in snapshots)
    //    {
    //        if (!deviceLookup.TryGetValue(kv.Key, out var device))
    //            continue;

    //        results.Add(new DeviceSnapshotDto
    //        {
    //            DeviceId = device.DeviceId,
    //            DeviceName = device.DeviceName,
    //            Timestamp = DateTime.UtcNow,
    //            Sensors = kv.Value
    //        });
    //    }
    //    swTotal.Stop();
    //    Console.WriteLine($"[DIAG] PLC={Name} PollAsync completed in {swTotal.ElapsedMilliseconds} ms, DevicesSnapshots={results.Count}");
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


//using BMS.Application.Abstraction;
//using BMS.Application.Abstractions;
//using BMS.Application.Enum;
//using BMS.Application.Models;
//using BMS.Application.Points.Dtos;
//using BMS.Domain.Entities.BMS;
//using System.ComponentModel.DataAnnotations;

//namespace BMS.Infrastructure.Modbus;

//public class ModbusPlcClient : IPlcClient
//{
//    private readonly IModbusConnectionManager _connectionManager;
//    private readonly IEnumerable<IDeviceClient> _devices;

//    public string Name { get; }

//    public ConnectionState State => _connectionManager.State;

//    public ModbusPlcClient(
//        string name,
//        IModbusConnectionManager connectionManager,
//        IEnumerable<IDeviceClient> devices)
//    {
//        Name = name;
//        _connectionManager = connectionManager;
//        _devices = devices;
//    }

//    public async Task<bool> TestConnectionAsync(CancellationToken token)
//    {
//        try
//        {
//            await _connectionManager.GetMasterAsync(token);
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken ct)
//    {
//        // همه Deviceها را یک بار به صورت Dictionary آماده می‌کنیم (برای lookup سریع + جلوگیری از First)
//        var deviceLookup = _devices.ToDictionary(d => d.DeviceId, d => d);

//        // جمع‌کردن تمام Points
//        //var allPoints = _devices.SelectMany(d => d.Points).ToList();

//        // کپی امن از نقطه‌ها با آدرس فیزیکی
//        var allPoints = _devices.SelectMany(d => d.Points)
//            .Where(p => p.RegisterType == RegisterType.Coil || p.RegisterType == RegisterType.HoldingRegister)
//            .Select(p =>
//            {
//                if (!p.RegisterType.HasValue || !p.Address.HasValue)
//                    return null;   // نقطه ناقص → رد شود

//                ushort physical;
//                try
//                {
//                    physical = MapLogicalToPhysical(p.RegisterType.Value, p.Address.Value);
//                }
//                catch (ArgumentOutOfRangeException ex)
//                {
//                    Console.WriteLine($"SKIPPED point {p.Id} ({p.Code}): {ex.Message}");
//                    return null;   // نقطه با آدرس نامعتبر → نادیده گرفته شود
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
//                    Offset=p.Offset,
//                    Scale=p.Scale
//                };
//            })
//            .Where(p => p != null)   // nullها را حذف می‌کنیم
//            .ToList();        // ساخت Batchها
//        var batches = new ModbusBatchBuilder().Build(allPoints);
//        Console.WriteLine($"Total batches: {batches.Count}");

//        // نتیجه نهایی polling
//        var snapshots = new Dictionary<Guid, List<SensorValueDto>>();

//        // اتصال به PLC
//        var master = await _connectionManager.GetMasterAsync(ct);

//        // پردازش Batchها
//        foreach (var batch in batches)
//        {
//            try
//            {
//                ushort start = batch.StartAddress;
//                ushort len = batch.Length;

//                bool[] coilBuffer = null;
//                ushort[] regBuffer = null;
//                Console.WriteLine($"BATCH: Type={batch.RegisterType}, Start={batch.StartAddress}, Len={batch.Length}, PointCount={batch.Points.Count}");
//                switch (batch.RegisterType)
//                {
//                    case RegisterType.Coil:
//                        coilBuffer = await _connectionManager.ExecuteWithRetryAsync(
//                            () => master.ReadCoilsAsync(1, start, len)
//                        );
//                        break;

//                    case RegisterType.HoldingRegister:
//                        regBuffer = await _connectionManager.ExecuteWithRetryAsync(
//                           () => master.ReadHoldingRegistersAsync(1, start, len)
//                        );
//                        break;
//                }

//                // Map every point inside the batch safely
//                foreach (var point in batch.Points)
//                {
//                    // DeviceId خراب / خالی
//                    if (point.DeviceId == Guid.Empty)
//                    {
//                        Console.WriteLine($"WARNING: Point {point.Id} has EMPTY DeviceId. Skipped.");
//                        continue;
//                    }

//                    // دستگاهی با این DeviceId وجود ندارد → Skip
//                    if (!deviceLookup.TryGetValue(point.DeviceId, out var device))
//                    {
//                        Console.WriteLine(
//                            $"WARNING: Point {point.Id} refers to UNKNOWN DeviceId {point.DeviceId}. Skipped.");
//                        continue;
//                    }

//                    int offset = point.Address.Value - batch.StartAddress;

//                    // Because point.Length is ushort (not nullable)
//                    int length = point.Length > 0 ? point.Length : 1;

//                    // Extract full slice based on point.Length
//                    Console.WriteLine($"  POINT: Code={point.Code}, Addr={point.Address}, RegType={point.RegisterType}, offset={offset}, length={length}, DataType={point.DataType}");
//                    object rawSlice = batch.RegisterType switch
//                    {
//                        RegisterType.Coil =>
//                            coilBuffer
//                                .Skip(offset)
//                                .Take(length)
//                                .ToArray(),

//                        RegisterType.HoldingRegister =>
//                            regBuffer
//                                .Skip(offset)
//                                .Take(length)
//                                .ToArray(),

//                    };
//                    if (rawSlice == null)
//                    {
//                        Console.WriteLine($"  >>> rawSlice is NULL for point {point.Code}. Skipping.");
//                        continue;
//                    }
//                    Console.WriteLine($"  >>> rawSlice type={rawSlice.GetType().Name}, length={(rawSlice is Array arr ? arr.Length : -1)}");
//                    if (batch.RegisterType == RegisterType.HoldingRegister && point.DataType == PointDataType.Boolean)
//                    {
//                        if (rawSlice is ushort[] ushortValues)
//                        {
//                            rawSlice = ushortValues.Select(v => v != 0).ToArray();
//                            Console.WriteLine($"  >>> Converted HoldingRegister UInt16[] to Boolean[] for point {point.Code}");
//                        }
//                    }
//                    // Engineering value
//                    //double engValue = ModbusValueParser.Parse(rawSlice, point);
//                    double engValue;
//                    try
//                    {
//                        engValue = ModbusValueParser.Parse(rawSlice, point);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"  >>> PARSE ERROR for point {point.Code} (Addr={point.Address}, RegType={point.RegisterType}, DataType={point.DataType}): {ex.Message}");
//                        continue; // از این نقطه بگذر
//                    }

//                    var sensor = new SensorValueDto
//                    {
//                        SensorId = point.Id,
//                        Name = point.Code,
//                        Address = point.Address.Value,
//                        Value = engValue
//                    };


//                    if (!snapshots.ContainsKey(point.DeviceId))
//                        snapshots[point.DeviceId] = new List<SensorValueDto>();

//                    snapshots[point.DeviceId].Add(sensor);
//                }
//                Console.WriteLine($"BATCH completed successfully.");

//            }
//            catch (Exception ex)
//            {
//                // در production exception را نگه دار، ولی worker را نکُش
//                Console.WriteLine($"ERROR while polling batch: {ex}");

//                //Console.WriteLine($"ERROR while polling batch: {ex.Message}");
//            }
//        }

//        // تبدیل snapshot dictionary به خروجی نهایی
//        var results = new List<DeviceSnapshotDto>();

//        foreach (var kv in snapshots)
//        {
//            if (!deviceLookup.TryGetValue(kv.Key, out var device))
//            {
//                Console.WriteLine(
//                    $"WARNING: Snapshot produced for invalid DeviceId {kv.Key}. Skipped.");
//                continue;
//            }

//            results.Add(new DeviceSnapshotDto
//            {
//                DeviceId = device.DeviceId,
//                DeviceName = device.DeviceName,
//                Timestamp = DateTime.UtcNow,
//                Sensors = kv.Value
//            });
//        }

//        return results;
//    }


//    //public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken token)
//    //{
//    //    var results = new List<DeviceSnapshotDto>();


//    //    foreach (var device in _devices)
//    //    {
//    //        try
//    //        {
//    //            var snapshot = await device.ReadAsync(token);
//    //            results.Add(snapshot);


//    //        }
//    //        catch (Exception ex)
//    //        {

//    //            throw ex;
//    //        }
//    //    }

//    //    return results;
//    //}

//    //public async Task<bool> WriteAsync0(PointDto command, CancellationToken token)
//    //{
//    //    if (command.ControllerName != Name)
//    //        return false;

//    //    var device = _devices.FirstOrDefault(d => d.DeviceId == command.DeviceId);
//    //    if (device is null)
//    //        throw new InvalidOperationException($"Device {command.DeviceId} not found in PLC {Name}");

//    //    return await device.WriteAsync(command, double.Parse(command.Value), token);
//    //}
//    public async Task<bool> WriteAsync(
//        PointDto point,
//        double engineeringValue,
//        CancellationToken token)
//    {
//        //var point = _config.DevicePoints
//        //    .First(p => p.Code == pointCode);

//        if (point == null)
//            throw new InvalidOperationException($"Point {point.Code} not found.");

//        // برای دمو: اگر CommandAddress ست شده باشد، حتی اگر IsWritable فراموش شده باشد اجازه می‌دهیم.
//        if (!point.IsWritable)
//            throw new InvalidOperationException($"Point {point.Code} is not writable.");

//        var master = await _connectionManager.GetMasterAsync(token);

//        //var registers = ModbusValueParser.BuildWriteRegisters(
//        //    engineeringValue,
//        //    point);

//        //var commandAddress = point.CommandAddress ?? point.Address;
//        //var feedbackAddress = point.FeedbackAddress ?? commandAddress;
//        //var readLength = (ushort)Math.Max(point.Length, registers.Length);
//        byte slaveId = 1;
//        if (!point.RegisterType.HasValue)
//            throw new InvalidOperationException("RegisterType is null");

//        ushort physAddr = MapLogicalToPhysical(point.RegisterType.Value, point.Address.Value);
//        switch (point.RegisterType)
//        {
//            case RegisterType.Coil:
//                // برای Coil، ما فقط به یک مقدار Boolean نیاز داریم.
//                // engineeringValue باید قابل تبدیل به Boolean باشد (معمولا 0 یا 1).
//                // تابع BuildWriteRegisters شما برای Boolean، یک ushort[1] برمی‌گرداند.
//                // ما از این ushort برای تعیین مقدار Boolean استفاده می‌کنیم.

//                bool valueToWrite = (engineeringValue != 0); // Convert double to bool

//                // NModbus: WriteSingleCoilAsync برای یک بیت، WriteMultipleCoilsAsync برای چند بیت.
//                // اگر نقطه ما فقط یک Coil است (point.Length == 1)
//                if (point.Length == 1)
//                {
//                    await _connectionManager.ExecuteWithRetryAsync(() =>
//                        master.WriteSingleCoilAsync(
//                            slaveId,
//                            physAddr, // آدرس Coil (Zero-Based)
//                            valueToWrite)
//                    );
//                }
//                else
//                {
//                    // اگر بخواهید چند Coil را بنویسید، این بخش نیاز به پیاده‌سازی دقیق دارد.
//                    // شما باید یک آرایه bool[] بسازید و از WriteMultipleCoilsAsync استفاده کنید.
//                    // تابع BuildWriteRegisters شما برای Boolean، یک ushort[1] برمی‌گرداند،
//                    // که برای چند Coil کافی نیست.
//                    // شما باید تابع جداگانه‌ای برای ساخت bool[] از engineeringValue داشته باشید.
//                    throw new NotImplementedException($"Writing multiple coils for point {point.Code} is not implemented.");
//                }
//                break;

//            case RegisterType.HoldingRegister:
//                // برای Holding Registers، از تابع BuildWriteRegisters استفاده می‌کنیم.
//                // این تابع برای DataType های مختلف (UInt16, Int16, Int32, Float32) ushort[] برمی‌گرداند.
//                var registersToWrite = ModbusValueParser.BuildWriteRegisters(
//                    engineeringValue,
//                    point);

//                if (registersToWrite.Length == 1)
//                {
//                    await _connectionManager.ExecuteWithRetryAsync(() =>
//                        master.WriteSingleRegisterAsync(
//                            slaveId,
//                            physAddr, // آدرس Holding Register (Zero-Based)
//                            registersToWrite[0])
//                    );
//                }
//                else
//                {
//                    await _connectionManager.ExecuteWithRetryAsync(() =>
//                        master.WriteMultipleRegistersAsync(
//                            slaveId,
//                            physAddr, // آدرس Holding Register (Zero-Based)
//                            registersToWrite)
//                    );
//                }
//                break;

//            // case RegisterType.DiscreteInput: // Read-only
//            // case RegisterType.InputRegister: // Read-only
//            //     throw new InvalidOperationException($"Point {point.Code} is read-only.");

//            default:
//                throw new ArgumentOutOfRangeException($"Unsupported RegisterType for writing: {point.RegisterType}");
//        }
//        return true;
//    }

//    /// <summary>
//    /// تبدیل آدرس منطقی به آدرس فیزیکی مودباس بر اساس جدول نگاشت ثابت
//    /// </summary>
//    private ushort MapLogicalToPhysical(RegisterType registerType, ushort logicalAddress)
//    {
//        // قوانین نگاشت برگرفته از جدول (اعداد هگز به دسیمال تبدیل شده‌اند)
//        if (registerType == RegisterType.Coil)
//        {
//            if (logicalAddress >= 0 && logicalAddress <= 1535)
//                return (ushort)(0x800 + (logicalAddress - 0));       // 0x800 = 2048
//            if (logicalAddress >= 1536 && logicalAddress <= 4095)
//                return (ushort)(0xB000 + (logicalAddress - 1536));   // 0xB000 = 45056
//        }
//        else if (registerType == RegisterType.HoldingRegister)
//        {
//            //if (logicalAddress >= 0 && logicalAddress <= 4095)
//            //    return (ushort)(0x1000 + (logicalAddress - 0));      // 0x1000 = 4096
//            //if (logicalAddress >= 4096 && logicalAddress <= 9999)
//            //    return (ushort)(0x9000 + (logicalAddress - 4096));   // 0x9000 = 36864
//            //if (logicalAddress >= 10000 && logicalAddress <= 11999)
//            //    return (ushort)(0xA710 + (logicalAddress - 10000));  // 0xA710 = 42768
//            return logicalAddress;
//        }

//        throw new ArgumentOutOfRangeException(
//            $"No mapping rule for RegisterType={registerType}, LogicalAddress={logicalAddress}");
//    }
//}
