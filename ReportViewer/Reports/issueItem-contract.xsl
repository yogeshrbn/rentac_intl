<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
				<style>
					body {
					font-family: Arial, sans-serif;
					font-size: 12px;
					color: #111;
					margin: 0;
					padding: 0;
					height: 100vh;
					}

					.page {
					display: flex;
					flex-direction: column;
					justify-content: space-between;
					height: 290mm; /* A4 (297mm) - top/bottom margins (20mm each) */
					box-sizing: border-box;
					page-break-after: always;
					padding: 0;
					}

					.page:last-of-type {
					page-break-after: auto;
					}

					header {
					text-align: center;
					font-size: 20px;
					font-weight: bold;
					padding-top: 10mm;
					}

					.content {
					flex: 1;
					height: 100%;
					padding: 0;
					}

					.footer {
					text-align: center;
					font-size: 12px;
					padding: 5mm 0 0;
					margin-top: 5mm;
					}

					/* Hide footer on all pages except the last */
					/*  .page:not(:last-of-type) .footer {
					display: none;
					}
					*/
					.text-right {
					text-align: right;
					}

					/*

					*/
					.table-container {
					height: 100%;
					display: table;
					width: 100%;
					table-layout: fixed;
					border-collapse: collapse;
					}

					.table-header {
					display: table-header-group;
					}

					.table-body {
					display: table-row-group;
					height: calc(100% - 10px);
					}
					.table-body td {
					display: table-cell;
					padding: 5px;
					border: 1px solid #000;
					vertical-align: top;

					}

					.table-row {
					display: table-row;
					}

					.table-cell {
					display: table-cell;
					padding: 5px;
					border: 1px solid #000;
					vertical-align: top;
					}

					/* Spacer row to fill space */
					.spacer-row {
					height: 100%;
					}

					.spacer-row .table-cell {
					border: 1px solid #000;
					}

					.text-center {
					text-align: center;
					}

					.firstRow {
					border-top: solid 1px;
					}

					.border {
					border: solid 1px #000;
					}

					.headertable, .address-table td {
					font-size: 14px;
					}

				</style>

			</head>

			<body>
				<div class="page">
					<div id="content">
						<img style="height:60px;max-width:100px;margin-bottom:0.2in;">
							<xsl:attribute name="src">
								<xsl:value-of select="NewDataSet/Header/CompanyLogo"/>
							</xsl:attribute>
						</img>
						<xsl:variable name="catRows" select="count(NewDataSet/Table) + 10" />

						<table  border="0px"  cellspacing="0" class="table-container">
							<colgroup>
								<col style="width:50px"/>
								<col style="width:250px"/>
								<col style="width:80px"/>
								<col style="width:80px"/>
								<col style="width:80px"/>

								<col style="width:80px"/>
								<col style="width:150px"/>

							</colgroup>
							<tbody class="table-body">
								<tr >
									<td colspan="7" style="">
										<table style="width:100%;border:none;" cellpadding="0" cellspacing="0">
											<tr>
												<td style="width:33.33%;border:none;font-weight:bold;">
													GST No: <xsl:value-of select="NewDataSet/Header/CompanyGSTNo"/>
												</td>
												<td style="width:33.33%;text-align:center;border:none;font-weight:bold;"> DELIVERY CHALLAN</td>
												<td style="width:33.33%;text-align:right;border:none;font-weight:bold;">
													<xsl:choose>
														<xsl:when test="count(NewDataSet/Config/root[Key = 'diveryChallanText'])> 0">
															<xsl:value-of select="NewDataSet/Config/root[Key='diveryChallanText']/Value" />
														</xsl:when>
														<xsl:otherwise>
															RETURNABLE
														</xsl:otherwise>
													</xsl:choose>

													 
												</td>
											</tr>
										</table>

									</td>

								</tr>
								<tr  >
									<td style="text-align: center;" colspan="7" class="padding table-cell">

										<p style="font-size:18px;font-weight:bold">
											<xsl:value-of select="NewDataSet/Header/Company"/>
										</p>


										<p>
											<xsl:value-of select="NewDataSet/Header/CompanyAddress1"/>
											<br />
											<xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Header/CompanyAddress2"/>
											<xsl:text> </xsl:text>  <xsl:value-of select="NewDataSet/Header/CompanyCity"/>
											<xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Header/CompanyZipCode" />
											<xsl:text> </xsl:text> <xsl:value-of select="NewDataSet/Header/CompanyState"/>
											<br />
											Email: <xsl:value-of select="NewDataSet/Header/CompanyEmail"/>
											<br />
											Phone: <xsl:value-of select="NewDataSet/Header/CompanyPhone1"/>

										</p>
									</td>

								</tr>
								<tr>
									<td colspan="3">
										<ul style="list-style-type:none;padding-left: 0px;">
											<li style="font-size:18px;font-weight:bold">
												Customer
											</li>
											<li>
												<xsl:value-of select="NewDataSet/Header/Client"/>
												<br/>
												<xsl:value-of select="NewDataSet/Header/BillAddress1"/>
												<xsl:text> </xsl:text>
												<xsl:value-of select="NewDataSet/Header/BillAddress2"/>
												<br/>
												<xsl:value-of select="NewDataSet/Header/BillCity"/>
												<xsl:text> </xsl:text>
												<xsl:value-of select="NewDataSet/Header/BillZipCode"/>
												<xsl:text> </xsl:text>
												<xsl:value-of select="NewDataSet/Header/BillState"/>
											</li>
											<li>
												GSTIN: <xsl:value-of select="NewDataSet/Table/ClientGST" />
											</li>
										</ul>
									</td>
									<td colspan="4">
										<ul style="list-style-type:none;padding-left: 0px;">
											<li style="font-size:18px;font-weight:bold">
												Delivery Address
											</li>
											<li>
												<xsl:value-of select="NewDataSet/Header/Client"/>
												<br/>
												<xsl:value-of select="NewDataSet/Table/SiteAddress"/>
												<br/>
												<xsl:value-of select="NewDataSet/Table/SiteCity"/>
												<xsl:text> </xsl:text>
												<xsl:value-of select="NewDataSet/Table/SiteState"/>
											</li>
										</ul>
									</td>
								</tr>
								<tr>
									<td colspan="3">
										<b>State</b>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/BillState"/>

										<b>State Code</b>

										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/BillStateGSTCode"/>

									</td>
									<td colspan="4">
										<b>State</b>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/SiteState"/>

										<b>State Code</b>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/SiteStateGSTCode"/>
									</td>
								</tr>
								<tr>
									<td colspan="3">
										<b>Challan Number:</b>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/ChallanNumber"/>

									</td>
									<td colspan="3">
										<b>Challan Date:</b>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Header/StartDate"/>
									</td>
									<td rowspan="{$catRows}">

										<div style="margin-bottom:100px;">
											<b>Vehicle No</b>
											<br/>
											<xsl:value-of select="NewDataSet/Table/Vehicle"/>
											<br/>
											<xsl:value-of select="NewDataSet/Header/VehicleRegNo"/>
										</div>
										<div style="margin-bottom:100px;">
											<b>Driver</b>
											<br/>
											<xsl:value-of select="NewDataSet/Table/Driver"/>
										</div>
										<div >
											<b>Cartage</b>
											<br/>
											<xsl:value-of select="NewDataSet/Header/Freight"/>
										</div>

									</td>
								</tr>

								<tr>

									<td  class="table-cell" style="width:20px;">
										S.No
									</td>
									<td   class="table-cell">Item</td>
									<td  class="table-cell" style=" text-align:center">HSN</td>

									<td class="table-cell"  style=" text-align:center">Qty</td>
									<td  class="table-cell" style=" text-align:center">Rate</td>
									<td  class="table-cell"   style="border-right:0px;text-align:right">Amount</td>
								</tr>
								<xsl:for-each select="NewDataSet/Table">
									<tr>
										<td   class="table-cell">
											<xsl:value-of select="position()" />
										</td>
										<td  class="table-cell" style="border-top: 0px;border-left:0px;">
											<xsl:value-of select="Product" />
										</td>
										<td  class="table-cell" style="border-top: 0px;border-left:0px;text-align:center">
											<xsl:value-of select="SacHSNCode" />
										</td>
										<td  class="table-cell" style="border-top: 0px;border-left:0px;text-align:center">
											<xsl:value-of select="SentQty" />
										</td>
										<td  class="table-cell" style="border-left:0px;border-top: 0px;text-align:center">
											<xsl:value-of select="Rate" />
										</td>
										<td  class="table-cell" style="border-left:0px;border-right: 0px;border-top: 0px;text-align:right">
											<xsl:value-of select="SubTotal" />
										</td>
									</tr>
								</xsl:for-each>
								<tr class="table-row spacer-row">
									<td class="table-cell"></td>
									<td class="table-cell"></td>
									<td class="table-cell"></td>
									<td class="table-cell"></td>
									<td class="table-cell"></td>
									<td class="table-cell"></td>
								</tr>
								<tr  >
									<td colspan="3" rowspan="7" class="table-cell" style="border-right:Solid 1px;">

									</td>
									<td colspan="2"  class="table-cell" style="text-align:right;">
										Sub Total
									</td>

									<td   class="table-cell" style="text-align: right;">

										<xsl:value-of select='format-number(NewDataSet/Table/ChallanSubTotal, "#.00")'/>
									</td>
								</tr>
								<tr  >

									<td colspan="2"  class="table-cell" style="border-left:0px;text-align:right;">
										Freight
									</td>

									<td class="table-cell" style="text-align: right;">

										<xsl:value-of select='format-number(NewDataSet/Table/Freight, "#.00")'/>
									</td>
								</tr>
								<tr >

									<td colspan="2"  class="table-cell" style="border-left:0px;text-align:right;">
										Other Charges
									</td>

									<td class="table-cell" style="text-align: right;">

										<xsl:value-of select='format-number(NewDataSet/Table/OtherChargeAmount, "#.00")'/>
									</td>
								</tr>
								<tr  >

									<td colspan="2"  class="table-cell" style="border-left:0px;text-align:right;">
										IGST
									</td>

									<td style="text-align: right;"  class="table-cell">
										<xsl:value-of select='format-number(NewDataSet/Table/IGSTAmount, "#.00")'/>
									</td>
								</tr>
								<tr >

									<td colspan="2" style="border-left:0px;text-align:right;"  class="table-cell">
										CGST
									</td>

									<td style="text-align: right;" class="table-cell">

										<xsl:value-of select='format-number(NewDataSet/Table/CGSTAmount, "#.00")'/>
									</td>
								</tr>
								<tr >

									<td colspan="2" style="border-left:0px;text-align:right;"  class="table-cell">
										SGST
									</td>

									<td style="text-align: right;"  class="table-cell">
										<xsl:value-of select='format-number(NewDataSet/Table/SGSTAmount, "#.00")'/>
									</td>
								</tr>
								<tr  >

									<td colspan="2"  class="table-cell" style="border-left:0px;text-align:right;">
										Total
									</td>

									<td class="table-cell" style="text-align: right;">

										<xsl:value-of select='format-number(NewDataSet/Table/ChallanTotal, "#.00")'/>
									</td>
								</tr>

								<tr class="table-row spacer-row">
									<td colspan="7" class="table-cell" style="border-right: solid 1px;padding:0">
										<div style="display:flex;">

											<div style="padding:10px;">
												<p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
													Additional Information
												</p>
												<p>
													<xsl:value-of select="NewDataSet/Header/Remarks"  disable-output-escaping="yes"/>
												</p>

												<p style="margin-bottom:10px;font-size:16px;line-height:30px;text-decoration:underline;">
													Terms and Conditions
												</p>

												<div>
													<xsl:value-of select="NewDataSet/Header/Tnc"  disable-output-escaping="yes"/>
												</div>

											</div>
										</div>
										<div >

											<table style="width:100%;border:none;"  cellpadding="0" cellspacing="0">
												<tr>
													<td style="border:none;">
														<span style="font-size:14px; ">CUSTOMER SIGNATURE</span>

													</td>
													<td style="border:none;text-align:right;">
														<div style="display:flex;width:100%;">
															<div style=" padding-top:15px;font-size:18px;font-weight:bold;">
																For  <xsl:value-of select="NewDataSet/Header/Company"/>
															</div>
															<div style="text-align:right;width:100%;margin-top:20px;">
																<img style="height:40px;">
																	<xsl:attribute name="src">
																		<xsl:value-of select="NewDataSet/Header/Signature"/>
																	</xsl:attribute>
																</img>
															</div>
															<div style="margin-top:20px;">
																AUTHORIZED SIGNAORY
															</div>
														</div>

													</td>
												</tr>
											</table>


										</div>


									</td>
								</tr>
							</tbody>
						</table>

					</div>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:transform>
