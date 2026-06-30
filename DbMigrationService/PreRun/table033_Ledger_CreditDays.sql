USE [$AppDb$]
GO

IF COL_LENGTH('Ledger', 'CreditDays') IS NULL
BEGIN
    ALTER TABLE Ledger ADD CreditDays INT NOT NULL CONSTRAINT DF_Ledger_CreditDays DEFAULT(0);
END
GO
