 USE [$AppDb$]
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
	@RefLedgerId int =0,
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
		ClosingBalance,
		RefLedgerId
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
		@closingBalance,
		@RefLedgerId
		)


	Update ledger
	set balance = @closingBalance where ledgerId = @ledgerId

	SELECT @@IDENTITY AS LedgerTransactionId, @NextReceiptNumber AS ReceiptNumber
 

	COMMIT

END TRY



BEGIN CATCH

	ROLLBACK

END CATCH
 