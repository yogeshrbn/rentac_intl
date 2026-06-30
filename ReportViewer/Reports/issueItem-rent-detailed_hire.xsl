<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
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
          font-size:11px;
          padding:5px;
          border-bottom:0px;
          border-right:0px;
          }
          .header{
          font-weight:bold
          }
          .text-center{text-align:center;}
          .text-right{text-align:right;}

        </style>

      </head>

      <body>

        <div id="printArea">


          <div id="container" class="container" style="padding-top:10px;">
            <table style="width: 100%;" border="0px"  cellspacing="0">
              <tr>
                <td colspan="12" style="border:solid 1px;border-bottom:0px;padding:8px;text-align:right">

                  <span style="text-decoration:underline;font-size:20px;font-weight:bold;">Returnable</span>

                </td>
              </tr>
              <tr>
                <td colspan="12" style="border:solid 1px;border-bottom:0px;padding:5px;text-align:right">
                  <span style="text-decoration:underline;font-size:14px;">Original for Consignee</span>

                </td>
              </tr>
              <tr>
                <td colspan="12" style="text-align:center;border-bottom:0px;padding:5px;background-color:#f9bf90;border-right:solid 1px;">
                  <span style="width:100%;" >
                    <span style="font-size:16px;font-weight:bold">DELIVERY CHALLAN</span><br/>TRANSPORTATION OF GOODS WITHOUT ISSUE OF INVOICE UNDER RULE-55 OF CGST RULES 2017
                  </span>
                  <p>
                    Goods supplied under lease arrangement on <b>Retunable basis</b> after complition of job (<b>Not meant for sale)</b>
                  </p>
                </td>
              </tr>
              <tr>
                <td colspan="2" rowspan="2" style="padding:5px;height:80px;border:solid 1px;border-bottom:0px;border-right:0px;">

                  <img style ="max-width:100px;">
                    <xsl:attribute name="src" >
                      <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                    </xsl:attribute>
                  </img>

                </td>
                <td colspan="10" style="text-align:center;padding:5px;border-bottom:0px;padding-top:20px;border-right:solid 1px;">
                  <p style="font-size:18px;font-weight:bold">
                    <xsl:value-of select="NewDataSet/Header/Company"/>
                  </p>
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                  ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,<xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                  ,<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>
                  <br />
                  Phone No.: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>, <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                </td>
              </tr>
              <tr>
                <td colspan="10" style="text-align:center;border-right:solid 1px;">
                  <p style="font-size:14px;font-weight:bold;">
                    GSTIN - <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                  </p>
                </td>

              </tr>
              <tr>
                <td colspan="2" >Delivery Challan No.</td>
                <td colspan="2" class="header">
                  <xsl:value-of select="NewDataSet/Header/ChallanNumber"/>
                </td>
                <td>Date</td>
                <td colspan="7" style="border-right:solid 1px;" class="header">
                  <xsl:value-of select="NewDataSet/Header/StartDate"/>
                </td>
              </tr>
              <tr>
                <td colspan="13" style="border-right:solid 1px;" >
                  Ship From
                  <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                </td>
              </tr>
              <tr>
                <td colspan="6">
                  Name Of Customer
                </td>
                <td colspan="6" style="border-right:solid 1px;" >Delivery Address</td>
              </tr>
              <tr>
                <td>Name</td>
                <td colspan="5" class="header">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
                <td>Name</td>
                <td colspan="5" style="border-right:solid 1px;" class="header">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
              </tr>
              <tr>
                <td>Address</td>
                <td colspan="5">
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                  ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,<xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                  ,<xsl:text> </xsl:text><xsl:value-of select="NewDataSet/Header/CompanyState"/>

                  <xsl:value-of select="NewDataSet/Header/BillAddress1"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/BillAddress2"/>

                  <br/>
                  <xsl:value-of select="NewDataSet/Header/BillCity"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/BillZipCode"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/BillState"/>
                </td>
                <td>Address</td>
                <td colspan="5" style="padding:5px;border-right:solid 1px;">
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/><xsl:text> </xsl:text>
                  ,<xsl:value-of select="NewDataSet/Header/CompanyCity"/><xsl:text> </xsl:text>,<xsl:value-of select="NewDataSet/Header/CompanyZipCode" />

                </td>
              </tr>
              <tr>
                <td style="border-bottom:0px;">State</td>
                <td colspan="5">
                  <xsl:value-of select="NewDataSet/Header/BillState"/>
                </td>
                <td></td>
                <td colspan="5" style="border-right:solid 1px;">
                  Contact Person- <xsl:value-of select="NewDataSet/Table/CompanyContactPerson" /> -  <xsl:value-of select="NewDataSet/Header/CompanyPhone1" />
                </td>
              </tr>
              <tr>
                <td style="width:80px;border-bottom:0px;">  State Code</td>
                <td  style="width:120px;border-bottom:0px;">
                  <xsl:value-of select="NewDataSet/Header/CompanyStateGSTCode"/>
                </td>
                <td  style="width:120px;border-bottom:0px;">GSTIN</td>
                <td colspan="3" style="border-bottom:0px;" class="header">
                  <xsl:value-of select="NewDataSet/Header/CompanyGSTNo" />
                </td>
                <td style="border-bottom:0px;">Place Of Supply</td>
                <td colspan="5" style="border-right:solid 1px;">
                  <xsl:value-of select="NewDataSet/Header/CompanyCity" />
                </td>
              </tr>
              <tr>
                <td colspan="3">Tax Is Payable On Reverse Charge</td>
                <td colspan="3">No</td>
                <td class="header">Vehicle No</td>
                <td colspan="5" style="border-right:solid 1px;" class="header">
                  <xsl:value-of select="NewDataSet/Header/Vehicle"/>
                </td>
              </tr>
              <tr>
                <td class="header" style="width:80px;">Sr.No</td>
                <td class="header" colspan="2" style="">Description of Goods or/and Services</td>
                <td class="header text-center" style="width:100px;">HSN/SAC Code</td>
                <td class="header text-center" style="width:80px;">UOM</td>
                <td class="header text-center" style="width:80px;">QTY</td>
                <td class="header text-right" style="width:115px;">Rate</td>
                <td class="header text-right" style="width:100px;">Taxable Value</td>
                <td class="header text-right" style="width:100px;">IGST @ 18%</td>
                <td class="header text-right" style="width:100px;">SGST @ 9%</td>
                <td class="header text-right" style="width:100px;">CGST @ 9%</td>
                <td class="header text-right" style="width:100px;border-right:solid 1px;" >Total (Rs.)</td>
              </tr>
              <xsl:for-each select="NewDataSet/Table">
                <tr >
                  <td >
                    <xsl:value-of select="position()" />
                  </td>
                  <td colspan="2">
                    <xsl:value-of select="Product" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="HSNCode" />
                  </td>
                  <td class="text-center">
                    <xsl:value-of select="Unit" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="SentQty" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="Rate" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="SubTotal" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="IGST" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="SGST" />
                  </td>
                  <td class="text-right">
                    <xsl:value-of select="CGST" />
                  </td>
                  <td style="border-right:solid 1px;" class="text-right">
                    <xsl:value-of select="SubTotal" />
                  </td>
                </tr>
              </xsl:for-each>
              <tr>
                <td colspan="7"></td>
                <td class="header text-right">
                  <xsl:value-of select="sum(NewDataSet/Table/SubTotal)"/>
                </td>
                <td class="header text-right">
                  <xsl:value-of select="sum(NewDataSet/Table/IGST)"/>
                </td>
                <td class="header text-right">
                  <xsl:value-of select="sum(NewDataSet/Table/SGST)"/>
                </td>
                <td class="header text-right">
                  <xsl:value-of select="sum(NewDataSet/Table/CGST)"/>
                </td>
                <td style="border-right:solid 1px;" class="header text-right">
                  <xsl:value-of select="sum(NewDataSet/Table/SubTotal) "/>
                </td>
              </tr>
              <tr>
                <td colspan="12" style="border-right:solid 1px;border-bottom:0px;padding-top:10px;">
                  <p style="font-size:16px;">Additional Information</p>
                  <p style="margin-bottom:10px;">
                    <xsl:value-of select="NewDataSet/Header/Remarks"/>
                  </p>

                  <p style="font-size:16px;">Terms and Conditions</p>
                  <xsl:value-of select="NewDataSet/Header/Tnc"/>
                </td>
              </tr>
              <tr>
                <td colspan="2" style="height:80px;border-top:0px;">


                  Signatore of Receiver
                </td>
                <td colspan="4" style="border-left:0px;border-top:0px;">
                  Name and Contact Number
                </td>
                <td colspan="6" style="border-left:0px;border-top:0px;border-right:solid 1px; text-align:right;">
                  <p style="font-size:16px;font-weight:bold;">
                    For  <xsl:value-of select="NewDataSet/Header/Company"/>
                  </p>

                </td>
              </tr>
              <tr>
                <td colspan="7" style="border-top:0px;height:40px;border-bottom:solid 1px;">
                  <span>GOODS SUPPLIED TO SEZ/NOT FOR SALE/ON RETURNABLE BASIS ONLY</span>
                </td>
                <td colspan="5" style="border-top:0px;border-left:0px; border-bottom:solid 1px;border-right:solid 1px;text-align:right;">
                  <p>
                    Authorized Signatory
                  </p>

                </td>
              </tr>
            </table>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
