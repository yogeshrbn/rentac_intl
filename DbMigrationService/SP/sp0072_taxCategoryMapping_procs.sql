CREATE OR ALTER PROCEDURE dbo.p_taxCategoryMapping_all
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id,
        m.TaxCategoryId,
        m.TaxId,
        m.IsDefault,
        t.Name AS TaxName,
        t.Code AS TaxCode,
        t.Rate,
        t.RateType
    FROM dbo.TaxCategoryMappings m
    INNER JOIN dbo.Taxes t ON t.Id = m.TaxId
    ORDER BY m.TaxCategoryId, t.Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategoryMapping_selByCategory
    @TaxCategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.Id,
        m.TaxCategoryId,
        m.TaxId,
        m.IsDefault,
        t.Name AS TaxName,
        t.Code AS TaxCode,
        t.Rate,
        t.RateType
    FROM dbo.TaxCategoryMappings m
    INNER JOIN dbo.Taxes t ON t.Id = m.TaxId
    WHERE m.TaxCategoryId = @TaxCategoryId
    ORDER BY t.Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategoryMapping_delByCategory
    @TaxCategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.TaxCategoryMappings
    WHERE TaxCategoryId = @TaxCategoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategoryMapping_ins
    @TaxCategoryId INT,
    @TaxId INT,
    @IsDefault BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TaxCategoryMappings (TaxCategoryId, TaxId, IsDefault)
    VALUES (@TaxCategoryId, @TaxId, @IsDefault);
END
GO

CREATE OR ALTER PROCEDURE dbo.p_taxCategory_del
    @TaxCategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    DELETE FROM dbo.TaxCategoryMappings
    WHERE TaxCategoryId = @TaxCategoryId;

    DELETE FROM dbo.LookupTaxCategory
    WHERE TaxCategoryId = @TaxCategoryId;

    COMMIT TRANSACTION;
END
GO
