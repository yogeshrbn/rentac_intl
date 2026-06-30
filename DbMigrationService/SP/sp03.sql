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