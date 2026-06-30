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
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">MATERIAL LOSS RECEIPT</p>
                Receipt #: <xsl:value-of select="data/Number" /><br />
              </td>
            </tr>

          </table>
          <table style="width:100%;border-collapse:collapse;margin-top:10px;margin-top:30px;">
            <tr>
              <td style="border: 0px;">
                <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Client</p>

                <xsl:value-of select="data/Ledger/Name" /><br />

                <xsl:value-of select="data/Ledger/Address1" />  <xsl:value-of select="data/Ledger/Address2" /><br />
                <xsl:value-of select="data/Ledger/City" />, PIN: -   <xsl:value-of select="data/Ledger/ZipCode" /><br />
                <xsl:value-of select="data/Ledger/State" /><br/>
                GSTIN: <xsl:value-of select="data/Ledger/GSTNo" /><br />
                Site: <xsl:value-of select="data/SiteAddress" /><br/>
              </td>
              <td style="vertical-align: top; border: 0px;text-align:right">
                Receipt Date:   <xsl:value-of select="util:DateToDDMMYYYY(data/EntryDate)"/>   <br />
              </td>
            </tr>
          </table>


          <table style="width:100%; border-collapse:collapse;margin-top:10px;">

            <tr>

              <th style="width:40%;">Product Name  Desc.</th>

              <th class="text-right" style="width:100px;">Quantity</th>

              <th class="text-right" style="width: 100px;">Rate</th>

              <th class="text-right" style="width: 100px;" >Amount</th>


            </tr>
            <xsl:for-each select="data/Items">
              <tr>

                <td>
                  <xsl:value-of select="Product" />
                </td>
                <td class="text-right" >
                  <xsl:value-of select="Quantity" />
                </td>
                <td class="text-right" >
                  <xsl:value-of select="Rate" />
                </td>
                <td class="text-right" >
                  <xsl:value-of select="Amount" />
                </td>
              </tr>
            </xsl:for-each>
            <tr>
              <td style=" padding:0px; vertical-align: top;" colspan="2" >

              </td>
              <th class="text-right" style="border-top: 0px; border-right: 0px;">SubTotal</th>
              <th style="border-top:0px;" class="text-right">
                <xsl:value-of select="sum(data/Items/Amount)"/>

              </th>
            </tr>



          </table>

          <footer>
            <div style="width:100%; text-align:center;font-size:12px;">
              <p style="text-align:right;margin-bottom:20px;margin-right:10px;">
                <b>
                  For <i>
                    <xsl:value-of select="data/Company/Name" />
                  </i>
                </b>
              </p>
              <p>
                If you have any questions about this invoice, please contact
                <br />

                Mobile:   <xsl:value-of select="data/Company/Phone1" />, Email:   <xsl:value-of select="data/Compay/Email" />
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
