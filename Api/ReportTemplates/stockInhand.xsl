<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
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
          <tr>
            <th>Item Code</th>
            <th>Item Name</th>
            <th>Balance</th>
            </tr>
          <xsl:for-each select="Data/Stock">
            <tr class="dataRow">

              <td cellspacing="0">
                <xsl:value-of select="Code" />
              </td>
              <td cellspacing="0">
                <xsl:value-of select="Name" />
              </td>
               <td>
                <xsl:value-of select="ClosingBalance" />
              </td>

            </tr>
          </xsl:for-each>

        </table>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
