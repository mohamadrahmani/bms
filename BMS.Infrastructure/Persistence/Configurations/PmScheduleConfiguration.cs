using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public class PmScheduleConfiguration
        : IEntityTypeConfiguration<PmSchedule>
    {
        public void Configure(
            EntityTypeBuilder<PmSchedule> builder)
        {
            builder.ToTable("PmSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.DeviceId)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Tag)
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.WarningDays)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.CreatedByUserId)
                .IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc)
                .IsRequired(false);

            builder.Property(x => x.UpdatedByUserId)
                .IsRequired(false);

            builder.Property(x => x.ClosedAtUtc)
                .IsRequired(false);

            builder.Property(x => x.ClosedByUserId)
                .IsRequired(false);


            // Device -> PM

            builder.HasOne(x => x.Device)
                .WithMany(x => x.PmSchedules)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);


            // PM -> History

            builder.HasMany(x => x.ServiceHistories)
                .WithOne(x => x.PmSchedule)
                .HasForeignKey(x => x.PmScheduleId)
                .OnDelete(DeleteBehavior.Restrict);


            // فقط یک PM فعال برای هر Device

            builder.HasIndex(x => x.DeviceId)
                .IsUnique()
                .HasDatabaseName(
                    "UX_PmSchedules_DeviceId_Active")
                .HasFilter("[IsActive] = 1");
        }
    }
}