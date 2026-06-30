USE [$AppDb$]
GO

-- When true, billing may use UnitSizeRate (unit-2 / size rate) for this product
-- in addition to the company Billing setting (see BAL/Objects/Billing.cs).
IF COL_LENGTH('dbo.Product', 'ApplyUnit2Rate') IS NULL
BEGIN
    ALTER TABLE dbo.Product ADD ApplyUnit2Rate BIT NOT NULL CONSTRAINT DF_Product_ApplyUnit2Rate DEFAULT (0);
END
GO
