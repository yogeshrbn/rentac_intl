-- Normalized damage component lines per GRN item (piece-wise components + cost).
-- FK to GRNItems: child rows are removed when the parent GRN line or GRN is cleared (CASCADE).

IF OBJECT_ID(N'dbo.GRNItemDamageComponents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GRNItemDamageComponents (
        GRNItemDamageComponentId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_GRNItemDamageComponents PRIMARY KEY,
        GRNItemId INT NOT NULL,
        ProductId INT NOT NULL,
        PieceNo INT NOT NULL,
        ComponentName NVARCHAR(200) NOT NULL,
        Cost DECIMAL(18, 2) NOT NULL
            CONSTRAINT DF_GRNItemDamageComponents_Cost DEFAULT (0),
        CONSTRAINT FK_GRNItemDamageComponents_GRNItems
            FOREIGN KEY (GRNItemId) REFERENCES dbo.GRNItems (GRNItemId) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX IX_GRNItemDamageComponents_GRNItemId
        ON dbo.GRNItemDamageComponents (GRNItemId);

    CREATE NONCLUSTERED INDEX IX_GRNItemDamageComponents_ProductId
        ON dbo.GRNItemDamageComponents (ProductId);
END
GO
