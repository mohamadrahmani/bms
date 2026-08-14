using BMS.Domain.Entities;
using BMS.Domain.Entities.PreventiveMaintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public sealed class PmScheduleConfiguration : BaseEntityConfiguration<PmSchedule, Guid>
{
    public override void Configure(EntityTypeBuilder<PmSchedule> builder)
    {
        base.Configure(builder);

        builder.ToTable("PmSchedules", table =>
            table.HasCheckConstraint("CK_PmSchedules_WarningDays", "[WarningDays] >= 0"));

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.DueDateUtc).HasColumnType("datetime2(7)").IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(x => x.ClosedAtUtc).HasColumnType("datetime2(7)");
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(x => x.DeviceId)
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0")
            .HasDatabaseName("UX_PmSchedules_DeviceId_Active");
        builder.HasIndex(x => new { x.DeviceId, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.IsActive, x.DueDateUtc });

        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmSchedules_Users_CreatedBy");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmSchedules_Users_UpdatedBy");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ClosedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmSchedules_Users_ClosedBy");

        builder.Navigation(x => x.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
