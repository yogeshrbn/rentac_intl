-- Link contract bill back to source quotation(s): Quotation.InvoiceId (not Invoice.QuotationId).
IF COL_LENGTH('dbo.Quotation', 'InvoiceId') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD InvoiceId INT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Quotation_CompanyId_InvoiceId'
      AND object_id = OBJECT_ID('dbo.Quotation')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Quotation_CompanyId_InvoiceId
    ON dbo.Quotation (CompanyId, InvoiceId)
    WHERE InvoiceId IS NOT NULL;
END
GO
