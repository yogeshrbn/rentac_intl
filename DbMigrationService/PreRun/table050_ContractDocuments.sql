-- Contract-scoped documents (e.g. install activity photos from ops dashboard). Binary files live in blob storage; this table stores paths + metadata.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ContractDocuments' AND schema_id = SCHEMA_ID(N'dbo'))
BEGIN
    CREATE TABLE dbo.ContractDocuments (
        ContractDocumentId INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ContractDocuments PRIMARY KEY,
        CompanyId INT NOT NULL,
        ContractId INT NOT NULL,
        JobCardId INT NULL,
        DocumentType NVARCHAR(50) NOT NULL,
        StoragePath NVARCHAR(500) NOT NULL,
        OriginalFileName NVARCHAR(255) NULL,
        ContentType NVARCHAR(100) NULL,
        CreatedBy INT NOT NULL,
        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_ContractDocuments_CreatedOn DEFAULT (SYSUTCDATETIME())
    );

    CREATE NONCLUSTERED INDEX IX_ContractDocuments_ContractId
        ON dbo.ContractDocuments (CompanyId, ContractId);

    CREATE NONCLUSTERED INDEX IX_ContractDocuments_JobCardId
        ON dbo.ContractDocuments (CompanyId, JobCardId)
        WHERE JobCardId IS NOT NULL;

    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'Contract' AND schema_id = SCHEMA_ID(N'dbo'))
       AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ContractDocuments_Contract')
    BEGIN
        ALTER TABLE dbo.ContractDocuments
            ADD CONSTRAINT FK_ContractDocuments_Contract
            FOREIGN KEY (ContractId) REFERENCES dbo.Contract (ContractId);
    END
END
GO
