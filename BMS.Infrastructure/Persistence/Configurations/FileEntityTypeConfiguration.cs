using BMS.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public class FileEntityTypeConfiguration : IEntityTypeConfiguration<FileEntityType>
{
    public void Configure(EntityTypeBuilder<FileEntityType> builder)
    {
        builder.ToTable("EntityTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TableName).HasMaxLength(200);
        builder.Property(x => x.DisplayNameFa).HasMaxLength(200);
        builder.Property(x => x.DisplayNameEn).HasMaxLength(200);
        builder.Property(x => x.IsActive).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasOne(x => x.Configuration).WithOne(x => x.EntityType)
            .HasForeignKey<Domain.Entities.Files.FileEntityTypeConfiguration>(x => x.EntityTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
