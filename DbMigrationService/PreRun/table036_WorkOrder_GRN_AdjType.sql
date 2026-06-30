USE [$AppDb$]
GO

-- AdjType: 1 = Rent, 2 = Hire (material adjustment and related flows)
IF COL_LENGTH('dbo.WorkOrder', 'AdjType') IS NULL
BEGIN
    ALTER TABLE dbo.WorkOrder ADD AdjType TINYINT NOT NULL CONSTRAINT DF_WorkOrder_AdjType DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.GRN', 'AdjType') IS NULL
BEGIN
    ALTER TABLE dbo.GRN ADD AdjType TINYINT NOT NULL CONSTRAINT DF_GRN_AdjType DEFAULT (1);
END
GO
