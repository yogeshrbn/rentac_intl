/*
    Add BillingToSiteId support for client sites.

    NOTE:
    - This repository references p_ClientSites_ins, p_ClientSites_upd and p_ClientSiteById_sel
      in DAL code, but full stored procedure bodies are not available here.
    - Apply the procedure changes below in database deployment scripts where those
      procedures are defined.
*/

IF COL_LENGTH('dbo.ClientSites', 'BillingToSiteId') IS NULL
BEGIN
    ALTER TABLE dbo.ClientSites
    ADD BillingToSiteId INT NULL;
END
GO

/*
Required stored procedure updates:

1) p_ClientSites_ins
   - Add input parameter:
       @f INT = NULL
   - Persist value into dbo.ClientSites.BillingToSiteId

2) p_ClientSites_upd
   - Add input parameter:
       @billingToSiteId INT = NULL
   - Update dbo.ClientSites.BillingToSiteId

3) p_ClientSiteById_sel
   - Include BillingToSiteId in SELECT projection so edit form can prefill value.
*/
