USE [$AppDb$]
GO

IF COL_LENGTH('dbo.InvoiceItems', 'Unit') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceItems ADD [Unit] NVARCHAR(20) NULL;
END
GO
