using MediatR;

namespace BMS.Application.Location.Sites.Commands;

public sealed record DeleteSiteCommand(Guid Id) : IRequest;
