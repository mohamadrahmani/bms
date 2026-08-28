using BMS.Domain.Entities.BMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations
{
    public class PmServiceHistoryConfiguration
        : IEntityTypeConfiguration<PmServiceHistory>
    {
        public void Configure(
            EntityTypeBuilder<PmServiceHistory> builder)
        {
            builder.ToTable("PmServiceHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.PmScheduleId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ActionDateUtc)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(4000);

            //builder.Property(x => x.PerformedByUserId)
              //  .IsRequired(false);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.CreatedByUserId)
                .IsRequired(false);


            // Snapshots

            builder.Property(x => x.DueDateSnapshot)
                .IsRequired();

            builder.Property(x => x.TitleSnapshot)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.BaseDescriptionSnapshot)
                .HasMaxLength(2000);


            builder.HasOne(x => x.PmSchedule)
                .WithMany(x => x.ServiceHistories)
                .HasForeignKey(x => x.PmScheduleId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(x => x.PmScheduleId)
                .HasDatabaseName(
                    "IX_PmServiceHistories_PmScheduleId");

            builder.HasIndex(x => x.ActionDateUtc)
                .HasDatabaseName(
                    "IX_PmServiceHistories_ActionDateUtc");
        }
    }
}