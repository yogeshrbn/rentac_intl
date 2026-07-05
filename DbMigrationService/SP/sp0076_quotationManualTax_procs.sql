CREATE OR ALTER PROCEDURE dbo.p_QuotationManualTax_ins
    @QuotationId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxCode NVARCHAR(20) = NULL,
    @Rate DECIMAL(18, 4),
    @RateType NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.QuotationManualTax (QuotationId, TaxId, TaxName, TaxCode, Rate, RateType)
    VALUES (@QuotationId, @TaxId, @TaxName, @TaxCode, @Rate, @RateType);
END
GO

CREATE OR ALTER PROCEDURE dbo.p_QuotationManualTax_sel
    @QuotationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id,
        m.QuotationId,
        m.TaxId,
        m.TaxName,
        m.TaxCode,
        m.Rate,
        m.RateType
    FROM dbo.QuotationManualTax m
    WHERE m.QuotationId = @QuotationId
    ORDER BY m.TaxName;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_QuotationManualTax_delByQuotation
    @QuotationId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.QuotationManualTax
    WHERE QuotationId = @QuotationId;
END
GO
