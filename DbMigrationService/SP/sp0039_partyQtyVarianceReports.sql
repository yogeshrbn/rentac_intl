/*
  Party Reports — variance from vw_GRN (aggregated per GRN header).

  Requires vw_GRN with at least: GRNId, GRN, Client, SiteName, ReceivingDate, LedgerId,
  LedgerSiteId, CompanyId, Breakage, ShortQty, ExcessQty; ChallanType should exist on the view
  (as used elsewhere for receiving filters). If ChallanType is not on vw_GRN, join WorkOrder
  and replace the @challanType predicate accordingly.

  Filters mirror p_partyReturnChallans:
    @From, @To, @ledgerId (0 = all), @CompanyId, @ledgerSiteId (0 = all), @challanType (0 = all).
*/

IF OBJECT_ID(N'dbo.p_partyBreakageReport', N'P') IS NOT NULL
    DROP PROCEDURE dbo.p_partyBreakageReport;
GO

CREATE PROCEDURE dbo.p_partyBreakageReport
    @From DATE,
    @To DATE,
    @ledgerId INT = 0,
    @CompanyId INT,
    @ledgerSiteId INT = 0,
    @challanType TINYINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.GRNId,
        MAX(v.GRN) AS GRN,
        MAX(v.Client) AS Client,
        MAX(v.Site) AS SiteName,
        CAST(MAX(v.ReceivingDate) AS SMALLDATETIME) AS ReceivingDate,
        MAX(v.LedgerId) AS LedgerId,
        MAX(v.LedgerSiteId) AS LedgerSiteId,
        CAST(SUM(ISNULL(v.Breakage, 0)) AS FLOAT) AS Breakage,
        CAST(0 AS FLOAT) AS ShortQty,
        CAST(0 AS FLOAT) AS ExcessQty
    FROM dbo.vw_GRN AS v
    WHERE v.CompanyId = @CompanyId
      AND CAST(v.ReceivingDate AS DATE) BETWEEN @From AND @To
      AND (@ledgerId = 0 OR v.LedgerId = @ledgerId)
      AND (@ledgerSiteId = 0 OR v.LedgerSiteId = @ledgerSiteId)
      AND (@challanType = 0 OR v.TypeId = @challanType)
    GROUP BY v.GRNId
    HAVING SUM(ISNULL(v.Breakage, 0)) > 0
    ORDER BY MAX(v.Client), MAX(v.Site), MAX(v.ReceivingDate);
END
GO

IF OBJECT_ID(N'dbo.p_partyLostReport', N'P') IS NOT NULL
    DROP PROCEDURE dbo.p_partyLostReport;
GO

CREATE PROCEDURE dbo.p_partyLostReport
    @From DATE,
    @To DATE,
    @ledgerId INT = 0,
    @CompanyId INT,
    @ledgerSiteId INT = 0,
    @challanType TINYINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.GRNId,
        MAX(v.GRN) AS GRN,
        MAX(v.Client) AS Client,
        MAX(v.Site) AS SiteName,
        CAST(MAX(v.ReceivingDate) AS SMALLDATETIME) AS ReceivingDate,
        MAX(v.LedgerId) AS LedgerId,
        MAX(v.LedgerSiteId) AS LedgerSiteId,
        CAST(0 AS FLOAT) AS Breakage,
        CAST(SUM(ISNULL(v.ShortQty, 0)) AS FLOAT) AS ShortQty,
        CAST(0 AS FLOAT) AS ExcessQty
    FROM dbo.vw_GRN AS v
    WHERE v.CompanyId = @CompanyId
      AND CAST(v.ReceivingDate AS DATE) BETWEEN @From AND @To
      AND (@ledgerId = 0 OR v.LedgerId = @ledgerId)
      AND (@ledgerSiteId = 0 OR v.LedgerSiteId = @ledgerSiteId)
      AND (@challanType = 0 OR v.TypeId = @challanType)
    GROUP BY v.GRNId
    HAVING SUM(ISNULL(v.ShortQty, 0)) > 0
    ORDER BY MAX(v.Client), MAX(v.Site), MAX(v.ReceivingDate);
END
GO

IF OBJECT_ID(N'dbo.p_partyExcessReport', N'P') IS NOT NULL
    DROP PROCEDURE dbo.p_partyExcessReport;
GO

CREATE PROCEDURE dbo.p_partyExcessReport
    @From DATE,
    @To DATE,
    @ledgerId INT = 0,
    @CompanyId INT,
    @ledgerSiteId INT = 0,
    @challanType TINYINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.GRNId,
        MAX(v.GRN) AS GRN,
        MAX(v.Client) AS Client,
        MAX(v.Site) AS SiteName,
        CAST(MAX(v.ReceivingDate) AS SMALLDATETIME) AS ReceivingDate,
        MAX(v.LedgerId) AS LedgerId,
        MAX(v.LedgerSiteId) AS LedgerSiteId,
        CAST(0 AS FLOAT) AS Breakage,
        CAST(0 AS FLOAT) AS ShortQty,
        CAST(SUM(ISNULL(v.ExcessQty, 0)) AS FLOAT) AS ExcessQty
    FROM dbo.vw_GRN AS v
    WHERE v.CompanyId = @CompanyId
      AND CAST(v.ReceivingDate AS DATE) BETWEEN @From AND @To
      AND (@ledgerId = 0 OR v.LedgerId = @ledgerId)
      AND (@ledgerSiteId = 0 OR v.LedgerSiteId = @ledgerSiteId)
      AND (@challanType = 0 OR v.TypeId = @challanType)
    GROUP BY v.GRNId
    HAVING SUM(ISNULL(v.ExcessQty, 0)) > 0
    ORDER BY MAX(v.Client), MAX(v.Site), MAX(v.ReceivingDate);
END
GO
