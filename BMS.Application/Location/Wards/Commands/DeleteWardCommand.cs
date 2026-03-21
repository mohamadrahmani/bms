using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class DeleteWardCommand : IRequest
{
    public Guid Id { get; set; }
}
