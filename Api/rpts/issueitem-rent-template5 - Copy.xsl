<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          .items-table { width: 100%; border-collapse: collapse; table-layout: fixed; }
          .items-table td, .items-table th { border: solid 1px #000; padding: 5px; }
          .spacer-row td { height: 120mm; vertical-align: top; border: solid 1px #000; }
          @media print {

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
          .subheading {font-size:12pt;font-weight:bold;}
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
          height: 258mm; /* A4 (297mm) - top/bottom margins (20mm each) */
          }
          .items-table { width: 100%; border-collapse: collapse; table-layout: fixed; }
          .items-table td, .items-table th { border: solid 1px #000; padding: 5px; }
          .spacer-row td { height: 120mm; vertical-align: top; border: solid 1px #000; }
          }
        </style>

      </head>

      <body>

        <div id="printArea" class="printArea">
          <img style="height:60px;max-width:100px;margin-bottom:0.2in;">
            <xsl:attribute name="src">
              <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
            </xsl:attribute>
          </img>
          <div id="container" class="container" style="padding-top:10px;">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <!--<colgroup>
                <col style="width:20%"/>
                <col style="width:30%"/>
                <col style="width:25%"/>
                <col style="width:25%"/>
              </colgroup>-->
              <tr  class="padding" style="height:25px;padding:3px;">
                <td   colspan="2"  style="width: 25%;border-right:0px;border-bottom:0px;"   class="subheading padding">
                  GSTIN: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td class="subheading"  style="width: 50%;border-right:0px;border-left:0px;border-bottom:0px; text-align: center;">
                  Delivery Challan                  
                </td>
                <td  style="width: 25%;border-left:0px; text-align:right;border-bottom:0px;" class="padding">
                  Mob: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                </td>
              </tr>
              <tr class="padding">
                <td colspan="4" style="text-align:center;border-top:none;border-bottom:0px;font-size:20pt;font-weight:bold;">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
              </tr>
              <tr class="padding">
                <td colspan="4" class="subheading" style="text-align:center;">
                  Deals In : Steel Plates, Channel, Props, Scaffolding and All Shuttering Materials ON HIRE BASIS
                </td>
              </tr>
              <tr class="padding">
                <td style="border-top:none !important; text-align: center;border-bottom:0px;" colspan="4" class="padding">
                  <ul style="margin:0px;">
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    </li>
                    <li>
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      ,
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </li>
                  </ul>
                </td>
              </tr>
              <tr class="padding">
                <td  colspan="4" >
                  <table style="width: 100%;" border="0px"  cellspacing="0">
                    <tr>
                      <td class="subheading noborder" style="width:33%;vertical-align:bottom;">
                        CHALLAN No.
                        <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>
                      </td>
                      <td class="subheading noborder" style="text-align:center;vertical-align:bottom;" >
                        <ul style="width:100%;margin:0px;">
                          <li>
                            SAC Code: <xsl:value-of select="NewDataSet/Table/sacCode"/>
                          </li>
                          <li>
                            HSN Code:  <xsl:value-of select="NewDataSet/Table/HSNCode"/>
                          </li>
                        </ul>
                      </td>
                      <td class="subheading noborder" style="width:33%;text-align:right;vertical-align:bottom;">
                        Dated.
                        <xsl:value-of select="NewDataSet/Header/StartDate"/>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>

              <tr class="padding">
                <td colspan="4" style="border-top: 0px;padding-top:0px;padding-bottom:0px;">
                  <table style="width: 100%" border="0" cellspacing="0" cellpadding="0">
                    <tr class="padding">
                      <td   class="padding noborder" style="border-right:solid 1px;width:50%;vertical-align:top;">
                        <ul style="margin:0px;padding:0px;">
                          <li class="subheading">Bill To</li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/Client"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/BillAddress1"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/BillAddress2"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/BillCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/BillZipCode"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/BillState"/>
                            <xsl:text> </xsl:text>
                            (<xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>)
                          </li>
                          <li>
                            GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
                          </li>
                        </ul>


                      </td>
                      <td class="padding noborder" style="vertical-align:top">
                        <ul style="margin:0px;padding:0px;">
                          <li class="subheading">Ship To</li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/Client"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteProject"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteZipCode"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteState"/>
                            <xsl:text> </xsl:text>
                            ( <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>)
                          </li>
                        </ul>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="border-top:0px;padding:0px;vertical-align:top;">
                  <table class="items-table" cellspacing="0">
                    <tr>
                      <th style="width:60px;border-bottom:solid 1px;border-top:0px;border-left:0px;">S.No</th>
                      <th style="width:335px;border-bottom:solid 1px;border-top:0px;">Description</th>
                      <th style="border-bottom:solid 1px;border-top:0px;width:100px;text-align:center">Qty</th>
                      <th style="border-bottom:solid 1px;border-top:0px;width:100px;text-align:center;border-right:0px;">Unit</th>
                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr class="detailsRow">
                        <td style="border-top: 0px;border-left:0px;text-align:center;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Product" />
                        </td>
                        <td class="noborder" style="border-bottom:solid 1px;border-right:solid 1px;text-align:center">
                          <xsl:value-of select="SentQty" />
                        </td>
                        <td class="noborder" style="border-bottom:solid 1px;text-align:center;border-right:0px;">
                          <xsl:value-of select="Unit" />
                        </td>
                      </tr>
                    </xsl:for-each>
                    <tr class="spacer-row">
                      <td style="height:120mm;border-left:0px;">&#160;</td>
                      <td style="height:120mm;">&#160;</td>
                      <td style="height:120mm;">&#160;</td>
                      <td style="height:120mm;border-right:0px;">&#160;</td>
                    </tr>
                    <tr class="detailsRow">
                      <td colspan="2" style="text-align:right;border:solid 1px #000;padding:5px;border-left:0px;">
                        <b>Total</b>
                      </td>
                      <td style="text-align:center;border:solid 1px #000;padding:5px;">
                        <b>
                          <xsl:value-of select="sum(NewDataSet/Table/SentQty)"/>
                        </b>
                      </td>
                      <td style="border:solid 1px #000;padding:5px;"></td>
                    </tr>
                    <tr class="detailsRow">
                      <td colspan="4" style="border:solid 1px #000;padding:5px;border-left:0px;border-bottom:0px;">
                        Total Qty In Words: <xsl:value-of select="util:QtyToWords(sum(NewDataSet/Table/SentQty))"/>
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
                      <b>Driver / Supervisor</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/Driver"/>
                    </div>
                    <div style="text-align: center;height:100px;">
                      <b>E-Way Bill No</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Table/ewayBillNo"/>
                    </div>
                    <!--<div style="text-align: center;height:100px;">
                      <b>Cartate</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/Freight"/>
                    </div>
                    <div style="text-align: center;height:100px;">
                      <b>Time</b>
                      <br/>

                    </div>-->
                  </div>
                </td>
              </tr>

              <tr>
                <td colspan="4"  style="border-top:0px;padding:0px;">
                  <table style="width:100%" cellpadding="0" cellspacing="0">
                    <colgroup>
                      <col style="width:45%"/>
                      <col style="width:20%"/>
                      <col style="width:35%"/>

                    </colgroup>
                    <tr>
                      <td  rowspan="2" class="subheading" style="width:30%;text-align:center;border-top:0px;border-left:0px;">
                        <ul>
                          <li>ON HIRE BASIS</li>
                          <li>ON RETURNABLE BASIS</li>
                          <li>NOT FOR SALE</li>
                        </ul>
                      </td>
                      <td  colspan="2"  style="border-top:0px;border-left:0px;border-right:0px" >
                        Estimated Value:    <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                      </td>
                    </tr>

                    <tr>
                      <td colspan="2"   style="border-top:0px;border-left:0px;border-right:0px">
                        Estimated Weight :     <xsl:value-of select="NewDataSet/Table/Weight"/>
                      </td>
                    </tr>
                    <tr>
                      <td class="padding noborder" style="border-right:solid 1px;vertical-align:top;" >
                        <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                          Terms and Conditions
                        </p>
                        <div style="font-size:8pt;">
                          <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                        </div>
                      </td>
                      <td class="padding noborder subheading" style="border-right:solid 1px;vertical-align:bottom;">
                        <span style="font-size:14px; ">CUSTOMER SIGNATURE</span>
                      </td>
                      <td class="padding noborder subheading" style="vertical-align:top;text-align:center;">
                        <table style="width:100%">
                          <tr>
                            <td class="noborder" style="text-align:center ">
                              For <span class="subheading">
                                <xsl:value-of select="NewDataSet/Header/Company"/>
                              </span>
                            </td>
                            <tr>
                              <td class="noborder" style="text-align:center">
                                <img style="height:60px;max-width:100px;">
                                  <xsl:attribute name="src">
                                    <xsl:value-of select="NewDataSet/Header/Signature"/>
                                  </xsl:attribute>
                                </img>
                              </td>
                            </tr>
                            <tr>
                              <td class="noborder" style="text-align:center"> AUTHORIZED SIGNAORY</td>
                            </tr>
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
