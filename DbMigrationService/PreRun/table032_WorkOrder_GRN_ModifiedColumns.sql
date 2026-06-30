USE [$AppDb$]
GO

IF COL_LENGTH('WorkOrder', 'modifiedBy') IS NULL
BEGIN
    ALTER TABLE WorkOrder
    ADD modifiedBy INT NULL;
END
GO

IF COL_LENGTH('WorkOrder', 'modifiedDate') IS NULL
BEGIN
    ALTER TABLE WorkOrder
    ADD modifiedDate DATETIME NULL;
END
GO

IF COL_LENGTH('WorkOrder', 'ModifiedOn') IS NULL
BEGIN
    ALTER TABLE WorkOrder
    ADD ModifiedOn DATETIME NULL;
END
GO

IF COL_LENGTH('GRN', 'modifiedBy') IS NULL
BEGIN
    ALTER TABLE GRN
    ADD modifiedBy INT NULL;
END
GO

IF COL_LENGTH('GRN', 'modifiedDate') IS NULL
BEGIN
    ALTER TABLE GRN
    ADD modifiedDate DATETIME NULL;
END
GO

IF COL_LENGTH('GRN', 'ModifiedOn') IS NULL
BEGIN
    ALTER TABLE GRN
    ADD ModifiedOn DATETIME NULL;
END
GO
