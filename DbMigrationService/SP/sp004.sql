 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_WorkOrder_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_WorkOrder_ins
 END
 GO
CREATE PROC [dbo].[p_WorkOrder_ins] (
	@Number NVARCHAR(20) = NULL
	,@LedgerId INT
	,@WorkOrderDate NVARCHAR(20) = NULL
	,@SubTotal NUMERIC(15, 3) = 0
	,@TotalTax NUMERIC(10, 3) = 0
	,@Total NUMERIC(15, 3) = 0
	,@CompanyId INT
	,@ClientAmount NUMERIC(15, 3) = 0
	,@CreatedBy INT
	,@RbnClientId INT = NULL
	,@FinYearId INT = NULL
	,@Type TINYINT = 2 -- rent delivery challan
	,@ParentWorkOrderId INT = NULL
	,@TransactionId INT = 0
	)
AS
DECLARE @lastNumber int
Select  @lastNumber  = MAX(SUBSTRING(Number, CHARINDEX('-',Number)+1,LEN(number)-Charindex('-',Number))) from WorkOrder
Where CompanyId = @CompanyId AND FinYearId = @FinYearId

SET @Number = (CASE WHEN @lastNumber IS NULL THEN '1' ELSE @lastNumber + 1 END)

INSERT INTO WorkOrder (
	Number
	,LedgerId
	,WorkOrderDate
	,SubTotal
	,TotalTax
	,Total
	,CompanyId
	,ClientAmount
	,CreatedBy
	,RbnClientId
	,FinYearId
	,[Type]
	,ParentWorkOrderId
	,TransactionId
	)
VALUES (
	@Number
	,@LedgerId
	,@WorkOrderDate
	,@SubTotal
	,@TotalTax
	,@Total
	,@CompanyId
	,@ClientAmount
	,@CreatedBy
	,@RbnClientId
	,@FinYearId
	,@Type
	,@ParentWorkOrderId
	,@TransactionId
	)

SELECT SCOPE_IDENTITY() AS WorkOrderId
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Site_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Site_ins
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
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_GetAllowedRoutes'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_GetAllowedRoutes
 END
 GO
 

CREATE proc [dbo].[p_GetAllowedRoutes](
@UserId INT
 
)
AS
/*
p_GetRouteAccess 1 , 'genBills'
*/
Select T1.RoleId,T1.[Add],T1.[Edit],T1.[Delete],T1.[View],T2.[route] from RoleFunction T1
INNER JOIN [Function] T2 ON T1.FunctionId = T2.FunctionId
INNER JOIN Users T3 ON T1.RoleId = T3.RoleId
WHERE T3.UserId = @UserId  
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_userById'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_userById
 END
 GO
 

 CREATE PROC [dbo].[p_userById] (@userId int,
@finYear int
)
AS
-- p_userById 1,1003
 DECLARE @minDate Date, @maxDate Date
 DECLARE @currentFinYearId int
 Select @minDate=MinDate,@maxDate = MaxDate, @currentFinYearId = FinYearId From FinYear where FinYearId= @finYear OR (
 @finYear = 0 AND
 GETDATE() BETWEEN MinDate
		AND MaxDate) 
 
  

SELECT  
	T2.RbnClientId
	,T2.DefaultCompanyId
	,T2.UserName
	,T2.FullName
	,T2.Email
	,T2.AllowSwitchCompany
	,T2.RoleId
	,T2.Phone
	,T2.ProfilePic
	,@currentFinYearId as FinYearId
	, @minDate As FinYearStart
	,@maxDate As FinYearEnd
FROM  Users T2 

WHERE T2.UserId=@userId
 
 
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_updateProfilePic'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_updateProfilePic
 END
 GO
 create proc [dbo].[p_updateProfilePic]
(
@userId int,
@profilePicPath nvarchar(50)
)
AS
Update Users
SET profilePic = @profilePicPath
Where UserId = @userId
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_ClientWiseItems'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_ClientWiseItems
 END
 GO

 
CREATE proc [dbo].[p_ClientWiseItems]
(
@ledgerId INT  = 0,
@companyId int,
@fromDate smallDateTime,
@endDate smallDateTime 
)
AS
 --Select PR.LedgerId,L.Name As ClientName, P.Name As Product, Isnull(L.Address1,'') + ' ' + isnull(L.Address2,'') Address,L.Email,L.Phone1 Phone,
 --PR.OpeningBalance,SUM(Issue.SentQty) As IssuedQty
 --,Sum(Rec.Quantity) As ReceivedQty,(PR.OpeningBalance +SUM(Issue.SentQty)- Sum(Rec.Quantity)) As ClosingBalance
 --from ProductRates PR
 --INNER JOIN Product P ON Pr.ProductId = P.ProductId
 --INNER JOIN Ledger L ON PR.LedgerId = L.LedgerId
 --LEFT OUTER JOIN vw_ItemsIssued Issue ON PR.LedgerId = Issue.LedgerId  AND PR.ProductId = Issue.SentProductId
 --LEFT OUTER JOIN vw_GRN Rec ON Rec.LedgerId = PR.LedgerId And Rec.ProductId = Pr.ProductId
 --where PR.LedgerId = case when @clientId > 0 then @clientId else PR.LedgerId end
 --GROUP BY PR.LedgerId,L.Name,P.Name , PR.OpeningBalance , Isnull(L.Address1,'') + ' ' + isnull(L.Address2,''),L.Email,L.Phone1  

  
  ;
 with cte
	 as (
	 select LedgerId,SentProductId,CompanyId,'Sent' as SentReceive, SUM(sentqty) as Qty from vw_ItemsIssued
	 Group by LedgerId,SentProductId,CompanyId
	 UNION ALL
	  select LedgerId,ProductId,CompanyId, 'Received' as SentReceive, SUM(Quantity) as Qty from vw_GRN
	 Group by LedgerId,ProductId,CompanyId
	 )
	 SELECT L.LedgerId,L.Name As ClientName, P.Name As Product,
	 Isnull(L.Address1,'') + ' ' + isnull(L.Address2,'') Address,L.Email,L.Phone1 Phone,
	 Sent as IssuedQty, Received As ReceivedQty, (Sent-Received) as ClosingBalance FROM (
	 select LedgerId,SentProductId, CompanyId,  Sum( CASE When SentReceive = 'Sent' THEN Qty END ) as Sent,
	 Sum( CASE When SentReceive = 'Received' THEN Qty END ) as Received
	 from cte
	 Group by LedgerId,SentProductId, CompanyId
	 ) T
	INNER JOIN Ledger l ON T.LedgerId = L.LedgerId
	INNER JOIN Product P ON T.SentProductId = P.ProductId
	 where l.LedgerId = case when @ledgerId >0 then @ledgerId else l.LedgerId end
	 And T.CompanyId = @companyId
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_ItemWiseClients'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_ItemWiseClients
 END
 GO
  
