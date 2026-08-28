using BMS.Domain.Enums;

namespace BMS.Application.Pm.DTOs
{
    public class PmHistoryDto
    {
        public Guid Id { get; set; }

        public Guid PmScheduleId { get; set; }

        public PmServiceStatus Status { get; set; }

        public DateTime ActionDateUtc { get; set; }

        public string? Description { get; set; }

        //public Guid? PerformedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public Guid? CreatedByUserId { get; set; }

        // Snapshots

        public DateOnly DueDateSnapshot { get; set; }

        public string TitleSnapshot { get; set; } = null!;

        public string? BaseDescriptionSnapshot { get; set; }
    }
}