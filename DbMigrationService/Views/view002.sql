 USE [$AppDb$]
GO 
if exists(select 1 from sys.views where name='vw_BillingReport' and type='v')
drop view vw_BillingReport;
go
CREATE VIEW [dbo].[vw_BillingReport]
AS
SELECT T3.WorkOrderNumber
	,T3.LedgerId
	,T5.NAME AS Company
	,T1.InvoiceId
	,T1.ProductId
	,T3.Freight
	,T3.TaxAmount
	,CAST(T3.Total AS NUMERIC(10, 2)) AS Total
	,Cast(T1.Rate AS NUMERIC(10, 2)) AS [Rate]
	,CAST(T1.Quantity AS INT) AS [Quantity]
	,CAST(T1.SubTotal AS NUMERIC(10, 2)) AS SubTotal
	,CAST(T1.Days AS INT) AS [Days]
	,CAST(T1.CB AS INT) AS [CB]
	,CAST(T1.OPB AS INT) AS [OPB]
	,T1.[From] AS [From]
	,T1.[To] AS [To]
	,CAST(CAST(T1.CB AS INT) AS NVARCHAR(10)) + (
		CASE 
			WHEN T1.OPB > 0
				THEN ' (' + (
						Cast(CAST(T1.OPB AS INT) AS NVARCHAR(10)) + (
							CASE 
								WHEN T1.Quantity >= 0
									THEN ' + '
								ELSE ''
								END
							) + CAST(CAST(T1.Quantity AS INT) AS NVARCHAR(10))
						) + ' )'
			ELSE ''
			END
		) AS QtyDesc
	,(CAST(Datepart(Day, T1.[From]) AS NVARCHAR(2)) + '/' + CAST(Datepart(MONTH, T1.[From]) AS NVARCHAR(2)) + ' - ' + CAST(Datepart(Day, T1.[To]) AS NVARCHAR(2)) + '/' + CAST(Datepart(MONTH, T1.[To]) AS NVARCHAR(2))) AS Period
	,CONVERT(NVARCHAR(12), T3.InvoiceDate, 103) AS InvoiceDate
	,T3.InvoiceNumber
	,T3.InvoiceType
	,T6.Address1 AS Site
	,T2.NAME AS Item
	,T5.Address1 AS CompanyAddress1
	,T5.Address2 AS CompanyAddress2
	,(T5.Address1 + ' ' + T5.Address2) AS CompanyAddress
	,T5.Email AS CompanyEmail
	,T5.Phone1 AS CompanyPhone
	,T5.City AS CompanyCity
	,T5.ZipCode AS CompanyZipCode
	,T5.TIN AS CompanyTIN
	,T5.PAN AS CompanyPAN
	,T5.GSTNo AS CompanyGST
	,T6.Address1 AS ClientAddress1
	,T6.Address2 AS ClientAddress2
	,T6.Email AS ClientEmail
	,T6.Phone1 AS ClientPhone
	,T6.City AS ClientCity
	,T6.ZipCode AS ClientZipCode
	,T6.NAME AS Client
	,T3.ParentInvoiceId
	,T3.BreakageTax
	,T3.BreakageAmount
	,T3.OutStanding
	,T3.OutStandingType
	,T3.RoundOff
	,T3.RoundedAmount
	,T2.Code AS HSNCode
	,T5.GSTRegisteredOffice
	,T7.Size AS ProductSize
	--	,T3.SiteAddress
	,T8.SiteAddress
	,T8.SiteGST
	,T8.ContactPerson
	,T8.City
	,T8.ContactPersonPhone
	,T8.STATE AS SiteState
	,T3.OtherChargeAmount  
FROM InvoiceItems T1
INNER JOIN Invoice T3 ON T1.InvoiceId = T3.InvoiceId
INNER JOIN Company T5 ON T3.CompanyId = T5.CompanyId
INNER JOIN Ledger T6 ON T3.LedgerId = T6.LedgerId
INNER JOIN RbnClients T4 ON T4.RbnClientId = T6.RbnClientId
INNER JOIN ClientSites T8 ON T3.LedgerSiteId = T8.LedgerSiteId
--INNER JOIN vw_WorkOrder T4 ON T3.WorkOrderNumber = T4.Number
LEFT OUTER JOIN Product T2 ON T1.ProductId = T2.ProductId
LEFT OUTER JOIN ProductSize T7 ON T1.ProductSizeId = T7.ProductSizeId
	AND T1.ProductId = T7.ProductId
--SELECT * FROM [vw_BillingReport] where InvoiceId = 4229
  
GO

