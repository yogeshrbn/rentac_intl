USE [$AppDb$]
GO

-- Additional quantity captured in purchase entry details.
IF COL_LENGTH('dbo.PurchaseItems', 'Unit1Quantity') IS NULL
BEGIN
    ALTER TABLE dbo.PurchaseItems
    ADD Unit1Quantity DECIMAL(18,3) NULL;
END
GO

/*
    Existing stored procedures to be updated in DB deployment layer:
    1) p_PurchaseItems_ins           -> add @Unit1Quantity DECIMAL(18,3) and persist to PurchaseItems.Unit1Quantity
    2) p_getPurchaseItems            -> include Unit1Quantity in SELECT list
    3) p_getPurchaseItems_byPurchaseId -> include Unit1Quantity in SELECT list
*/