CREATE proc [dbo].[p_ItemWiseClients]
(
@ProductId INT  = 0,
@fromDate smallDateTime,
@endDate smallDateTime 
)
AS

	 
 with cte
	 as (
	 select LedgerId,SentProductId,CompanyId,'Sent' as SentReceive, SUM(sentqty) as Qty from vw_ItemsIssued
	 Group by LedgerId,SentProductId,CompanyId
	 UNION ALL
	  select LedgerId,ProductId,CompanyId, 'Received' as SentReceive, SUM(Quantity) as Qty from vw_GRN
	 Group by LedgerId,ProductId,CompanyId
	 )
	 SELECT L.LedgerId,L.Name As ClientName, P.Name As Product,Sent as IssuedQty, Received As ReceivedQty, (Sent-Received) as ClosingBalance FROM (
	 select LedgerId,SentProductId, CompanyId,  Sum( CASE When SentReceive = 'Sent' THEN Qty END ) as Sent,
	 Sum( CASE When SentReceive = 'Received' THEN Qty END ) as Received
	 from cte
	 Group by LedgerId,SentProductId, CompanyId
	 ) T
	INNER JOIN Ledger l ON T.LedgerId = L.LedgerId
	INNER JOIN Product P ON T.SentProductId = P.ProductId
	 where P.ProductId = case when @ProductId >0 then @ProductId else P.ProductId end
	  
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_TransferChallan_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_TransferChallan_ins
 END
 GO
 
CREATE Proc [dbo].[p_TransferChallan_ins]
(
@ChallanNumber nvarchar(30),
@CreatedBy INT,
@VehicleId INT = NULL
,@DriverId INT = NULL
 
,@LedgerSiteId INT
,@GrnID INT
,@CompanyId INT
)
AS

DECLARE @rbnClientId int,@ledgerId int

Select @rbnClientId = rbnClientId from Company where CompanyId = @CompanyId;
DECLARE @TransferDate smallDateTime

DECLARE @WorkORderId AS INT,@SourceSiteId INT 
SELECT @TransferDate = ReceivingDate,@SourceSiteId  = LedgerSiteId FROM GRN WHERe GRNId = @GrnID

 select @ledgerId = LedgerId from ClientSites where LedgerSiteId = @LedgerSiteId

INSERT INTO WorkOrder(Number,[Type],LedgerId,WorkOrderDate,CompanyId,CreatedBy,RbnClientId,FinYearId)
SELECT @ChallanNumber As Number, 2 as [Type],@ledgerId,ReceivingDate, @CompanyId, @CreatedBy As CreatedBy,@rbnClientId,FinYearId FROM GRN
WHERE GRNId = @GrnID

SET @WorkORderId = SCOPE_IDENTITY();


INSERT INTO SiteInfo(WorkOrderId,JobNumber,ChallanNumber,SubTotal,Freight,StartDate,VehicleId,DriverId,LedgerSiteId)
SELECT @WorkORderId,@ChallanNumber As JobNUmber,@ChallanNumber as ChallanNumber,0 As SubTotal, 0 As Freight,@TransferDate, @VehicleId As VehicleId,@DriverId As DriverId,
@LedgerSiteId As LedgerSiteId

DECLARE @SiteId AS INT
SET @SiteId = SCOPE_IDENTITY();
 
INSERT INTO WorkOrderItems(ProductId,Rate,PurchaseQty,SubTotal,SiteId,ProductSizeId)
SELECT T1.ProductId,T2.Rate,Quantity, (T2.Rate * Quantity) ,@SiteId, T1.ProductSizeId FROM GRNItems T1
OUTER APPLY(
 SELECT TOP 1 * FROM vw_BillViewSentItems   WHERE LedgerSiteId = @SourceSiteId
 AND Productid = T1.ProductId -- AND  ProductSizeId =  T1.ProductSizeId
 ORDER BY StartDate DESC
) T2
WHERE GRNId = @GrnID
 
   
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_PartyStock_balance'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.[p_PartyStock_balance]
 END
 GO
 
CREATE PROC [dbo].[p_PartyStock_balance] (
	@LedgerId INT = NULL
	,@FromDate SMALLDATETIME
	,@EndDate SMALLDATETIME
	,@CompanyId INT
	,@LedgerSiteId INT = NULL
	)
	-- p_PartyStock_balance 2048 ,'01-01-2018','09/17/2024',1030,60255

	 
AS
SELECT Item
	,Client
	,Pvt.LedgerId
	,T4.ContactPersonMobile,T4.ContactPersonName
	,PVt.ProductId
	,T2.Size -- AS [Weight]
	,ISNULL(T3.OpeningBalance, 0) AS OPB
	,Client
	,[1] AS SentQty
	,[2] AS RecQty
	,((ISNULL(T3.OpeningBalance, 0) + ISNULL([1], 0)) - ISNULL([2], 0)) AS ClosingBalance
	--,(((ISNULL(T3.OpeningBalance, 0) + ISNULL([1], 0)) - ISNULL([2], 0)) * Isnull(T2.Size, 0)) AS SizeBalance
	,(((ISNULL(T3.OpeningBalance, 0) + ISNULL([1], 0)) - ISNULL([2], 0))) AS SizeBalance
