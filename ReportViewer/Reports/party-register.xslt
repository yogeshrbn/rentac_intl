<?xml version="1.0" encoding="utf-8"?>


<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        #preview
        #pdf
        <style>
          td {
          line-height:20px;
          padding:0px;
          }
          .headerUL li  {
          padding-bottom:3px;
          padding-top:0px;
          margin:0px;
          }

        </style>
        <meta name="viewport" content="width=device-width,initial-scale=1" />
      </head>
      <body>
        <div id="printArea" style="padding:2px;">

          <table style="width:100%;border-collapse:collapse;">
            <tr>
              <td class="text-center" style="border:none; border-bottom:solid 1px;" >
                <ul class="headerUL" style="list-style:none;padding:none;">
                  <li>Party Register For</li>
                  <li>
                    <h3 style="padding:0px;margin:0px;">
                      <xsl:value-of select="d/client/Name"/>
                    </h3>
                  </li>
                  <li>
                    <h3 style="padding:0px;;margin:0px;">
                      Under  <xsl:value-of select="d/company/Name"/>
                    </h3>
                  </li>
                  <li>
                    Party Site:  <xsl:value-of select="d/delivery/Site"/>
                  </li>
                  <li>
                    From:  <xsl:value-of select="d/from"/> To  <xsl:value-of select="d/to"/>
                  </li>
                </ul>
              </td>
            </tr>
          </table>
          <table style="width:100%;border-collapse:collapse;" >
            <tr>
              <td style="width:50%;vertical-align:top;border:0;">
                <table style="width:100%">
                  <tr>
                    <td class="noborder">
                      <strong>Quantity Previously Supplied:</strong>

                    </td>
                    <td  class="noborder">
                      <strong>
                        <xsl:value-of select="d/prevSent"/>
                      </strong>
                    </td>
                  </tr>
                  <tr>
                    <td class="noborder" colspan="2">
                      <strong>Supplied</strong>
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2" class="noborder">
                      <hr/>
                    </td>
                  </tr>
                </table>
                <xsl:for-each select="d/delivery">
                  <div style="border-bottom:dashed 1px #000;padding-bottom:10px;padding-top:10p;padding-right:10px;">
                    <table style="width:100%">

                      <tr>
                        <td class="noborder" style="width:50%;">

                          <strong>
                            <xsl:value-of select="Number"/>
                          </strong>
                        </td>
                        <td class="noborder">
                          <strong>Date:</strong>
                          <xsl:value-of select="util:DateToDDMMYYYY(WorkOrderDate)"/>
                        </td>

                      </tr>
                      <tr >
                        <td colspan="3" class="noborder">
                          <strong>Driver:</strong>
                          <xsl:value-of select="Driver"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Vechicle:</strong>
                          <xsl:value-of select="Vehicle"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Cartage Paid:</strong>
                          <xsl:value-of select="Freight"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Total Qty:</strong>
                          <xsl:value-of select="d/delivery/Site"/>
                        </td>
                      </tr>
                    </table>

                    <table class="table table-condensed;border-collapse:collapse;border"   cellspacing="0">
                      <tr class="headerRow">
                        <th  style="width:400px;border:0">Item</th>
                        <th class="text-center" style="border:0;width:10px;" >Qty</th>
                        <th class="text-left" style="border:0;width:10px;" >Remarks</th>
                      </tr>
                      <xsl:for-each select="Items">
                        <tr  >
                          <td  class="noborder" style="border-right:none;border-top:none;">
                            <xsl:value-of select="Product"/>
                          </td>
                          <td class="text-center noborder" style="border-top:none;">
                            <xsl:value-of select="SentQty"/>
                          </td>
                          <td class="noborder" style="border-top:none;">
                            <xsl:value-of select="Remarks"/>
                          </td>
                        </tr>
                      </xsl:for-each>
                    </table>

                   
                  </div>
                </xsl:for-each>
                <table style="width:100%" >
                  <tr>
                    <td  style="border:0;border-top:solid 1px;" class="text-right">
                      <strong>
                        Challan Grand Total: <xsl:value-of select="d/totalSentQty" />
                      </strong>
                    </td>
                  </tr>
                </table>
              </td>
              <td style="width:50%;vertical-align:top;border:0;"   cellspacing="0">
                <table style="width:100%">
                  <tr>
                    <td class="noborder" colspan="2">
                      <strong>
                        Quantity Previously Received:  <xsl:value-of select="d/prevReceived"/>
                      </strong>

                    </td>

                    <td class="noborder" colspan="2">
                      <strong>Shortage: 0</strong>
                    </td>
                  </tr>
                  <tr>
                    <td class="noborder" colspan="4">
                      <strong>Received</strong>
                    </td>
                  </tr>
                  <tr>
                    <td colspan="4" class="noborder">
                      <hr/>
                    </td>
                  </tr>
                </table>
                <xsl:for-each select="d/returns">
                  <div style="border-bottom:dashed 1px #000;padding-bottom:10px;padding-top:10p;padding-left:10px;">
                    <table style="width:100%">

                      <tr>
                        <td class="noborder" style="width:50%;">
                          <strong>
                            <xsl:value-of select="GRN"/>
                          </strong>

                        </td>
                        <td class="noborder">
                          <strong>Date:</strong>
                          <xsl:value-of select="util:DateToDDMMYYYY(ReceivingDate)"/>
                        </td>

                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Driver:</strong>
                          <xsl:value-of select="Driver"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Vechicle:</strong>
                          <xsl:value-of select="VehicleNo"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Cartage Paid:</strong>
                          <xsl:value-of select="Freight"/>
                        </td>
                      </tr>
                      <tr>
                        <td colspan="3" class="noborder">
                          <strong>Total Qty:</strong>
                          <xsl:value-of select="d/delivery/Site"/>
                        </td>
                      </tr>
                    </table>

                    <table class="table table-condensed" >
                      <tr class="headerRow">
                        <th style="width:60%" class="noborder">Item</th>
                        <th class="text-center noborder"  >Recd</th>
                        <th class="text-center noborder" >Scrap</th>
                        <th class="text-center noborder" >Short</th>

                        <th class="text-center noborder " >Excess</th>
                        <th class="noborder " >Remarks</th>
                      </tr>
                      <xsl:for-each select="Items">
                        <tr ng-repeat="p in item.Items ">
                          <td class="noborder">
                            <xsl:value-of select="Product"/>
                          </td>
                          <td class="text-center noborder">
                            <xsl:value-of select="Quantity"/>
                          </td>
                          <td class="text-center noborder">
                            <xsl:value-of select="ShortQty"/>
                          </td>
                          <td class="text-center noborder">
                            <xsl:value-of select="Scrap"/>
                          </td>
                          <td class="text-center noborder">
                            <xsl:value-of select="ExcessQty"/>
                          </td>
                          <td class="noborder">
                            <xsl:value-of select="Remarks"/>
                          </td>
                        </tr>
                      </xsl:for-each>
                    </table>

                   

                  </div>
                </xsl:for-each>
                <table style="width:100%" border="border-top:solid 1px;">
                  <tr>
                    <td class="noborder text-right">
                      <strong>  Received Grand Total:</strong>
                  </td>
                    <td class="noborder text-right">
                      <xsl:value-of select="d/totalReceivedQty" />
                    </td>
                  </tr>
                  <tr>
                    <td class="noborder text-right">
                      <strong>Short Grand Total:</strong> 
                    </td>
                    <td class="noborder text-right">
                      <xsl:value-of select="d/totalShortQty" />
                    </td>
                  </tr>
                  <tr>
                    <td class="noborder text-right">
                      <strong>Scrap Grand Total:</strong>
                    </td>
                    <td class="noborder text-right">
                      <xsl:value-of select="d/totalScrapQty" />
                    </td>
                  </tr>
                  <tr>
                    
                    <td  colspan="2" class="text-right" style="border:0;border-top:solid 1px;">
                      <strong> <xsl:value-of select="d/totalReturned" />
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
