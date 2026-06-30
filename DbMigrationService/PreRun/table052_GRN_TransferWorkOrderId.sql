-- Links a transfer GRN (ChallanType 12) to the issued WorkOrder for edit/load.
IF COL_LENGTH('dbo.GRN', 'TransferWorkOrderId') IS NULL
BEGIN
    ALTER TABLE dbo.GRN ADD TransferWorkOrderId INT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_GRN_TransferWorkOrderId' AND object_id = OBJECT_ID('dbo.GRN')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_GRN_TransferWorkOrderId ON dbo.GRN (TransferWorkOrderId)
    WHERE TransferWorkOrderId IS NOT NULL;
END
GO
