using BMS.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public class FileContentConfiguration : IEntityTypeConfiguration<FileContent>
{
    public void Configure(EntityTypeBuilder<FileContent> builder)
    {
        builder.ToTable("FileContents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FileId).IsRequired();
        builder.Property(x => x.Content).HasColumnType("varbinary(max)");
        builder.HasIndex(x => x.FileId).IsUnique();
        builder.HasOne(x => x.File).WithOne(x => x.Content)
            .HasForeignKey<FileContent>(x => x.FileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
