-- Link quotations to contracts (many quotations per contract).
IF COL_LENGTH('dbo.Quotation', 'ContractId') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD ContractId INT NULL;
END
GO

-- Optional index for list-by-contract lookups.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_Quotation_CompanyId_ContractId' AND object_id = OBJECT_ID('dbo.Quotation')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Quotation_CompanyId_ContractId
    ON dbo.Quotation (CompanyId, ContractId)
    WHERE ContractId IS NOT NULL;
END
GO

-- Backfill from legacy single-quotation link on Contract.
UPDATE q
SET q.ContractId = c.ContractId
FROM dbo.Quotation q
INNER JOIN dbo.Contract c ON c.QuotationId = q.QuotationId AND c.QuotationId > 0
WHERE (q.ContractId IS NULL OR q.ContractId = 0);
GO
