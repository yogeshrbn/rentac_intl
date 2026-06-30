<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:util="urn:util-format">
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <style type="text/css" media="print">

          @media print {

          footer {
          position: fixed;
          height: 20px;
          background: red;
          color: #000000;
          border-bottom: solid 1px;
          bottom: 0.5in;
          }
          }

          .header {
          position: fixed;
          top: 0;
          }



          @page {
          margin-top: 0.25in;
          margin-bottom: 0.3in;
          padding-bottom: 20px;
          }

          body {
          padding-top: 72px;
          padding-bottom: 72px;
          padding-left: 0.25in;
          padding-right: 0.25in;
          }
        </style>
        <style>
          #printArea {

          font-family: Arial;
          }
          .border-row, td,th  {
          border:solid 1px;
          font-size:11px
          vertical-align: center;
          padding:5px;

          }


          .text-center{
          text-align:center;
          }
          .noborder{border:none !important;}
          h5 {line-height:15pt;margin:0px;font-size:16px}
          td {border:none;}
          .clientUl li {margin-bottom:5px;}
        </style>
      </head>
      <body style="border:0px;">
        <div id="printArea">
          <div style="border:solid 1px;padding:10px;" >
            <table style="width:100%;border:none;" >
              <tr>
                <td style="width:100px;">
                  <img style="height:60px;max-width:150px;">
                    <xsl:attribute name="src">
                      <xsl:value-of select="data/Company/Logo"/>
                    </xsl:attribute>
                  </img>
                </td>
                <td  style="text-align: center; vertical-align:middle;font-size:26px;"  class="noborder">
                   INDEPENDENT CONTRACTOR AGREEMENT 
                </td>
              </tr>
            </table>
            <table style="width:100%;border:none;" >

              <tr>
                <td colspan="3" class="noborder" style="text-align:center;font-size:16px;">
                  <p>
                    THIS INDEPENDENT CONTRACTOR AGREEMENT (the "Agreement") is dated effective from <b>
                  
                      <xsl:value-of select="util:DateToDDMMYYYY(data/EffectiveFrom)"/>
                    </b>.
                  </p>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:30px;" class="noborder">
                  <xsl:text> </xsl:text>

                </td>
              </tr>
              <tr >
                <td style="border:solid 1px;width:49%;vertical-align:top;">

                  <ul style="list-style:none;padding-left:0px;" class="clientUl">
                    <li style="font-size:16px;font-weight:bold;padding-bottom:10px;">Contractor</li>
                    <li>
                      <xsl:value-of select="data/Company/Name" />
                    </li>
                    <li>
                      <xsl:value-of select="data/Company/Address1" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/Company/Address2" />
                    </li>
                    <li>
                      <xsl:value-of select="data/Company/City" /> ( <xsl:value-of select="data/Company/State" />)
                    </li>
                    <li>
                      ZipCode: <xsl:value-of select="data/Company/ZipCode" />
                    </li>
                    <li>
                      GSTIN:  <xsl:value-of select="data/Company/GSTNo" />
                    </li>
                  </ul>





                </td>
                <td>
                  <xsl:text> </xsl:text>
                </td>
                <td style="border:solid 1px;width:49%">

                  <ul style="list-style:none;padding-left:0px;" class="clientUl">
                    <li style="font-size:16px;font-weight:bold;padding-bottom:10px;">Site</li>
                    <li>
                      <xsl:value-of select="data/SiteAddress" />
                    </li>
                    <li>
                      <xsl:value-of select="data/Ledger/Name" />
                    </li>
                    <li>
                      <xsl:value-of select="data/Ledger/Address1" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="data/Ledger/Address2" />
                    </li>
                    <li>
                      <xsl:value-of select="data/Ledger/City" /> ( <xsl:value-of select="data/Ledger/State" />)
                    </li>
                    <li>
                      ZipCode: <xsl:value-of select="data/Ledger/ZipCode" />
                    </li>
                    <li>
                      GSTIN:  <xsl:value-of select="data/Ledger/GSTNo" />
                    </li>
                  </ul>

                </td>
              </tr>

              <tr>
                <td colspan="3">
                  <b>BACKGROUND</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style: none; padding-left: 15px;">
                    <li>A. The Client is of the opinion that the Contractor has the necessary qualifications, experience and abilities to provide services to the Client.</li>
                    <li>B. The Contractor is agreeable to providing such services to the Client on the terms and conditions set out in this Agreement.</li>
                  </ul>
                  <div>
                    <p>
                      <b>IN CONSIDERATION OF</b> the matters described above and of the mutual benefits and obligations set forth in this Agreement,
                      the receipt and sufficiency of which consideration is hereby acknowledged, the Client and the Contractor (individually the "Party"
                      and collectively the "Parties" to this Agreement) agree as follows:
                    </p>

                  </div>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:20px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>AREA COVERED</b>
                </td>
              </tr>
              <tr>
                <td colspan="3"  >
                  <ul style="list-style: none; padding-left: 15px;">
                    <li>
                      A. The Client is agree that the total area covered in this contract is <xsl:value-of select="data/Area" />
                      <xsl:if test="data/MeasureType = 1">
                        SQFT
                      </xsl:if>
                      <xsl:if test="data/MeasureType = 2">
                        SQMTR
                      </xsl:if>
                    </li>
                    <li>
                      B. The Client is agree that the total dauration (in days) of the contract is <xsl:value-of select="data/Duration" /> Days
                    </li>

                    <li>
                      <xsl:if test="data/ContractType = '1'">
                        C. Both the parties agrees that the contract is a running contract at the rate of <xsl:value-of select="data/Rate" /> Per
                        <xsl:if test="data/MeasureType = 1">
                          SQFT
                        </xsl:if>
                        <xsl:if test="data/MeasureType = 2">
                          SQMTR
                        </xsl:if>
                      </xsl:if>
                      <xsl:if test="data/ContractType = '2'">
                        C. Both the parties agrees that the contract is a fixed value contract worth Indian Ruppes <xsl:value-of select="data/ContractValue" /> Rs.
                      </xsl:if>

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>SERVICE PROVIDED</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The Client hereby agrees to engage the Contractor to provide the Client with the following services  (the "Services"):
                      <ul>
                        <li>
                          <xsl:value-of select="data/Title" />
                        </li>
                      </ul>
                    </li>
                    <li>The Services will also include any other tasks which the Parties may agree on. The Contractor hereby agrees to provide such Services to the Client.</li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>TERMS OF AGREEMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                    <li>In the event that either Party wishes to terminate this Agreement prior to the completion of the Services, that Party will be required to provide 10 days' written notice to the other Party.</li>
                    <xsl:for-each select="data/Conditions">
                      <li>
                        <xsl:value-of select="Condition"/>
                      </li>
                    </xsl:for-each>

                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>PERFORMANCE</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>CURRENCY</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>PAYMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>REIMBERSEMENT OF EXPENSES</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>CONFIDENTIALITY</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>OWNERSHIP OF INTELLECTUAL PROPERTY</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>RETURN OF PROPERTY</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>CAPACITY/INDEPENDENT CONTRACTOR</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>NOTICE</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>INDEMNIFICATION</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>MODIFICATION OF AGREEMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>TERMS OF ASSENCE</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>ASSIGMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>
              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>ENTIRE AGREEMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>ENUREMENT</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>TITLES/HEADINGS</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>GENDER</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>GOVERNING LAW</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>SEVERABILITY</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3" style="line-height:10px;">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <b>WAIVER</b>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <ul style="list-style:auto">
                    <li style="padding-bottom:10px;">
                      The term of this Agreement (the "Term") will begin on the date of this Agreement and will remain in full force and effect until the completion of the Services,
                      subject to earlier termination as provided in this Agreement. The Term may be extended with the written consent of the Parties.

                    </li>
                  </ul>
                </td>

              </tr>
              <tr>
                <td colspan="3">
                  <b>IN WITNESS WHEREOF</b> the Parties have duly affixed their signatures under hand and seal on this ________ day of ________________, ________.
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <table style="width:100%; page-break-inside: avoid;">
                    <tr>
                      <td colspan="2"></td>
                      <td style="width:350px;">
                        <table style="width:100%;">
                          <tr>
                            <td colspan="3">
                              <xsl:value-of select="data/Ledger/Name" />
                            </td>
                          </tr>
                          <tr>
                            <td>Per:</td>
                            <td style="border-bottom:solid 1px;width:70%;"></td>
                            <td>Seal</td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr>
                      <td colspan="3">
                        <xsl:text> </xsl:text>
                      </td>
                    </tr>
                    <tr>
                      <td colspan="2"></td>
                      <td>
                        <table style="width:100%;">
                          <tr>
                            <td colspan="3">
                              <xsl:value-of select="data/Company/Name" />
                            </td>
                          </tr>
                          <tr>
                            <td>Per:</td>
                            <td style="border-bottom:solid 1px;width:70%;"></td>
                            <td>Seal</td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>





            </table>
          </div>
        </div>
      </body>
    </html>

  </xsl:template>
</xsl:transform>
