<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          @media print {
          body {
          font-family: calibri;
          font-size: 12pt;
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
          .header-cell {
          font-size:12pt !important;
          font-weight:bold;
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
          font-size:13px;
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
          .header {font-weight:bold;}

          .headertable, .address-table td {
          font-size: 14px;
          }
          .rowcell {font-size:13px;}
          strong {
          font-size:13px;
          }
          }
          .address-list {
          list-style-type:none;
          padding-left:0px;
          margin-left:0px;
          font-size:12pt;
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

            <table    cellspacing="0" class="table-container">
              <colgroup>
                <col style="width:50px"/>
                <col style="width:100px"/>
                <col style="width:150px"/>
                <col style="width:100px"/>
                <col style="width:100px"/>

              </colgroup>
              <tbody class="table-body">
                <tr >
                  <td class="text-center" colspan="5" >
                    <table style="width:100%;" border="0px"  cellspacing="0" >
                      <tr>
                        <td style="width:150px;border:none;" >
                          <img style ="max-width:250px;max-height:60px;">
                            <xsl:attribute name="src" >
                              <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                            </xsl:attribute>
                          </img>
                        </td>
                        <td class="text-right header-cell" style="vertical-align:bottom;border:none;font-size:16pt">
                          RENTAL OUTWARD CHALLAN
                        </td>
                      </tr>
                    </table>

                  </td>

                </tr>
                <tr>
                  <td colspan="5" class="table-cell text-center"  style="height: 50px; vertical-align: middle;font-size:36pt;font-weight:bold;">

                    <xsl:value-of select="NewDataSet/Header/Company"/>

                  </td>

                </tr>
                <tr>
                  <td colspan="3"  rowspan="2" class="table-cell">
                    <h3 class="company_name_headding">Headquaters:</h3>
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
                  <td colspan="2" class="table-cell header-cell" >
                   Warehouse:
                    <span style="width:100%;font-weight:normal"><xsl:value-of select="NewDataSet/Table/WarehouseAddress"></xsl:value-of>
                    </span>
                  </td>

                </tr>

                <tr>
                  <td colspan="2" class="table-cell">
                    <xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="5" class="table-cell" style="font-size:12pt;font-weight:bold;">
                    Name:
                    <xsl:value-of select="NewDataSet/Table/Client"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="5" class="table-cell" style="font-size:12pt;vertical-align:top;padding-bottom:0px;">
                    <span style="font-weight:bold;">Dispatch Addr:</span>
                    <ul class="address-list" style="margin:0px;">
                      <li>
                        <xsl:value-of select="NewDataSet/Table/SiteProject"/>
                      </li>
                      <li>
                        <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                      </li>
                      <li>
                        <xsl:value-of select="NewDataSet/Table/SiteCity"/>( <xsl:value-of select="NewDataSet/Header/SiteState"/>)
                      </li>
                    </ul>
                  </td>
                </tr>

                <tr>
                  <td colspan="2" class="table-cell" style="border-right:none !important;font-size:12pt;font-weight:bold;">
                    GSTIN:
                    <xsl:text>  </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/ClientGST" />
                  </td>
                  <td colspan="3" class="table-cell text-right" style="border-left:none;font-size:12pt;font-weight:bold;">
                    CHN:
                    <xsl:text>  </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>

                    |  Date:

                    <xsl:text>  </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/StartDate"/>
                  </td>
                </tr>
                <tr>
                  <td class="table-cell text-center header-cell">
                    Sr.No.
                  </td>
                  <td colspan="3" class="table-cell text-center header-cell">
                    Material
                  </td>
                  <!--<td class="table-cell">
                    <strong> Size/Length/Type</strong>
                  </td>-->
                  <td class="table-cell text-center header-cell">
                    Quantity (NOS)
                  </td>
                </tr>
                <xsl:for-each select="NewDataSet/Table">
                  <tr>
                    <td   class="table-cell rowcell text-center" style="font-size:14px;font-weight:bold;">
                      <xsl:value-of select="position()" />
                    </td>
                    <td colspan="3"  class="table-cell rowcell" style="border-top: 0px;border-left:0px;font-size:14px;font-weight:bold;">
                      <xsl:value-of select="Product" />
                    </td>

                    <td  class="table-cell rowcell" style="border-top: 0px;border-left:0px;text-align:center;font-size:14px;font-weight:bold;">
                      <xsl:value-of select="SentQty" />
                    </td>
                  </tr>
                </xsl:for-each>
                <tr class="table-row spacer-row">
                  <td class="table-cell" ></td>
                  <td class="table-cell" colspan="3" ></td>

                  <td class="table-cell" ></td>

                </tr>
                <tr>
                  <td colspan="5"   class="header-cell">
                    Contact Person:   <xsl:value-of select="NewDataSet/Table/SiteContactPerson" /> -  <xsl:value-of select="NewDataSet/Table/SiteContactPersonPhone" />
                  </td>
                </tr>
                <tr>

                  <td colspan="5" class="header-cell">
                    Advance Deposit taken in TF a/c (Rs.) :
                  </td>

                </tr>
                <tr>
                  <td colspan="5" >
                    <i>Material is given on rental basis not on sale. If any of the material gets replaced, dirty, damaged or destroyed totally (irreparable), lost or stolen than extra charges will be applied accordingly.</i>
                  </td>
                </tr>
                <tr>
                  <td colspan="2" class="header-cell">
                    TRANSP.:  <xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Table/TransporterName"/>
                  </td>
                  <td class="header-cell">
                    Vehicle No.: <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/Vehicle"/>
                  </td>
                  <td rowspan="3"  class="text-center header-cell" style="vertical-align:bottom;">
                    <img style="height:40px;max-width:100px;">
                      <xsl:attribute name="src">
                        <xsl:value-of select="NewDataSet/Header/Signature"/>
                      </xsl:attribute>
                    </img>
                  </td>
                  <td rowspan="3" class="text-center header-cell" style="font-weight:bold;font-size:12pt;padding:0px;">
                    <table style="width:100%;height:100px;" border="0px"  cellspacing="0">
                      <tr>
                        <td style="height:70px;border:none;"></td>
                      </tr>
                      <tr>
                        <td class="header-cell text-center;" style="text-align:center;border:none;border-top:solid 1px;vertical-align:bottom;">
                          Receiver's
                        </td>
                      </tr>
                    </table>
                   
                  </td>
                </tr>
                <tr>
                  <td colspan="3" class="header-cell">
                    DCN: <xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Table/Driver"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3" class="header-cell">
                    TRANP. charge paid-by: Party \  Company
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
