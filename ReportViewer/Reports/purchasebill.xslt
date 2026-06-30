<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:util="urn:util-format">
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
                    <xsl:value-of select="data/CompanyDTO/Logo"/>
                  </xsl:attribute>
                </img>
                <br/><br/>
                <xsl:value-of select="data/CompanyDTO/Name" /><br />


                <xsl:value-of select="data/CompanyDTO/Address1" />  <xsl:value-of select="data/CompanyDTO/Address2" /><br />
                <xsl:value-of select="data/CompanyDTO/City" />, PIN: -   <xsl:value-of select="data/CompanyDTO/ZipCode" /><br />
                <xsl:value-of select="data/CompanyDTO/State" /><br/>
                GSTIN:   <xsl:value-of select="data/CompanyDTO/GSTNo" /><br />
              </td>
              <td style=" text-align:right;vertical-align:top" class="noborder">
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">PURCHASE INVOICE</p>
                Invoice #: <xsl:value-of select="data/VoucherNumber" /><br />
                Invoice Date:   <xsl:value-of select="util:DateToDDMMYYYY(data/PurchaseDate)"/> <br />
              </td>
            </tr>

          </table>
          <table style="width:100%;border-collapse:collapse;margin-top:10px;margin-top:30px;">



          </table>


          <table style="width:100%; border-collapse:collapse;margin-top:-1px;">
            <tr>
              <td colspan="2">
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Vendor</p>
                <xsl:value-of select="data/LedgerDTO/Name" /><br />

                <xsl:value-of select="data/LedgerDTO/Address1" />  <xsl:value-of select="data/LedgerDTO/Address2" /><br />
                <xsl:value-of select="data/LedgerDTO/City" />, PIN: -   <xsl:value-of select="data/LedgerDTO/ZipCode" /><br />
                <xsl:value-of select="data/LedgerDTO/State" /><br/>
                GSTIN: <xsl:value-of select="data/LedgerDTO/GSTNo" />
              </td>
              <td colspan="3" style="vertical-align:top;">
                <table style="width:100%;margin-top:0px;">
                  <tr>
                    <td class="noborder">
                      Vendor Bill No
                    </td>
                    <td class="noborder">
                      <xsl:value-of select="data/BillNumber" />
                    </td>
                  </tr>

                </table>
              </td>

            </tr>

            <tr>

              <th style="width:50%;" colspan="2">Product Name  Desc.</th>
              <th class="text-right" style="width:80px;">Quantity</th>
              <th class="text-right" style="width: 80px;">Rate</th>
              <th class="text-right"  style="width: 80px;">Amount</th>

            </tr>
            <xsl:for-each select="data/Items">
              <tr>
                <td colspan="2">
                  <xsl:value-of select="Item" />
                  <xsl:text> </xsl:text>
                  <span style="display: block;font-size: 10px;">
                  <xsl:value-of select="Quantity" />
                    <xsl:text> </xsl:text>
                  <xsl:value-of select="Unit" />
                  </span>
                </td>
                <td class="text-right">
               
                  <xsl:choose>
                    <xsl:when test="Unit1Quantity > 0">
                      <xsl:value-of select="Unit1Quantity" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="PurchaseUnitName" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="Quantity" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="Unit" />
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
                <td class="text-right">
                  <xsl:value-of select="Rate" />
                </td>
                <td class="text-right">
                  <xsl:value-of select="SubTotal" />
                </td>

              </tr>
            </xsl:for-each>
            <tr>
              <td style=" padding:0px; vertical-align: bottom;" colspan="3" rowspan="7">
                <table style="width:85%;margin-top:0px;">
                  <tr>
                    <td class="noborder text-center" style="text-decoration:underline" >Terms and Conditions</td>
                  </tr>
                  <tr>
                    <td style="vertical-align:top;padding:0px;" class="noborder">
                      <ul>

                        <li>Payment is to be made within 30 days on preparation of the bill or interest will be charged @ 24% p.a.</li>
                        <li>Please include the invoice number on your check</li>
                      </ul>

                    </td>
                  </tr>
                </table>
              </td>

              <th class="text-right" style="border-right:0px;">IGST</th>
              <th  class="text-right">
                <xsl:value-of select="sum(data/Items/SubTotal)"/>
              </th>
            </tr>
            <tr>
              <th class="text-right" style="border-right:0px;">IGST</th>
              <th  class="text-right">
                <xsl:value-of select="sum(data/Items/IGST)"/>
              </th>
            </tr>
            <tr>
              <th class="text-right" style="border-top: 0px; border-right: 0px;">SGST</th>
              <th style="border-top:0px;" class="text-right">
                <xsl:value-of select="sum(data/Items/SGST)"/>
              </th>
            </tr>
            <tr>
              <th class="text-right" style="border-top: 0px; border-right: 0px;">CGST</th>
              <th style="border-top:0px;" class="text-right">
                <xsl:value-of select="sum(data/Items/CGST)"/>
              </th>
            </tr>

            <tr>
              <th class="text-right" style="border-top: 0px; border-right: 0px;">Discount</th>
              <th style="border-top:0px;" class="text-right">
                <xsl:value-of select="data/DiscountAmount"/>
              </th>
            </tr>
            <tr>
              <th class="text-right" style="border-top: 0px; border-right: 0px;">Total</th>
              <th style="border-top:0px;" class="text-right">
                <xsl:value-of select="data/Total"/>
              </th>
            </tr>

          </table>

          <footer>
            <div style="width:100%; text-align:center;font-size:12px;">
              <p style="text-align:right;margin-bottom:20px;margin-right:10px;">
                <b>
                  For <i>
                    <xsl:value-of select="data/CompanyDTO/Name" />
                  </i>
                </b>
              </p>
              <p>
                If you have any questions about this invoice, please contact
                <br />

                Mobile:  <xsl:value-of select="data/CompanyDTO/Phone1" />, Email:   <xsl:value-of select="data/CompanyDTO/Email" />
              </p>
              <p>
                Thank you for your Business!
              </p>
            </div>
          </footer>
        </div>

      </body>
    </html>



  </xsl:template>
</xsl:transform>
