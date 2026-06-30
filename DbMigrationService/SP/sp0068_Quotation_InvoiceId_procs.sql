-- Quotation.InvoiceId: link bill to quotation after contract measure bill save.
-- Run PreRun/table057_Quotation_InvoiceId.sql first.
USE [$AppDb$]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Set InvoiceId on quotations included in a contract bill (skip rows already billed).
CREATE OR ALTER PROCEDURE dbo.p_Quotation_linkInvoice
    @companyId INT,
    @invoiceId INT,
    @quotationIds NVARCHAR(MAX)
AS
BEGIN
   SET NOCOUNT ON;

    IF @companyId IS NULL OR @companyId <= 0
        THROW 50001, 'Company is required.', 1;

    IF @invoiceId IS NULL OR @invoiceId <= 0
        THROW 50002, 'Invoice is required.', 1;

    IF @quotationIds IS NULL OR LTRIM(RTRIM(@quotationIds)) = ''
        THROW 50003, 'At least one quotation is required.', 1;

    --IF EXISTS (
    --    SELECT 1
    --    FROM dbo.Quotation q
    --    INNER JOIN SplitComma(@quotationIds) s
    --        ON q.QuotationId = TRY_CAST(LTRIM(RTRIM(s.value)) AS INT)
    --    WHERE q.CompanyId = @companyId
    --      AND ISNULL(q.InvoiceId, 0) > 0
    --      AND q.InvoiceId <> @invoiceId
    --)
    --    THROW 50004, 'One or more quotations are already billed.', 1;

    UPDATE q
    SET q.InvoiceId = @invoiceId
    FROM dbo.Quotation q
    INNER JOIN dbo.SplitComma(@quotationIds) s
        ON q.QuotationId = TRY_CAST(LTRIM(RTRIM(s.value)) AS INT)
    WHERE q.CompanyId = @companyId
      AND ISNULL(q.InvoiceId, 0) = 0;
END
GO

-- List quotations for a contract (includes InvoiceId for billed indicator).
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
        ISNULL(q.InvoiceId, 0) AS InvoiceId,
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
