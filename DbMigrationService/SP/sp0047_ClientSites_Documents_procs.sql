-- Client site document file names (see table042_ClientSites_Documents.sql).
-- If p_ClientSites_sel / p_ClientSiteById_sel are maintained outside this repo, add these columns to their SELECT lists
-- so ConstructList maps them; otherwise the API merges rows via p_ClientSites_DocumentNames_sel.

CREATE OR ALTER PROCEDURE dbo.p_ClientSites_UpdateDocuments
    @LedgerSiteId INT,
    @Document1FileName NVARCHAR(260) = NULL,
    @Document2FileName NVARCHAR(260) = NULL,
    @Document3FileName NVARCHAR(260) = NULL,
    @Document4FileName NVARCHAR(260) = NULL,
    @Document5FileName NVARCHAR(260) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ClientSites
    SET
        Document1FileName = @Document1FileName,
        Document2FileName = @Document2FileName,
        Document3FileName = @Document3FileName,
        Document4FileName = @Document4FileName,
        Document5FileName = @Document5FileName
    WHERE LedgerSiteId = @LedgerSiteId;
END
GO

CREATE OR ALTER PROCEDURE dbo.p_ClientSites_DocumentNames_sel
    @LedgerId INT = NULL,
    @LedgerSiteId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @LedgerSiteId IS NOT NULL AND @LedgerSiteId > 0
    BEGIN
        SELECT
            LedgerSiteId,
            LedgerId,
            Document1FileName,
            Document2FileName,
            Document3FileName,
            Document4FileName,
            Document5FileName
        FROM dbo.ClientSites
        WHERE LedgerSiteId = @LedgerSiteId;
        RETURN;
    END

    IF @LedgerId IS NOT NULL AND @LedgerId > 0
    BEGIN
        SELECT
            LedgerSiteId,
            LedgerId,
            Document1FileName,
            Document2FileName,
            Document3FileName,
            Document4FileName,
            Document5FileName
        FROM dbo.ClientSites
        WHERE LedgerId = @LedgerId;
    END
END
GO
