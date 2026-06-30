USE [$AppDb$]
GO

IF COL_LENGTH('dbo.Quotation', 'PartyType') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD PartyType TINYINT NOT NULL CONSTRAINT DF_Quotation_PartyType DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Quotation', 'UnregisteredPartyName') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD UnregisteredPartyName NVARCHAR(500) NULL;
END
GO

IF COL_LENGTH('dbo.Quotation', 'UnregisteredPartyAddress') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD UnregisteredPartyAddress NVARCHAR(MAX) NULL;
END
GO

IF COL_LENGTH('dbo.Quotation', 'GstRate') IS NULL
BEGIN
    
        ALTER TABLE dbo.Quotation ADD GstRate FLOAT NOT NULL CONSTRAINT DF_Quotation_GstRate DEFAULT (0);
END
GO

-- Header BIT flags: IGST, CGST, SGST (rename from GstIgst / legacy ManualGst* / GstCgst / GstSgst)
IF COL_LENGTH('dbo.Quotation', 'IGST') IS NULL
BEGIN
    
        ALTER TABLE dbo.Quotation ADD [IGST] BIT NOT NULL CONSTRAINT DF_Quotation_IGST DEFAULT (0);
END
GO

IF COL_LENGTH('dbo.Quotation', 'CGST') IS NULL
BEGIN
   
        ALTER TABLE dbo.Quotation ADD [CGST] BIT NOT NULL CONSTRAINT DF_Quotation_CGST DEFAULT (0);
END
GO

IF COL_LENGTH('dbo.Quotation', 'SGST') IS NULL
BEGIN
   
        ALTER TABLE dbo.Quotation ADD [SGST] BIT NOT NULL CONSTRAINT DF_Quotation_SGST DEFAULT (0);
END
GO
