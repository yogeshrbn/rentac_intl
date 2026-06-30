<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          body {
          font-size:10pt;
          font-family:arial;
          height: 100vh;
          }
          .main-sheet {  height: 100%;display: table; width: 100%; border-collapse: collapse; table-layout: fixed; }
          .main-sheet td, .main-sheet th { border: solid 1px #000; padding: 5px;font-size:12pt; }
          .main-sheet tr.spacer-row td { height: 100%; vertical-align: top; border: solid 1px #000; }
          h2{margin:0px;padding:0px;}
          p{margin:0px}
          ul{
          list-style:none;
          }
          ul li {

          padding-left:0px;
          margin-left:0px;
          padding-top:3px;
          font-size:11pt;
          }

          @media print {

          p{margin:0px !important;}
          h2{margin:0px;padding:0px; !important;}

          body {
          font-size:10pt;
          font-family:arial;
          height: 100vh;
          }
          .noborder {border:0px;

          }
          td {
          border:solid 1px;
          padding:5px;
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
          .subheading {font-size:11pt;font-weight:bold;}
          ul{
          list-style:none;
          }
          ul li {

          padding-left:0px;
          margin-left:0px;
          padding-top:3px;
          font-size:11pt;
          }
          .printArea {
          height: 335mm; /* A4 (297mm) - top/bottom margins (20mm each) */
          }
          .main-sheet { width: 100%; border-collapse: collapse; table-layout: fixed; }
          .main-sheet td, .main-sheet th { border: solid 1px #000; padding: 5px; }
          .main-sheet tr.spacer-row td { height: 100%; vertical-align: top; border: solid 1px #000; }
          }
        </style>
      </head>

      <body>

        <div id="printArea" class="printArea">
          <img style="height:60px;max-width:100px;margin-bottom:0.1in;">
            <xsl:attribute name="src">
              <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
            </xsl:attribute>
          </img>

          <div id="container" class="container" style="padding-top:10px;">

            <table class="main-sheet" style="width: 100%;" border="0px"  cellspacing="0">
              <tr  class="padding" style="height:25px;padding:3px;">
                <td style="width: 25%;border-right:0px;border-bottom:0px;padding-left:3px;padding-right:3px;border-right:0px; "   class="padding">
                  <span style="font-weight:bold;">GST No:</span>
                  <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td  style="width: 55%;border-right:0px;border-bottom:0px;border-right:0px;border-left:0px;  text-align: center;font-weight:bold;">
                  Material Inward Slip (Contract)
                </td>
                <td style="width: 20%; border: none;text-align:right;border-top:solid 1px;;font-weight:bold;border-right:solid 1px;" class="padding">
                  ON Hire Only
                </td>
              </tr>
              <tr >
                <td   colspan="3" style="text-align:center;">
                  <h2 style="margin:0px;">
                    <xsl:value-of select="NewDataSet/Header/Company"/>
                  </h2>
                  <div style="padding:5px;margin:0px;">
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
                <td colspan="3" style="border-top: 0px; padding:0px;">
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
                      <td   style="width: 50%;height:25px;border-top:0px;border:0px;border-top:solid 1px;  padding-left:3px;padding-right:3px;" colspan="2">
                        <b>Challan Number:</b>
                        <span>
                          <xsl:value-of select="NewDataSet/Header/GRN"/>
                        </span>
                      </td>
                      <td  style="width: 50%; border-top:0px;border:0px;border-top:solid 1px;padding-left:3px;" colspan="2">
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
                <td colspan="2" style="border-top:0px;padding:0px;border-bottom:none;" valign="top">


                  <table id="items" style="width: 100%;height:100%;" border="0"  cellspacing="0">
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
                      <td colspan="3" class="padding" style="text-align: right;border-top:0px;border-left:0px;font-weight:bold;">
                        Total
                      </td>
                      <td  class="padding" style="text-align: center;border-top:0px;border-left:0px;border-right:0px;font-weight:bold;">
                        <xsl:value-of select="sum(NewDataSet/Table/Quantity)"/>
                      </td>
                      <td style="border-top:0px;border-left:0px;border-right:0px;">
                      </td>
                    </tr>

                    <tr  class="detailsRow">
                      <td colspan="5"  style="border: 0px;">
                        Quantity In Words: <xsl:value-of select="util:QtyToWords(sum(NewDataSet/Table/Quantity))"/>
                      </td>
                    </tr>
                    <tr class="spacer-row">
                      <td colspan="5" style="border-bottom:0px;border-left:0px;border-right:0px;"></td>
                    </tr>
                  </table>
                </td>
                <td style="border-left: 0px; border-top: 0px; border-bottom: none;">
                  <div  >
                    <div style="text-align: center;height:80px;padding-top:10px;">
                      <b>Vehicle No</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/vehicleNo"/>

                    </div>
                    <div style="text-align: center;height:80px;">
                      <b>Driver</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/driver"/>
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
                    <div style="text-align: center;height:80px;">
                      <b>Weight (Approx)</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Weight"/>
                    </div>
                    <div style="text-align: center;height:80px;">
                      <b>Goods Value (Approx)</b>
                      <br/>
                      <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                    </div>

                  </div>

                </td>
              </tr>
              <tr class="spacer-row">
                <td colspan="2" style="border-top:0px;border-bottom:0px;"></td>
                <td style="border-top:0px;"></td>
              </tr>
              <tr>
                <td colspan="3"  >
                  <table style="width:100%;">
                    <tr>
                      <td style="width:50%;border:none;">
                        Terms and Conditions
                        <div style="margin-bottom:20px;font-size:9pt;line-height:15px;">
                          <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                        </div>
                      </td>
                      <td style="text-align:right;border:none;">
                        <h3>
                          For <xsl:value-of select="NewDataSet/Header/Company"/>
                        </h3>
                      </td>
                    </tr>
                    <tr>
                      <td style="border:none;">
                        <h4>
                          CUSTOMER SIGNATURE
                        </h4>
                      </td>
                      <td style="border:none;text-align:right;">
                        <table style="width:100%;">
                          <tr>
                            <td style="border:none;text-align:right;">
                              <img style="height:60px;max-width:100px;float:right;">
                                <xsl:attribute name="src">
                                  <xsl:value-of select="NewDataSet/Header/Signature"/>
                                </xsl:attribute>
                              </img>
                            </td>
                          </tr>

                          <tr>
                            <td style="border:none;text-align:right;">
                              <h4>
                                AUTHORIZED SIGNAORY
                              </h4>
                            </td>
                          </tr>
                        </table>


                      </td>
                    </tr>
                  </table>

                </td>
              </tr>
            </table>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
