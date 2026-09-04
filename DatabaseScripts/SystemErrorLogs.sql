/*
    System error log module
    SQL Server

    Run this script against the BmsDb database before starting the API.
    The script is idempotent for the table, permissions and SuperAdmin grants.
*/

IF OBJECT_ID(N'dbo.SystemErrorLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemErrorLogs
    (
        Id UNIQUEIDENTIFIER NOT NULL,
        IsDeleted BIT NOT NULL
            CONSTRAINT DF_SystemErrorLogs_IsDeleted DEFAULT (0),
        CreatedAtUtc DATETIME2 NOT NULL,
        UpdatedAtUtc DATETIME2 NULL,

        ErrorId UNIQUEIDENTIFIER NOT NULL,
        OccurredAtUtc DATETIME2 NOT NULL,
        ExceptionType NVARCHAR(512) NOT NULL,
        Message NVARCHAR(MAX) NOT NULL,
        StackTrace NVARCHAR(MAX) NULL,
        InnerException NVARCHAR(MAX) NULL,

        RequestPath NVARCHAR(2048) NULL,
        HttpMethod NVARCHAR(16) NULL,
        QueryString NVARCHAR(4000) NULL,
        RequestBody NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(64) NULL,
        UserAgent NVARCHAR(1024) NULL,
        StatusCode INT NULL,
        UserId UNIQUEIDENTIFIER NULL,
        EnvironmentName NVARCHAR(128) NULL,

        IsResolved BIT NOT NULL
            CONSTRAINT DF_SystemErrorLogs_IsResolved DEFAULT (0),
        ResolvedAtUtc DATETIME2 NULL,
        ResolvedByUserId UNIQUEIDENTIFIER NULL,
        ResolutionNote NVARCHAR(2000) NULL,

        CONSTRAINT PK_SystemErrorLogs PRIMARY KEY (Id),
        CONSTRAINT UQ_SystemErrorLogs_ErrorId UNIQUE (ErrorId)
    );

    CREATE INDEX IX_SystemErrorLogs_IsDeleted
        ON dbo.SystemErrorLogs (IsDeleted);

    CREATE INDEX IX_SystemErrorLogs_CreatedAtUtc
        ON dbo.SystemErrorLogs (CreatedAtUtc);

    CREATE INDEX IX_SystemErrorLogs_OccurredAtUtc_IsResolved
        ON dbo.SystemErrorLogs (OccurredAtUtc, IsResolved);

    CREATE INDEX IX_SystemErrorLogs_UserId
        ON dbo.SystemErrorLogs (UserId);
END;
GO

