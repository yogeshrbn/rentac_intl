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
          
          </center>
          <br/>
          <br/>
        </p>
        <p>
          Dear <xsl:value-of select="Data/PartyName" />
        </p>
        <p>
          Items have been inssued to your vide challan # <xsl:value-of select="Data/ChallanNumber"/>
        </p>

        <p>The detailed challan is attached with this email.</p>

        <p>Thank you!</p>

        <p>
          Thanks<br/>
          <xsl:value-of select="Data/Company" />
        </p>
      </body>
    </html>
  </xsl:template>
</xsl:transform>