FROM (
	SELECT Product AS Item
		,Client
		,LedgerId
		,ProductId
		,TranType
		,Sum(Quantity) AS Quantity
	FROM (
		SELECT JobNumber
			,Item AS Product
			,ProductId
			,Client
			,LedgerId
			,StartDate AS TransDate
			,PurchaseQty Quantity
			,1 AS TranType
		FROM vw_BillViewSentItems
		WHERE LedgerId = ISNULL(@LedgerId, LedgerId)
			AND CompanyId = @CompanyId
			--	AND StartDate <= @Date
			AND StartDate 
				<= @EndDate
			AND ProductCategory <> 1013
			AND ChallanType IN (
				1
				,2
				) -- Rent Only and delivery challan
			--And WorkOrderiD IN(60255,60256)
			-- workOrderid will be of all challans workorder id whose parent workorder is the passed
			--work order. it may be the same workorder in case of rent. but for fixed sites (type=7) it will be the 
			--challan's work orderid which is all children workorders of type 1 of the passed workorder.
			AND (
			/*	WorkOrderID IN (
					SELECT ISNULL(T3.WorkOrderId, @WorkORderID)
					FROM vw_SiteInfo T3
					WHERE (
							T3.ParentWorkOrderId = @WorkORderID
							OR T3.WorkORderId = @WorkORderID
							)
					)
				OR WorkOrderId = ISNULL(@WorkOrderId, WorkOrderId) */
				(LedgerSiteid = @LedgerSiteId OR @LedgerSiteId IS NULL OR @LedgerSiteId = 0)
				)
		
		UNION ALL
		
		SELECT GRN AS JobNumber
			,Item
			,ProductId
			,Client
			,LedgerId
			,ReceivingDate AS TransDate
			,(Quantity + Breakage) AS Quantity
			,2 AS TranType
		FROM vw_GRN
		WHERE LedgerId = ISNULL(@LedgerId, LedgerId)
			AND CompanyId = @CompanyId --	AND ReceivingDate BETWEEN @FromDate AND @ToDate
			--AND ReceivingDate <= @Date
			AND ReceivingDate  
				 <= @EndDate
			AND ProductCategory <> 1013
			--select received items from workorder. If items never received for the given workorder and given workorder is null then
			--show total received from the client till date else show for the workorder passed as paramter
			--AND ISNULL(WorkORderId, 0) = (
			--	CASE 
			--		WHEN @WorkOrderId IS NULL
			--			AND WorkOrderId IS NULL
			--			THEN 0
			--		ELSE @WorkOrderId
			--		END
			--	)
		AND	(LedgerSiteid = @LedgerSiteId OR @LedgerSiteId IS NULL OR @LedgerSiteId = 0)
		) T1
	GROUP BY Product
		,ProductId
		,TranType
		,T1.Client
		,T1.LedgerId
	) AS T1
PIVOT(SUM(Quantity) FOR TranType IN (
			[1]
			,[2]
			)) AS PVT
INNER JOIN Product T2 ON Pvt.ProductId = T2.ProductId
INNER JOIN Ledger T4 ON PVT.LedgerId = T4.LedgerId
LEFT OUTER JOIN dbo.fn_party_OpeningBalance(@LedgerId, @FromDate) T3 ON T2.NAME = T3.Product
AND T3.LedgerSiteId = @LedgerSiteId
ORDER BY Pvt.Client
 
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_GeneratBillV4'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_GeneratBillV4
 END
 GO
CREATE PROC [dbo].[p_GeneratBillV4] (
	@From SMALLDATETIME
	,@To SMALLDATETIME
	-- ,@WorkOrderNumber NVARCHAR(100)
	,@LedgerId INT
	,@FinYearId INT = NULL
	,@LedgerSiteId INT
	)
AS
/*
[p_GeneratBillV4] '04/01/2024','08/15/2024',2047,1005,3008
-- Select *from ledger where CompanyId = 1007
[p_GeneratBill_TEST] '2018-04-01','2018-12-31',2073,1
SELECT * FROM Invoice Where LedgerId = 1047

*/

 

--	DECLARE @BillDate SmallDateTime = '01/30/2018'
DECLARE @lastBillDate SMALLDATETIME

SELECT @lastBillDate = Max([To])
FROM Invoice
WHERE LedgerId = @LedgerId
	AND StatusId <> 2
 --	select *from invoice where ledgerid=2047
SET @lastBillDate = (
		CASE 
			WHEN @LastBilLDate IS NOT NULL
				THEN DATEADD(Day, 1, @LastBillDate)
			ELSE @LastBillDate
			END
		)
SET @From = (CASE WHEN @lastBillDate IS NOT NULL THEN @lastBillDate ELSE @From END)
 

DECLARE @tblOutPut TABLE (
	id INT IDENTITY
	,Item NVARCHAR(50)
	,ProductId INT
	,[From] SMALLDATETIME
	,[To] SMALLDATETIME
	,Days INT
	,Rate NUMERIC(10, 2)
	,OPB NUMERIC(10, 2)
	,Quantity NUMERIC(10, 2)
	,ClosingBalanceance NUMERIC(10, 2)
	,ItemCategory INT
	,Amount NUMERIC(10, 2)
	,ChargeReturnedDate BIT
	,Mode INT
	,ProductSizeId INT
	)
DECLARE @tblData TABLE (
	Id INT
	,RowNum INT
	,JobNumber NVARCHAR(50)
	,Item NVARCHAR(100)
	,ProductId INT
	,[From] SMALLDATETIME
	,[To] SMALLDATETIME
	,[Days] INT
	,Rate NUMERIC(10, 3)
	,OPB NUMERIC(10, 3)
	,Quantity NUMERIC(10, 3)
	,Mode TINYINT
	,ClosingBalance NUMERIC(10, 3)
	,ChargeReturnedDate BIT
	,ProductSizeId INT
	)

