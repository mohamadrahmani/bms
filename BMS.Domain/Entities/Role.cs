using System.Collections.Generic;

namespace BMS.Domain.Entities
{
    public class Role : BaseEntity<int>
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        // ================================
        // UserRoles (Existing)
        // ================================
        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

        // ================================
        // RolePermissions (NEW - Fixed)
        // ================================
        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

        public Role() { } // For EF

        public Role(string name, string? description)
        {
            Name = name;
            Description = description;
            IsActive = true;
        }

        // For Seeding / Internal Usage
        public Role(int id, string name, string? description, bool isActive)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
        }
    }
}
