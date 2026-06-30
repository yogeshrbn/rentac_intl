<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <!-- Summary GST %: first line with non-empty Item and positive tax amount for that component; use line IGSTRate/CGSTRate/SGSTRate only (no reverse calc from taxable). -->
  <xsl:template name="summaryGstPercent">
    <xsl:param name="component"/>
    <xsl:variable name="line" select="/d/data/Table[normalize-space(Item) != '' and (($component = 'IGST' and number(IGST) > 0) or ($component = 'CGST' and number(CGST) > 0) or ($component = 'SGST' and number(SGST) > 0))][1]"/>
    <xsl:if test="$line">
      <xsl:choose>
        <xsl:when test="$component = 'IGST' and number($line/IGSTRate) > 0">
          <xsl:value-of select="format-number(number($line/IGSTRate), '#0.##')"/>
        </xsl:when>
        <xsl:when test="$component = 'CGST' and number($line/CGSTRate) > 0">
          <xsl:value-of select="format-number(number($line/CGSTRate), '#0.##')"/>
        </xsl:when>
        <xsl:when test="$component = 'SGST' and number($line/SGSTRate) > 0">
          <xsl:value-of select="format-number(number($line/SGSTRate), '#0.##')"/>
        </xsl:when>
      </xsl:choose>
    </xsl:if>
  </xsl:template>

  <xsl:template match="/">
    <xsl:variable name="qType" select="d/data/Table/QuotationType" />
    <xsl:variable name="rowsToSpan" select="d/rowsToSpan" />

    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        <style>

          @media print {

          body {
          font-size:18pt;
          font-family:arial;
          color: #111;
          margin: 0px;
          padding: 0;
          height: auto;
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
          font-size: 9pt;
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
          font-size: 11pt;
          color: #000;
          font-family: arial;
          padding:0px;
          }
          .no-page-break {
          page-break-inside: avoid !important;
          break-inside: avoid !important;
          -webkit-column-break-inside: avoid !important;
          -moz-column-break-inside: avoid !important;
          -ms-column-break-inside: avoid !important;
          -o-column-break-inside: avoid !important;
          column-break-inside: avoid !important;
          -fs-page-break-inside: avoid !important;
          }
          /* For table rows */
          tr.no-page-break {
          display: table-row;
          }

          /* For table cells */
          td.no-page-break {
          display: table-cell;
          }
          .allow-page-break {
          /* No page break restrictions */
          page-break-inside: auto;
          break-inside: auto;
          }
          }

        </style>
        <style>
          pre {
          background: none;
          overflow: hidden;
          border: none;
          font-size: 10pt;
          color: #000;
          font-family: arial;
          padding:0px;
          }
          p{margin-bottom:0px !important}
        </style>
      </head>
      <body>
        <div class="page">
          <div id="content">
            <xsl:if test="d/data/Table/QuotationType = 15">
              <table  border="0px"  cellspacing="0" class="table-container">
                <colgroup>
                  <col style="width:250px"/>
                  <col style="width:50px"/>
                  <col style="width:50px"/>
                  <col style="width:80px"/>


                  <col style="width:80px"/>

                  <col style="width:100px"/>
                </colgroup>
                <tbody class="table-body">
                  <tr class="table-row">
                    <td style="border:0px;"  colspan="2">
                      <img style="height:80px;max-width:250px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/CompanyLogo"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td  class="table-cell" colspan="4"     style="border:none;text-align:right;" >

                      <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">
                        <xsl:if test="d/data/Table/QuotationType = 15">RENTAL</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 16">CONTRACT</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 17">SALES</xsl:if>
                        <xsl:text> </xsl:text>
                        <xsl:choose>
                          <xsl:when test="d/data/Table/Category ='pi'">
                            PROFORMA INVOICE
                          </xsl:when>
                          <xsl:otherwise>
                            QUOTATION
                          </xsl:otherwise>
                        </xsl:choose>
                      </p>
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> #: <xsl:value-of select="d/data/Table/QuotationNumber" /><br />
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> Date:  <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/QuotationDate)"/> <br />
                      <p style="margin:0">
                        PO #: <xsl:value-of select="d/data/Table/poNumber" />
                      </p>
                      <p  style="margin:0">
                        PO Date #: <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/poDate)" />
                      </p>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="6"  class="table-cell">

                      <xsl:value-of select="d/data/Table/CompanyName" /><br />


                      <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                      <b>
                        GSTIN:
                      </b> <xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                      <b>MSME Number:</b> <xsl:value-of select="d/data/Table/MSMENumber" /> <xsl:text> | </xsl:text>
                      <b>Mobile: </b><xsl:value-of select="d/data/Table/CompanyPhone" />
                      <xsl:if test="d/data/Table/CompanyPhone2 != '' ">
                        , <xsl:value-of select="d/data/Table/CompanyPhone2" />
                      </xsl:if>
                      <br/>
                      <b>Email: </b><xsl:value-of select="d/data/Table/CompanyEmail" /> |  <strong>Web:</strong> <xsl:value-of select="d/data/Table/CompanyWeb" />
                      <br />
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">
                      <strong>Customer</strong>
                    </td>
                    <td colspan="4" class="table-cell">
                      <strong>Delivery Address</strong>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">
                      <xsl:value-of select="d/data/Table/PartyName" /><br />
                      <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                      Phone: -   <xsl:value-of select="d/data/Table/PartyPhone" /><br />
                      GSTIN: <xsl:value-of select="d/data/Table/PartyGST" /><br />
                    </td>
                    <td colspan="4" style="vertical-align:top;" class="table-cell">
                      <xsl:choose >
                        <xsl:when test="d/data/Table/LedgerSiteId > 0">
                          <xsl:if test="d/data/Table/SiteProject != ''">
                            <xsl:value-of select="d/data/Table/SiteProject" />
                            <xsl:text> </xsl:text>
                            <br/>
                          </xsl:if>
                          <xsl:value-of select="d/data/Table/SiteAddress" />
                          <xsl:text> </xsl:text>  <xsl:value-of select="d/data/Table/SiteAddress2" />  <br />
                          <xsl:value-of select="d/data/Table/SiteCity" /> ( <xsl:value-of select="d/data/Table/SiteState" /> ), PIN: -   <xsl:value-of select="d/data/Table/SiteZipCode" /><br />

                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                          <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />

                        </xsl:otherwise>
                      </xsl:choose>
                    </td>

                  </tr>
                  <tr class="table-row">

                    <th class="table-cell" style="width:40%;border-top:0px;">Product Name  Desc.</th>
                    <th  class="table-cell text-center" style="width:100px;border-top:0px;">Quantity</th>
                    <th  class="table-cell text-center" style="width:100px;border-top:0px;">UOM</th>
                    <th class="table-cell text-center" style="width: 100px;border-top:0px;">Rate</th>

                    <th  class="table-cell text-center" style="width: 100px;border-top:0px;">Duration</th>

                    <th class="table-cell text-right" style="width: 100px;border-top:0px;" >Amount</th>
                  </tr>
                  <xsl:for-each select="d/data/Table">
                    <tr class="table-row">
                      <td class="table-cell">
                        <span style="width:100%;">
                          <b>
                            <xsl:value-of select="Item" />
                          </b>
                        </span>
                        <!--  <span style="width:100%;display:block"> -->
                        <p style="padding:0px;margin:0px;font-family:Calibri;font-size:12pt;white-space: preserve;white-space: pre-wrap;">
                          <xsl:value-of select="Description"/>
                        </p>
                        <!--  </span> -->
                      </td>
                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Quantity" />
                      </td>
                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Unit" />
                      </td>
                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Rate" />
                      </td>

                      <td  class="table-cell text-center" >
                        <xsl:value-of select="duration" />
                      </td>

                      <td  class="table-cell text-right" >
                        <xsl:value-of select="format-number(ItemSubTotal,'#,##0.00')" />
                      </td>
                    </tr>
                  </xsl:for-each>
                  <xsl:if test="not(string(d/data/Table/tnc))">
                    <tr class="spacer-row">
                      <td class="table-cell"></td>
                      <td class="table-cell"></td>
                      <td class="table-cell"></td>
                      <td class="table-cell"></td>

                      <td class="table-cell"></td>

                      <td class="table-cell"></td>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <td class="table-cell" colspan="3" rowspan="{$rowsToSpan}" >

                      <div style="width:60%;float:left;">


                        <xsl:if test="d/Config/c/config[Key='printBankDetails']/Value='true'">
                          <ul style="list-style:none;">
                            <li>
                              <b>Bank Details</b>
                            </li>
                            <li>
                              Bank A/C No: <xsl:value-of select="d/data/Table/bankAccNumber"/>
                            </li>
                            <li>
                              Bank:  <xsl:value-of select="d/data/Table/bankName"/>
                            </li>
                            <li>
                              Branch Address:  <xsl:value-of select="d/data/Table/bankBranch"/>
                            </li>
                            <li>
                              IFSC Code: <xsl:value-of select="d/data/Table/IFSCCode"/>
                            </li>
                          </ul>
                        </xsl:if>
                      </div>
                      <div style="width:40%;float:right;text-align:right;">
                        <img style="max-width:120px;margin-right:20px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="d/data/Table/QrCode"/>
                          </xsl:attribute>
                        </img>
                      </div>


                    </td>
                    <th class="text-right table-cell" colspan="2"  >SubTotal</th>
                    <th style="border-top:0px;"  class="text-right table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >Freight (TO &amp; FRO)</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(d/data/Table/Freight,'#,##0.00')"/>
                    </th>
                  </tr>

                  <xsl:if test="d/data/Table/charge1 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge1,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge2 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge2,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge3 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"   >
                        Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge3,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge4 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge4,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge5 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Other Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge5,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>


                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">IGST<xsl:variable name="igstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'IGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($igstPct)) > 0"> (<xsl:value-of select="$igstPct"/>%)</xsl:if></th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">SGST<xsl:variable name="sgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'SGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($sgstPct)) > 0"> (<xsl:value-of select="$sgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">CGST<xsl:variable name="cgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'CGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($cgstPct)) > 0"> (<xsl:value-of select="$cgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <xsl:if test="d/data/Table/DiscountAmount > 0">
                    <tr>
                      <th class="text-right table-cell" colspan="2"  >Discount</th>
                      <th   class="text-right  table-cell" >
                        <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#,##0.00')" />
                      </th>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >Total</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(d/data/Table/Total,'#,##0.00')" />
                    </th>
                  </tr>


                  <tr   class="allow-page-break">
                    <td colspan="6" class="allow-page-break"  style="font-size:13px;border-bottom:none;padding:12px;">
                      <b style="text-decoration:underline"> Additional Information</b>
                      <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                  <!--<tr  class="no-page-break">
                    <td colspan="6" style="border-top:none;border-bottom:none;"  class="no-page-break">
                      <b style="text-decoration:underline">Terms and Conditions</b>
                      <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                    </td>
                  </tr>-->
                  <tr>
                    <td colspan="6" style="font-size:13px;border-top:none;border-bottom:none;padding:12px;">
                      <b style="text-decoration:underline">Terms and Conditions</b>
                      <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                  <tr  class="no-page-break">
                    <td class="no-page-break" colspan="3" style="vertical-align:bottom;
                        text-align:center;border-right:none;border-top:none;">
                      <i style="font-size:16pt;">Customer Acceptance (Sign here)</i>
                      <br />
                      <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>

                    </td>
                    <td colspan="3"  class="no-page-break" style="border-left:none; vertical-align:bottom;text-align:right;
                        border-top:none;">

                      <img style="max-width:100px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/Signature"/>
                        </xsl:attribute>
                      </img>
                      <br/>
                      <b style="font-size:20pt;">
                        For <i>
                          <xsl:value-of select="d/data/Table/CompanyName" />
                        </i>
                      </b>

                    </td>
                  </tr>
                </tbody>
              </table>
            </xsl:if>
            <xsl:if test="d/data/Table/QuotationType = 16">
              <table  border="0px"  cellspacing="0" class="table-container">
                <colgroup>
                  <col style="width:50px"/>
                  <col style="width:300px"/>
                  <col  style="width:150px"/>
                  <col style="width:100px"/>
                  <col style="width:70px"/>
                  <col style="width:80px"/>
                  <col style="width:100px"/>
                </colgroup>
                <tbody class="table-body">
                  <tr class="table-row">
                    <td style="border:0px;"  colspan="2">
                      <img style="height:80px;max-width:250px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/CompanyLogo"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td   colspan="5"     style="border:none;text-align:right;" >

                      <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">
                        <xsl:if test="d/data/Table/QuotationType = 15">RENTAL</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 16">CONTRACT</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 17">SALES</xsl:if>
                        <xsl:text> </xsl:text>
                        <xsl:choose>
                          <xsl:when test="d/data/Table/Category ='pi'">
                            PROFORMA INVOICE
                          </xsl:when>
                          <xsl:otherwise>
                            QUOTATION
                          </xsl:otherwise>
                        </xsl:choose>
                      </p>
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> #: <xsl:value-of select="d/data/Table/QuotationNumber" /><br />
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> Date:  <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/QuotationDate)"/> <br />
                      <p style="margin:0">
                        PO #: <xsl:value-of select="d/data/Table/poNumber" />
                      </p>
                      <p  style="margin:0">
                        PO Date #: <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/poDate)" />
                      </p>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="7"  class="table-cell">

                      <xsl:value-of select="d/data/Table/CompanyName" /><br />


                      <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                      <b>
                        GSTIN:
                      </b> <xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                      <b>MSME Number:</b> <xsl:value-of select="d/data/Table/MSMENumber" /> <xsl:text> | </xsl:text>
                      <b>Mobile: </b><xsl:value-of select="d/data/Table/CompanyPhone" />
                      <xsl:if test="d/data/Table/CompanyPhone2 != '' ">
                        , <xsl:value-of select="d/data/Table/CompanyPhone2" />
                      </xsl:if>
                      <br/>
                      <b>Email: </b><xsl:value-of select="d/data/Table/CompanyEmail" /> |  <strong>Web:</strong> <xsl:value-of select="d/data/Table/CompanyWeb" />
                      <br />
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="3" class="table-cell">
                      <strong>Customer</strong>
                    </td>
                    <td colspan="4" class="table-cell">
                      <strong>Delivery Address</strong>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="3" class="table-cell">
                      <xsl:value-of select="d/data/Table/PartyName" /><br />
                      <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                      Phone: -   <xsl:value-of select="d/data/Table/PartyPhone" /><br />
                      GSTIN: <xsl:value-of select="d/data/Table/PartyGST" /><br />
                    </td>
                    <td colspan="4" style="vertical-align:top;" class="table-cell">
                      <xsl:choose >
                        <xsl:when test="d/data/Table/LedgerSiteId > 0">
                          <xsl:if test="d/data/Table/SiteProject != ''">
                            <xsl:value-of select="d/data/Table/SiteProject" />
                            <xsl:text> </xsl:text>
                            <br/>
                          </xsl:if>
                          <xsl:value-of select="d/data/Table/SiteAddress" />
                          <xsl:text> </xsl:text>  <xsl:value-of select="d/data/Table/SiteAddress2" />  <br />
                          <xsl:value-of select="d/data/Table/SiteCity" /> ( <xsl:value-of select="d/data/Table/SiteState" /> ), PIN: -   <xsl:value-of select="d/data/Table/SiteZipCode" /><br />

                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                          <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />

                        </xsl:otherwise>
                      </xsl:choose>
                    </td>

                  </tr>
                  <tr class="table-row">
                    <th class="table-cell text-center" style="border-top:0px;">Sr.No</th>
                    <th class="table-cell" style="border-top:0px;">Product Name</th>
                    <th class="table-cell text-center" style="border-top:0px;">Duration</th>
                    <th class="table-cell text-center" style="border-top:0px;">Area</th>
                    <th class="table-cell text-center" style="border-top:0px;">Qty</th>
                    <th class="table-cell text-center" style="border-top:0px;">Total Area</th>
                    <th class="table-cell text-right" style="border-top:0px;">Total</th>
                  </tr>
                  <xsl:for-each select="d/data/Table[normalize-space(Item) != '']">
                    <tr class="table-row">
                      <td class="table-cell text-center">
                        <xsl:value-of select="position()" />
                      </td>
                      <td class="table-cell">
                        <span style="width:100%;">
                          <b>
                            <xsl:value-of select="Item" />
                          </b>
                        </span>
                        <p style="padding:0px;margin:0px;font-family:Calibri;font-size:12pt;white-space: preserve;white-space: pre-wrap;">
                          <xsl:value-of select="Description"/>
                        </p>
                      </td>
                      <td class="table-cell text-center">
                      
                     
                      
                        <xsl:if test="normalize-space(From) != ''">
                          <xsl:value-of select="util:DateToDDMMYYYY(From)" />
                        </xsl:if>
                        <xsl:text> to </xsl:text>
                        <xsl:if test="normalize-space(To) != ''">
                          <xsl:value-of select="util:DateToDDMMYYYY(To)" />
                        </xsl:if>
                      </td>
                      <td class="table-cell text-center">
                        <xsl:value-of select="Area" />
                      </td>
                      <td class="table-cell text-center">
                        <xsl:value-of select="Quantity" />
                        <xsl:text> </xsl:text> <xsl:value-of select="Unit" />
                      </td>
                      <td class="table-cell text-center">
                        <xsl:choose>
                          <xsl:when test="Area > 0">
                            <xsl:value-of select="format-number(Quantity * Area, '#,##0')" />
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="format-number(Quantity * /d/data/Table[1]/Area, '#,##0')" />
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td class="table-cell text-right">
                        <xsl:value-of select="format-number(ItemSubTotal,'#,##0.00')" />
                      </td>
                    </tr>
                  </xsl:for-each>
                  <tr class="spacer-row">
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                  </tr>
                  <!--<xsl:if test="not(string(d/data/Table/tnc))">
                  
                  </xsl:if>-->
                  <tr class="table-row">
                    <td class="table-cell" colspan="4" rowspan="{$rowsToSpan}" >

                      <div style="width:60%;float:left;">


                        <xsl:if test="d/Config/c/config[Key='printBankDetails']/Value='true'">
                          <ul style="list-style:none;">
                            <li>
                              <b>Bank Details</b>
                            </li>
                            <li>
                              Bank A/C No: <xsl:value-of select="d/data/Table/bankAccNumber"/>
                            </li>
                            <li>
                              Bank:  <xsl:value-of select="d/data/Table/bankName"/>
                            </li>
                            <li>
                              Branch Address:  <xsl:value-of select="d/data/Table/bankBranch"/>
                            </li>
                            <li>
                              IFSC Code: <xsl:value-of select="d/data/Table/IFSCCode"/>
                            </li>
                          </ul>
                        </xsl:if>
                      </div>
                      <div style="width:40%;float:right;text-align:right;">
                        <img style="max-width:120px;margin-right:20px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="d/data/Table/QrCode"/>
                          </xsl:attribute>
                        </img>
                      </div>


                    </td>
                    <td class="text-right table-cell" colspan="2"  >SubTotal</td>
                    <td style="border-top:0px;"  class="text-right table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#,##0.00')"/>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >Freight (TO &amp; FRO)</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(d/data/Table/Freight,'#,##0.00')"/>
                    </th>
                  </tr>

                  <xsl:if test="d/data/Table/charge1 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge1,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge2 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge2,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge3 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"   >
                        Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge3,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge4 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge4,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge5 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Other Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge5,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>


                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">IGST<xsl:variable name="igstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'IGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($igstPct)) > 0"> (<xsl:value-of select="$igstPct"/>%)</xsl:if></th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">SGST<xsl:variable name="sgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'SGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($sgstPct)) > 0"> (<xsl:value-of select="$sgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">CGST<xsl:variable name="cgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'CGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($cgstPct)) > 0"> (<xsl:value-of select="$cgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <xsl:if test="d/data/Table/DiscountAmount > 0">
                    <tr>
                      <th class="text-right table-cell" colspan="2"  >Discount</th>
                      <th   class="text-right  table-cell" >
                        <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#,##0.00')" />
                      </th>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >Total</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(d/data/Table/Total,'#,##0.00')" />
                    </th>
                  </tr>


                  <tr   class="allow-page-break">
                    <td colspan="7" class="allow-page-break"  style="font-size:13px;border-bottom:none;padding:12px;">
                      <b style="text-decoration:underline"> Additional Information</b>
                      <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                  <!--<tr  class="no-page-break">
                    <td colspan="6" style="border-top:none;border-bottom:none;"  class="no-page-break">
                      <b style="text-decoration:underline">Terms and Conditions</b>
                      <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                    </td>
                  </tr>-->
                  <tr>
                    <td colspan="7" style="font-size:13px;border-top:none;border-bottom:none;padding:12px;">
                      <b style="text-decoration:underline">Terms and Conditions</b>
                      <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                  <tr  class="no-page-break">
                    <td class="no-page-break" colspan="4" style="vertical-align:bottom;
                        text-align:center;border-right:none;border-top:none;">
                      <i style="font-size:16pt;">Customer Acceptance (Sign here)</i>
                      <br />
                      <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>

                    </td>
                    <td colspan="3"  class="no-page-break" style="border-left:none; vertical-align:bottom;text-align:right;
                        border-top:none;">

                      <img style="max-width:100px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/Signature"/>
                        </xsl:attribute>
                      </img>
                      <br/>
                      <b style="font-size:20pt;">
                        For <i>
                          <xsl:value-of select="d/data/Table/CompanyName" />
                        </i>
                      </b>

                    </td>
                  </tr>
                </tbody>
              </table>
            </xsl:if>

            <xsl:if test="d/data/Table/QuotationType = 17">
              <table  border="0px"  cellspacing="0" class="table-container" style="width:100%">
                <colgroup>
                  <col style="width:250px"/>
                  <col style="width:50px"/>
                  <col style="width:50px"/>
                  <col style="width:80px"/>




                  <col style="width:100px"/>
                </colgroup>
                <tbody class="table-body">
                  <tr class="table-row">
                    <td style="border:0px;"  colspan="2">
                      <img style="height:80px;max-width:250px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/CompanyLogo"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td  class="table-cell" colspan="3"     style="border:none;text-align:right;" >

                      <p style="margin:0px;padding:0px;font-size:18px;font-weight:bold;margin-bottom:5px;">
                        <xsl:if test="d/data/Table/QuotationType = 15">RENTAL</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 16">CONTRACT</xsl:if>
                        <xsl:if test="d/data/Table/QuotationType = 17">SALES</xsl:if>
                        <xsl:text> </xsl:text>
                        <xsl:choose>
                          <xsl:when test="d/data/Table/Category ='pi'">
                            PROFORMA INVOICE
                          </xsl:when>
                          <xsl:otherwise>
                            QUOTATION
                          </xsl:otherwise>
                        </xsl:choose>
                      </p>
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> #: <xsl:value-of select="d/data/Table/QuotationNumber" /><br />
                      <xsl:choose>
                        <xsl:when test="d/data/Table/Category ='pi'">
                          PI
                        </xsl:when>
                        <xsl:otherwise>
                          Quotation
                        </xsl:otherwise>
                      </xsl:choose> Date:  <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/QuotationDate)"/> <br />
                      <p style="margin:0">
                        PO #: <xsl:value-of select="d/data/Table/poNumber" />
                      </p>
                      <p  style="margin:0">
                        PO Date #: <xsl:value-of select="util:DateToDDMMYYYY(d/data/Table/poDate)" />
                      </p>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="5"  class="table-cell">

                      <xsl:value-of select="d/data/Table/CompanyName" /><br />


                      <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                      <b>
                        GSTIN:
                      </b> <xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                      <b>MSME Number:</b> <xsl:value-of select="d/data/Table/MSMENumber" /> <xsl:text> | </xsl:text>
                      <b>Mobile: </b><xsl:value-of select="d/data/Table/CompanyPhone" />
                      <xsl:if test="d/data/Table/CompanyPhone2 != '' ">
                        , <xsl:value-of select="d/data/Table/CompanyPhone2" />
                      </xsl:if>
                      <br/>
                      <b>Email: </b><xsl:value-of select="d/data/Table/CompanyEmail" /> |  <strong>Web:</strong> <xsl:value-of select="d/data/Table/CompanyWeb" />
                      <br />
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">
                      <strong>Customer</strong>
                    </td>
                    <td colspan="3" class="table-cell">
                      <strong>Delivery Address</strong>
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">
                      <xsl:value-of select="d/data/Table/PartyName" /><br />
                      <xsl:value-of select="d/data/Table/PartyAddress" />   <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/PartyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/PartyCity" />, PIN: -   <xsl:value-of select="d/data/Table/PartyZipCode" /><br />
                      Phone: -   <xsl:value-of select="d/data/Table/PartyPhone" /><br />
                      GSTIN: <xsl:value-of select="d/data/Table/PartyGST" /><br />
                    </td>
                    <td colspan="3" style="vertical-align:top;" class="table-cell">
                      <xsl:choose >
                        <xsl:when test="d/data/Table/LedgerSiteId > 0">
                          <xsl:if test="d/data/Table/SiteProject != ''">
                            <xsl:value-of select="d/data/Table/SiteProject" />
                            <xsl:text> </xsl:text>
                            <br/>
                          </xsl:if>
                          <xsl:value-of select="d/data/Table/SiteAddress" />
                          <xsl:text> </xsl:text>  <xsl:value-of select="d/data/Table/SiteAddress2" />  <br />
                          <xsl:value-of select="d/data/Table/SiteCity" /> ( <xsl:value-of select="d/data/Table/SiteState" /> ), PIN: -   <xsl:value-of select="d/data/Table/SiteZipCode" /><br />

                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="d/data/Table/ShipAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/ShipAddress2" /><br />
                          <xsl:value-of select="d/data/Table/ShipCity" /> ( <xsl:value-of select="d/data/Table/ShipStateName" /> ), PIN: -   <xsl:value-of select="d/data/Table/ShipZipCode" /><br />

                        </xsl:otherwise>
                      </xsl:choose>
                    </td>

                  </tr>
                  <tr class="table-row">

                    <th class="table-cell" style="width:40%;border-top:0px;">Product Name  Desc.</th>
                    <th  class="table-cell text-center" style="width:100px;border-top:0px;">Quantity</th>
                    <th  class="table-cell text-center" style="width:100px;border-top:0px;">UOM</th>
                    <th class="table-cell text-center" style="width: 100px;border-top:0px;">Rate</th>

                    <th class="table-cell text-right" style="width: 100px;border-top:0px;" >Amount</th>
                  </tr>
                  <xsl:for-each select="d/data/Table">
                    <tr class="table-row">
                      <td class="table-cell">
                        <span style="width:100%;">
                          <b>
                            <xsl:value-of select="Item" />
                          </b>
                        </span>
                        <span style="width:100%;display:block">
                          <pre style="padding:0px;margin:0px;">
                            <xsl:value-of select="Description"   disable-output-escaping="yes"/>
                          </pre>
                        </span>

                      </td>

                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Quantity" />
                      </td>
                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Unit" />
                      </td>
                      <td  class="table-cell text-center" >
                        <xsl:value-of select="Rate" />
                      </td>



                      <td  class="table-cell text-right" >

                        <xsl:value-of select="format-number(ItemSubTotal,'#,##0.00')" />
                      </td>
                    </tr>
                  </xsl:for-each>
                  <tr class="spacer-row">
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>



                    <td class="table-cell"></td>
                  </tr>
                  <tr class="table-row">
                    <td class="table-cell" colspan="2" rowspan="{$rowsToSpan}" >

                      <div style="width:60%;float:left;">


                        <xsl:if test="d/Config/c/config[Key='printBankDetails']/Value='true'">
                          <ul style="list-style:none;">
                            <li>
                              <b>Bank Details</b>
                            </li>
                            <li>
                              Bank A/C No: <xsl:value-of select="d/data/Table/bankAccNumber"/>
                            </li>
                            <li>
                              Bank:  <xsl:value-of select="d/data/Table/bankName"/>
                            </li>
                            <li>
                              Branch Address:  <xsl:value-of select="d/data/Table/bankBranch"/>
                            </li>
                            <li>
                              IFSC Code: <xsl:value-of select="d/data/Table/IFSCCode"/>
                            </li>
                          </ul>
                        </xsl:if>
                      </div>
                      <div style="width:40%;float:right;text-align:right;">
                        <img style="max-width:120px;margin-right:20px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="d/data/Table/QrCode"/>
                          </xsl:attribute>
                        </img>
                      </div>


                    </td>
                    <th class="text-right table-cell" colspan="2"  >SubTotal</th>
                    <th style="border-top:0px;"  class="text-right table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2" >Freight (TO &amp; FRO)</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(d/data/Table/Freight,'#,##0.00')"/>
                    </th>
                  </tr>

                  <xsl:if test="d/data/Table/charge1 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge1,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge2 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge2,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge3 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"   >
                        Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge3,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge4 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge4,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge5 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Other Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge5,'#,##0.00')"/>
                      </th>
                    </tr>
                  </xsl:if>


                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">IGST<xsl:variable name="igstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'IGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($igstPct)) > 0"> (<xsl:value-of select="$igstPct"/>%)</xsl:if></th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">SGST<xsl:variable name="sgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'SGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($sgstPct)) > 0"> (<xsl:value-of select="$sgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2">CGST<xsl:variable name="cgstPct"><xsl:call-template name="summaryGstPercent"><xsl:with-param name="component" select="'CGST'"/></xsl:call-template></xsl:variable><xsl:if test="string-length(normalize-space($cgstPct)) > 0"> (<xsl:value-of select="$cgstPct"/>%)</xsl:if></th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#,##0.00')"/>
                    </th>
                  </tr>
                  <xsl:if test="d/data/Table/DiscountAmount > 0">
                    <tr>
                      <th class="text-right table-cell" colspan="2"  >Discount</th>
                      <th   class="text-right  table-cell" >
                        <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#,##0.00')" />
                      </th>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >Total</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(d/data/Table/Total,'#,##0.00')" />
                    </th>
                  </tr>
                  <tr  class="no-page-break">
                    <td colspan="5" class="no-page-break" style="font-size:13px;border-bottom:none;">

                      <b style="text-decoration:underline"> Additional Information</b>
                      <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>

                    </td>
                  </tr>
                  <tr  class="no-page-break">
                    <td colspan="5" style="border-top:none;border-bottom:none;" class="no-page-break">
                      <b style="text-decoration:underline">Terms and Conditions</b>
                      <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                  <tr  class="no-page-break">
                    <td  class="no-page-break" colspan="3" style="vertical-align:bottom;text-align:center;border-right:none;
                         border-top:none;">
                      <i style="font-size:14pt;">Customer Acceptance (Sign here)</i>
                      <br />
                      <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>

                    </td>
                    <td colspan="2"  class="no-page-break" style="border-left:none; vertical-align:bottom;text-align:right;border-top:none;">

                      <img style="max-width:100px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="d/data/Table/Signature"/>
                        </xsl:attribute>
                      </img>
                      <br/>
                      <b>
                        For <i>
                          <xsl:value-of select="d/data/Table/CompanyName" />
                        </i>
                      </b>

                    </td>
                  </tr>


                </tbody>
              </table>
            </xsl:if>
            <xsl:if test="count(d/items/name1) > 0 and d/data/Table/QuotationType != 17 ">
              <table style="width:100%;height:auto;" class="table-container itemtable"  cellpadding="0" cellspacing="0">
                <colgroup>
                  <col width="50px"/>
                  <col width="400px"/>
                  <col width="70px"/>
                  <col width="100px"/>
                  <col width="400px"/>
                  <col width="70px"/>
                  <col width="100px"/>
                </colgroup>
                <tbody class="table-body">
                  <tr class="table-row">
                    <td class="table-cell" style="font-weight:bold">Sr.No</td>
                    <td class="table-cell" style="font-weight:bold;width:450px;">Name Of Item</td>
                    <td class="table-cell text-center"  style="border-right:0px;font-weight:bold;width:30px;">UOM</td>
                    <td class="table-cell" style="border-right:0px;font-weight:bold;width:30px;text-align:right;">Cost</td>
                    <td class="table-cell" style="border-right:0px;font-weight:bold;width:450px;">Name Of Item</td>
                    <td class="table-cell  text-center" style="border-right:0px;font-weight:bold;width:30px;">UOM</td>
                    <td class="table-cell" style="font-weight:bold;width:30px;text-align:right;">Cost</td>
                  </tr>
                  <xsl:for-each select="d/items">

                    <tr class="table-row">
                      <td class="table-cell" style=" text-align:center;">
                        <xsl:value-of select="position()" />
                      </td>
                      <td  class="table-cell">
                        <xsl:value-of select="name1" />
                      </td>
                      <td  class="table-cell  text-center">
                        <xsl:value-of select="unit1" />
                      </td>
                      <td class="table-cell" style="text-align:right;">
                        <xsl:value-of select="cost1" />
                      </td>

                      <td class="table-cell" style="width:250px;">
                        <xsl:value-of select="name2" />
                      </td>
                      <td class="table-cell  text-center"  >
                        <xsl:value-of select="unit2" />
                      </td>
                      <td class="table-cell" style="text-align:right;">
                        <xsl:value-of select="cost2" />
                      </td>

                    </tr>
                  </xsl:for-each>
                  <tr class="spacer-row">
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                    <td class="table-cell"></td>
                  </tr>
                </tbody>
              </table>
            </xsl:if>

          </div>
        </div>
      </body>
    </html>


  </xsl:template>
</xsl:transform>