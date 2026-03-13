using System;

namespace BMS.Domain.Entities.Location
{
    public class Site : BaseEntity<Guid>
    {
        public string Name { get; private set; } = default!;
        public string? Address { get; private set; }
        public string? Description { get; private set; }
    }
}