INSERT INTO @tblData (
	id
	,RowNum
	,JobNumber
	,Item
	,[From]
	,Rate
	,OPB
	,Quantity
	,ProductId
	,Mode
	,ClosingBalance
	,ChargeReturnedDate
	,ProductSizeId
	)
SELECT ROW_NUMBER() OVER (
		ORDER BY ProductId
			-- ,ProductSizeId
			,StartDate
			,Mode
		) Id
	,ROW_NUMBER() OVER (
		PARTITION BY ProductId
		--,ProductSizeId
		ORDER BY ProductId
			,StartDate
			,Mode
		) RowNum
	,*
FROM (
	SELECT T1.JobNumber
		,T1.Item
		,T1.StartDate
		,T1.Rate
		,SUM(T1.OpeningBalance) AS OpeningBalance
		,SUM(T1.PurchaseQty) AS Quantity
		,T1.ProductId
		,1 Mode
		--, T1.OpeningBalance + T1.PurchaseQty ClosingBalance
		,SUM(T1.OpeningBalance) + SUM(T1.PurchaseQty) AS ClosingBalance
		,0 AS ChargeReturnedDate
		,T1.ProductSizeId
	FROM vw_BillViewSentItems T1
	WHERE -- T1.WorkOrderNumber = @WorkOrderNumber AND
		T1.ChallanType = 2 -- for rent only
		--AND T1.FinYearId = @FinYearId
		AND T1.StartDate BETWEEN @From
			AND @To
		AND T1.LedgerId = @LedgerId
		AND T1.LedgerSiteId = @LedgerSiteId
		AND (T1.StartDate >= @lastBillDate OR @lastBillDate IS NULL)
	GROUP BY T1.JobNumber
		,T1.Item
		,T1.StartDate
		,T1.Rate
		,T1.OpeningBalance
		,T1.ProductId
		,T1.PurchaseQty
		,T1.ProductSizeId
	
	UNION ALL
	
	SELECT JobNumber
		,Item
		,ReceivingDate
		,0
		,0
		--	,- ( (T1.Quantity) +  (T1.Breakage))
		,- (Sum(T1.Quantity) + SUM(T1.Breakage))
		,ProductId
		,2 Mode
		,- (Sum(T1.Quantity) + SUM(T1.Breakage)) ClosingBalance
		,ISNULL(ChargeReturnedDate, 0) AS ChargeReturnedDate
		,T1.ProductSizeId
	FROM vw_Grn T1
	WHERE -- T1.WorkOrderNumber = @WorkOrderNumber AND
		--T1.FinYearId = @FinYearId AND
		 T1.ReceivingDate BETWEEN @From
			AND @To
		AND T1.LedgerId = @LedgerId
		AND T1.LedgerSiteId = @LedgerSiteId
		AND (T1.ReceivingDate >= @lastBillDate OR @lastBillDate IS NULL)
	GROUP BY JobNumber
		,Item
		,ReceivingDate
		,ProductId
		,ChargeReturnedDate
		,T1.ProductSizeId
		-- ) 
	) T1
ORDER BY T1.Item
	,T1.StartDate

	/*
--update the rate column of returned items to calculate the amount
UPDATE T2
SET T2.Rate = T1.Rate
FROM @tblData T1
LEFT OUTER JOIN @tblData T2 ON T1.ProductId = T2.ProductId
	AND T2.Mode = 2
	--AND T1.ProductSizeId = T2.ProductSizeId
WHERE T1.Mode = 1

--update the end date and calculate days
 Update T1
 SET T1.[To] =  (CASE WHEN T2.[From] IS NULL THEN @to ELSE DATEADD(DAY,-1,T2.[From]) END),
 T1.Days = DATEDIFF(Day,T1.[From],(CASE WHEN T2.[From] IS NULL THEN @to ELSE DATEADD(DAY,-1,T2.[From]) END)) + 1
 from @tblData T1
 OUTER APPLY (Select * From @tblData WHERE 
				ProductId = T1.ProductId
				AND RowNum = T1.RowNum + 1)
				T2

--update closing balance for the next rows

Update T1
 SET  T1.ClosingBalance = RunningAgeTotal
 from @tblData T1
 INNER JOIN ( SELECT
SUM (Quantity) OVER (Partition by Productid  ORDER BY Productid, [From]) AS RunningAgeTotal, Id
FROM @tblData
) T2 ON T1.Id = T2.Id

*/
--SELECT * FROM @tblData
  --  [p_GeneratBill] '01/01/2021','01/04/2021',2038,1002,3003

DECLARE @Freight NUMERIC(10, 2)

SELECT @Freight = SUM(Freight)
FROM vw_SiteInfo
WHERE LedgerSiteId = @LedgerSiteId
	AND StartDate >= ISNULL(@lastBillDate, StartDate)

--Delete previously billed consumables as consumeables billed as a whole,there is no return for it
DELETE T1
FROM @tblData T1
INNER JOIN Product T2 ON T1.ProductId = T2.ProductId
WHERE T2.Category = 1013
	AND T1.[From] <= @LastBilLDate	 
	  

 SELECT T1.id,T1.Item,T1.ProductId,ISNULL(T1.ProductSizeId,0) ProductSizeId ,Convert(NVARCHAR(10), Cast([From] AS SMALLDATETIME), 103) As [From],
  Convert(NVARCHAR(10),Cast([To] AS SMALLDATETIME), 103) As [To],T1.Days,T1.JobNumber,T1.ChargeReturnedDate,T1.ClosingBalance,
  T1.Mode, T1.Mode as TranType,T1.OPB,T1.Quantity,T1.Rate,
  (CASE WHEN P.Category= 1013 THEN Rate * ClosingBalance ELSE  (Days*Rate *ClosingBalance) END) As Amount,

 P.Category As ItemCategory, @Freight As Freight from @tblData T1
 INNER JOIN Product P ON T1.ProductId = P.Productid
 ORDER BY T1.[From]

  
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_LedgerTransactions_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_LedgerTransactions_ins
 END
 GO
 
