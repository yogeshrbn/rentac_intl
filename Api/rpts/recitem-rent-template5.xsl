<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:util="urn:util-format">
  <xsl:template match="/">
    <xsl:variable name="itemCount" select="count(NewDataSet/Table)"/>
    <!-- Store the value in a variable for reuse -->
    <xsl:variable name="printWeightSetting"
                  select="//Config/root[Key='printWeightOnChallan']/Value"/>

    <!-- Receiving challan print settings (RECEIVINGCHALLAN / RECEIVINGCHALLAN), merged into XML by HomeController -->
    <xsl:variable name="cfgPrintShowBreakageCharges" select="string(//Config/root[Key='printShowBreakageCharges']/Value)"/>
    <xsl:variable name="cfgPrintShowContactNo" select="string(//Config/root[Key='printShowContactNo']/Value)"/>
    <xsl:variable name="cfgPrintShowCartage" select="string(//Config/root[Key='printShowCartage']/Value)"/>
    <xsl:variable name="cfgPrintShowTime" select="string(//Config/root[Key='printShowTime']/Value)"/>
    <xsl:variable name="cfgPrintShowClientGSTIN" select="string(//Config/root[Key='printShowClientGSTIN']/Value)"/>
    <xsl:variable name="cfgPrintAddBreakageQtyInQuantityColumn" select="string(//Config/root[Key='printAddBreakageQtyInQuantityColumn']/Value)"/>

    <xsl:variable name="printShowBreakageChargesOn"
                  select="$cfgPrintShowBreakageCharges = '1' or $cfgPrintShowBreakageCharges = 'true' or $cfgPrintShowBreakageCharges = 'True'"/>
    <xsl:variable name="printShowContactNoOn"
                  select="$cfgPrintShowContactNo = '1' or $cfgPrintShowContactNo = 'true' or $cfgPrintShowContactNo = 'True'"/>
    <xsl:variable name="printShowCartageOn"
                  select="$cfgPrintShowCartage = '1' or $cfgPrintShowCartage = 'true' or $cfgPrintShowCartage = 'True'"/>
    <xsl:variable name="printShowTimeOn"
                  select="$cfgPrintShowTime = '1' or $cfgPrintShowTime = 'true' or $cfgPrintShowTime = 'True'"/>
    <!-- Default on when key missing (legacy); off only when explicitly 0/false -->
    <xsl:variable name="printShowClientGSTINOn"
                  select="not($cfgPrintShowClientGSTIN = '0' or translate($cfgPrintShowClientGSTIN, 'FALSE', 'false') = 'false')"/>
    <xsl:variable name="printAddBreakageQtyInQuantityColumnOn"
                  select="$cfgPrintAddBreakageQtyInQuantityColumn = '1' or $cfgPrintAddBreakageQtyInQuantityColumn = 'true' or $cfgPrintAddBreakageQtyInQuantityColumn = 'True'"/>

    <xsl:variable name="sumQuantity" select="sum(NewDataSet/Table/Quantity)"/>
    <xsl:variable name="sumBreakage" select="sum(NewDataSet/Table/Breakage)"/>

    <xsl:variable name="mainColSpan">
      <xsl:choose>
        <xsl:when test="$printShowBreakageChargesOn">6</xsl:when>
        <xsl:otherwise>5</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="shipToColSpan">
      <xsl:choose>
        <xsl:when test="$printShowBreakageChargesOn">4</xsl:when>
        <xsl:otherwise>3</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="spacerDataColSpan">
      <xsl:choose>
        <xsl:when test="$printShowBreakageChargesOn">5</xsl:when>
        <xsl:otherwise>4</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="qtyWordsColSpan">
      <xsl:choose>
        <xsl:when test="$printShowBreakageChargesOn">5</xsl:when>
        <xsl:otherwise>4</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style>
          .main-sheet {  height: 100%;display: table; width: 100%; border-collapse: collapse; table-layout: fixed; }
          .main-sheet td, .main-sheet th { border: solid 1px #000; padding: 5px; }
          .main-sheet tr.spacer-row td { height: 100%; vertical-align: top; border: solid 1px #000; }
          ul{
          list-style:none;
          }
          ul li {

          padding-left:0px;
          margin-left:0px;
          padding-top:3px;
          font-size:11pt;
          }
          @media print {

          body {
          font-size:10pt;
          font-family:arial;
          height: 100vh;
          }
          .noborder {border:0px;

          }
          td {
          border:solid 1px;
          padding:5px;
          }
          .nopadding {
          padding:0px;
          line-height: 20px !important;
          }
          .ulitems {line-height:15px;}
          .tnc p,li {
          padding:0px;
          margin:0px;
          }
          .detailsRow td {padding:5px; !important;}
          .subheading {font-size:11pt;font-weight:bold;}
          ul{
          list-style:none;
          }
          ul li {

          padding-left:0px;
          margin-left:0px;
          padding-top:3px;
          font-size:11pt;
          }
          .printArea {
          height: 335mm; /* A4 (297mm) - top/bottom margins (20mm each) */
          }
          .main-sheet { width: 100%; border-collapse: collapse; table-layout: fixed; }
          .main-sheet td, .main-sheet th { border: solid 1px #000; padding: 5px; }
          .main-sheet tr.spacer-row td { height: 100%; vertical-align: top; border: solid 1px #000; }
          }
        </style>

      </head>

      <body>

        <div id="printArea" class="printArea">

          <div id="container" class="container" style="padding-top:5px;">
            <table class="main-sheet" style="width: 100%;" border="0px"  cellspacing="0">
              <colgroup>
                <col style="width:50px"/>
                <col style="width:280px"/>
                <col style="width:80px"/>
                <col style="width:80px"/>
                <xsl:if test="$printShowBreakageChargesOn">
                  <col style="width:72px"/>
                </xsl:if>
                <col style="width:150px"/>
              </colgroup>
              <tr>
                <td style="padding:0px;border-bottom:0px;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  <table style="width:100%" border="0">
                    <tr>
                      <td style="width:30%;border:0px;" class="subheading">
                        GSTIN: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
                      </td>
                      <td style="text-align:center;border:0px;"  class="subheading">
                        <xsl:choose >
                          <xsl:when test="NewDataSet/Table/TypeId = 10">
                            Hire  Challan
                          </xsl:when>
                          <xsl:otherwise>
                            Receiving Goods
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td style="width:30%;border:0px;text-align:right;"  class="subheading">
                        Mob: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                      </td>
                    </tr>

                  </table>
                </td>
              </tr>
              <tr>
                <td style="padding:0px;border-top:0px;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  <table style="width:100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                      <td style="width:100px;border:0px;"  >
                        <img style="height:60px;max-width:100px;">
                          <xsl:attribute name="src">
                            <xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
                          </xsl:attribute>
                        </img>
                      </td>
                      <td    style="width:80%;text-align:center;border:0px;font-size:18pt;font-weight:bold;">
                        <xsl:value-of select="NewDataSet/Header/Company"/>
                      </td>
                      <td  style="height:60px;max-width:100px;border:0px;"> </td>
                    </tr>
                  </table>
                </td>
              </tr>
              <!--<tr  class="padding" style="height:25px;padding:3px;">
                <td   colspan="2"  style="width: 25%;border-right:0px;border-bottom:0px;"   class="padding">
                
                </td>
                <td class="subheading"  style="border-right:0px;border-left:0px;border-bottom:0px; text-align: center;">
                 
                </td>
                <td colspan="2"  style="width: 25%;border-left:0px; text-align:right;border-bottom:0px;" class="padding">
               
                </td>

              </tr>-->
              <!--<tr class="padding">
                <td colspan="5" style="text-align:center;border-top:none;border-bottom:0px;font-size:20pt;font-weight:bold;">
                  <xsl:value-of select="NewDataSet/Header/Company"/>
                </td>
              </tr>-->
              <tr class="padding">
                <td class="subheading" style="text-align:center;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  Deals In : Steel Plates, Channel, Props, Scaffolding and All Shuttering Materials ON HIRE BASIS
                </td>
              </tr>
              <tr class="padding">
                <td style="border-top:none !important; text-align: center;border-bottom:0px;" class="padding">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  <ul style="margin:0px;">
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    </li>
                    <li>
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      ,
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </li>
                  </ul>
                </td>
              </tr>
              <tr class="padding">
                <td>
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  <table style="width: 100%;" border="0px"  cellspacing="0">
                    <tr>
                      <td class="subheading noborder" style="width:33%;vertical-align:bottom;border:0px">
                        CH. No:-
                        <xsl:value-of select="NewDataSet/Header/GRN"/>
                      </td>
                      <!--<td class="subheading noborder" style="text-align:center;vertical-align:bottom;border:0px" >
                        <ul style="width:100%;margin:0px;">
                          <li>
                            SAC Code: <xsl:value-of select="NewDataSet/Table/sacCode"/>
                          </li>
                          <li>
                            HSN Code:  <xsl:value-of select="NewDataSet/Table/HSNCode"/>
                          </li>
                        </ul>
                      </td>-->
                      <td class="subheading noborder" style="width:50%;text-align:right;vertical-align:bottom;border:0px">
                        Dated:-
                        <xsl:value-of select="NewDataSet/Header/ReceivingDate"/>

                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
              <tr>
                <td colspan="2">
                  <ul style="margin:0px;padding:0px;">
                    <li class="subheading">Ship From</li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/Client"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Table/SiteProject"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Table/SiteAddress"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Table/SiteAddress2"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Table/SiteCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Table/SiteZipCode"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Table/SiteState"/>
                      <xsl:text> </xsl:text>
                      <xsl:if test="NewDataSet/Header/SiteStateGSTCode != ''">
                        ( <xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>)
                      </xsl:if>
                    </li>

                    <xsl:choose >
                      <xsl:when test="NewDataSet/Table/SiteGST != ''">
                        <li>
                          GSTIN: <xsl:value-of select="NewDataSet/Table/SiteGST" />
                        </li>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:if test="$printShowClientGSTINOn and NewDataSet/Table/ClientGST != ''">
                          <li>
                            GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
                          </li>
                        </xsl:if>
                      </xsl:otherwise>
                    </xsl:choose>
                    <xsl:if test="$printShowContactNoOn">
                      <xsl:variable name="scp" select="string(NewDataSet/Table[1]/SiteContactPerson)"/>
                      <xsl:variable name="scph" select="string(NewDataSet/Table[1]/SiteContactPersonPhone)"/>
                      <xsl:if test="string-length(normalize-space($scp)) &gt; 0 or string-length(normalize-space($scph)) &gt; 0">
                        <li>
                          <span class="subheading">Contact: </span>
                          <xsl:value-of select="$scp"/>
                          <xsl:if test="string-length(normalize-space($scp)) &gt; 0 and string-length(normalize-space($scph)) &gt; 0">
                            <xsl:text> — </xsl:text>
                          </xsl:if>
                          <xsl:value-of select="$scph"/>
                        </li>
                      </xsl:if>
                    </xsl:if>
                  </ul>
                </td>
                <td>
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$shipToColSpan"/>
                  </xsl:attribute>
                  <ul style="margin:0px;padding:0px;">
                    <li class="subheading">Ship To</li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
                    </li>
                    <li>
                      <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Header/CompanyState"/>
                    </li>
                    <li>
                      Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
                      ,
                      Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>
                    </li>
                  </ul>
                </td>

              </tr>
              <tr class="detailsRow">
                <td style="text-align:center;" class="subheading">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  HSN Code: <xsl:value-of select="NewDataSet/Table/HSNCode"/> | SAC Code: <xsl:value-of select="NewDataSet/Table/sacCode"/>
                </td>
              </tr>
              <tr class="detailsRow">
                <th style="width:60px;border-bottom:solid 1px;border-top:0px;">S.No</th>
                <th style="border-bottom:solid 1px;border-top:0px;">Description</th>
                <th style="border-bottom:solid 1px;border-top:0px;width:80px;text-align:center">Qty</th>
                <th style="border-bottom:solid 1px;border-top:0px;width:80px;text-align:center;">Unit</th>
                <xsl:if test="$printShowBreakageChargesOn">
                  <th style="border-bottom:solid 1px;border-top:0px;width:72px;text-align:center;font-size:9pt;">Brkg / Charge</th>
                </xsl:if>
                <td style="border-left:0px;border-top:0px;border-bottom:solid 1px;vertical-align:top;">&#160;</td>
              </tr>
              <xsl:for-each select="NewDataSet/Table">
                <tr class="detailsRow">
                  <td style="border-top: 0px;text-align:center;">
                    <xsl:value-of select="position()" />
                  </td>
                  <td style="border-top: 0px;border-left:0px;">
                    <xsl:value-of select="Item" />
                  </td>
                  <td class="noborder" style="border-bottom:solid 1px;border-right:solid 1px;text-align:center">
                    <xsl:choose>
                      <xsl:when test="$printAddBreakageQtyInQuantityColumnOn">
                        <xsl:variable name="qn" select="number(Quantity)"/>
                        <xsl:choose>
                          <xsl:when test="string-length(normalize-space(string(Breakage))) = 0">
                            <xsl:value-of select="$qn"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$qn + number(Breakage)"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="Quantity" />
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                  <td class="noborder" style="border-bottom:solid 1px;text-align:center;border-right:1px;">
                    <xsl:value-of select="Unit" />
                  </td>
                  <xsl:if test="$printShowBreakageChargesOn">
                    <td class="noborder" style="border-bottom:solid 1px;border-right:solid 1px;text-align:center;font-size:9pt;vertical-align:middle;">
                      <xsl:choose>
                        <xsl:when test="string-length(normalize-space(string(Breakage))) = 0">0</xsl:when>
                        <xsl:otherwise>
                          <xsl:value-of select="Breakage"/>
                        </xsl:otherwise>
                      </xsl:choose>
                      <xsl:if test="string-length(normalize-space(string(BreakageRate))) &gt; 0 and number(BreakageRate) != 0">
                        <br/>
                        <xsl:value-of select="util:FormatNumber(number(Breakage) * number(BreakageRate))"/>
                      </xsl:if>
                    </td>
                  </xsl:if>
                  <xsl:if test="position() = 1">
                    <td style="border-left:solid 1px;border-top:0px;vertical-align:top;">

                      <xsl:attribute name="rowspan">
                        <xsl:value-of select="$itemCount + 1"/>
                      </xsl:attribute>
                      <div>
                        <div style="text-align: center;height:100px;padding-top:10px;">
                          <b>Vehicle No</b>
                          <br/>
                          <xsl:value-of select="vehicleNo"/>
                          <!--<br/>
                          <xsl:value-of select="VehicleRegNo"/>-->
                        </div>
                        <div style="text-align: center;height:100px;">
                          <b>Driver / Supervisor</b>
                          <br/>
                          <xsl:value-of select="driver"/>
                        </div>
                        <xsl:if test="$cfgPrintShowContactNo">
                          <div style="text-align: center;height:100px;">
                            <b>Mobile No</b>
                            <br/>
                          </div>
                        </xsl:if>

                        <xsl:if test="LRNumber != ''">
                          <div style="text-align: center;height:100px;">
                            <b>LR No</b>
                            <br/>
                            <xsl:value-of select="LRNumber"/>
                          </div>
                        </xsl:if>
                        <xsl:if test="CRNumber != ''">
                          <div style="text-align: center;height:100px;">
                            <b>CR No</b>
                            <br/>
                            <xsl:value-of select="CRNumber"/>
                          </div>
                        </xsl:if>
                        <xsl:if test="GRNumber != ''">
                          <div style="text-align: center;height:100px;">
                            <b>GR No</b>
                            <br/>
                            <xsl:value-of select="GRNumber"/>
                          </div>
                        </xsl:if>
                        <xsl:if test="$printShowTimeOn">
                          <xsl:variable name="rdStr" select="string(NewDataSet/Header/ReceivingDate)"/>
                          <!--<xsl:if test="contains($rdStr, 'T')">-->
                          <div style="text-align: center;height:50px;">
                            <b>Time</b>
                            <br/>
                          </div>
                          <!--<xsl:variable name="tPart" select="substring-after($rdStr, 'T')"/>-->
                          <!--<xsl:choose>
                              <xsl:when test="contains($tPart, '.')">
                                <xsl:value-of select="substring-before($tPart, '.')"/>
                              </xsl:when>
                              <xsl:when test="contains($tPart, '+')">
                                <xsl:value-of select="substring-before($tPart, '+')"/>
                              </xsl:when>
                              <xsl:when test="contains($tPart, 'Z')">
                                <xsl:value-of select="substring-before($tPart, 'Z')"/>
                              </xsl:when>
                              <xsl:otherwise>
                                <xsl:value-of select="$tPart"/>
                              </xsl:otherwise>
                            </xsl:choose>-->
                          <!--</xsl:if>-->
                        </xsl:if>
                        <xsl:if test="$printShowCartageOn">
                          <div style="text-align: center;height:100px;padding-top:10px;">
                            <b>Cartage</b>
                            <br/>
                            <xsl:value-of select="util:FormatNumber(freight)"/>
                          </div>
                        </xsl:if>

                      </div>
                    </td>
                  </xsl:if>
                </tr>
              </xsl:for-each>
              <tr class="spacer-row">
                <td style="border-top:0px;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$spacerDataColSpan"/>
                  </xsl:attribute>
                  &#160;
                </td>
              </tr>
              <tr class="detailsRow">
                <td colspan="2" style="text-align:right;border:solid 1px #000;padding:5px;">
                  <b>Total</b>
                </td>
                <td style="text-align:center;border:solid 1px #000;padding:5px;">
                  <b>
                    <xsl:choose>
                      <xsl:when test="$printAddBreakageQtyInQuantityColumnOn">
                        <xsl:value-of select="$sumQuantity + $sumBreakage"/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="$sumQuantity"/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </b>
                </td>
                <td style="border:solid 1px #000;padding:5px;">
                </td>
                <xsl:if test="$printShowBreakageChargesOn">
                  <td style="text-align:center;border:solid 1px #000;padding:5px;font-size:9pt;">
                    <b>
                      <xsl:value-of select="util:FormatNumber($sumBreakage)"/>
                    </b>
                  </td>
                </xsl:if>
                <td class="subheading" style="border:solid 1px #000;padding:5px;text-align:center">
                  <xsl:if test="NewDataSet/Table/ewayBillNo != ''">
                    Eway Bill No
                  </xsl:if>
                </td>
              </tr>
              <tr class="detailsRow">
                <td style="border:solid 1px #000;padding:5px;border-bottom:0px;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$qtyWordsColSpan"/>
                  </xsl:attribute>
                  Total Qty In Words:
                  <xsl:choose>
                    <xsl:when test="$printAddBreakageQtyInQuantityColumnOn">
                      <xsl:value-of select="util:QtyToWords($sumQuantity + $sumBreakage)"/>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="util:QtyToWords($sumQuantity)"/>
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
                <td style="border:solid 1px #000;padding:5px;border-bottom:0px;text-align:center">
                  <xsl:if test="NewDataSet/Table/ewayBillNo != ''">
                    <xsl:value-of select="NewDataSet/Table/ewayBillNo"/>
                  </xsl:if>
                </td>
              </tr>

              <tr>
                <td style="border-top:solid 1px;padding:0px;">
                  <xsl:attribute name="colspan">
                    <xsl:value-of select="$mainColSpan"/>
                  </xsl:attribute>
                  <table style="width:100%" cellpadding="0" cellspacing="0">
                    <colgroup>
                      <col style="width:50%"/>
                      <col style="width:20%"/>
                      <col style="width:30%"/>

                    </colgroup>
                    <tr>
                      <td  rowspan="2" class="subheading" style="width:30%;text-align:center;border-top:0px;border-left:0px;">
                        <ul style="margin:0px;">
                          <li>ON HIRE BASIS</li>
                          <li>ON RETURNABLE BASIS</li>
                          <li>NOT FOR SALE</li>
                        </ul>
                      </td>
                      <td  colspan="2"  style="border-top:0px;border-left:0px;border-right:0px" >
                        Estimated Value:    <xsl:value-of select="util:FormatNumber(NewDataSet/Table/ApproximateValue)"/>
                      </td>
                    </tr>
                    <tr>
                      <td colspan="2"   style="border-top:0px;border-left:0px;border-right:0px">
                        <xsl:if test="$printWeightSetting = 'masters'">
                          <!-- Print weight information -->
                          Estimated Weight : <xsl:value-of select="util:FormatNumber(sum(NewDataSet/Table/TotalWeight))"/>
                        </xsl:if>
                        <xsl:if test="$printWeightSetting = 'challan'">
                          <!-- Print weight information -->
                          Estimated Weight : <xsl:value-of select="util:FormatNumber(NewDataSet/Table/Weight)"/>
                        </xsl:if>
                      </td>
                    </tr>
                    <tr>
                      <td class="padding noborder" style="border-top:0px;border-left:0px;border-bottom:0px;border-right:solid 1px;vertical-align:top;" >
                        <p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
                          Terms and Conditions
                        </p>
                        <div style="font-size:8pt;">
                          <xsl:value-of select="NewDataSet/Header/Tnc" disable-output-escaping="yes"/>
                        </div>
                      </td>
                      <td class="padding noborder subheading" style="border-top:0px;border-bottom:0px;border-left:0px;border-right:solid 0px;vertical-align:bottom;">
                        <span style="font-size:14px; ">CUSTOMER SIGNATURE</span>
                      </td>
                      <td class="padding noborder subheading" style="border-top:0px;border-bottom:0px;vertical-align:top;text-align:center;border-right:0px;">
                        <table style="width:100%" border="0">
                          <tr>
                            <td class="noborder" style="text-align:center;border:0px; ">
                              For <span class="subheading">
                                <xsl:value-of select="NewDataSet/Header/Company"/>
                              </span>
                            </td>
                            <tr>
                              <td class="noborder" style="text-align:center;border:0px;">
                                <img style="height:60px;max-width:100px;">
                                  <xsl:attribute name="src">
                                    <xsl:value-of select="NewDataSet/Header/Signature"/>
                                  </xsl:attribute>
                                </img>
                              </td>
                            </tr>
                            <tr>
                              <td class="noborder" style="text-align:center;border:0px;"> AUTHORIZED SIGNAORY</td>
                            </tr>
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
