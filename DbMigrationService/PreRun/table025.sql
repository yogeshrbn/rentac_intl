-- GRN (receiving challan): GR Number
USE [$AppDb$]
GO

IF COL_LENGTH('GRN', 'GRNumber') IS NULL
BEGIN
    ALTER TABLE GRN ADD GRNumber NVARCHAR(50) NULL;
END
GO