CREATE PROC [dbo].[p_LedgerTransactions_ins] (
	@DrLedgerId INT
	,@CrLedgerId INT
	,@TransactoinAmount NUMERIC(10, 3)
	,@TransactionDate SMALLDATETIME
	,@Description NVARCHAR(100) = NULL
	,@CreatedBy INT
	,@TransactionMode int
	,@TransactionType TINYINT
	,@EntryType INT = NULL
	,@Narration NVARCHAR(50) = NULL
	,@TranRefNumber NVARCHAR(50) = NULL
	,@FinYearId INT = NULL
	,@CompanyId INT = NULL
	,@WorkOrderId INT = NULL
	,@LedgerSiteId INT = NULL,
	@ChequeNumber NVARCHAR(50) = null,
	@ChequeDate datetime = null,
	@ChequeBankId int = null,
	@TDS int = null,
	@Discount int = null,
    @ExecutiveName nvarchar(200) = null
	)
	AS 
 BEGIN TRAN
 --   [p_LedgerTransactions_ins] 2048,0,0,'08/18/2024','Bill',23,1,1,1,'Bill','6',1005,1030,null,null

BEGIN TRY

	DECLARE @NextReceiptNumber NVARCHAR(20)
	DECLARE @lastclosingBalance numeric(10,2)
	DECLARE @closingBalance numeric(10,2)
	DECLARE @ledgerId int
	SET @ledgerId = (CASE WHEN @DrLedgerId > 0 THEN @DrLedgerId ELSE @CrLedgerId END)
	--DECLARE @SiteclosingBalance numeric(10,2)
	--get next bill number
	
	SELECT   @lastclosingBalance = ClosingBalance   from LedgerTransactions
	Where Ledgerid = @LedgerId
	 And TransactionDate = (
	 Select Max(TransactionDate) [lastTxnDate] 	 from LedgerTransactions
	Where Ledgerid = @LedgerId
	)
	 SET @lastclosingBalance = ISNULL(@lastclosingBalance,0)
	SET @closingBalance = (CASE WHEN @TransactionType IN(1,3) THEN @lastclosingBalance - @TransactoinAmount
									ELSE @lastclosingBalance + @TransactoinAmount END)
								
 Select *from LedgerTransactions

DECLARE @lastNumber int
Select  @lastNumber  = MAX(SUBSTRING(ReceiptNumber, CHARINDEX('-',ReceiptNumber)+1,LEN(ReceiptNumber)-Charindex('-',ReceiptNumber))) 
from LedgerTransactions
Where CompanyId = @CompanyId AND FinYearId = @FinYearId

SET @NextReceiptNumber = (CASE WHEN @lastNumber IS NULL THEN '1' ELSE @lastNumber + 1 END)

	--EXEC p_getNextId 'LedgerTransaction'

	--	,@FinYearId

	--	,@CompanyId

	--	,NULL

	--	,NULL

	--	,@NextReceiptNumber OUTPUT

		

	INSERT INTO LedgerTransactions (
	LedgerId,
		DrLedgerId

		,CrLederId

		,TransactionAmount

		,TransactionDate

		,Description

		,CreatedBy

		,TransactionType

		,ReceiptNumber

		,EntryType

		,Narration

		,TranRefNumber

		,FinYearId

		,WorkOrderId

		,LedgerSiteId,
		ChequeNumber,
		ChequeDate,
		ChequeBankId,
		TDS,
		Discount,
		ExecutiveName,
		TransactionMode,
		companyId,
		OpBalance,
		ClosingBalance
		)

	VALUES (
	(CASE WHEN @DrLedgerId > 0 THEN @DrLedgerId ELSE @CrLedgerId END),
		@DrLedgerId

		,@CrLedgerId

		,@TransactoinAmount

		,@TransactionDate

		,@Description

		,@CreatedBy

		,@TransactionType

		,@NextReceiptNumber

		,@EntryType

		,@Narration

		,@TranRefNumber

		,@FinYearId

		,@WorkOrderId

		,@LedgerSiteId,
		@ChequeNumber,
		GETDATE(),
		@ChequeBankId,
		@TDS,
		@Discount,
		@ExecutiveName,
		@TransactionMode,
		@CompanyId,
		@lastclosingBalance,
		@closingBalance
		)


	Update ledger
	set balance = @closingBalance where ledgerId = @ledgerId

	SELECT @@IDENTITY AS LedgerTransactionId, @NextReceiptNumber AS ReceiptNumber
 

	COMMIT

END TRY



BEGIN CATCH

	ROLLBACK

END CATCH 
GO

 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Invoice_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Invoice_ins
 END
 GO
 

CREATE PROC [dbo].[p_Invoice_ins] (
	@CompanyId INT
	,@WorkOrderId INT = NULL
	,@InvoiceDate SMALLDATETIME
	,@CreatedBy INT
	,@Details NVARCHAR(500) = NULL
	,@SubTotal NUMERIC(10, 3),
	@total Numeric(10,2)
	,@TaxAmount NUMERIC(10, 3) = 0
	,@InvoiceNumber NVARCHAR(20)
	,@InvoiceType TINYINT = 1
	,@From SMALLDATETIME = NULL
	,@To SMALLDATETIME = NULL
	,@LedgerId INT = NULL
	,@WorkOrderNumber NVARCHAR(50) = NULL
	,@JobNumber NVARCHAR(50) = NULL
	,@ParentInvoiceId INT = NULL
	,@Freight NUMERIC(10, 2)
	,@FreightTax NUMERIC(10, 2) = NULL
	,@FinyearId INT = NULL
	,@BreakageAmount NUMERIC(10, 2) = NULL
	,@BreakageTax NUMERIC(10, 2) = NULL
	,@BranchCode NVARCHAR(20) = NULL
	,@Category NVARCHAR(20) = NULL
	,@ContractorCode NVARCHAR(20) = NULL
	,@OutStanding NUMERIC(10, 2) = NULL
	,@OutStandingType TINYINT
	,@RoundOff BIT = 0
	,@SiteAddress NVARCHAR(300) = NULL
	,@LedgerSiteId INT
	)
