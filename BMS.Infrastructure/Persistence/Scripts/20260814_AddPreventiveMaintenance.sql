SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'[dbo].[PmSchedules]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PmSchedules]
    (
        [Id] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [DueDateUtc] datetime2(7) NOT NULL,
        [WarningDays] int NOT NULL,
        [IsActive] bit NOT NULL CONSTRAINT [DF_PmSchedules_IsActive] DEFAULT (1),
        [CreatedByUserId] uniqueidentifier NULL,
        [UpdatedByUserId] uniqueidentifier NULL,
        [ClosedAtUtc] datetime2(7) NULL,
        [ClosedByUserId] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_PmSchedules_IsDeleted] DEFAULT (0),
        [CreatedAtUtc] datetime2(7) NOT NULL,
        [UpdatedAtUtc] datetime2(7) NULL,

        CONSTRAINT [PK_PmSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PmSchedules_WarningDays] CHECK ([WarningDays] >= 0),
        CONSTRAINT [FK_PmSchedules_Devices_DeviceId]
            FOREIGN KEY ([DeviceId]) REFERENCES [dbo].[Devices] ([Id]),
        CONSTRAINT [FK_PmSchedules_Users_CreatedBy]
            FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
        CONSTRAINT [FK_PmSchedules_Users_UpdatedBy]
            FOREIGN KEY ([UpdatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
        CONSTRAINT [FK_PmSchedules_Users_ClosedBy]
            FOREIGN KEY ([ClosedByUserId]) REFERENCES [dbo].[Users] ([Id])
    );

    CREATE UNIQUE INDEX [UX_PmSchedules_DeviceId_Active]
        ON [dbo].[PmSchedules] ([DeviceId])
        WHERE [IsActive] = 1 AND [IsDeleted] = 0;

    CREATE INDEX [IX_PmSchedules_DeviceId_CreatedAtUtc]
        ON [dbo].[PmSchedules] ([DeviceId], [CreatedAtUtc]);

    CREATE INDEX [IX_PmSchedules_IsActive_DueDateUtc]
        ON [dbo].[PmSchedules] ([IsActive], [DueDateUtc]);

    CREATE INDEX [IX_PmSchedules_IsDeleted]
        ON [dbo].[PmSchedules] ([IsDeleted]);

    CREATE INDEX [IX_PmSchedules_CreatedAtUtc]
        ON [dbo].[PmSchedules] ([CreatedAtUtc]);
END;

IF OBJECT_ID(N'[dbo].[PmServiceHistories]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PmServiceHistories]
    (
        [Id] uniqueidentifier NOT NULL,
        [PmScheduleId] uniqueidentifier NOT NULL,
        [Status] tinyint NOT NULL,
        [ActionDateUtc] datetime2(7) NOT NULL,
        [Description] nvarchar(4000) NULL,
        [PerformedByUserId] uniqueidentifier NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [DueDateUtcSnapshot] datetime2(7) NOT NULL,
        [TitleSnapshot] nvarchar(200) NOT NULL,
        [BaseDescriptionSnapshot] nvarchar(2000) NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_PmServiceHistories_IsDeleted] DEFAULT (0),
        [CreatedAtUtc] datetime2(7) NOT NULL,
        [UpdatedAtUtc] datetime2(7) NULL,

        CONSTRAINT [PK_PmServiceHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PmServiceHistories_Status] CHECK ([Status] IN (1, 2)),
        CONSTRAINT [FK_PmServiceHistories_PmSchedules_PmScheduleId]
            FOREIGN KEY ([PmScheduleId]) REFERENCES [dbo].[PmSchedules] ([Id]),
        CONSTRAINT [FK_PmServiceHistories_Users_PerformedBy]
            FOREIGN KEY ([PerformedByUserId]) REFERENCES [dbo].[Users] ([Id]),
        CONSTRAINT [FK_PmServiceHistories_Users_CreatedBy]
            FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])
    );

    CREATE UNIQUE INDEX [UX_PmServiceHistories_PmScheduleId]
        ON [dbo].[PmServiceHistories] ([PmScheduleId]);

    CREATE INDEX [IX_PmServiceHistories_ActionDateUtc]
        ON [dbo].[PmServiceHistories] ([ActionDateUtc]);

    CREATE INDEX [IX_PmServiceHistories_IsDeleted]
        ON [dbo].[PmServiceHistories] ([IsDeleted]);

    CREATE INDEX [IX_PmServiceHistories_CreatedAtUtc]
        ON [dbo].[PmServiceHistories] ([CreatedAtUtc]);
END;

IF OBJECT_ID(N'[dbo].[PmAttachments]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[PmAttachments]
    (
        [Id] uniqueidentifier NOT NULL,
        [PmScheduleId] uniqueidentifier NULL,
        [PmServiceHistoryId] uniqueidentifier NULL,
        [FileName] nvarchar(255) NOT NULL,
        [ContentType] nvarchar(150) NOT NULL,
        [FileExtension] nvarchar(20) NULL,
        [FileSize] bigint NOT NULL,
        [FileContent] varbinary(max) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_PmAttachments_IsDeleted] DEFAULT (0),
        [CreatedAtUtc] datetime2(7) NOT NULL,
        [UpdatedAtUtc] datetime2(7) NULL,

        CONSTRAINT [PK_PmAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PmAttachments_FileSize] CHECK ([FileSize] >= 0),
        CONSTRAINT [CK_PmAttachments_ExactlyOneOwner] CHECK
        (
            ([PmScheduleId] IS NOT NULL AND [PmServiceHistoryId] IS NULL)
            OR
            ([PmScheduleId] IS NULL AND [PmServiceHistoryId] IS NOT NULL)
        ),
        CONSTRAINT [FK_PmAttachments_PmSchedules_PmScheduleId]
            FOREIGN KEY ([PmScheduleId]) REFERENCES [dbo].[PmSchedules] ([Id]),
        CONSTRAINT [FK_PmAttachments_PmServiceHistories_PmServiceHistoryId]
            FOREIGN KEY ([PmServiceHistoryId]) REFERENCES [dbo].[PmServiceHistories] ([Id]),
        CONSTRAINT [FK_PmAttachments_Users_CreatedBy]
            FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])
    );

    CREATE INDEX [IX_PmAttachments_PmScheduleId]
        ON [dbo].[PmAttachments] ([PmScheduleId]);

    CREATE INDEX [IX_PmAttachments_PmServiceHistoryId]
        ON [dbo].[PmAttachments] ([PmServiceHistoryId]);

    CREATE INDEX [IX_PmAttachments_IsDeleted]
        ON [dbo].[PmAttachments] ([IsDeleted]);

    CREATE INDEX [IX_PmAttachments_CreatedAtUtc]
        ON [dbo].[PmAttachments] ([CreatedAtUtc]);
END;

COMMIT TRANSACTION;
