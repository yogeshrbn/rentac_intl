 USE [$AppDb$]
GO 
if exists(select 1 from sys.views where name='vw_Inventory' and type='v')
drop view vw_Inventory;
go

CREATE VIEW [dbo].[vw_Inventory]
AS
SELECT  
	 T3.Site
	,T3.SiteId
	,T4.PurchaseQty AS Quantity
	,T3.WorkOrderId
	,T4.ProductId AS  ProductId
	,T3.StartDate AS [Date]
	,T5.NAME AS Product
	,T7.NAME AS Client
	,T8.NAME AS Company
	,T6.Number AS WOrkOrderNumber
	,1 AS Mode
	,T3.JobNumber
FROM SiteInfo T3
INNER JOIN WorkOrder T6 ON T3.WorkOrderId = T6.WorkOrderId
INNER JOIN Ledger T7 ON T7.LedgerId = T6.LedgerId
INNER JOIN WorkOrderItems T4 ON T3.SiteId = T4.SiteId
INNER JOIN Product T5 ON T4.ProductId = T5.ProductId
INNER JOIN Company T8 ON T6.CompanyId = T8.CompanyId
-- Where T6.Number = '2565'
UNION  
SELECT  SITE,0 As SiteId,Quantity, WorkOrderid,ProductId,ReceivingDate As [Date],Item as Product,CLient,  T8.Name as Company
,JobNumber as WOrkOrderNumber,2 As Mode,JobNumber FROM vw_GRN T1
INNER JOIN Company T8 ON T1.CompanyId = T8.CompanyId
--Where JobNumber = '2565'
GO
if exists(select 1 from sys.views where name='vw_SiteInfo' and type='v')
drop view vw_SiteInfo;
go

CREATE VIEW [dbo].[vw_SiteInfo]
AS
SELECT T1.SiteId,T1.WorkOrderId,T1.JobNumber,T1.ChallanNumber,T1.Site,T1.ShaftSize,T1.ShaftHeight,T1.SiteEng,T1.SubTotal,
T1.TaxAmount,T1.Freight,T1.Total,T1.CreatedBy,T1.CreatedDate,T1.StartDate,T1.Duration,T1.ParentSite,T1.Closed,
T1.PaymentClosedDate,T1.FreightTax,T1.State,T1.VehicleId,T1.DriverId,T1.LedgerSiteId,
T5.Name As Driver,T6.Name As Vehicle
	,T2.Client
	,T2.Company
	,T2.Number AS WorkOrderNumber
	,T2.LedgerId
	,T2.FinYearId
	,T2.ClientAmount
	,T2.[Type]
	,T2.CompanyId
	,T2.ParentWorkOrderId
	,T2.Type AS ChallanType
	,T2.RbnClientId
	,T3.GSTCode AS SiteStateGSTCode
	,T4.SiteAddress
	,T4.ContactPerson
	,T4.ContactPersonPhone
	,T4.SiteGST,
	T1.ClosedDate ,
	(CASE WHEN T1.PaymentClosedDate IS NOT NULL THEN 1 ELSE 0 END) as PaymentClosed
FROM SiteInfo T1
INNER JOIN vw_WorkOrder T2 ON T1.WorkOrderId = T2.WorkOrderId
INNER JOIN ClientSites T4 ON T1.LedgerSiteId = T4.LedgerSiteId
LEFT OUTER JOIN LookupStates T3 ON T1.STATE = T3.StateName
LEFT OUTER JOIN Employee T5 ON T1.DriverId = T5.EmployeeId
LEFT OUTER JOIN Vehicle T6 ON T1.VehicleId = T6.VehicleId
GO
if exists(select 1 from sys.views where name='vw_ItemsIssued' and type='v')
drop view vw_ItemsIssued;
go

CREATE VIEW [dbo].[vw_ItemsIssued]
AS
SELECT T3.Site
	,T3.SiteId
	,CAST(T4.PurchaseQty AS INT) AS SentQty
	,T3.WorkOrderId
	,T4.ProductId AS SentProductId
	,T3.StartDate AS SentDate
	,T5.NAME AS Product
	,T5.Code AS HSNCode
	,T7.NAME AS Client
	,T8.NAME AS Company
	,T6.LedgerId
	,T6.Number AS WorkOrderNumber
	,T3.JobNumber
	,T3.ChallanNumber
	,T6.RbnClientId
	,T8.CompanyId
	,	CAST(T4.Rate AS NUMERIC(10, 2)) As Rate
	,CAST(T4.SubTotal AS NUMERIC(10, 2)) As SubTotal
	,T4.WorkOrderItemId
	,(
		CASE 
			WHEN T4.ProductSizeId = 0
				THEN NULL
			ELSE T4.ProductSizeId
			END
		) AS ProductSizeId
	,T9.Size
	,CAST(T3.SubTotal AS NUMERIC(10, 2)) AS ApproxValue
	,T6.Type AS ChallanType
	,T5.Category AS ProductCategory
	,T3.Vehicle
	,T3.Driver
	,CAST(T3.Freight AS NUMERIC(10, 2)) AS Freight
	,T3.[State]
	,T3.LedgerSiteId
	,T6.TransactionId
FROM SiteInfo T3
INNER JOIN WorkOrder T6 ON T3.WorkOrderId = T6.WorkOrderId
INNER JOIN Ledger T7 ON T7.LedgerId = T6.LedgerId
INNER JOIN WorkOrderItems T4 ON T3.SiteId = T4.SiteId
INNER JOIN Product T5 ON T4.ProductId = T5.ProductId
INNER JOIN Company T8 ON T5.CompanyId = T8.CompanyId
LEFT OUTER JOIN ProductSize T9 ON T4.PRoductSizeId = T9.ProductSizeId
GO