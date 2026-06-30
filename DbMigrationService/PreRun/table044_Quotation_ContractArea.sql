USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Quotation', 'Area') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD Area REAL NULL;
END
GO

IF COL_LENGTH('dbo.Quotation', 'MeasureType') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD MeasureType TINYINT NULL;
END
GO
