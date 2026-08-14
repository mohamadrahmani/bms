using BMS.Domain.Entities;
using BMS.Domain.Entities.PreventiveMaintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public sealed class PmAttachmentConfiguration : BaseEntityConfiguration<PmAttachment, Guid>
{
    public override void Configure(EntityTypeBuilder<PmAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("PmAttachments", table =>
        {
            table.HasCheckConstraint("CK_PmAttachments_FileSize", "[FileSize] >= 0");
            table.HasCheckConstraint(
                "CK_PmAttachments_ExactlyOneOwner",
                "([PmScheduleId] IS NOT NULL AND [PmServiceHistoryId] IS NULL) OR " +
                "([PmScheduleId] IS NULL AND [PmServiceHistoryId] IS NOT NULL)");
        });

        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
        builder.Property(x => x.FileExtension).HasMaxLength(20);
        builder.Property(x => x.FileContent).HasColumnType("varbinary(max)").IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasIndex(x => x.PmScheduleId);
        builder.HasIndex(x => x.PmServiceHistoryId);

        builder.HasOne(x => x.PmSchedule)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.PmScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PmServiceHistory)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.PmServiceHistoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PmAttachments_Users_CreatedBy");
    }
}
