using BMS.Domain.Entities.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BMS.Infrastructure.Persistence.Configurations;

public sealed class SystemErrorLogConfiguration
    : BaseEntityConfiguration<SystemErrorLog, Guid>
{
    public override void Configure(EntityTypeBuilder<SystemErrorLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("SystemErrorLogs");

        builder.Property(x => x.ErrorId)
            .IsRequired();

        builder.HasIndex(x => x.ErrorId)
            .IsUnique();

        builder.Property(x => x.OccurredAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.OccurredAtUtc, x.IsResolved });

        builder.Property(x => x.ExceptionType)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.StackTrace)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.InnerException)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.RequestPath)
            .HasMaxLength(2048);

        builder.Property(x => x.HttpMethod)
            .HasMaxLength(16);

        builder.Property(x => x.QueryString)
            .HasMaxLength(4096);

        builder.Property(x => x.RequestBody)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IpAddress)
            .HasMaxLength(64);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1024);

        builder.Property(x => x.EnvironmentName)
            .HasMaxLength(128);

        builder.Property(x => x.ResolutionNote)
            .HasMaxLength(2000);

        builder.Property(x => x.IsResolved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.UserId);
    }
}
