USE [$AppDb$]

GO



IF COL_LENGTH('dbo.WorkOrder', 'ShipFrom') IS NULL

BEGIN

    ALTER TABLE dbo.WorkOrder ADD ShipFrom NVARCHAR(500) NULL;

END

GO

