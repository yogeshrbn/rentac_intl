<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        #customCSS
      </head>
      <body>
        <div id="printArea" class="quotation">

          <style>
            th{
            font-weight:bold;
            }
            table th td p {
            font-family:calibri;
            font-size: 14px;
            }
            .noborder {border:0px;

            }
            p {
            margin:0px;
            }
          
          </style>

          <table style="width: 100%" class="lineItemTable" cellpadding="0" cellspacing="0" >
            <tr>
              <td colspan="2" class="noborder">
                <img style="height:60px;max-width:250px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="d/data/Table/CompanyLogo"/>
                  </xsl:attribute>
                </img>
              </td>
              <td colspan="8" class="text-right noborder " style="vertical-align:top;">
                <h3 style="font-size:20px;" class="quotationTitle">
                  <xsl:if test="d/data/Table/QuotationType = 15">RENTAL</xsl:if>
                  <xsl:if test="d/data/Table/QuotationType = 16">CONTRACT</xsl:if>
                  <xsl:if test="d/data/Table/QuotationType = 17">SALES</xsl:if>
                  <xsl:choose>
                    <xsl:when test="d/data/Table/Category ='pi'">
                      PROFORMA INVOICE
                    </xsl:when>
                    <xsl:otherwise>
                      QUOTATION
                    </xsl:otherwise>
                  </xsl:choose>
                </h3>
                <h5>
                  <xsl:value-of select="d/data/Table/CompanyName" />
                </h5>
                <p>
                  <xsl:value-of select="d/data/Table/CompanyGST" />
                </p>
                <p>
                  <xsl:value-of select="d/data/Table/CompanyAddress1" />
                </p>
                <p>
                  <xsl:value-of select="d/data/Table/CompanyAddress1" />,<xsl:value-of select="d/data/Table/CompanyCity" />, Haryana -  <xsl:value-of select="d/data/Table/CompanyZipCode" />
                </p>
                <p>
                  Mobile :    <xsl:value-of select="d/data/Table/CompanyPhone" />
                </p>
                <p>
                  Email :   <xsl:value-of select="d/data/Table/CompanyEmail" />
                </p>
                <p>
                  Website : <xsl:value-of select="d/data/Table/CompanyWebsite" />
                </p>
              </td>
            </tr>
            <tr>
              <td colspan="5" style="width:50%;border:0px;">
                <p>
                  <strong>Bill To</strong>
                </p>

                <br />
                <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                GSTIN:  <xsl:value-of select="d/data/Table/PartyGST" /><br />
                <p>
                  <xsl:value-of select="d/data/Table/PartyName" />
                </p>
                <p>

                </p>
                <p>
                  Att:   <xsl:value-of select="d/data/Table/PartyContactPerson" />
                </p>
                <p>
                  <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                  <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />

                </p>
              </td>
              <td colspan="5" class="text-right" style="vertical-align:bottom;border:0px;">
                <p>
                  Date: <xsl:text> </xsl:text><xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/QuotationDate)"/>
                </p>
                <p>
                  Quotation Number : <xsl:text> </xsl:text><xsl:value-of select="d/data/Table/QuotationNumber" />
                </p>
                <p>
                  Grand Total (INR) : <xsl:text> </xsl:text><xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
                </p>
              </td>
            </tr>
            <tr>
              <td colspan="10" style="border-bottom:0px;padding-top:10px;padding-bottom:10px;">
                Dear <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyName" /> <xsl:text> </xsl:text><br/> Please find below a cost-breakdown for the quotation. Please consider this quotation,and do not hesitate to contact me with any questions.
                <br/> <br/> Many thanks,<br/><xsl:value-of select="d/data/Table/CompanyName" />
              </td>
            </tr>
            <tr>
              <th class="text-center"  style="width:10%;border-bottom:0px;border-right:0px;" >S.No</th>
              <th class="text-center" style="width:10%;border-bottom:0px;border-right:0px;">Item Code</th>
              <th  colspan="5" style="width:50%;border-bottom:0px;border-right:0px;">Item Details</th>
              <th class="text-center" style="width:10%;border-bottom:0px;border-right:0px;">Quantity</th>
              <th class="text-right"  style="width:10%;border-bottom:0px;border-right:0px;">Price</th>
              <th class="text-right" style="width:10%;border-bottom:0px;">Total</th>
            </tr>
            <xsl:for-each select="d/data/Table">
              <tr>
                <td class="text-center" style="border-bottom:0px;border-right:0px;">
                  <xsl:value-of select="position()" />
                </td>
                <td  class="text-center" style="border-bottom:0px;border-right:0px;">
                  <xsl:value-of select="HSNCode" />
                </td>
                <td style="border-bottom:0px;border-right:0px;" colspan="5">
                  <span style="width:100%;">
                    <b>
                      <xsl:value-of select="Item" />
                    </b>
                  </span>
                  <!--  <span style="width:100%;display:block"> -->
                  <p style="padding:0px;margin:0px;font-family:Calibri;font-size:12pt;white-space: preserve;white-space: pre-wrap;">
                    <xsl:value-of select="Description"/>
                  </p>
                  <!--  </span> -->

                
                </td>
                <td class="text-center" style="border-bottom:0px;border-right:0px;">
                  <xsl:value-of select="format-number(Quantity,'#.00')" />
                </td>
                <td  class="text-right" style="border-bottom:0px;border-right:0px;">
                  <xsl:value-of select="format-number(Rate,'#.00')" />
                </td>
                <td  class="text-right" style="border-bottom:0px;">
                  <xsl:value-of select="format-number(ItemSubTotal,'#.00')" />
                </td>
              </tr>
            </xsl:for-each>
            <tr>
              <td style=" padding:0px;border-right:0px;border-bottom:solid 1px; " colspan="7" rowspan="100%">
                <table style="width:100%;margin-top:0px;" class="lineItemTable">
                  <tr>
                    <td style="vertical-align:top;padding:10px;" class="noborder">
                      <div style="width:60%;float:left;">


                        <xsl:if test="d/Config/c/config[Key='printBankDetails']/Value='true'">
                          <ul style="list-style:none;">
                            <li>
                              <b>Bank Details</b>
                            </li>
                            <li>
                              Bank A/C No: <xsl:value-of select="d/data/Table/bankAccNumber"/>
                            </li>
                            <li>
                              Bank:  <xsl:value-of select="d/data/Table/bankName"/>
                            </li>
                            <li>
                              Branch Address:  <xsl:value-of select="d/data/Table/bankBranch"/>
                            </li>
                            <li>
                              IFSC Code: <xsl:value-of select="d/data/Table/IFSCCode"/>
                            </li>
                          </ul>
                        </xsl:if>
                        <i>Customer Acceptance (Sign here)</i>
                        <br />
                        <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>
                      </div>
                      <div style="width:40%;float:right;text-align:right;">
                        <img style="max-width:120px;margin-right:20px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="d/data/Table/QrCode"/>
                          </xsl:attribute>
                        </img>
                      </div>
                    </td>
                  </tr>
                </table>
              </td>
              <td class="text-right" colspan="2" style="border-top: solid 1px; border-right: 0px;border-bottom:0px;line-height:30px;">SubTotal</td>
              <td style="border-top: solid 1px;border-bottom:0px;"  class="text-right" >
                <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#.00')"/>
              </td>
            </tr>


            <xsl:if test="d/data/Table/charge1 > 0">
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">
                  Loading Charges
                </td>
                <td  class="text-right"  >
                  <xsl:value-of select="format-number(d/data/Table/charge1,'#.00')"/>
                </td>
              </tr>
            </xsl:if>
            <xsl:if test="d/data/Table/charge2 > 0">
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">
                  Un-loading Charges
                </td>
                <td  class="text-right"  >
                  <xsl:value-of select="format-number(d/data/Table/charge2,'#.00')"/>
                </td>

              </tr>
            </xsl:if>
            <xsl:if test="d/data/Table/charge3 > 0">
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">
                  Installation Charges
                </td>
                <td  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/charge3,'#.00')"/>
                </td>


              </tr>
            </xsl:if>
            <xsl:if test="d/data/Table/charge4 > 0">
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">
                  Un-Installation Charges
                </td>
                <td  class="text-right">
                  <xsl:value-of select="format-number(d/data/Table/charge4,'#.00')"/>
                </td>

              </tr>
            </xsl:if>
            <xsl:if test="d/data/Table/charge5 > 0">
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">
                  Other Charges
                </td>
                <td  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/charge5,'#.00')"/>
                </td>
              </tr>
            </xsl:if>
            <tr>
              <td colspan="2" class="text-right" style="border-bottom:0px;border-right:0px;line-height:30px;">
                <b> GST Tax (18%)</b>
              </td>
              <td   class="text-right" style="border-bottom:0px;" >
                <xsl:value-of select="format-number(d/data/Table/TaxAmount,'#.00')"/>
              </td>
            </tr>
            <tr>
              <td colspan="2" class="text-right" style="border-right:0px;line-height:30px;">
                <b>Total</b>
              </td>
              <td  class="text-right" >
                <xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
              </td>
            </tr>

          </table>


          <div style="margin-top:10px;">
            <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>
          </div>
          <br/>
          <div>
            <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
          </div>
        </div>
        <footer >
          <img style="max-width:100px;float:right;margin-right:20px;">
            <xsl:attribute name="src">
              <xsl:value-of select="d/data/Table/Signature"/>
            </xsl:attribute>
          </img>
          <br/>
          <div style="float:right;">
          <b>
            For <i>
              <xsl:value-of select="d/data/Table/CompanyName" />
            </i>
          </b></div>
        </footer>
      </body>
    </html>


  </xsl:template>
</xsl:transform>
