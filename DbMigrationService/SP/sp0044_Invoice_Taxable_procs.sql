-- Invoice.Taxable column: run PreRun/table039_Invoice_Taxable.sql via DbUp first.
--
-- Alter existing procedures (script from SSMS) — do not add new procedure names.
--
-- p_Invoice_ins_v3
--   Add parameter @Taxable NUMERIC(15, 3) = 0
--   INSERT into Invoice must include Taxable column.
--
-- p_Invoice_upd_v3
--   Add parameter @Taxable NUMERIC(15, 3) = 0
--   UPDATE Invoice SET ... Taxable = @Taxable WHERE ...
--
-- p_invoiceById
--   Include Taxable in SELECT so edit/load maps to BillingDTO.Taxable.
--
-- BAL/DAL/BillingDAL.cs passes @Taxable in Add() for insert and update.
USE [$AppDb$]
GO
