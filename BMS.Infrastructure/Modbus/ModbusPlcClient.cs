using BMS.Application.Abstraction;
using BMS.Application.Abstractions;
using BMS.Application.Enum;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;

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
            await _connectionManager.GetMasterAsync(token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken ct)
    {
        // همه Deviceها را یک بار به صورت Dictionary آماده می‌کنیم (برای lookup سریع + جلوگیری از First)
        var deviceLookup = _devices.ToDictionary(d => d.DeviceId, d => d);

        // جمع‌کردن تمام Points
        var allPoints = _devices.SelectMany(d => d.Points).ToList();

        // ساخت Batchها
        var batches = new ModbusBatchBuilder().Build(allPoints);

        // نتیجه نهایی polling
        var snapshots = new Dictionary<Guid, List<SensorValueDto>>();

        // اتصال به PLC
        var master = await _connectionManager.GetMasterAsync(ct);

        // پردازش Batchها
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
                        coilBuffer = await _connectionManager.ExecuteWithRetryAsync(
                            () => master.ReadCoilsAsync(1, start, len)
                        );
                        break;

                    case RegisterType.HoldingRegister:
                        regBuffer = await _connectionManager.ExecuteWithRetryAsync(
                           () => master.ReadHoldingRegistersAsync(1, start, len)
                        );
                        break;
                }

                // Map every point inside the batch safely
                foreach (var point in batch.Points)
                {
                    // DeviceId خراب / خالی
                    if (point.DeviceId == Guid.Empty)
                    {
                        Console.WriteLine($"WARNING: Point {point.Id} has EMPTY DeviceId. Skipped.");
                        continue;
                    }

                    // دستگاهی با این DeviceId وجود ندارد → Skip
                    if (!deviceLookup.TryGetValue(point.DeviceId, out var device))
                    {
                        Console.WriteLine(
                            $"WARNING: Point {point.Id} refers to UNKNOWN DeviceId {point.DeviceId}. Skipped.");
                        continue;
                    }

                    int offset = point.Address.Value - batch.StartAddress;

                    // Because point.Length is ushort (not nullable)
                    int length = point.Length > 0 ? point.Length : 1;

                    // Extract full slice based on point.Length
                    object rawSlice = batch.RegisterType switch
                    {
                        RegisterType.Coil =>
                            coilBuffer
                                .Skip(offset)
                                .Take(length)
                                .ToArray(),

                        RegisterType.HoldingRegister =>
                            regBuffer
                                .Skip(offset)
                                .Take(length)
                                .ToArray(),

                    };

                    // Engineering value
                    double engValue = ModbusValueParser.Parse(rawSlice, point);

                    var sensor = new SensorValueDto
                    {
                        SensorId = point.Id,
                        Name = point.Code,
                        Address = point.Address.Value,
                        Value = engValue
                    };


                    if (!snapshots.ContainsKey(point.DeviceId))
                        snapshots[point.DeviceId] = new List<SensorValueDto>();

                    snapshots[point.DeviceId].Add(sensor);
                }
            }
            catch (Exception ex)
            {
                // در production exception را نگه دار، ولی worker را نکُش
                Console.WriteLine($"ERROR while polling batch: {ex.Message}");
            }
        }

        // تبدیل snapshot dictionary به خروجی نهایی
        var results = new List<DeviceSnapshotDto>();

        foreach (var kv in snapshots)
        {
            if (!deviceLookup.TryGetValue(kv.Key, out var device))
            {
                Console.WriteLine(
                    $"WARNING: Snapshot produced for invalid DeviceId {kv.Key}. Skipped.");
                continue;
            }

            results.Add(new DeviceSnapshotDto
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName,
                Timestamp = DateTime.UtcNow,
                Sensors = kv.Value
            });
        }

        return results;
    }


    //public async Task<IEnumerable<DeviceSnapshotDto>> PollAsync(CancellationToken token)
    //{
    //    var results = new List<DeviceSnapshotDto>();


    //    foreach (var device in _devices)
    //    {
    //        try
    //        {
    //            var snapshot = await device.ReadAsync(token);
    //            results.Add(snapshot);


    //        }
    //        catch (Exception ex)
    //        {

    //            throw ex;
    //        }
    //    }

    //    return results;
    //}

    //public async Task<bool> WriteAsync0(PointDto command, CancellationToken token)
    //{
    //    if (command.ControllerName != Name)
    //        return false;

    //    var device = _devices.FirstOrDefault(d => d.DeviceId == command.DeviceId);
    //    if (device is null)
    //        throw new InvalidOperationException($"Device {command.DeviceId} not found in PLC {Name}");

    //    return await device.WriteAsync(command, double.Parse(command.Value), token);
    //}
    public async Task<bool> WriteAsync(
        PointDto point,
        double engineeringValue,
        CancellationToken token)
    {
        //var point = _config.DevicePoints
        //    .First(p => p.Code == pointCode);

        if (point == null)
            throw new InvalidOperationException($"Point {point.Code} not found.");

        // برای دمو: اگر CommandAddress ست شده باشد، حتی اگر IsWritable فراموش شده باشد اجازه می‌دهیم.
        if (!point.IsWritable)
            throw new InvalidOperationException($"Point {point.Code} is not writable.");

        var master = await _connectionManager.GetMasterAsync(token);

        //var registers = ModbusValueParser.BuildWriteRegisters(
        //    engineeringValue,
        //    point);

        //var commandAddress = point.CommandAddress ?? point.Address;
        //var feedbackAddress = point.FeedbackAddress ?? commandAddress;
        //var readLength = (ushort)Math.Max(point.Length, registers.Length);
        byte slaveId = 1;

        switch (point.RegisterType)
        {
            case RegisterType.Coil:
                // برای Coil، ما فقط به یک مقدار Boolean نیاز داریم.
                // engineeringValue باید قابل تبدیل به Boolean باشد (معمولا 0 یا 1).
                // تابع BuildWriteRegisters شما برای Boolean، یک ushort[1] برمی‌گرداند.
                // ما از این ushort برای تعیین مقدار Boolean استفاده می‌کنیم.

                bool valueToWrite = (engineeringValue != 0); // Convert double to bool

                // NModbus: WriteSingleCoilAsync برای یک بیت، WriteMultipleCoilsAsync برای چند بیت.
                // اگر نقطه ما فقط یک Coil است (point.Length == 1)
                if (point.Length == 1)
                {
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteSingleCoilAsync(
                            slaveId,
                            point.Address.Value, // آدرس Coil (Zero-Based)
                            valueToWrite)
                    );
                }
                else
                {
                    // اگر بخواهید چند Coil را بنویسید، این بخش نیاز به پیاده‌سازی دقیق دارد.
                    // شما باید یک آرایه bool[] بسازید و از WriteMultipleCoilsAsync استفاده کنید.
                    // تابع BuildWriteRegisters شما برای Boolean، یک ushort[1] برمی‌گرداند،
                    // که برای چند Coil کافی نیست.
                    // شما باید تابع جداگانه‌ای برای ساخت bool[] از engineeringValue داشته باشید.
                    throw new NotImplementedException($"Writing multiple coils for point {point.Code} is not implemented.");
                }
                break;

            case RegisterType.HoldingRegister:
                // برای Holding Registers، از تابع BuildWriteRegisters استفاده می‌کنیم.
                // این تابع برای DataType های مختلف (UInt16, Int16, Int32, Float32) ushort[] برمی‌گرداند.
                var registersToWrite = ModbusValueParser.BuildWriteRegisters(
                    engineeringValue,
                    point);

                if (registersToWrite.Length == 1)
                {
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteSingleRegisterAsync(
                            slaveId,
                            point.Address.Value, // آدرس Holding Register (Zero-Based)
                            registersToWrite[0])
                    );
                }
                else
                {
                    await _connectionManager.ExecuteWithRetryAsync(() =>
                        master.WriteMultipleRegistersAsync(
                            slaveId,
                            point.Address.Value, // آدرس Holding Register (Zero-Based)
                            registersToWrite)
                    );
                }
                break;

            // case RegisterType.DiscreteInput: // Read-only
            // case RegisterType.InputRegister: // Read-only
            //     throw new InvalidOperationException($"Point {point.Code} is read-only.");

            default:
                throw new ArgumentOutOfRangeException($"Unsupported RegisterType for writing: {point.RegisterType}");
        }
        return true;
    }
}
