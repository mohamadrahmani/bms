using BMS.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;

namespace Bms.Infrastructure.Seeds;

public static class FileSeed
{
    public static readonly Guid PmEntityTypeId =
        Guid.Parse("4e4e7b58-2d3c-4e99-bb7f-1b1e5c7f9a01");
    public static readonly Guid PmOperationHistoryEntityTypeId =
        Guid.Parse("670D275A-BA85-4F31-9313-57C9CDCCBE5D");
    private static readonly Guid PmConfigurationId =
        Guid.Parse("8b2a4e47-8d85-4d4e-9f6a-7e4f2c5b8a11");
    private static readonly Guid PmOperationHistoryConfigurationId =
        Guid.Parse("f3c1e8a2-6d47-4b90-a5e2-9c7f1d3b8a64");

    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileEntityType>().HasData(new
        {
            Id = PmEntityTypeId,
            Code = "PM",
            Name = "Preventive Maintenance",
            TableName = "PmSchedules",
            DisplayNameFa = "تعمیرات پیشگیرانه",
            DisplayNameEn = "Preventive Maintenance",
            IsActive = true,
            IsDeleted = false,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<FileEntityTypeConfiguration>().HasData(new
        {
            Id = PmConfigurationId,
            EntityTypeId = PmEntityTypeId,
            IsUploadAllowed = true,
            MaxFileSize = 2048, // KB
            MaxFileCount = 5,
            AllowedExtensions = "txt,png,jpg",
            AllowedContentTypes = "*",
            IsDeleted = false,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<FileEntityType>().HasData(new
        {
            Id = PmOperationHistoryEntityTypeId,
            Code = "PM_OPERATION_HISTORY",
            Name = "PM Operation History",
            TableName = "PmServiceHistories",
            DisplayNameFa = "سوابق عملیات تعمیرات پیشگیرانه",
            DisplayNameEn = "PM Operation History",
            IsActive = true,
            IsDeleted = false,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<FileEntityTypeConfiguration>().HasData(new
        {
            Id = PmOperationHistoryConfigurationId,
            EntityTypeId = PmOperationHistoryEntityTypeId,
            IsUploadAllowed = true,
            MaxFileSize = 2048,
            MaxFileCount = 5,
            AllowedExtensions = "txt,png,jpg",
            AllowedContentTypes = "*",
            IsDeleted = false,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
