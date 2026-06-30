<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          body {
          font-family:arial;
          margin:0px;
          padding:0px;
          }
          .container {
          width: 100%;
          min-height: 600px;
          border: solid 1px;
          margin: auto;
          }
          .header {font-weight:bold;}
          .text-right {text-align:right;}
          .text-left {text-align:left;}
          .text-center {text-align:center;}
          .noborder {border:0px !important;}
          <!--h4{font-weight:bold;font-size:14px;}-->

          table {

          }

          tr {
          line-height: 25px;
          }

          .divreport  td {

          font-size: 9pt;
          border:solid 1px;
          }

          td{

          <!--font-size:12pt;-->
          padding-left:2px;
          padding-right:2px;
          padding-top:0px;
          padding-bottom:0px;


          }

          strong {
          font-size: 22px;
          }

          #items th {
          padding-left: 5px;
          padding-top: 5px;
          padding-bottom: 5px;
          text-align: left;
          background-color: #e6e4e4;
          color: black;
          font-size: 13px;
          font-weight: bold;
          }

          #items td, #items th {
          border: 1px solid #252222;

          border-left: none;
          }
          .padding {
          padding: 2px;
          }
          .headerRow td { background-color: #e6e4e4;
          color: black;
          font-weight: bold;}
          ul{
          list-style:none;
          padding-left:0px
          }
          ul li {

          padding-left:none;
          margin-left:none;
          padding-top:1px;
          font-size:11pt;
          }


          @media print {
          body {
          font-family:arial;
          margin:0px;
          padding:0px;
          }
          .container {
          width: 100%;
          min-height: 600px;
          border: solid 1px;
          margin: auto;
          }
          .header {font-weight:bold;}
          .text-right {text-align:right;}
          .text-left {text-align:left;}
          .text-center {text-align:center;}
          .noborder {border:0px !important;}

          h4{font-weight:bold;font-size:14pt;}

          table {

          }

          tr {
          line-height: 22px;
          }

          .divreport  td {

          font-size: 9pt;
          border:solid 1px;
          }

          td{


          padding-left:2px;
          padding-right:2px;
          padding-top:0px;
          padding-bottom:0px;


          }

          strong {
          font-size: 22px;
          }

          #items th {
          padding-left: 5px;
          padding-top: 5px;
          padding-bottom: 5px;
          text-align: left;
          background-color: #e6e4e4;
          color: black;
          font-size: 13px;
          font-weight: bold;
          }

          #items td, #items th {
          border: 1px solid #252222;

          border-left: none;
          }
          .padding {
          padding: 2px;
          }
          .headerRow td { background-color: #e6e4e4;
          color: black;
          font-weight: bold;}
          ul{
          list-style:none;
          padding-left:0px
          }
          ul li {

          padding-left:none;
          margin-left:none;
          padding-top:1px;
          font-size:11pt;
          }

          .noborder {
          border:none;
          }
          }

        </style>

      </head>

      <body>

        <div class="divreport">
          <img style="height:60px;max-width:100px;margin-bottom:0.2in;">
            <xsl:attribute name="src">
              <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
            </xsl:attribute>
          </img>

          <div id="container" class="container" style="padding-top:10px;border:none;">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <tr  class="padding" style="height:25px;padding:3px;">
                <td style="width: 30%; border-bottom:0px;border-right:0px;font-size:12pt;"   class="padding subheading">
                  GST No: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                </td>
                <td class="subheading" style="width: 40%;border-right:0px;border-left:none; border-bottom:0px; text-align: center;font-size:12pt;">
                  Material Inward Slip (Rental)
                </td>
                <td colspan="2" style="width: 30%;border-left:0px;border-right:solid 1px;  text-align:right;border-bottom:0px;font-size:12pt;"
                    class="padding subheading">
                  <!--<xsl:choose>
						<xsl:when test="count(NewDataSet/Config/root[Key = 'diveryChallanText'])> 0">
							<xsl:value-of select="NewDataSet/Config/root[Key='diveryChallanText']/Value" />
						</xsl:when>
						<xsl:otherwise>
							RETURNABLE
						</xsl:otherwise>
					</xsl:choose>-->
                  On Hire Only
                </td>
              </tr>
              <tr class="padding">
                <td style="text-align: center;border-bottom:0px;" colspan="4" class="padding">
                  <ul style="margin-bottom:0px;">
                    <li>
                      <strong style="margin-bottom:10px;">
                        <xsl:value-of select="NewDataSet/Header/Company"/>
                      </strong>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    </li>
                    <li>
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      ,
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </li>
                  </ul>


                </td>

              </tr>
              <tr>
                <td colspan="4" style="border-top: 0px;padding:0px; border-left: 0px; border-right: 0px;">
                  <table style="width: 100%"  cellspacing="0">

                    <tr >
                      <td class="padding" style="width: 50%; height: 100px; border-bottom: none; 
                           vertical-align: top;border-right:none;"
                                    colspan="2">

                        <ul>
                          <li>
                            <b >Shipped From</b>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/Client"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteProject"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteZipCode"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Table/SiteState"/>
                          </li>
                         
                        </ul>

                      </td>
                      <td class="padding" style="width: 50%; height: 100px; border-bottom: none;   vertical-align: top;"
                                      colspan="2">
                        <ul>
                          <li>
                            <b>Shipped To</b>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                          </li>
                          <li>
                            <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                            <xsl:text> </xsl:text>
                            <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                          </li>
                      
                          
                        </ul>

                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style=" height:25px; border-bottom: 0px; border-right: 0px; ">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillState"/>
                      </td>
                      <td class="padding" style="border-bottom: 0px; border-right: 0px;">
                        <b>State Code</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>
                      </td>
                      <td class="padding" style="border-bottom: 0px; border-right: 0px; ">
                        <b>State</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteState"/>
                      </td>
                      <td class="padding" style="border-bottom: 0px; ">
                        <b>State Code</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>

                      </td>
                    </tr>
                    <tr >
                      <td class="padding" style="width: 50%;height:25px;  border-bottom: 0px; border-right: 0px;" colspan="2">
                        <b>Challan Number:</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/GRN"/>

                      </td>
                      <td class="padding" style="width: 50%; border-bottom: 0px;" colspan="2">
                        <b>Challan Date:</b>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/RentStartDate"/>

                      </td>

                    </tr>
                  </table>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="padding:0px;padding-left:1px;border-top:none;" valign="top">

                  <table id="items" style="width: 100%;" cellspacing="0" >
                    <tr>
                      <th style="border-top: 0px;width:60px;">
                        S.No
                      </th>
                      <th style="border-top: 0px;border-left:0px;width:310px">Item</th>
                      <th style="border-top: 0px;border-left:0px;width:150px;text-align:center">HSN/SAC Code</th>

                      <th style="border-top: 0px;border-left:0px;width:100px;text-align:center;border-right:none;">Qty</th>
                      <!--<th style="border-top: 0px;border-left:0px;width:60px;text-align:center">Rate</th>
                      <th style="border-top: 0px;border-left:0px;border-right:0px;width:100px;text-align:right">Amount</th>-->
                    </tr>
                    <xsl:for-each select="NewDataSet/Table">
                      <tr class="detailsRow">
                        <td style="border-top: 0px;border-left:0px;text-align:center;">
                          <xsl:value-of select="position()" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;">
                          <xsl:value-of select="Item" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center">
                          <xsl:value-of select="HSNCode" />
                        </td>
                        <td style="border-top: 0px;border-left:0px;text-align:center;border-right:none;">
                          <xsl:value-of select="Quantity" />
                        </td>
                        <!--<td style="border-left:0px;border-top: 0px;text-align:center">
                          <xsl:value-of select="Rate" />
                        </td>
                        <td style="border-left:0px;border-right: 0px;border-top: 0px;text-align:right">
                          <xsl:value-of select="SubTotal" />
                        </td>-->
                      </tr>
                    </xsl:for-each>
                    <tr  class="detailsRow">

                      <td colspan="3" style="border-top: 0px; border-left:0px;text-align:right">
                        <b>Total</b>
                      </td>
                      <td style="border-top: 0px;border-left:0px;text-align:center;border-right:none;">
                        <b>
                          <xsl:value-of select="sum(NewDataSet/Table/Quantity)"/>
                        </b>
                      </td>
                    </tr>
                    <tr  class="detailsRow">
                      <td colspan="4"  style="border-top: 0px;border-left:0px;border-right:none; ">
                        Quantity In Words: <xsl:value-of select="util:QtyToWords(sum(NewDataSet/Table/Quantity))"/>
                      </td>
                    </tr>
                  </table>

                </td>
                <td style="border-left:0px;border-top:0px;">
                  <div  >
                    <div style="text-align: center;height:50px;padding-top:10px;">
                      <b>Vehicle No</b>
                      <br/>
                      <xsl:value-of select="vehicleNo"/>
                      <br/>
                  
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>Driver</b>
                      <br/>
                      <xsl:value-of select="driver"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>LR Number</b>
                      <br/>
                      <xsl:value-of select="LRNumber"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>CR Number</b>
                      <br/>
                      <xsl:value-of select="CRNumber"/>
                    </div>
                    <!--<div style="text-align: center;height:100px;">
                      <b>Time</b>
                      <br/>

                    </div>-->
                    <div style="text-align: center;height:50px;">
                      <b>Weight (Approx)</b>
                      <br/>
                      <xsl:value-of select="Weight"/>
                    </div>
                    <div style="text-align: center;height:50px;">
                      <b>Goods Value (Approx)</b>
                      <br/>

                      <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                    </div>
                    <!--<div style="text-align: center;">
                      <b>Cartage</b>
                      <br/>
                      <xsl:value-of select="NewDataSet/Header/Freight"/>
                    </div>-->
                  </div>
                </td>
              </tr>
              <tr>

              </tr>
              <tr  >

                <td colspan="4" style="border-top:none;">
                  <table style="width:100%;">
                    <tr>
                      <td colspan="2" class="text-right noborder">
                        <h4>
                          For  <xsl:value-of select="NewDataSet/Header/Company"/>
                        </h4>
                      </td>
                    </tr>
                    <tr>
                      <td style="width:55%;border:none;">
                        Terms and Conditions
                        <div style="font-size:8pt;">
                          <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                        </div>
                        <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                          <span style="font-size:14pt; ">CUSTOMER SIGNATURE</span>
                        </div>
                      </td>
                      <td style="border:none;">
                        <table style="width:100%">

                          <tr>
                            <td class ="noborder">
                              <img style="height:60px;max-width:100px;float:right;">
                                <xsl:attribute name="src">
                                  <xsl:value-of select="NewDataSet/Header/Signature"/>
                                </xsl:attribute>
                              </img>
                            </td>
                          </tr>
                          <tr>
                            <td class="text-right noborder">
                              <h4>AUTHORIZED SIGNAORY</h4>
                            </td>
                          </tr>
                        </table>


                      </td>
                    </tr>
                  </table>
                  <!--<div style="padding:10px; width:70%;">

                    <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                      Terms and Conditions
                    </p>
                    <div style="font-size:8pt;">
                      <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                    </div>
                    <div style="float: left; padding: 75px 0 0px 5px; vertical-align: bottom;">
                      <span style="font-size:14px; ">CUSTOMER SIGNATURE</span>
                    </div>
                  </div>-->

                  <!--<div style="width: 30%;float: right; padding: 5px 5px 0px 0px; text-align:right;">
                    <p style="width: 100%; padding-top:15px; float: left;font-size:12pt; ">
                      For  <xsl:value-of select="NewDataSet/Header/Company"/>
                      <br/>
                      <img style="height:60px;max-width:100px;float:right;">
                        <xsl:attribute name="src">
                          <xsl:value-of select="NewDataSet/Header/Signature"/>
                        </xsl:attribute>
                      </img>
                      <br/>
                      AUTHORIZED SIGNAORY
                    </p>
                  </div>-->
                </td>
              </tr>
            </table>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
