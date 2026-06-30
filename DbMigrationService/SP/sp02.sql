 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_LastInvoice_sel'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_LastInvoice_sel
 END
 GO
 Create proc [dbo].[p_LastInvoice_sel]
(
@LedgerId INT
,@FinYearId INT
,@ledgerSiteId INT = NULL
)
AS
Select TOP 1 * from Invoice
Where [To] =
 (SELECT Max([To]) From Invoice Where LedgerId = @LedgerId
AND ( FinYearId = @FinYearId OR @FinYearId = 0) AND LedgerSiteId = ISNULL(@ledgerSiteId,LedgerSiteId)
)
 AND LedgerId = @LedgerId
AND ( FinYearId = @FinYearId OR @FinYearId = 0)  AND LedgerSiteId = ISNULL(@ledgerSiteId,LedgerSiteId)
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Site_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.[p_Site_ins]
 END
 GO
 CREATE PROC [dbo].[p_Site_ins] (
	@WorkOrderId INT
	,@JobNumber NVARCHAR(20) = NULL
	,@ChallanNumber NVARCHAR(20) = NULL
	,@Site NVARCHAR(100) = NULL
	,@ShaftSize NVARCHAR(50) = NULL
	,@ShaftHeight NVARCHAR(50) = NULL
	,@SiteEng NVARCHAR(50) = NULL
	,@StartDate NVARCHAR(20)
	,@Duration INT = 0
	,@Doc1 NVARCHAR(100) = NULL
	,@Doc2 NVARCHAR(100) = NULL
	,@Doc3 NVARCHAR(100) = NULL
	,@SubTotal NUMERIC(10, 3)
	,@TaxAmount NUMERIC(10, 3)
	,@Freight NUMERIC(10, 3)
	,@FreightTax NUMERIC(10, 2) = 0
	,@Total NUMERIC(10, 3)
	,@CreatedBy INT = NULL
	,@ParentSiteId INT = NULL
	,@Vehicle NVARCHAR(50) = NULL
	,@Driver NVARCHAR(100) = NULL
	,@State NVARCHAR(100) = NULL
	,@VehicleId INT = NULL
	,@DriverId INT = NULL
	,@LedgerSiteId INT = NULL
	)
AS

 
Select  @ChallanNumber  =  Number  from WorkOrder
Where WorkOrderId= @WorkOrderId

 

INSERT INTO SiteInfo (
	WorkOrderId
	,JobNumber
	,ChallanNumber
	,Site
	,ShaftSize
	,Shaftheight
	,SiteEng
	,Doc1
	,Doc2
	,Doc3
	,SubTotal
	,TaxAmount
	,Freight
	,Total
	,CreatedBy
	,StartDate
	,Duration
	,ParentSite
	,Driver
	,Vehicle
	,FreightTax
	,[State]
	,VehicleId,DriverId,LedgerSiteId
	)
VALUES (
	@WorkOrderId
	,@JobNumber
	,@ChallanNumber
	,@Site
	,@ShaftSize
	,@ShaftHeight
	,@SiteEng
	,@Doc1
	,@Doc2
	,@Doc3
	,@SubTotal
	,@TaxAmount
	,@Freight
	,@Total
	,@CreatedBy
	,@StartDate
	,@Duration
	,@ParentSiteId
	,@Driver
	,@Vehicle
	,@FreightTax
	,@State,@VehicleId,@DriverId,@LedgerSiteId
	)

SELECT SCOPE_IDENTITY() SITEID
GO