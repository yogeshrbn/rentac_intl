<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
      </head>
      <body>
        <div id="printArea">
          <table style="width: 100%" cellpadding="0" cellspacing="0">
            <tr>
              <td style="border:0px;">
                <img style="height:60px;max-width:150px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="data/Company/Logo"/>
                  </xsl:attribute>
                </img>
                <br/><br/>
                <xsl:value-of select="data/Company/Name" /><br />
                <xsl:value-of select="data/Company/Address1" />  <xsl:value-of select="data/Company/Address2" /><br />
                <xsl:value-of select="data/Company/City" />, PIN: -   <xsl:value-of select="data/Company/ZipCode" /><br />
                <xsl:value-of select="data/Company/State" /><br/>
                GSTIN:   <xsl:value-of select="data/Company/GSTNo" /><br />
              </td>
              <td style=" text-align:right;vertical-align:top" class="noborder">
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">MATERIAL ADJUSTMENT</p>
                Voucher #: <xsl:value-of select="data/Number" /><br />
              </td>
            </tr>
          </table>
          <table style="width:100%;border-collapse:collapse;margin-top:30px;">
            <tr>
              <td style="border: 0px;">
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Party</p>
                <xsl:value-of select="data/Ledger/Name" /><br />
                <xsl:value-of select="data/Ledger/Address1" />  <xsl:value-of select="data/Ledger/Address2" /><br />
                <xsl:value-of select="data/Ledger/City" />, PIN: -   <xsl:value-of select="data/Ledger/ZipCode" /><br />
                <xsl:value-of select="data/Ledger/State" /><br/>
                GSTIN: <xsl:value-of select="data/Ledger/GSTNo" /><br />
                Site: <xsl:value-of select="data/SiteAddress" /><br/>
              </td>
              <td style="vertical-align: top; border: 0px;text-align:right">
                Date: <xsl:value-of select="data/WorkOrderDate"/><br />
              </td>
            </tr>
          </table>

          <p style="margin-top:18px;margin-bottom:6px;font-weight:bold;">1. Items issued</p>
          <table style="width:100%; border-collapse:collapse;">
            <xsl:if test="count(data/IssueItems) > 0">
            <tr>
              <th style="width:55%;">Product</th>
              <!--<th class="text-right" style="width:15%;">Excess Qty</th>-->
              <th class="text-right" style="width:15%;">Qty</th>
            </tr>
            </xsl:if>
            <xsl:for-each select="data/IssueItems">
              <tr>
                <td><xsl:value-of select="Product" /></td>
                <!--<td class="text-right"><xsl:value-of select="ExcessQty" /></td>-->
                <td class="text-right"><xsl:value-of select="SentQty" /></td>
              </tr>
            </xsl:for-each>
            <xsl:if test="count(data/IssueItems) = 0">
              <tr><td colspan="2" style="font-style:italic;">No issued lines.</td></tr>
            </xsl:if>
          </table>

          <p style="margin-top:22px;margin-bottom:6px;font-weight:bold;">2. Items received</p>
          <table style="width:100%; border-collapse:collapse;">
            <tr>
              <th style="width:75%;">Product</th>
              <th class="text-right" style="width:25%;">Qty</th>
            </tr>
            <xsl:for-each select="data/ReceiveItems">
              <tr>
                <td><xsl:value-of select="Product" /></td>
                <td class="text-right"><xsl:value-of select="Quantity" /></td>
              </tr>
            </xsl:for-each>
            <xsl:if test="count(data/ReceiveItems) = 0">
              <tr><td colspan="2" style="font-style:italic;">No received lines.</td></tr>
            </xsl:if>
          </table>

          <footer>
            <div style="width:100%; text-align:center;font-size:12px;margin-top:28px;">
              <p style="text-align:right;margin-bottom:20px;margin-right:10px;">
                <b>
                  For <i>
                    <xsl:value-of select="data/Company/Name" />
                  </i>
                </b>
              </p>
              <p>
                Mobile:   <xsl:value-of select="data/Company/Phone1" />, Email:   <xsl:value-of select="data/Company/Email" />
              </p>
            </div>
          </footer>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
