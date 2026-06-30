-- Voucher-level TDS total for quick receipt / payment allocations.
IF OBJECT_ID(N'dbo.LedgerTransactions', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.LedgerTransactions', 'TotalTds') IS NULL
    BEGIN
        ALTER TABLE dbo.LedgerTransactions ADD TotalTds DECIMAL(18, 2) NOT NULL CONSTRAINT DF_LedgerTransactions_TotalTds DEFAULT (0);
    END
END
GO
