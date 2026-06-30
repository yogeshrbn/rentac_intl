<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          @media print {
          body {
          font-family: arial;
          font-size: 13px;
          color: #111;
          margin: 0;
          padding: 0;
          height: 100vh;
          width:100%;
          }

          .page {
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          /*  height: 290mm; */
          /* A4 (297mm) - top/bottom margins (20mm each) */
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
          width:100%;
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
          height:100%;
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
          height: 100%;
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
          padding: 2px;
          font-size:12px;
          border: 1px solid #000;
          vertical-align: top;
          }

          /* Spacer row tofill space */
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
          .header {font-weight:bold;}

          .headertable, .address-table td {
          font-size: 14px;
          }
          .rowcell {font-size:13px;}
          strong {
          font-size:11px;
          }
          .header-cell{
          font-size:11px;
          font-weight:bold;
          }
          }
          .address-list {
          list-style-type:none;
          padding-left:0px;
          margin-left:0px;
          }
          .company_name_headding
          {
          font-size:18px;
          }


        </style>

      </head>

      <body>
        <div class="page">
          <div id="content">

            <table  border="0px"  cellspacing="0" class="table-container">
              <colgroup>
                <col style="width:50px"/>
                <col style="width:350px"/>
                <col style="width:80px"/>
                <col style="width:60px"/>
                <col style="width:60px"/>
                <col style="width:60px"/>
                <col style="width:60px"/>
                <col style="width:60px"/>
              </colgroup>
              <tbody class="table-body">
                <tr class="table-row">
                  <td class="table-cell text-center" colspan="8">
                    <span>RENTAL INWARD CHALLAN</span>
                  </td>

                </tr>
                <tr>
                  <td colspan="8" class="table-cell text-center"  style="height: 50px; vertical-align: middle;font-size:22px;font-weight:bold;">

                    <xsl:value-of select="NewDataSet/Header/Company"/>

                  </td>

                </tr>
                <tr>
                  <td colspan="3" rowspan="2" class="table-cell">
                    <h3 class="company_name_headding">Company:</h3>
                    <ul style="list-style-type:none;padding-left: 0px;margin-bottom:0px;padding-bottom:0px;">


                      <li>
                        <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                      </li>
                      <li>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                        ( <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Header/CompanyState"/>)
                      </li>
                      <li>
                        Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>,   Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      </li>
                    </ul>
                  </td>
                  <td colspan="5" class="table-cell">
                    <strong>Warehouse:</strong>
                    <xsl:value-of select="NewDataSet/Header/WarehouseAddress"></xsl:value-of>
                  </td>

                </tr>
                <tr>
                  <td colspan="5" class="table-cell">
                    <xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3" class="table-cell">
                    <strong>Name:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/Client"/>
                  </td>
                  <td colspan="5">
                    <strong>Date:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/ReceivingDate"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3" class="table-cell">
                    <strong> Location:</strong>
                    <ul class="address-list">
                      <li>
                        <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                      </li>
                      <li>
                        <xsl:value-of select="NewDataSet/Table/SiteCity"/>( <xsl:value-of select="NewDataSet/Header/SiteState"/>)
                      </li>
                      <li>
                        Pin: <xsl:value-of select="NewDataSet/Header/SiteZipCode"/>
                      </li>
                    </ul>

                  </td>
                  <td class="table-cell" colspan="5">
                    <strong>CHN:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/GRN"/>
                  </td>
                </tr>
              
                <tr class="header_row">
                  <td class="table-cell header-cell text-center" rowspan="2">
                    Sr.No
                  </td>
                  <td   class="table-cell"  rowspan="2">
                    <strong> Material</strong>
                  </td>
                  <td class="table-cell header-cell"  rowspan="2">

                    Size/Length
                    /Type

                  </td>
                  <td class="table-cell text-center" colspan="5">
                    <strong> Quantity</strong>
                  </td>
                </tr>
                <tr class="header_row">
                  <td class="table-cell"  >
                    <strong>Proper</strong>
                  </td>
                  <td class="table-cell"  >
                    <strong>Damaged</strong>
                  </td>
                  <td class="table-cell"  >
                    <strong>Irreparable</strong>
                  </td>
                  <td class="table-cell"  >
                    <strong>Replaced</strong>
                  </td>
                  <td class="table-cell text-right"  >
                    <strong>Total</strong>
                  </td>
                </tr>
                <xsl:for-each select="NewDataSet/Table">
                  <tr>
                    <td   class="table-cell rowcell text-center">
                      <xsl:value-of select="position()" />
                    </td>
                    <td   class="table-cell rowcell" style="border-top: 0px;border-left:0px;">
                      <xsl:value-of select="Item" />
                    </td>
                    <td >

                    </td>
                    <td  class="table-cell rowcell" style="border-top: 0px;border-left:0px;text-align:center">
                      <xsl:value-of select="Quantity" />
                    </td>
                    <td >
                      <xsl:choose>
                        <xsl:when test="Breakage > 0">
                          <xsl:value-of select="Breakage" />
                        </xsl:when>
                        <xsl:otherwise>
                          -
                        </xsl:otherwise>
                      </xsl:choose>
                    </td>
                    <td >

                    </td>
                    <td >

                    </td>
                    <td class="text-right">
                      <xsl:value-of select="sum(Quantity) + sum(Breakage)"/>
                    </td>
                  </tr>
                </xsl:for-each>
                <tr class="table-row spacer-row">
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                  <td class="table-cell" ></td>
                </tr>


                <tr>
                  <td colspan="8">
                    <strong>Cleaning charges need to be included? :   Yes   \   No</strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="2">
                    <strong>TRANSP: </strong>
                    <xsl:value-of select="NewDataSet/Table/Driver"/>
                  </td>
                  <td  colspan="2">
                    <strong> Vehicle No.:</strong>
                    <xsl:value-of select="NewDataSet/Header/vehicleNo"/>
                  </td>
                  <td rowspan="3" colspan="2"></td>
                  <td rowspan="3" colspan="2" class="text-center" style="vertical-align:bottom;">
                    <strong>Receiver's</strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="4">
                    <strong>DCN:</strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="4">
                    <strong>TRANP. charge paid-by:</strong> Party \  Company
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
