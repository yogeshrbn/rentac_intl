<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <!-- Group by Party (LedgerId), then by Site (LedgerSiteId) within each party -->
  <xsl:key name="keyParty" match="data/details/Table" use="LedgerId" />
  <xsl:key name="keyPartySite" match="data/details/Table" use="concat(LedgerId, '|', LedgerSiteId)" />

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
          .product-header {
          background-color:#e8e8e8;
          font-weight:bold;
          padding:8px 5px;
          font-size:14px;
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>

      <body>
        <div id="printArea" style="padding:10px;">

          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="6">
                <ul class="headerUL" style="list-style:none;padding:none;">
                  <li>
                    <h2 style="color:green;padding:0px;margin:0px;">
                      <xsl:value-of select="data/comp/Name"/>
                    </h2>
                  </li>
                  <li>
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
              <td colspan="6" class="noborder text-center">
                <strong>Item Wise Client Balance</strong>
                <xsl:text> - Date: </xsl:text>
                <xsl:value-of select="util:DateToDDMMYYYY(data/toDate)"/>
                <br/>
                <span style="font-weight:bold;"> 
                  Product: <xsl:value-of select="data/details/Table/Product"/>
                </span>
                <hr style="border-color:#000" />
              </td>
            </tr>
            <tr>
              <th style="width:80px;" class="text-center">S.No</th>
              <!--<th colspan="2">Item</th>-->
              <!--<th class="text-center">Sent Qty</th>-->
              <th colspan="">Party</th>
              <th class="text-right">Sent</th>
              <th class="text-right">Recvd</th>
              <th class="text-right">Excess</th>
              <th class="text-right">Bal</th>

              <!--<th class="text-center">Short</th>-->
            </tr>
            <!-- Group by Party (Client), then by Site within each party -->
            <xsl:for-each select="data/details/Table">
              <xsl:sort select="ClientName"/>
              <xsl:variable name="partyId" select="LedgerId" />
              <xsl:variable name="partyName" select="ClientName" />
              <tr>
                <td class="text-center">
                  <xsl:value-of select="position()"/>
                </td>
                <td >
                  <xsl:value-of select="ClientName"/> [ <xsl:value-of select="siteAddress"/> ]
                </td>
                <td class="text-right">
                  <xsl:value-of select="IssuedQty"/>
                </td>
                <td class="text-right">
                  <xsl:value-of select="ReceivedQty"/>
                </td>
               
                <td class="text-right">
                  <xsl:value-of select="ExcessQty"/>
                </td>
                <td class="text-right">
                  <xsl:value-of select="ClosingBalance"/>
                </td>
                <!--<td class="text-center">
                  <xsl:value-of select="ShortQty"/>
                </td>-->
              </tr>


            </xsl:for-each>
            <!--<tr>
              <td colspan="5" style="line-height:40px;" class="noborder"></td>
            </tr>-->
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
