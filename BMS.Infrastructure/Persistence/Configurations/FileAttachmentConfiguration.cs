using BMS.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder.ToTable("FileAttachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.EntityTypeId).IsRequired();
        builder.Property(x => x.EntityId).IsRequired();
        builder.HasOne(x => x.File).WithMany(x => x.Attachments)
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.FileId, x.EntityTypeId, x.EntityId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.EntityTypeId, x.EntityId, x.IsDeleted });
    }
}
