IF OBJECT_ID(N'dbo.WorkOrderTax', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[WorkOrderTax](
        [Id] [uniqueidentifier] NOT NULL,
        [WorkOrderItemId] [int] NOT NULL,
        [SiteId] [int] NOT NULL,
        [ProductId] [int] NOT NULL,
        [TaxCategoryId] [int] NOT NULL,
        [TaxId] [int] NOT NULL,
        [TaxName] [nvarchar](100) NOT NULL,
        [TaxCode] [nvarchar](20) NULL,
        [Rate] [decimal](18, 4) NOT NULL,
        [RateType] [nvarchar](20) NOT NULL,
        [Amount] [decimal](18, 2) NOT NULL,
        CONSTRAINT [PK_WorkOrderTax] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[WorkOrderTax] ADD CONSTRAINT [DF_WorkOrderTax_Id] DEFAULT (NEWID()) FOR [Id];
    ALTER TABLE [dbo].[WorkOrderTax] ADD CONSTRAINT [DF_WorkOrderTax_RateType] DEFAULT ('Percentage') FOR [RateType];

    CREATE NONCLUSTERED INDEX [IX_WorkOrderTax_SiteId]
        ON [dbo].[WorkOrderTax]([SiteId] ASC);

    CREATE NONCLUSTERED INDEX [IX_WorkOrderTax_WorkOrderItemId]
        ON [dbo].[WorkOrderTax]([WorkOrderItemId] ASC);
END
GO
