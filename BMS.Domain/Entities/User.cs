using System;
using System.Collections.Generic;
using System.Linq;
using BMS.Domain.Exceptions;

namespace BMS.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        // ========================
        // Security Constants
        // ========================
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        // ========================
        // Properties
        // ========================
        public Guid PersonId { get; private set; }
        public Person Person { get; private set; } = default!;

        public string UserName { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;

        public bool IsActive { get; private set; }
        public DateTime? LastLoginAtUtc { get; private set; }

        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutUntilUtc { get; private set; }

        // 🔐 Permission Version (for JWT invalidation)
        public int PermissionVersion { get; private set; } = 1;

        public bool IsLocked =>
            LockoutUntilUtc.HasValue &&
            LockoutUntilUtc.Value > DateTime.UtcNow;

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        private readonly List<UserPermission> _userPermissions = new();
        public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions.AsReadOnly();

        // ========================
        // Constructors
        // ========================
        private User() { } // For EF

        public User(Guid personId, string userName, string passwordHash)
        {
            if (personId == Guid.Empty)
                throw new UserDomainException("PersonId is required.");

            PersonId = personId;

            SetUserName(userName);
            SetPasswordHash(passwordHash);

            IsActive = true;
            FailedLoginAttempts = 0;
            PermissionVersion = 1;
        }

        // ========================
        // Permission Version Logic
        // ========================
        public void IncrementPermissionVersion()
        {
            PermissionVersion++;
        }

        // ========================
        // Authentication Behavior
        // ========================
        public void EnsureCanLogin()
        {
            if (!IsActive)
                throw new UserDomainException("Invalid credentials.");

            if (IsLocked)
                throw new UserDomainException("Account is temporarily locked.");
        }

        public void RegisterLoginResult(bool isPasswordValid)
        {
            if (!isPasswordValid)
            {
                RegisterFailedLogin();
                throw new UserDomainException("Invalid credentials.");
            }

            RegisterSuccessfulLogin();
        }

        private void RegisterFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= MaxFailedAttempts)
            {
                Lock();
            }
        }

        private void RegisterSuccessfulLogin()
        {
            FailedLoginAttempts = 0;
            Unlock();
            LastLoginAtUtc = DateTime.UtcNow;
        }

        public void Lock()
        {
            LockoutUntilUtc = DateTime.UtcNow.Add(LockoutDuration);
            FailedLoginAttempts = 0;
        }

        public void Unlock()
        {
            LockoutUntilUtc = null;
        }

        // ========================
        // Profile Behavior
        // ========================
        public void UpdateProfile(string userName, bool isActive)
        {
            SetUserName(userName);

            if (isActive)
                Activate();
            else
                Deactivate();
        }

        public void ChangePassword(string newPasswordHash)
        {
            SetPasswordHash(newPasswordHash);
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // ========================
        // Roles
        // ========================
        public void AssignRole(int roleId)
        {
            if (roleId <= 0)
                throw new UserDomainException("Invalid role id.");

            if (_userRoles.Any(r => r.RoleId == roleId))
                return;

            _userRoles.Add(new UserRole(Id, roleId));

            // 🔐 Permission state changed
            IncrementPermissionVersion();
        }

        public void SyncRoles(IEnumerable<int> roleIds)
        {
            if (roleIds is null)
                throw new UserDomainException("RoleIds cannot be null.");

            _userRoles.Clear();

            foreach (var roleId in roleIds.Distinct())
            {
                if (roleId <= 0)
                    throw new UserDomainException("Invalid role id.");

                _userRoles.Add(new UserRole(Id, roleId));
            }

            // 🔐 Permission state changed
            IncrementPermissionVersion();
        }

        // ========================
        // Permissions
        // ========================
        public void AssignPermission(int permissionId)
        {
            if (permissionId <= 0)
                throw new UserDomainException("Invalid permission id.");

            if (_userPermissions.Any(p => p.PermissionId == permissionId))
                return;

            _userPermissions.Add(new UserPermission(Id, permissionId));

            // 🔐 Permission state changed
            IncrementPermissionVersion();
        }

        // ========================
        // Private Invariants
        // ========================
        private void SetUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new UserDomainException("Username is required.");

            UserName = userName.Trim().ToLowerInvariant();
        }

        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new UserDomainException("Password hash is required.");

            PasswordHash = passwordHash;
        }
    }
}