AS
DECLARE @lastNumber int
Select  @lastNumber  = MAX(SUBSTRING(InvoiceNumber, CHARINDEX('-',InvoiceNumber)+1,LEN(InvoiceNumber)-Charindex('-',InvoiceNumber))) from Invoice
Where CompanyId = @CompanyId AND FinYearId = @FinYearId

SET @InvoiceNumber = (CASE WHEN @lastNumber IS NULL THEN '1' ELSE @lastNumber + 1 END)

INSERT INTO Invoice (
	CompanyId
	,WorkOrderId
	,InvoiceDate
	,CreatedBy
	,Details
	,SubTotal
	,TaxAmount
	,InvoiceNumber
	,InvoiceType
	,[From]
	,[To]
	,LedgerId
	,WorkOrderNumber
	,JobNumber
	,ParentInvoiceId
	,Freight
	,FreightTax
	,FinYearId
	,BreakageAmount
	,BreakageTax
	,Category
	,BranchCode
	,ContractorCode
	,OutStanding
	,OutStandingType
	,RoundOff
	,SiteAddress
	,LedgerSiteID,Total
	)
VALUES (
	@CompanyId
	,@WorkOrderId
	,@InvoiceDate
	,@CreatedBy
	,@Details
	,@SubTotal
	,@TaxAmount
	,@InvoiceNumber
	,@InvoiceType
	,@From
	,@To
	,@LedgerId
	,@WorkOrderNumber
	,@JobNumber
	,@ParentInvoiceId
	,@Freight
	,@FreightTax
	,@FinyearId
	,@BreakageAmount
	,@BreakageTax
	,@Category
	,@BranchCode
	,@ContractorCode
	,@OutStanding
	,@OutStandingType
	,@RoundOff
	,@SiteAddress
	,@LedgerSiteId,
	@total
	)

UPDATE Invoice
SET RoundedAmount = (
		CASE 
			WHEN @RoundOff = 1
				THEN ROUND(Total, 0)
			ELSE Total
			END
		)
WHERE InvoiceId = @@IDENTITY

 


SELECT *
FROM Invoice
WHERE InvoiceId = @@IDENTITY
	--RETURN @@IDENTITY
	 
 
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_GRN_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_GRN_ins
 END
 GO
 
CREATE PROC [dbo].[p_GRN_ins] (
	@JobNumber NVARCHAR(20) = NULL
	,@SiteName NVARCHAR(100) = NULL
	,@ReceivingDate SMALLDATETIME
	,@GRN NVARCHAR(20) = NULL
	,@Receiver NVARCHAR(100) = NULL
	,@Sender NVARCHAR(100) = NULL
	,@LedgerId INT
	,@FinYearId INT = 0
	,@Remarks NVARCHAR(200) = NULL
	,@WorkOrderId INT = NULL
	,@LedgerSiteId INT = NULL
	,@TransactionId INT = 0,
	@companyId int
	)
AS
DECLARE @lastNumber int
Select  @lastNumber  = MAX(SUBSTRING(GRN, CHARINDEX('-',GRN)+1,LEN(GRN)-Charindex('-',GRN))) from GRN
Where CompanyId = @CompanyId AND FinYearId = @FinYearId

SET @GRN = (CASE WHEN @lastNumber IS NULL THEN '1' ELSE @lastNumber + 1 END)

INSERT INTO GRN (
	JobNumber
	,SiteName
	,ReceivingDate
	,GRN
	,Receiver
	,Sender
	,LedgerId
	,FinYearId
	,Remarks
	,WorkOrderId
	,LedgerSiteId
	,TransactionId
	)
VALUES (
	@JobNumber
	,@SiteName
	,@ReceivingDate
	,@GRN
	,@Receiver
	,@Sender
	,@LedgerId
	,@FinYearId
	,@Remarks
	,@WorkOrderId
	,@LedgerSiteId
	,@TransactionId
	)

SELECT SCOPE_IDENTITY()

 
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_GRN_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Account_ledger
 END
 GO
 
CREATE PROC [dbo].[p_Account_ledger] (  
 @LedgerId INT  
 ,@From SMALLDATETIME  
 ,@To SMALLDATETIME  
 ,@LedgerSiteId INT = NULL  
 )  
AS  
-- [p_Account_ledger] 2047,'01/01/2024','08/08/2024'  

if (@LedgerSiteId <= 0)
Begin
	set @LedgerSiteId = null
End;

Select L.Name,L.Code,LT.Narration,Lt.TransactionDate,LT.TransactionType,LT.TransactionAmount,
LT.Closingbalance As Closingbalance,LT.OPBalance, L.balance as Balance
from Ledger L
INNER JOIN LedgerTransactions LT ON L.LedgerId = LT.LedgerId
Where L.LedgerId = @LedgerId
And TransactionDate Between @From and @To
And (LedgerSiteId = @LedgerSiteId OR @LedgerSiteId IS NULL)

