USE [$AppDb$]
GO

IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_LedgerTransactions_setContractId' AND type = 'P')
    DROP PROCEDURE dbo.p_LedgerTransactions_setContractId;
GO

CREATE PROCEDURE dbo.p_LedgerTransactions_setContractId
    @LedgerTransactionId INT,
    @CompanyId INT,
    @ContractId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.LedgerTransactions
    SET ContractId = @ContractId
    WHERE LedgerTransactionId = @LedgerTransactionId
      AND CompanyId = @CompanyId;
END
GO

IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_LedgerTransactions_sel_contractReceipts' AND type = 'P')
    DROP PROCEDURE dbo.p_LedgerTransactions_sel_contractReceipts;
GO

CREATE PROCEDURE dbo.p_LedgerTransactions_sel_contractReceipts
    @ContractId INT,
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        lt.LedgerTransactionId,
        lt.TransactionDate,
        lt.TransactionAmount,
        lt.ReceiptNumber,
        lt.Narration,
        lt.TranRefNumber,
        lt.TransactionMode,
        lt.EntryType,
        lt.TransactionType,
        lt.Description,
        lt.LedgerId,
        lt.LedgerSiteId,
        lt.ContractId,
        ls.SiteAddress AS Site
    FROM dbo.LedgerTransactions lt
    LEFT JOIN dbo.ClientSites ls ON ls.LedgerSiteId = lt.LedgerSiteId
    WHERE lt.ContractId = @ContractId
      AND lt.CompanyId = @CompanyId
      AND lt.EntryType = 8
      AND lt.TransactionType = 2
      AND ISNULL(lt.TransactionStatus, 0) <> 2
    ORDER BY lt.TransactionDate DESC, lt.LedgerTransactionId DESC;
END
GO
