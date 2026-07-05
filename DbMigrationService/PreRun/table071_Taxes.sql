IF OBJECT_ID(N'dbo.Taxes', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Taxes](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Code] [nvarchar](20) NOT NULL,
        [Country] [nvarchar](10) NULL,
        [Description] [nvarchar](500) NULL,
        [Rate] [decimal](18, 4) NOT NULL,
        [RateType] [nvarchar](20) NOT NULL,
        [IsActive] [bit] NOT NULL,
        [IsCompound] [bit] NOT NULL,
        [IsDefault] [bit] NOT NULL,
        [EffectiveFrom] [date] NOT NULL,
        [EffectiveTo] [date] NULL,
        [ApplicableTo] [nvarchar](50) NULL,
        [CustomerType] [nvarchar](50) NULL,
        [Location] [nvarchar](100) NULL,
        [MinAmount] [decimal](18, 2) NULL,
        [MaxAmount] [decimal](18, 2) NULL,
        [CreatedBy] [nvarchar](100) NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [ModifiedBy] [nvarchar](100) NULL,
        [ModifiedDate] [datetime] NULL,
        [RowVersion] [timestamp] NOT NULL,
        CONSTRAINT [PK_Taxes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Taxes_Code] UNIQUE NONCLUSTERED ([Code] ASC)
    );

    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_Id] DEFAULT (NEWID()) FOR [Id];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_Rate] DEFAULT ((0)) FOR [Rate];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_RateType] DEFAULT ('Percentage') FOR [RateType];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_IsActive] DEFAULT ((1)) FOR [IsActive];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_IsCompound] DEFAULT ((0)) FOR [IsCompound];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_IsDefault] DEFAULT ((0)) FOR [IsDefault];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_EffectiveFrom] DEFAULT (GETDATE()) FOR [EffectiveFrom];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_MinAmount] DEFAULT ((0)) FOR [MinAmount];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [DF_Taxes_CreatedDate] DEFAULT (GETDATE()) FOR [CreatedDate];
    ALTER TABLE [dbo].[Taxes] ADD CONSTRAINT [CK_Taxes_RateType] CHECK ([RateType] = 'Fixed' OR [RateType] = 'Percentage');
END
GO
