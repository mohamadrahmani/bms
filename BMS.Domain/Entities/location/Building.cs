using System;

namespace BMS.Domain.Entities.Location
{
    /// Building نمایانگر یک ساختمان در مجموعه (Site) است.
    /// یک Site می‌تواند چند Building داشته باشد.
    /// مثال: در یک بیمارستان ممکن است ساختمان درمان، آزمایشگاه و اداری جدا باشند.

    public class Building : BaseEntity<Guid>
    {
        /// شناسه سایت (مجموعه) که این ساختمان به آن تعلق دارد.
        public Guid SiteId { get; private set; }
        public string Name { get; private set; } = default!;
        /// کد ساختمان
        /// یک کد کوتاه برای شناسایی سریع ساختمان در سیستم BMS
        public string Code { get; private set; } = default!;
        public string? Description { get; private set; }

        private Building() { }

        public Building(Guid siteId, string name, string code, string? description)
        {
            if (siteId == Guid.Empty)
                throw new ArgumentException("SiteId is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Building name is required");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Building code is required");

            SiteId = siteId;
            Name = name.Trim();
            Code = code.Trim();
            Description = description?.Trim();
        }

        /// <summary>
        /// بروزرسانی اطلاعات ساختمان
        /// </summary>
        public void Update(Guid siteId, string name, string code, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Building name is required");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Building code is required");
            SiteId = siteId;
            Name = name.Trim();
            Code = code.Trim();
            Description = description?.Trim();

            SetUpdated();
        }
    }
}
