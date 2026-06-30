-- Site (issue challan): GR Number
USE [$AppDb$]
GO


IF COL_LENGTH('SiteInfo', 'GRNumber') IS NULL
BEGIN
    ALTER TABLE SiteInfo ADD GRNumber NVARCHAR(50) NULL;
END
GO
