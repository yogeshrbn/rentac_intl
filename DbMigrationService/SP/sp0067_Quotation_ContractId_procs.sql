-- Quotation.ContractId: list by contract + DBA notes for insert/update/select procs.
-- Run PreRun/table056_Quotation_ContractId.sql first.
--
-- Full bodies for p_Quotation_ins / p_Quotation_upd / p_getQuotation_byId are not fully versioned in this repo.
-- Export current definitions from SQL Server, merge the changes below, then deploy (CREATE OR ALTER).
--
-- p_Quotation_ins / p_Quotation_upd
--   Add parameter (match BAL/DAL/BillingDAL.cs AddQuotation when ContractId > 0):
--     @contractId INT = NULL
--   INSERT/UPDATE dbo.Quotation must set:
--     ContractId = NULLIF(@contractId, 0)
--
-- p_getQuotation_byId (and p_getQuotation_byNumber if applicable)
--   Add to SELECT list (alias to match QuotationDTO / LedgerTransactionDTO):
--     q.ContractId
--
USE [$AppDb$]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- List quotations for a contract: by Quotation.ContractId or legacy Contract.QuotationId.
CREATE OR ALTER PROCEDURE dbo.p_quotations_selByContractId
    @contractId INT,
    @companyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        q.QuotationId,
        q.CompanyId,
        q.QuotationNumber,
        q.QuotationDate,
        q.LedgerId,
        q.LedgerSiteId,
        q.QuotationType,
        ISNULL(q.SubTotal, 0) AS SubTotal,
        ISNULL(q.Total, 0) AS Total,
        ISNULL(q.TaxAmount, 0) AS TaxAmount,
        ISNULL(q.Freight, 0) AS Freight,
        ISNULL(q.FreightTax, 0) AS FreightTax,
        q.[From],
        q.[To],
        ISNULL(q.Area, 0) AS Area,
        ISNULL(q.MeasureType, 0) AS MeasureType,
        q.LineTotalMode,
        q.PartyType,
        q.UnregisteredPartyPhone,
        q.UnregisteredPartyName,
        q.UnregisteredPartyAddress,
        q.Category,
        ISNULL(q.ContractId, 0) AS ContractId,
        CASE
            WHEN q.LedgerId > 0 THEN ISNULL(l.Name, '')
            ELSE ISNULL(q.UnregisteredPartyName, '')
        END AS Client
    FROM dbo.Quotation q
    LEFT JOIN dbo.Ledger l ON l.LedgerId = q.LedgerId
    WHERE q.CompanyId = @companyId
      AND (
          q.ContractId = @contractId
          OR EXISTS (
              SELECT 1
              FROM dbo.Contract c
              WHERE c.CompanyId = @companyId
                AND c.ContractId = @contractId
                AND c.QuotationId = q.QuotationId
          )
      )
    ORDER BY q.QuotationDate DESC, q.QuotationId DESC;
END
GO
