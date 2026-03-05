using System;
using System.Collections.Generic;

namespace BMS.Domain.Entities
{
    public class Permission : BaseEntity<int>
    {
        public string Key { get; private set; } = default!;
        public string Title { get; private set; } = default!;
        public string Module { get; private set; } = default!;
        public int RiskLevel { get; private set; }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

        private Permission() { } // For EF

        // Constructor for normal usage (runtime)
        public Permission(string key, string title, string module, int riskLevel)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is required");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(module))
                throw new ArgumentException("Module is required");

            Key = key;
            Title = title;
            Module = module;
            RiskLevel = riskLevel;
        }

        // Constructor ONLY for seeding
        public Permission(int id, string key, string title, string module, int riskLevel)
            : this(key, title, module, riskLevel)
        {
            Id = id;
        }
    }
}
