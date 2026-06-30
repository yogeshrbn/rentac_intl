<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

        <style type="text/css">
          body,td,th {
          font-family: Verdana;
          font-size: 12px;
          }
        </style>
      </head>

      <body>
        <p>
          <center>
            <img>
              <!--<xsl:attribute name="src">
          http://woodside.worldsportsrx.com/Baseball/images/<xsl:value-of select="UserInfo/Sport"/>-logo-small.png
        </xsl:attribute>-->
            </img>
          </center>
          <br/>
          <br/>
        </p>
        <p>
          Dear <xsl:value-of select="Data/Name" />
        </p>
       
        <p>
          Your account has been created successfully.
        </p>
        <p>
          Login details are:
        </p>
        <table>
          <tr>
            <td>Login Name</td>
            <td>
              <xsl:value-of select="Data/LoginName"/>
            </td>
          </tr>
          <tr>
            <td>Password</td>
            <td>
              <xsl:value-of select="Data/Password"/>
            </td>
          </tr>
        </table>



        <p>Thank you!</p>

        <p>
          Thanks<br/>
          Team Rentac
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
