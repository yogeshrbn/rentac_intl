-- Add optional PO-based challan filter inputs to bill generation SP (genBill screen).
USE [$AppDb$]
GO

-- Update stored procedure in DB (run in SSMS against your DB):
--
-- p_GeneratBillV8:
--   1) Add optional parameters
--      @filterChallansByPO BIT = 0,
--      @poNumber NVARCHAR(100) = NULL
--
--   2) Apply PO condition only when @filterChallansByPO = 1
--      Example pattern in challan/billable item source query:
--      AND (
--            ISNULL(@filterChallansByPO, 0) = 0
--            OR ISNULL(LTRIM(RTRIM(@poNumber)), '') = ''
--            OR ISNULL(LTRIM(RTRIM(src.PONumber)), '') = LTRIM(RTRIM(@poNumber))
--          )
--
--   3) Keep existing behavior unchanged when @filterChallansByPO = 0.
--
-- BAL/DAL/BillingDAL.cs now passes:
--   @filterChallansByPO and @poNumber to p_GeneratBillV8.
