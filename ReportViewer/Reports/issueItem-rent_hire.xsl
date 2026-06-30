<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
                <td style="width: 40%;border-right:0px;border-bottom:0px; text-align: center;">DELIVERY CHALLAN</td>
                <td colspan="2" style="width: 30%; border: none;text-align:right;border-bottom:0px;" class="padding">
                  RETURNABLE
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
                      <td class="padding" style="width: 50%; height: 50px; border-bottom: none; border-left: none; vertical-align: top;"
                                    colspan="4">
                        <p>
                          Ship From: <xsl:value-of select="NewDataSet/Header/Client"/>
                          <br/>
                          <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Table/SiteState"/>
                        </p>

                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style="width: 50%; height: 100px; border-bottom: none; border-left: none; border-right: 0px; vertical-align: top;"
                                    colspan="2">
                        <b>Customer</b>
                        <br/>
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                          <tr>
                            <td class="noborder">
                              <p style="font-size:14px;">
                                <xsl:value-of select="NewDataSet/Header/Company"/>
                              </p>

                              <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                              <br />
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                            </td>
                          </tr>
                        </table>

                      </td>
                      <td class="padding" style="width: 50%; height: 100px; border-bottom: none; border-right: none; vertical-align: top;"
                                      colspan="2">
                        <b>Delivery Address</b>
                        <br/>
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                          <tr>
                            <td class="noborder">
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                              <br />
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyState"/>

                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style="border-top:0px;height:25px; border-bottom: 0px; border-right: 0px; border-left: none;">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteState"/>

                      </td>
                      <td class="padding" style="border-top:0px;border-left:none;border-bottom: 0px; border-right: 0px;">
                        <b>State Code</b>

                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>

                      </td>
                      <td class="padding" style="border-bottom: 0px; border-right: 0px;border-top:0px;">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillState"/>

                      </td>
                      <td class="padding" style="border-left:none;border-bottom: 0px; border-right: none;border-top:0px;">
                        <b>State Code</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>

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

                  <table id="items" style="width: 100%;" cellspacing="0"  >
                    <tr>
                      <th style="border-top: 0px;border-left:0px;width:60px;">
                        S.No
                      </th>
                      <th style="border-top: 0px;border-left:0px;width:250px">Item</th>
                      <th style="border-top: 0px;border-left:0px;width:150px;text-align:center">HSN Code</th>

                      <th style="border-top: 0px;border-left:0px;width:100px;text-align:center">Qty</th>
                      <th style="border-top: 0px;border-left:0px;width:60px;text-align:center">Rate</th>
                      <th style="border-top: 0px;border-left:0px;border-right:0px;width:100px;text-align:right">Amount</th>
                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr>
                        <td style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Product" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="HSNCode" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="SentQty" />
                        </td>
                        <td style="border-left:0px;border-top: 0px;text-align:center">
                          <xsl:value-of select="Rate" />
                        </td>
                        <td style="border-left:0px;border-right: 0px;border-top: 0px;text-align:right">
                          <xsl:value-of select="SubTotal" />
                        </td>
                      </tr>
                    </xsl:for-each>
                    <tr  >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        Sub Total
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <!--<xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>-->
                        <xsl:value-of select='format-number(NewDataSet/Table/ChallanSubTotal, "#.00")'/>
                      </td>
                    </tr>
                    <tr  >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        Freight
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <!--<xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>-->
                        <xsl:value-of select='format-number(NewDataSet/Table/Freight, "#.00")'/>
                      </td>
                    </tr>
                    <tr >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        Other Charges
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <!--<xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>-->
                        <xsl:value-of select='format-number(NewDataSet/Table/OtherChargeAmount, "#.00")'/>
                      </td>
                    </tr>
                    <tr  >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        IGST
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <xsl:value-of select='format-number(NewDataSet/Table/IGSTAmount, "#.00")'/>
                      </td>
                    </tr>
                    <tr >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        CGST
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <!--<xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>-->
                        <xsl:value-of select='format-number(NewDataSet/Table/CGSTAmount, "#.00")'/>
                      </td>
                    </tr>
                    <tr >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        SGST
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <xsl:value-of select='format-number(NewDataSet/Table/SGSTAmount, "#.00")'/>
                      </td>
                    </tr>
                    <tr  >
                      <td colspan="3" style="border:0px;border-right:Solid 1px;">

                      </td>
                      <td colspan="2" style="border-left:0px;border-top:0px;text-align:right;border-top:0px;">
                        Total
                      </td>

                      <td style="border-left:0px;border-right:0px;border-top:0px;text-align: right;">
                        <!--<xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>-->
                        <xsl:value-of select='format-number(NewDataSet/Table/ChallanTotal, "#.00")'/>
                      </td>
                    </tr>
                    <tr >
                      <td colspan="5" style="border-top:0px;border-bottom:0px;height: 100px; border-left:0px;border-right:0px;border-top:none;
                            vertical-align: top">
                        Rupees in Words<br/>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/Rupees"/>
                        </span>
                      </td>
                    </tr>
                  </table>

                </td>
                <td style="border-left:0px;border-top:0px;">
                  <div  >
                    <div style="text-align: center;height:100px;padding-top:10px;">
                      <b>Vehicle No</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Vehicle"/>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/VehicleRegNo"/>


                    </div>
                    <div style="text-align: center;height:100px;">
                      <b>Driver</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Driver"/>
                    </div>
                    <div style="text-align: center;height:100px;">
                      <b>Time</b>
                      <br/>

                    </div>
                    <div style="text-align: center;">
                      <b>Cartage</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/Freight"/>
                    </div>
                  </div>
                </td>
              </tr>

              <tr>
                <td colspan="4" style="border-bottom: none; border-left: none; border-right: none;border-top:0px;">
                  <div style="padding:10px; width:70%;">
                    <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Additional Information
                    </p>
                    <p>
                      <xsl:value-of select="NewDataSet/Header/Remarks"/>
                    </p>
                    <br/>
                    <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Terms and Conditions
                    </p>

                    <p>
                      <xsl:value-of select="NewDataSet/Header/Tnc"/>
                    </p>
                    <!--<ul style="list-style-type:decimal;font-size:12px;" >
                      <li class="ulitems">Material in brokent state will be charged separately</li>
                      <li  class="ulitems">Lost items will be charged separately</li>
                      <li  class="ulitems">The buyer acknowledges that they are responsible for inspecting the goods immediately upon receipt.</li>
                      <li  class="ulitems">The ownership of goods remains with the seller until full payment is made by the buyer.</li>
                    </ul>-->
                  </div>
                  <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                    <span style="font-size:14px; ">CUSTOMER SIGNATURE</span>
                  </div>
                  <div style="width: 400px; height:100px;float: right; padding: 5px 5px 0px 0px; text-align:right;">
                    <span style="width: 100%; padding-top:15px; float: left;font-size:14px; ">
                      For  <xsl:value-of select="NewDataSet/Header/Company"/>
                    </span>

                    <div style="font-size:14px; padding-top:65px;">AUTHORIZED SIGNAORY</div>
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
