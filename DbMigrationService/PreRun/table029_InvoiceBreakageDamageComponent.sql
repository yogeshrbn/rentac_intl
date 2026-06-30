-- Snapshot of GRN damage component lines at rent bill save (gen bill), keyed by invoice.

IF OBJECT_ID(N'dbo.InvoiceBreakageDamageComponent', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InvoiceBreakageDamageComponent (
        InvoiceBreakageDamageComponentId INT IDENTITY(1, 1) NOT NULL
            CONSTRAINT PK_InvoiceBreakageDamageComponent PRIMARY KEY,
        InvoiceId INT NOT NULL,
        CompanyId INT NOT NULL,
        GRNItemId INT NULL,
        ProductId INT NOT NULL,
        ParentItem NVARCHAR(300) NULL,
        GRN NVARCHAR(100) NULL,
        ComponentName NVARCHAR(200) NULL,
        Quantity DECIMAL(18, 4) NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_Qty DEFAULT (0),
        Rate DECIMAL(18, 4) NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_Rate DEFAULT (0),
        Cost DECIMAL(18, 2) NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_Cost DEFAULT (0),
        ReceivingDate DATE NULL,
        IGST FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_IGST DEFAULT (0),
        CGST FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_CGST DEFAULT (0),
        SGST FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_SGST DEFAULT (0),
        IGSTRate FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_IGSTR DEFAULT (0),
        CGSTRate FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_CGSTR DEFAULT (0),
        SGSTRate FLOAT NOT NULL
            CONSTRAINT DF_InvoiceBreakageDamageComponent_SGSTR DEFAULT (0)
    );

    CREATE NONCLUSTERED INDEX IX_InvoiceBreakageDamageComponent_Invoice
        ON dbo.InvoiceBreakageDamageComponent (InvoiceId, CompanyId);
END
GO
