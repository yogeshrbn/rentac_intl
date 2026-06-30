<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <style>
          body { font-family: Arial, sans-serif; font-size: 12px; }
          table.data { width: 100%; border-collapse: collapse; margin-top: 12px; }
          table.data th, table.data td { border: 1px solid #333; padding: 4px 6px; }
          table.data th { background: #f0f0f0; text-align: left; }
          td.num { text-align: right; }
          .title { font-size: 16px; font-weight: bold; text-align: center; margin-bottom: 8px; }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:10px;">
          <div class="title">
            <xsl:value-of select="d/title"/>
          </div>
          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td colspan="2" class="text-center" style="font-size:14px;font-weight:bold;">
                <xsl:value-of select="d/companyName"/>
              </td>
            </tr>
            <tr>
              <td style="width:50%;">
                <strong>Period:</strong>
                <xsl:text> </xsl:text>
                <xsl:value-of select="d/from"/>
                <xsl:text> — </xsl:text>
                <xsl:value-of select="d/to"/>
              </td>
              <td style="width:50%;text-align:right;">
                <strong>Printed:</strong>
                <xsl:text> </xsl:text>
                <xsl:value-of select="d/printDate"/>
              </td>
            </tr>
          </table>

          <table class="data">
            <tr>
              <th>Party</th>
              <th>Site</th>
              <th>Challan#</th>
              <th>Challan date</th>
              <th style="text-align:right;">
                <xsl:value-of select="d/quantityLabel"/>
              </th>
            </tr>
            <xsl:for-each select="d/rows/row">
              <tr>
                <td><xsl:value-of select="Party"/></td>
                <td><xsl:value-of select="Site"/></td>
                <td><xsl:value-of select="Challan"/></td>
                <td><xsl:value-of select="ChallanDate"/></td>
                <td class="num"><xsl:value-of select="Quantity"/></td>
              </tr>
            </xsl:for-each>
          </table>
          <table style="width:100%;margin-top:10px;">
            <tr>
              <td style="text-align:right;font-weight:bold;">
                Total:
                <xsl:text> </xsl:text>
                <xsl:value-of select="d/totalQuantity"/>
              </td>
            </tr>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
