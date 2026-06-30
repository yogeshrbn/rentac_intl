-- Line-level Area on QuotationItems (From/To already used by p_QuotationItems_ins).
-- Run DDL from PreRun/table045_QuotationItems_LineArea.sql via DbUp first.
--
-- p_QuotationItems_ins
--   @area REAL = NULL — persist on INSERT
--
-- p_getQuotationItems
--   SELECT must include Area, From, To for Dapper → QuotationItemDTO
--
-- BAL/DAL/BillingDAL.cs AddQuotationItem passes @area
USE [$AppDb$]
GO
