/*
    Point raw and display scaling ranges
    SQL Server

    Run against the intended BmsDb database before deploying the API.
    Existing points keep NULL ranges until an operator edits them.
*/

IF COL_LENGTH(N'dbo.Points', N'MinRawValue') IS NULL
    ALTER TABLE dbo.Points ADD MinRawValue FLOAT NULL;
GO

IF COL_LENGTH(N'dbo.Points', N'MaxRawValue') IS NULL
    ALTER TABLE dbo.Points ADD MaxRawValue FLOAT NULL;
GO

IF COL_LENGTH(N'dbo.Points', N'MinDisplayValue') IS NULL
    ALTER TABLE dbo.Points ADD MinDisplayValue FLOAT NULL;
GO

IF COL_LENGTH(N'dbo.Points', N'MaxDisplayValue') IS NULL
    ALTER TABLE dbo.Points ADD MaxDisplayValue FLOAT NULL;
GO
