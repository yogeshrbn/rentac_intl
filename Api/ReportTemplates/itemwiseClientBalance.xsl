<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <!-- Define keys: group by Product, then by Client+Site within each product -->
  <xsl:key name="keyProduct" match="Ledger" use="Product" />
  <xsl:key name="keyClientSite" match="Ledger" use="concat(Product, '|', LedgerId, '|', LedgerSiteId)" />
 
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1" crossorigin="anonymous"></link>
        <style type="text/css">
          .header {
          position: fixed;
          left: 20px;
          top: 20px;
          }

          .footer{
          position: fixed;
          left: 20px;
          top: 780px;
          }
          body,td,th {
          font-family: Verdana;
          font-size: 12px;

          }
          table
          {

          page-break-inside: avoid;
          }
          table, tr, td
          {
          page-break-inside: avoid;
          }
          td
          {  color: black;
          font-family: Arial;

          border:1px solid #000000;


          }
          .DarkBack
          {  background-color: #f3f3f3;

          color: white;
          font-weight: bold;
          }
          .LightBack
          {
          background-color: #f3f3f3;
          color: black;
          line-height:25px;
          font-size:16px;
          font-weight:500;
          }
          .RightJustified
          {  text-align: right;
          }
          .dataRow {line-height:20px;

          }
          .dataRow td {padding-left:5px;   font-size: x-small;}
          .client-site-header {
          background-color: #e8e8e8;
          font-weight: bold;
          padding: 5px;
          }
          .headerUL li {
          padding-bottom: 8px;
          padding-top: 0;
          margin: 0;
          }
          .noborder {
          border: none !important;
          }
        </style>

      </head>

      <body>
        <div id="printArea" style="padding:10px;">
          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="8" style="border:none;padding:2px;">
                <ul class="headerUL" style="list-style:none;padding:0;margin:0;">
                  <li>
                    <h2 style="color:green;padding:0;margin:0;">
                      <xsl:value-of select="/*/comp/Name"/>
                    </h2>
                  </li>
                  <li>
                    <xsl:value-of select="/*/comp/Address1"/>
                    <xsl:if test="/*/comp/Address2 != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="/*/comp/Address2"/>
                    </xsl:if>
                    <xsl:if test="/*/comp/City != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="/*/comp/City"/>
                    </xsl:if>
                    <xsl:if test="/*/comp/State != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="/*/comp/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone No.: </b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="/*/comp/Phone1"/>
                  </li>
                </ul>
              </td>
            </tr>
            <tr>
              <td colspan="8" class="noborder text-center" style="border:none;padding:8px 2px;">
                <strong>Item Wise Client Balance</strong>
                <xsl:text> - Date: </xsl:text>
                <xsl:value-of select="/*/toDate"/>
                <hr style="border-color:#000" />
              </td>
            </tr>
          </table>

        <table class="table table-responsive" style="width:100%;border-collapse:collapse;margin-top:10px;" >

          <xsl:for-each select="//Ledger[generate-id(.) = generate-id(key('keyProduct', Product)[1])]">
            <xsl:sort select="Product"/>
            <xsl:variable name="lngProduct" select="Product" />
            <xsl:variable name="lstClients" select="key('keyProduct', $lngProduct)" />
            <xsl:call-template name="ShowLedgerInProducts">
              <xsl:with-param name="lstClients" select="$lstClients" />
              <xsl:with-param name="productName" select="$lngProduct" />
            </xsl:call-template>
          </xsl:for-each>

        </table>

        </div>
      </body>
    </html>
  </xsl:template>


  <xsl:template name="ShowLedgerInProducts">
    <xsl:param name="lstClients" />
    <xsl:param name="productName" />

    <!-- Product header -->
    <tr class="LightBack">
      <td colspan="5" style="padding-left:5px;background-color:#f3f3f3;">
        <xsl:value-of select="$productName" />
      </td>
    </tr>

    <!-- Within each product, group by Client and Site -->
    <xsl:for-each select="$lstClients[generate-id(.) = generate-id(key('keyClientSite', concat(Product, '|', LedgerId, '|', LedgerSiteId))[1])]">
      <xsl:sort select="ClientName"/>
      <xsl:sort select="SiteAddress"/>
      <xsl:variable name="clientSiteGroup" select="key('keyClientSite', concat(Product, '|', LedgerId, '|', LedgerSiteId))" />
      <!-- Client and Site header -->
      <tr class="client-site-header">
        <td colspan="5" style="padding-left:10px;">
          <strong>Party</strong>: <xsl:value-of select="$clientSiteGroup[1]/ClientName"/>
          <xsl:if test="$clientSiteGroup[1]/SiteAddress != ''">
            <xsl:text> | </xsl:text>
            <strong>Site</strong>: <xsl:value-of select="$clientSiteGroup[1]/SiteAddress"/>
          </xsl:if>
        </td>
      </tr>
      <!-- Rows for this client-site (typically one row per product, but structure supports multiple) -->
      <xsl:for-each select="$clientSiteGroup">
        <tr class="dataRow">
          <td cellspacing="0">
            <xsl:value-of select="ClientName" />
          </td>
          <td cellspacing="0">
            <xsl:value-of select="OpeningBalance" />
          </td>
          <td>
            <xsl:value-of select="IssuedQty" />
          </td>
          <td>
            <xsl:value-of select="ReceivedQty" />
          </td>
          <td>
            <xsl:value-of select="ClosingBalance" />
          </td>
        </tr>
      </xsl:for-each>
    </xsl:for-each>


  </xsl:template>


</xsl:transform>
