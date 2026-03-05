using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class RoleConfiguration
        : BaseEntityConfiguration<Role, int>
    {
        public override void Configure(EntityTypeBuilder<Role> b)
        {
            base.Configure(b);

            b.ToTable("Roles");

            // =========================
            // Scalar Properties
            // =========================

            b.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            b.HasIndex(x => x.Name)
                .IsUnique();

            b.Property(x => x.Description)
                .HasMaxLength(500);

            b.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // =========================
            // Relationships
            // =========================

            b.HasMany(r => r.UserRoles)
             .WithOne(ur => ur.Role)
             .HasForeignKey(ur => ur.RoleId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(r => r.RolePermissions)
             .WithOne(rp => rp.Role)
             .HasForeignKey(rp => rp.RoleId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Backing Fields (مهم برای DDD)
            // =========================

            b.Navigation(r => r.UserRoles)
             .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.Navigation(r => r.RolePermissions)
             .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
