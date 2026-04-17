using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Persons.Commands;

[Audit(EventType.DeleteData, "Persons")]
public sealed record DeletePersonCommand(Guid Id) : IRequest;
