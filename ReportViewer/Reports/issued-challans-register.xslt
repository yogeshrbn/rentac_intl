<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          .headerUL li {
          padding-bottom:8px;
          padding-top:0px;
          margin:0px;
          }
          td, th { border:none; padding:4px 2px; }
          th { border-bottom:solid 1px #000; }
          .footerrow td { border-top:solid 2px; }
          .deleted-row, .deleted-row td { color:red; }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:10px;">
          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="5">
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
              <td colspan="5" class="noborder text-center">
                <strong>
                  <xsl:value-of select="data/reportTitle"/>
                </strong>
                <xsl:if test="data/challanTypeLabel != ''">
                  <xsl:text> (</xsl:text>
                  <xsl:value-of select="data/challanTypeLabel"/>
                  <xsl:text>)</xsl:text>
                </xsl:if>
                <br />
                <xsl:text>From </xsl:text>
                <xsl:value-of select="util:DateToDDMMYYYY(data/fromDate)"/>
                <xsl:text> To </xsl:text>
                <xsl:value-of select="util:DateToDDMMYYYY(data/toDate)"/>
                <hr style="border-color:#000" />
              </td>
            </tr>
            <tr>
              <th>Party</th>
              <th>Site</th>
              <th>Voucher#</th>
              <th class="text-center">Date</th>
              <th class="text-center">Qty</th>
            </tr>
            <xsl:for-each select="data/challan">
              <xsl:sort select="SentDate"/>
              <xsl:sort select="ChallanNumber"/>
              <tr>
                <xsl:attribute name="class">
                  <xsl:if test="Deleted = 1 or Deleted = '1'">deleted-row</xsl:if>
                </xsl:attribute>
                <td><xsl:value-of select="Client"/></td>
                <td><xsl:value-of select="Site"/></td>
                <td><xsl:value-of select="ChallanNumber"/></td>
                <td class="text-center">
                  <xsl:value-of select="util:DateToDDMMYYYY(SentDate)"/>
                </td>
                <td class="text-center">
                  <xsl:value-of select="SentQty"/>
                </td>
              </tr>
            </xsl:for-each>
            <tr class="footerrow">
              <td colspan="4" style="text-align:right;">
                <strong>Total Qty</strong>
              </td>
              <td class="text-center">
                <strong>
                  <xsl:value-of select="sum(data/challan/SentQty)"/>
                </strong>
              </td>
            </tr>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
