using BMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration
        : BaseEntityConfiguration<User, Guid>
    {
        public override void Configure(EntityTypeBuilder<User> b)
        {
            base.Configure(b);

            b.ToTable("Users");

            // =========================
            // Scalar Properties
            // =========================

            b.Property(x => x.UserName)
                .HasMaxLength(100)
                .IsRequired();

            b.HasIndex(x => x.UserName)
                .IsUnique();

            b.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.IsActive)
                .IsRequired();


            b.Property(x => x.LastLoginAtUtc)
                .HasColumnType("datetime2");

            // =========================
            // One-to-One : User <-> Person
            // =========================

            b.HasOne(x => x.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(x => x.PersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // ❗ هیچ HasMany یا SetPropertyAccessMode اینجا تعریف نکن
            // Relationship مربوط به UserRole و UserPermission
            // در Configuration همان entity‌ها تعریف شده است.
        }
    }
}
