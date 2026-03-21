//using System;

//namespace BMS.Domain.Entities.Location
//{
//    public class Site : BaseEntity<Guid>
//    {
//        public string Name { get; private set; } = default!;
//        public string? Address { get; private set; }
//        public string? Description { get; private set; }
//    }
//}
using System;

namespace BMS.Domain.Entities.Location
{
    public class Site : BaseEntity<Guid>
    {
        public string Name { get; private set; } = default!;
        public string? Address { get; private set; }
        public string? Description { get; private set; }

        private Site() { } // For EF Core

        public Site(string name, string? address, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Site name is required");

            Name = name.Trim();
            Address = address?.Trim();
            Description = description?.Trim();
        }

        public void Update(string name, string? address, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Site name is required");

            Name = name.Trim();
            Address = address?.Trim();
            Description = description?.Trim();

            SetUpdated();
        }
    }
}
