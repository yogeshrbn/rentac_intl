-- Contract quotation: Area + MeasureType on Quotation header; Period uses existing From/To columns.
-- Run DDL from PreRun/table044_Quotation_ContractArea.sql via DbUp first.
--
-- Alter existing procedures (script from SSMS) — do not add new procedure names.
--
-- p_Quotation_ins
--   @area          REAL = NULL
--   @measureType   TINYINT = NULL
--   INSERT must set Area, MeasureType (and existing @From, @To).
--
-- p_Quotation_upd
--   Same parameters; UPDATE Quotation SET Area = @area, MeasureType = @measureType, ...
--
-- p_getQuotation_byId
--   SELECT must include Area, MeasureType, From, To (for Dapper → QuotationDTO).
--
-- BAL/DAL/BillingDAL.cs AddQuotation passes @area, @measureType.
USE [$AppDb$]
GO
