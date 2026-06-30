<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

        <style type="text/css">

          #printArea {
          width: 100%;
          margin: 20px;
          font-family: Arial;
          }

          #container {
          width: 90%;
          min-height: 600px;
          border: solid 1px;
          margin: auto;
          }

          table {
          }

          tr {
          line-height: 25px;
          }

          td {
          border: solid 1px #000;
          font-size: 13px;
          }

          strong {
          font-size: 22px;
          }

          #items th {
          padding-top: 5px;
          padding-bottom: 5px;
          text-align: left;
          background-color: #e6e4e4;
          color: black;
          font-weight: bold;
          }

          #items td, #items th {
          border: 1px solid #252222;
          padding: 5px;
          border-left: none;
          }
          .padding td {
          padding: 3px;
          }
          .headerRow td { background-color: #e6e4e4;
          color: black;
          font-weight: bold;}
        </style>
      </head>

      <body>

        <div id="printArea">
          <div id="container">
            <table style="width: 100%" cellpadding="0" cellspacing="0">
              <tr>
                <td colspan="3">
                  <img style="height:60px;max-width:100px;margin-bottom:0.2in;">
                    <xsl:attribute name="src">
                      <xsl:value-of select="Data/CompanyLogo" />
                    </xsl:attribute>
                  </img>
                </td>
              </tr>
              <tr class="padding">
                <!-- <td style="width: 20%; border: none;">GISTIN No:</td>-->
                <td colspan="3" style="width: 60%; border-top: none; border-bottom: none; text-align: center;font-weight:bold">Debit Note</td>
                <!--<td style="width: 20%; border: none;">Mobile:9811553130</td>-->
              </tr>

              <tr>
                <td colspan="3" style="border-top: none; border-left: none; border-right: none;">
                  <table style="width: 100%" cellpadding="0" cellspacing="0">
                    <tr class="padding">
                      <td style="width: 50%; height: 100px; border-bottom: none; border-left: none; border-right: none; vertical-align: top;"
                          colspan="2">
                         Debit Note: -  <xsl:value-of select="Data/ReceiptNumber"/>
                        <br />
                        Invoice Number: <xsl:value-of select="Data/InvoiceNumber"/>
                        <p>
                          <xsl:value-of select="Data/PartyName"/>
                          <br/>
                          <xsl:value-of select="Data/ClientBIllAddress"/>
                        </p>
                      </td>
                      <td style="width: 50%; height: 100px; border-bottom: none; border-right: none; vertical-align: top;"
                          colspan="2">
                        <strong>
                          <xsl:value-of select="Data/Company"/>
                        </strong>

                        <p>
                          <xsl:value-of select="Data/CompanyAddress"/>
                          <br />

                        </p>

                      </td>

                    </tr>

                    <tr class="padding">
                      <td style="width: 50%; border-left: none; border-bottom: none; border-right: none;" colspan="2">
                        <xsl:value-of select="Data/Narration"/>
                      </td>
                      <td style="width: 50%; border-bottom: none; border-right: none;" colspan="2">
                        Amount : <xsl:value-of select="Data/Amount"/>
                      </td>

                    </tr>
                  </table>
                </td>
              </tr>

              <tr>
                <td colspan="3" style="border-bottom: none; border-left: none; border-right: none;">
                  <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                    <span style="font-size: 16px; font-weight: bold;">CUSTOMER SIGNATURE</span>
                  </div>
                  <div style="width: 50%; float: right; padding: 0px 5px 0px 0px; text-align: right;">
                    <span style="width: 100%; float: left; padding-bottom: 50px; font-size: 16px; font-weight: bold;">
                      For -  <xsl:value-of select="Data/Company"/>
                    </span>
                    <span style="font-size: 16px; font-weight: bold;">AUTHORIZED SIGNAORY</span>
                  </div>
                </td>
              </tr>
            </table>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
