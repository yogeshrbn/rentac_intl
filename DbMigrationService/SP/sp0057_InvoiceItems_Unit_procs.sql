-- Measure / contract bills: line measurement unit (SQFT, SQMTR, etc.)
-- 1) Run PreRun/table048_InvoiceItems_Unit.sql (adds InvoiceItems.Unit when missing).
-- 2) On the target database, alter dbo.p_InvoiceItems_ins (script full definition from SSMS):
--      Add parameter @unit NVARCHAR(20) = NULL (typically last, with a default).
--      Persist to InvoiceItems.Unit in the INSERT (e.g. Unit = NULLIF(LTRIM(RTRIM(@unit)), '')).
-- 3) If edit/print should show stored unit per line, ensure p_invoiceById / p_selectBillItems
--    return InvoiceItems.Unit (or equivalent) mapped to BillingItemDTO.Unit.
--
-- BAL/DAL/BillingDAL.cs AddItem passes @unit for standard invoice line inserts.
USE [$AppDb$]
GO
