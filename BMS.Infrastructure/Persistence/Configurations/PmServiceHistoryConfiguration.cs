using BMS.Domain.Entities;
using BMS.Domain.Entities.PreventiveMaintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public sealed class PmServiceHistoryConfiguration : BaseEntityConfiguration<PmServiceHistory, Guid>
{
    public override void Configure(EntityTypeBuilder<PmServiceHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable("PmServiceHistories", table =>
            table.HasCheckConstraint("CK_PmServiceHistories_Status", "[Status] IN (1, 2)"));

        builder.Property(x => x.Status).HasConversion<byte>().HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.ActionDateUtc).HasColumnType("datetime2(7)").IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.DueDateUtcSnapshot).HasColumnType("datetime2(7)").IsRequired();
        builder.Property(x => x.TitleSnapshot).HasMaxLength(200).IsRequired();
        builder.Property(x => x.BaseDescriptionSnapshot).HasMaxLength(2000);

        builder.HasIndex(x => x.PmScheduleId)
            .IsUnique()
            .HasDatabaseName("UX_PmServiceHistories_PmScheduleId");
        builder.HasIndex(x => x.ActionDateUtc);

        builder.HasOne(x => x.PmSchedule)
            .WithOne(x => x.ServiceHistory)
            .HasForeignKey<PmServiceHistory>(x => x.PmScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmServiceHistories_Users_PerformedBy");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmServiceHistories_Users_CreatedBy");

        builder.Navigation(x => x.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
