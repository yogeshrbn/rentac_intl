USE [$AppDb$]
GO

IF COL_LENGTH('dbo.LedgerTransactions', 'ContractId') IS NULL
BEGIN
    ALTER TABLE dbo.LedgerTransactions ADD ContractId INT NULL;
END
GO
