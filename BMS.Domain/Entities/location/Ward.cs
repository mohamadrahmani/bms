using System;

namespace BMS.Domain.Entities.Location
{
    /// Ward نمایانگر یک بخش یا ناحیه در یک طبقه از ساختمان است.
    /// در بیمارستان مثال: ICU ، Emergency ، Radiology
    public class Ward : BaseEntity<Guid>
    {
        public Guid FloorId { get; private set; }

        public string Name { get; private set; } = default!;

        /// نوع بخش
        /// این فیلد به صورت string ذخیره می‌شود تا سیستم به یک دامنه خاص وابسته نشود
        /// مثال:
        /// ICU

        public string? Type { get; private set; }
        public string? Description { get; private set; }

        private Ward() { } // EF Core

        public Ward(Guid floorId, string name, string? type, string? description)
        {
            if (floorId == Guid.Empty)
                throw new ArgumentException("FloorId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ward name is required");

            FloorId = floorId;
            Name = name.Trim();
            Type = type?.Trim();
            Description = description?.Trim();
        }

        public void Update(Guid floorId, string name, string? type, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ward name is required");
            FloorId = floorId;
            Name = name.Trim();
            Type = type?.Trim();
            Description = description?.Trim();

            SetUpdated();
        }
    }
}
