USE [$AppDb$]
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'p_GRN_LinkTransferWorkOrder' AND type = 'P')
    DROP PROCEDURE dbo.p_GRN_LinkTransferWorkOrder;
GO

CREATE PROCEDURE dbo.p_GRN_LinkTransferWorkOrder
    @GRNId INT,
    @WorkOrderId INT
AS
SET NOCOUNT ON;
UPDATE dbo.GRN
SET TransferWorkOrderId = @WorkOrderId
WHERE GRNId = @GRNId;
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'p_TransferGrn_lookupByWorkOrderId' AND type = 'P')
    DROP PROCEDURE dbo.p_TransferGrn_lookupByWorkOrderId;
GO

CREATE PROCEDURE dbo.p_TransferGrn_lookupByWorkOrderId
    @workOrderId INT
AS
SET NOCOUNT ON;
SELECT TOP 1
    X.GRNId,
    X.SourceLedgerId,
    X.SourceLedgerSiteId,
    X.Remarks,
    X.TransferDate
FROM (
    SELECT
        G.GRNId,
        G.LedgerId AS SourceLedgerId,
        G.LedgerSiteId AS SourceLedgerSiteId,
        G.Remarks,
        G.ReceivingDate AS TransferDate,
        0 AS SortKey
    FROM dbo.GRN G WITH (NOLOCK)
    WHERE G.TransferWorkOrderId = @workOrderId
    UNION ALL
    SELECT
        G.GRNId,
        G.LedgerId AS SourceLedgerId,
        G.LedgerSiteId AS SourceLedgerSiteId,
        G.Remarks,
        G.ReceivingDate AS TransferDate,
        1 AS SortKey
    FROM dbo.GRN G WITH (NOLOCK)
    INNER JOIN dbo.WorkOrder W WITH (NOLOCK) ON W.WorkOrderId = @workOrderId
    INNER JOIN dbo.SiteInfo SI WITH (NOLOCK) ON SI.WorkOrderId = W.WorkOrderId
    WHERE G.Typeid = 12
      AND G.CompanyId = W.CompanyId
      AND (G.TransferWorkOrderId IS NULL OR G.TransferWorkOrderId <> @workOrderId)
      AND CAST(G.ReceivingDate AS DATE) = CAST(W.WorkOrderDate AS DATE)
      AND G.LedgerSiteId <> SI.LedgerSiteId
) X
ORDER BY X.SortKey, X.GRNId DESC;
GO
