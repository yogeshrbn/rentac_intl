-- Per-bill TDS amount on ledger transaction allocation rows.
IF OBJECT_ID(N'dbo.LedgerTransactionDetails', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.LedgerTransactionDetails', 'TdsAmount') IS NULL
BEGIN
    ALTER TABLE dbo.LedgerTransactionDetails ADD TdsAmount DECIMAL(18, 2) NOT NULL CONSTRAINT DF_LedgerTxnDetails_TdsAmount DEFAULT (0);
END
GO

IF OBJECT_ID(N'dbo.LedgerTransactionDetail', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.LedgerTransactionDetail', 'TdsAmount') IS NULL
BEGIN
    ALTER TABLE dbo.LedgerTransactionDetail ADD TdsAmount DECIMAL(18, 2) NOT NULL CONSTRAINT DF_LedgerTxnDetail_TdsAmount DEFAULT (0);
END
GO
