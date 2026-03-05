using System;


namespace BMS.Domain.Entities
{
    public class RolePermission : BaseEntity<Guid>
    {
        public int RoleId { get; private set; }
        public Role Role { get; private set; } = default!;

        public int PermissionId { get; private set; }
        public Permission Permission { get; private set; } = default!;

        public RolePermission() { } // EF

        public RolePermission(int roleId, int permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }

        // 👇 مخصوص Seed
        public RolePermission(Guid id, int roleId, int permissionId)
        {
            Id = id;
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
