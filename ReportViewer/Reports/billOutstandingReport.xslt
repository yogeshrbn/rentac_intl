<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0"
               xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
               xmlns:util="urn:util-format"
               xmlns="http://www.w3.org/1999/xhtml">

  <!-- Define a key for grouping by Client -->
  <xsl:key name="client-group" match="d/reportData" use="Client" />

  <xsl:template match="/">
    <html>
      <head>
        #preview
        #pdf
        <title>Bill Outstandings Report</title>
        <style>
          .td-heading {
          line-height: 20px;
          padding: 0px;
          }
          .headerUL li {
          padding-bottom: 8px;
          padding-top: 0px;
          margin: 0px;
          }
          td, th {
          border: none;
          padding: 2px;
          }
          .footerrow td {
          border-top: solid 2px;
          }
          .text-right {
          text-align: right;
          }
          .text-center {
          text-align: center;
          }
          .noborder {
          border: none !important;
          }
          .client-group {
          background-color: #f8f9fa;
          font-weight: bold;
          }
          .client-total {
          background-color: #e9ecef;
          font-weight: bold;
          }

          /* Print styles for repeating headers and keeping clients together */
          @media print {
          table {
          page-break-inside: auto;
          }
          tr {
          page-break-inside: avoid;
          page-break-after: auto;
          }
          thead {
          display: table-header-group;
          }
          tfoot {
          display: table-footer-group;
          }
          .client-block {
          page-break-inside: avoid;
          break-inside: avoid;
          display: block;
          }
          .client-group, .client-total {
          page-break-inside: avoid;
          break-inside: avoid;
          }
          .page-break {
          page-break-before: always;
          }
          .no-print {
          display: none;
          }
          .print-area {
          margin: 0;
          padding: 0;
          }
          /* Ensure client blocks don't break across pages */
          .client-container {
          page-break-inside: avoid;
          break-inside: avoid-page;
          }
          }

          /* Client container to keep all client data together */
          .client-container {
          margin-bottom: 15px;
          }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:10px;">
          <table style="width:100%;border-collapse:collapse;">
            <!-- Company Header -->
            <tr>
              <td class="text-center noborder" colspan="8">
                <ul class="headerUL" style="list-style:none;padding:0;">
                  <li>
                    <h2 style="color:green;padding:0;margin:0;">
                      <xsl:value-of select="d/company/Name"/>
                    </h2>
                  </li>
                  <li>
                    <xsl:value-of select="d/company/Address1"/>
                    <xsl:if test="d/company/Address2 != ''">
                      <xsl:text>, </xsl:text>
                      <xsl:value-of select="d/company/Address2"/>
                    </xsl:if>
                    <xsl:if test="d/company/City != ''">
                      <xsl:text>, </xsl:text>
                      <xsl:value-of select="d/company/City"/>
                    </xsl:if>
                    <xsl:if test="d/company/State != ''">
                      <xsl:text>, </xsl:text>
                      <xsl:value-of select="d/company/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone No.: </b>
                    <xsl:value-of select="d/company/Phone1"/>
                  </li>
                </ul>
              </td>
            </tr>

            <!-- Client Information -->
            <tr>
              <td colspan="7" class="noborder td-heading">
                
              </td>
              <td class="text-right">
                Date: <xsl:value-of select="util:DateToDDMMYYYY(util:CurrentDate())"/>
              </td>
            </tr>
           
            <!-- Report Title -->
            <tr>
              <td colspan="8" class="noborder text-center">
                <strong>Bill Outstandings</strong>
                <hr style="border-color:#000" />
              </td>
            </tr>

            <!-- Outstanding Bills Table -->
            <tr>
              <td colspan="8" class="noborder" style="vertical-align:top;">
                <table style="width:100%;border-collapse:collapse;">
                  <!-- Table Header that will repeat on each page -->
                  <thead style="display:table-header-group;">
                    <tr>
                      <th style="border-bottom:2px solid #000;width:400px">Party</th>
                      <th class="text-right" style="border-bottom:2px solid #000;width:150px">Total Bill Amount</th>
                      <th class="text-right" style="border-bottom:2px solid #000;width:150px">Total Paid Amount</th>
                      <th class="text-right" style="border-bottom:2px solid #000;width:150px">Due Amount</th>
                    </tr>
                  </thead>

                  <tbody>
                    <!-- Group report data by Client using Muenchian Method -->
                    <xsl:for-each select="d/reportData[generate-id() = generate-id(key('client-group', Client)[1])]">
                      <xsl:sort select="Client"/>

                      <xsl:variable name="current-client" select="Client" />
                      <xsl:variable name="client-group" select="key('client-group', $current-client)" />

                      <!-- Wrap each client in a container to keep together -->
                      <xsl:variable name="client-row-count" select="count($client-group) + 3" />

                      <!-- Client Container - keeps all client data together -->
                      <tr class="client-container">
                        <td colspan="4" style="padding:0;border:none;">
                          <table style="width:100%;border-collapse:collapse;margin-bottom:10px;">
                            <!-- Client Group Header -->
                            <tr class="client-group">
                              <td colspan="4">
                                <strong>
                                <xsl:value-of select="$current-client"/>
                                </strong>
                              </td>
                            </tr>

                            <!-- Individual bills for this client -->
                            <xsl:for-each select="$client-group">
                              <tr>
                                <td style="width:400px">
                                  <xsl:value-of select="BillReference"/>
                                  <xsl:if test="not(BillReference) or BillReference = ''">
                              
                                    <xsl:value-of select="Site"/>
                                  </xsl:if>
                                </td>
                                <td class="text-right" style="width:150px">
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="TotalBillAmount"/>
                                  </xsl:call-template>
                                </td>
                                <td class="text-right" style="width:150px">
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="TotalPaidAmount"/>
                                  </xsl:call-template>
                                </td>
                                <td class="text-right" style="width:150px">
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="DueAmount"/>
                                  </xsl:call-template>
                                </td>
                              </tr>
                            </xsl:for-each>

                            <!-- Client Subtotal -->
                            <tr class="client-total">
                              <td class="text-right">
                                <strong>
                                  Subtotal for <xsl:value-of select="$current-client"/>:
                                </strong>
                              </td>
                              <td class="text-right">
                                <strong>
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="sum($client-group/TotalBillAmount)"/>
                                  </xsl:call-template>
                                </strong>
                              </td>
                              <td class="text-right">
                                <strong>
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="sum($client-group/TotalPaidAmount)"/>
                                  </xsl:call-template>
                                </strong>
                              </td>
                              <td class="text-right">
                                <strong>
                                  <xsl:call-template name="format-amount">
                                    <xsl:with-param name="amount" select="sum($client-group/DueAmount)"/>
                                  </xsl:call-template>
                                </strong>
                              </td>
                            </tr>

                            <!-- Spacer between client groups -->
                            <tr>
                              <td colspan="4" style="height: 15px; border: none;"></td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </xsl:for-each>

                    <!-- Grand Totals -->
                    <tr class="footerrow">
                      <td>
                        <strong>GRAND TOTAL:</strong>
                      </td>
                      <td class="text-right">
                        <strong>
                          <xsl:call-template name="format-amount">
                            <xsl:with-param name="amount" select="sum(d/reportData/TotalBillAmount)"/>
                          </xsl:call-template>
                        </strong>
                      </td>
                      <td class="text-right">
                        <strong>
                          <xsl:call-template name="format-amount">
                            <xsl:with-param name="amount" select="sum(d/reportData/TotalPaidAmount)"/>
                          </xsl:call-template>
                        </strong>
                      </td>
                      <td class="text-right">
                        <strong>
                          <xsl:call-template name="format-amount">
                            <xsl:with-param name="amount" select="sum(d/reportData/DueAmount)"/>
                          </xsl:call-template>
                        </strong>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </td>
            </tr>

            <tr>
              <td colspan="8" style="line-height:40px;" class="noborder"></td>
            </tr>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>

  <!-- Template to format currency amounts -->
  <xsl:template name="format-amount">
    <xsl:param name="amount" select="0"/>
    <xsl:value-of select="format-number($amount, '#,##0.00')"/>
  </xsl:template>

</xsl:transform>