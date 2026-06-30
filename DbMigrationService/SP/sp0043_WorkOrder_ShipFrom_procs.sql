-- Ship From on WorkOrder (issue challan). Run DDL from PreRun/table038_WorkOrder_ShipFrom.sql via DbUp first.
--
-- Alter existing procedures (script from SSMS) — do not add new procedure names.
--
-- p_WorkOrder_insV1
--   Add parameter @shipFrom NVARCHAR(500) = NULL
--   INSERT into WorkOrder must set ShipFrom column.
--
-- p_WorkOrder_upd
--   Add parameter @shipFrom NVARCHAR(500) = NULL
--   UPDATE WorkOrder SET ... ShipFrom = @shipFrom WHERE ...
--
-- p_WorkOrder_sel
--   Include ShipFrom in SELECT so edit screen loads the field.
--
-- BAL/DAL/WorkOrderDAL.cs passes @shipFrom for both insert and update.
USE [$AppDb$]
GO
