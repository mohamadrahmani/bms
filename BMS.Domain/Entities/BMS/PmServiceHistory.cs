using BMS.Domain.Enums;

namespace BMS.Domain.Entities.BMS
{
    public class PmServiceHistory : BaseEntity<Guid>
    {
        private PmServiceHistory()
        {
        }

        public PmServiceHistory(
            Guid pmScheduleId,
            PmServiceStatus status,
            DateTime actionDateUtc,
            string? description,
            //Guid? performedByUserId,
            DateOnly dueDateSnapshot,
            string titleSnapshot,
            string? baseDescriptionSnapshot,
            Guid? createdByUserId = null)
        {
            if (pmScheduleId == Guid.Empty)
                throw new ArgumentException("PmScheduleId is required.");

            if (string.IsNullOrWhiteSpace(titleSnapshot))
                throw new ArgumentException(
                    "TitleSnapshot is required.");

            PmScheduleId = pmScheduleId;

            Status = status;
            ActionDateUtc = actionDateUtc;

            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            //PerformedByUserId = performedByUserId;

            CreatedAtUtc = DateTime.UtcNow;
            CreatedByUserId = createdByUserId;

            DueDateSnapshot = dueDateSnapshot;
            TitleSnapshot = titleSnapshot.Trim();

            BaseDescriptionSnapshot =
                string.IsNullOrWhiteSpace(baseDescriptionSnapshot)
                    ? null
                    : baseDescriptionSnapshot.Trim();
        }


        public Guid PmScheduleId { get; private set; }

        public PmServiceStatus Status { get; private set; }

        public DateTime ActionDateUtc { get; private set; }

        public string? Description { get; private set; }

        //public Guid? PerformedByUserId { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public Guid? CreatedByUserId { get; private set; }


        // Snapshot

        public DateOnly DueDateSnapshot { get; private set; }

        public string TitleSnapshot { get; private set; } = null!;

        public string? BaseDescriptionSnapshot { get; private set; }


        // Navigation

        public PmSchedule PmSchedule { get; private set; } = null!;

        public void Update(PmServiceStatus status, DateTime actionDateUtc, string? description)
        {
            Status = status;
            ActionDateUtc = actionDateUtc;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            SetUpdated();
        }
    }
}
