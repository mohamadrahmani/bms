using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Pm.Commands
{
    public class UpdatePmCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string? Tag { get; set; }

        public string? Description { get; set; }

        public DateOnly DueDate { get; set; }

        public int WarningDays { get; set; }
    }
}