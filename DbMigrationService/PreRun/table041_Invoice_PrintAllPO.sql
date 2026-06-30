-- Print all PO of billed challans on rent bill (gen bill settings).
USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Invoice', 'PrintAllPO') IS NULL
BEGIN
    ALTER TABLE dbo.Invoice ADD PrintAllPO BIT NOT NULL
        CONSTRAINT DF_Invoice_PrintAllPO DEFAULT 0;
END
GO
