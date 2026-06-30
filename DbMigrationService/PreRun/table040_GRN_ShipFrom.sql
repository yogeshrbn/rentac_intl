-- Ship From on GRN (receive items / inv_grn).
IF COL_LENGTH('dbo.GRN', 'ShipFrom') IS NULL
BEGIN
    ALTER TABLE dbo.GRN ADD ShipFrom NVARCHAR(500) NULL;
END
GO
