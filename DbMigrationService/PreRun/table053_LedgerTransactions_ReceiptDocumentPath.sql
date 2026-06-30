-- Receipt image (permanent path) for cash receipt / quick receipt vouchers
IF COL_LENGTH('dbo.LedgerTransactions', 'ReceiptDocumentPath') IS NULL
BEGIN
    ALTER TABLE dbo.LedgerTransactions ADD ReceiptDocumentPath NVARCHAR(500) NULL;
END
GO
