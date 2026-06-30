<?xml version="1.0" encoding="utf-8"?>


<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>

        #preview
        #pdf
        <style>
          .taxRow td {font-size:9px;}
          th {font-weight:bold !important;}
          .totalrow td {
          background-color:#e3e3e3;
          }
          th, td {
          border-color:#3a3a3a;
          }
        </style>

        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:0px;">
          <style>
            th{
            font-weight:bold;
            }
            table th td p {
            font-family:calibri;
            font-size: 14px;
            }
            .noborder {border:0px;

            }
            p {
            margin:0px;
            }

            body {
            margin-left: 0.1in;
            margin-right: 0.1in
            }
            th, td {
            font-size:12px;

            border-style:thin;
            font-weight:normal;
            padding:2px;
            }
            td,p{
            margin:0;

            }
            .br-0{
            border-right:0;
            }
            .bt-0{
            border-top:0;
            }
            .bl-0{
            border-left:0;
            }
            .bb-0{
            border-bottom:0;
            }
          </style>
          <table cellpadding="0" cellspacing="0" style="width:100%" class="noborder">
            <tr>
              <td colspan="2" rowspan="4" class="br-0 bb-0">
                <table>
                  <tr>
                    <td class="noborder">
                      <img style="height:60px;max-width:150px;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="data/Table/CompanyLogo"/>
                        </xsl:attribute>
                      </img>
                    </td>
                    <td class="noborder" style="vertical-align:top;">
                      <ul style="list-style:none;">
                        <li>
                          <xsl:value-of select="data/Table/Company" />
                        </li>
                        <li>
                          <xsl:value-of select="data/Table/CompanyAddress1" />
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="data/Table/CompanyAddress2" />
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="data/Table/CompanyCity" />
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="data/Table/CompanyZipCode" />
                        </li>
                        <li>
                          GSTIN/UIN:  <xsl:value-of select="data/Table/CompanyGST" />
                        </li>
                        <li>
                          State Name :      <xsl:value-of select="data/Table/CompanyStateName" />, Code : <xsl:value-of select="data/Table/CompanyStateCode" /> CIN: <xsl:value-of select="data/Table/CompanyCIN" />
                        </li>
                        <li>
                          E-Mail :   <xsl:value-of select="data/Table/CompanyEmail" />
                        </li>
                      </ul>
                    </td>
                  </tr>
                </table>


              </td>
              <td  class="br-0 bb-0 text-center" colspan="4">
                <p >
                  Invoice No
                </p>
                <p >
                  <strong>
                    <xsl:value-of select="data/Table/InvoiceNumber" />
                  </strong>
                </p>
              </td>
              <td colspan="4"  class="bb-0 text-center" >
                <p >
                  Dated
                </p>
                <p >
                  <strong>
                    <xsl:value-of select="util:DateToDDMMYYYY(data/Table/InvoiceDate)"/>
                  </strong>
                </p>

              </td>

            </tr>

            <tr>
              <td class="br-0  bb-0 text-center" colspan="4">
                <p >
                  Delivery Note
                </p>
                <p >
                  -
                </p>

              </td>
              <td colspan="4" class="bb-0 text-center" >
                <p >
                  Mode/Terms of Payment
                </p>
                <p >
                  -
                </p>

              </td>
            </tr>
            <tr>
              <td class="br-0 bb-0 text-center" colspan="4">
                <p >
                  Reference No. &amp; Date
                </p>
                <p >
                  -
                </p>

              </td>
              <td colspan="4" class="bb-0 text-center" >
                <p >
                  Other References
                </p>
                <p >
                  -
                </p>

              </td>

            </tr>
            <tr>
              <td class="br-0  bb-0 text-center" colspan="4">
                <p >
                  Buyer's Order No
                </p>
                <p >
                  -
                </p>

              </td>
              <td colspan="4"  class="bb-0 text-center">
                <p >
                  Dated
                </p>
                <p >
                  -
                </p>

              </td>
            </tr>

            <tr>
              <td colspan="2" rowspan="3" class="br-0  bb-0">
                <p>Consignee (Ship to)</p>

                <strong> <xsl:value-of select="data/Table/Client" />
                </strong><br />
                <xsl:value-of select="data/Table/ClientShipAddress1" />  <xsl:value-of select="data/Table/ClientShipAddress2" /><br />
                <xsl:value-of select="data/Table/ClientShipCity" />, PIN: -   <xsl:value-of select="data/Table/ClientShipZipCode" /><br />
                GSTIN: <xsl:value-of select="data/Table/ClientGST" /><br />
                State Name: <xsl:value-of select="data/Table/ClientShipStateName" />, Code: <xsl:value-of select="data/Table/ClientShipStateCode" />, <br />

                <hr/>
                <p>Buyer (Bill to)</p>
                <strong><xsl:value-of select="data/Table/Client" />
                </strong><br />
                <xsl:value-of select="data/Table/ClientAddress1" />  <xsl:value-of select="data/Table/ClientAddress2" /><br />
                <xsl:value-of select="data/Table/ClientCity" />, PIN: -   <xsl:value-of select="data/Table/ClientZipCode" /><br />
                GSTIN: <xsl:value-of select="data/Table/ClientGST" /><br />
                State Name: <xsl:value-of select="data/Table/ClientStateName" />, Code: <xsl:value-of select="data/Table/ClientStateCode" />, <br />

              </td>
              <td class="br-0 bb-0 text-center" colspan="4">
                <p >
                  Dispatch Doc No
                </p>
                <p >
                  -
                </p>

              </td>
              <td colspan="4" class="text-center bb-0" >
                <p >
                  Delivery Note Date
                </p>
                <p >
                  -
                </p>

              </td>
            </tr>
            <tr>
              <td class="br-0  bb-0 text-center" colspan="4">
                <p >
                  Dispatched through
                </p>
                <p >
                  -
                </p>


              </td>
              <td colspan="4" class="text-center  bb-0"  >
                <p >
                  Destination
                </p>
                <p >
                  -
                </p>

              </td>
            </tr>

            <tr>
              <td colspan="8" class="bb-0" style="vertical-align:top;"  >
                Terms of Delivery
              </td>

            </tr>
            <tr>
              <th class="br-0  bb-0 text-center" style="width:30px;">Sl. No</th>
              <th style="width:450px;" class="br-0  bb-0">
                Description of

                Services
              </th>
              <th class="br-0  bb-0" style="width:50px;">HSN/SAC</th>
              <th class="br-0  bb-0 text-center" style="width:50px;">Quantity</th>
              <th class="br-0  bb-0 text-center" style="width:40px;">Rate</th>
              <th class="br-0  bb-0 text-center" style="width:40px;">Per</th>
              <th class="br-0  bb-0 text-center" style="width:40px;">Disc %</th>
              <th style="width:210px;" class="bb-0 text-right" colspan="3">Amount</th>
            </tr>
            <xsl:for-each select="data/Table">
              <tr>
                <td class="br-0  bb-0 text-center">
                  <xsl:value-of select="position()" />
                </td>
                <td class="br-0  bb-0">
                  <xsl:value-of select="Item" />
                </td>
                <td class="br-0  bb-0">
                  <xsl:value-of select="HSNCode" />
                </td>
                <td class="br-0  bb-0 text-center">
                  <xsl:value-of select="Quantity" />
                </td>
                <td class="br-0  bb-0 text-center">
                  <xsl:value-of select="Rate" />
                </td>
                <td class="br-0  bb-0 text-center">
                  <xsl:value-of select="UnitName" />
                </td>
                <td class="br-0  bb-0 text-center"></td>
                <td class="bb-0 text-right" colspan="3">
                  <xsl:value-of select="SubTotal" />
                </td>

              </tr>
            </xsl:for-each>
            <tr class="totalrow">
              <td class="br-0  bb-0"></td>
              <td class="br-0  bb-0">
                <strong>Total</strong>
              </td>
              <td class="br-0  bb-0"></td>
              <td class="br-0  bb-0"></td>
              <td class="br-0  bb-0"></td>
              <td class="br-0  bb-0"></td>
              <td class="br-0  bb-0"></td>
              <td class="bb-0 text-right" colspan="3">
                <strong><xsl:value-of select="sum(data/Table/SubTotal)"/></strong> 
              </td>
            </tr>
            <tr>
              <td colspan="10"  class="bb-0" style="line-height:50px;">
                Amount Chargeable (in words)
                <xsl:value-of select="util:AmountToWords(sum(data/Table/SubTotal))"/>
              </td>
            </tr>
            <tr class="taxRow">
              <th colspan="2" rowspan="2"  class="br-0 bb-0" >HSN/SAC</th>

              <th rowspan="2" class="br-0 bb-0 text-center">Taxable Value</th>
              <th colspan="2" class="br-0 bb-0 text-center">IGST</th>
              <th colspan="2" class="br-0 bb-0 text-center">CGST</th>
              <th colspan="2" class="br-0 bb-0 text-center">SGST</th>
              <th rowspan="2" class="bb-0 text-center">Total</th>
            </tr>
            <tr class="taxRow">
              <td class="br-0 bb-0  text-center">Rate</td>
              <td class="br-0 bb-0  text-center">Amount</td>
              <td class="br-0 bb-0  text-center">Rate</td>
              <td class="br-0 bb-0  text-center">Amount</td>
              <td class="br-0 bb-0  text-center" style="width:30px;" >Rate</td>
              <td class="bb-0 br-0  text-center"  style="width:40px;" >Amount</td>

            </tr>
            <xsl:for-each select="data/Table5">
              <tr class="taxRow">
                <td colspan="2" class="br-0 bb-0">
                  <xsl:value-of select="HSNCode"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="TaxAbleValue"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="IGSTRate"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="IGST"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="CGSTRate"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="CGST"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="SGSTRate"/>
                </td>
                <td class="br-0 bb-0 text-center">
                  <xsl:value-of select="SGST"/>
                </td>
                <td class="bb-0 text-center">
                  <xsl:value-of select="CGST + IGST + SGST"/>
                </td>

              </tr>
            </xsl:for-each>
            <tr class="taxRow totalrow">
              <td colspan="2" class="br-0 bb-0">
                <strong>Total</strong>
              </td>
              <td class="br-0 bb-0 text-center">
                <xsl:value-of select="sum(data/Table5/TaxAbleValue)"/>
              </td>
              <td class="br-0 bb-0">

              </td>
              <td class="br-0 bb-0 text-center">
                <xsl:value-of select="sum(data/Table5/IGST)"/>
              </td>
              <td class="br-0 bb-0">

              </td>
              <td class="br-0 bb-0 text-center">
                <xsl:value-of select="sum(data/Table5/CGST)"/>
              </td>
              <td class="br-0 bb-0 ">

              </td>
              <td class="br-0 bb-0 text-center">
                <xsl:value-of select="sum(data/Table5/SGST)"/>
              </td>
              <td class="bb-0 text-center">
                <xsl:value-of select="sum(data/Table5/IGST) + sum(data/Table5/SGST) + sum(data/Table5/CGST)"/>
              </td>
            </tr>
            <tr>
              <td colspan="2" class="br-0 bb-0"  >
                Tax Amount (in words) :  <xsl:value-of select="util:AmountToWords(sum(data/Table5/IGST) + sum(data/Table5/SGST) + sum(data/Table5/CGST))"/>
              </td>
              <td colspan="8" class="bb-0" style="text-align:left;padding-left:0;">

                <ul style="list-style:none;">
                  <li>
                    A/c Holder's Name: <xsl:value-of select="data/Table/Company" />
                  </li>
                  <li>
                    Bank Name: <xsl:value-of select="data/Table/CompanyBankName" />
                  </li>
                  <li>
                    A/c No: <xsl:value-of select="data/Table/CompanyBankAccNumber" />
                  </li>
                  <li>
                    IFS Code: <xsl:value-of select="data/Table/CompanyBankIFSC" />
                  </li>
                  <li>
                    Branch: <xsl:value-of select="data/Table/CompanyBankBranch" />
                  </li>
                </ul>
              </td>
            </tr>
            <tr>
              <td colspan="2" class="br-0">
                Company's PAN :  <xsl:value-of select="data/Table/CompanyPAN" />
                <p>
                  <u>Declaration</u>
                </p>
                We declare that this invoice shows the actual price of the goods
                described and that all particulars are true and correct.
              </td>
              <td colspan="8" class="text-right">
                <p>
                  For    <xsl:value-of select="data/Table/Company" />
                </p>
                <p style="margin-top:30px;">
                  Authorised Signatory
                </p>
              </td>
            </tr>

          </table>
        </div>

      </body>
    </html>



  </xsl:template>
</xsl:transform>
