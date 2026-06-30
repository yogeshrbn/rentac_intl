USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Contract', 'ApproximateWeight') IS NULL
BEGIN
    ALTER TABLE dbo.Contract ADD ApproximateWeight REAL NULL;
END
GO
