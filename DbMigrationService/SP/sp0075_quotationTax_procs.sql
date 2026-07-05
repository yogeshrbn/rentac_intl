CREATE OR ALTER PROCEDURE dbo.p_QuotationTax_ins
    @QuotationItemId INT,
    @QuotationId INT,
    @ProductId INT,
    @TaxCategoryId INT,
    @TaxId INT,
    @TaxName NVARCHAR(100),
    @TaxCode NVARCHAR(20) = NULL,
    @Rate DECIMAL(18, 4),
    @RateType NVARCHAR(20),
    @Amount DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.QuotationTax (
        QuotationItemId, QuotationId, ProductId, TaxCategoryId,
        TaxId, TaxName, TaxCode, Rate, RateType, Amount
    )
    VALUES (
        @QuotationItemId, @QuotationId, @ProductId, @TaxCategoryId,
        @TaxId, @TaxName, @TaxCode, @Rate, @RateType, @Amount
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.p_QuotationTax_sel
    @QuotationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        qt.Id,
        qt.QuotationItemId,
        qt.QuotationId,
        qt.ProductId,
        qt.TaxCategoryId,
        qt.TaxId,
        qt.TaxName,
        qt.TaxCode,
        qt.Rate,
        qt.RateType,
        qt.Amount
    FROM dbo.QuotationTax qt
    WHERE qt.QuotationId = @QuotationId
    ORDER BY qt.QuotationItemId, qt.TaxName;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_QuotationTax_delByQuotation
    @QuotationId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.QuotationTax
    WHERE QuotationId = @QuotationId;
END
GO
