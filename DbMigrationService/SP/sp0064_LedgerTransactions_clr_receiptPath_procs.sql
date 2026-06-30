IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_LedgerTransactions_clr_receiptPath' AND type = 'P')
    DROP PROCEDURE dbo.p_LedgerTransactions_clr_receiptPath;
GO

CREATE PROCEDURE dbo.p_LedgerTransactions_clr_receiptPath
    @LedgerTransactionId INT,
    @companyId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.LedgerTransactions
    SET ReceiptDocumentPath = NULL
    WHERE LedgerTransactionId = @LedgerTransactionId
      AND companyId = @companyId;
END
GO
