/*
    Purchase header: WarehouseId (see PreRun/table037_Purchase_WarehouseId.sql).

    Full bodies of p_Purchase_ins, p_Purchase_upd and p_Purchase_byId are not in this repo.
    Apply the following in SSMS (or your procedure source) so BAL/DAL/PurchaseDAL.cs matches the database.

    Parameter name matches GRN/WorkOrder style: @warehouseId (INT).

    1) p_Purchase_ins
       - Add input parameter: @warehouseId INT = NULL
       - INSERT into dbo.Purchase including WarehouseId (or map NULL when not provided).

    2) p_Purchase_upd
       - Add input parameter: @warehouseId INT = NULL
       - UPDATE dbo.Purchase SET ... WarehouseId = @warehouseId (or equivalent).

    3) p_Purchase_byId
       - Include WarehouseId in the SELECT list so Dapper maps to PurchaseDTO.WarehouseId for edit screens.
*/
USE [$AppDb$]
GO
