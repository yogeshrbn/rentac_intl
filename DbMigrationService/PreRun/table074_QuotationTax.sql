IF OBJECT_ID(N'dbo.QuotationItems', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1 FROM sys.columns
       WHERE object_id = OBJECT_ID(N'dbo.QuotationItems') AND name = N'QuotationItemId'
   )
   AND NOT EXISTS (
       SELECT 1 FROM sys.columns
       WHERE object_id = OBJECT_ID(N'dbo.QuotationItems') AND name = N'Id'
   )
BEGIN
    ALTER TABLE dbo.QuotationItems ADD QuotationItemId INT IDENTITY(1,1) NOT NULL;
END
GO

IF OBJECT_ID(N'dbo.QuotationTax', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuotationTax](
        [Id] [uniqueidentifier] NOT NULL,
        [QuotationItemId] [int] NOT NULL,
        [QuotationId] [int] NOT NULL,
        [ProductId] [int] NOT NULL,
        [TaxCategoryId] [int] NOT NULL,
        [TaxId] [int] NOT NULL,
        [TaxName] [nvarchar](100) NOT NULL,
        [TaxCode] [nvarchar](20) NULL,
        [Rate] [decimal](18, 4) NOT NULL,
        [RateType] [nvarchar](20) NOT NULL,
        [Amount] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_QuotationTax] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[QuotationTax] ADD CONSTRAINT [DF_QuotationTax_Id] DEFAULT (NEWID()) FOR [Id];
    ALTER TABLE [dbo].[QuotationTax] ADD CONSTRAINT [DF_QuotationTax_RateType] DEFAULT ('Percentage') FOR [RateType];

    CREATE NONCLUSTERED INDEX [IX_QuotationTax_QuotationId]
        ON [dbo].[QuotationTax]([QuotationId] ASC);

    CREATE NONCLUSTERED INDEX [IX_QuotationTax_QuotationItemId]
        ON [dbo].[QuotationTax]([QuotationItemId] ASC);
END
GO
