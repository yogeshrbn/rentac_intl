 USE [$AppDb$]
GO 
IF COL_LENGTH('GRN', 'RentStopDate') IS NULL
BEGIN

 Alter table GRN
 add RentStopDate dateTime
END
GO 
 