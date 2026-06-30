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
          table {

          }

          tr {
          line-height: 17px;
          }

          td {

          font-size: 9pt;
          }

          td{

          font-size:9pt;
          padding-left:2px;
          padding-right:2px;
          padding-top:0px;
          padding-bottom:0px;

          border-top:solid 1px;
          border-left:solid 1px;
          border-bottom:0px;
          border-right:0px;
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
          padding-top:3px;
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
          table {

          }

          tr {
          line-height: 17px;
          }

          td {

          font-size: 9pt;
          }

          td{

          font-size:9pt;
          padding-left:2px;
          padding-right:2px;
          padding-top:0px;
          padding-bottom:0px;

          border-top:solid 1px;
          border-left:solid 1px;
          border-bottom:0px;
          border-right:0px;
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
          padding-top:3px;
          font-size:11pt;
          }

          }


        </style>

      </head>

      <body>

        <table style="width: 100%;" border="0"  cellspacing="0">
          <tr>
            <td colspan="6" style="padding:5px;">
              <span style="text-decoration:underline;font-size:13pt;">Original for Consignee</span>

            </td>
            <td colspan="6" style="padding:4px;text-align:right;border-right:solid 1px;border-left:0px;">

              <span style="text-decoration:underline;font-size:16pt;font-weight:bold;">Returnable</span>

            </td>
          </tr>

          <tr>
            <td colspan="12" style="text-align:center;border-bottom:0px;padding-top:3px;background-color:#f9bf90;border-right:solid 1px;">
              <ul style="font-size:8pt;margin-bottom:2px;">
                <li style="font-size:15pt;font-weight:bold;margin:0px;padding:0px;">DELIVERY CHALLAN</li>
                <li style="padding-top:5px;font-size:9pt">TRANSPORTATION OF GOODS WITHOUT ISSUE OF INVOICE UNDER RULE-55 OF CGST RULES 2017</li>
                <li style="font-size:9pt">
                  Goods supplied under lease arrangement on <b>Retunable basis</b> after complition of job (<b>Not meant for sale)</b>
                </li>
              </ul>
              <!--<span style="width:100%;margin:0px;padding:0px;" >
                <p style="font-size:16pt;font-weight:bold;margin:0px;padding:0px;">DELIVERY CHALLAN</p>
                
              </span>
              <p>
               
              </p>-->
            </td>
          </tr>
          <tr>
            <td colspan="12" style="border-top:none;border-right:solid 1px;padding-right:none;">
              <table style="width:100%;border:none;" >
                <tr>
                  <td style="border-left:none;width:150px;">
                    <img style ="max-width:250px;max-height:70px;">
                      <xsl:attribute name="src" >
                        <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                      </xsl:attribute>
                    </img>
                  </td>
                  <td class="text-center"  style="">
                    <ul style="margin-bottom:0px;padding-right:200px;">
                      <li style="font-size:14px;font-weight:bold;">
                        <xsl:value-of select="NewDataSet/Header/Company"/>
                      </li>
                      <li>
                        <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/><xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                        ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,
                        <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/> PIN: <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />

                      </li>
                      <li>
                        Phone No.: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                        <xsl:if   test ="NewDataSet/Header/CompanyPhone2 != ''">
                          ,  <xsl:value-of select="NewDataSet/Header/CompanyPhone2"/>
                        </xsl:if>
                        , Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>  <xsl:choose >
                          <xsl:when  test ="NewDataSet/Header/CompanyWebsite != ''">
                            <xsl:text> </xsl:text>, Website: <xsl:value-of select="NewDataSet/Header/CompanyWebsite"/>
                          </xsl:when>
                          <xsl:otherwise>
                          </xsl:otherwise>
                        </xsl:choose>
                      </li>
                      <li style="font-size:12px;font-weight:bold;">
                        GSTIN - <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                      </li>
                    </ul>
                    <xsl:if test="NewDataSet/Table/ShipFrom != ''">
                      <div style="width:100%;border-top:solid 1px;" class="text-center">
                        <span style="padding-right:200px;width:100%;"><xsl:value-of select="NewDataSet/Table/ShipFrom"/>
                        </span>
                      </div>
                    </xsl:if>
                  </td>
                </tr>
              </table>
            </td>
 
          </tr>
        
          <tr>
            <td colspan="6" style="border-right:0px;" class="header">
              Delivery Challan No:  <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>

            </td>
            <td colspan="6" style="border-right:solid 1px;" class="header">
              Date:  <xsl:value-of select="NewDataSet/Header/StartDate"/>
            </td>
          </tr>
          <tr>
            <td colspan="6">
              Name Of Customer
            </td>
            <td colspan="6" style="border-right:solid 1px;" >Delivery Address</td>
          </tr>
          <tr>
            <td style="width:50px;border-right:0px;">Name</td>
            <td colspan="5" class="header">

              <xsl:value-of select="NewDataSet/Header/Client"/>

            </td>
            <td>Name</td>
            <td colspan="5" style="border-right:solid 1px;" class="header">
              <xsl:choose>
                <xsl:when test="NewDataSet/Table/SezDescription != ''">
                  <xsl:value-of select="NewDataSet/Table/SezDescription" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="NewDataSet/Header/Client"/>
                </xsl:otherwise>
              </xsl:choose>
            </td>
          </tr>
          <tr>
            <td style="border-right:0px;vertical-align:top;">Address</td>
            <td colspan="5" style="vertical-align:top;padding-left:4px;">
              <ul style="margin-bottom:0px;">
                <li>
                  <xsl:value-of select="NewDataSet/Header/BillAddress1"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/BillAddress2"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/BillCity"/>
                </li>
                <li>
                  <xsl:value-of select="NewDataSet/Header/BillState"/>
                  <xsl:text> </xsl:text>
                  (<xsl:value-of select="NewDataSet/Header/BillZipCode"/>)
                </li>
              </ul>
              <!--<div style="width:100%;padding-bottom:2px;margin-top:-5px;">
             
              </div>
              <span style="display:block;width:100%;margin-top:10px;border:0px;">
              
              </span>-->

            </td>
            <td style="vertical-align:top;" >Address</td>
            <td colspan="5" style="border-right:solid 1px;vertical-align:top;">
              <ul style="margin-bottom:0px;">
                <xsl:if test="NewDataSet/Table/SiteProject != ''">
                  <li>
                    <xsl:value-of select="NewDataSet/Table/SiteProject" />
                  </li>
                  <li>
                    <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteCity"/> <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Table/SiteState"/>
                    (<xsl:value-of select="NewDataSet/Table/SiteZipCode"/>)
                  </li>
                </xsl:if>


              </ul>
              <!--<div style="width:100%;padding-bottom:2px;margin-top:-5px;">

                <xsl:if test="NewDataSet/Table/SiteProject != ''">
                  <p style="padding-bottom:5px;">
                    <xsl:value-of select="NewDataSet/Table/SiteProject" />
                  </p>

                </xsl:if>

              
              </div>-->

            </td>
          </tr>
          <tr>
            <td style="border-bottom:0px;border-right:0px;">State</td>
            <td colspan="5" >
              <xsl:value-of select="NewDataSet/Header/BillState"/> | State Code:  <xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>
              | GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
              <!--<xsl:choose>
                    <xsl:when test="NewDataSet/Table/SiteGST = ''">
                      <xsl:value-of select="NewDataSet/Table/SiteGST" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="NewDataSet/Table/ClientGST" />
                    </xsl:otherwise>
                  </xsl:choose>-->

            </td>


            <td colspan="6" style="border-right:solid 1px;" >
              Contact Person: <xsl:value-of select="NewDataSet/Table/SiteContactPerson" /> -  <xsl:value-of select="NewDataSet/Table/SiteContactPersonPhone" />
            </td>

          </tr>

          <tr>
            <td colspan="6">Tax Is Payable On Reverse Charge: No</td>

            <td  colspan="6" style="border-right:solid 1px;">
              Vehicle No: <xsl:value-of select="NewDataSet/Table/Vehicle"/>,
              Place Of Supply:    <xsl:value-of select="NewDataSet/Table/SiteCity" />
            </td>

          </tr>
          <tr>
            <td class="header" style="width:40px;text-align:center;">Sr.No</td>
            <td class="header" colspan="2" style="width:480px;">Description of Goods or/and Services</td>
            <td class="header text-center" style="width:100px;">HSN/SAC Code</td>
            <td class="header text-center" style="width:50px;">UOM</td>
            <td class="header text-center" style="width:50px;">QTY</td>
            <td class="header text-right" style="width:80px;">Rate</td>
            <td class="header text-right" style="width:90px;">Taxable Value</td>
            <td class="header text-right" style="width:80px;">IGST @ 18%</td>
            <td class="header text-right" style="width:80px;">SGST @ 9%</td>
            <td class="header text-right" style="width:80px;">CGST @ 9%</td>
            <td class="header text-right" style="width:100px;border-right:solid 1px;" >Total (Rs.)</td>
          </tr>

          <xsl:for-each select="NewDataSet/Table[Unit='Set']">

            <tr >
              <td style="text-align:center;">
                <xsl:value-of select="position()" />
              </td>
              <td colspan="2">
                <b>
                  <xsl:value-of select="Product" />
                </b>
              </td>
              <td class="text-center">
                <xsl:value-of select="SacHSNCode" />
              </td>
              <td class="text-center">
                <xsl:value-of select="Unit" />
              </td>
              <td class="text-center">
                <xsl:value-of select="SentQty" />
              </td>
              <td class="text-right">
                <xsl:choose>
                  <xsl:when test="Rate > 0">
                    <xsl:value-of select="Rate" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="SubTotal > 0">
                    <xsl:value-of select="SubTotal" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="IGST > 0">
                    <xsl:value-of select="IGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="SGST > 0">
                    <xsl:value-of select="SGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="CGST > 0">
                    <xsl:value-of select="CGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style="border-right:solid 1px;" class="text-right">

                <xsl:choose>
                  <xsl:when test="SubTotal > 0">
                    <xsl:value-of select="SubTotal" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>

          </xsl:for-each>
          <xsl:if test="count(NewDataSet/Table/Product) > 0">

            <tr >
              <td style="text-align:center;">
                <p style="color:#fff;">Product</p>
              </td>
              <td colspan="2">

              </td>
              <td class="text-center">

              </td>
              <td class="text-center">

              </td>
              <td class="text-right">

              </td>
              <td class="text-right">

              </td>
              <td class="text-right">

              </td>
              <td class="text-right">

              </td>
              <td class="text-right">

              </td>
              <td class="text-right">

              </td>
              <td style="border-right:solid 1px;" class="text-right">

              </td>
            </tr>
          </xsl:if>
          <xsl:for-each select="NewDataSet/Table[Unit !='Set']">

            <tr >
              <td style="text-align:center;">
                <xsl:value-of select="position()" />
              </td>
              <td colspan="2">
                <xsl:value-of select="Product" />
              </td>
              <td class="text-center">
                <xsl:value-of select="SacHSNCode" />
              </td>
              <td class="text-center">
                <xsl:value-of select="Unit" />
              </td>
              <td class="text-center">
                <xsl:value-of select="SentQty" />
              </td>
              <td class="text-right">
                <xsl:choose>
                  <xsl:when test="Rate > 0">
                    <xsl:value-of select="Rate" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="SubTotal > 0">
                    <xsl:value-of select="SubTotal" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="IGST > 0">
                    <xsl:value-of select="IGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="SGST > 0">
                    <xsl:value-of select="SGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="text-right">

                <xsl:choose>
                  <xsl:when test="CGST > 0">
                    <xsl:value-of select="CGST" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style="border-right:solid 1px;" class="text-right">

                <xsl:choose>
                  <xsl:when test="SubTotal > 0">
                    <xsl:value-of select="SubTotal" />
                  </xsl:when>
                  <xsl:otherwise>
                    -
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>

          </xsl:for-each>
          <tr>
            <td colspan="5" class="header text-right">Total</td>
            <td class="header text-center">
              <xsl:value-of select="sum(NewDataSet/Table[Unit !='Set']/SentQty)"/>
            </td>
            <td></td>
            <td class="header text-right">
              <xsl:value-of select="util:FormatNumber(sum(NewDataSet/Table/SubTotal))"/>
            </td>
            <td class="header text-right">
              <xsl:value-of select="NewDataSet/Table/IGSTAmount"/>
            </td>
            <td class="header text-right">
              <xsl:value-of select="NewDataSet/Table/SGSTAmount"/>
            </td>
            <td class="header text-right">
              <xsl:value-of select="NewDataSet/Table/CGSTAmount"/>
            </td>
            <td style="border-right:solid 1px;" class="header text-right">
              <xsl:value-of select="util:FormatNumber((sum(NewDataSet/Table/SubTotal) + NewDataSet/Table/IGSTAmount +
                               NewDataSet/Table/SGSTAmount +  NewDataSet/Table/CGSTAmount))"/>
            </td>
          </tr>

          <tr>
            <td colspan="12" style="border-right:solid 1px;border-bottom:0px;padding-top:10px;">

              <div class="print-tnc">
                <xsl:value-of select="NewDataSet/Header/Remarks"  disable-output-escaping="yes"/>
              </div>
              <xsl:if test="NewDataSet/Header/Tnc != ''">
                <div  class="print-tnc" style="padding-top:20px !important" >
                  <xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
                </div>
              </xsl:if>
            </td>
          </tr>

          <tr>
            <td colspan="2" style="height:80px;padding-top:2px;border-bottom:solid 1px;">


              Signatore of Receiver
            </td>
            <td colspan="4" style="border-left:0px;border-bottom:solid 1px;">
              Name and Contact Number
            </td>
            <td colspan="6" style="border-left:0px;border-bottom:solid 1px;border-right:solid 1px;vertical-align:bottom; 
                    text-align:right;padding-top:10px;">
              <p style="font-size:14px;font-weight:bold;">
                For  <xsl:value-of select="NewDataSet/Header/Company"/>
              </p>
              <p style="text-align:right;margin-top:20px;">
                <img style="height:40px;max-width:100px;float:right;">
                  <xsl:attribute name="src">
                    <xsl:value-of select="NewDataSet/Header/Signature"/>
                  </xsl:attribute>
                </img>
                <p style="margin-top:20px;">
                  Authorized Signatory
                </p>
              </p>
            </td>
          </tr>
          <!--<tr>
                <td colspan="7" style="border-top:0px;height:10px;border-bottom:solid 1px;">
                  <span>GOODS SUPPLIED TO SEZ/NOT FOR SALE/ON RETURNABLE BASIS ONLY</span>
                </td>
                <td colspan="5" style="border-top:0px;border-left:0px; border-bottom:solid 1px;border-right:solid 1px;
                    text-align:right;">


                </td>
              </tr>-->
        </table>

      </body>
    </html>
  </xsl:template>
</xsl:transform>
