using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class UserPermissionConfiguration
        : BaseEntityConfiguration<UserPermission, Guid>
    {
        public override void Configure(EntityTypeBuilder<UserPermission> b)
        {
            base.Configure(b);

            b.ToTable("UserPermissions");

            // هر User فقط یک بار می‌تواند یک Permission خاص داشته باشد
            b.HasIndex(x => new { x.UserId, x.PermissionId })
             .IsUnique();

            b.Property(x => x.IsGranted)
             .IsRequired()
             .HasDefaultValue(true);

            // روابط
            b.HasOne(x => x.User)
             .WithMany(u => u.UserPermissions)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();

            b.HasOne(x => x.Permission)
             .WithMany()
             .HasForeignKey(x => x.PermissionId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();
        }
    }
}
