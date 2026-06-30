<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template name="getColspan">
    <xsl:param name="baseCol"/>
    <xsl:attribute name="colspan">
      <xsl:choose>
        <xsl:when test="d/data/Table/QuotationType != 17">4</xsl:when>
        <xsl:otherwise>3</xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
  </xsl:template>
  <xsl:template match="/">
    <xsl:variable name="qType" select="d/data/Table/QuotationType" />

    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview

        <style>
          @media print {

          body {
          font-family: arial;
          font-size: 13px;
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

          .itemtable td {
          font-size:10px;
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
          }
        </style>
      </head>
      <body>
        <div class="page">
          <div id="content">
            <xsl:if test="d/data/Table/QuotationType != 17">
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
                      <img style="height:60px;max-width:250px;">
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
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="6"  class="table-cell">

                      <xsl:value-of select="d/data/Table/CompanyName" /><br />


                      <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                      <strong>
                        GSTIN:
                      </strong> <xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                      <strong>MSME Number:</strong> <xsl:value-of select="d/data/Table/MSMENumber" /> <xsl:text> | </xsl:text>
                      <strong>Mobile: </strong><xsl:value-of select="d/data/Table/CompanyPhone" />
                      <xsl:if test="d/data/Table/CompanyPhone2 != '' ">
                        , <xsl:value-of select="d/data/Table/CompanyPhone2" />
                      </xsl:if>
                      <br/>
                      <strong>Email: </strong><xsl:value-of select="d/data/Table/CompanyEmail" /> |  <strong>Web:</strong> <xsl:value-of select="d/data/Table/CompanyWeb" />
                      <br />
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">Customer</td>
                    <td colspan="4" class="table-cell">Delivery Address</td>
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

                      <td  class="table-cell text-center" >
                        <xsl:value-of select="duration" />
                      </td>

                      <td  class="table-cell text-right" >
                        <xsl:value-of select="ItemSubTotal" />
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
                  </tr>
                  <tr class="table-row">
                    <td class="table-cell" colspan="3" rowspan="6" >

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
                      <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >Freight (TO &amp; FRO)</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(d/data/Table/Freight,'#.00')"/>
                    </th>
                  </tr>

                  <xsl:if test="d/data/Table/charge1 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge1,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge2 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge2,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge3 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"   >
                        Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge3,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge4 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge4,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge5 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Other Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge5,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>


                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >IGST</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >SGST</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >CGST</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#.00')"/>
                    </th>
                  </tr>
                  <xsl:if test="d/data/Table/DiscountAmount > 0">
                    <tr>
                      <th class="text-right table-cell" colspan="2"  >Discount</th>
                      <th   class="text-right  table-cell" >
                        <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#.00')" />
                      </th>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >Total</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
                    </th>
                  </tr>


                  <tr>
                    <td colspan="3" style="font-size:11px; border-right:none;">
                      <xsl:if test="d/data/Table/AddInfo != ''">

                        <h4>  Additional Information</h4>
                        <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>

                      </xsl:if>

                      <div style="text-align:left;width:100%;font-size:11px;">
                        <b style="text-decoration:underline">Terms and Conditions</b>
                        <br/>

                        <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                      </div>
                      <br/>
                      <i>Customer Acceptance (Sign here)</i>
                      <br />
                      <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>

                    </td>
                    <td colspan="3" style="border-left:none; vertical-align:bottom;text-align:right">

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
            <xsl:if test="d/data/Table/QuotationType = 17">
              <table  border="0px"  cellspacing="0" class="table-container">
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
                      <img style="height:60px;max-width:250px;">
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
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="5"  class="table-cell">

                      <xsl:value-of select="d/data/Table/CompanyName" /><br />


                      <xsl:value-of select="d/data/Table/CompanyAddress1" />  <xsl:text> </xsl:text> <xsl:value-of select="d/data/Table/CompanyAddress2" /><br />
                      <xsl:value-of select="d/data/Table/CompanyCity" />, PIN: -   <xsl:value-of select="d/data/Table/CompanyZipCode" /><br />
                      <strong>
                        GSTIN:
                      </strong> <xsl:value-of select="d/data/Table/CompanyGST" /> <xsl:text> | </xsl:text>
                      <strong>MSME Number:</strong> <xsl:value-of select="d/data/Table/MSMENumber" /> <xsl:text> | </xsl:text>
                      <strong>Mobile: </strong><xsl:value-of select="d/data/Table/CompanyPhone" />
                      <xsl:if test="d/data/Table/CompanyPhone2 != '' ">
                        , <xsl:value-of select="d/data/Table/CompanyPhone2" />
                      </xsl:if>
                      <br/>
                      <strong>Email: </strong><xsl:value-of select="d/data/Table/CompanyEmail" /> |  <strong>Web:</strong> <xsl:value-of select="d/data/Table/CompanyWeb" />
                      <br />
                    </td>
                  </tr>
                  <tr class="table-row">
                    <td colspan="2" class="table-cell">Customer</td>
                    <td colspan="3" class="table-cell">Delivery Address</td>
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

                    <th class="table-cell"  >Product Name  Desc.</th>
                    <th  class="table-cell text-center"  >Quantity</th>
                    <th  class="table-cell text-center" >UOM</th>
                    <th class="table-cell text-center"  >Rate</th>

                    <th class="table-cell text-right"  >Amount</th>
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
                        <xsl:value-of select="ItemSubTotal" />
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
                    <td class="table-cell" colspan="2" rowspan="6" >

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
                    <th class="text-right table-cell"  colspan="2"  >SubTotal</th>
                    <th style="border-top:0px;"  class="text-right table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/ItemSubTotal),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >Freight (TO &amp; FRO)</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(d/data/Table/Freight,'#.00')"/>
                    </th>
                  </tr>

                  <xsl:if test="d/data/Table/charge1 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge1,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge2 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-loading Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge2,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge3 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"   >
                        Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge3,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge4 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Un-Installation Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge4,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>
                  <xsl:if test="d/data/Table/charge5 > 0">
                    <tr class="table-row">
                      <th class="text-right table-cell" colspan="2"  >
                        Other Charges
                      </th>
                      <th  class="text-right table-cell">
                        <xsl:value-of select="format-number(d/data/Table/charge5,'#.00')"/>
                      </th>
                    </tr>
                  </xsl:if>


                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"   >IGST</th>
                    <th  class="text-right table-cell">
                      <xsl:value-of select="format-number(sum(d/data/Table/IGST),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >SGST</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/SGST),'#.00')"/>
                    </th>
                  </tr>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >CGST</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(sum(d/data/Table/CGST),'#.00')"/>
                    </th>
                  </tr>
                  <xsl:if test="d/data/Table/DiscountAmount > 0">
                    <tr>
                      <th class="text-right table-cell" colspan="2"  >Discount</th>
                      <th   class="text-right  table-cell" >
                        <xsl:value-of select="format-number(d/data/Table/DiscountAmount,'#.00')" />
                      </th>
                    </tr>
                  </xsl:if>
                  <tr class="table-row">
                    <th class="text-right table-cell" colspan="2"  >Total</th>
                    <th   class="text-right  table-cell" >
                      <xsl:value-of select="format-number(d/data/Table/Total,'#.00')" />
                    </th>
                  </tr>


                  <tr>
                    <td colspan="3" style="font-size:11px; border-right:none;">
                      <xsl:if test="d/data/Table/AddInfo != ''">

                        <h4>  Additional Information</h4>
                        <xsl:value-of select="d/data/Table/AddInfo" disable-output-escaping="yes"/>

                      </xsl:if>

                      <div style="text-align:left;width:100%;font-size:11px;">
                        <b style="text-decoration:underline">Terms and Conditions</b>
                        <br/>

                        <xsl:value-of select="d/data/Table/tnc" disable-output-escaping="yes"/>
                      </div>
                      <br/>
                      <i>Customer Acceptance (Sign here)</i>
                      <br />
                      <span style="width: 50%; display: inline-block;margin-top:10px; border-bottom: solid 1px #212121;"> </span>

                    </td>
                    <td colspan="2" style="border-left:none; vertical-align:bottom;text-align:right">

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
              <table style="width:100%;height:auto;" class="table-container"  cellpadding="0" cellspacing="0">
                <colgroup>
                  <col width="40px"/>
                  <col width="400px"/>
                  <col width="80px"/>
                  <col width="100px"/>
                  <col width="400px"/>
                  <col width="80px"/>
                  <col width="100px"/>
                </colgroup>
                <tbody class="table-body">
                  <tr class="table-row">
                    <td class="table-cell" style="font-weight:bold">Sr.No</td>
                    <td class="table-cell" style="font-weight:bold;width:450px;">Name Of Item</td>
                    <td class="table-cell"  style="border-right:0px;font-weight:bold;width:30px;">UOM</td>
                    <td class="table-cell" style="border-right:0px;font-weight:bold;width:30px;text-align:right;">Cost</td>
                    <td class="table-cell" style="border-right:0px;font-weight:bold;width:450px;">Name Of Item</td>
                    <td class="table-cell" style="border-right:0px;font-weight:bold;width:30px;">UOM</td>
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
                      <td  class="table-cell">
                        <xsl:value-of select="unit1" />
                      </td>
                      <td class="table-cell" style="text-align:right;">
                        <xsl:value-of select="cost1" />
                      </td>

                      <td class="table-cell" style="width:250px;">
                        <xsl:value-of select="name2" />
                      </td>
                      <td class="table-cell"  >
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
