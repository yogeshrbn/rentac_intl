IF OBJECT_ID(N'dbo.TaxCategoryMappings', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TaxCategoryMappings](
        [Id] [uniqueidentifier] NOT NULL,
        [TaxCategoryId] [int] NOT NULL,
        [TaxId] [uniqueidentifier] NOT NULL,
        [IsDefault] [bit] NOT NULL,
        CONSTRAINT [PK_TaxCategoryMappings] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[TaxCategoryMappings] ADD CONSTRAINT [DF_TaxCategoryMappings_Id] DEFAULT (NEWID()) FOR [Id];
    ALTER TABLE [dbo].[TaxCategoryMappings] ADD CONSTRAINT [DF_TaxCategoryMappings_IsDefault] DEFAULT ((0)) FOR [IsDefault];

    CREATE UNIQUE NONCLUSTERED INDEX [UQ_TaxCategoryMappings_CategoryTax]
        ON [dbo].[TaxCategoryMappings]([TaxCategoryId] ASC, [TaxId] ASC);
END
GO
