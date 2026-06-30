<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        #pdf
        #preview
      </head>
      <body>

        <div id="printArea" >

          <xsl:variable name="imageUrl" select="d/bgImage" />
         
          <header style="display:none;" >
            <div style="background-image: url('{$imageUrl}'); 
                  background-size: cover; 
                  height: 220px; 
                 
                 ">
           
                
             

              <table style="width: 100%;border:0px;" cellpadding="0" cellspacing="0">

                <tr>
                  <td style="border:0px;padding:30px;">

                    <img style="height:60px;max-width:150px;">
                      <xsl:attribute name="src">
                        <xsl:value-of select="d/data/Table/CompanyLogo"/>
                      </xsl:attribute>
                    </img>



                  </td>
                  <td style=" text-align:right;padding-top:30px;padding-right:30px;" class="noborder">

                    <xsl:value-of select="d/data/Table/CompanyName" /><br />
                    <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                    <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                    <xsl:value-of select="d/data/Table/CompanyState" />, India<br />
                    <xsl:value-of select="d/data/Table/CompanyWebsite" /><br />
                    <xsl:value-of select="d/data/Table/CompanyEmail" /><br />
                    GSTIN:<xsl:value-of select="d/data/Table/CompanyGST" />

                  </td>
                </tr>

              </table>
            </div>
          </header>



          <div class="content-block" style="">
            <table style="width:100%; border-collapse:collapse;">
              <tr>
                <td colspan="2" class="subHeading1 noborder">Invoicing Address</td>
                <td colspan="4" class="subHeading1 noborder">Site Address</td>
              </tr>
              <tr>
                <td colspan="2" style="border:0px;">
                  <xsl:value-of select="d/data/Table/PartyName" /><br />
                  <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                  <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />

                  <xsl:value-of select="d/data/Table/PartyState" /><br />
                  <xsl:value-of select="d/data/Table/PartyPhone" /><br />
                  <strong>Shipping Address</strong>
                  <xsl:value-of select="d/data/Table/ShipAddress1" /><br />
                  <xsl:value-of select="d/data/Table/ShipAddress2" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                  <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />

                  <xsl:value-of select="d/data/Table/PartyState" /><br />

                </td>
                <td colspan="4" style="vertical-align:top;border:0px;">
                  <xsl:value-of select="d/data/Table/PartyName" />
                  <br />
                  <xsl:value-of select="d/data/Table/ShipAddress1" />
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="d/data/Table/ShipAddress2" />
                  <br />
                  <xsl:value-of select="d/data/Table/ShipCity" />
                  <xsl:value-of select="d/data/Table/ShipZipCode" />
                  <br>

                  </br>
                  <xsl:value-of select="d/data/Table/ShipStateName" />
                </td>

              </tr>
              <tr>
                <td colspan="5" class="noborder" >
                  <label style="font-size:26px;">
                    Order # <xsl:value-of select="d/data/Table/QuotationNumber" />
                  </label>
                </td>
              </tr>
              <tr>
                <th colspan="2"  class="noborder" style="padding:0px;padding-left:5px;">
                  Order Date
                </th>
                <th colspan="3"  class="noborder" style="padding:0px;">
                  Salesperson
                </th>
              </tr>
              <tr>
                <td colspan="2"  class="noborder" style="padding:0px;padding-left:5px;">
                  <xsl:value-of select="d/data/Table/QuotationNumber" />
                </td>
                <td colspan="3"  class="noborder" style="padding:0px;">
                  Nancy
                </td>
              </tr>
              <tr>
                <td colspan="5"  class="noborder" >

                </td>
              </tr>
              <tr>

                <th style="width:40%;border-top:solid 1px;">Description</th>
                <th  class="text-center" style="width:100px;border-top:solid 1px;;">Quantity</th>

                <th class="text-center" style="width: 100px;border-top:solid 1px;;">Unit Price</th>
                <th class="text-center" style="width: 100px;border-top:solid 1px;;">Taxes</th>
                <th class="text-right" style="width: 100px;border-top:solid 1px;;" >Amount</th>
              </tr>
              <xsl:for-each select="d/data/Table">
                <tr>
                  <td>
                    <b>
                      <xsl:value-of select="Item" />
                    </b>
                    <br/>
                    <xsl:value-of select="Description" />
                  </td>

                  <td  class="text-center" >
                    <xsl:value-of select="Quantity" />
                  </td>

                  <td  class="text-center" >
                    <xsl:value-of select="Rate" />
                  </td>
                  <td  class="text-center" >
                    <xsl:value-of select="duration" />
                  </td>
                  <td  class="text-right" >
                    <xsl:value-of select="ItemSubTotal" />
                  </td>
                </tr>
              </xsl:for-each>


            </table>



            <div style="width:100%; text-align:center;">
              <table>
                <tr>
                  <td class="noborder" style="text-decoration:underline" >
                    <h4>  Additional Information</h4>
                  </td>
                </tr>
                <tr>
                  <td style="vertical-align:top;padding:0px;" class="noborder">
                    <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>

                  </td>
                </tr>
                <tr>
                  <td class="noborder" style="text-decoration:underline" >
                    <b>Terms and Conditions</b>
                  </td>
                </tr>
                <tr>
                  <td  class="noborder">
                    <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                  </td>
                </tr>
              </table>

              <!--<p>
                If you have any questions about this price quote, please contact
                At: <br />

                Mobile:    <xsl:value-of select="d/data/Table/CompanyPhone" />, Email:    <xsl:value-of select="d/data/Table/CompanyEmail" />
              </p>
              <p>
                Thank you for your Business!
              </p>-->

            </div>

          </div>
        </div>

      </body>
    </html>


  </xsl:template>
</xsl:transform>
