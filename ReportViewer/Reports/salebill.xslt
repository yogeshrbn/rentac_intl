<?xml version="1.0" encoding="utf-8"?>


<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <xsl:variable name="freightIGST">
      <xsl:choose>
        <xsl:when test="number(data/Table/FreightTax) > 0 and number(data/Table/IGST) > 0">
          <xsl:value-of select="number(data/Table/FreightTax)" />
        </xsl:when>
        <xsl:otherwise>
          0
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="freightSGST">
      <xsl:choose>
        <xsl:when test="number(data/Table/FreightTax) > 0 and number(data/Table/SGST) > 0">
          <xsl:value-of select="round(number(data/Table/FreightTax) div 2)" />
        </xsl:when>
        <xsl:otherwise>
          0
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="rowSpan">
      <xsl:choose>
        <xsl:when test="data/Table/OtherChargeAmount > 0">
          8
        </xsl:when>
        <xsl:otherwise>
          7
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>



    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview

        <meta name="viewport" content="width=device-width,initial-scale=1" />

        <style>
          @media print {

          body {
          font-family: arial;
          font-size:13px !important;
          color: #111;
          margin: 0px;
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
          width: 99.8% !important;
          table-layout: fixed;
          border-collapse: collapse;
          }

          .table-header {
          display: table-header-group;
          }

          .table-body {
          display: table-row-group;
          height: 100% !important;
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
          font-size:12pt !important;
          border: 1px solid #000;
          vertical-align: top;
          }
          .table-cell strong {
          font-size:12pt;
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

          .itemtable td {
          font-size:12pt !important;
          }
          pre {
          background: none;
          overflow: hidden;
          border: none;
          font-size: 11px;
          color: #000;
          font-family: arial;
          padding:0px;
          }
          .no-border {
          border:none !important;
          }
          }
        </style>

      </head>
      <body>
        <div class="page">


          <div id="content">

            <table class="table-container" cellpadding="0" cellspacing="0" style="width:100%;" >
              <colgroup>
                <col width="50px"/>
                <col width="320px"/>
                <col width="70px"/>
                <col width="80px"/>
                <col width="80px"/>
                <col width="80px"/>
                <col width="100px"/>

              </colgroup>
              <tbody class="table-body">
                <tr class="table-row">
                  <td style="border:0px;" colspan="4" class="table-cell">
                    <img style="height:60px;max-width:150px;">
                      <xsl:attribute name="src">
                        <xsl:value-of select="data/Table/CompanyLogo"/>
                      </xsl:attribute>
                    </img>
                    <br/><br/>
                    <xsl:value-of select="data/Table/Company" /><br />
                    <xsl:value-of select="data/Table/CompanyAddress1" />  <xsl:value-of select="data/Table/CompanyAddress2" /><br />
                    <xsl:value-of select="data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="data/Table/CompanyZipCode" /><br />
                    Mobile: <xsl:value-of select="data/Table/CompanyPhone" />, Email:   <xsl:value-of select="data/Table/CompanyEmail" /><br />
                  </td>
                  <td style=" text-align:right;vertical-align:top; border:0;" colspan="3"  >

                    <strong>PAN:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="data/Table/CompanyPAN" />
                    <br />
                    <strong>GSTIN:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="data/Table/CompanyGST"/>
                    <br />
                    <strong>MSME Number:</strong>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="data/Table/MSMENumber" />
                    <br />
                  </td>
                </tr>
                <tr class="table-row">
                  <td style="border:0px;padding:0;" colspan="2">
                    <table width="100%" cellpadding="0" cellspacing="0">
                      <tr>
                        <td class="no-border">
                          <strong style="font-size:18px">TAX INVOICE</strong>
                        </td>
                        <td class="no-border">
                          <xsl:if test="data/Table/billPONumber != ''">
                            PO#: <xsl:value-of select="data/Table/billPONumber" /> |
                            PO Date: <xsl:value-of select="util:DateToDDMMYYYY(data/Table/billPODate)" />
                          </xsl:if>
                        </td>
                      </tr>
                    </table>

                  </td>
                  <td class="text-right" colspan="5" style="border:0px;">
                    Invoice #: <xsl:value-of select="data/Table/InvoiceNumber" /> Date:  <xsl:value-of select="util:DateToDDMMYYYY(data/Table/InvoiceDate)"/>
                  </td>
                </tr>
                <tr class="table-row">
                  <td  colspan="3" class="table-cell">
                    <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Bill TO</p>
                    <xsl:choose>
                      <xsl:when test="data/Table/BillFromSiteAddress = 1 ">
                        <xsl:value-of select="data/Table/Client" /><br />
                        <xsl:value-of select="data/Table/SiteProject" />  <xsl:value-of select="data/Table/SiteAddress" /> <xsl:value-of select="data/Table/SiteAddress2" /><br />
                        <xsl:value-of select="data/Table/City" />, PIN: -   <xsl:value-of select="data/Table/SiteZipCode" /><br />
                        GSTIN: <xsl:value-of select="data/Table/SiteGST" /><br />
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="data/Table/Client" /><br />
                        <xsl:value-of select="data/Table/ClientAddress1" />  <xsl:value-of select="data/Table/ClientAddress2" /><br />
                        <xsl:value-of select="data/Table/ClientCity" />, PIN: -   <xsl:value-of select="data/Table/ClientZipCode" /><br />
                        GSTIN: <xsl:value-of select="data/Table/ClientGST" /><br />
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                  <td style="vertical-align: top;" class="table-cell" colspan="4">
                    <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">Ship TO</p>
                    <xsl:choose>
                      <xsl:when test="data/Table/ShipTo != '' ">
                        <xsl:value-of select="data/Table/ShipTo" />
                        <br />
                      </xsl:when>
                      <xsl:when test="data/Table/LedgerSiteId > 0">
                        <xsl:value-of select="data/Table/Client" /><br />
                        <xsl:value-of select="data/Table/SiteProject" />  <xsl:value-of select="data/Table/SiteAddress" /> <xsl:value-of select="data/Table/SiteAddress2" /><br />
                        <xsl:value-of select="data/Table/City" />, PIN: -   <xsl:value-of select="data/Table/SiteZipCode" /><br />
                        GSTIN: <xsl:value-of select="data/Table/SiteGST" /><br />
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="data/Table/Client" /><br />
                        <xsl:value-of select="data/Table/ClientShipAddress1" /> <xsl:text>  </xsl:text> <xsl:value-of select="data/Table/ClientShipAddress2" /><br />
                        <xsl:value-of select="data/Table/ClientShipCity" />, <xsl:text>  </xsl:text>(<xsl:value-of select="data/Table/ClientShipStateName" />) <xsl:text>  </xsl:text> PIN: -   <xsl:value-of select="data/Table/ClientShipZipCode" /><br />
                        GSTIN: <xsl:value-of select="data/Table/ClientGST" /><br />
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                </tr>
                <tr class="table-row">

                  <td style="width:40%;" class="table-cell" colspan="2">Product Name  Desc.</td>
                  <td style="width:80px;" class="table-cell text-center">UOM</td>

                  <td style="width:80px;" class="table-cell text-center">HSN Code</td>
                  <td class="text-center "  style="width:100px;">Quantity</td>

                  <td class="text-right table-cell"   style="width: 100px;">Rate</td>

                  <td class="text-right table-cell" style="width: 100px;" >Amount</td>






                </tr>
                <xsl:for-each select="data/Table">
                  <tr class="table-row">

                    <td colspan="2" class="table-cell">
                      <xsl:value-of select="Item" />
                      <xsl:if test="LineItem != ''">
                        <br/>
                        <span style="font-size:11px;padding-left:8px;">
                          <xsl:value-of select="LineItem" />
                        </span>
                      </xsl:if>
                    </td>
                    <td class="table-cell text-center">
                      <xsl:value-of select="UnitName" />
                    </td>
                    <td class="table-cell text-center">
                      <xsl:value-of select="HSNCode" />
                    </td>
                    <td class="text-center table-cell" >
                      <xsl:value-of select="Quantity" />
                    </td>
                    <td class="text-right table-cell" >
                      <xsl:value-of select="Rate" />
                    </td>
                    <td class="text-right table-cell" >
                      <xsl:value-of select="format-number(SubTotal,'#,###.00')" />
                    </td>
                  </tr>
                </xsl:for-each>
                <tr class="spacer-row">
                  <td class="table-cell" colspan="2"></td>
                  <td class="table-cell"></td>
                  <td class="table-cell"></td>
                  <td class="table-cell"></td>
                  <td class="table-cell"></td>
                  <td class="table-cell"></td>
                </tr>
                <tr class="table-row">
                  <td style=" padding:5px; vertical-align: bottom;" colspan="4" rowspan="{$rowSpan}" >
                    <strong>In Words</strong>
                 
                    <br/>
                    <p>
                      <xsl:value-of select="util:AmountToWords(data/Table/RoundedAmount)"/>
                    </p>
                    <div style="width:60%;float:left;">
                      <xsl:if test="data/Table/PrintBankDetails = 'True'">
                        <ul style="list-style:none;padding:0px;">
                          <li>
                            <b>Bank Details</b>
                          </li>
                          <li>
                            Bank A/C No: <xsl:value-of select="data/Table/CompanyBankAccNumber"/>
                          </li>
                          <li>
                            Bank:  <xsl:value-of select="data/Table/CompanyBankName"/>
                          </li>
                          <li>
                            Branch Address:  <xsl:value-of select="data/Table/CompanyBankBranch"/>
                          </li>
                          <li>
                            IFSC Code: <xsl:value-of select="data/Table/CompanyBankIFSC"/>
                          </li>
                        </ul>
                      </xsl:if>
                      <br />
                      <i>Customer Acceptance (Sign here)</i>
                      <br />

                    </div>
                    <div style="width:40%;float:right;text-align:right;">

                      <xsl:if test="data/Table/PrintQrCode = 'True'">
                        <img style="max-width:120px;margin-right:20px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="data/Table/QrCode"/>
                          </xsl:attribute>
                        </img>
                      </xsl:if>
                    </div>

                  </td>
                  <td class="text-right table-cell" colspan="2" >SubTotal</td>
                  <td    class="text-right table-cell">
                    <!--<xsl:value-of select="format-number(sum(data/Table/SubTotal),'#,###.00')"/>-->
                    <xsl:value-of select="util:FormatNumber(sum(data/Table/SubTotal) )"/>
                  </td>
                </tr>
                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">Freight</td>
                  <td    class="text-right">
                    <xsl:value-of select="util:FormatNumber(data/Table/Freight)"/>

                  </td>
                </tr>

                <xsl:if test="data/Table/OtherChargeAmount > 0">
                <tr class="table-row">

           
                  <td class="text-right table-cell" colspan="2">Other Charges</td>
                  <td class="table-cell text-right">
                    <xsl:value-of select="util:FormatNumber(data/Table/OtherChargeAmount)" />
                  </td>
                </tr>
                </xsl:if>
                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">Discount</td>
                  <td  class="text-right" >
                    <xsl:value-of select="util:FormatNumber(data/Table/discount)" />
                  </td>
                </tr>
                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">
                    IGST ( <xsl:value-of select="data/Table/IGSTRate"/> % )
                  </td>
                  <td class="text-right" >
                    <xsl:value-of select="util:FormatNumber(sum(data/Table/IGST) +  data/Table/chargesTaxIGST + data/Table/FreightIGST   )"/>
                  </td>
                </tr>
                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">
                    SGST ( <xsl:value-of select="data/Table/SGSTRate"/> % )
                  </td>
                  <td   class="text-right" >
                    <xsl:value-of select="util:FormatNumber(sum(data/Table/SGST) + data/Table/chargesTaxSGST + data/Table/FreightSGST)"/>
                  </td>
                </tr>
                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">
                    CGST ( <xsl:value-of select="data/Table/CGSTRate"/> % )
                  </td>
                  <td   class="text-right table-cell" >
                    <xsl:value-of select="util:FormatNumber(sum(data/Table/CGST) + data/Table/chargesTaxSGST + data/Table/FreightSGST)"/>
                  </td>
                </tr>

                <tr class="table-row">
                  <td class="text-right table-cell" colspan="2">
                    Total
                  </td>
                  <td   class="text-right table-cell" >
                    <xsl:value-of select="util:FormatNumber(data/Table/Total)" />
                  </td>
                </tr>
                <tr class="table-row">

                  <td  class="text-right table-cell" colspan="7" >
                    <div style="text-align:right;float:right;">
                      <table border="0"  cellpadding="0" cellspacing="0">
                        <tr>
                          <td style="border:0px; text-align:right;padding:0px;">
                            <img style="max-width:100px;max-height:50px;">
                              <xsl:attribute name="src">
                                <xsl:value-of select="data/Table/Signature"/>
                              </xsl:attribute>
                            </img>
                          </td>
                        </tr>
                        <tr>
                          <td style="border:0px; line-height:30px;padding:0px;">
                            For <i>
                              <xsl:value-of select="data/Table/Company" />
                            </i>
                          </td>
                        </tr>
                      </table>
                    </div>

                  </td>
                </tr>
                <xsl:if test="data/Table/SignedQrCode != ''">
                  <tr class="table-row">
                    <td rowspan="3" class="table-cell" colspan="3" >
                      <img  style="width:120px;height:120px;" >
                        <xsl:attribute name="src">
                          <xsl:value-of select="data/Table/SignedQrCode"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td  colspan="2">IRN</td>
                    <td  >
                      <xsl:value-of select="data/Table/IRN" />
                    </td>
                  </tr>
                  <tr>
                    <td  colspan="2" class="table-cell">Ack No</td>
                    <td  >
                      <xsl:value-of select="data/Table/IrnAckNo" />
                    </td>

                  </tr>
                  <tr>

                    <td  colspan="2" class="table-cell">Ack Date</td>
                    <td  class="table-cell">
                      <xsl:value-of  select="util:DateToDDMMYYYY(data/Table/IrnACKDate)"/>
                    </td>

                  </tr>

                </xsl:if>
                <xsl:if test="data/Table/Tnc != ''">
                  <tr class="table-row">
                    <td colspan="7" class="table-cell">
                      <xsl:value-of select="data/Table/Tnc"  disable-output-escaping="yes"/>
                    </td>
                  </tr>
                </xsl:if>
              </tbody>
            </table>


          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
