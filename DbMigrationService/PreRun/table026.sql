-- E-way bill: optional ApproximateValue (declared total for portal / print; see BAL EwayBillDTO / BillingDAL).
-- Also update p_EwayBill_insv2, p_EwayBill_upd, p_EwayBill_ById, p_printEwayBill for @approximateValue as needed.
USE [$AppDb$]
GO

IF COL_LENGTH('EwayBill', 'ApproximateValue') IS NULL
BEGIN
    ALTER TABLE EwayBill ADD ApproximateValue DECIMAL(18,2) NULL;
END
GO
