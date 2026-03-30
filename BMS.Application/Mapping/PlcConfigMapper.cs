using BMS.Application.Models;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Mapping;
public static class PlcConfigMapper
{
    public static PlcConfig ToConfig(Controller controller)
    {
        if (controller == null)
            throw new ArgumentNullException(nameof(controller));

        return new PlcConfig
        {
            Id = controller.Id,
            Name = controller.Name,
            IpAddress = controller.IpAddress,
            Port = controller.Port,
            Devices = controller.Devices?
                .Where(d => d.IsActive.Value)
                .Select(ToDeviceConfig)
                .ToList() ?? new List<DeviceConfig>()
        };
    }

    public static DeviceConfig ToDeviceConfig(Device device)
    {
        return new DeviceConfig
        {
            Id = device.Id,
            Name = device.Name,
            //SlaveId = device.SlaveId,
            DevicePoints = device.DevicePoints?
                //.Where(p => p.IsWritable)
                .Select(ToPointConfig)
                .ToList() ?? new List<PointConfig>()
        };
    }

    public static PointConfig ToPointConfig(Point point)
    {
        return new PointConfig
        {
            Id = point.Id,
            Code = point.Code,
            Address = point.Address,
            DataType = point.DataType,
            //FunctionCode = point.FunctionCode,
            Unit = point.Unit,
            Scale=point.Scale,
            IsWritable= point.IsWritable
        };
    }

    public static List<PlcConfig> ToConfigs(IEnumerable<Controller> controllers)
    {
        return controllers
            .Where(c => c.IsActive)
            .Select(ToConfig)
            .ToList();
    }
}
