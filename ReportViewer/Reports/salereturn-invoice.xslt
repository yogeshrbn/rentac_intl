<?xml version="1.0" encoding="utf-8"?>

 
  <xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  
    <xsl:template match="/">
      <html xmlns="http://www.w3.org/1999/xhtml">
        <head>

          #preview
          #pdf

          <meta name="viewport" content="width=device-width,initial-scale=1" />
        </head>
        <body>
          <div id="printArea">
            <table style="width: 100%" cellpadding="0" cellspacing="0">
              <tr>
                <td style="border:0px;">
                  <img style="height:60px;max-width:150px;">
                    <xsl:attribute name="src">
                      <xsl:value-of select="data/Table/CompanyLogo"/>
                    </xsl:attribute>
                  </img>
                  <br/><br/>
                  <xsl:value-of select="data/Table/Company" /><br />


                  <xsl:value-of select="data/Table/CompanyAddress1" />  <xsl:value-of select="data/Table/CompanyAddress2" /><br />
                  <xsl:value-of select="data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="data/Table/CompanyZipCode" /><br />
                  GSTIN:   <xsl:value-of select="data/Table/CompanyGST" /><br />
                </td>
                <td style=" text-align:right;vertical-align:top" class="noborder">
                  <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">SALES RETURN INVOICE</p>
                  Invoice #: <xsl:value-of select="data/Table/InvoiceNumber" /><br />
                  Invoice Date:  <xsl:value-of select="util:DateToDDMMYYYY(data/Table/InvoiceDate)"/><br />
                 
                </td>
              </tr>

            </table>
            <table style="width:100%;border-collapse:collapse;margin-top:10px;margin-top:30px;">


              <tr>

                <td  colspan="3">
                  <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Bill TO</p>
                  <xsl:value-of select="data/Table/Client" /><br />
                  <xsl:value-of select="data/Table/ClientAddress1" />  <xsl:value-of select="data/Table/ClientAddress2" /><br />
                  <xsl:value-of select="data/Table/ClientCity" />, PIN: -   <xsl:value-of select="data/Table/ClientZipCode" /><br />
                  GSTIN: <xsl:value-of select="data/Table/SiteGST" /><br />

                </td>
                <td style="vertical-align: top;" colspan="3">
                  <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Ship TO</p>
                  <xsl:value-of select="data/Table/Client" /><br />
                  <xsl:value-of select="data/Table/ClientAddress1" />  <xsl:value-of select="data/Table/ClientAddress2" /><br />
                  <xsl:value-of select="data/Table/ClientCity" />, PIN: -   <xsl:value-of select="data/Table/ClientZipCode" /><br />
                  GSTIN: <xsl:value-of select="data/Table/SiteGST" /><br />
                </td>
              </tr>
              <tr>
                <th style="width:80px;">HSN Code</th>
                <th style="width:40%;" colspan="2">Product Name  Desc.</th>

                <th class="text-center" style="width:100px;">Quantity</th>

                <th class="text-right" style="width: 100px;">Rate</th>

                <th class="text-right" style="width: 100px;" >Amount</th>


              </tr>
              <xsl:for-each select="data/Table">
                <tr>
                  <td>
                    <xsl:value-of select="HSNCode" />
                  </td>
                  <td colspan="2">
                    <xsl:value-of select="Item" />
                  </td>
                  <td class="text-right" >
                    <xsl:value-of select="Quantity" />
                  </td>
                  <td class="text-right" >
                    <xsl:value-of select="Rate" />
                  </td>
                  <td class="text-right" >
                    <xsl:value-of select="SubTotal" />
                  </td>
                </tr>
              </xsl:for-each>
              <tr>
                <td style=" padding:0px; vertical-align: bottom;" colspan="4" rowspan="7">
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
                <th class="text-right" style="border-top: 0px; border-right: 0px;">SubTotal</th>
                <th style="border-top:0px;"  class="text-right">
                  <xsl:value-of select="sum(data/Table/SubTotal)"/>

                </th>

              </tr>
              <tr>
                <th class="text-right" style="border-right:0px;">IGST</th>
                <th class="text-right" >
                  <xsl:value-of select="sum(data/Table/IGST)"/>
                </th>
              </tr>
              <tr>
                <th class="text-right" style="border-top: 0px; border-right: 0px;">SGST</th>
                <th style="border-top:0px;" class="text-right" >
                  <xsl:value-of select="sum(data/Table/SGST)"/>

                </th>
              </tr>
              <tr>
                <th class="text-right" style="border-top: 0px; border-right: 0px;">CGST</th>
                <th style="border-top:0px;" class="text-right" >
                  <xsl:value-of select="sum(data/Table/CGST)"/>
                </th>
              </tr>
              <tr>
                <th class="text-right" style="border-top: 0px; border-right: 0px;">Discount</th>
                <th style="border-top:0px;" class="text-right" >
                  <xsl:value-of select="data/Table/discount" />
                </th>
              </tr>
              <tr>
                <th class="text-right" style="border-top: 0px; border-right: 0px;">Total</th>
                <th style="border-top:0px;" class="text-right" >
                  <xsl:value-of select="data/Table/Total" />
                </th>
              </tr>

            </table>

            <footer>
              <div style="width:100%; text-align:center;font-size:12px;">
                <p style="text-align:right;margin-bottom:20px;margin-right:10px">
                  <b>
                    For <i>
                      <xsl:value-of select="data/Table/Company" />
                    </i>
                  </b>
                </p>
                <p>
                  If you have any questions about this invoice, please contact
                  <br />

                  Mobile:   <xsl:value-of select="data/Table/CompanyPhone" />, Email:   <xsl:value-of select="data/Table/CompanyEmail" />
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
 