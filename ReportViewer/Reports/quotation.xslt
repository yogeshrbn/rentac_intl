<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template name="quotationTaxRowName">
    <xsl:choose>
      <xsl:when test="normalize-space(TaxName) != ''">
        <xsl:value-of select="TaxName"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="Name"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="printQuotationLineTaxes">
    <xsl:param name="cellTag">td</xsl:param>
    <xsl:for-each select="d/data/Table1">
      <tr>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:attribute name="style">border-right:0px;</xsl:attribute>
          <xsl:call-template name="quotationTaxRowName"/>
          <xsl:text> (</xsl:text>
          <xsl:value-of select="format-number(number(Rate), '#0.##')"/>
          <xsl:text>%)</xsl:text>
        </xsl:element>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:value-of select="format-number(number(Amount | TaxAmount), '#.00')"/>
        </xsl:element>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="printQuotationHeaderTaxes">
    <xsl:param name="cellTag">td</xsl:param>
    <xsl:if test="number(d/data/Table/FreightTax) &gt; 0">
      <tr>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:attribute name="style">border-right:0px;</xsl:attribute>
          Freight Tax
        </xsl:element>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:value-of select="format-number(number(d/data/Table/FreightTax), '#.00')"/>
        </xsl:element>
      </tr>
    </xsl:if>
    <xsl:if test="number(d/data/Table/chargesTax) &gt; 0">
      <tr>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:attribute name="style">border-right:0px;</xsl:attribute>
          Other Charges Tax
        </xsl:element>
        <xsl:element name="{$cellTag}">
          <xsl:attribute name="class">text-right</xsl:attribute>
          <xsl:value-of select="format-number(number(d/data/Table/chargesTax), '#.00')"/>
        </xsl:element>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template name="printQuotationLegacyGst">
    <xsl:param name="cellTag">td</xsl:param>
    <tr>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:attribute name="style">border-right:0px;</xsl:attribute>
        IGST
      </xsl:element>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#.00')"/>
      </xsl:element>
    </tr>
    <tr>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:attribute name="style">border-top: 0px; border-right: 0px;</xsl:attribute>
        SGST
      </xsl:element>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:attribute name="style">border-top:0px;</xsl:attribute>
        <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#.00')"/>
      </xsl:element>
    </tr>
    <tr>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:attribute name="style">border-top: 0px; border-right: 0px;</xsl:attribute>
        CGST
      </xsl:element>
      <xsl:element name="{$cellTag}">
        <xsl:attribute name="class">text-right</xsl:attribute>
        <xsl:attribute name="style">border-top:0px;</xsl:attribute>
        <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#.00')"/>
      </xsl:element>
    </tr>
  </xsl:template>

  <xsl:template name="printQuotationTaxSection">
    <xsl:param name="cellTag">td</xsl:param>
    <xsl:choose>
      <xsl:when test="count(d/data/Table1) &gt; 0">
        <xsl:call-template name="printQuotationLineTaxes">
          <xsl:with-param name="cellTag" select="$cellTag"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="printQuotationLegacyGst">
          <xsl:with-param name="cellTag" select="$cellTag"/>
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

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
              pre {
              background: none;
              overflow: hidden;
              border: none;
              font-size: 11pt;
              color: #000;
              font-family: arial;
              padding:0px;
              }            
          </style>
          <table style="width: 100%" cellpadding="0" cellspacing="0">
            <tr>
              <td style="border:0px;">
                <img style="height:60px;max-width:250px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="d/data/Table/CompanyLogo"/>
                  </xsl:attribute>
                </img>
                <br/><br/>

                <xsl:value-of select="d/data/Table/CompanyName" /><br />



                <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                GSTIN:<xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                <strong>Mobile:</strong><xsl:value-of select="d/data/Table/CompanyPhone" /> |  <strong>Email:</strong><xsl:value-of select="d/data/Table/CompanyEmail" />
                <br />
              </td>
              <td style=" text-align:right;vertical-align:top" class="noborder">
                <p class="quotationTitle" style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">
                  <xsl:if test="d/data/Table/QuotationType = 15">RENTAL</xsl:if>
                  <xsl:if test="d/data/Table/QuotationType = 16">CONTRACT</xsl:if>
                  <xsl:if test="d/data/Table/QuotationType = 17">SALES</xsl:if>
                  <xsl:text> </xsl:text>
                  <xsl:choose>
                    <xsl:when test="d/data/Table/Category ='pi'">
                      PROFORMA INVOICE
                    </xsl:when>
                    <xsl:otherwise>
                      QUOTATION
                    </xsl:otherwise>
                  </xsl:choose>
                  
                </p>
                <xsl:choose>
                  <xsl:when test="d/data/Table/Category ='pi'">
                    PI
                  </xsl:when>
                  <xsl:otherwise>
                    Quotation
                  </xsl:otherwise>
                </xsl:choose> #: <xsl:value-of select="d/data/Table/QuotationNumber" /><br />
                <xsl:choose>
                  <xsl:when test="d/data/Table/Category ='pi'">
                    PI
                  </xsl:when>
                  <xsl:otherwise>
                    Quotation
                  </xsl:otherwise>
                </xsl:choose> Date:  <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/QuotationDate)"/> <br />
              </td>
            </tr>
          </table>

          <xsl:if test="d/data/Table/QuotationType != 17">
            <table style="width:100%; border-collapse:collapse;" class="lineItemTable">
              <tr>
                <td colspan="2" class="subHeading1">Customer</td>
                <td colspan="4" class="subHeading1">Delivery Address</td>
              </tr>
              <tr>
                <td colspan="2">
                  <xsl:value-of select="d/data/Table/PartyName" /><br />
                  <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                  <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                  Phone: -   <xsl:value-of select="d/data/Table/PartyPhone" /><br />
                  GSTIN: <xsl:value-of select="d/data/Table/PartyGST" /><br />
                </td>
                <td colspan="4" style="vertical-align:top;">
                  <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                  <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />
                </td>

              </tr>
              <!--<xsl:if test="d/data/Table/QuotationType = 16">
                <tr>
                  <td colspan="2" class="subHeading1">Contract</td>
                  <td colspan="4">
                    Area: <xsl:value-of select="d/data/Table/Area" />
                    <xsl:text> </xsl:text>
                    <xsl:choose>
                      <xsl:when test="d/data/Table/MeasureType = 2">SQMTR</xsl:when>
                      <xsl:otherwise>SQFT</xsl:otherwise>
                    </xsl:choose>
                    <br />
                    Period: <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/From)" />
                    <xsl:text> to </xsl:text>
                    <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/To)" />
                    <br />
                    Line total:
                    <xsl:choose>
                      <xsl:when test="d/data/Table/LineTotalMode = 'area'">Qty × Area × Rate</xsl:when>
                      <xsl:otherwise>Qty × Rate</xsl:otherwise>
                    </xsl:choose>
                    <br />
                    Total Area (Qty × Area):
                    <xsl:value-of select="format-number(sum(d/data/Table[Item != '']/Quantity * d/data/Table[Item != '']/Area), '#.00')" />
                  </td>
                </tr>
              </xsl:if>-->
              <tr>

                <th style="width:40%;border-top:0px;">Product Name  Desc.</th>
                <xsl:if test="d/data/Table/QuotationType = 16">
                  <th class="text-center" style="width: 80px;border-top:0px;">Area</th>
                  <th class="text-center" style="width: 80px;border-top:0px;">Qty</th>
                  <th class="text-center" style="width: 90px;border-top:0px;">Total Area</th>
                  <th class="text-center" style="width: 100px;border-top:0px;">Rate</th>
                  <th class="text-center" style="width: 90px;border-top:0px;">From</th>
                  <th class="text-center" style="width: 90px;border-top:0px;">To</th>
                </xsl:if>
                <xsl:if test="d/data/Table/QuotationType != 16">
                  <th  class="text-center" style="width:100px;border-top:0px;">Quantity</th>
                  <th  class="text-center" style="width:100px;border-top:0px;">UOM</th>
                  <th class="text-center" style="width: 100px;border-top:0px;">Rate</th>
                </xsl:if>
                <xsl:if test="d/data/Table/QuotationType = 15">
                  <th class="text-center" style="width: 100px;border-top:0px;">Duration</th>
                </xsl:if>
                <th class="text-right" style="width: 100px;border-top:0px;" >Amount</th>
              </tr>
              <xsl:for-each select="d/data/Table">
                <tr>
                  <td>
                   
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

                  <xsl:if test="/d/data/Table/QuotationType = 16">
                    <td class="text-center">
                      <xsl:value-of select="Area" />
                    </td>
                    <td class="text-center">
                      <xsl:value-of select="Quantity" />
                    </td>
                    <td class="text-center">
                      <xsl:choose>
                        <xsl:when test="Area > 0">
                          <xsl:value-of select="format-number(Quantity * Area, '#.00')" />
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="format-number(Quantity * /d/data/Table[1]/Area, '#.00')" />
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                    <td class="text-center">
                      <xsl:value-of select="Rate" />
                    </td>
                    <td class="text-center">
                      <xsl:value-of select="util:DateToDDMMYYYY(From)" />
                    </td>
                    <td class="text-center">
                      <xsl:value-of select="util:DateToDDMMYYYY(To)" />
                    </td>
                  </xsl:if>
                  <xsl:if test="/d/data/Table/QuotationType != 16">
                    <td  class="text-center" >
                      <xsl:value-of select="Quantity" />
                    </td>
                    <td  class="text-center" >
                      <xsl:value-of select="Unit" />
                    </td>
                    <td  class="text-center" >
                      <xsl:value-of select="Rate" />
                    </td>
                  </xsl:if>
                  <xsl:if test="/d/data/Table/QuotationType = 15">
                    <td  class="text-center" >
                      <xsl:value-of select="duration" />
                    </td>
                  </xsl:if>
                  <td  class="text-right" >
                    <xsl:value-of select="util:FormatNumber(ItemSubTotal)" />
                  </td>
                </tr>
              </xsl:for-each>
              <tr>
                <td style=" padding:0px; vertical-align:bottom;" colspan="3" rowspan="20">
                  <table style="width:85%;margin-top:0px;">

                    <tr>
                      <td style="vertical-align:bottom;padding:0px;padding-bottom:50px;" class="noborder">
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
                          <!--<i>Customer Acceptance (Sign here)</i>
                          <br />
                          <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>-->
                        </div>
                        <div style="width:40%;float:right;text-align:right;">
                          <img style="max-width:100px;">
                            <xsl:attribute name="src">
                              <xsl:value-of select="d/data/Table/QrCode"/>
                            </xsl:attribute>
                          </img>
                        </div>
                      </td>
                    </tr>
                  </table>
                </td>
                <td class="text-right" colspan="2" style="border-top: 0px; border-right: 0px;">SubTotal</td>
                <td style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#.00')"/>

                </td>

              </tr>
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">Freight</td>
                <td  class="text-right">
                  <xsl:value-of select="format-number(d/data/Table/Freight,'#.00')"/>
                </td>
              </tr>
              <!--<tr>
              <th class="text-right" colspan="2"  style="border-right:0px;">Freight Tax</th>
              <th  class="text-right">
                <xsl:value-of select="format-number(d/data/Table/FreightTax,'#.00')"/>
              </th>
            </tr>-->
              <tr>
                <td class="text-right" colspan="2"  style="border-right:0px;">Other Charges</td>
                <td  class="text-right">
                  <xsl:value-of select="format-number(d/data/Table/OtherChargeAmount,'#.00')"/>
                </td>
              </tr>
              <!--<tr>
              <th class="text-right" colspan="2"  style="border-right:0px;">Charges Tax</th>
              <th  class="text-right">
                <xsl:value-of select="format-number(d/data/Table/chargesTax,'#.00')"/>
              </th>
            </tr>-->
              <xsl:call-template name="printQuotationTaxSection">
                <xsl:with-param name="cellTag" select="'td'"/>
              </xsl:call-template>
              <tr>
                <td class="text-right" colspan="2"  style="border-top: 0px; border-right: 0px;">Discount</td>
                <td style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#.00')" />
                </td>


              </tr>
              <tr>
                <td class="text-right" colspan="2"  style="border-top: 0px; border-right: 0px;">Total</td>
                <td style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
                </td>
              </tr>
            </table>

          </xsl:if>
          <xsl:if test="d/data/Table/QuotationType = 17">
            <table style="width:100%; border-collapse:collapse;">
              <tr>
                <td colspan="2" class="subHeading1">Customer</td>
                <td colspan="3" class="subHeading1">Delivery Address</td>
              </tr>
              <tr>
                <td colspan="2">
                  <xsl:value-of select="d/data/Table/PartyName" /><br />
                  <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                  <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                  GSTIN: <xsl:value-of select="d/data/Table/PartyGST" /><br />
                </td>
                <td colspan="3" style="vertical-align:top;">
                  <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                  <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />
                </td>

              </tr>
              <tr>

                <th style="width:50%;border-top:0px;">Product Name  Desc.</th>
                <th  class="text-center" style="width:100px;border-top:0px;">Quantity</th>
                <th  class="text-center" style="width:100px;border-top:0px;">UOM</th>
                <th class="text-center" style="width: 100px;border-top:0px;">Rate</th>
                <!--<th class="text-center" style="width: 100px;border-top:0px;">Duration</th>-->
                <th class="text-right" style="border-top:0px;" >Amount</th>
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
                    <xsl:value-of select="Unit" />
                  </td>
                  <td  class="text-center" >
                    <xsl:value-of select="Rate" />
                  </td>
                  <!--<td  class="text-center" >
                    <xsl:value-of select="duration" />
                  </td>-->
                  <td  class="text-right" >
                    <xsl:value-of select="ItemSubTotal" />
                  </td>
                </tr>
              </xsl:for-each>
              <tr>
                <td style=" padding:0px; vertical-align:bottom;" colspan="2" rowspan="20">
                  <table style="width:85%;margin-top:0px;">

                    <tr>
                      <td style="vertical-align:bottom;padding:0px;padding-bottom:50px;" class="noborder">
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
                        <i>Customer Acceptance (Sign here)</i>
                        <br />
                        <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>
                      </td>
                    </tr>
                  </table>
                </td>
                <th class="text-right" colspan="2" style="border-top: 0px; border-right: 0px;">SubTotal</th>
                <th style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#.00')"/>

                </th>

              </tr>
              <tr>
                <th class="text-right" colspan="2"  style="border-right:0px;">Freight</th>
                <th  class="text-right">
                  <xsl:value-of select="format-number(d/data/Table/Freight,'#.00')"/>
                </th>
              </tr>
              <!--<tr>
              <th class="text-right" colspan="2"  style="border-right:0px;">Freight Tax</th>
              <th  class="text-right">
                <xsl:value-of select="format-number(d/data/Table/FreightTax,'#.00')"/>
              </th>
            </tr>-->
              <tr>
                <th class="text-right" colspan="2"  style="border-right:0px;">Other Charges</th>
                <th  class="text-right">
                  <xsl:value-of select="format-number(d/data/Table/OtherChargeAmount,'#.00')"/>
                </th>
              </tr>
              <!--<tr>
              <th class="text-right" colspan="2"  style="border-right:0px;">Charges Tax</th>
              <th  class="text-right">
                <xsl:value-of select="format-number(d/data/Table/chargesTax,'#.00')"/>
              </th>
            </tr>-->
              <xsl:call-template name="printQuotationTaxSection">
                <xsl:with-param name="cellTag" select="'th'"/>
              </xsl:call-template>
              <tr>
                <th class="text-right" colspan="2"  style="border-top: 0px; border-right: 0px;">Discount</th>
                <th style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#.00')" />
                </th>
              </tr>
              <tr>
                <th class="text-right" colspan="2"  style="border-top: 0px; border-right: 0px;">Total</th>
                <th style="border-top:0px;"  class="text-right" >
                  <xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
                </th>
              </tr>
            </table>

          </xsl:if>
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
            <div style="text-align:right;width:250px;max-width:250px; float:right; margin-top:20px;margin-bottom:20px;margin-right:10px;">

              <p  >
                <img style="max-width:100px;margin-bottom:20px;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="d/data/Table/Signature"/>
                  </xsl:attribute>
                </img>
                <br/>
                <b>
                  For <i>
                    <xsl:value-of select="d/data/Table/CompanyName" />
                  </i>
                </b>
              </p>
            </div>
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

      </body>
    </html>


  </xsl:template>
</xsl:transform>
