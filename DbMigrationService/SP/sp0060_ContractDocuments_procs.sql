CREATE OR ALTER PROCEDURE dbo.p_ContractDocument_ins
    @CompanyId INT,
    @ContractId INT,
    @JobCardId INT = NULL,
    @DocumentType NVARCHAR(50),
    @StoragePath NVARCHAR(500),
    @OriginalFileName NVARCHAR(255) = NULL,
    @ContentType NVARCHAR(100) = NULL,
    @CreatedBy INT,
    @CreatedOn DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'dbo.ContractDocuments', N'U') IS NULL
        RETURN;

    INSERT INTO dbo.ContractDocuments (
        CompanyId,
        ContractId,
        JobCardId,
        DocumentType,
        StoragePath,
        OriginalFileName,
        ContentType,
        CreatedBy,
        CreatedOn
    )
    VALUES (
        @CompanyId,
        @ContractId,
        @JobCardId,
        @DocumentType,
        @StoragePath,
        @OriginalFileName,
        @ContentType,
        @CreatedBy,
        @CreatedOn
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS ContractDocumentId;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_ContractDocuments_selByContract
    @CompanyId INT,
    @ContractId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ContractDocumentId,
        CompanyId,
        ContractId,
        JobCardId,
        DocumentType,
        StoragePath,
        OriginalFileName,
        ContentType,
        CreatedBy,
        CreatedOn
    FROM dbo.ContractDocuments
    WHERE CompanyId = @CompanyId AND ContractId = @ContractId
    ORDER BY CreatedOn DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_ContractDocuments_selByJobCards
    @CompanyId INT,
    @JobCardIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    IF @JobCardIds IS NULL OR LEN(LTRIM(RTRIM(@JobCardIds))) = 0
    BEGIN
        SELECT
            ContractDocumentId,
            CompanyId,
            ContractId,
            JobCardId,
            DocumentType,
            StoragePath,
            OriginalFileName,
            ContentType,
            CreatedBy,
            CreatedOn
        FROM dbo.ContractDocuments
        WHERE 1 = 0;
        RETURN;
    END

    SELECT
        cd.ContractDocumentId,
        cd.CompanyId,
        cd.ContractId,
        cd.JobCardId,
        cd.DocumentType,
        cd.StoragePath,
        cd.OriginalFileName,
        cd.ContentType,
        cd.CreatedBy,
        cd.CreatedOn
    FROM dbo.ContractDocuments cd
    WHERE cd.CompanyId = @CompanyId
      AND cd.JobCardId IN (
          SELECT TRY_CAST(LTRIM(RTRIM(value)) AS INT)
          FROM STRING_SPLIT(@JobCardIds, ',')
          WHERE TRY_CAST(LTRIM(RTRIM(value)) AS INT) IS NOT NULL
      )
    ORDER BY cd.CreatedOn DESC;
END
GO
