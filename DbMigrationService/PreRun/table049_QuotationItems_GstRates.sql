USE [$AppDb$]
GO

IF COL_LENGTH('dbo.QuotationItems', 'IGSTRate') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationItems ADD IGSTRate DECIMAL(9, 4) NULL;
END
GO

IF COL_LENGTH('dbo.QuotationItems', 'CGSTRate') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationItems ADD CGSTRate DECIMAL(9, 4) NULL;
END
GO

IF COL_LENGTH('dbo.QuotationItems', 'SGSTRate') IS NULL
BEGIN
    ALTER TABLE dbo.QuotationItems ADD SGSTRate DECIMAL(9, 4) NULL;
END
GO
