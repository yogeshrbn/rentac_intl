<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- Party stock register (challan-wise). Header layout inspired by cleintWiseItemBalance.xslt (not shared file). -->
  <xsl:key name="by-item" match="data/details/Table" use="Item" />

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          .headerUL li { padding-bottom:8px; padding-top:0px; margin:0px; }
          td, th { border:none; padding:4px; }
          .inner th, .inner td { border:1px solid #ccc; }
          .inner th { background:#e8e8e8; font-weight:bold; }
          .groupBar { background:#E3F2FD; font-weight:bold; }
          .totalBar { background:#FFF3E0; font-weight:bold; }
        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:10px;">
          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center noborder" colspan="7">
                <ul class="headerUL" style="list-style:none;padding:0;margin:0;">
                  <li>
                    <h2 style="color:green;padding:0;margin:0;">
                      <xsl:value-of select="data/comp/Name"/>
                    </h2>
                  </li>
                  <li>
                    <xsl:value-of select="data/comp/Address1"/>
                    <xsl:if test="data/comp/Address2 != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/Address2"/>
                    </xsl:if>
                    <xsl:if test="data/comp/City != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/City"/>
                    </xsl:if>
                    <xsl:if test="data/comp/State != ''">
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/comp/State"/>
                    </xsl:if>
                  </li>
                  <li>
                    <b>Phone No.: </b>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="data/comp/Phone1"/>
                  </li>
                </ul>
              </td>
            </tr>
            <tr>
              <td colspan="7" class="noborder text-center" style="padding-top:8px;">
                <strong>Party stock register (challan wise)</strong>
                <xsl:text> — Period: </xsl:text>
                <xsl:value-of select="data/dateRangeText"/>
                <hr style="border-color:#000;margin:8px 0;" />
              </td>
            </tr>
            <tr>
              <td colspan="7" class="noborder" style="padding-bottom:12px;">
                <div style="background-color:#f3f3f3;font-weight:bold;padding:8px 5px;">
                  <strong>Party</strong>
                  <xsl:text>: </xsl:text>
                  <xsl:value-of select="data/partyName"/>
                  <xsl:text> | </xsl:text>
                  <strong>Site</strong>
                  <xsl:text>: </xsl:text>
                  <xsl:value-of select="data/siteAddress"/>
                </div>
              </td>
            </tr>

            <xsl:for-each select="data/details/Table[generate-id() = generate-id(key('by-item', Item)[1])]">
              <xsl:sort select="Item" />
              <xsl:variable name="g" select="key('by-item', Item)" />

              <tr>
                <td colspan="7" class="noborder" style="padding-top:10px;">
                  <table class="inner" style="width:100%;border-collapse:collapse;">
                    <tr class="groupBar">
                      <td colspan="7">
                        <xsl:text>Item: </xsl:text>
                        <xsl:value-of select="$g[1]/Item" />
                      </td>
                    </tr>
                    <tr>
                      <th class="text-right">Opening bal.</th>
                      <th>Date</th>
                      <th>Challan no.</th>
                      <th class="text-right">Receive</th>
                      <th class="text-right">Issue</th>
                      <th class="text-right">Balance</th>
                      <th class="text-right">Closing bal.</th>
                    </tr>
                    <xsl:for-each select="$g">
                      <tr>
                        <td class="text-right">
                          <xsl:choose>
                            <xsl:when test="OpeningBalance != ''">
                              <xsl:value-of select="OpeningBalance" />
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:text></xsl:text>
                            </xsl:otherwise>
                          </xsl:choose>
                        </td>
                        <td>
                          <xsl:value-of select="DisplayDate" />
                        </td>
                        <td>
                          <xsl:value-of select="ChallanNo" />
                        </td>
                        <td class="text-right">
                          <xsl:if test="Receive != 0">
                            <xsl:value-of select="Receive" />
                          </xsl:if>
                        </td>
                        <td class="text-right">
                          <xsl:if test="Issue != 0">
                            <xsl:value-of select="Issue" />
                          </xsl:if>
                        </td>
                        <td class="text-right">
                          <xsl:value-of select="Balance" />
                        </td>
                        <td class="text-right">
                          <xsl:if test="ClosingBalance != ''">
                            <xsl:value-of select="ClosingBalance" />
                          </xsl:if>
                        </td>
                      </tr>
                    </xsl:for-each>
                    <tr class="totalBar">
                      <td colspan="3" class="text-right">Total</td>
                      <td class="text-right">
                        <xsl:value-of select="sum($g/Receive)" />
                      </td>
                      <td class="text-right">
                        <xsl:value-of select="sum($g/Issue)" />
                      </td>
                      <td class="text-right">
                        <xsl:value-of select="$g[last()]/Balance" />
                      </td>
                      <td class="text-right">
                        <xsl:value-of select="$g[last()]/ClosingBalance" />
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </xsl:for-each>

            <tr>
              <td colspan="7" style="line-height:24px;" class="noborder"></td>
            </tr>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