/*
WITH Tran_CTE (  
 rowNum
 ,Name
 ,Code
 ,TransactionDate  
 ,EntryTypeName  
 ,TransactionAmount  
 ,TransType  
 ,Remarks  
 )  
AS (  
 SELECT ROW_NUMBER() OVER (  
   ORDER BY TransactionDate  
   ) AS rowNum  
  ,T1.*  
 FROM (  
  SELECT 
  Name,Code,
  CreationDate AS TransactionDate  
   ,'Opening Balance' AS EntryTypeName  
   ,(  
    CASE   
     WHEN TransType = 1  
      THEN OpeningBal  
     ELSE - OpeningBal  
     END  
    ) AS TransactionAmount  
   ,TransType  
   ,'' AS Remarks  
  FROM Ledger  
  WHERE LedgerId = case when @LedgerId > 0 then @LedgerId else LedgerId end  
    
  UNION ALL  
    
  -- bills will alwasy be debit  
  SELECT 
   L.Name,L.Code,
   InvoiceDate  
   ,('Party Bill -' + Convert(NVARCHAR(12), [From], 110) + ' To ' + Convert(NVARCHAR(12), [To], 110)) AS EntryType  
   ,Total  
   ,1 TransactionMode  
   ,'' AS Remarks  
  FROM Invoice I 
  inner join Ledger L on I.LedgerId = L.LedgerId
  WHERE I.LedgerId =case when @LedgerId > 0 then @LedgerId else I.LedgerId end 
   AND InvoiceDate BETWEEN @From  
    AND @To  
   AND StatusId <> 2 -- exclude deleted bills  
   AND LedgerSiteId = ISNULL(@LedgerSiteId,LedgerSiteId)  
  
  UNION ALL  
    
  SELECT 
   L.Name,L.Code,
   TransactionDate  
   ,T2.NAME + (  
    CASE   
     WHEN TranRefNumber IS NOT NULL  
      OR TranRefNumber <> ''  
      THEN + ' Ref#: ' + TranRefNumber  
     ELSE ''  
     END  
    ) AS EntryTypeName  
   ,(  
    CASE   
     WHEN T1.CrLederId = @LedgerId  
      THEN - TransactionAmount  
     ELSE TransactionAmount  
     END  
    ) TransactionAmount  
   ,(  
    CASE   
     WHEN T1.CrLederId = @LedgerId  
      THEN 2  
     ELSE 1  
     END  
    ) AS TransType  
   ,Description AS Remarks  
  FROM LedgerTransactions T1  
   inner join Ledger L on  case when T1.DrLedgerId >0 then T1.DrLedgerId else T1.CrLederId end = L.LedgerId
  LEFT OUTER JOIN LookupEntryTypes T2 ON T1.EntryType = T2.EntryTypeId  
  WHERE (  
    CrLederId = case when @LedgerId > 0 then @LedgerId else CrLederId end  
    OR DrLedgerId = case when @LedgerId > 0 then @LedgerId else DrLedgerId end  
    )  
   AND TransactionStatus <> 2  
   AND TransactionDate BETWEEN @From  
    AND @To  
    AND T1.LedgerSiteId = ISNULL(@LedgerSiteId,T1.LedgerSiteId)  
  ) T1  
  -- ORDER BY T1.TransactionDate  
 )  
--SELECT * From Tran_CTE  
SELECT T1.rowNum
 ,T1.Name
 ,T1.Code
 ,T1.EntryTypeName  
 ,T1.TransactionDate  
 ,T1.TransType AS TransactionType  
 ,(  
  CASE   
   WHEN T1.TransType = 2  
    THEN - T1.TransactionAmount  
   ELSE T1.TransactionAmount  
   END  
  ) AS TransactionAmount  
 ,(  
  CASE   
   WHEN SUM(T2.TransactionAmount) < 0  
    THEN - SUM(T2.TransactionAmount)  
   ELSE SUM(T2.TransactionAmount)  
   END  
  ) AS Balance  
 ,(  
  CASE   
   WHEN SUM(T2.TransactionAmount) < 0  
    THEN 'Cr'  
   ELSE 'Dr'  
   END  
  ) AS TransactionTypeName  
 ,T1.Remarks AS Description  
FROM Tran_CTE T1  
INNER JOIN Tran_CTE T2 ON T1.rowNum >= T2.rowNum  
GROUP BY T1.rowNum  
 ,T1.Name  
 ,T1.Code  
 ,T1.EntryTypeName  
 ,T1.TransactionDate  
 ,T1.TransType  
 ,T1.TransactionAmount  
 ,T1.Remarks  
ORDER BY TransactionDate  
 */
  
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_RBNClient_updInfo'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_RBNClient_updInfo
 END
 GO
 
CREATE proc [dbo].[p_RBNClient_updInfo] (
@rbnClientId int,
@Address1 nvarchar(100),
@Address2 nvarchar(100)=null,
@stateId int,
@city nvarchar(50),
@pan nvarchar(20)=null,
@gst nvarchar(20)=null,
@email nvarchar(100),
@mobile nvarchar(20),
@spocname nvarchar(50)=null,
@nogst tinyint,
@pinCode int
)
AS
Update RbnClients
SET StateId=@stateId,City=@city,ZipCode=@pinCode,Address1=@Address1,Address2= @Address2,
pan=@pan,GSTNo=@gst,nogst=@nogst,email = @email,mobile = @mobile,SpocName=@spocname
where RbnClientId=@rbnClientId

 
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_overdueAmount'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_overdueAmount
 END
 GO
 
