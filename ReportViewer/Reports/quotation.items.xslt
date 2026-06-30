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

          <h3 style="text-align:center;width:100%;">Annexure - 1 </h3>
          <div style="width:100%">
            <div style="width:100%;float:left;">
             
            
              <table style="width:100%;" cellpadding="0" cellspacing="0">
                <tr>
                  <td style="text-align:center;border-right:0px;font-weight:bold">Sr.No</td>
                  <td style="border-right:0px;font-weight:bold;width:250px;">Name Of Item</td>
                  <td style="border-right:0px;font-weight:bold;width:50px;">UOM</td>
                  <td style="border-right:0px;font-weight:bold;width:100px;text-align:right;">Cost</td>
                  <td style="border-right:0px;font-weight:bold;width:250px;">Name Of Item</td>
                  <td style="border-right:0px;font-weight:bold;width:50px;">UOM</td>
                  <td style="font-weight:bold;width:100px;text-align:right;">Cost</td>
                </tr>
                <xsl:for-each select="items/data">
           
                  <tr>
                    <td style="border-top:0px;text-align:center;border-right:0px;">
                      <xsl:value-of select="position()" />
                    </td>
                    <td style="border-right:0px;border-top:0px;">
                      <xsl:value-of select="name1" />
                    </td>
                    <td style="border-right:0px;border-top:0px;">
                      <xsl:value-of select="unit1" />
                    </td>
                    <td style="border-right:0px;border-top:0px;text-align:right;">
                      <xsl:value-of select="cost1" />
                    </td>

                    <td style="border-right:0px;border-top:0px;width:250px;">
                      <xsl:value-of select="name2" />
                    </td>
                    <td style="border-right:0px;border-top:0px;">
                      <xsl:value-of select="unit2" />
                    </td>
                    <td style="border-top:0px;text-align:right;">
                      <xsl:value-of select="cost2" />
                    </td>

                  </tr>
                </xsl:for-each>
              </table>
            </div>
         
          </div>



        </div>
      </body>
    </html>


  </xsl:template>
</xsl:transform>
