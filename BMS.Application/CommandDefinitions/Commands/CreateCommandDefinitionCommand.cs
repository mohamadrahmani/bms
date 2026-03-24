using BMS.Application.CommandDefinitions.Dtos;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.CommandDefinitions.Commands;

public class CreateCommandDefinitionCommand : IRequest<ApiResponse<Guid>>
{
    public string Name { get; set; }
    public string Code { get; set; }

    public DeviceType DeviceType { get; set; }

    public bool HasParameter { get; set; }

    public string? ParameterType { get; set; }
}
