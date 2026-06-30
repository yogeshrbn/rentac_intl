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
          Welcome <xsl:value-of select="Data/Company" /> !
        </p>
        <p>Thank you for subscribing Rentac!</p>
        <p>As a valued customer you can avail all of the Rentac features to manage your business.</p>
        <p>
          Now you can manage your clients,inventory, challans and generate invoice in no-time. We are committed to support you whenever required.
        </p>
        <p>We also committed to improve Rentac with new tools and features to give you a smooth experience.</p>
        <p>
          NOTE: You will receive your login credentials in a separate email
        </p>
        <br/>
        <p>
          Feel free to contact us at <a href="tel:9899730364">9899730364</a>
          OR
          write us at  <a href="mailto:contact@rbntechnologies.com">contact@rbntechnologies.com</a>
        </p>
        <h3>Thank you once again!</h3>




        <p>
          <br/>
          Team Rentac
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
