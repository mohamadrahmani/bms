using System;


namespace BMS.Domain.Entities
{
    public class UserPermission : BaseEntity<Guid>
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;

        public int PermissionId { get; private set; }
        public Permission Permission { get; private set; } = default!;

        public bool IsGranted { get; private set; } = true;

        private UserPermission() { } // For EF

        public UserPermission(Guid userId, int permissionId, bool isGranted = true)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (permissionId <= 0)
                throw new ArgumentException("PermissionId is invalid");

            UserId = userId;
            PermissionId = permissionId;
            IsGranted = isGranted;
        }

        public void Grant()
        {
            IsGranted = true;
        }

        public void Deny()
        {
            IsGranted = false;
        }
    }
}
