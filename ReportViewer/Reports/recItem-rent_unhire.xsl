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
          .padding {
          padding-left:5px;
          }
          td {
          line-height:25px !important;
          height:25px !important;
          }
          th {
          line-height:25px !important;
          height:25px !important;
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
                <td style="width: 30%;border-right:0px;border-bottom:0px;padding-left:3px;padding-right:3px;"   class="padding">
                  GST No: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td  style="width: 40%;border-right:0px;border-bottom:0px; text-align: center;">DELIVERY CHALLAN</td>
                <td style="width: 30%; border: none;text-align:right;border-bottom:0px;" class="padding">
                  RETURNABLE
                </td>
              </tr>
              <tr class="padding">
                <td style="border-top:none !important; text-align: center;border-bottom:0px;" colspan="3" class="padding">
                  <strong style="margin-bottom:10px;">
                    <xsl:value-of select="NewDataSet/Header/Company"/>
                  </strong>
                  <div style="padding:10px; padding-top:15px;">
                    <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    <p style="margin-top:3px;">
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                    </p>
                    <p style="margin-top:3px;">
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </p>
                  </div>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="border-top: 0px; border-left: 0px; border-right: 0px;">
                  <table style="width: 100%"   cellspacing="0">
                    <tr class="padding">
                      <td class="padding" style="width: 50%; height: 100px;padding-left:3px; border-bottom: none; border-left: none; border-right: 0px; vertical-align: top;"
                                    colspan="2">
                        <b>Shipped From</b>
                        <br/>
                        <table border="0" width="100%"  cellspacing="0">
                          <tr>
                            <td class="noborder">
                              <xsl:value-of select="NewDataSet/Header/Company"/>
                              <br/>

                              <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                              <xsl:text> </xsl:text>

                              <br/>

                              <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                            </td>
                          </tr>
                        </table>
                      </td>
                      <td class="padding" style="width: 50%; height: 100px;padding-left:3px; border-bottom: none; border-right: none; vertical-align: top;"
                                     colspan="2">
                        <b>Ship To</b>
                        <br/>
                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                          <tr>
                            <td class="noborder">
                              <xsl:value-of select="NewDataSet/Header/Client"/>
                              <br/>
                              <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                              <br/>
                              <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                              <xsl:text> </xsl:text>
                              <xsl:value-of select="NewDataSet/Table/SiteState"/>

                            </td>
                          </tr>
                        </table>
                      </td>

                    </tr>
                    <!--<tr class="padding" style="border-top:0px;height:25px; border-bottom: 0px; border-right: 0px;  display:0px;">
                      <td class="padding" style="border-top:0px;height:25px; border-bottom: 0px; border-right: 0px; border-left: none;padding-left:3px;">
                        <b>State</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/SiteState"/>
                        </span>
                      </td>
                      <td  class="padding" style="border-top:0px;border-left:none;border-bottom: 0px; border-right: 0px;padding-left:3px;">
                        <b>State Code</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>
                        </span>
                      </td>
                      <td class="padding" style="border-bottom: 0px; border-right: 0px;border-top:0px;padding-left:3px;">
                        <b>State</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/BillState"/>
                        </span>
                      </td>
                      <td class="padding" style="border-left:none;border-bottom: 0px; border-right: none;border-top:0px;padding-left:3px;">
                        <b>State Code</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>
                        </span>
                      </td>
                    </tr>-->
                    <tr class="padding">
                      <td   style="width: 50%;height:25px;border-top:0px;   border-bottom: 0px; border-right: 0px;padding-left:3px;padding-right:3px;" colspan="2">
                        <b>Receipt Number:</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/GRN"/>
                        </span>
                      </td>
                      <td  style="width: 50%; border-top:0px;border-bottom: 0px; border-right: none;padding-left:3px;" colspan="2">
                        <b>Receipt Date:</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/ReceivingDate"/>
                        </span>
                      </td>

                    </tr>
                  </table>
                </td>
              </tr>
              <tr>
                <td colspan="2" style="border-top:0px;" valign="top">


                  <table id="items" style="width: 100%"  cellspacing="0">
                    <tr>
                      <th style="border-top: 0px;border-left:0px;width:60px;padding-left:3px;padding-right:3px;" class="padding">
                        S.No
                      </th>
                      <th style="border-top: 0px;border-left:0px;width:250px;padding-left:3px;padding-right:3px;">Item</th>
                      <th style="border-top: 0px;border-left:0px;width:150px;text-align:center">HSN Code</th>
                      <th style="border-top: 0px;border-left:0px;width:150px;text-align:center">Qty</th>
                      <th style="border-top: 0px;border-left:0px;border-right:0px;width:80px;padding-left:3px;padding-right:3px;">Break</th>

                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr>
                        <td  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Item" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="HSNCode" />
                        </td>
                        <td  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Quantity" />
                        </td>
                        <td  style="border-top: 0px;border-left:0px;border-right:0px;">
                          <xsl:value-of select="Breakage" />
                        </td>

                      </tr>
                    </xsl:for-each>
                    <tr class="headerRow">
                      <td colspan="3" style="text-align: right;border-top:0px;border-left:0px;">
                        Total
                      </td>
                      <td style="text-align: right;border-top:0px;border-left:0px;">
                        <xsl:value-of select="sum(NewDataSet/Table/Quantity)"/>
                      </td>
                      <td style="text-align: right;border-top:0px;border-left:0px;border-right:0px;">

                      </td>

                    </tr>

                  </table>



                </td>
                <td style="border-left: 0px; border-top: 0px; border-right: none; border-bottom: none;">
                  <div style="text-align: center; padding: 30px;">
                    <b>Sender</b>
                    <br/>
                    <xsl:value-of select="NewDataSet/Header/Sender"/>
                  </div>
                  <div style="text-align: center; padding: 40px;">
                    <b>Receiver</b>
                    <br/>
                    <xsl:value-of select="NewDataSet/Header/Receiver"/>
                  </div>
                  <div style="text-align: center; padding: 20px;">
                    <b>Time</b>
                    <br/>

                  </div>

                </td>
              </tr>
              <tr>
                <td colspan="3" style="border-bottom: none; border-left: none; border-right: none;border-top:0px;">
                  <div style="padding:10px; width:70%;">
                    <p style="margin-bottom:20px;line-height:30px;text-decoration:underline;">
                      Terms and Conditions
                    </p>

                    <ul style="list-style-type:decimal;font-size:12px;" >
                      <li class="ulitems">Material in brokent state will be charged separately</li>
                      <li  class="ulitems">Lost items will be charged separately</li>
                      <li  class="ulitems">The buyer acknowledges that they are responsible for inspecting the goods immediately upon receipt.</li>
                      <li  class="ulitems">The ownership of goods remains with the seller until full payment is made by the buyer.</li>
                    </ul>
                  </div>
                  <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">


                    <span style="font-size:14px;font-weight:bold;">CUSTOMER SIGNATURE</span>
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
