USE [$AppDb$]
GO

IF COL_LENGTH('dbo.QuotationItems', 'LineTotalMode') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationItems ADD LineTotalMode VARCHAR(20) NULL;
END
GO
