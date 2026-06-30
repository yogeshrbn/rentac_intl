IF COL_LENGTH('dbo.Purchase', 'WarehouseId') IS NULL
BEGIN
    ALTER TABLE dbo.Purchase
    ADD WarehouseId INT NULL;
END
GO
