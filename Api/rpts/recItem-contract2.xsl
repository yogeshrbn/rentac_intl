<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
      </head>
      <body>
        <div id="printArea">

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
            .tnc ul {
            list-style:disc;
            }
            .tnc ul li {
            <!--margin:0px !important;
            margin-top:0px !important;
            padding-top:0px !important;
            padding-bottom:0px !important;-->
            line-height:12px;
            display:inline-block;
            font-size:11px !important;
            }

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
          </style>
          <div id="container" class="container" style="padding-top:10px;">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <tr>
                <td colspan="9" style="border:solid 1px;border-bottom:0px;padding:8px;text-align:right">

                  <span style="text-decoration:underline;font-size:16px;font-weight:bold;">Returnable</span>

                </td>
              </tr>
              <tr>
                <td colspan="9" style="border:solid 1px;border-bottom:0px;padding:5px;text-align:right">
                  <span style="text-decoration:underline;font-size:14px;">Original for Consignee</span>

                </td>
              </tr>
              <tr>
                <td colspan="9" style="text-align:center;border-bottom:0px;padding:5px;background-color:#f9bf90;border-right:solid 1px;">
                  <span style="width:100%;" >
                    <span style="font-size:16px;font-weight:bold">DELIVERY CHALLAN</span><br/>TRANSPORTATION OF GOODS WITHOUT ISSUE OF INVOICE UNDER RULE-55 OF CGST RULES 2017
                  </span>
                  <p>
                    Goods supplied under lease arrangement on <b>Retunable basis</b> after complition of job (<b>Not meant for sale)</b>
                  </p>
                </td>
              </tr>
              <tr>
                <td colspan="2" rowspan="2" style="padding:5px;border:solid 1px;
                    border-bottom:0px;border-right:0px;">

                  <img style ="width:140px;max-height:80px;">
                    <xsl:attribute name="src" >
                      <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                    </xsl:attribute>
                  </img>

                </td>
                <td colspan="7" style="text-align:center;padding:5px;border-bottom:0px;padding-top:10px;border-right:solid 1px;">
                  <div style="padding-right:150px;">
                    <p style="font-size:18px;font-weight:bold;margin-bottom:5px;">
                      <xsl:value-of select="NewDataSet/Header/Company"/>
                    </p>
                    <p style="margin-bottom:5px;">
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/><xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                      ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,
                      <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/> PIN: <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />

                    </p>
                    <p style="margin-top:5px;">
                      Phone No.: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                      <xsl:if   test ="NewDataSet/Header/CompanyPhone2 != ''">
                        ,  <xsl:value-of select="NewDataSet/Header/CompanyPhone2"/>
                      </xsl:if>
                      , Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                    </p>
                    <p style="margin-top:10px;">
                      Website:
                      <xsl:choose >
                        <xsl:when  test ="NewDataSet/Header/CompanyWebsite != ''">
                          <xsl:value-of select="NewDataSet/Header/CompanyWebsite"/>
                        </xsl:when>
                        <xsl:otherwise>
                          NA
                        </xsl:otherwise>
                      </xsl:choose>

                    </p>
                  </div>
                </td>
              </tr>
              <tr>
                <td colspan="7" style="text-align:center;border-right:solid 1px;">
                  <p style="font-size:12px;font-weight:bold;padding-right:150px;">
                    GSTIN - <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                  </p>
                </td>

              </tr>
              <tr>
                <td  colspan="4"   class="header">
                  Delivery Challan No.:  <xsl:value-of select="NewDataSet/Header/GRN"/>
                </td>



                <td colspan="2" style="border-right:0px;"  class="header">
                  Date:   <xsl:value-of select="NewDataSet/Header/ReceivingDate"/>
                </td>
                <td colspan="3" style="border-left:0px;border-right:solid 1px;">

                </td>
              </tr>
              <tr>
                <td colspan="9" style="border-right:solid 1px;text-align:center;" class="header" >
                  Ship From:
                  <xsl:value-of select="NewDataSet/Header/Client"/>

                  GSTIN: <xsl:value-of select="NewDataSet/Header/ClientGSTNo"/>
                  <div style="padding-bottom:5px;padding-top:10px;">
                    Site: <xsl:if test="NewDataSet/Header/SiteProject != ''">
                      <xsl:value-of select="NewDataSet/Header/SiteProject" />,
                    </xsl:if>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                    <xsl:text> </xsl:text>
                  </div><div >

                    <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                    <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Table/SiteCity"/><xsl:text>, </xsl:text>
                    <xsl:value-of select="NewDataSet/Table/SiteStateName"/><xsl:text> </xsl:text>
                    <xsl:text> </xsl:text>,
                    PIN: <xsl:value-of select="NewDataSet/Table/SiteZipCode" />
                  </div>
                </td>
              </tr>
              <tr>
                <td colspan="4">
                  Name Of Customer
                </td>
                <td colspan="5" style="border-right:solid 1px;" >Delivery Address</td>
              </tr>
              <tr>
                <td>Name</td>
                <td colspan="3" class="header">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
                <td>Name</td>
                <td colspan="4" style="border-right:solid 1px;" class="header">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
              </tr>
              <tr>
                <td style="vertical-align:top;padding-top:0px;">Address</td>
                <td colspan="3" style="padding:5px;padding-top:0px;vertical-align:top">
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                  <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                  ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,PIN: <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                  ,<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>

                </td>
                <td  style="vertical-align:top;padding-top:0px;">Address</td>
                <td colspan="4" style="padding:5px;padding-top:0px;border-right:solid 1px;vertical-align:top;">
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                  <xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                  ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,PIN: <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                  ,<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>
                </td>
              </tr>
              <tr>
                <td style="border-bottom:0px;" colspan="4">
                  State

                  <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                </td>

                <td colspan="5" style="border-right:solid 1px;">
                  Contact Person: <xsl:value-of select="NewDataSet/Header/CompanyContactPerson" />, Mob:  <xsl:value-of select="NewDataSet/Header/CompanyPhone1" />
                </td>
              </tr>
              <tr>
                <td   style="border-bottom:0px;" colspan="4">
                  State Code:
                  <xsl:value-of select="NewDataSet/Header/CompanyStateGSTCode"/>
                  |
                  GSTIN:
                  <xsl:value-of select="NewDataSet/Header/CompanyGSTNo" />
                </td>
                <td style="border-bottom:0px;border-right:solid 1px;" colspan="5">
                  Place Of Supply

                  <xsl:value-of select="NewDataSet/Header/CompanyState" />
                </td>
              </tr>
              <tr>
                <td colspan="4">Tax Is Payable On Reverse Charge: No</td>
                <td colspan="5" style="border-right:solid 1px;" class="header">
                  Vehicle No: <xsl:value-of select="NewDataSet/Header/vehicleNo"/>
                </td>
              </tr>
              <tr>
                <td class="header text-center" style="width:40px;">Sr.No</td>
                <td class="header" colspan="3"  >Description of Goods or/and Services</td>
                <td class="header text-center" style="width:70px;">HSN/SAC</td>
                <td class="header text-center" style="width:50px;">UOM</td>
                <td class="header text-right" style="width:30px;">QTY</td>
                <td class="header text-right" style="width:90px;">Damage (If any)</td>
                <td class="header text-right" style="width:100px;border-right:solid 1px;">Taxable Value</td>
              </tr>

              <xsl:for-each select="NewDataSet/Table[Unit='Set']">
                <tr >
                  <td class="text-center">
                    <xsl:value-of select="position()" />
                  </td>
                  <td colspan="3">
                    <xsl:value-of select="Item" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="HSNCode" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="Unit" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="Quantity" />
                  </td>
                  <td class="text-right">
                    <xsl:choose>
                      <xsl:when test="Breakage > 0">
                        <xsl:value-of select="Breakage" />
                      </xsl:when>
                      <xsl:otherwise>
                        -
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                  <td class="text-right" style="border-right:solid 1px;">
                    <xsl:choose>
                      <xsl:when test="ItemSubTotal > 0">
                        <xsl:value-of select="format-number(ItemSubTotal,'#.00')" />
                      </xsl:when>
                      <xsl:otherwise>
                        -
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>

                </tr>
              </xsl:for-each>

              <xsl:if test="count(NewDataSet/Table[Unit='Set']) > 0">

                <tr >
                  <td style="text-align:center;">
                    <p style="color:#fff;">Product</p>
                  </td>
                  <td colspan="3">

                  </td>
                  <td class="text-center">

                  </td>
                  <td class="text-center">

                  </td>
                  <td class="text-right">

                  </td>
                  <td class="text-right">

                  </td>
                  <td class="text-right" style="border-right:solid 1px;">

                  </td>

                </tr>

              </xsl:if>
              <xsl:for-each select="NewDataSet/Table[Unit != 'Set']">
                <tr >
                  <td class="text-center">
                    <xsl:value-of select="position()" />
                  </td>
                  <td colspan="3">
                    <xsl:value-of select="Item" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="HSNCode" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="Unit" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="Quantity" />
                  </td>
                  <td class="text-right">
                    <xsl:choose>
                      <xsl:when test="Breakage > 0">
                        <xsl:value-of select="Breakage" />
                      </xsl:when>
                      <xsl:otherwise>
                        -
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                  <td class="text-right" style="border-right:solid 1px;">
                    <xsl:choose>
                      <xsl:when test="ItemSubTotal > 0">
                        <xsl:value-of select="format-number(ItemSubTotal,'#.00')" />
                      </xsl:when>
                      <xsl:otherwise>
                        -
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>

                </tr>
              </xsl:for-each>
              <tr>
                <td colspan="8" class="text-right" style="border-bottom:solid 1px;">Total</td>

                <td style="border-right:solid 1px;border-bottom:solid 1px;" class="header text-right">
                  <xsl:value-of select="format-number(sum(NewDataSet/Table/ItemSubTotal),'#.00') "/>

                </td>
              </tr>

              <tr>
                <td colspan="2" style="height:80px;border-top:0px;">


                  Signatore of Receiver
                </td>
                <td colspan="2" style="border-left:0px;border-top:0px;">
                  Name and Contact Number
                </td>
                <td colspan="5"  style="vertical-align:bottom;font-size:14px;font-weight:bold; border-left:0px;border-top:0px;border-right:solid 1px; text-align:right;">

                  For  <xsl:value-of select="NewDataSet/Header/Company"/>


                </td>
              </tr>
              <tr>
                <td colspan="9" style="border-top:0px;border-right:solid 1px;height:50px;padding-top:40px;border-bottom:solid 1px;text-align:right;">

                  Authorized Signatory

                </td>
              </tr>
              <tr>
                <td colspan="9" style="vertical-align:top;border-top:0px;border-right:solid 1px;height:80px;border-bottom:solid 1px;">
                  <div style="margin-bottom:20px;" class="tnc">
                    <xsl:value-of select="NewDataSet/Header/Remarks" disable-output-escaping="yes"/>
                  </div>
                  <div style="margin-bottom:20px" class="tnc">
                    <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
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
