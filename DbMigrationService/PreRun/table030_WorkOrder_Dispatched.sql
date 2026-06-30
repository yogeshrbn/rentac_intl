USE [$AppDb$]
GO

IF COL_LENGTH('WorkOrder', 'dispatched') IS NULL
BEGIN
    ALTER TABLE WorkOrder
    ADD dispatched BIT NOT NULL CONSTRAINT DF_WorkOrder_dispatched DEFAULT(0);
END
GO

IF COL_LENGTH('WorkOrder', 'dispatchedDate') IS NULL
BEGIN
    ALTER TABLE WorkOrder
    ADD dispatchedDate DATETIME NULL;
END
GO
