using BMS.Application.Features.Pm.DTOs;

namespace BMS.Application.Pm.DTOs
{
    public class PmDto
    {
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public string Title { get; set; } = null!;
        public string? Tag { get; set; }

        public string? Description { get; set; }

        public DateOnly DueDate { get; set; }

        public int WarningDays { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? ClosedAtUtc { get; set; }

        public PmIndicator Indicator { get; set; }
    }
}