<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          td, th { border: 1px solid #ddd; padding: 4px; font-size: 11pt; }
          .text-right { text-align: right; }
          .text-center { text-align: center; }
          .noborder { border: none !important; }
          .headerUL li { padding-bottom: 4px; margin: 0; list-style: none; }
          .party-header {  font-weight: bold; }
          .site-header {   font-weight: bold; }
          .subtotal-row { background-color:#f3f3f3;  font-weight: bold; }
          .party-subtotal-row { font-weight: bold; }
          @media print {
          .no-page-break { page-break-inside: avoid !important; }
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body style="margin:0px;">
        <div id="printArea" style="padding:5px;">

          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="7">
                <ul class="headerUL" style="list-style:none;padding:0;">
                  <li>
                    <h2 style="color:green;padding:0;margin:0;">
                      <xsl:value-of select="d/company/Name"/>
                    </h2>
                  </li>
                  <li>
                    <xsl:value-of select="d/company/Address1"/>
                    <xsl:if test="d/company/Address2 != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/Address2"/>
                    </xsl:if>
                    <xsl:if test="d/company/City != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/City"/>
                    </xsl:if>
                    <xsl:if test="d/company/State != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone:</b>
                    <xsl:value-of select="d/company/Phone1"/>
                  </li>
                  <li>
                    <strong>Party Payments Report</strong>
                  </li>
                  <li>From <xsl:value-of select="util:DateToDDMMYYYY(d/from)"/> To <xsl:value-of select="util:DateToDDMMYYYY(d/to)"/></li>
                </ul>
              </td>
            </tr>
          </table>

          <table style="width:100%;border-collapse:collapse;margin-top:10px;">
            <thead>
              <tr>
                <th style="width:100px;">Date</th>
                <th class="text-right" style="width:140px;">Amount</th>
                <th style="width:80px;text-align:center;">Mode</th>
                <th style="width:100px;text-align:center;">Payment Date</th>
                <th style="width:150px;">Chq.Number</th>
                <th>Narration</th>
              </tr>
            </thead>
            <tbody>
              <xsl:for-each select="d/parties">
                <tr class="party-header no-page-break">
                  <td colspan="6" style="padding:8px;">
                    <strong>
                      Party: <xsl:value-of select="partyName"/>
                    </strong>
                  </td>
                </tr>
                <xsl:for-each select="sites">
                  <tr class="site-header no-page-break">
                    <td colspan="6" style="padding:6px 8px 6px 24px;">
                      <strong>
                        Site: <xsl:value-of select="Site"/>
                      </strong>
                    </td>
                  </tr>
                  <xsl:for-each select="items">
                    <tr class="no-page-break">
                      <td> </td>
                      <td class="text-right">
                        <xsl:value-of select="util:FormatNumber(TransactionAmount)"/>
                      </td>
                      <td  style="text-align:center;">
                        <xsl:choose>
                          <xsl:when test="TransactionMode = 1">Cash</xsl:when>
                          <xsl:when test="TransactionMode = 2">Bank</xsl:when>
                          <xsl:when test="TransactionMode = 3">Cheque</xsl:when>
                          <xsl:when test="TransactionMode = 4">NEFT/RTGS</xsl:when>
                          <xsl:when test="TransactionMode = 5">Others</xsl:when>
                          <xsl:when test="TransactionMode = 6">UPI</xsl:when>
                          <xsl:otherwise>-</xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td style="text-align:center;">
                        <xsl:value-of select="util:DateToDDMMYYYY(TransactionDate)"/>
                      </td>
                      <td>
                        <xsl:value-of select="ChequeNumber"/>
                      </td>
                      <td>
                        <xsl:value-of select="Narration"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                  <tr class="subtotal-row no-page-break">
                    <td  style="padding-left:24px;">
                      Subtotal (<xsl:value-of select="siteName"/>):
                    </td>
                    <td class="text-right">
                      <xsl:value-of select="util:FormatNumber(subtotal)"/>
                    </td>
                    <td colspan="4"></td>
                  </tr>
                </xsl:for-each>
                <!--<tr class="party-subtotal-row no-page-break">
                  <td   style="padding-left:16px;">
                    Party Subtotal (<xsl:value-of select="partyName"/>):
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="util:FormatNumber(partySubtotal)"/>
                  </td>
                  <td colspan="4"></td>
                </tr>-->
                <tr>
                  <td colspan="6" style="height:8px;border:none;"></td>
                </tr>
              </xsl:for-each>
              <tr class="subtotal-row">
                <td  class="text-right">
                  <strong>Grand Total</strong>
                </td>
                <td class="text-right">
                  <strong>
                    <xsl:value-of select="util:FormatNumber(d/grandTotal)"/>
                  </strong>
                </td>
                <td colspan="4"></td>
              </tr>
            </tbody>
          </table>

        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
