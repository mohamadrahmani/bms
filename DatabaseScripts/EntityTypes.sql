/*
    EntityTypes database update
    SQL Server / dbo

    This script is idempotent and preserves the existing FileEntityTypes data.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.FileEntityTypes', N'U') IS NOT NULL
   AND OBJECT_ID(N'dbo.EntityTypes', N'U') IS NULL
BEGIN
    EXEC sys.sp_rename
        @objname = N'dbo.FileEntityTypes',
        @newname = N'EntityTypes',
        @objtype = N'OBJECT';
END;

IF OBJECT_ID(N'dbo.EntityTypes', N'U') IS NULL
    THROW 50001, 'Neither dbo.FileEntityTypes nor dbo.EntityTypes exists.', 1;

IF COL_LENGTH(N'dbo.EntityTypes', N'TableName') IS NULL
    ALTER TABLE dbo.EntityTypes ADD TableName nvarchar(200) NULL;

IF COL_LENGTH(N'dbo.EntityTypes', N'DisplayNameFa') IS NULL
    ALTER TABLE dbo.EntityTypes ADD DisplayNameFa nvarchar(200) NULL;

IF COL_LENGTH(N'dbo.EntityTypes', N'DisplayNameEn') IS NULL
    ALTER TABLE dbo.EntityTypes ADD DisplayNameEn nvarchar(200) NULL;

UPDATE dbo.EntityTypes
SET TableName = N'PmSchedules',
    DisplayNameFa = N'تعمیرات پیشگیرانه',
    DisplayNameEn = N'Preventive Maintenance'
WHERE Code = N'PM';

UPDATE dbo.EntityTypes
SET TableName = N'PmServiceHistories',
    DisplayNameFa = N'سوابق عملیات تعمیرات پیشگیرانه',
    DisplayNameEn = N'PM Operation History'
WHERE Code = N'PM_OPERATION_HISTORY';

DECLARE @EntityTypes TABLE
(
    Code nvarchar(100) NOT NULL,
    TableName nvarchar(200) NOT NULL,
    DisplayNameFa nvarchar(200) NOT NULL,
    DisplayNameEn nvarchar(200) NOT NULL
);

INSERT INTO @EntityTypes (Code, TableName, DisplayNameFa, DisplayNameEn)
VALUES
    (N'Users', N'Users', N'کاربران', N'Users'),
    (N'Persons', N'Persons', N'اشخاص', N'Persons'),
    (N'Controllers', N'Controllers', N'کنترلرها', N'Controllers'),
    (N'Devices', N'Devices', N'دستگاه‌ها', N'Devices'),
    (N'Points', N'Points', N'نقاط', N'Points'),
    (N'DeviceSchedules', N'DeviceSchedules', N'برنامه‌های دستگاه', N'Device Schedules'),
    (N'Sites', N'Sites', N'سایت‌ها', N'Sites'),
    (N'Buildings', N'Buildings', N'ساختمان‌ها', N'Buildings'),
    (N'Floors', N'Floors', N'طبقات', N'Floors'),
    (N'Wards', N'Wards', N'بخش‌ها', N'Wards'),
    (N'Rooms', N'Rooms', N'اتاق‌ها', N'Rooms');

INSERT INTO dbo.EntityTypes
(
    Id, Code, Name, TableName, DisplayNameFa, DisplayNameEn,
    IsActive, IsDeleted, CreatedAtUtc
)
SELECT
    NEWID(), e.Code, e.DisplayNameEn, e.TableName,
    e.DisplayNameFa, e.DisplayNameEn,
    1, 0, SYSUTCDATETIME()
FROM @EntityTypes e
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.EntityTypes t
    WHERE t.Code = e.Code
);

UPDATE t
SET t.TableName = e.TableName,
    t.DisplayNameFa = e.DisplayNameFa,
    t.DisplayNameEn = e.DisplayNameEn
FROM dbo.EntityTypes t
INNER JOIN @EntityTypes e ON e.Code = t.Code;

COMMIT TRANSACTION;
