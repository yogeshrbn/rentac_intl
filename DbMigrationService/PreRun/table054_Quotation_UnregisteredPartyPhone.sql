USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Quotation', 'UnregisteredPartyPhone') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD UnregisteredPartyPhone NVARCHAR(50) NULL;
END
GO
