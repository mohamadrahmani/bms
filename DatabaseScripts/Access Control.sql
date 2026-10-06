USE [BmsDb]
GO

INSERT INTO [dbo].[Permissions]  ([Key],                 [Title],                         [Module],        [RiskLevel], [IsDeleted], [CreatedAtUtc], [UpdatedAtUtc])
                           select 'DeviceShedules.View', N'مشاهده زمانبندی دستگاه ها', 'DeviceShedules', 0,            0,           GETDATE(),      null
GO

INSERT INTO [dbo].[Permissions]  ([Key],                 [Title],                          [Module],        [RiskLevel], [IsDeleted], [CreatedAtUtc], [UpdatedAtUtc])
                           select 'DeviceShedules.Create', N'ایجاد زمانبندی دستگاه ها', 'DeviceShedules', 1,            0,           GETDATE(),      null
GO


INSERT INTO [dbo].[Permissions]  ([Key],                   [Title],                          [Module],        [RiskLevel], [IsDeleted], [CreatedAtUtc], [UpdatedAtUtc])
                           select 'DeviceShedules.Update', N'ویرایش زمانبندی دستگاه ها',  'DeviceShedules', 2,            0,           GETDATE(),      null
GO


INSERT INTO [dbo].[Permissions]  ([Key],                   [Title],                          [Module],        [RiskLevel], [IsDeleted], [CreatedAtUtc], [UpdatedAtUtc])
                           select 'DeviceShedules.Delete', N'حذف زمانبندی دستگاه ها',  'DeviceShedules',    3,            0,           GETDATE(),      null
GO

-- تکمیل دسترسی مدیر

INSERT INTO [dbo].[RolePermissions]    (Id,      [RoleId], [PermissionId], [IsDeleted], [CreatedAtUtc], [UpdatedAtUtc])
select                                  newid(), 1,        p.Id,           0,           GETDATE(),      null 
from Permissions p where not exists (select top 1 1 from RolePermissions rp where rp.PermissionId = p.Id and rp.RoleId = 1)

GO




/*
    New permissions.
    IDs 64 and 65 correspond to PermissionSeed.cs.
*/
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Permissions
    WHERE Id = 64 OR [Key] = N'SystemErrorLogs.View'
)
BEGIN
SET IDENTITY_INSERT Permissions ON;

    INSERT INTO dbo.Permissions
    (
        Id, [Key], Title, Module, RiskLevel, IsDeleted, CreatedAtUtc, UpdatedAtUtc
    )
    VALUES
    (
        64, N'SystemErrorLogs.View', N'مشاهده گزارش خطاهای سیستمی',
        N'Logs', 3, 0, SYSUTCDATETIME(), NULL
    );

	SET IDENTITY_INSERT Permissions Off;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Permissions
    WHERE Id = 65 OR [Key] = N'SystemErrorLogs.Resolve'
)
BEGIN
SET IDENTITY_INSERT Permissions ON;
    INSERT INTO dbo.Permissions
    (
        Id, [Key], Title, Module, RiskLevel, IsDeleted, CreatedAtUtc, UpdatedAtUtc
    )
    VALUES
    (
        65, N'SystemErrorLogs.Resolve', N'بررسی و بستن خطاهای سیستمی',
        N'Logs', 4, 0, SYSUTCDATETIME(), NULL
    );
	SET IDENTITY_INSERT Permissions Off;
END;
GO

/*
    Grant both permissions to SuperAdmin (RoleId = 1).
    Other roles can be granted access through the existing permission management UI.
*/
INSERT INTO dbo.RolePermissions
(
    Id, RoleId, PermissionId, IsDeleted, CreatedAtUtc, UpdatedAtUtc
)
SELECT NEWID(), 1, p.Id, 0, SYSUTCDATETIME(), NULL
FROM dbo.Permissions p
WHERE p.Id IN (64, 65)
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.RolePermissions rp
      WHERE rp.RoleId = 1
        AND rp.PermissionId = p.Id
  );
GO
