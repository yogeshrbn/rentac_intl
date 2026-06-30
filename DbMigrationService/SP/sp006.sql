 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_ledger_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_ledger_upd
 END
 GO
 CREATE PROC [dbo].[p_ledger_upd] (
	@Name NVARCHAR(200)
	,@Code NVARCHAR(20)
	,@Address1 NVARCHAR(100) = NULL
	,@Address2 NVARCHAR(100) = NULL
	,@Email NVARCHAR(200) = NULL
	,@Fax NVARCHAR(15) = NULL
	,@Phone1 NVARCHAR(15) = NULL
	,@Phone2 NVARCHAR(15) = NULL
	,@Contact NVARCHAR(50) = NULL
	,@City NVARCHAR(50) = NULL
	,@State NVARCHAR(50) = NULL
	,@ZipCode NVARCHAR(10) = NULL
	,@Web NVARCHAR(100) = NULL
	,@TIN NVARCHAR(20) = NULL
	,@TAN NVARCHAR(20) = NULL
	,@AccGroup TINYINT = NULL
	,@OpeningBal NUMERIC(10, 2) = 0
	,@TransType TINYINT = NULL
	,@GSTNumber NVARCHAR(20) = NULL
	,@AadharNumber NVARCHAR(20) = NULL
	,@PAN NVARCHAR(20) = NULL
	,@ServiceTax NVARCHAR(20) = NULL
	,@ContactPersonName NVARCHAR(50) = NULL
	,@ContactPersonDesignation NVARCHAR(50) = NULL
	,@ContactPersonOffPhone NVARCHAR(20) = NULL
	,@ContactPersonMobile NVARCHAR(20) = NULL
	,@DefaultRate NUMERIC(5, 2) = 0
	,@CompanyId int
	,@RbnClientId int
	,@LedgerId INT
 
	)
AS
IF EXISTS(SELECT GSTNo from Ledger where GSTNo = @GSTNumber AND @GSTNumber IS NOT NULL  AND LEN(TRIM(@GSTNumber)) > 1
AND LedgerId <> @LedgerId AND CompanyId = @CompanyId
) 
BEGIN

