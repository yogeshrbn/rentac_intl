-- Print all PO of billed challans on rent bill (gen bill settings).
USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Invoice', 'PONumbers') IS NULL
BEGIN
    ALTER TABLE dbo.Invoice 
       Add PONumbers nvarchar(50) NULL
END
GO
