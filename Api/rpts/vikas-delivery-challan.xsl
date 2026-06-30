<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>Delivery Challan</title>
        <style type="text/css">
          body { font-family: Arial, sans-serif; font-size: 11px; margin: 15px; color: #000; }
          #printArea { width: 100%; max-width: 210mm; margin: 0 auto; }
          table { border-collapse: collapse; width: 100%; }
          td, th { border: 1px solid #000; padding: 4px 6px; vertical-align: top; }
          .border-none td, .border-none th { border: none; }
          .text-center { text-align: center; }
          .text-right { text-align: right; }
          .underline { border-bottom: 1px dotted #000; min-width: 80px; display: inline-block; }
          .header-row { font-weight: bold; background-color: #f0f0f0; }
          .section-title { font-weight: bold; margin-bottom: 4px; }
          .terms { font-size: 9px; line-height: 1.4; }
        </style>
      </head>
      <body>
        <div id="printArea">
          <!-- Header -->
          <table class="border-none" cellpadding="0" cellspacing="0">
            <tr>
              <td style="width: 30%;">GSTIN : <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/></td>
              <td style="width: 40%; text-align: center; font-size: 16px; font-weight: bold;">Delivery Challan</td>
              <td style="width: 30%;"></td>
            </tr>
            <tr>
              <td colspan="3" style="text-align: center; font-size: 10px; padding-top: 2px;">
                Deals In : Steel Plates, Channel, Props, Scaffolding &amp; All Shuttering Materials ON HIRE BASIS
              </td>
            </tr>
          </table>

          <!-- Main content table -->
          <table style="margin-top: 8px;">
            <tr>
              <td style="width: 55%;">
                <!-- Challan info -->
                <table class="border-none" style="width: 100%;">
                  <tr>
                    <td style="width: 50%;"><b>CHALLAN No.</b> <xsl:value-of select="NewDataSet/Header/ChallanNumber"/></td>
                    <td><b>Dated</b> <xsl:value-of select="util:DateToDDMMYYYY(NewDataSet/Header/StartDate)"/></td>
                  </tr>
                  <tr>
                    <td>SAC CODE : 997313</td>
                    <td>HSN CODE : 73084000</td>
                  </tr>
                </table>

                <!-- Bill to -->
                <table class="border-none" style="width: 100%; margin-top: 8px;">
                  <tr><td class="section-title">Bill to</td></tr>
                  <tr><td>Names <xsl:value-of select="NewDataSet/Header/Client"/></td></tr>
                  <tr><td><xsl:value-of select="NewDataSet/Header/BillAddress1"/><br/><xsl:value-of select="NewDataSet/Header/BillAddress2"/></td></tr>
                  <tr><td><xsl:value-of select="NewDataSet/Header/BillCity"/>, <xsl:value-of select="NewDataSet/Header/BillState"/> <xsl:value-of select="NewDataSet/Header/BillZipCode"/></td></tr>
                  <tr><td>State <xsl:value-of select="NewDataSet/Header/BillState"/> State Code <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/></td></tr>
                  <tr><td>GSTIN / Unique ID <xsl:value-of select="NewDataSet/Header/ClientGSTNo"/></td></tr>
                </table>

                <!-- Place of Supply -->
                <table class="border-none" style="width: 100%; margin-top: 8px;">
                  <tr><td class="section-title">Place of Supply</td></tr>
                  <tr><td>Names <xsl:value-of select="NewDataSet/Header/Client"/></td></tr>
                  <tr><td><xsl:value-of select="NewDataSet/Table/SiteAddress"/><xsl:value-of select="NewDataSet/Table/Site"/></td></tr>
                  <tr><td><xsl:value-of select="NewDataSet/Table/SiteCity"/>, <xsl:value-of select="NewDataSet/Table/SiteState"/></td></tr>
                  <tr><td>State <xsl:value-of select="NewDataSet/Header/SiteState"/> State Code <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/></td></tr>
                  <tr><td>GSTIN / Unique ID <xsl:value-of select="NewDataSet/Header/ClientGSTNo"/></td></tr>
                </table>

                <!-- Terms & Conditions -->
                <div class="terms" style="margin-top: 8px;">
                  <div class="section-title">Terms &amp; Conditions :-</div>
                  <div>1. Subject to Gurugram Jurisdiction only.</div>
                  <div>2. Payment has to be made Immediately on presentation of the bills.</div>
                  <div>3. On none payment of two bills the firm have right to take the goods back at customer cost &amp; risk</div>
                  <div>4. In case of loss/damage full value of material will be charged as per actual along with hire charges.</div>
                  <div>5. The goods all the times remains the sole property of firm.</div>
                  <div>6. Responsibility of goods once removed from our Godown rest with customer.</div>
                  <div>7. Customer will be responsible for transportation of goods both ways.</div>
                  <div>8. Work Timings 8.00 am to 6.00 pm SUNDAY CLOSED</div>
                </div>
              </td>
              <td style="width: 45%; vertical-align: top; padding-left: 10px;">
                <!-- Items table -->
                <table>
                  <tr class="header-row">
                    <th style="width: 30px;">S.No</th>
                    <th>DESCRIPTION</th>
                    <th style="width: 60px;">SIZE</th>
                    <th style="width: 50px;">QTY.</th>
                    <th style="width: 50px;">Units</th>
                    <th style="width: 80px;">REMARKS</th>
                  </tr>
                  <xsl:for-each select="NewDataSet/Table">
                    <tr>
                      <td><xsl:value-of select="position()"/></td>
                      <td><xsl:value-of select="Product"/></td>
                      <td><xsl:value-of select="Size"/></td>
                      <td><xsl:value-of select="SentQty"/></td>
                      <td><xsl:value-of select="Unit"/></td>
                      <td><xsl:value-of select="Remarks"/></td>
                    </tr>
                  </xsl:for-each>
                </table>

                <!-- Vehicle info -->
                <table class="border-none" style="width: 100%; margin-top: 10px;">
                  <tr><td><b>Vehicle No.</b></td><td><xsl:value-of select="NewDataSet/Header/VehicleRegNo | NewDataSet/Table/Vehicle"/></td></tr>
                  <tr><td><b>Driver Names</b></td><td><xsl:value-of select="NewDataSet/Table/Driver"/></td></tr>
                  <tr><td><b>Mobile No.</b></td><td><xsl:value-of select="NewDataSet/Header/CompanyPhone1"/></td></tr>
                  <tr><td><b>Cartage</b></td><td><xsl:value-of select="NewDataSet/Header/Freight"/></td></tr>
                  <tr><td><b>Time</b></td><td></td></tr>
                </table>

                <!-- Totals -->
                <table class="border-none" style="margin-top: 8px;">
                  <tr>
                    <td style="font-weight: bold;">TOTAL</td>
                    <td><xsl:value-of select="format-number(sum(NewDataSet/Table/SubTotal), '#,##0.00')"/></td>
                  </tr>
                  <tr>
                    <td>Total Qty In Words:</td>
                    <td></td>
                  </tr>
                  <tr>
                    <td>Estimate Value</td>
                    <td><xsl:value-of select="format-number(sum(NewDataSet/Table/SubTotal), '#,##0.00')"/></td>
                  </tr>
                  <tr>
                    <td>Estimate Weight</td>
                    <td></td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>

          <!-- Footer badges -->
          <table class="border-none" style="width: 100%; margin-top: 8px;">
            <tr>
              <td class="text-center" style="padding: 4px; border: 1px solid #000;">ON HIRE BASIS</td>
              <td class="text-center" style="padding: 4px; border: 1px solid #000;">ON RETURNABLE BASIS</td>
              <td class="text-center" style="padding: 4px; border: 1px solid #000;">NOT FOR SALE</td>
            </tr>
          </table>

          <!-- Footer addresses & signatures -->
          <table class="border-none" style="width: 100%; margin-top: 15px;">
            <tr>
              <td>
                <b>B.O :</b> <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/> &nbsp;&nbsp;
                <b>H.O :</b> <xsl:value-of select="NewDataSet/Header/CompanyPhone2"/>
              </td>
            </tr>
            <tr>
              <td style="padding-top: 8px; font-size: 9px;">
                <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                <br/><xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyZipCode"/>
                <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>
                <br/>E-mail: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
              </td>
            </tr>
            <tr>
              <td style="padding-top: 20px;">
                <table class="border-none" style="width: 100%;">
                  <tr>
                    <td style="width: 50%;">For<br/><b>Authorised Signatory</b></td>
                    <td style="width: 50%; text-align: right;">Customer's Signature</td>
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
