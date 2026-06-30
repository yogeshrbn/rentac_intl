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
          line-height:20px !important;
          height:20px !important;
          font-size:11px;
          }
          th {
          line-height:20px !important;
          height:20px !important;
          }
          .ulitems {line-height:15px;}
          .line-item {
          font-size:12px;
          }
          ul{
          margin-left:0px;
          }
          ul li {
          line-height:15px;
          padding-left:0px;
          margin-left:0px;
          }
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
                <td style="width: 25%;border-right:0px;border-bottom:0px;padding-left:3px;padding-right:3px;"   class="padding">
                  <span style="font-weight:bold;">GST No:</span>
                  <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td  style="width: 55%;border-right:0px;border-bottom:0px; text-align: center;font-weight:bold;">
                  <xsl:value-of select="NewDataSet/Config/NewDataSet/Table1[Key = 'diveryChallanText']/Value"/>
                </td>
                <td style="width: 20%; border: none;text-align:right;border-bottom:0px;font-weight:bold;" class="padding">
                  <xsl:value-of select="NewDataSet/Config/NewDataSet/Table1[Key = 'returnableText']/Value"/>
                </td>
              </tr>
              <tr class="padding">
                <td style="border-top:none !important; text-align: center;border-bottom:0px;pading-bottom:0px;" colspan="3" class="padding">
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
                      <td class="padding" style="width: 50%; height: 100px;text-align:left;
                          border-bottom: none; border-left: none; border-right: 0px; vertical-align: top;"
                                    colspan="2">
                        <b>Shipped From</b>
                        <ul style="list-style-type:none;padding-left: 0px !important;
                            margin:0px !important;padding:0px !important;">
                          <li>
                            <xsl:value-of select="NewDataSet/Header/Client"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteState"/>
                          </li>
                        </ul>

                      </td>
                      <td class="padding" style="width: 50%; height: 100px;padding-left:3px; border-bottom: none; border-right: none; vertical-align: top;"
                                     colspan="2">
                        <b>Ship To</b>
                        <br/>
                        <ul style="list-style-type:none;padding-left: 0px;margin-left:0px;margin-bottom:0px;
                            padding-bottom:0px;">
                          <li>
                            <xsl:value-of select="NewDataSet/Header/Company"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                            <xsl:text> </xsl:text>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                          </li>
                        </ul>
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


                  <table id="items" style="width: 100%" border="0"  cellspacing="0">
                    <tr>
                      <th style="width:100px;border-top: 0px;border-left:0px;padding-left:3px;padding-right:3px;" class="padding">
                        S.No
                      </th>
                      <th style="width:500px;border-top: 0px;border-left:0px;padding-left:3px;padding-right:3px;">Item</th>
                      <th style="width:150px;border-top: 0px;border-left:0px;text-align:center">HSN/SAC</th>
                      <th style="width:80px;border-top: 0px;border-left:0px;text-align:center">Qty</th>

                      <th style="width:100px;border-top: 0px;border-left:0px;border-right:0px;width:150px;text-align:center">
                        Rate
                      </th>

                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr>
                        <td class="line-item"  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td  class="line-item"  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Item" />
                        </td>
                        <td  class="line-item" style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="HSNCode" />
                        </td>
                        <td  class="line-item"  style="border-top: 0px;border-left:0px;text-align:center">

                          <xsl:value-of select="Quantity" />
                        </td>


                        <td style="border-left:0px;border-top:0px;border-right:0px;text-align:center">
                          <xsl:value-of select="Rate" />
                        </td>

                      </tr>
                    </xsl:for-each>
                    <tr class="headerRow">
                      <td colspan="3" style="text-align: right;border-top:0px;border-left:0px;">
                        Total
                      </td>
                      <td style="text-align: center;border-top:0px;border-left:0px;">
                        <xsl:value-of select="sum(NewDataSet/Table/Quantity)"/>
                      </td>
                      <td style="border-top:0px;border-left:0px;border-right:0px;">
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
                      <xsl:value-of select="NewDataSet/Header/Remarks" disable-output-escaping="yes"/>
                    </p>
                    <p style="margin-bottom:20px;line-height:30px;text-decoration:underline;">
                      <xsl:value-of select="NewDataSet/Header/tnc" disable-output-escaping="yes"/>
                    </p>


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
