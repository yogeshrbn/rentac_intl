CREATE OR ALTER PROCEDURE dbo.p_InvoiceTax_ins
    @TaxId INT,
    @Rate NUMERIC(15, 2),
    @Amount NUMERIC(15, 2),
    @InvoiceId BIGINT,
    @ProductId INT = NULL,
    @InvoiceItemId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.InvoiceTax (TaxId, Rate, Amount, InvoiceId, ProductId, InvoiceItemId)
    VALUES (@TaxId, @Rate, @Amount, @InvoiceId, @ProductId, @InvoiceItemId);
END
GO

CREATE OR ALTER PROCEDURE dbo.p_InvoiceTax_selByInvoice
    @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        it.InvoiceTax AS InvoiceTaxId,
        it.InvoiceItemId,
        it.InvoiceId,
        it.ProductId,
        it.TaxId,
        it.Rate,
        it.Amount,
        t.Name,
        t.Code AS TaxCode,
        t.RateType
    FROM dbo.InvoiceTax it
    LEFT JOIN dbo.Taxes t ON t.Id = it.TaxId
    WHERE it.InvoiceId = @InvoiceId
    ORDER BY it.InvoiceItemId, t.Name;
END
GO
