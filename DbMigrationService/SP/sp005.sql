 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_selectBills'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_selectBills
 END
 GO
CREATE PROC [dbo].[p_selectBills] (
	@LedgerId INT = NULL
	,@From SMALLDATETIME
	,@To SMALLDATETIME
	,@CompanyId INT
	,@LedgerSiteId INT = NULL
	)
AS
-- [p_selectBills] 2053,'01/04/2024','10/15/2024',1035

SELECT T1.* 
	,T2.NAME AS Client
FROM Invoice T1
INNER JOIN Ledger T2 ON T1.LedgerId = T2.LedgerId
WHERE --InvoiceType IN(2,3) AND
	T1.LedgerId = ISNULL(@LedgerId, T1.LedgerId)
	AND InvoiceDate BETWEEN @From
		AND @To
	AND T2.CompanyId = (
		CASE 
			WHEN @CompanyId IS NULL
				OR @CompanyId = 0
				THEN T2.CompanyId
			ELSE @CompanyId
			END
		)
	AND LedgerSiteId = ISNULL(@LedgerSiteId,LedgerSiteId)
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
	,@otherCharges numeric(10,2)=0
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
	,LedgerSiteID,Total,
	OtherChargeAmount
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
	@total,@otherCharges
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