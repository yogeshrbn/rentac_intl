 USE [$AppDb$]
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Vehicle_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Vehicle_upd
 END
 GO
 CREATE proc [dbo].[p_Vehicle_upd]
(
@Name nvarchar(50)
,@RegNumber nvarchar(30)
,@ChachisNumber nvarchar(30) = NULL
,@EngineNumber nvarchar(30) = NULL
,@VehicleId INT
,@CompanyId INT
)
AS
IF (EXISTS(SELECT RegNumber FROM Vehicle Where RegNumber = @RegNumber AND CompanyId = @CompanyId
AND @RegNumber IS NOT NULL AND LEN(@RegNumber) >1 AND VehicleId != @VehicleId
))
BEGIN
 RAISERROR('A vehicle with same registration number already exists',16,1);
 return 0
END
IF (EXISTS(SELECT RegNumber FROM Vehicle Where ChachisNumber = @ChachisNumber AND CompanyId = @CompanyId
AND @ChachisNumber IS NOT NULL AND LEN(@ChachisNumber) >1  AND VehicleId != @VehicleId
))
BEGIN
 RAISERROR('A vehicle with same chachis number already exists',16,1);
 return 0
END
IF (EXISTS(SELECT RegNumber FROM Vehicle Where EngineNumber = @EngineNumber AND CompanyId = @CompanyId
AND @EngineNumber IS NOT NULL AND LEN(@EngineNumber) >1 AND VehicleId != @VehicleId
))
BEGIN
 RAISERROR('A vehicle with same EngineNumber  already exists',16,1);
 return 0
END
UPDATE Vehicle
SET RegNumber = @RegNumber,ChachisNumber = @ChachisNumber, EngineNumber =  @EngineNumber
,Name = @Name  
WHERE VehicleId = @VehicleId
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Vehicle_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Vehicle_ins
 END
 GO
 CREATE proc [dbo].[p_Vehicle_ins]
(
@Name nvarchar(50)
,@RegNumber nvarchar(30)
,@ChachisNumber nvarchar(30) = NULL
,@EngineNumber nvarchar(30) = NULL
,@CompanyId INT
)
AS
IF (EXISTS(SELECT RegNumber FROM Vehicle Where RegNumber = @RegNumber AND CompanyId = @CompanyId
AND @RegNumber IS NOT NULL AND LEN(@RegNumber) >1
))
BEGIN
 RAISERROR('A vehicle with same registration number already exists',16,1);
 return 0
END
IF (EXISTS(SELECT RegNumber FROM Vehicle Where ChachisNumber = @ChachisNumber AND CompanyId = @CompanyId
AND @ChachisNumber IS NOT NULL AND LEN(@ChachisNumber) >1
))
BEGIN
 RAISERROR('A vehicle with same chachis number already exists',16,1);
 return 0
END
IF (EXISTS(SELECT RegNumber FROM Vehicle Where EngineNumber = @EngineNumber AND CompanyId = @CompanyId
AND @EngineNumber IS NOT NULL AND LEN(@EngineNumber) >1
))
BEGIN
 RAISERROR('A vehicle with same EngineNumber  already exists',16,1);
 return 0
END
INSERT INTO Vehicle(Name,RegNumber,ChachisNumber,EngineNumber,CompanyId)
Values(@Name,@RegNumber,@ChachisNumber,@EngineNumber,@CompanyId)
 
 SELECT @@IDENTITY As VehicleId
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_employee_sellAll'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_employee_sellAll
 END
 GO
 CREATE proc [dbo].[p_employee_sellAll](
 
@CompanyId int
 
)
AS

Select T1.*,T2.Name As RoleName,T3.Name As StatusName from Employee T1
INNER JOIN LookupRoles T2 ON T1.RoleId = T2.RoleId
INNER JOIN LookupEmployeeStatus T3 ON T1.StatusId = T3.EmployeeStatusId
 WHERE CompanyId = @CompanyId
 ORDER By T1.Name
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_Vehicle_sellAll'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_Vehicle_sellAll
 END
 GO
 
CREATE proc [dbo].[p_Vehicle_sellAll]
(
 
@CompanyId INT
)
AS
SELECT * FROM Vehicle WHERE CompanyId = @CompanyId
ORDER BY Name
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_employee_ins'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_employee_ins
 END
 GO
 CREATE proc [dbo].[p_employee_ins](
@Name nvarchar(50)
,@EmployeeCode nvarchar(10)
,@Address nvarchar(100)
,@Aadhar nvarchar(20)
,@Phone nvarchar(25)
,@CompanyId int
,@RoleId TinyINt
)
AS
IF (EXISTS(SELECT NAME FROM Employee Where Aadhar = @Aadhar AND CompanyId = @CompanyId
AND @Aadhar IS NOT NULL AND LEN(@Aadhar) >1
))
BEGIN
 RAISERROR('An employee with same aadhaar already exists',16,1);
 return 0
END
 
IF (EXISTS(SELECT NAME FROM Employee Where EmployeeCode = @EmployeeCode AND CompanyId = @CompanyId
AND @EmployeeCode IS NOT NULL AND LEN(@EmployeeCode) >1  
))
BEGIN
 RAISERROR('An employee with same code already exists',16,1);
 return 0
END
 
INSERT INTO Employee(Name,EmployeeCode,Address,Aadhar,Phone,CompanyId,CreatedON,RoleId)
Values(@Name,@EmployeeCode,@Address,@Aadhar,@Phone,@CompanyId,GetDate(),@RoleId)
	Select @@IDENTITY As EmployeeId
 

 
GO
  IF EXISTS (
        SELECT type_desc, type
        FROM sys.procedures WITH(NOLOCK)
        WHERE NAME = 'p_employee_upd'
            AND type = 'P'
      )
 BEGIN
 DROP PROCEDURE dbo.p_employee_upd
 END
 GO
 CREATE proc [dbo].[p_employee_upd](
@Name nvarchar(50)
,@EmployeeCode nvarchar(10)
,@Address nvarchar(100)
,@Aadhar nvarchar(20)
,@Phone nvarchar(25)
,@CompanyId int
,@RoleId TinyINt
,@EmployeeId INT
)
AS
IF (EXISTS(SELECT NAME FROM Employee Where Aadhar = @Aadhar AND CompanyId = @CompanyId
AND @Aadhar IS NOT NULL AND LEN(@Aadhar) >1 AND EmployeeId != @EmployeeId
))
BEGIN
 RAISERROR('An employee with same aadhaar already exists',16,1);
 return 0
END
IF (EXISTS(SELECT NAME FROM Employee Where EmployeeCode = @EmployeeCode AND CompanyId = @CompanyId
AND @EmployeeCode IS NOT NULL AND LEN(@EmployeeCode) >1 AND EmployeeId != @EmployeeId
))
BEGIN
 RAISERROR('An employee with same code already exists',16,1);
 return 0
END
 
UPDATE Employee
SET Name = @Name,EmployeeCode= @EmployeeCode,Address = @Address,Aadhar = @Aadhar
,RoleId = @RoleId WHERE EmployeeId = @EmployeeId
 
  SELECT @@IDENTITY
GO