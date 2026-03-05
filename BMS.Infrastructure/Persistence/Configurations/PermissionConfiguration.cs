using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class PermissionConfiguration
        : BaseEntityConfiguration<Permission, int>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.ToTable("Permissions");

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Key)
                .IsUnique();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.RiskLevel)
                .IsRequired();

            // ✅ Backing field access
            builder.Navigation(x => x.RolePermissions)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
