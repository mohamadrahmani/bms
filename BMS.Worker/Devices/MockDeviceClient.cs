//using BMS.Worker.Abstractions;
//using BMS.Application.Models;
//using BMS.Application.Abstractions;

//namespace BMS.Worker.Devices;

//public class MockDeviceClient : IDeviceClient
//{
//    public Task<DeviceSnapshotDto> ReadAsync(CancellationToken cancellationToken)
//    {
//        var snapshot = new DeviceSnapshotDto
//        {
//            DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
//            Timestamp = DateTime.UtcNow,
//            Sensors = new List<SensorValueDto>
//            {
//                new()
//                {
//                    SensorId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
//                    Value = Random.Shared.NextDouble() * 30
//                }
//            }
//        };

//        return Task.FromResult(snapshot);
//    }
//}
