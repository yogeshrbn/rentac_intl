-- Invoice: cash bill flag (used by gen bill / billing save)
USE [$AppDb$]
GO

IF COL_LENGTH('Invoice', 'IsCashBill') IS NULL
BEGIN
    ALTER TABLE Invoice ADD IsCashBill BIT NOT NULL CONSTRAINT DF_Invoice_IsCashBill DEFAULT 0;
END
GO
