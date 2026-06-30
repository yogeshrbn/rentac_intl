-- Preview next issue challan number via fn_getNextChallanNumberV2 (read-only path for UI).
-- Parameters match BAL/DAL/WorkorderDAL.GetNextChallanNumberPreview (same as GetLastChallanNumber).
-- If fn parameter order differs in your database, adjust the SELECT list accordingly.
USE [$AppDb$]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.p_previewNextChallanNumberV2
    @finYearId INT,
    @type INT,
    @companyId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @number nvarchar(30) =''
    SELECT @number  = dbo.fn_getNextChallanNumberV2(@companyId, @finYearId,@type,'');

    if @number IS NOT NULL AND LEN(@number) > 0
    BEGIN
        SELECT REPLACE(@number,',','');

    END
   ELSE 
   BEGIN
    SELECT @number
    END
END
GO

 