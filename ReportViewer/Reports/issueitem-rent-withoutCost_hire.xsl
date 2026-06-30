<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>

          .noborder {border:0px;

          }
          .nopadding {
          padding:0px;
          line-height: 20px !important;
          }
          .ulitems {line-height:15px;}
          .tnc p,li {
          padding:0px;
          margin:0px;
          }
          .detailsRow td {padding:5px; !important;}
        </style>

      </head>

      <body>

        <div id="printArea">
          <img style="height:60px;max-width:100px;margin-bottom:0.2in;">
            <xsl:attribute name="src">
              <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
            </xsl:attribute>
          </img>

          <div id="container" class="container" style="padding-top:10px;">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <tr  class="padding" style="height:25px;padding:3px;">
                <td style="width: 30%;border-right:0px;border-bottom:0px;"   class="padding">
                  GST No: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td style="width: 40%;border-right:0px;border-left:0px;border-bottom:0px; text-align: center;">
                  Material Outward Slip (Rental)
                </td>
                <td colspan="2" style="width: 30%;border-left:0px; border: none;text-align:right;border-bottom:0px;" class="padding">
                  <!--<xsl:choose>
						<xsl:when test="count(NewDataSet/Config/root[Key = 'diveryChallanText'])> 0">
							<xsl:value-of select="NewDataSet/Config/root[Key='diveryChallanText']/Value" />
						</xsl:when>
						<xsl:otherwise>
							RETURNABLE
						</xsl:otherwise>
					</xsl:choose>-->
                  On Hire Only
                </td>
              </tr>
              <tr class="padding">
                <td style="border-top:none !important; text-align: center;border-bottom:0px;" colspan="4" class="padding">

                  <p style="font-size:18px;">
                    <xsl:value-of select="NewDataSet/Header/Company"/>
                  </p>


                  <p>
                    <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                    <br />
                    <xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    <xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                    <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                    <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    <br />
                    Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                    <br />
                    Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>

                  </p>
                </td>

              </tr>
              <tr>
                <td colspan="4" style="border-top: 0px; border-left: 0px; border-right: 0px;">
                  <table style="width: 100%"  cellspacing="0">

                    <tr >
                      <td class="padding" style="width: 50%; border-bottom: none; border-left: none;
                          border-right: 0px; vertical-align: top;"
                                    colspan="2">


                        <div style="padding:0; width:100%;">
                          <b >Ship From</b>
                          <br/>
                          <table border="0" style="margin-top:20px;" width="100%" cellpadding="0" cellspacing="0">
                            <tr>
                              <td class="noborder">
                                <xsl:value-of select="NewDataSet/Header/Client"/>
                              </td>
                            </tr>
                            <tr>
                              <td class="noborder">
                                <xsl:value-of select="NewDataSet/Header/SiteAddress"/>
                                <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                                <xsl:text> </xsl:text>
                                <xsl:value-of select="NewDataSet/Table/SiteState"/>
                              </td>
                            </tr>
                          </table>
                        </div>

                      </td>
                      <td class="padding" style="width: 50%; border-bottom: none; border-right: none; vertical-align: top;"
                                      colspan="2">
                        <b>Delivery Address</b>
                        <br/>
                        <table style="width:100%;margin-top:5px; border-collapse:collapse;" cellspacing="0" cellpadding="2">
                          <tr>
                            <td class="noborder">
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                              <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,<xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                              ,<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>
                            </td>
                          </tr>                         
                          <tr>
                            <td class="noborder">
                              GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style="border-top:0px;height:25px; border-bottom: 0px; border-right: 0px; border-left: none;">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillState"/>
                      </td>
                      <td class="padding" style="border-top:0px;border-left:none;border-bottom: 0px; border-right: 0px;">
                        <b>State Code</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>
                      </td>
                      <td class="padding" style="border-bottom: 0px; border-right: 0px;border-top:0px;">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteState"/>
                      </td>
                      <td class="padding" style="border-left:none;border-bottom: 0px; border-right: none;border-top:0px;">
                        <b>State Code</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>

                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style="width: 50%;height:25px; border-left: none; border-bottom: 0px; border-right: 0px;" colspan="2">
                        <b>Challan Number:</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>

                      </td>
                      <td class="padding" style="width: 50%; border-bottom: 0px; border-right: none;" colspan="2">
                        <b>Challan Date:</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/StartDate"/>

                      </td>

                    </tr>
                  </table>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="border-top:0px;" valign="top">

                  <table id="items" style="width: 100%;" cellspacing="0" class="itemsTable" >
                    <tr>
                      <th style="border-top: 0px;border-left:0px;width:60px;">
                        S.No
                      </th>
                      <th style="border-top: 0px;border-left:0px;width:310px">Item</th>
                      <th style="border-top: 0px;border-left:0px;width:150px;text-align:center">HSN/SAC Code</th>

                      <th style="border-top: 0px;border-left:0px;border-right: 0px;width:100px;text-align:center">Qty</th>
                      <!--<th style="border-top: 0px;border-left:0px;width:60px;text-align:center">Rate</th>
                      <th style="border-top: 0px;border-left:0px;border-right:0px;width:100px;text-align:right">Amount</th>-->
                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr class="detailsRow">
                        <td style="border-top: 0px;border-left:0px;text-align:center;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Product" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="SacHSNCode" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;border-right: 0px;text-align:center">
                          <xsl:value-of select="SentQty" />
                        </td>
                        <!--<td style="border-left:0px;border-top: 0px;text-align:center">
                          <xsl:value-of select="Rate" />
                        </td>
                        <td style="border-left:0px;border-right: 0px;border-top: 0px;text-align:right">
                          <xsl:value-of select="SubTotal" />
                        </td>-->
                      </tr>
                    </xsl:for-each>
                    <tr  class="detailsRow">

                      <td colspan="3" style="border-top: 0px; border-left:0px;text-align:right">
                        <b>Total</b>
                      </td>
                      <td style="border-top: 0px;border-left:0px;border-right: 0px;text-align:center">
                        <b>
                          <xsl:value-of select="sum(NewDataSet/Table/SentQty)"/>
                        </b>
                      </td>
                    </tr>
                    <tr  class="detailsRow">
                      <td colspan="4"  style="border-top: 0px;border-left:0px;border-bottom:0px;">
                        Quantity In Words: <xsl:value-of select="util:QtyToWords(sum(NewDataSet/Table/SentQty))"/>
                      </td>
                    </tr>
                  </table>

                </td>
                <td style="border-left:0px;border-top:0px;">
                  <div  >
                    <div style="text-align: center;height:50px;padding-top:10px;">
                      <b>Vehicle No</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Vehicle"/>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/VehicleRegNo"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>Driver</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Driver"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>LR Number</b>
                      <br/>
                      <!--<xsl:value-of select="NewDataSet/Table/Driver"/>-->
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>CR Number</b>
                      <br/>
                      <!--<xsl:value-of select="NewDataSet/Table/Driver"/>-->
                    </div>
                    <!--<div style="text-align: center;height:100px;">
                      <b>Time</b>
                      <br/>

                    </div>-->
                    <div style="text-align: center;height:50px;">
                      <b>Weight (Approx)</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Weight"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>Goods Value (Approx)</b>
                      <br/>

                      <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                    </div>
                    <!--<div style="text-align: center;">
                      <b>Cartage</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/Freight"/>
                    </div>-->
                  </div>
                </td>
              </tr>

              <tr  class="detailsRow">
                <td colspan="4" style="border-bottom: none; border-left: none; border-right: none;border-top:0px;">
                  <div style="padding:10px; width:70%;">
                    <!--<p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Additional Information
                    </p>
                    <p>
                      <xsl:value-of select="NewDataSet/Header/Remarks" disable-output-escaping="yes"/>
                    </p>-->
                    <!--<br/>-->
                    <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Terms and Conditions
                    </p>
                    <div style="font-size:8pt;">
                      <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                    </div>
                  </div>
                  <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                    <span style="font-size:14px; ">CUSTOMER SIGNATURE</span>
                  </div>
                  <div style="width: 400px; height:100px;float: right; padding: 5px 5px 0px 0px; text-align:right;">
                    <p style="width: 100%; padding-top:15px; float: left;font-size:12pt; ">
                      For  <xsl:value-of select="NewDataSet/Header/Company"/>
                      <br/>
                      <img style="height:40px;max-width:100px;float:right;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="NewDataSet/Header/Signature"/>
                        </xsl:attribute>
                      </img>
                      <br/>
                      AUTHORIZED SIGNAORY
                    </p>
                  </div>
                </td>
              </tr>
            </table>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
