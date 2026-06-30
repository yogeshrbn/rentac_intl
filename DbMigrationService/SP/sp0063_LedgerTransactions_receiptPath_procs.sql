IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_LedgerTransactions_upd_receiptPath' AND type = 'P')
    DROP PROCEDURE dbo.p_LedgerTransactions_upd_receiptPath;
GO

CREATE PROCEDURE dbo.p_LedgerTransactions_upd_receiptPath
    @LedgerTransactionId INT,
    @companyId INT,
    @path NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.LedgerTransactions
    SET ReceiptDocumentPath = @path
    WHERE LedgerTransactionId = @LedgerTransactionId
      AND companyId = @companyId;
END
GO
