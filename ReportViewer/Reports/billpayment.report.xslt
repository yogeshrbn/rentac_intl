<?xml version="1.0" encoding="utf-8"?>


<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          @media print {
          <!--.td-heading {
          line-height:20px;
          padding:0px;
          }
          .headerUL li  {
          padding-bottom:8px;
          padding-top:0px;
          margin:0px;
          }
          td,th {border:none;}
          td {
          line-height:20px;
          padding:0px !important;
          padding-left:3px !important;


          font-size:12px;

          }
          th {
          padding:0px !important;
          padding-left:3px !important;
          }-->
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
          td,th {
          border:none;
          font-size:12px;
          }
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body style="margin:0px;">
        <div id="printArea" style="padding:5px !important;margin:0px;">

          <table style="width:100%;border-collapse:collapse;">
            <colgroup>
              <col  style="width:350px"/>
              <col/>
            </colgroup>

            <tr>
              <td class="text-center noborder" colspan="2">
                <ul class="headerUL" style="list-style:none;padding:none;">
                  <li>
                    <h2 style="color:green;padding:0px;;margin:0px;">
                      <xsl:value-of select="d/company/Name"/>
                    </h2>
                  </li>
                  <li  >
                    <xsl:value-of select="d/company/Address1"/>
                    <xsl:if test="d/company/Address2 != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/Address2"/>
                    </xsl:if>

                    <xsl:if test="d/company/City != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/City"/>
                    </xsl:if>
                    <xsl:if test="d/company/State != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="d/company/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone No.: </b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="d/company/Phone1"/>
                  </li>
                </ul>


              </td>
            </tr>
            <tr>
              <td colspan="2" class="noborder td-heading">
                <strong>Party</strong>:   <xsl:value-of select="d/client/Name"/>
              </td>
            </tr>
            <tr>
              <td  colspan="2" class="noborder td-heading">
                <strong>Address</strong>:   <xsl:value-of select="d/client/Address1"/>   <xsl:text> </xsl:text>
                <xsl:value-of select="d/client/Address2"/>   <xsl:text> </xsl:text><xsl:value-of select="d/client/City"/>
                <xsl:text> </xsl:text><xsl:value-of select="d/client/State"/>   <xsl:text> </xsl:text>

                <hr style="border-color:#000" />
              </td>

            </tr>
            <tr>
              <td colspan="2" class="text-center noborder" style="line-height:30px;">
                Bill Payment Report between  <xsl:value-of select="util:DateToDDMMYYYY(d/from)"/> and  <xsl:value-of select="util:DateToDDMMYYYY(d/to)"/>
              </td>
            </tr>
            <tr>
              <td colspan="2" style="font-size:14px;line-height:40px;">
                <strong>Opening Balance:</strong>
                <xsl:text> </xsl:text>
                <xsl:choose>
                  <xsl:when test="d/openingBalance > 0">
                    <xsl:value-of select="d/openingBalance"/> CR
                   </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="(d/openingBalance) * -1"/> DR
                  </xsl:otherwise>
                </xsl:choose>
                
              </td>
            </tr>
            <tr>
              <td  class="noborder text-center">
                <strong>Bill Detail</strong>
              </td>
              <td   class="noborder text-center">
                <strong>Payment Detail</strong>
              </td>
            </tr>
            <tr  class="no-page-break">
              <td    style="vertical-align:top;"  >
                <table style="width:100%;border-collapse:collapse;page-break-inside: avoid;">
                  <tr>
                    <th style="wdith:250px;">BillNo</th>
                    <th style="width:30px;text-align:center;"> Bill Date</th>

                    <th class="text-right;" style="max-wdith:100px; text-align:right">Bill Amount</th>

                  </tr>
                  <xsl:for-each select="d/reportData">
                    <tr>
                      <td>
                        <xsl:value-of select ="InvoiceNumber"/>
                      </td>
                      <td style="text-align:center;">
                        <xsl:value-of select ="util:DateToDDMMYYYY(InvoiceDate)"/>
                      </td>
                      <td class="text-right">
                        <xsl:value-of select ="util:FormatNumber(Total)"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>
              </td>
              <td  style="vertical-align:top;" >
                <table style="width:100%;border-collapse:collapse;page-break-inside:avoid;" >
                  <tr>
                    <!--<th style="width:20pt"></th>-->
                    <th  style="width:120px;text-align:center">Payment Date</th>
                    <th class="text-right"  style="width:120px;">Paid Amount</th>

                    <th  style="width:120px;text-align:center;">Pay Mode</th>

                    <th  style="width:120px;">Cheque No</th>
                    <th  style="width:130px;">Remarks</th>
                  </tr>
                  <xsl:for-each select="d/payments">
                    <tr  style="page-break-inside: avoid;">
                      <!--<td></td>-->
                      <td style="text-align:center">
                        <xsl:value-of select ="util:DateToDDMMYYYY(TransactionDate)"/>
                      </td>
                      <td class="text-right">
                        <xsl:value-of select ="util:FormatNumber(TransactionAmount)"/>
                      </td>

                      <td style="text-align:center;">

                        <xsl:choose>
                          <xsl:when test="TransactionMode = 1">
                            Cash
                          </xsl:when>
                          <xsl:when test="TransactionMode = 2">
                            Bank
                          </xsl:when>
                          <xsl:when test="TransactionMode = 3">
                            Cheque
                          </xsl:when>
                          <xsl:when test="TransactionMode = 4">
                            NEFT/RTGS
                          </xsl:when>
                          <xsl:when test="TransactionMode = 5">
                            OTHERS
                          </xsl:when>
                          <xsl:when test="TransactionMode = 6">
                            UPI
                          </xsl:when>
                        </xsl:choose>

                      </td>
                      <td>
                        <xsl:value-of select ="ChequeNumber"/>
                      </td>
                      <td>
                        <p style="text-overflow:hidden;width:130px;">
                          <xsl:value-of select ="Narration"/>
                        </p>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>


              </td>
            </tr>
            <tr>

              <td   style="border-top:solid 2px;" colspan="2">
                <table style="width:100%;border-collapse:collapse; page-break-inside:avoid;">

                  <tr>
                    <td  style="width:85%;padding:0px;" class="noborder text-right">
                      <strong>Total Bill Amount :</strong>
                    </td>
                    <td   class="noborder text-right" style="padding:0px;">
                      <strong>
                        <xsl:value-of select ="util:FormatNumber(d/totalBilled)"/>
                      </strong>
                    </td>
                  </tr>
                  <tr>
                    <td  class="noborder text-right"  style="padding:0px;">
                      <strong>Total Paid Amount :</strong>
                    </td>
                    <td  class="noborder  text-right"  style="padding:0px;">
                      <strong>
                        <xsl:value-of select ="util:FormatNumber(d/totalPaidAmount)"/>
                      </strong>
                    </td>
                  </tr>
                  <tr >
                    <td colspan="2" style="line-height:25px !important;">
                      <br/>
                    </td>
                  </tr>
                  <tr>
                    <td  class="noborder text-right" style="color:red;padding-top:20px;!important;">
                      <strong>Total Due Amount :</strong>
                    </td>
                    <td  class="noborder text-right" style="color:red;padding-top:20px;!important;" >
                      <strong>
                        <xsl:value-of select ="util:FormatNumber(d/balance)"/>
                      </strong>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>

          </table>


        </div>

      </body>
    </html>



  </xsl:template>
</xsl:transform>
