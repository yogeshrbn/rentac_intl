IF OBJECT_ID(N'dbo.QuotationManualTax', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuotationManualTax](
        [Id] [uniqueidentifier] NOT NULL,
        [QuotationId] [int] NOT NULL,
        [TaxId] [int] NOT NULL,
        [TaxName] [nvarchar](100) NOT NULL,
        [TaxCode] [nvarchar](20) NULL,
        [Rate] [decimal](18, 4) NOT NULL,
        [RateType] [nvarchar](20) NOT NULL,
        CONSTRAINT [PK_QuotationManualTax] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[QuotationManualTax] ADD CONSTRAINT [DF_QuotationManualTax_Id] DEFAULT (NEWID()) FOR [Id];
    ALTER TABLE [dbo].[QuotationManualTax] ADD CONSTRAINT [DF_QuotationManualTax_RateType] DEFAULT ('Percentage') FOR [RateType];

    CREATE NONCLUSTERED INDEX [IX_QuotationManualTax_QuotationId]
        ON [dbo].[QuotationManualTax]([QuotationId] ASC);
END
GO
