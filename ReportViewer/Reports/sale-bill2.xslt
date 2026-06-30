<?xml version="1.0" encoding="utf-8"?>
<!-- Sale invoice layout inspired by RBN-style tax invoice (Invoice_0039): TAX INVOICE, Bill To, DESCRIPTION/SAC/AMOUNT, tax summary, bank notes, signatory. -->
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:output method="html" indent="yes" omit-xml-declaration="yes"/>

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <meta name="viewport" content="width=device-width,initial-scale=1" />
        <style type="text/css">
          body { font-family: Calibri, Arial, sans-serif; font-size: 12px; color: #222; margin: 12px 16px; }
          h1 { font-size: 20px; margin: 0 0 12px 0; letter-spacing: 0.05em; }
          .muted { color: #444; font-size: 11px; }
          table.layout { width: 100%; border-collapse: collapse; }
          table.layout td { vertical-align: top; padding: 4px 0; }
          table.items { width: 100%; border-collapse: collapse; margin-top: 12px; }
          table.items th, table.items td { border: 1px solid #333; padding: 6px 8px; }
          table.items th { background: #f0f0f0; font-weight: bold; text-align: left; }
          table.items td.desc { vertical-align: top; }
          table.items td.num, table.items th.num { text-align: right; }
          table.items td.sac, table.items th.sac { text-align: center; width: 90px; }
          .totals { width: 100%; margin-top: 8px; border-collapse: collapse; }
          .totals td { padding: 4px 8px; border: none; }
          .totals .lbl { text-align: right; width: 72%; }
          .totals .val { text-align: right; width: 28%; font-weight: normal; }
          .totals .grand .val { font-weight: bold; font-size: 13px; }
          .words { margin: 10px 0; font-style: italic; font-size: 11px; }
          ul.notes { margin: 8px 0; padding-left: 18px; font-size: 11px; }
          ul.notes li { margin: 4px 0; }
          .footer-co { margin-top: 16px; font-size: 11px; line-height: 1.45; }
          .sign { margin-top: 28px; text-align: right; }
          .banner { margin-top: 14px; padding: 10px 12px; border: 1px solid #333; background: #fafafa; font-size: 11px; }
          .banner strong { display: inline-block; min-width: 140px; }
          pre.lineitem { margin: 4px 0 0 0; white-space: pre-wrap; font-family: inherit; font-size: 11px; border: 0; background: transparent; padding: 0; }
        </style>
      </head>
      <body>
        <table class="layout" cellpadding="0" cellspacing="0">
          <tr>
            <td style="width:55%;">
              <xsl:if test="string(data/Table/CompanyLogo) != ''">
                <img style="max-height:56px;max-width:160px;margin-bottom:6px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="data/Table/CompanyLogo"/>
                  </xsl:attribute>
                </img>
                <br/>
              </xsl:if>
              <div class="muted">
                <xsl:value-of select="data/Table/Company"/>
                <br/>
                <xsl:value-of select="data/Table/CompanyAddress1"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="data/Table/CompanyAddress2"/>
                <br/>
                <xsl:value-of select="data/Table/CompanyCity"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="data/Table/CompanyZipCode"/>
              </div>
            </td>
            <td style="width:45%; text-align:right;">
              <h1>TAX INVOICE</h1>
              <div>
                <strong>INVOICE NUMBER:</strong>
                <xsl:text> </xsl:text>
                <xsl:value-of select="data/Table/InvoiceNumber"/>
              </div>
              <div style="margin-top:4px;">
                <strong>Date:</strong>
                <xsl:text> </xsl:text>
                <xsl:value-of select="util:DateToDDMMYYYY(data/Table/InvoiceDate)"/>
              </div>
            </td>
          </tr>
        </table>

        <table class="layout" style="margin-top:14px;">
          <tr>
            <td style="width:58%;">
              <strong>Bill To:</strong>
              <br/>
              <strong>
                <xsl:value-of select="data/Table/Client"/>
              </strong>
              <br/>
              <span class="muted">GSTIN: </span>
              <xsl:value-of select="data/Table/ClientGST"/>
              <br/>
              <span class="muted">Bill Address</span>
              <br/>
              <xsl:choose>
                <xsl:when test="data/Table/BillFromSiteAddress = 1">
                  <xsl:value-of select="data/Table/SiteProject"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/SiteAddress"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/SiteAddress2"/>
                  <br/>
                  <xsl:value-of select="data/Table/City"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/SiteZipCode"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="data/Table/ClientAddress1"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/ClientAddress2"/>
                  <br/>
                  <xsl:value-of select="data/Table/ClientCity"/>
                  <xsl:text>, </xsl:text>
                  <xsl:value-of select="data/Table/ClientStateName"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/ClientZipCode"/>
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td style="width:42%;">
              <xsl:if test="data/Table/billPONumber != ''">
                <div class="muted">
                  <strong>PO#:</strong>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="data/Table/billPONumber"/>
                </div>
                <div class="muted">
                  <strong>PO Date:</strong>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="util:DateToDDMMYYYY(data/Table/billPODate)"/>
                </div>
              </xsl:if>
            </td>
          </tr>
        </table>

        <table class="items" cellspacing="0">
          <thead>
            <tr>
              <th style="width:62%;">DESCRIPTION</th>
              <th class="sac">SAC</th>
              <th class="num" style="width:22%;">AMOUNT</th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="data/Table">
              <tr>
                <td class="desc">
                  <span style="font-size:14px;line-height:1;">&#8226;</span>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="Item"/>
                  <xsl:if test="string(LineItem) != ''">
                    <pre class="lineitem">
                      <xsl:value-of select="LineItem"/>
                    </pre>
                  </xsl:if>
                </td>
                <td class="sac">
                  <xsl:value-of select="HSNCode"/>
                </td>
                <td class="num">
                  <xsl:value-of select="format-number(SubTotal,'#,###.00')"/>
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>

        <table class="totals">
          <tr>
            <td class="lbl">Sub Total</td>
            <td class="val">
              <xsl:value-of select="format-number(sum(data/Table/SubTotal),'#,###.00')"/>
            </td>
          </tr>
        </table>

        <div class="words">
          (RS
          <xsl:text> </xsl:text>
          <xsl:value-of select="util:AmountToWords(data/Table/RoundedAmount)"/>
          <xsl:text> </xsl:text>
          ONLY)
        </div>

        <table class="totals">
          <tr>
            <td class="lbl">
              IGST (<xsl:value-of select="data/Table/IGSTRate"/>%)
            </td>
            <td class="val">
              <xsl:value-of select="format-number(sum(data/Table/IGST),'#,###.00')"/>
            </td>
          </tr>
          <tr>
            <td class="lbl">
              CGST (<xsl:value-of select="data/Table/CGSTRate"/>%)
            </td>
            <td class="val">
              <xsl:value-of select="format-number(sum(data/Table/CGST),'#,###.00')"/>
            </td>
          </tr>
          <tr>
            <td class="lbl">
              SGST (<xsl:value-of select="data/Table/SGSTRate"/>%)
            </td>
            <td class="val">
              <xsl:value-of select="format-number(sum(data/Table/SGST),'#,###.00')"/>
            </td>
          </tr>
          <xsl:if test="data/Table/Freight != 0">
            <tr>
              <td class="lbl">Freight</td>
              <td class="val">
                <xsl:value-of select="format-number(data/Table/Freight,'#,###.00')"/>
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="data/Table/discount != 0">
            <tr>
              <td class="lbl">Discount</td>
              <td class="val">
                <xsl:value-of select="format-number(data/Table/discount,'#,###.00')"/>
              </td>
            </tr>
          </xsl:if>
          <tr>
            <td class="lbl">Total</td>
            <td class="val grand">
              <xsl:value-of select="format-number(data/Table/Total,'#,###.00')"/>
            </td>
          </tr>
        </table>

        <xsl:if test="data/Table/PrintBankDetails = 'True'">
          <ul class="notes">
            <li>Make all checks payable to <strong><xsl:value-of select="data/Table/Company"/></strong></li>
            <li>
              <strong>Bank Details:</strong>
              <xsl:value-of select="data/Table/CompanyBankName"/>
              <xsl:if test="string(data/Table/CompanyBankBranch) != ''">
                <xsl:text>, Branch: </xsl:text>
                <xsl:value-of select="data/Table/CompanyBankBranch"/>
              </xsl:if>
            </li>
            <li>
              Current Account No: <xsl:value-of select="data/Table/CompanyBankAccNumber"/>
              <xsl:text>, IFSC Code: </xsl:text>
              <xsl:value-of select="data/Table/CompanyBankIFSC"/>
            </li>
          </ul>
        </xsl:if>

        <div class="footer-co">
          <strong>GSTN No:</strong>
          <xsl:text> </xsl:text>
          <xsl:value-of select="data/Table/CompanyGST"/>
          <br/>
          <strong>PAN No:</strong>
          <xsl:text> </xsl:text>
          <xsl:value-of select="data/Table/CompanyPAN"/>
          <br/>
          <xsl:value-of select="data/Table/CompanyAddress1"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="data/Table/CompanyAddress2"/>
          <xsl:text> </xsl:text>
          <xsl:value-of select="data/Table/CompanyCity"/>
          <xsl:text>, </xsl:text>
          <xsl:value-of select="data/Table/CompanyZipCode"/>
          <xsl:if test="string(data/Table/CompanyEmail) != ''">
            <xsl:text>, </xsl:text>
            <xsl:value-of select="data/Table/CompanyEmail"/>
          </xsl:if>
        </div>

        <div class="sign">
          <div>For <strong><xsl:value-of select="data/Table/Company"/></strong></div>
          <div style="margin-top:36px;">Authorized Signatory</div>
          <xsl:if test="string(data/Table/Signature) != ''">
            <div style="margin-top:8px;">
              <img style="max-width:120px;max-height:50px;">
                <xsl:attribute name="src">
                  <xsl:value-of select="data/Table/Signature"/>
                </xsl:attribute>
              </img>
            </div>
          </xsl:if>
        </div>

        <div class="banner">
          <div><strong>ORIGINAL INVOICE</strong></div>
          <div style="margin-top:6px;">
            <strong>TOTAL AMOUNT:</strong>
            INR
            <xsl:text> </xsl:text>
            <xsl:value-of select="format-number(data/Table/Total,'#,###.00')"/>
          </div>
          <div style="margin-top:4px;">
            <strong>Date:</strong>
            <xsl:text> </xsl:text>
            <xsl:value-of select="util:DateToDDMMYYYY(data/Table/InvoiceDate)"/>
          </div>
          <div style="margin-top:4px;">
            <strong>INVOICE NUMBER:</strong>
            <xsl:text> </xsl:text>
            <xsl:value-of select="data/Table/InvoiceNumber"/>
          </div>
          <div style="margin-top:6px;" class="muted">
            <xsl:value-of select="data/Table/Company"/>
            <xsl:text> — </xsl:text>
            <xsl:value-of select="data/Table/CompanyAddress1"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="data/Table/CompanyCity"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="data/Table/CompanyZipCode"/>
          </div>
        </div>

        <xsl:if test="data/Table/SignedQrCode != ''">
          <table class="layout" style="margin-top:16px;">
            <tr>
              <td style="width:140px;">
                <img style="width:120px;height:120px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="data/Table/SignedQrCode"/>
                  </xsl:attribute>
                </img>
              </td>
              <td>
                <div><strong>IRN</strong> — <xsl:value-of select="data/Table/IRN"/></div>
                <div><strong>Ack No</strong> — <xsl:value-of select="data/Table/IrnAckNo"/></div>
                <div><strong>Ack Date</strong> — <xsl:value-of select="util:DateToDDMMYYYY(data/Table/IrnACKDate)"/></div>
              </td>
            </tr>
          </table>
        </xsl:if>

        <xsl:if test="data/Table/Tnc != ''">
          <div style="margin-top:14px; font-size:11px;">
            <xsl:value-of select="data/Table/Tnc" disable-output-escaping="yes"/>
          </div>
        </xsl:if>

      </body>
    </html>
  </xsl:template>
</xsl:transform>