RAISERROR('Cannot Insert duplicate GST No',16,1)
return 0;
END
IF EXISTS(SELECT PAN from Ledger where PAN = @PAN AND @PAN IS NOT NULL AND LEN(TRIM(@pan)) > 1
AND LedgerId <> @LedgerId AND CompanyId = @CompanyId) 
BEGIN
RAISERROR('Cannot Insert duplicate PAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where Name = TRIM(@name) AND RbnClientId = @RbnClientId
AND LedgerId <> @LedgerId AND CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate ledger',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where TAN = TRIM(@TAN)   AND @TAN IS NOT NULL AND LEN(TRIM(@TAN)) > 1
AND LedgerId <> @LedgerId AND CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate TAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where TIN = TRIM(@TIN)   AND @TIN IS NOT NULL AND LEN(TRIM(@TIN)) > 1
AND LedgerId <> @LedgerId AND CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate TIN',16,1)
return 0;
END

UPDATE Ledger
SET NAME = @Name
	,Code = @Code
	,Address1 = @Address1
	,Address2 = @Address2
	,Email = @Email
	,Fax = @Fax
	,Phone1 = @Phone1
	,Phone2 = @Phone2
	,Contact = @Contact
	,City = @City
	,STATE = @State
	,ZipCode = @ZipCode
	,Web = @Web
	,TIN = @TIN
	,[TAN] = @TAN
	,AccountGroup = @AccGroup
	,GSTNo = @GSTNumber
	,OpeningBal = @OpeningBal
	,TransType = @TransType
	,AadharCard = @AadharNumber
	,ServiceTaxNumber = @ServiceTax
	,PAN = @PAN
	,ContactPersonName = @ContactPersonName
	,ContactPersonDesignation = @ContactPersonDesignation
	,ContactPersonOffPhone = @ContactPersonOffPhone
	,ContactPersonMobile = @ContactPersonMobile
	,DefaultRate = @DefaultRate
	 
WHERE LedgerId = @LedgerId
 
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Ledger_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Ledger_ins
 END
 GO
 
CREATE PROC [dbo].[p_Ledger_ins] (
	@Name NVARCHAR(200)
	,@Code NVARCHAR(20)
	,@Address1 NVARCHAR(100) = NULL
	,@Address2 NVARCHAR(100) = NULL
	,@Email NVARCHAR(200) = NULL
	,@Fax NVARCHAR(15) = NULL
	,@Phone1 NVARCHAR(15) = NULL
	,@Phone2 NVARCHAR(15) = NULL
	,@Contact NVARCHAR(50) = NULL
	,@City NVARCHAR(50) = NULL
	,@State NVARCHAR(50) = NULL
	,@ZipCode NVARCHAR(10) = NULL
	,@Web NVARCHAR(100) = NULL
	,@TIN NVARCHAR(20) = NULL
	,@TAN NVARCHAR(20) = NULL
	,@AccGroup TINYINT = NULL
	,@OpeningBal NUMERIC(10, 2) = 0
	,@TransType TINYINT = NULL
	,@GSTNumber NVARCHAR(20) = NULL
	,@AadharNumber NVARCHAR(20) = NULL
	,@PAN NVARCHAR(20) = NULL
	,@ServiceTax NVARCHAR(20) = NULL
	,@ContactPersonName NVARCHAR(50) = NULL
	,@ContactPersonDesignation NVARCHAR(50) = NULL
	,@ContactPersonOffPhone NVARCHAR(20) = NULL
	,@ContactPersonMobile NVARCHAR(20) = NULL
	,@RbnClientId INT = NULL
	,@DefaultRate NUMERIC(5, 2) = 0
	,@CompanyId INT
	,@FinYearId INT = NULL 
	)
AS

IF EXISTS(SELECT GSTNo from Ledger where GSTNo = @GSTNumber AND @GSTNumber IS NOT NULL  AND LEN(TRIM(@GSTNumber)) > 1
  AND  CompanyId = @CompanyId
) 
BEGIN

RAISERROR('Cannot Insert duplicate GST No',16,1)
return 0;
END
IF EXISTS(SELECT PAN from Ledger where PAN = @PAN AND @PAN IS NOT NULL AND LEN(TRIM(@pan)) > 1
 AND  CompanyId = @CompanyId
 ) 
BEGIN
RAISERROR('Cannot Insert duplicate PAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where Name = TRIM(@name) AND RbnClientId = @RbnClientId
   AND  CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate ledger',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where Code = TRIM(@Code) AND RbnClientId = @RbnClientId
  AND  CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate ledger code',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where TAN = TRIM(@TAN) AND @TAN IS NOT NULL AND LEN(TRIM(@TAN)) > 1
 AND  CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate TAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Ledger where TIN = TRIM(@TIN)   AND @TIN IS NOT NULL AND LEN(TRIM(@TIN)) > 1 
 AND  CompanyId = @CompanyId
) 
BEGIN
RAISERROR('Cannot Insert duplicate TIN',16,1)
return 0;
END

INSERT INTO Ledger (
	NAME
	,Code
	,Address1
	,Address2
	,Email
	,Fax
	,Phone1
	,Phone2
	,Contact
	,City
	,STATE
	,ZipCode
	,Web
	,TIN
	,[TAN]
	,AccountGroup
	,GSTNo
	,OpeningBal
	,TransType
	,AadharCard
	,ServiceTaxNumber
	,PAN
	,ContactPersonName
	,ContactPersonDesignation
	,ContactPersonOffPhone
	,ContactPersonMobile
	,RbnClientId
	,DefaultRate
	,CompanyId
	,FinYearId
	,IsActive
 
	)
VALUES (
	@Name
	,@Code
	,@Address1
	,@Address2
	,@Email
	,@Fax
	,@Phone1
	,@Phone2
	,@Contact
	,@City
	,@State
	,@ZipCode
	,@Web
	,@TIN
	,@TAN
	,@AccGroup
	,@GSTNumber
	,@OpeningBal
	,@TransType
	,@AadharNumber
	,@ServiceTax
	,@PAN
	,@ContactPersonName
	,@ContactPersonDesignation
	,@ContactPersonOffPhone
	,@ContactPersonMobile
	,@RbnClientId
	,@DefaultRate
	,@CompanyId
	,@FinYearId
	,1
 
	)

SELECT @@IDENTITY AS LedgerId

GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Product_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Product_upd
 END
 GO
 CREATE PROC [dbo].[p_Product_upd] (
	@Name NVARCHAR(50)
	,@Code NVARCHAR(50) = NULL
	,@Unit NVARCHAR(20) = NULL
	,@Packing NVARCHAR(20) = NULL
	,@Salt INT = NULL
	,@ProductType INT
	,@LocalTax NUMERIC(10, 3) = NULL
	,@VATRate NUMERIC(10, 3) = NULL
	,@CST BIT = NULL
	,@CSTRate NUMERIC(10, 3) = NULL
	,@ExiseRate NUMERIC(10, 3) = NULL
	,@PurchaseRate NUMERIC(10, 3) = NULL
	,@MRP NUMERIC(10, 3) = NULL
	,@PackingQty NUMERIC(10, 3) = NULL
	,@Category INT
	,@ProductId INT
	,@Description NVARCHAR(100) = NULL
	,@UOM INT
	,@Size NVARCHAR(20)
	,@companyId INT
	,@SortOrder INT = NULL
	)
AS
IF EXISTS(SELECT Name from Product where Name = TRIM(@name) AND CompanyId = @CompanyId
And ProductId <> @ProductId) 
BEGIN

RAISERROR('Cannot create duplicate product',16,1)
return 0;
END
IF EXISTS(SELECT Code from Product where Code = TRIM(@Code) AND CompanyId = @CompanyId
And ProductId <> @ProductId) 
BEGIN

RAISERROR('Cannot create duplicate product code',16,1)
return 0;
END

UPDATE Product
SET NAME = @Name
	,Code = @Code
	,UNIT = @Unit
	,Packing = @Packing
	,Salt = @Salt
	,Type = @ProductType
	,LocalTax = @LocalTax
	,VATRate = @VATRate
	,CSt = @CST
	,CSTRate = @CSTRate
	,ExiseRate = @ExiseRate
	,PurchaseRate = @PurchaseRate
	,Mrp = @MRP
	,PackingQty = @PackingQty
	,Category = @Category
	,Description = @Description
	,UOM = @UOM
	,Size = @Size
	,SortOrder = @SortOrder
WHERE ProductId = @ProductId

 
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Product_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Product_ins
 END
 GO
 CREATE PROC [dbo].[p_Product_ins] (
	@Name NVARCHAR(50)
	,@Code NVARCHAR(50)
	,@UOM INT
	,@Size NVARCHAR(20)
	,@CompanyId INT
	,@ProductType INT
	,@Category INT
	,@StoreId INT
	,@Description NVARCHAR(100) = NULL
	,@RbnClientId INT
	,@SortOrder INT = NULL
	)
AS

 
IF EXISTS(SELECT Name from Product where Name = TRIM(@name) AND CompanyId = @CompanyId) 
BEGIN

RAISERROR('Cannot create duplicate product',16,1)
return 0;
END
IF EXISTS(SELECT Code from Product where Code = TRIM(@Code) AND CompanyId = @CompanyId) 
BEGIN

RAISERROR('Cannot create duplicate product code',16,1)
return 0;
END
  

	INSERT INTO Product (
		NAME
		,Code
		,CompanyId
		,Type
		,Category
		,StoreID
		,Description
		,UOM
		,Size
		,RBnClientId
		,SORTOrder
		)
	VALUES (
		@Name
		,@Code
		,@CompanyId
		,@ProductType
		,@Category
		,@StoreId
		,@Description
		,@UOM
		,@Size
		,@RbnClientId
		,@SortOrder
		)

	SELECT @@IDENTITY
 
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Company_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Company_upd
 END
 GO
 
CREATE PROC [dbo].[p_Company_upd] (
	@Name NVARCHAR(200)
	,@Address1 NVARCHAR(100)
	,@Address2 NVARCHAR(100) = NULL
	,@Email NVARCHAR(200) = NULL
	,@Fax NVARCHAR(15) = NULL
	,@Phone1 NVARCHAR(15) = NULL
	,@Phone2 NVARCHAR(15) = NULL
	,@Contact NVARCHAR(50) = NULL
	,@City NVARCHAR(50) = NULL
	,@State NVARCHAR(50) = NULL
	,@ZipCode NVARCHAR(10) = NULL
	,@Web NVARCHAR(100) = NULL
	,@TIN NVARCHAR(20) = NULL
	,@TAN NVARCHAR(20) = NULL
	,@SignAuthority NVARCHAR(50) =''
	,@GSTNo NVARCHAR(20) = NULL
	,@CompanyId INT
	,@ReportHeader NVARCHAR(200) = NULL,
	@RbnClientId INT
	,@PAN nvarchar(20) = NULL
	)
AS

IF EXISTS(SELECT GSTNo from Company where GSTNo = @GSTNo AND @GSTNo IS NOT NULL  AND LEN(TRIM(@GSTNo)) > 1
AND CompanyId <> @CompanyId
) 
BEGIN

RAISERROR('Cannot Insert duplicate GST No',16,1)
return 0;
END
IF EXISTS(SELECT PAN from Company where PAN = @PAN AND @PAN IS NOT NULL AND LEN(TRIM(@pan)) > 1
AND CompanyId <> @CompanyId) 
BEGIN
RAISERROR('Cannot Insert duplicate PAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Company where Name = TRIM(@name) AND RbnClientId = @RbnClientId
AND CompanyId <> @CompanyId
) 
BEGIN

RAISERROR('Cannot Insert duplicate company name',16,1)
return 0;
END

UPDATE Company
SET NAME = @Name
	,Address1 = @Address1
	,Address2 = @Address2
	,Email = @Email
	,Fax = @Fax
	,Phone1 = @Phone1
	,Phone2 = @Phone2
	,Contact = @Contact
	,City = @City
	,STATE = @State
	,ZipCode = @ZipCode
	,Web = @Web
	,TIN = @TIN
	,[TAN] = @TAN
	,SignAuthority = @SignAuthority
	,GSTNo = @GSTNo
	,ReportHeader = @ReportHeader
	,PAN = @PAN
WHERE CompanyId = @CompanyId

 
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Company_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Company_ins
 END
 GO
  CREATE PROC [dbo].[p_Company_ins] (
	@Name NVARCHAR(200)
	,@Address1 NVARCHAR(100)
	,@Address2 NVARCHAR(100) = NULL
	,@Email NVARCHAR(200) = NULL
	,@Fax NVARCHAR(15) = NULL
	,@Phone1 NVARCHAR(15) = NULL
	,@Phone2 NVARCHAR(15) = NULL
	,@Contact NVARCHAR(50) = NULL
	,@City NVARCHAR(50) = NULL
	,@State NVARCHAR(50) = NULL
	,@ZipCode NVARCHAR(10) = NULL
	,@Web NVARCHAR(100) = NULL
	,@TIN NVARCHAR(20) = NULL
	,@TAN NVARCHAR(20) = NULL
	,@SignAuthority NVARCHAR(50) = NULL
	,@GSTNo NVARCHAR(20) = NULL
	,@RbnClientId INT
	,@ReportHeader NVARCHAR(200) = NULL
	,@PAN nvarchar(20) =''
	)
AS

IF EXISTS(SELECT GSTNo from Company where GSTNo = @GSTNo AND @GSTNo IS NOT NULL  AND LEN(TRIM(@GSTNo)) > 1) 
BEGIN

RAISERROR('Cannot Insert duplicate GST No',16,1)
return 0;
END
IF EXISTS(SELECT PAN from Company where PAN = @PAN AND @PAN IS NOT NULL AND LEN(TRIM(@pan)) > 1) 
BEGIN
RAISERROR('Cannot Insert duplicate PAN',16,1)
return 0;
END
IF EXISTS(SELECT Name from Company where Name = TRIM(@name) AND RbnClientId = @RbnClientId) 
BEGIN

RAISERROR('Cannot Insert duplicate company name',16,1)
return 0;
END
 


INSERT INTO Company (
	NAME
	,Address1
	,Address2
	,Email
	,Fax
	,Phone1
	,Phone2
	,Contact
	,City
	,STATE
	,ZipCode
	,Web
	,TIN
	,[TAN]
	,SignAuthority
	,RbnClientId
	,GSTNo
	,ReportHeader
	,PAN
	)
VALUES (
	@Name
	,@Address1
	,@Address2
	,@Email
	,@Fax
	,@Phone1
	,@Phone2
	,@Contact
	,@City
	,@State
	,@ZipCode
	,@Web
	,@TIN
	,@TAN
	,@SignAuthority
	,@RbnClientId
	,@GSTNo
	,@ReportHeader
	,@PAN
	)

	select @@IDENTITY

	 
	  
	   
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_clientPackage_sel'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_clientPackage_sel
 END
 GO
 CREATE proc [dbo].[p_clientPackage_sel]
(
@rbnClientId int
)
AS
 select Top 1 *  from clientpackage where rbnClientId = @rbnClientId
  
 ORDER BY validTill DESC
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_payments_ByOrderId'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_payments_ByOrderId
 END
 GO
CREATE proc [dbo].[p_payments_ByOrderId](
@status nvarchar(10),
@orderId  nvarchar(20)=null,
@paymentdate smalLDateTime=null,
@error nvarchar(300)=null,
@payment_id nvarchar(30)=null,
@payment_signature nvarchar(max)=null
)
AS

Update payments
SET status=@status,   error=@error,paymentDate=@paymentdate 
,payment_id=@payment_id,payment_signature=@payment_signature
where orderId= @orderId

 Select *from Payments where orderId = @orderId
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_packageById'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE p_packageById
 END
 GO
 
 Create proc [dbo].[p_packageById]
 (
 @packageId int
 )
 AS

 Select *from LookupPackage where PackageId = @packageId
GO

/****** Object:  StoredProcedure [dbo].[p_payments_ins]    Script Date: 21-10-2024 13:16:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_payments_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE p_payments_ins
 END
 GO
CREATE proc [dbo].[p_payments_ins](
@amount numeric(10,2),
@creationDate smallDatetime,
@status nvarchar(10)='New',
@uniqueId nvarchar(10),
@clientId int,
@packageId int,
@error nvarchar(300)=null
)
AS

insert into payments(amount,created_date,status,uniqueId,clientId,error,packageId)
Values(@amount,@creationDate,@status,@uniqueId,@clientId,@error,@packageId)

 
 
GO