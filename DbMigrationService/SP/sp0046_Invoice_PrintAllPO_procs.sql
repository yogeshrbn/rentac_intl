-- Invoice.PrintAllPO column: run PreRun/table041_Invoice_PrintAllPO.sql via DbUp first.
--
-- Alter existing procedures (script from SSMS) — do not add new procedure names.
--
-- p_Invoice_ins_v3
--   Add parameter @printAllPO BIT = 0
--   INSERT into Invoice must include PrintAllPO column.
--
-- p_Invoice_upd_v3
--   Add parameter @printAllPO BIT = 0
--   UPDATE Invoice SET ... PrintAllPO = @printAllPO WHERE ...
--
-- p_invoiceById
--   Include PrintAllPO in SELECT so edit screen loads the flag.
--
-- BAL/DAL/BillingDAL.cs passes @printAllPO for insert and update.
USE [$AppDb$]
GO
