-- PreviewContractBill.cshtml (contract bill HTML) optionally reads these columns from dbo.p_ContractBillingReport:
--   QuotationType (per row, from Invoice) — when 16, Area / Total Area columns are shown (no per-line Duration column).
--   Area (per line, from InvoiceItems or computed) — line area for contract measure items.
--   ContractArea (optional, invoice- or contract-level area) — fallback for TotalArea when line Area is 0.
-- The full definition of p_ContractBillingReport is not versioned in this repository; alter the procedure
-- in SSMS to SELECT these fields (join Invoice, Contract as needed), then refresh Api/reports.xsd if you use the typed dataset.
SET NOCOUNT ON;
