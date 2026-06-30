USE [$AppDb$]
GO

IF COL_LENGTH('dbo.WorkOrder', 'SezDescription') IS NULL
BEGIN
    ALTER TABLE dbo.WorkOrder ADD SezDescription NVARCHAR(500) NULL;
END
GO
