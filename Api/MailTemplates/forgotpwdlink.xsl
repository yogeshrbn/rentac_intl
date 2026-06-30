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
          Dear User
        </p>
        <p>
          Please click on the link below to reset your password.
        </p>
        <p>
          <xsl:attribute name="vLink">
            <xsl:value-of select="Data/ForgotPasswordVerifyLink" />
          </xsl:attribute>

       
          <xsl:element name="a">
            <xsl:attribute name="href">
              <xsl:value-of select="Data/ForgotPasswordVerifyLink"/>
            </xsl:attribute>
            <xsl:value-of select="Data/ForgotPasswordVerifyLink"/>
          </xsl:element>
        </p>
        
        <p>Thank you!</p>

        <p>
          Thanks<br/>
          Team Rentac
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
