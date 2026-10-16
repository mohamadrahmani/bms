/*
    Data point history storage
    SQL Server

    Verify the target database and its backup before running this script.
    Run against the database configured for the API before starting the historian.
*/

IF OBJECT_ID(N'dbo.DataPointHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DataPointHistory
    (
        Id BIGINT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_DataPointHistory PRIMARY KEY CLUSTERED,
        DeviceId UNIQUEIDENTIFIER NOT NULL,
        PointId UNIQUEIDENTIFIER NOT NULL,
        TimestampUtc DATETIME2 NOT NULL,
        ValueString NVARCHAR(256) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.DataPointHistory', N'U') IS NOT NULL
    AND NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.DataPointHistory')
            AND name = N'IX_DataPointHistory_DeviceId_PointId_TimestampUtc'
    )
BEGIN
    CREATE INDEX IX_DataPointHistory_DeviceId_PointId_TimestampUtc
        ON dbo.DataPointHistory (DeviceId, PointId, TimestampUtc);
END;
GO
