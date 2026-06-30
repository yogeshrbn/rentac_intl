USE [$AppDb$]
GO

IF COL_LENGTH('dbo.QuotationItems', 'Area') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationItems ADD Area REAL NULL;
END
GO
