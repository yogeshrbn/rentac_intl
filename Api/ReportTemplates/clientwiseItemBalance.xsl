<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <!-- Define keys used to group elements -->
  <xsl:key name="keyLedgerID" match="Ledger" use="LedgerId" />
  <xsl:key name="keyProduct" match="Ledger" use="Product" />

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1" crossorigin="anonymous"></link>
        <style type="text/css">
          .header {
          position: fixed;
          left: 20px;
          top: 20px;
          }

          .footer{
          position: fixed;
          left: 20px;
          top: 780px;
          }
          body,td,th {
          font-family: Verdana;
          font-size: 12px;

          }
          table
          {

          page-break-inside: avoid;
          }
          table, tr, td
          {
          page-break-inside: avoid;
          }
          td
          {  color: black;
          font-family: Arial;

          border:1px solid #000000;


          }
          .DarkBack
          {  background-color: #f3f3f3;

          color: white;
          font-weight: bold;
          }
          .LightBack
          {
          background-color: #f3f3f3;
          color: black;
          line-height:25px;
          font-size:16px;
          font-weight:500;
          }
          .RightJustified
          {  text-align: right;
          }
          .dataRow {line-height:20px;

          }
          .dataRow td {padding-left:5px;   font-size: x-small;}
        </style>

      </head>

      <body>
        <!--<div class="header">
          logo
        </div>
        <div class="footer">
          disclaimer
        </div>-->

        <table class="table table-responsive" style="width:100%;" >

          <xsl:for-each select="//Ledger[generate-id(.) = generate-id(key('keyLedgerID', LedgerId)[1])]">
            <xsl:variable name="lngLedgerId">
              <xsl:value-of select="LedgerId" />
            </xsl:variable>
            <xsl:variable name="lstProducts" select="//Ledger[LedgerId=$lngLedgerId]" />
            <xsl:call-template name="ShowProductsInLedger">
              <xsl:with-param name="lstProducts" select="$lstProducts" />
            </xsl:call-template>
          </xsl:for-each>

        </table>

      </body>
    </html>
  </xsl:template>


  <xsl:template name="ShowProductsInLedger">
    <xsl:param name="lstProducts" />

    <!-- Show the name of the Team currently being processed -->
    <tr class="LightBack">
      <td colspan="5" style="padding-left:5px;background-color:#f3f3f3;">

        <xsl:value-of select="$lstProducts[1]/ClientName" />
      </td>
      <!--<td colspan="1" class="DarkBack RightJustified">HOURS</td>-->
    </tr>

    <!-- Show the total hours for each Employee in the Team -->
    <!--<xsl:for-each select="$lstProducts [generate-id(.) = generate-id(key('keyProduct', Product)[1])]">-->
    <xsl:for-each select="$lstProducts">
      <xsl:variable name="lngProduct" select="Product" />
      <!-- Show details of each Employee -->
      <tr class="dataRow">

        <td cellspacing="0">
          <xsl:value-of select="Product" />

        </td>
        <td cellspacing="0">
          <xsl:value-of select="OpeningBalance" />
        </td>
        <td>
          <xsl:value-of select="IssuedQty" />
        </td>
        <td>
          <xsl:value-of select="ReceivedQty" />
        </td>
        <td>
          <xsl:value-of select="ClosingBalance" />
        </td>

      </tr>
    </xsl:for-each>


  </xsl:template>


</xsl:transform>
