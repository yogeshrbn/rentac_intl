USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Quotation', 'LineTotalMode') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD LineTotalMode VARCHAR(20) NULL;
END
GO
