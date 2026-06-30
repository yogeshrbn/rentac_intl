-- Per-line line total mode on QuotationItems (quantity | area); NULL = use quotation header LineTotalMode.
-- Run DDL from PreRun/table055_QuotationItems_LineTotalMode.sql via DbUp first.
--
-- p_QuotationItems_ins
--   @lineTotalMode VARCHAR(20) = NULL — persist on INSERT (optional override)
--
-- p_getQuotationItems
--   SELECT must include LineTotalMode for Dapper → QuotationItemDTO.LineTotalMode
--
-- BAL/DAL/BillingDAL.cs AddQuotationItem passes @lineTotalMode
USE [$AppDb$]
GO
