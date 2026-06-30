-- Per-quotation line total calculation mode (quantity | area).
-- Run DDL from PreRun/table046_Quotation_LineTotalMode.sql via DbUp first.
--
-- p_Quotation_ins / p_Quotation_upd
--   @lineTotalMode VARCHAR(20) = 'quantity'
--
-- p_getQuotation_byId
--   SELECT LineTotalMode
--
-- BAL/DAL/BillingDAL.cs AddQuotation passes @lineTotalMode
USE [$AppDb$]
GO
