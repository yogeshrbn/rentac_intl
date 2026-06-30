<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <style>
          .header, .header-space,
          .footer, .footer-space {
          height: 100px;
          }
          .header {
          position: fixed;
          top: 0;
          }
          .footer {
          position: fixed;
          bottom: 0;
          }
          .content{height:300px;padding:2px;background-color:red;}
        </style>
      </head>
      <body>
        

        <div id="printArea">
          <div id="container">


            <table>
              <thead>
                <tr>
                  <td>
                    <div class="header-space"></div>
                  </td>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
                <tr>
                  <td>
                    <div class="content">...</div>
                  </td>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <td>
                    <div class="footer-space"></div>
                  </td>
                </tr>
              </tfoot>
            </table>
            <div class="header">...</div>
            <div class="footer">Footer</div>
          
          </div>
        </div>
      
      </body>
    </html>

  </xsl:template>
</xsl:transform>
