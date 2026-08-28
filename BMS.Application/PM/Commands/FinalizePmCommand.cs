using BMS.Application.Models;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.Pm.Commands
{
    public class FinalizePmCommand
        : IRequest<ApiResponse<bool>>
    {
        public Guid PmScheduleId { get; set; }

        public PmServiceStatus Status { get; set; }

        public DateTime ActionDateUtc { get; set; }

        public string? Description { get; set; }

        //public Guid? PerformedByUserId { get; set; }
    }
}