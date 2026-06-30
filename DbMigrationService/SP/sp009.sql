 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_PostStock'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_PostStock
 END
 GO
 CREATE PROC [dbo].[p_PostStock] (
	@ProductId INT
	,@ProductSizeId INT = NULL
	,@Quantity NUMERIC(10, 2)
	,@FINYEAR INT
	,@PostingType TINYINT
	,@PostingDate SMALLDATETIME = NULL
	,@Remarks NVARCHAR(100) = NLL
	,@VoucherId INT = NULL
	,@PostedBy INT
	,@CompanyId INT
	,@headerId int
	,@deleted tinyint =0
	)
AS
DECLARE @FirstDateOfFinyear SMALLDATETIME

SELECT @FirstDateOfFinyear = MInDate
FROM FinYear
WHERE FinYearId = @FINYEAR

--insert the opening balance only once for a product size. The next time it will calculate
-- on stock reconcilation
IF (
		  EXISTS (
			SELECT ProductId
			FROM StockTransaction
			WHERE ProductId = @ProductId And CompanyId = @CompanyId
			AND @PostingType = 1
				AND FinYear= @FINYEAR
			)
		)
BEGIN
Update StockTransaction
SET Quantity = @Quantity   
WHERE ProductId = @ProductId
			 
				AND @PostingType = 1
				AND FinYear= @FINYEAR
END

ELSE if @deleted =1 
BEGIN

Delete from StockTransaction where TransactionHeaderId = @headerId
And companyid = @companyId And ProductId = @ProductId
END
ELSE
BEGIN
Delete from StockTransaction where TransactionHeaderId = @headerId
And companyid = @companyId And ProductId = @ProductId
INSERT INTO StockTransaction (
		ProductId
		,ProductSizeId
		,Quantity
		,FinYear
		,PostingDate
		,PostingType
		,Remarks
		,VoucherId
		,PostedBy
		,CreatedOn
		,CompanyId
		,TransactionHeaderId
		)
	VALUES (
		@ProductId
		,@ProductSizeId
		,@Quantity
		,@FINYEAR
		,@PostingDate
		,@PostingType
		,@Remarks
		,@VoucherId
		,@PostedBy
		,GetDate()
		,@CompanyId
		,@headerId
		)
END
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_stockTransaction_del'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_stockTransaction_del
 END
 GO
 CREATE proc [dbo].[p_stockTransaction_del]
(
@stockTransactionHeaderId int,
@companyId int
)
AS

Delete from StockTransactionHeader where StockTransactionHeaderId = @stockTransactionHeaderId
And companyid = @companyId
Delete from StockTransaction where TransactionHeaderId = @stockTransactionHeaderId
And companyid = @companyId  
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_StockTransactionHeader_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_StockTransactionHeader_ins
 END
 GO
 
CREATE proc [dbo].[p_StockTransactionHeader_ins]
(
@companyId int,
@voucherId nvarchar(20),
@postingDate date,
@postingType tinyint,
@finyearId int,
@guId nvarchar(50)=NULL,
@rbnClientId int,
@createdBy int=NULL,
@createdOn smallDateTime= NULL,
@stockTransactionHeaderId int=0,
@ModifiedBy Int =0,
@ModifiedOn SmallDateTime= NULL
)
AS

IF(@stockTransactionHeaderId = 0)
BEGIN
INSERT INTO StockTransactionHeader(PostingDate,VoucherId,PostingType,CompanyId,FinYearId,GuId,RbnClientId,
CreatedBy,CreatedOn)
Values(@postingDate,@voucherId,@postingType,@companyId,@finyearId,@guId,@rbnClientId,@createdBy,@createdOn)
SELECT SCOPE_IDENTITY()

END
IF (@stockTransactionHeaderId > 0)
BEGIN
 UPDATE StockTransactionHeader
 SET PostingDate = @postingDate, VoucherId = @voucherId, ModifiedBy = @ModifiedBy, ModifiedOn= @ModifiedOn 
 Where StockTransactionHeaderId = @stockTransactionHeaderId
 SELECT @stockTransactionHeaderId
END
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_stockpostingByHeader_sel'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_stockpostingByHeader_sel
 END
 GO
 CREATE proc [dbo].[p_stockpostingByHeader_sel]
 (
 @TransactionHeaderId int,
 @companyId int
 )
 AS
 Select t1.*,T2.ProductId,T2.Quantity,T3.Name as Product from StockTransactionHeader t1
 INNER JOIN StockTransaction T2 ON T1.StockTransactionHeaderId = T2.TransactionHeaderId
 INNER JOIN Product T3 ON T2.ProductId = T3.ProductId
 Where   stockTransactionHeaderId = @TransactionHeaderId
  And T1.companyId = @companyId
GO
 IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_ItemStock_insupd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_ItemStock_insupd
 END
 GO
 CREATE proc [dbo].[p_ItemStock_insupd]
 (
 @effectiveDate date,
 @finYearId int,
 @companyId int,
 @rbnClientId int
 )
 /*

 p_ItemStock_insupd '2024-10-10',1005,1034,28
 */
 
 AS
DECLARE @opbalance numeric(10,2)=0;
DECLARE @FinYearMinDate SmallDateTime,@FinYearMaxDate SmallDateTime
SELECT @FinYearMinDate = MinDate,@FinYearMaxDate = MaxDate From FinYear Where FinYearId = @FinYearId

DELETE FROM ItemStock where Cast(EffectiveDate as Date) = CAST(@effectiveDate as Date)
And companyId = @companyId

SeLECT @opbalance = Quantity From StockTransaction
where PostingType =1 and FinYear = @finYearId And CompanyId = @companyId
 ;
 with cte
 as
 (
Select v1.SentProductId,v1.CompanyId, SUM(sentQty)as qty, 1 as txnType  from vw_ItemsIssued v1
WHERE SentDate Between @FinYearMinDate ANd @FinYearMaxDate AND CompanyId = @CompanyId
GROUP BY SentProductId,CompanyId
UNION ALL
Select v1.ProductId,v1.CompanyId,  SUM(Quantity) as qty, 2 as txnType from vw_GRN v1
Where v1.CompanyId = @companyId and v1.RbnClientId = @rbnClientId
and v1.FinYearId = @finYearId
group by v1.ProductId,v1.CompanyId
) 
INSERT INTO ItemStock(ProductId,OpeningStock,EffectiveDate,StockInHand,OnSite,FinYearId,CompanyId)

select DIStiNCT SentProductId,@opbalance as OpBalance, CAST( GetDate() as Date), 
@opBalance + ISNULL(tr.qty,0) - ISNULL(ts.qty,0) as ClosingBalance,
ISNULL(ts.qty,0) -  ISNULL(tr.qty,0)  as onSite, @finYearId,@companyId

from cte t1
OUTER APPLY(SELECT qty from cte s where s.SentProductId = t1.SentProductId
and s.txnType =1
)
As ts
OUTER APPLY(SELECT qty from cte s where s.SentProductId = t1.SentProductId
and s.txnType =2
)
As tr
GO
IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_stockpostingHeader_sel'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_stockpostingHeader_sel
 END
 GO
 CREATE proc [dbo].[p_stockpostingHeader_sel]
 (
 @from date,
 @to date,
 @companyId int
 )
 AS
 Select t1.*,u.FullName as CreatedByName,'' as Remarks,t2.Name as PostingTypeName from StockTransactionHeader t1
 INNER JOIN LookupStockPostingType t2 on t1.postingType = t2.PostingTypeId
 INNER JOIN Users u on t1.createdBy = u.UserId
 Where Postingdate between @from and @to
 and companyid = @companyId
