using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class RolePermissionConfiguration
        : BaseEntityConfiguration<RolePermission, Guid>
    {
        public override void Configure(EntityTypeBuilder<RolePermission> b)
        {
            base.Configure(b);

            b.ToTable("RolePermissions");

            // =========================
            // Keys
            // =========================

            b.HasIndex(x => new { x.RoleId, x.PermissionId })
             .IsUnique();

            // =========================
            // Relationships
            // =========================

            b.HasOne(rp => rp.Role)
             .WithMany(r => r.RolePermissions)
             .HasForeignKey(rp => rp.RoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(rp => rp.Permission)
             .WithMany(p => p.RolePermissions)
             .HasForeignKey(rp => rp.PermissionId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
