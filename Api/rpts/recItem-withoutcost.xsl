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
          .addresstable  td {
          padding-top:0px !important;
          padding-bottom:0px !important;
          padding-left:0px !important;
          padding-right:0px !important;
          }
          .ship-address-table {
          border-collapse: collapse;
          }
          .ship-address-table td {
          padding: 0 !important;
          }
          .detailsRow td {padding:5px; !important;}
          @media print {
          ol li {
              padding:0px;
              padding-bottom:0px;
              margin:0px;
              margin-bottom:0px;
              }
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
                <td style="width: 25%;border-right:0px;border-bottom:0px;padding-left:3px;padding-right:3px;border-right:0px; "   class="padding">
                  <span style="font-weight:bold;">GST No:</span>
                  <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td  style="width: 55%;border-right:0px;border-bottom:0px;border-right:0px;border-left:0px;  text-align: center;font-weight:bold;">
                  Material Inward Slip (Rental)
                </td>
                <td style="width: 20%; border: none;text-align:right;border-bottom:0px;font-weight:bold;border-left:0px;" class="padding">
                  ON Hire Only
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
                    <p style="margin-top:3px;">
                      <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    </p>
                    <p style="margin-top:3px;">
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      ,
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </p>
                  </div>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="border-top: 0px; border-left: 0px; border-right: 0px;">
                  <table style="width: 100%"   cellspacing="0">

                    <tr class="padding">
                      <td class="padding" style="width: 50%;text-align:left;
                          border-bottom: none; border-left: none; border-right: 0px; vertical-align: top;"
                                    colspan="2">
                        <span style="font-size:12pt;font-weight:bold;">Shipped From</span>
                        <br/>
                        <div style="width:100%;padding:5px;0px;">
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            <xsl:value-of select="NewDataSet/Header/Client"/>
                          </div>
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteState"/>
                          </div>
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            GSTIN: <xsl:value-of select="NewDataSet/Header/ClientGSTNo" />
                          </div>

                        </div>
                      </td>
                      <td class="padding" style="width: 50%; padding-left:3px; border-bottom: none;
                          border-right: none; vertical-align: top;"
                                     colspan="2">

                        <span style="font-size:12pt;font-weight:bold;">Ship To</span>
                        <br/>
                        <div style="width:100%;padding:5px;0px;">
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            <xsl:value-of select="NewDataSet/Header/Company"/>
                          </div>
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                            <xsl:text> </xsl:text>
                          </div>
                          <div style="width:100%;padding:5px 0px 0px 0px;">
                            <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyState"/>

                          </div>
                        </div>
                      </td>
                    </tr>
                    <tr class="padding">
                      <td   style="width: 50%;height:25px;border-top:0px;   border-bottom: 0px; border-right: 0px;padding-left:3px;padding-right:3px;" colspan="2">
                        <b>Challan Number:</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/GRN"/>
                        </span>
                      </td>
                      <td  style="width: 50%; border-top:0px;border-bottom: 0px; border-right: none;padding-left:3px;" colspan="2">
                        <b>Challan Date:</b>
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
                      <th style="width:50px;border-top: 0px;border-left:0px;padding-left:3px;padding-right:3px;" class="padding">
                        S.No
                      </th>
                      <th style="width:310px;border-top: 0px;border-left:0px;padding-left:3px;padding-right:3px;">Item</th>
                      <th style="width:80px;border-top: 0px;border-left:0px;text-align:center">HSN/SAC</th>
                      <th style="width:50px;border-top: 0px;border-left:0px;text-align:center;">Qty</th>

                      <th style="width:270px;border-top: 0px;border-left:0px;border-right:0px;width:150px;text-align:center">
                        Remarks
                      </th>

                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr>
                        <td class="line-item padding"  style="border-top: 0px;border-left:0px;text-align:center;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td  class="line-item padding"  style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Item" />
                        </td>
                        <td  class="line-item padding" style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="HSNCode" />
                        </td>
                        <td  class="line-item padding"  style="border-top: 0px;border-left:0px;text-align:center;">

                          <xsl:value-of select="Quantity" />
                        </td>


                        <td style="border-left:0px;border-top:0px;border-right:0px;text-align:center">
                          <xsl:value-of select="Remarks" />
                        </td>

                      </tr>
                    </xsl:for-each>
                    <tr class="headerRow">
                      <td colspan="3" class="padding" style="text-align: right;border-top:0px;border-left:0px;">
                        Total
                      </td>
                      <td  class="padding" style="text-align: center;border-top:0px;border-left:0px;border-right:0px;">
                        <xsl:value-of select="sum(NewDataSet/Table/Quantity)"/>
                      </td>
                      <td style="border-top:0px;border-left:0px;border-right:0px;">
                      </td>
                    </tr>
                    <tr  class="detailsRow">
                      <td colspan="5"  style="border-top: 0px;border-left:0px;border-bottom:0px;">
                        Quantity In Words: <xsl:value-of select="util:QtyToWords(sum(NewDataSet/Table/Quantity))"/>
                      </td>
                    </tr>
                  </table>
                </td>
                <td style="border-left: 0px; border-top: 0px; border-right: none; border-bottom: none;">
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
                      <xsl:value-of select="NewDataSet/Table/LRNumber"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>CR Number</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/CRNumber"/>
                    </div>
                    <!--<div style="text-align: center;height:100px;">
                      <b>Time</b>
                      <br/>

                    </div>-->
                    <div style="text-align: center;height:50px;">
                      <b>Weight (Approx)</b>
                      <br/>
                      <xsl:value-of select="sum(NewDataSet/Table/TotalWeight)"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>Goods Value (Approx)</b>
                      <br/>
                      <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                    </div>

                  </div>

                </td>
              </tr>
              <tr>
                <td colspan="3" style="border-bottom: none; border-left: none; border-right: none;border-top:0px;">
                  <div style="padding:10px; width:70%;">
                    <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Terms and Conditions
                    </p>
                    <div style="margin-bottom:20px;font-size:9pt;line-height:15px;">
                      <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                    </div>

                  </div>
                  <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                    <span style="font-size:14px;font-weight:bold;">CUSTOMER SIGNATURE</span>
                  </div>
                  <div style="width: 400px;float: right; padding: 5px 5px 0px 0px; text-align:right;">
                    <span style="width: 100%; padding-top:15px; float: left;font-size:14px; font-weight:bold;">
                      For  <xsl:value-of select="NewDataSet/Header/Company"/>
                    </span>
                    <br/>
                    <img style="height:60px;max-width:100px;float:right;">
                      <xsl:attribute name="src">
                        <xsl:value-of select="NewDataSet/Header/Signature"/>
                      </xsl:attribute>
                    </img>

                    <div style="font-size:14px; padding-top:10px;font-weight:bold;">AUTHORIZED SIGNAORY</div>
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
