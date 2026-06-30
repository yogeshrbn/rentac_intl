-- Add UseTradeNameForBilling column to Ledger
USE [$AppDb$]
GO

IF COL_LENGTH('Ledger', 'UseTradeNameForBilling') IS NULL
BEGIN
    ALTER TABLE Ledger ADD UseTradeNameForBilling TINYINT NOT NULL DEFAULT 0;
END
GO
