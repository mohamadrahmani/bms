using Policy = BMS.Domain.Entities.Files.FileEntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public class FileEntityTypePolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("FileEntityTypeConfigurations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.EntityTypeId).IsRequired();
        builder.Property(x => x.MaxFileSize).IsRequired(); // KB
        builder.Property(x => x.MaxFileCount).IsRequired();
        builder.Property(x => x.AllowedExtensions).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.AllowedContentTypes).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => x.EntityTypeId).IsUnique();
    }
}
