<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <xsl:variable name="rowSpan">
      <xsl:choose>
        <xsl:when test="NewDataSet/Table/ShipFrom != ''">3</xsl:when>
        <xsl:otherwise>2</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          .noborder {border:0px;

          }
          .nopadding {
          padding:0px;
          line-height: 20px !important;
          }
          .ulitems {line-height:15px;}
          td{
          font-size:10px;
          padding:2px;
          border-bottom:0px;
          border-right:0px;
          }
          .header{
          font-weight:bold
          }
          .text-center{text-align:center;}
          .text-right{text-align:right;}

          @media print {
          /* Styles within this block apply only when printing */
          body, html {
          margin: 0 !important;
          padding: 0 !important;
          }

          /* You can also target specific elements if needed */
          p, h1, h2, div {
          margin: 0;
          padding: 0;
          }

          /* Remove default page margins if desired */
          @page {
          margin: 0;
          }
          }
          .print-tnc ul {
          list-style:disc;
          }
          .print-tnc ul li {
          margin-bottom:-5px;
          font-size:9px;
          }

        </style>
        <link rel="stylesheet" href="print.css" />
      </head>

      <body>

        <div >


          <div  style="">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <tr>
                <td colspan="6" style="border:solid 1px;border-bottom:0px;padding:5px;border-right:0px;">
                  <span style="text-decoration:underline;font-size:14px;">Original for Consignee</span>

                </td>
                <td colspan="6" style="border:solid 1px;border-left:0px;border-bottom:0px;padding:4px;text-align:right">

                  <span style="text-decoration:underline;font-size:16px;font-weight:bold;">Returnable</span>

                </td>
              </tr>

              <tr>
                <td colspan="12" style="text-align:center;border-bottom:0px;padding:5px;background-color:#f9bf90;border-right:solid 1px;">
                  <span style="width:100%;margin:0px;padding:0px;" >
                    <p style="font-size:16px;font-weight:bold;margin:0px;padding:0px;">DELIVERY CHALLAN</p>
                    <br/>TRANSPORTATION OF GOODS WITHOUT ISSUE OF INVOICE UNDER RULE-55 OF CGST RULES 2017
                  </span>
                  <p>
                    Goods supplied under lease arrangement on <b>Retunable basis</b> after complition of job (<b>Not meant for sale)</b>
                  </p>
                </td>
              </tr>
              <tr>
                <td colspan="2" rowspan="{$rowSpan}" style="padding:5px;height:80px;border:solid 1px;
                    border-bottom:0px;border-right:0px;">

                  <img style ="max-width:250px;max-height:80px;">
                    <xsl:attribute name="src" >
                      <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                    </xsl:attribute>
                  </img>

                </td>
                <td colspan="10" style="text-align:center;padding:5px;border-bottom:0px;
                   border-right:solid 1px;">
                  <p style="font-size:14px;font-weight:bold;margin-right:250px;">
                    <xsl:value-of select="NewDataSet/Header/Company"/>
                  </p>
                  <p style="margin-right:250px;">
                    <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/><xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                    ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,
                    <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/> PIN: <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />

                    <br/>

                  </p>
                  <p style="margin-right:250px;margin-top:-5px;">
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
                  </p>
                </td>
              </tr>
              <tr>
                <td colspan="10" style="text-align:center;border-top:0px;border-right:solid 1px;padding:none;border-left:none;">
                  <p style="font-size:12px;font-weight:bold;margin-right:250px;">
                    GSTIN - <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                  </p>
                             
                </td>

              </tr>
              <xsl:if test="NewDataSet/Table/ShipFrom != ''">
              <tr>
                <td colspan="10" style="text-align:center;border-top:solid 1px;height:20px;border-right:solid 1px;padding:none;border-left:none;">
                  <p style="font-size:12px;font-weight:bold;margin-right:250px;">
                    Ship From - <xsl:value-of select="NewDataSet/Table/ShipFrom"/>
                  </p>

                </td>

              </tr>
              </xsl:if>
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
                  <!--<xsl:value-of select="NewDataSet/Header/Client"/>-->
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
                <td colspan="5" style="vertical-align:top;">
                  <div style="width:100%;padding-bottom:2px;margin-top:-5px;">
                    <xsl:value-of select="NewDataSet/Header/BillAddress1"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/BillAddress2"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Header/BillCity"/>
                  </div>
                  <span style="display:block;width:100%;margin-top:10px;border:solid 1px;red;">
                    <xsl:value-of select="NewDataSet/Header/BillState"/>
                    <xsl:text> </xsl:text>
                    (<xsl:value-of select="NewDataSet/Header/BillZipCode"/>)
                  </span>

                </td>
                <td style="vertical-align:top;" >Address</td>
                <td colspan="5" style="border-right:solid 1px;">
                  <div style="width:100%;padding-bottom:2px;margin-top:-5px;">

                    <xsl:if test="NewDataSet/Table/SiteProject != ''">
                      <p style="padding-bottom:5px;">
                        <xsl:value-of select="NewDataSet/Table/SiteProject" />
                      </p>

                    </xsl:if>

                    <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteCity"/> <xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Table/SiteState"/>
                    (<xsl:value-of select="NewDataSet/Table/SiteZipCode"/>)
                  </div>

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
                  <td class="text-right">
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
                  <td class="text-right">
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
                <td class="header text-right">
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
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
