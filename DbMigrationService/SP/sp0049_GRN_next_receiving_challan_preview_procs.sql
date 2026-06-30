-- Preview next receiving (return) challan number via fn_getNextRecevingChallanNumberV2 (read-only for UI).
-- Parameters match BAL/DAL/GRNDAL.GetNextReceivingChallanNumberPreview.
-- If the function uses a different parameter order than (@finYearId, @type, @companyId), adjust the SELECT.
USE [$AppDb$]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.p_previewNextReceivingChallanNumberV2
    @finYearId INT,
    @type INT,
    @companyId INT
AS
BEGIN
   SET NOCOUNT ON;
     DECLARE @number nvarchar(30) =''
    SELECT  @number  =  dbo.fn_getNextRecevingChallanNumberV2(@companyId,@finYearId,@type,'');
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
