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