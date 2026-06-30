<?xml version="1.0" encoding="utf-8"?>


<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          td {
          line-height:20px;
          padding:0px;
          }
          .headerUL li  {
          padding-bottom:3px;
          padding-top:0px;
          margin:0px;
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:10px;">

          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td colspan="3" class="text-center noborder" style="font-size:16px;font-weight:bold;">
                <xsl:value-of select="d/company/Name"/>
              </td>
            </tr>
            <tr>
              <td  style="width:20%;vertical-align:top;" class="noborder">
                <strong>TIN/CST No: </strong>
                <xsl:value-of select="d/company/TIN"/>
              </td>
              <td style="width:60%;vertical-align:top;"  class="text-center noborder"  >
                <ul   style="list-style:none;padding:0px;margin-top:0px; width:100%;display:table">
                  <li>
                    <xsl:value-of select="d/company/Address1"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="d/company/Address2"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="d/company/City"/>
                  </li>
                  <li>
                    <strong>Phone No :</strong>
                    <xsl:value-of select="d/company/Phone1"/>
                  </li>
                  <li>
                    <strong>
                      (Challan Report Between  <xsl:value-of select="d/from"/> To  <xsl:value-of select="d/to"/>)
                    </strong>
                  </li>
                </ul>

              </td>
              <td style="width:20%;vertical-align:top;" class="text-right noborder">
                <strong>Date:</strong>
                <xsl:value-of select="d/printDate"/>
              </td>
            </tr>
          
            <tr>
              <td colspan="3"  style="border:0;border-top:solid 1px;">
               
                <xsl:for-each select="d/delivery">
                  <div style="border-bottom:dashed 1px #000;padding-bottom:10px;padding-top:10p;padding-right:10px;">
                    <table style="width:100%">

                      <tr>
                        <td class="noborder" style="width:50%;">

                          <strong>
                         Challan#   <xsl:text> </xsl:text><xsl:value-of select="Number"/>
                          </strong>
                        </td>
                        <td class="noborder">
                          <strong>Challan Date:</strong>
                          <xsl:text >   </xsl:text><xsl:value-of select="util:DateToDDMMYYYY(WorkOrderDate)"/>
                        </td>

                      </tr>
                      <tr >
                        <td colspan="3" class="noborder">
                          <strong>Party:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Client"/>
                        </td>
                      </tr>
                      <tr >
                        <td colspan="3" class="noborder">
                          <strong>Site:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Site"/>
                        </td>
                      </tr>
                      <tr >
                        <td colspan="3" class="noborder">
                          <strong>Driver:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Driver"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Cartage Paid:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Freight"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Additional Exp.:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Freight"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Vechicle No:</strong>
                          <xsl:text> </xsl:text><xsl:value-of select="Vehicle"/>
                        </td>
                      </tr>
                     
                    
                    </table>

                    <table class="table table-condensed;border-collapse:collapse;border"   cellspacing="0">
                      <tr class="headerRow">
                        <th  style="width:400px;border:0">Item</th>
                        <th class="text-center" style="border:0;width:10px;" >Qty</th>

                      </tr>
                      <xsl:for-each select="Items">
                        <tr  >
                          <td  class="noborder" style="border-right:none;border-top:none;">
                            <xsl:value-of select="Product"/>
                          </td>
                          <td class="text-center noborder" style="border-top:none;">
                            <xsl:value-of select="SentQty"/>
                          </td>

                        </tr>
                      </xsl:for-each>
                    </table>


                  </div>
                </xsl:for-each>
              </td>
            </tr>
          </table>

        </div>

      </body>
    </html>



  </xsl:template>
</xsl:transform>
