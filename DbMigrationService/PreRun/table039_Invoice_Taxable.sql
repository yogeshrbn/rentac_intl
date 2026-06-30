-- Pre-tax taxable value on invoice (rent/sale bills).
IF COL_LENGTH('dbo.Invoice', 'Taxable') IS NULL
BEGIN
    ALTER TABLE dbo.Invoice ADD Taxable NUMERIC(15, 3) NOT NULL
        CONSTRAINT DF_Invoice_Taxable DEFAULT 0;
END
GO
