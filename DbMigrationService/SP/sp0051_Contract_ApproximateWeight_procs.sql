-- Approximate weight on Contract. Run DDL from PreRun/table043_Contract_ApproximateWeight.sql via DbUp first.
--
-- Alter existing procedures (script from SSMS) — do not add new procedure names.
--
-- p_contract_ins
--   Add parameter @approximateWeight REAL = NULL
--   INSERT into Contract must set ApproximateWeight column.
--
-- p_contract_upd
--   Add parameter @approximateWeight REAL = NULL
--   UPDATE Contract SET ... ApproximateWeight = @approximateWeight WHERE ...
--
-- p_contract_byId
--   Include ApproximateWeight in SELECT so edit screen loads the field.
--
-- BAL/DAL/ContractDAL.cs passes @approximateWeight for both insert and update.
USE [$AppDb$]
GO
