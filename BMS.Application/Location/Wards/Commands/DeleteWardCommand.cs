using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class DeleteWardCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
