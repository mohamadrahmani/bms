-- Review the target database and take a backup before running this script.
-- Run in SSMS/sqlcmd before deploying the application changes. Safe to rerun.
-- DeviceType values follow BMS.Domain.Entities.BMS.DeviceType; NULL means global.
-- If the older GUID-based category script was already executed, stop and migrate
-- that data separately with an explicit Code-to-integer-ID mapping.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Points', N'U') IS NULL
    THROW 50001, 'dbo.Points does not exist in the selected database.', 1;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.PointCategories') AND name = N'Id' AND system_type_id = TYPE_ID(N'uniqueidentifier'))
    THROW 50006, 'GUID PointCategories already exist; migrate their IDs explicitly before running this script.', 1;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Points') AND name = N'PointCategoryId' AND system_type_id = TYPE_ID(N'uniqueidentifier'))
    THROW 50007, 'GUID PointCategoryId already exists; migrate its values before running this script.', 1;

IF OBJECT_ID(N'dbo.PointCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PointCategories
    (
        Id int NOT NULL CONSTRAINT PK_PointCategories PRIMARY KEY,
        Code nvarchar(100) NOT NULL,
        Title nvarchar(200) NOT NULL,
        DeviceType int NULL,
        SortOrder int NOT NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_PointCategories_IsDeleted DEFAULT (0),
        CreatedAtUtc datetime2(7) NOT NULL,
        UpdatedAtUtc datetime2(7) NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PointCategories') AND name = N'IX_PointCategories_Code')
    CREATE UNIQUE INDEX IX_PointCategories_Code ON dbo.PointCategories(Code);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PointCategories') AND name = N'IX_PointCategories_DeviceType_SortOrder')
    CREATE INDEX IX_PointCategories_DeviceType_SortOrder ON dbo.PointCategories(DeviceType, SortOrder);

DECLARE @GeneralId int = 1;

IF EXISTS (SELECT 1 FROM dbo.PointCategories WHERE Code = N'General' AND Id <> @GeneralId)
    THROW 50002, 'General category code already has a different id.', 1;

IF EXISTS (SELECT 1 FROM dbo.PointCategories WHERE Id = @GeneralId AND Code <> N'General')
    THROW 50003, 'General category id already belongs to another code.', 1;

IF NOT EXISTS (SELECT 1 FROM dbo.PointCategories WHERE Id = @GeneralId)
    INSERT INTO dbo.PointCategories (Id, Code, Title, DeviceType, SortOrder, IsDeleted, CreatedAtUtc)
    VALUES (@GeneralId, N'General', N'General', NULL, 0, 0, SYSUTCDATETIME());

IF COL_LENGTH(N'dbo.Points', N'PointCategoryId') IS NULL
    EXEC(N'ALTER TABLE dbo.Points ADD PointCategoryId int NULL');

EXEC sys.sp_executesql
    N'UPDATE dbo.Points SET PointCategoryId = @CategoryId WHERE PointCategoryId IS NULL',
    N'@CategoryId int', @CategoryId = @GeneralId;

DECLARE @HasUnknown bit = 0;
EXEC sys.sp_executesql
    N'SELECT @Result = CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Points AS point
        LEFT JOIN dbo.PointCategories AS category ON category.Id = point.PointCategoryId
        WHERE category.Id IS NULL) THEN 1 ELSE 0 END',
    N'@Result bit OUTPUT', @Result = @HasUnknown OUTPUT;
IF @HasUnknown = 1
    THROW 50004, 'Points contain unknown category ids.', 1;

DECLARE @HasMismatch bit = 0;
EXEC sys.sp_executesql
    N'SELECT @Result = CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Points AS point
        JOIN dbo.Devices AS device ON device.Id = point.DeviceId
        JOIN dbo.PointCategories AS category ON category.Id = point.PointCategoryId
        WHERE category.DeviceType IS NOT NULL AND category.DeviceType <> device.Type) THEN 1 ELSE 0 END',
    N'@Result bit OUTPUT', @Result = @HasMismatch OUTPUT;
IF @HasMismatch = 1
    THROW 50005, 'Points contain categories incompatible with their device type.', 1;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Points') AND name = N'PointCategoryId' AND is_nullable = 1)
    EXEC(N'ALTER TABLE dbo.Points ALTER COLUMN PointCategoryId int NOT NULL');

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'dbo.Points') AND name = N'FK_Points_PointCategories_PointCategoryId')
    EXEC(N'ALTER TABLE dbo.Points WITH CHECK ADD CONSTRAINT FK_Points_PointCategories_PointCategoryId
        FOREIGN KEY (PointCategoryId) REFERENCES dbo.PointCategories(Id)');

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Points') AND name = N'IX_Points_DeviceId_PointCategoryId')
    EXEC(N'CREATE INDEX IX_Points_DeviceId_PointCategoryId ON dbo.Points(DeviceId, PointCategoryId)');

COMMIT TRANSACTION;

-- After reviewing the category code, title, device type, and target database,
-- add specific categories with IDs matching PointCategory enum members, e.g.
-- only after adding AHU_Temperatures = 2 to the backend enum:
-- INSERT INTO dbo.PointCategories (Id, Code, Title, DeviceType, SortOrder, IsDeleted, CreatedAtUtc)
-- VALUES (2, N'AHU_Temperatures', N'Temperatures', 2, 10, 0, SYSUTCDATETIME());
