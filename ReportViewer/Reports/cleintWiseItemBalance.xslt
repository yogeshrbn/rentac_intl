<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <!-- Group by Client (LedgerId) and Site (LedgerSiteId) -->
  <xsl:key name="client-site-group" match="data/details/Table" use="concat(LedgerId, '|', LedgerSiteId)" />
 
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          .td-heading {
          line-height:20px;
          padding:0px;
          }
          .headerUL li  {
          padding-bottom:8px;
          padding-top:0px;
          margin:0px;
          }
          td,th {border:none;padding:2px;}
          .footerrow td {
          border-top:solid 2px;
          }
          .client-site-header {
          background-color:#f3f3f3;
          font-weight:bold;
          padding:8px 5px;
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
     
      <body>
        <div id="printArea" style="padding:10px;">

          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="8">
                <ul class="headerUL" style="list-style:none;padding:none;">
                  <li>
                    <h2 style="color:green;padding:0px;;margin:0px;">
                      <xsl:value-of select="data/comp/Name"/>
                    </h2>
                  </li>
                  <li  >
                    <xsl:value-of select="data/comp/Address1"/>
                    <xsl:if test="data/comp/Address2 != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/Address2"/>
                    </xsl:if>

                    <xsl:if test="data/comp/City != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/City"/>
                    </xsl:if>
                    <xsl:if test="data/comp/State != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone No.: </b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="data/comp/Phone1"/>
                  </li>
                </ul>


              </td>
            </tr>
            <tr>
              <td colspan="8" class="noborder text-center">
                <strong>Item Balance On Party</strong>
                <xsl:text> - Date: </xsl:text>
                <xsl:value-of select="util:DateToDDMMYYYY(data/toDate)"/>
                <hr style="border-color:#000" />
              </td>
            </tr>
            <!-- Group by Client and Site -->
            <xsl:for-each select="data/details/Table[generate-id() = generate-id(key('client-site-group', concat(LedgerId, '|', LedgerSiteId))[1])]">
              <xsl:sort select="ClientName"/>
              <xsl:sort select="SiteAddress"/>
              <xsl:variable name="current-group" select="key('client-site-group', concat(LedgerId, '|', LedgerSiteId))" />
              <tr>
                <td colspan="8" class="noborder" style="vertical-align:top;">
                  <!-- Client and Site header -->
                  <div class="client-site-header" style="margin-top:15px;margin-bottom:5px;">
                    <strong>Party</strong>: <xsl:value-of select="$current-group[1]/ClientName"/>
                    <xsl:if test="$current-group[1]/SiteAddress != ''">
                      <xsl:text> | </xsl:text>
                      <strong>Site</strong>: <xsl:value-of select="$current-group[1]/SiteAddress"/>
                    </xsl:if>
                  </div>
                  <table style="width:100%;border-collapse:collapse;">
                    <tr>
                      <th style="width:80px;" class="text-center">S.No</th>
                      <th colspan="2">Item</th>
                      <th class="text-center">Sent Qty</th>
                      <th class="text-center">Recvd Qty</th>
                      <th class="text-center">Balance</th>
                      <th class="text-center">Excess Qty</th>
                      <th class="text-center">Short Qty</th>
                    </tr>
                    <xsl:for-each select="$current-group">
                      <tr>
                        <td class="text-center">
                          <xsl:value-of select="position()" />
                        </td>
                        <td colspan="2">
                          <xsl:value-of select="Product"/>
                        </td>
                        <td class="text-center">
                          <xsl:value-of select="IssuedQty"/>
                        </td>
                        <td class="text-center">
                          <xsl:value-of select="ReceivedQty"/>
                        </td>
                        <td class="text-center">
                          <xsl:value-of select="ClosingBalance"/>
                        </td>
                        <td class="text-center">
                          <xsl:value-of select="ExcessQty"/>
                        </td>
                        <td class="text-center">
                          <xsl:value-of select="ShortQty"/>
                        </td>
                      </tr>
                    </xsl:for-each>
                    <tr class="footerrow">
                      <td colspan="2" style="text-align:right;">
                        <strong>Total</strong>
                      </td>
                      <td></td>
                      <td class="text-center">
                        <strong><xsl:value-of select="sum($current-group/IssuedQty)"/></strong>
                      </td>
                      <td class="text-center">
                        <strong><xsl:value-of select="sum($current-group/ReceivedQty)"/></strong>
                      </td>
                      <td class="text-center">
                        <strong><xsl:value-of select="sum($current-group/ClosingBalance)"/></strong>
                      </td>
                      <td class="text-center">
                        <strong><xsl:value-of select="sum($current-group/ExcessQty)"/></strong>
                      </td>
                      <td class="text-center">
                        <strong><xsl:value-of select="sum($current-group/ShortQty)"/></strong>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </xsl:for-each>
            <tr>
              <td colspan="8" style="line-height:40px;" class="noborder"></td>
            </tr>           
          </table>
        

        </div>

      </body>
    </html>



  </xsl:template>
</xsl:transform>
