CREATE OR ALTER PROCEDURE dbo.p_taxCategory_ins
    @TaxName NVARCHAR(50),
    @CompanyId INT = NULL,
    @CreatedBy INT = NULL,
    @CreatedOn DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TaxCategoryId INT;

    SELECT @TaxCategoryId = ISNULL(MAX(TaxCategoryId), 0) + 1
    FROM dbo.LookupTaxCategory;

    INSERT INTO dbo.LookupTaxCategory (
        TaxCategoryId,
        TaxName,
        CompanyId,
        CreatedOn,
        CreatedBy
    )
    VALUES (
        @TaxCategoryId,
        @TaxName,
        @CompanyId,
        @CreatedOn,
        @CreatedBy
    );

    SELECT @TaxCategoryId AS TaxCategoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategory_upd
    @TaxCategoryId INT,
    @TaxName NVARCHAR(50),
    @ModifiedBy INT = NULL,
    @ModifiedOn DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.LookupTaxCategory
    SET
        TaxName = @TaxName,
        ModifiedBy = @ModifiedBy,
        ModifiedOn = @ModifiedOn
    WHERE TaxCategoryId = @TaxCategoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategory_sel
    @TaxCategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        TaxCategoryId,
        TaxName,
        CompanyId,
        CreatedOn,
        CreatedBy,
        ModifiedOn,
        ModifiedBy
    FROM dbo.LookupTaxCategory
    WHERE TaxCategoryId = @TaxCategoryId;
END
GO
