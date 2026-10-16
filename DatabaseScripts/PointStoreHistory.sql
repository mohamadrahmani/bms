/*
    Point value history flag
    SQL Server

    Run against the intended BmsDb database before deploying the API.
    Existing points default to StoreHistory = 0.
*/

IF COL_LENGTH(N'dbo.Points', N'StoreHistory') IS NULL
BEGIN
    ALTER TABLE dbo.Points
        ADD StoreHistory BIT NOT NULL
            CONSTRAINT DF_Points_StoreHistory DEFAULT (0) WITH VALUES;
END;
GO
