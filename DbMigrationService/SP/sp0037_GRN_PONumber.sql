-- Add PO Number column to GRN for Receive Items (inv_grn)
USE [$AppDb$]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GRN') AND name = 'PONumber')
BEGIN
    ALTER TABLE GRN ADD PONumber NVARCHAR(50) NULL;
END
GO

-- Update stored procedures (run in SSMS against your DB):
--
-- p_GRN_insV1: Add parameter @PONumber NVARCHAR(50) = NULL
--   and include PONumber in INSERT column list and VALUES.
--
-- p_GRN_upd: Add parameter @PONumber NVARCHAR(50) = NULL
--   and add PONumber = @PONumber in UPDATE SET clause.
--
-- p_grn_byId: Include PONumber in SELECT list so edit loads it.
--
-- BAL/DAL/GRNDAL.cs passes @PONumber for both insert and update.
