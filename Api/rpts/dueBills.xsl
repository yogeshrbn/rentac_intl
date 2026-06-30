<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:key name="client-group" match="NewDataSet/Table" use="Client" />

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style type="text/css">
          #printArea { width: 100%; margin: 20px; font-family: Arial; }
          #container { width: 90%; min-height: 600px; border: solid 1px; margin: auto; }
          table { border-collapse: collapse; width: 100%; }
          tr { line-height: 25px; }
          td, th { border: solid 1px #000; font-size: 13px; padding: 5px; }
          strong { font-size: 16px; }
          #items th { padding-top: 5px; padding-bottom: 5px; text-align: left; background-color: #e6e4e4; color: black; font-weight: bold; }
          #items td, #items th { border: 1px solid #252222; padding: 5px; }
          .party-header { background-color: #BBDEFB; font-weight: bold; }
          .headerRow td { background-color: #e6e4e4; color: black; font-weight: bold; }
        </style>
      </head>
      <body>
        <div id="printArea">
          <div style="padding-left:50px;border-bottom:solid 2px;margin-bottom:10px;">
            <div style="width:100%;text-align:center;">
              <strong>
                <xsl:value-of select="NewDataSet/Header/Data/Company" />
              </strong>
              <p>
                <xsl:value-of select="NewDataSet/Header/Data/CompanyAddress" />
                <br />
                <b>Email:</b>
                <xsl:value-of select="NewDataSet/Header/Data/CompanyEmail" />
              </p>
            </div>
          </div>
          <div id="container">
            <!--<table style="width: 100%;border:0px;" border="0" cellpadding="0" cellspacing="0">
              <tr>
                <td colspan="4" style="border: none;">-->
                  <table id="items" style="width: 100%" cellpadding="0" cellspacing="0">
                    <tr class="headerRow">
                      <th style="width: 40px;">S.No</th>
                      <th style="width: 25%;">Party</th>
                      <th style="width: 35%;">Site</th>
                      <th style="width: 100px;">Last Invoice #</th>
                      <th style="width: 100px;">Last Billed On</th>
                    </tr>
                    <xsl:for-each select="NewDataSet/Table[generate-id() = generate-id(key('client-group', Client)[1])]">
                      <xsl:sort select="Client" />
                      <xsl:variable name="current-client" select="Client" />
                      <xsl:variable name="client-rows" select="key('client-group', $current-client)" />
                      <tr class="party-header">
                        <td colspan="5">
                          <strong>
                            Party: <xsl:value-of select="$current-client" />
                          </strong>
                        </td>
                      </tr>
                      <xsl:for-each select="$client-rows">
                        <tr>
                          <td>
                            <xsl:value-of select="position()" />
                          </td>
                          <td></td>
                          <td>
                            <xsl:value-of select="SiteAddress" />
                          </td>
                          <td>
                            <xsl:value-of select="LastInvoiceNumber" />
                          </td>
                          <td>
                            <xsl:value-of select="LastBillDate" />
                          </td>
                        </tr>
                      </xsl:for-each>
                    </xsl:for-each>
                  </table>
                <!--</td>
              </tr>
            </table>-->
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
