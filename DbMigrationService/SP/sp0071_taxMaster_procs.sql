CREATE OR ALTER PROCEDURE dbo.p_TaxMaster_all
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Code,
        Country,
        Description,
        Rate,
        RateType,
        IsActive,
        IsCompound,
        IsDefault,
        EffectiveFrom,
        EffectiveTo,
        ApplicableTo,
        CustomerType,
        Location,
        MinAmount,
        MaxAmount,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    FROM dbo.Taxes
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_TaxMaster_sel
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Code,
        Country,
        Description,
        Rate,
        RateType,
        IsActive,
        IsCompound,
        IsDefault,
        EffectiveFrom,
        EffectiveTo,
        ApplicableTo,
        CustomerType,
        Location,
        MinAmount,
        MaxAmount,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    FROM dbo.Taxes
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_TaxMaster_ins
    @Id UNIQUEIDENTIFIER = NULL,
    @Name NVARCHAR(100),
    @Code NVARCHAR(20),
    @Country NVARCHAR(10) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Rate DECIMAL(18, 4),
    @RateType NVARCHAR(20),
    @IsActive BIT,
    @IsCompound BIT,
    @IsDefault BIT,
    @EffectiveFrom DATE,
    @EffectiveTo DATE = NULL,
    @ApplicableTo NVARCHAR(50) = NULL,
    @CustomerType NVARCHAR(50) = NULL,
    @Location NVARCHAR(100) = NULL,
    @MinAmount DECIMAL(18, 2) = NULL,
    @MaxAmount DECIMAL(18, 2) = NULL,
    @CreatedBy NVARCHAR(100),
    @CreatedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = '00000000-0000-0000-0000-000000000000'
        SET @Id = NEWID();

    INSERT INTO dbo.Taxes (
        Id, Name, Code, Country, Description, Rate, RateType,
        IsActive, IsCompound, IsDefault, EffectiveFrom, EffectiveTo,
        ApplicableTo, CustomerType, Location, MinAmount, MaxAmount,
        CreatedBy, CreatedDate
    )
    VALUES (
        @Id, @Name, @Code, @Country, @Description, @Rate, @RateType,
        @IsActive, @IsCompound, @IsDefault, @EffectiveFrom, @EffectiveTo,
        @ApplicableTo, @CustomerType, @Location, @MinAmount, @MaxAmount,
        @CreatedBy, @CreatedDate
    );

    SELECT @Id AS Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_TaxMaster_upd
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Code NVARCHAR(20),
    @Country NVARCHAR(10) = NULL,
    @Description NVARCHAR(500) = NULL,
    @Rate DECIMAL(18, 4),
    @RateType NVARCHAR(20),
    @IsActive BIT,
    @IsCompound BIT,
    @IsDefault BIT,
    @EffectiveFrom DATE,
    @EffectiveTo DATE = NULL,
    @ApplicableTo NVARCHAR(50) = NULL,
    @CustomerType NVARCHAR(50) = NULL,
    @Location NVARCHAR(100) = NULL,
    @MinAmount DECIMAL(18, 2) = NULL,
    @MaxAmount DECIMAL(18, 2) = NULL,
    @ModifiedBy NVARCHAR(100),
    @ModifiedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Taxes
    SET
        Name = @Name,
        Code = @Code,
        Country = @Country,
        Description = @Description,
        Rate = @Rate,
        RateType = @RateType,
        IsActive = @IsActive,
        IsCompound = @IsCompound,
        IsDefault = @IsDefault,
        EffectiveFrom = @EffectiveFrom,
        EffectiveTo = @EffectiveTo,
        ApplicableTo = @ApplicableTo,
        CustomerType = @CustomerType,
        Location = @Location,
        MinAmount = @MinAmount,
        MaxAmount = @MaxAmount,
        ModifiedBy = @ModifiedBy,
        ModifiedDate = @ModifiedDate
    WHERE Id = @Id;
END
GO
