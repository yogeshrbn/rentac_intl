-- Optional site documents (up to 5): stored file names only; binary files live under Api/docs/sites/{LedgerId}/{LedgerSiteId}/.
IF COL_LENGTH('dbo.ClientSites', 'Document1FileName') IS NULL
BEGIN
    ALTER TABLE dbo.ClientSites ADD
        Document1FileName NVARCHAR(260) NULL,
        Document2FileName NVARCHAR(260) NULL,
        Document3FileName NVARCHAR(260) NULL,
        Document4FileName NVARCHAR(260) NULL,
        Document5FileName NVARCHAR(260) NULL;
END
GO
