CREATE OR ALTER PROCEDURE dbo.p_WorkOrderTax_ins
    @WorkOrderItemId INT,
    @SiteId INT,
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

    INSERT INTO dbo.WorkOrderTax (
        WorkOrderItemId, SiteId, ProductId, TaxCategoryId,
        TaxId, TaxName, TaxCode, Rate, RateType, Amount
    )
    VALUES (
        @WorkOrderItemId, @SiteId, @ProductId, @TaxCategoryId,
        @TaxId, @TaxName, @TaxCode, @Rate, @RateType, @Amount
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.p_WorkOrderTax_sel
    @SiteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        wt.Id,
        wt.WorkOrderItemId,
        wt.SiteId,
        wt.ProductId,
        wt.TaxCategoryId,
        wt.TaxId,
        wt.TaxName,
        wt.TaxCode,
        wt.Rate,
        wt.RateType,
        wt.Amount
    FROM dbo.WorkOrderTax wt
    WHERE wt.SiteId = @SiteId
    ORDER BY wt.WorkOrderItemId, wt.TaxName;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_WorkOrderTax_delBySite
    @SiteId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.WorkOrderTax
    WHERE SiteId = @SiteId;
END
GO