CREATE proc [dbo].[p_overdueAmount]
(
@companyId int,
@dueFrom int
)
AS
Select T2.LedgerId, T2.Name,T2.Code, ISNULL(Sum(T1.balance),0) as AmountDue from Invoice T1
Inner join Ledger T2 ON T1.LedgerId = T2.LedgerId
Where T1.CompanyId=@companyId AND DateDiff(Day, T1.CreationDate ,GetDate()) >=@dueFrom
GROUP BY T2.LedgerId, T2.Name,T2.Code
 
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_getBalanceAfterLastBill'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_getBalanceAfterLastBill
 END
 GO
 
 CREATE proc [dbo].[p_getBalanceAfterLastBill] (
 @ledgerId int,

 @ledgerSiteId int
 )
 AS
 /*
 p_getBalanceAfterLastBill 2047,3009
 */
 
 Select T3.ProductId, T1.[To], T3.CB,T3.Rate,T4.Name as Product from Invoice T1
 INNER JOIN (
 Select Max([To]) as LastBillDate From Invoice T1
 Where   LedgerId = @ledgerId
 And LedgerSiteId = @ledgerSiteId)
 T2 ON T1.[To] = T1.[To]
 INNER JOIN InvoiceItems T3 ON T1.InvoiceId = T3.InvoiceId
 INNER JOIN Product T4 ON T3.ProductId = T4.ProductId
  Where  LedgerId = @ledgerId
 And LedgerSiteId = @ledgerSiteId
 AND T2.LastBillDate = T3.[To]
   

 --  select *from Invoice
 --select *from InvoiceItems where InvoiceId= 4240
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_siteItemBalance_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_siteItemBalance_ins
 END
 GO
 
 CREATE proc [dbo].[p_siteItemBalance_ins]
 (
 @ledgerSiteId int,
 @itemId int,
 @balance numeric(10,2),
 @updatedDate smallDateTime,
 @updatedBy int,
 @ledgerId int,
 @companyId int
 )
 AS
 IF NOT EXISTS(SELECT itemId from SiteItemBalance where LedgerSiteid = @ledgerSiteId And itemId = @itemId)
 BEGIN
 INSERT INTO SiteItemBalance(ledgerSiteId,ItemId,Balance,UpdatedDate,UpdatedBy,LedgerId,CompanyId)
 Values(@ledgerSiteId,@itemId,@balance,@updatedDate,@updatedBy,@ledgerId,@companyId)
 END
 ELSE 
 BEGIN
 UPDATE SiteItemBalance
 SET Balance = Balance+ @balance Where ledgerSiteId = @ledgerSiteId And Itemid = @itemId and LedgerId= @ledgerId
 END
GO

IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_BillingReport'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_BillingReport
 END
 GO
 
CREATE PROC [dbo].[p_BillingReport] (@InvoiceId INT)
/*

[p_BillingReport] 4222

*/
AS
DECLARE @Freight NUMERIC(10, 2) = 0
DECLARE @InvoiceFrom SMALLDATETIME
	,@InvoiceTo SMALLDATETIME
DECLARE @LedgerId INT
	,@CompanyId INT
DECLARE @TotalBalanceItems NUMERIC(10, 2) = 0
	,@TotalLossCharges NUMERIC(10, 2) = 0
DECLARE @BalanceItems NVARCHAR(1000) = ''
DECLARE @lederSiteId int

SELECT @Freight = (Freight + BreakageAmount)
	,@InvoiceFrom = [From]
	,@InvoiceTo = [To]
	,@LedgerId = LedgerId
	,@CompanyId = CompanyId
	,@lederSiteId = LedgerSiteId
FROM Invoice
WHERE InvoiceId = @InvoiceId

SELECT @BalanceItems = @BalanceItems + (Item + ': ' + Cast(ClosingBalance AS NVARCHAR(10))) + ', '
FROM dbo.fun_PartyStock_balance(@LedgerId, @InvoiceFrom, @InvoiceTo, @CompanyId, NULL,@lederSiteId)
WHERE ClosingBalance <> 0

SELECT @TotalBalanceItems = SUM(ClosingBalance)
FROM dbo.fun_PartyStock_balance(@LedgerId, @InvoiceFrom, @InvoiceTo, @CompanyId, NULL,@lederSiteId)

SELECT @TotalLossCharges = SUM(Amount)
FROM LossItems
WHERE InvoiceId = @InvoiceId

DECLARE @Receipts nvarchar(500) = ''
DECLARE @ReceiptAmount NUMERIC(10,2)
SELECT @Receipts = @Receipts + ',' +  CAST(TransactionAmount AS nvarchar(10)) + ' Cheq no: ' +
ChequeNumber + ' dtd ' + Convert(nvarchar(10),ChequeDate,103) 
  FROM LedgerTransactions WHERE CrLederId = @LedgerId

DECLARE @LastBillNo nvarchar(20),@LastBillAmount Numeric(10,2),@LastBillDate Nvarchar(10)

SELECT @LastBillNo = T2.InvoiceNumber,@LastBillAmount = T2.Total, @LastBillDate = T2.InvoiceDate
 FROM Invoice T1
CROSS APPLY (
SELECT  InvoiceDate, Total,InvoiceNumber FROM Invoice WHERE InvoiceDate < T1.InvoiceDate
AND InvoiceDate = (SELECT Max(InvoiceDate) From Invoice Where InvoiceDate < T1.InvoiceDate
AND LedgerId = T1.LedgerId AND LedgerSiteId = T1.LedgerSiteId AND InvoiceId <> T1.InvoiceId)
AND LedgerId = T1.LedgerId AND LedgerSiteId = T1.LedgerSiteId

 
) T2
WHERE T1.InvoiceId = @InvoiceId

  SELECT 
@ReceiptAmount = SUM(TransactionAmount)
  FROM LedgerTransactions WHERE CrLederId = @LedgerId

SELECT *
	,@BalanceItems AS BalanceItems
	,@TotalBalanceItems TotalBalance
	,@TotalLossCharges AS LossCharges
	,@Receipts As Receipts
	,@ReceiptAmount As ReceiptAmount
	,@LastBillAmount AS LastBillAmount
	,@LastBillDate AS LastBillDate
	,@LastBillNo As LastInvoiceNumber
FROM vw_BillingReport
WHERE InvoiceId = @InvoiceId 

EXEC p_invoiceTax_sel @InvoiceId

--Breakage Items
EXEC p_BreakageItemsByInvoice_sel @InvoiceId

--LossItems
EXEC p_LossItemsByInvoice_sel @InvoiceId

	--SElect * from InvoiceItems Where ProductSizeId >0
	GO
	 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Config_sel'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Config_sel
 END
 GO
 create proc [dbo].[p_Config_sel] (
@companyId int
)
AS

Select * from Config where CompanyId  = @companyId
GO