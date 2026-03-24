
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Application.CommandDefinitions.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.CommandDefinitions.Commands;

public class UpdateCommandDefinitionCommand : IRequest<ApiResponse<CommandDefinitionDto>>
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Code { get; set; }

    public DeviceType DeviceType { get; set; }

    public bool HasParameter { get; set; }

    public string? ParameterType { get; set; }
}
