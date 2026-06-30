<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <xsl:variable name="itemCount" select="count(NewDataSet/Table)"/>
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          body {
          font-family: Arial, sans-serif;
          font-size: 12px;
          color: #111;
          margin: 0;
          padding: 0;
          height: 100vh;
          }

          .page {
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          height: 290mm; /* A4 (297mm) - top/bottom margins (20mm each) */
          box-sizing: border-box;
          page-break-after: always;
          padding: 0;
          }

          .page:last-of-type {
          page-break-after: auto;
          }

          header {
          text-align: center;
          font-size: 20px;
          font-weight: bold;
          padding-top: 10mm;
          }

          .content {
          flex: 1;
          height: 100%;
          padding: 0;
          }

          .footer {
          text-align: center;
          font-size: 12px;
          padding: 5mm 0 0;
          margin-top: 5mm;
          }

          /* Hide footer on all pages except the last */
          /*  .page:not(:last-of-type) .footer {
          display: none;
          }
          */
          .text-right {
          text-align: right;
          }

          /*

          */
          .table-container {
          height: 100%;
          display: table;
          width: 100%;
          table-layout: fixed;
          border-collapse: collapse;
          }

          .table-header {
          display: table-header-group;
          }

          .table-body {
          display: table-row-group;
          height: calc(100% - 10px);
          }
          .table-body td {
          display: table-cell;
          padding: 5px;
          border: 1px solid #000;
          vertical-align: top;

          }

          .table-row {
          display: table-row;
          }

          .table-cell {
          display: table-cell;
          padding: 5px;
          border: 1px solid #000;
          vertical-align: top;
          }

          /* Spacer row to fill space */
          .spacer-row {
          height: 100%;
          }

          .spacer-row .table-cell {
          border: 1px solid #000;
          }

          .text-center {
          text-align: center;
          }

          .firstRow {
          border-top: solid 1px;
          }

          .border {
          border: solid 1px #000;
          }

          .headertable, .address-table td {
          font-size: 14px;
          }
          ul {list-style:none;}
          .subheading {font-size:14px;font-weight:bold;}
        </style>

      </head>

      <body>
        <div class="page">
          <div id="content">
            <img style="height:60px;max-width:100px;margin-bottom:0.2in;">
              <xsl:attribute name="src">
                <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
              </xsl:attribute>
            </img>
            <xsl:variable name="catRows" select="count(NewDataSet/Table) + 9" />

            <table  border="0px"  cellspacing="0" class="table-container">
              <colgroup>
                <col style="width:50px"/>
                <col style="width:250px"/>
                <col style="width:80px"/>
                <!--<col style="width:80px"/>-->
                <col style="width:80px"/>

                <col style="width:80px"/>
                <col style="width:150px"/>

              </colgroup>
              <tbody class="table-body">
                <tr >
                  <td colspan="6" style="">
                    <table style="width:100%;border:none;" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="width:33.33%;border:none;font-weight:bold;">
                          GST No: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                        </td>
                        <td style="width:33.33%;text-align:center;border:none;font-weight:bold;">

                          <xsl:choose>
                            <xsl:when test="count(NewDataSet/Config/root[Key = 'heading']) != ''">
                              <xsl:value-of select="NewDataSet/Config/root[Key='heading']/Value" />
                            </xsl:when>
                            <xsl:otherwise>
                              RETURN CHALLAN
                            </xsl:otherwise>
                          </xsl:choose>
                        </td>
                        <td style="width:33.33%;text-align:right;border:none;font-weight:bold;">
                          RETURNABLE
                        </td>
                      </tr>
                    </table>

                  </td>

                </tr>
                <tr  >
                  <td style="text-align: center;" colspan="6" class="padding table-cell">

                    <p style="font-size:18px;font-weight:bold;margin-bottom:0px;">
                      <xsl:value-of select="NewDataSet/Header/Company"/>
                    </p>
                    <p style="margin-bottom:5px;">
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
                  <td colspan="3">
                    <ul style="margin:0px;padding:0px;">
                      <li class="subheading">Ship From</li>
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
                        <xsl:if test="NewDataSet/Header/SiteStateGSTCode != ''">
                          ( <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>)
                        </xsl:if>
                      </li>

                      <xsl:choose >
                        <xsl:when test="NewDataSet/Table/SiteGST != ''">
                          <li>
                            GSTIN: <xsl:value-of select="NewDataSet/Table/SiteGST" />
                          </li>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:if  test="NewDataSet/Table/ClientGST != ''">
                            <li>
                              GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
                            </li>
                          </xsl:if>
                        </xsl:otherwise>
                      </xsl:choose>
                    </ul>
                  </td>
                  <td colspan="3">
                    <ul style="margin:0px;padding:0px;">
                      <li class="subheading">Ship To</li>
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
                <tr>
                  <td colspan="3">
                    <b>State</b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/BillState"/>

                    <b>State Code</b>

                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>

                  </td>
                  <td colspan="3">
                    <b>State</b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/SiteState"/>

                    <b>State Code</b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <b>Challan Number:</b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/GRN"/>

                  </td>
                  <td colspan="3">
                    <b>Challan Date:</b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/ReceivingDate"/>
                  </td>

                </tr>

                <tr>

                  <td  class="table-cell" style="width:20px;">
                    S.No
                  </td>
                  <td   class="table-cell">Item</td>
                  <td  class="table-cell" style=" text-align:center">HSN</td>
                  <td  class="table-cell"   style=" text-align:center">Unit</td>
                  <td class="table-cell"  style=" text-align:center">Qty</td>

                  <td class="table-cell"  ></td>
                  <!--<td  class="table-cell"   style="border-right:0px;text-align:right">Amount</td>-->
                </tr>
                <xsl:for-each select="NewDataSet/Table">
                  <tr>
                    <td   class="table-cell">
                      <xsl:value-of select="position()" />
                    </td>
                    <td  class="table-cell" style="border-top: 0px;border-left:0px;">
                      <xsl:value-of select="Item" />
                    </td>
                    <td  class="table-cell" style="border-top: 0px;border-left:0px;text-align:center">
                      <xsl:value-of select="HSNCode" />
                    </td>
                    <td  class="table-cell" style="border-top: 0px;border-left:0px;text-align:center">
                      <xsl:value-of select="Unit" />
                    </td>
                    <td  class="table-cell" style="border-top: 0px;border-left:0px;text-align:center">
                      <xsl:value-of select="Quantity" />
                    </td>

                    <!--<td  class="table-cell" style="border-left:0px;border-top: 0px;text-align:center">
                      <xsl:value-of select="Rate" />
                    </td>-->
                    <!--<td  class="table-cell" style="border-left:0px;border-right: 0px;border-top: 0px;text-align:right">
                      <xsl:value-of select="SubTotal" />
                    </td>-->
                    <xsl:if test="position() = 1">
                      <td style="border-left:solid 1px;border-top:0px; border-right:solid 1px;vertical-align:top;">

                        <xsl:attribute name="rowspan">
                          <xsl:value-of select="$itemCount + 2"/>
                        </xsl:attribute>
                        <div>
                          <div style="text-align: center;height:100px;padding-top:10px;">
                            <b>Vehicle No</b>
                            <br/>
                            <xsl:value-of select="vehicleNo"/>
                          </div>
                          <div style="text-align: center;height:100px;">
                            <b>Driver / Supervisor</b>
                            <br/>
                            <xsl:value-of select="driver"/>
                          </div>
                          <xsl:if test="LRNumber != ''">
                            <div style="text-align: center;height:100px;">
                              <b>LR No</b>
                              <br/>
                              <xsl:value-of select="LRNumber"/>
                            </div>
                          </xsl:if>
                          <xsl:if test="CRNumber != ''">
                            <div style="text-align: center;height:100px;">
                              <b>CR No</b>
                              <br/>
                              <xsl:value-of select="CRNumber"/>
                            </div>
                          </xsl:if>
                          <xsl:if test="GRNumber != ''">
                            <div style="text-align: center;height:100px;">
                              <b>GR No</b>
                              <br/>
                              <xsl:value-of select="GRNumber"/>
                            </div>
                          </xsl:if>
                        </div>
                      </td>
                    </xsl:if>
                  </tr>
                </xsl:for-each>

                <tr  >
                  <td colspan="2" class="table-cell" style="border-right:none;">

                  </td>
                  <td    colspan="2" class="table-cell" style="border-left:none;text-align:right;">
                    Total
                  </td>

                  <td     class="table-cell text-center" >

                    <xsl:value-of select='sum(NewDataSet/Table/Quantity)'/>
                  </td>
                </tr>
                <tr class="table-row spacer-row">
                  <td class="table-cell" colspan="5"></td>

                </tr>



                <tr class="table-row">
                  <td colspan="6" class="table-cell" style="border-right: solid 1px;padding:0">
                    <div style="display:flex;">

                      <div style="padding:10px;">
                        <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                          Additional Information
                        </p>
                        <p>
                          <xsl:value-of select="NewDataSet/Header/Remarks"  disable-output-escaping="yes"/>
                        </p>

                        <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                          Terms and Conditions
                        </p>

                        <div>
                          <xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
                        </div>

                      </div>
                    </div>
                    <div >

                      <table style="width:100%;border:none;"  cellpadding="0" cellspacing="0">
                        <tr>
                          <td style="border:none;vertical-align:bottom;">
                            <span style="font-size:16pt; ">CUSTOMER SIGNATURE</span>

                          </td>
                          <td style="border:none;text-align:right;">
                            <div style="display:flex;width:100%;">
                              <div style=" padding-top:15px;font-size:18px;font-weight:bold;">
                                For  <xsl:value-of select="NewDataSet/Header/Company"/>
                              </div>
                              <div style="text-align:right;width:100%;margin-top:20px;">
                                <img style="height:40px;">
                                  <xsl:attribute name="src">
                                    <xsl:value-of select="NewDataSet/Header/Signature"/>
                                  </xsl:attribute>
                                </img>
                              </div>
                              <div style="margin-top:16px;font-size:16pt;">
                                AUTHORIZED SIGNAORY
                              </div>
                            </div>

                          </td>
                        </tr>
                      </table>


                    </div>


                  </td>
                </tr>
              </tbody>
            </table>

          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
