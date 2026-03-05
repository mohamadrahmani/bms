using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class UserRoleConfiguration
        : BaseEntityConfiguration<UserRole, Guid>
    {
        public override void Configure(EntityTypeBuilder<UserRole> b)
        {
            base.Configure(b);

            b.ToTable("UserRoles");

            // =========================
            // Index (Prevent duplicate role per user)
            // =========================

            b.HasIndex(x => new { x.UserId, x.RoleId })
                .IsUnique();

            // =========================
            // Properties
            // =========================

            b.Property(x => x.RoleId)
                .IsRequired();

            b.Property(x => x.IsActive)
                .HasDefaultValue(true);

            // =========================
            // Relationships
            // =========================

            b.HasOne(x => x.User)
                .WithMany(u => u.UserRoles) // ✅ use navigation property
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
