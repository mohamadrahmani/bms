

namespace BMS.Domain.Entities.BMS;

public class CommandDefinition : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }

    public CommandType CommandType { get; set; }
    public DeviceType DeviceType { get; set; }

    public bool HasParameter { get; set; }

    public string? ParameterType { get; set; }
}
