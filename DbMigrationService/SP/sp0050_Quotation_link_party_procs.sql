-- Link a saved quotation to a newly created ledger party (registered).
-- Run after dbo.Quotation exists with CompanyId, LedgerId, PartyType, UnregisteredParty* columns.
USE [$AppDb$]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.p_Quotation_linkParty
    @quotationId INT,
    @ledgerId INT,
    @companyId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Quotation
    SET LedgerId = @ledgerId,
        PartyType = 1,
        UnregisteredPartyName = NULL,
        UnregisteredPartyAddress = NULL,
        UnregisteredPartyPhone = NULL
    WHERE QuotationId = @quotationId
      AND CompanyId = @companyId;
END
GO
