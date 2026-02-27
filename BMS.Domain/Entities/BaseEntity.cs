using System;

namespace BMS.Domain.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;

        public bool IsDeleted { get; protected set; }

        public DateTime CreatedAtUtc { get; protected set; }
        public DateTime? UpdatedAtUtc { get; protected set; }

        protected BaseEntity()
        {
            // فقط اگر TKey از نوع Guid باشد در Domain تولید شود
            if (typeof(TKey) == typeof(Guid))
            {
                Id = (TKey)(object)Guid.NewGuid();
            }

            CreatedAtUtc = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        protected void SetUpdated()
        {
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
