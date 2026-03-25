using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.CommandDefinitions.Dtos;

public class CommandDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public CommandType CommandType { get; set; }
    public DeviceType DeviceType { get; set; }
    public bool HasParameter { get; set; }
    public string? ParameterType { get; set; }
}
