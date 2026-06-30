<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"   xmlns:util="urn:util-format">
  <xsl:template match="/">
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
        </style>

      </head>

      <body>
        <div class="page">
          <div id="content">



            <table  border="0px"  cellspacing="0" class="table-container">
              <colgroup>
                <col style="width:50px"/>
                <col style="width:60px"/>
                <col style="width:250px"/>
                <col style="width:100px"/>
                <col style="width:50px"/>

                <col style="width:120px"/>
                <col style="width:80px"/>

              </colgroup>
              <tbody class="table-body">
                <tr>
                  <td colspan="4" style="font-weight:bold;">
                    OUTWARD HIRE DELIVERY CHALLAN (RETURNABLE)
                  </td>
                  <td colspan="3" style="font-weight:bold;">
                    E WAY BILL:- <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Table/ewayBillNo"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="4">
                    <ul style="list-style-type:none;padding-left: 0px;margin-bottom:0px;padding-bottom:0px;">

                      <li style="font-size:18px;font-weight:bold">
                        <xsl:value-of select="NewDataSet/Header/Company"/>
                      </li>
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
                      <li>
                        <strong>GSTIN:</strong>
                        <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                      </li>
                    </ul>
                  </td>
                  <td colspan="3" class="text-center">
                    <img style="height:90px;max-width:200px;">
                      <xsl:attribute name="src">
                        <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                      </xsl:attribute>
                    </img>
                  </td>
                </tr>


                <tr>
                  <td colspan="4" rowspan="5">
                    <ul style="font-size:14px;list-style-type:none;padding-left: 0px;">

                      <li style="font-size:16px;font-weight:bold">
                        Bill To
                      </li>
                      <li>
                        <xsl:value-of select="NewDataSet/Header/Client"/>
                        <br/>
                        <xsl:value-of select="NewDataSet/Header/BillAddress1"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillAddress2"/>
                        <br/>
                        <xsl:value-of select="NewDataSet/Header/BillCity"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillZipCode"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillState"/>
                      </li>
                      <li>
                        <strong> GSTIN: </strong>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/ClientGST"/>
                      </li>
                    </ul>
                  </td>
                  <td colspan="3">
                    <strong>Challan No:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>Date</strong>
                    <xsl:text> </xsl:text>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/StartDate"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>VEHICLE No</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/Vehicle"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>DRIVER NAME</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/Driver"/>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>
                      Weight (KG) <xsl:text> </xsl:text>   <xsl:value-of select="NewDataSet/Table/Weight"/>
                    </strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="4" style="pading-bottom:0px;" >
                    <ul style="font-size:14px;list-style-type:none;padding-left: 0px;padding-top:0px;padding-bottom:0px;margin:0px;">
                      <li style="font-size:16px;font-weight:bold">
                        Ship To
                      </li>

                      <li>
                        <xsl:value-of select="NewDataSet/Header/Client"/>
                        <br/>
                        <xsl:if test="NewDataSet/Table/SiteProject != ''">                          
                            <xsl:value-of select="NewDataSet/Table/SiteProject" />                          
                         <xsl:text> </xsl:text>
                        </xsl:if>

                        <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                        
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Table/SiteState"/>
                        (<xsl:value-of select="NewDataSet/Table/SiteZipCode"/>)
                      </li>
                      <li>
                        Contact No:<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Table/SiteContactPersonPhone" />  <xsl:text> </xsl:text> | <xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Table/SiteContactPerson" />
                      </li>
                    </ul>
                  </td>
                  <td colspan="3" style="vertical-align:bottom;pading-bottom:0px;">
                    RECEIVER'S SIGNATURE
                  </td>
                </tr>
                <!--<tr>
                  <td colspan="3">
                    <strong>
                      Site ATDN<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Table/SiteContactPerson" />
                    </strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>
                      Contact No.<xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Table/SiteContactPersonPhone" />
                    </strong>
                  </td>
                </tr>
                <tr>
                  <td colspan="3">
                    <strong>Location</strong>
                  </td>
                </tr>-->
                <xsl:for-each select="NewDataSet/Table[Unit='Set']">
                  <tr>
                    <td colspan="7" class="table-cell text-center" style="font-weight:bold;">
                      <xsl:value-of select="Product" /> (<xsl:value-of select="SentQty" />)
                    </td>
                  </tr>
                </xsl:for-each>
                <tr>

                  <td  class="table-cell text-center header" style="width:20px;">
                    S.No
                  </td>
                  <td   class="table-cell header">Part No</td>
                  <td   class="table-cell header">Description</td>
                  <td  class="table-cell header" style=" text-align:center">PICTURE</td>

                  <td class="table-cell header"  style=" text-align:center">Qty</td>
                  <td  class="table-cell header" style=" text-align:center">Charges/PCS</td>
                  <td  class="table-cell header"   style="text-align:right">Amount</td>
                </tr>

                <xsl:for-each select="NewDataSet/Table[Unit != 'Set']">
                  <tr>
                    <td   class="table-cell rowcell text-center">
                      <xsl:value-of select="position()" />
                    </td>
                    <td   class="table-cell rowcell">
                      <xsl:value-of select="ProductCode" />
                    </td>
                    <td  class="table-cell rowcell" style="border-top: 0px;border-left:0px;">
                      <xsl:value-of select="Product" />
                    </td>
                    <td  class="table-cell rowcell" style="border-top: 0px;border-left:0px;text-align:center">
                      <img style="height:28px;max-width:100px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="util:getDocs(CompanyId,Image1)"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td  class="table-cell rowcell" style="border-top: 0px;border-left:0px;text-align:center">
                      <xsl:value-of select="SentQty" />
                    </td>
                    <td  class="table-cell rowcell" style="border-left:0px;border-top: 0px;text-align:right">
                      <xsl:value-of select="Rate" />
                    </td>
                    <td  class="table-cell rowcell" style="text-align:right">
                      <xsl:value-of select='format-number(SubTotal, "#.00")'/>
                    </td>
                  </tr>
                </xsl:for-each>
                <xsl:if test="count(NewDataSet/Table) &lt; 18">
                  <tr class="table-row spacer-row">
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                  </tr>
                </xsl:if>
                <tr  >
                  <td colspan="3" class="table-cell" style="border-right:Solid 1px;">

                  </td>
                  <td   class="table-cell text-center">
                    <strong>Total</strong>
                  </td>
                  <td   class="table-cell" style="text-align: center;">
                    <xsl:value-of select="sum(NewDataSet/Table[Unit != 'Set']/SentQty)"/>
                  </td>
                  <td   class="table-cell" style="text-align: right;">
                  </td>
                  <td  class="table-cell" style="text-align: right;">
                    <xsl:value-of select="format-number(sum(NewDataSet/Table[Unit != 'Set']/SubTotal), '#.00')"/>
                  </td>
                </tr>
                <tr class="table-row spacer-row">
                  <td colspan="5" class="table-cell" style="height:100px !important;font-size:11px">
                    <xsl:value-of select="NewDataSet/Header/Remarks"  disable-output-escaping="yes"/>

                    <xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
                  </td>

                  <td colspan="2" class="table-cell" style="vertical-align:bottom;">

                    <span style="font-size:16px;min-height:60px;font-weight:bold;">
                      For <br/><xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/Company"/>
                    </span>

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
