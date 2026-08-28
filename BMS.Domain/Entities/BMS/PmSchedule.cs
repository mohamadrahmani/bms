using BMS.Domain.Entities;

namespace BMS.Domain.Entities.BMS
{
    public class PmSchedule : BaseEntity<Guid>
    {
        private PmSchedule()
        {
        }

        public PmSchedule(
            Guid deviceId,
            string title,
            string tag,
            string? description,
            DateOnly dueDate,
            int warningDays)
        {
            if (deviceId == Guid.Empty)
                throw new ArgumentException("DeviceId is required.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("PM title is required.");

            if (warningDays < 0)
                throw new ArgumentException(
                    "WarningDays cannot be negative.");

            DeviceId = deviceId;
            Title = title.Trim();
            Tag = tag.Trim();
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            DueDate = dueDate;
            WarningDays = warningDays;

            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Guid DeviceId { get; private set; }

        public string Title { get; private set; } = null!;
        public string? Tag { get; private set; }

        public string? Description { get; private set; }

        public DateOnly DueDate { get; private set; }

        public int WarningDays { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public Guid? CreatedByUserId { get; private set; }

        public DateTime? UpdatedAtUtc { get; private set; }

        public Guid? UpdatedByUserId { get; private set; }

        public DateTime? ClosedAtUtc { get; private set; }

        public Guid? ClosedByUserId { get; private set; }


        // Navigation

        public Device Device { get; private set; } = null!;

        public ICollection<PmServiceHistory> ServiceHistories { get; private set; }
            = new List<PmServiceHistory>();


        public void Update(
            string title,
            string? description,
            DateOnly dueDate,
            int warningDays,
            Guid? updatedByUserId = null)
        {
            if (!IsActive)
                throw new InvalidOperationException(
                    "Closed PM cannot be updated.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("PM title is required.");

            if (warningDays < 0)
                throw new ArgumentException(
                    "WarningDays cannot be negative.");

            Title = title.Trim();
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

            DueDate = dueDate;
            WarningDays = warningDays;

            UpdatedAtUtc = DateTime.UtcNow;
            UpdatedByUserId = updatedByUserId;
        }

        public void Close(Guid? closedByUserId = null)
        {
            if (!IsActive)
                throw new InvalidOperationException(
                    "PM is already closed.");

            IsActive = false;
            ClosedAtUtc = DateTime.UtcNow;
            ClosedByUserId = closedByUserId;
        }
    }
}