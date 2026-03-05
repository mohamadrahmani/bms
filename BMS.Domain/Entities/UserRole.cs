using System;


namespace BMS.Domain.Entities
{
    public class UserRole : BaseEntity<Guid>
    {
        // =========================
        // Properties
        // =========================

        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;

        public int RoleId { get; private set; }
        public Role Role { get; private set; } = default!;

        public bool IsActive { get; private set; } = true;

        // =========================
        // Constructors
        // =========================

        private UserRole() { } // For EF

        public UserRole(Guid userId, int roleId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (roleId <= 0)
                throw new ArgumentException("RoleId is invalid");

            UserId = userId;
            RoleId = roleId;
            IsActive = true;
        }

        // =========================
        // Behavior
        // =========================

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
