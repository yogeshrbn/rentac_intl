

var GLOBAL_TOKEN;

app.controller('SettingsController', function ($scope, $routeParams, $http, $rootScope) {

});
app.controller('BillingSettings', function ($scope, $routeParams, $http, $rootScope, toaster) {
    $scope.editorOptions = {

        height: 200

    };
    var billSettings = [];
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'PreFix', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'Start', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'PreFixRent', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'PreFixSale', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'PreFixMeasure', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'PreFixContract', 'Value': '' });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'allowEditBillNo', 'Value': false });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'samebillprefix', 'Value': true });

    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'billNoStartContract', 'Value': 1 });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'billNoStartMeasure', 'Value': 1 });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'billNoStartSale', 'Value': 1 });
    billSettings.push({ 'Heading': 'GST Bill', 'Category': 'InvoiceGST', 'SubCategory': 'BILLNO', 'Key': 'billNoStartRent', 'Value': 1 });


    //billSettings.push({
    //    'Heading': 'GST Bill',
    //    'Category': 'InvoiceGST',
    //    'SubCategory': 'BILLNO',
    //    'Key': 'allowEditBillNo',
    //    'Value': false
    //});
    $scope.Settings = { defaultTaxRate: 0, autoRoundOffTaxable : false};
    $scope.BillSettings = billSettings;
    var config = new $.Config({});
    $scope.billSentDate = 0;
    $scope.billReceivedDate = 0;
    $scope.FreightTax = 0;
    $scope.billBreakage = 0;
    $scope.billNegativeQty = 0;
    $scope.allowEditBillNo = false;
    $scope.applyTax = true;
    $scope.applyTaxOn = 'itemlevel';
    //$scope.defaultTaxRate = 0;
    //$scope.autoRoundOffTaxable = false;
    $scope.adjustAdvance = false;
    $scope.showExcessQty = false;
    $scope.showDiscriptionColumn = false;
    $scope.showChallans = true;
    $scope.includechallansonbilltoprint = 'current';
    $scope.printDamageComponentsDetails = false;
    $scope.showDetailedDayCalculation = true;
    $scope.applyUnit2Rate = false;
    $scope.repeatCompanyDetailsOnEachPage = false;
    $scope.repeatTableHeader = false;
    $scope.invoiceTitle = 'TAX Invoice';
    $scope.billPeriodText = 'Bill Period';
    $scope.showCompanyPhone = 'yes';
    $scope.showCompanyEmail = 'yes';
    $scope.showCompanyWebsite = 'yes';
    $scope.showMSMENumber = 'yes';
    $scope.showClientMobile = 'yes';
    $scope.showClientEmail = 'yes';
    $scope.showClientPin = 'yes';
    $scope.showPONumber = 'yes';
    $scope.roundOffTotalMethod = 'none';
    $scope.defaultRoundOffTotal = false;
    $scope.allowrateEdits = 0;
    $scope.recheckbalanace = 0;
    $scope.billUptoEndDate = false;
    $scope.printBankDetails = false;
    $scope.printBalanceMaterial = false;
    $scope.hideZeroAmountItem = false;
    $scope.hideBOMComponents = false;
    $scope.groupItemsOnPrint = false;
    $scope.rentalsaccode = '997313';
    $scope.contractsaccode = '995457';
    $scope.contractbillcodetype = 'sac';

    $scope.defaultRateType = 'day'
    $scope.dayscalctype = 'fixed';
    $scope.dayscalctype_days = 30;
    $scope.BillSettings.samebillprefix = true;

    $scope.BillSettings.BillNoStartRent = 1;
    $scope.BillSettings.BillNoStartSale = 1;
    $scope.BillSettings.BillNoStartMeasure = 1;
    $scope.BillSettings.BillNoStartContract = 1;


    $scope.discount_type = 'invoicelevel';

    FormsValidation.init('frmBillConfig');
    $scope.SaveBillingSetting = function () {


        var m = $('#frmBillConfig').valid();
        if (!m) {
            return;
        }

        var data = $scope.BillSettings;
        data[0].Value = $scope.BillSettings.PreFix;
        data[1].Value = $scope.BillSettings.Start;

        //--rent
        data[2].Value = $scope.BillSettings.PreFixRent;
        //--sale
        data[3].Value = $scope.BillSettings.PreFixSale;
        //--measure
        data[4].Value = $scope.BillSettings.PreFixMeasure;
        //--contract
        data[5].Value = $scope.BillSettings.PreFixContract;
        data[6].Value = $scope.BillSettings.allowEditBillNo;
        data[7].Value = $scope.BillSettings.samebillprefix;

        data[8].Value = $scope.BillSettings.BillNoStartContract;
        data[9].Value = $scope.BillSettings.BillNoStartMeasure;
        data[10].Value = $scope.BillSettings.BillNoStartSale;
        data[11].Value = $scope.BillSettings.BillNoStartRent;

        if ($scope.BillSettings.samebillprefix == false) {
            //if ($scope.BillSettings.PreFixRent === $scope.BillSettings.PreFixSale
            //    && $scope.BillSettings.PreFixSale === $scope.BillSettings.PreFixMeasure
            //    && $scope.BillSettings.PreFixMeasure === $scope.BillSettings.PreFixContract) {
            //    alert('Prefixes must be unique');
            //    return;
            //}
            const prefixes = [
                $scope.BillSettings.PreFixRent,
                $scope.BillSettings.PreFixSale,
                $scope.BillSettings.PreFixMeasure,
                $scope.BillSettings.PreFixContract

            ];

            const hasDuplicates = prefixes.some((prefix, index) =>
                prefix && prefixes.indexOf(prefix) !== index
            );

            if (hasDuplicates) {
                alert('Prefixes must be unique');
                return;
            }

        }



        config.AddBillingSetting(data, function (e) {

            var response = e.data;
            alert(response.Message);
        });
    };

    function GetBillingConfig() {
        config.GetBillingConfig(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var billPrefix = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'prefix');
                    var billStart = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'start');
                    if (billPrefix) {
                        $scope.BillSettings.PreFix = billPrefix.Value;
                    }
                    if (billStart) {
                        $scope.BillSettings.Start = billStart.Value;
                    }
                    var billStartNoRent = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'billnostartrent');
                    var billPrefixRent = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'prefixrent');
                    if (billStartNoRent) {
                        $scope.BillSettings.BillNoStartRent = billStartNoRent.Value;
                    }
                    if (billPrefixRent) {
                        $scope.BillSettings.PreFixRent = billPrefixRent.Value;
                    }

                    var billStartNoSale = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'billnostartsale');
                    var billPrefixSale = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'prefixsale');
                    if (billStartNoSale) {
                        $scope.BillSettings.BillNoStartSale = billStartNoSale.Value;
                    }
                    if (billPrefixSale) {
                        $scope.BillSettings.PreFixSale = billPrefixSale.Value;
                    }
                    var billStartNoMeasure = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'billnostartmeasure');
                    var billPrefixMeasure = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'prefixmeasure');
                    if (billStartNoMeasure) {
                        $scope.BillSettings.BillNoStartMeasure = billStartNoMeasure.Value;
                    }
                    if (billPrefixMeasure) {
                        $scope.BillSettings.PreFixMeasure = billPrefixMeasure.Value;
                    }

                    var billStartNoContract = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'billnostartcontract');
                    var billPrefixContract = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key.toLowerCase() == 'prefixcontract');
                    if (billStartNoContract) {
                        $scope.BillSettings.BillNoStartContract = billStartNoContract.Value;
                    }
                    if (billPrefixContract) {
                        $scope.BillSettings.PreFixContract = billPrefixContract.Value;
                    }


                    var allowEditBillNo = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key == 'allowEditBillNo');
                    if (allowEditBillNo) {
                        $scope.BillSettings.allowEditBillNo = allowEditBillNo.Value == 'true';
                    }
                    var samebillprefix = response.Data.find(o => o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST' && o.Key == 'samebillprefix');
                    if (samebillprefix) {
                        $scope.BillSettings.samebillprefix = samebillprefix.Value == 'true';
                    }

                    var chargeDates = response.Data.filter(o => o.SubCategory == 'chagedates');
                    var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                    if (chargeDates) {

                        var x = chargeDates.find(o => o.Key == 'billSentDate');
                        if (x)
                            $scope.billSentDate = x.Value == 'true';

                        x = chargeDates.find(o => o.Key == 'billReceivedDate');
                        if (x)
                            $scope.billReceivedDate = x.Value == 'true';
                    }

                    if (freightTax) {
                        $scope.FreightTax = freightTax.Value
                    }


                    var brk = response.Data.find(o => o.SubCategory == 'breakageBill' && o.Key == 'breakageBill');
                    if (brk) {


                        $scope.billBreakage = brk.Value == 'true';

                    }
                    var billNegativeQty = response.Data.find(o => o.SubCategory == 'Billing' &&
                        o.Key == 'billNegativeQty');
                    if (billNegativeQty) {


                        $scope.billNegativeQty = billNegativeQty.Value == 'true';

                    }
                    var printBankDetails = response.Data.find(o => o.SubCategory == 'Billing' &&
                        o.Key == 'printBankDetails');
                    if (printBankDetails) {


                        $scope.printBankDetails = printBankDetails.Value == 'true';

                    }
                    var printBalanceMaterial = response.Data.find(o => o.SubCategory == 'Billing' &&
                        o.Key == 'printBalanceMaterial');
                    if (printBalanceMaterial) {
                        $scope.printBalanceMaterial = printBalanceMaterial.Value == 'true';
                    }
                    var tnc = response.Data.find(o => o.SubCategory == 'Other' && o.Category == 'Invoice' && o.Key == 'tnc');

                    if (tnc) {
                        $scope.tnc = tnc.Value;
                    }
                    var addInfo = response.Data.find(o => o.SubCategory == 'Other' && o.Category == 'Invoice' && o.Key == 'addInfo');

                    if (addInfo) {
                        $scope.addInfo = addInfo.Value;
                    }

                    var hideZeroAmountItem = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'hideZeroAmountItem');

                    if (hideZeroAmountItem) {
                        $scope.hideZeroAmountItem = hideZeroAmountItem.Value == 'true';
                    }
                    var hideBOMComponents = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'hidebomcomponents');

                    if (hideBOMComponents) {
                        $scope.hideBOMComponents = hideBOMComponents.Value == 'true';
                    }
                    var groupItemsOnPrint = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'groupItemsOnPrint');
                    if (groupItemsOnPrint) {
                        var gp = groupItemsOnPrint.Value;
                        $scope.groupItemsOnPrint = gp == '1' || gp == 1 || gp === true || ('' + gp).toLowerCase() === 'true';
                    }
                    var applyTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'applyTax');

                    if (applyTax) {
                        $scope.applyTax = applyTax.Value == 'true';
                    }
                    var defaultRateType = response.Data.find(o => o.Key == 'defaultRateType');

                    if (defaultRateType) {
                        $scope.defaultRateType = defaultRateType.Value;
                    }
                    var adjustAdvance = response.Data.find(o => o.Key == 'adjustAdvance');

                    if (adjustAdvance) {
                        $scope.adjustAdvance = adjustAdvance.Value == '1';
                    }
                    var showExcessQty = response.Data.find(o => o.Key == 'showExcessQty');

                    if (showExcessQty) {
                        $scope.showExcessQty = showExcessQty.Value == '1';
                    }
                    var showDiscriptionColumn = response.Data.find(o => o.Key == 'showDiscriptionColumn');

                    if (showDiscriptionColumn) {
                        $scope.showDiscriptionColumn = showDiscriptionColumn.Value == '1';
                    }
                    var showChallans = response.Data.find(o => o.Key == 'showChallans');

                    if (showChallans) {
                        $scope.showChallans = showChallans.Value == '1' || showChallans.Value == 1;
                    }
                    var includechallansonbilltoprint = response.Data.find(o => o.Key == 'includechallansonbilltoprint');
                    if (includechallansonbilltoprint && includechallansonbilltoprint.Value != null && includechallansonbilltoprint.Value !== undefined) {
                        var pdc = ('' + includechallansonbilltoprint.Value).trim();
                        if (pdc === 'all' || pdc === 'alldeliveryandcurrentreturns' || pdc === 'current') {
                            $scope.includechallansonbilltoprint = pdc;
                        }
                    }
                    var printDamageComponentsDetails = response.Data.find(o => o.Key == 'printDamageComponentsDetails');
                    if (printDamageComponentsDetails) {
                        var pd = printDamageComponentsDetails.Value;
                        $scope.printDamageComponentsDetails = pd == '1' || pd == 1 || pd === true || ('' + pd).toLowerCase() === 'true';
                    }
                    var showDetailedDayCalculation = response.Data.find(o => o.Key == 'showDetailedDayCalculation');
                    if (showDetailedDayCalculation) {
                        var v = showDetailedDayCalculation.Value;
                        $scope.showDetailedDayCalculation = v == '1' || v == 1 || v === true || ('' + v).toLowerCase() === 'true';
                    }
                    var repeatCompanyDetailsOnEachPage = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'repeatCompanyDetailsOnEachPage');
                    if (repeatCompanyDetailsOnEachPage) {
                        var rcd = repeatCompanyDetailsOnEachPage.Value;
                        $scope.repeatCompanyDetailsOnEachPage = rcd == '1' || rcd == 1 || rcd === true || ('' + rcd).toLowerCase() === 'true';
                    }
                    var repeatTableHeader = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'repeatTableHeader');
                    if (repeatTableHeader) {
                        var rth = repeatTableHeader.Value;
                        $scope.repeatTableHeader = rth == '1' || rth == 1 || rth === true || ('' + rth).toLowerCase() === 'true';
                    }
                    var applyUnit2Rate = response.Data.find(o => o.SubCategory == 'Billing' && o.Key == 'applyUnit2Rate');
                    if (applyUnit2Rate) {
                        var u2 = applyUnit2Rate.Value;
                        $scope.applyUnit2Rate = u2 == '1' || u2 == 1 || u2 === true || ('' + u2).toLowerCase() === 'true';
                    }
                    var invoiceTitle = response.Data.find(o => o.Key == 'invoiceTitle');

                    if (invoiceTitle && invoiceTitle.Value) {
                        $scope.invoiceTitle = invoiceTitle.Value;
                    }
                    var billPeriodText = response.Data.find(o => o.Key == 'billPeriodText');
                    if (billPeriodText && billPeriodText.Value != null && billPeriodText.Value !== undefined) {
                        $scope.billPeriodText = ('' + billPeriodText.Value).trim() || 'Bill Period';
                    }
                    function normalizeContactShow(v) {
                        if (!v) return null;
                        var s = ('' + v).toLowerCase();
                        if (s === 'no' || s === 'ifexists' || s === 'yes') return s;
                        return null;
                    }
                    var showCompanyPhone = response.Data.find(o => o.Key == 'showCompanyPhone');
                    if (showCompanyPhone && normalizeContactShow(showCompanyPhone.Value))
                        $scope.showCompanyPhone = normalizeContactShow(showCompanyPhone.Value);
                    var showCompanyEmail = response.Data.find(o => o.Key == 'showCompanyEmail');
                    if (showCompanyEmail && normalizeContactShow(showCompanyEmail.Value))
                        $scope.showCompanyEmail = normalizeContactShow(showCompanyEmail.Value);
                    var showCompanyWebsite = response.Data.find(o => o.Key == 'showCompanyWebsite');
                    if (showCompanyWebsite && normalizeContactShow(showCompanyWebsite.Value))
                        $scope.showCompanyWebsite = normalizeContactShow(showCompanyWebsite.Value);
                    var showMSMENumber = response.Data.find(o => o.Key == 'showMSMENumber');
                    if (showMSMENumber && normalizeContactShow(showMSMENumber.Value))
                        $scope.showMSMENumber = normalizeContactShow(showMSMENumber.Value);
                    var showClientMobile = response.Data.find(o => o.Key == 'showClientMobile');
                    if (showClientMobile && normalizeContactShow(showClientMobile.Value))
                        $scope.showClientMobile = normalizeContactShow(showClientMobile.Value);
                    var showClientEmail = response.Data.find(o => o.Key == 'showClientEmail');
                    if (showClientEmail && normalizeContactShow(showClientEmail.Value))
                        $scope.showClientEmail = normalizeContactShow(showClientEmail.Value);
                    var showClientPin = response.Data.find(o => o.Key == 'showClientPin');
                    if (showClientPin && normalizeContactShow(showClientPin.Value))
                        $scope.showClientPin = normalizeContactShow(showClientPin.Value);
                    var showPONumber = response.Data.find(o => o.Key == 'showPONumber');
                    if (showPONumber && normalizeContactShow(showPONumber.Value))
                        $scope.showPONumber = normalizeContactShow(showPONumber.Value);
                    var roundOffTotalMethod = response.Data.find(o => o.Key == 'roundOffTotalMethod');
                    if (roundOffTotalMethod && roundOffTotalMethod.Value) {
                        var rm = ('' + roundOffTotalMethod.Value).toLowerCase();
                        if (rm === 'none' || rm === 'nearest' || rm === 'up' || rm === 'down')
                            $scope.roundOffTotalMethod = rm;
                    }
                    var defaultRoundOffTotal = response.Data.find(o => o.Key == 'defaultRoundOffTotal');
                    if (defaultRoundOffTotal) {
                        $scope.defaultRoundOffTotal = defaultRoundOffTotal.Value == '1' || defaultRoundOffTotal.Value === true || defaultRoundOffTotal.Value === 'true';
                    }
                    var contractbillcodetype = response.Data.find(o => o.Key == 'contractbillcodetype');

                    if (contractbillcodetype) {
                        $scope.contractbillcodetype = contractbillcodetype.Value;
                    }

                    var dayscalctype = response.Data.find(o => o.Key == 'dayscalctype');

                    if (dayscalctype) {
                        $scope.dayscalctype = dayscalctype.Value;
                    }

                    var dayscalctype_days = response.Data.find(o => o.Key == 'dayscalctype_days');

                    if (dayscalctype_days) {
                        $scope.dayscalctype_days = dayscalctype_days.Value;
                    }


                    var allowrateEdits = response.Data.find(o => o.Key.toLowerCase() == 'allowrateedits');

                    if (allowrateEdits) {
                        $scope.allowrateEdits = allowrateEdits.Value == '1' || allowrateEdits.Value == 'true';
                    }
                    var recheckbalance = response.Data.find(o => o.Key.toLowerCase() == 'recheckbalance');

                    if (recheckbalance) {
                        $scope.recheckbalance = recheckbalance.Value == '1' || recheckbalance.Value == 'true';
                    }
                    var billUptoEndDate = response.Data.find(o => o.Key.toLowerCase() == 'billuptoenddate');
                    if (billUptoEndDate) {
                        $scope.billUptoEndDate = billUptoEndDate.Value == '1' || billUptoEndDate.Value == 'true';
                    }
                    var rentalsaccode = response.Data.find(o => o.Key.toLowerCase() == 'rentalsaccode');

                    if (rentalsaccode) {
                        $scope.rentalsaccode = rentalsaccode.Value;
                    }
                    var contractsaccode = response.Data.find(o => o.Key.toLowerCase() == 'contractsaccode');

                    if (contractsaccode) {
                        $scope.contractsaccode = contractsaccode.Value;
                    }
                    var discount_type = response.Data.find(o => o.Key == 'discount_type');
                    if (discount_type) {
                        $scope.discount_type = discount_type.Value;
                    }
                    var applyTaxOn = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'applyTaxOn');
                    if (applyTaxOn && applyTaxOn.Value) {
                        var ato = ('' + applyTaxOn.Value).toLowerCase();
                        if (ato === 'subtotal' || ato === 'itemlevel') {
                            $scope.applyTaxOn = ato;
                        }
                    }
                    var defaultTaxRate = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'defaultTaxRate');
                    if (defaultTaxRate && defaultTaxRate.Value != null && defaultTaxRate.Value !== '') {
                        $scope.Settings.defaultTaxRate = +defaultTaxRate.Value;
                    }
                    var autoRoundOffTaxable = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'autoRoundOffTaxable');
                    if (autoRoundOffTaxable) {
                        $scope.Settings.autoRoundOffTaxable = autoRoundOffTaxable.Value == '1' || autoRoundOffTaxable.Value === true || autoRoundOffTaxable.Value === 'true';
                    }
                }

            }
        });
    }
    GetBillingConfig();


    $scope.saveConfig = function () {

        var data = [];
        var billSentDate = {
            'Category': 'Invoice',
            'SubCategory': 'chagedates',
            'Key': 'billSentDate',
            'Value': $scope.billSentDate
        }
        var billReceiveDate = {
            'Category': 'Invoice',
            'SubCategory': 'chagedates',
            'Key': 'billReceivedDate',
            'Value': $scope.billReceivedDate
        }
        var breakageBill = {
            'Category': 'Invoice',
            'SubCategory': 'breakageBill',
            'Key': 'breakageBill',
            'Value': $scope.billBreakage
        }
        var freightTaxVal = +$scope.FreightTax
        var frieghtTax = {
            'Category': 'Invoice',
            'SubCategory': 'Tax',
            'Key': 'freightTax',
            'Value': freightTaxVal
        }
        var billNegativeQty = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'billNegativeQty',
            'Value': $scope.billNegativeQty
        }
        var printBankDetails = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'printBankDetails',
            'Value': $scope.printBankDetails
        }
        var printBalanceMaterial = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'printBalanceMaterial',
            'Value': $scope.printBalanceMaterial
        }
        var hideZeroAmountItem = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'hideZeroAmountItem',
            'Value': $scope.hideZeroAmountItem
        }
        var hideBOMComponents = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'hidebomcomponents',
            'Value': $scope.hideBOMComponents
        }
        var applyTax = {
            'Category': 'Invoice',
            'SubCategory': 'Tax',
            'Key': 'applyTax',
            'Value': $scope.applyTax
        }
        var applyTaxOnVal = ('' + ($scope.applyTaxOn || 'itemlevel')).toLowerCase();
        if (applyTaxOnVal !== 'subtotal' && applyTaxOnVal !== 'itemlevel') {
            applyTaxOnVal = 'itemlevel';
        }
        var applyTaxOn = {
            'Category': 'Invoice',
            'SubCategory': 'Tax',
            'Key': 'applyTaxOn',
            'Value': applyTaxOnVal
        }
        var defaultTaxRateVal = +$scope.Settings.defaultTaxRate;
        if (isNaN(defaultTaxRateVal) || defaultTaxRateVal < 0) {
            defaultTaxRateVal = 0;
        }
        var defaultTaxRate = {
            'Category': 'Invoice',
            'SubCategory': 'Tax',
            'Key': 'defaultTaxRate',
            'Value': defaultTaxRateVal
        }
        var autoRoundOffTaxable = {
            'Category': 'Invoice',
            'SubCategory': 'Tax',
            'Key': 'autoRoundOffTaxable',
            'Value': $scope.Settings.autoRoundOffTaxable == true ? 1 : 0
        }
        var defaultRateType = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'defaultRateType',
            'Value': $scope.defaultRateType
        }
        var adjustAdvance = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'adjustAdvance',
            'Value': $scope.adjustAdvance == true ? 1 : 0
        }
        var showExcessQty = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showExcessQty',
            'Value': $scope.showExcessQty == true ? 1 : 0
        }
        var showDiscriptionColumn = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showDiscriptionColumn',
            'Value': $scope.showDiscriptionColumn == true ? 1 : 0
        }
        var showChallans = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showChallans',
            'Value': $scope.showChallans == true ? 1 : 0
        }
        var includechallansonbilltoprintVal = ('' + ($scope.includechallansonbilltoprint || 'current')).trim();
        if (includechallansonbilltoprintVal !== 'all' && includechallansonbilltoprintVal !== 'alldeliveryandcurrentreturns' && includechallansonbilltoprintVal !== 'current') {
            includechallansonbilltoprintVal = 'current';
        }
        var includechallansonbilltoprintCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'includechallansonbilltoprint',
            'Value': includechallansonbilltoprintVal
        }
        var printDamageComponentsDetails = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'printDamageComponentsDetails',
            'Value': $scope.printDamageComponentsDetails == true ? 1 : 0
        }
        var showDetailedDayCalculation = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showDetailedDayCalculation',
            'Value': $scope.showDetailedDayCalculation == true ? 1 : 0
        }
        var repeatCompanyDetailsOnEachPage = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'repeatCompanyDetailsOnEachPage',
            'Value': $scope.repeatCompanyDetailsOnEachPage == true ? 1 : 0
        }
        var repeatTableHeader = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'repeatTableHeader',
            'Value': $scope.repeatTableHeader == true ? 1 : 0
        }
        var applyUnit2Rate = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'applyUnit2Rate',
            'Value': $scope.applyUnit2Rate == true ? 1 : 0
        }
        var invoiceTitle = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'invoiceTitle',
            'Value': $scope.invoiceTitle || 'TAX Invoice'
        }
        var billPeriodText = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'billPeriodText',
            'Value': ($scope.billPeriodText != null && $scope.billPeriodText !== undefined && ('' + $scope.billPeriodText).trim() !== '')
                ? ('' + $scope.billPeriodText).trim()
                : 'Bill Period'
        }
        var showCompanyPhoneCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showCompanyPhone',
            'Value': $scope.showCompanyPhone || 'yes'
        }
        var showCompanyEmailCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showCompanyEmail',
            'Value': $scope.showCompanyEmail || 'yes'
        }
        var showCompanyWebsiteCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showCompanyWebsite',
            'Value': $scope.showCompanyWebsite || 'yes'
        }
        var showMSMENumberCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showMSMENumber',
            'Value': $scope.showMSMENumber || 'yes'
        }
        var showClientMobileCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showClientMobile',
            'Value': $scope.showClientMobile || 'yes'
        }
        var showClientEmailCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showClientEmail',
            'Value': $scope.showClientEmail || 'yes'
        }
        var showClientPinCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showClientPin',
            'Value': $scope.showClientPin || 'yes'
        }
        var showPONumberCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'showPONumber',
            'Value': $scope.showPONumber || 'yes'
        }
        var roundOffTotalMethodCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'roundOffTotalMethod',
            'Value': $scope.roundOffTotalMethod || 'none'
        }
        var defaultRoundOffTotalCfg = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'defaultRoundOffTotal',
            'Value': $scope.defaultRoundOffTotal == true ? 1 : 0
        }
        var contractbillcodetype = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'contractbillcodetype',
            'Value': $scope.contractbillcodetype
        }
        var dayscalctype = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'dayscalctype',
            'Value': $scope.dayscalctype
        }
        var dayscalctype_days = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'dayscalctype_days',
            'Value': $scope.dayscalctype_days
        }
        var allowrateEdits = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'allowrateEdits',
            'Value': $scope.allowrateEdits == true ? 1 : 0
        }
        var recheckbalance = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'recheckbalance',
            'Value': $scope.recheckbalance == true ? 1 : 0
        }
        var billUptoEndDate = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'billUptoEndDate',
            'Value': $scope.billUptoEndDate == true ? 1 : 0
        }
        var discount_type = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'discount_type',
            'Value': $scope.discount_type
        }
        var contractsaccode = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'contractsaccode',
            'Value': $scope.contractsaccode
        }
        var rentalsaccode = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'rentalsaccode',
            'Value': $scope.rentalsaccode
        }
        var groupItemsOnPrint = {
            'Category': 'Invoice',
            'SubCategory': 'Billing',
            'Key': 'groupItemsOnPrint',
            'Value': $scope.groupItemsOnPrint == true ? 1 : 0
        }
        
        data.push(billSentDate);
        data.push(billReceiveDate);
        data.push(frieghtTax);
        data.push(breakageBill);
        data.push(billNegativeQty);
        data.push(printBankDetails);
        data.push(printBalanceMaterial);
        data.push(hideZeroAmountItem);
        data.push(hideBOMComponents);
        data.push(applyTax);
        data.push(applyTaxOn);
        data.push(defaultTaxRate);
        data.push(autoRoundOffTaxable);
        data.push(adjustAdvance);
        data.push(showExcessQty);
        data.push(showDiscriptionColumn);
        data.push(showChallans);
        data.push(includechallansonbilltoprintCfg);
        data.push(printDamageComponentsDetails);
        data.push(showDetailedDayCalculation);
        data.push(repeatCompanyDetailsOnEachPage);
        data.push(repeatTableHeader);
        data.push(applyUnit2Rate);
        data.push(invoiceTitle);
        data.push(billPeriodText);
        data.push(showCompanyPhoneCfg);
        data.push(showCompanyEmailCfg);
        data.push(showCompanyWebsiteCfg);
        data.push(showMSMENumberCfg);
        data.push(showClientMobileCfg);
        data.push(showClientEmailCfg);
        data.push(showClientPinCfg);
        data.push(showPONumberCfg);
        data.push(roundOffTotalMethodCfg);
        data.push(defaultRoundOffTotalCfg);
        data.push(contractbillcodetype);
        data.push(dayscalctype);
        data.push(dayscalctype_days);
        data.push(discount_type);

        data.push(allowrateEdits);
        data.push(recheckbalance);
        data.push(billUptoEndDate);
        data.push(contractsaccode);
        data.push(rentalsaccode);
        data.push(groupItemsOnPrint);


        config.SaveConfig(data, function (e) {

            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });

    }
    $scope.saveOtherConfig = function () {
        var data = [];
        var tnc = {
            'Category': 'Invoice',
            'SubCategory': 'Other',
            'Key': 'tnc',
            'Value': $scope.tnc

        }
        data.push(tnc);
        var addInfo = {
            'Category': 'Invoice',
            'SubCategory': 'Other',
            'Key': 'addInfo',
            'Value': $scope.addInfo

        }
        data.push(addInfo);
        config.SaveConfig(data, function (e) {

            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });
    }
});
app.controller('DelivheryChallanSettingsController', function ($scope, $routeParams, $http, $rootScope, toaster) {

    var challanSettings = [];
    //billSettings.push({ 'Heading': 'NON GST Bill', 'Category': 'Invoice', 'SubCategory': 'BILLNO', 'Key': 'Prefix', 'Value': '' });
    //billSettings.push({ 'Heading': 'NON GST Bill', 'Category': 'Invoice', 'SubCategory': 'BILLNO', 'Key': 'Start', 'Value': '' });
    $scope.editorOptions = {

        height: 200

    };
    challanSettings.push({
        'Heading': 'ISSUECHALLAN',
        'Category': 'ISSUECHALLAN',
        'SubCategory': 'ISSUECHALLAN',
        'Key': 'resetinterval',
        'Value': '0'
    });
    challanSettings.push({
        'Heading': 'ISSUECHALLAN',
        'Category': 'ISSUECHALLAN',
        'SubCategory': 'ISSUECHALLAN',
        'Key': 'rateType',
        'Value': '2'
    });
    challanSettings.push({
        'Heading': 'ISSUECHALLAN',
        'Category': 'ISSUECHALLAN',
        'SubCategory': 'ISSUECHALLAN',
        'Key': 'allowEditChallanNumber',
        'Value': '0'
    });
    $scope.ChallanSettings = challanSettings;
    var config = new $.Config({});
    $scope.resetInterval = 0;
    $scope.rateType = 2;
    $scope.showRateOf = 2;
    $scope.applyTax = 2;
    $scope.billReceivedDate = 0;
    $scope.freightTax = false;
    $scope.otherChargesTax = false;

    $scope.billBreakage = 0;
    $scope.allowEditChallanNumber = 0;
    $scope.enableIssuedListPreview = true;
    $scope.showItemSizeColumnOnPrint = true;
    $scope.printShowCartage = false;
    $scope.printShowTime = false;
    $scope.printShowContactNo = false;
    $scope.printShowRate = false;
    $scope.promptHeaderOriginalDuplicate = false;
    $scope.printratetype = 'sale';
    $scope.warnchallanwithoutrate = 'no';
    //--rent challan
    $scope.number = {};
    $scope.number.rentPrefix = '';
    $scope.number.rentStart = '';
    //--contract challan
    $scope.number.contractPrefix = '';
    $scope.number.contractStart = '';
    //--material transfer challan
    $scope.number.matPrefix = '';
    $scope.number.matStart = '';
    //--material adjust challan
    $scope.number.adjPrefix = '';
    $scope.number.adjStart = '';
    //--hire challan
    $scope.number.hirePrefix = '';
    $scope.number.hireStart = '';
    //--sale challan
    $scope.number.salePrefix = '';
    $scope.number.saleStart = '';
    $scope.samePrefix = true;


    FormsValidation.init('frmBillConfig');


    function GetBillingConfig() {
        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var chResetInterval = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'resetinterval');

                    if (chResetInterval) {
                        $scope.resetInterval = parseInt(chResetInterval.Value);
                    }
                    var rateType = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'rateType');

                    if (rateType) {
                        $scope.rateType = parseInt(rateType.Value);
                    }
                    var showRateOf = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'showRateOf');

                    if (showRateOf) {
                        $scope.showRateOf = parseInt(showRateOf.Value);
                    }
                    var applyTax = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'applyTax');

                    if (applyTax) {
                        $scope.applyTax = parseInt(applyTax.Value);
                    }
                    var allowEditChallanNumber = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN'
                        && o.Key == 'allowEditChallanNumber');

                    if (allowEditChallanNumber) {
                        $scope.allowEditChallanNumber = parseInt(allowEditChallanNumber.Value);
                    }
                    var enableIssuedListPreview = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'enableIssuedListPreview');
                    if (enableIssuedListPreview) {
                        var epv = enableIssuedListPreview.Value;
                        $scope.enableIssuedListPreview = epv == '1' || epv == 1 || epv === true || ('' + epv).toLowerCase() === 'true';
                    }
                    var showItemSizeColumnOnPrint = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'showItemSizeColumnOnPrint');
                    if (showItemSizeColumnOnPrint) {
                        var ssp = showItemSizeColumnOnPrint.Value;
                        $scope.showItemSizeColumnOnPrint = ssp == '1' || ssp == 1 || ssp === true || ('' + ssp).toLowerCase() === 'true';
                    }
                    var printShowCartage = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'printShowCartage');
                    if (printShowCartage) {
                        var pc = printShowCartage.Value;
                        $scope.printShowCartage = pc == '1' || pc == 1 || pc === true || ('' + pc).toLowerCase() === 'true';
                    }
                    var printShowTime = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'printShowTime');
                    if (printShowTime) {
                        var pt = printShowTime.Value;
                        $scope.printShowTime = pt == '1' || pt == 1 || pt === true || ('' + pt).toLowerCase() === 'true';
                    }
                    var printShowContactNo = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'printShowContactNo');
                    if (printShowContactNo) {
                        var pn = printShowContactNo.Value;
                        $scope.printShowContactNo = pn == '1' || pn == 1 || pn === true || ('' + pn).toLowerCase() === 'true';
                    }
                    var printShowRate = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'printShowRate');
                    if (printShowRate) {
                        var pr = printShowRate.Value;
                        $scope.printShowRate = pr == '1' || pr == 1 || pr === true || ('' + pr).toLowerCase() === 'true';
                    }
                    var promptHeaderOriginalDuplicate = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'promptHeaderOriginalDuplicate');
                    if (promptHeaderOriginalDuplicate) {
                        var ph = promptHeaderOriginalDuplicate.Value;
                        $scope.promptHeaderOriginalDuplicate = ph == '1' || ph == 1 || ph === true || ('' + ph).toLowerCase() === 'true';
                    }
                    var freightTax = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'freightTax');

                    if (freightTax) {
                        $scope.freightTax = freightTax.Value == 'true';
                    }

                    var otherChargesTax = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'otherChargesTax');

                    if (otherChargesTax) {
                        $scope.otherChargesTax = otherChargesTax.Value == 'true';
                    }
                    var tnc = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'tnc');

                    if (tnc) {
                        $scope.tnc = tnc.Value;
                    }
                    var addInfo = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'addInfo');

                    if (addInfo) {
                        $scope.addInfo = addInfo.Value;
                    }
                    var billPrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'billPrefix');
                    if (billPrefix) {
                        $scope.number.billPrefix = billPrefix.Value;
                    }
                    var start = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'start');
                    if (start) {
                        $scope.number.start = start.Value;
                    }
                    //---rent challan
                    var rentPrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'rentPrefix');
                    if (rentPrefix) {
                        $scope.number.rentPrefix = rentPrefix.Value;
                    }
                    var rentStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'rentStart');
                    if (rentStart) {
                        $scope.number.rentStart = rentStart.Value;
                    }
                    //---contract challan
                    var contractPrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'contractPrefix');
                    if (contractPrefix && contractPrefix.Value) {
                        $scope.number.contractPrefix = contractPrefix.Value;
                    }

                    var contractStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'contractStart');
                    if (contractStart && contractStart.Value) {
                        $scope.number.contractStart = contractStart.Value;
                    }
                    //---material transfer challan
                    var matPrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'matPrefix');
                    if (matPrefix) {
                        $scope.number.matPrefix = matPrefix.Value;
                    }
                    var matStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'matStart');
                    if (matStart) {
                        $scope.number.matStart = matStart.Value;
                    }
                    //---material adjust challan
                    var adjPrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'adjPrefix');
                    if (adjPrefix) {
                        $scope.number.adjPrefix = adjPrefix.Value;
                    }
                    var adjStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'adjStart');
                    if (adjStart) {
                        $scope.number.adjStart = adjStart.Value;
                    }
                    //---material adjust challan
                    var hirePrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'hirePrefix');
                    if (hirePrefix) {
                        $scope.number.hirePrefix = hirePrefix.Value;
                    }
                    var hireStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'hireStart');
                    if (hireStart) {
                        $scope.number.hireStart = hireStart.Value;
                    }
                    //---sale challan
                    var salePrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'salePrefix');
                    if (salePrefix) {
                        $scope.number.salePrefix = salePrefix.Value;
                    }
                    var saleStart = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'saleStart');
                    if (saleStart) {
                        $scope.number.saleStart = saleStart.Value;
                    }
                    var printratetype = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'printratetype');

                    if (printratetype) {
                        $scope.printratetype = printratetype.Value;
                    }
                    var warnchallanwithoutrate = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'warnchallanwithoutrate');

                    if (warnchallanwithoutrate) {
                        $scope.warnchallanwithoutrate = warnchallanwithoutrate.Value;
                    }

                    var samePrefix = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'samePrefix');

                    if (samePrefix && samePrefix.Value) {
                        $scope.samePrefix = samePrefix.Value.toLowerCase() == 'true';
                    }
                    var diveryChallanText = response.Data.find(o => o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN'
                        && o.Key == 'diveryChallanText');

                    if (diveryChallanText) {
                        $scope.diveryChallanText = diveryChallanText.Value;
                    }
                }

            }
        }, 'issuechallan');
    }
    GetBillingConfig();


    $scope.saveConfig = function () {
        debugger
        var data = [];
        var chResetInterval = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'resetinterval', 'Value': parseInt($scope.resetInterval)
        }
        var rateType = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'rateType', 'Value': parseInt($scope.rateType)
        }
        var showRateOf = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'showRateOf', 'Value': parseInt($scope.showRateOf)
        }
        var applyTax = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'applyTax', 'Value': parseInt($scope.applyTax)

        }
        var allowEditChallanNumber = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'allowEditChallanNumber', 'Value': parseInt($scope.allowEditChallanNumber)

        }
        var enableIssuedListPreviewCfg = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'enableIssuedListPreview', 'Value': $scope.enableIssuedListPreview ? 1 : 0
        }
        var freightTax = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'freightTax', 'Value': $scope.freightTax

        }
        var otherChargesTax = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'otherChargesTax', 'Value': $scope.otherChargesTax

        }
        var billPrefix = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'billPrefix', 'Value': $scope.number.billPrefix

        }
        var start = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'start', 'Value': $scope.number.start

        }
        var printratetype = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'printratetype', 'Value': $scope.printratetype

        }
        var warnchallanwithoutrate = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'warnchallanwithoutrate', 'Value': $scope.warnchallanwithoutrate

        }
        var diveryChallanText = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'diveryChallanText', 'Value': $scope.diveryChallanText

        }
        debugger
        //---rent challan
        var rentPrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'rentPrefix', 'Value': $scope.number.rentPrefix }
        var rentStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'rentStart', 'Value': $scope.number.rentStart }
        var contractPrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'contractPrefix', 'Value': $scope.number.contractPrefix }
        var contractStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'contractStart', 'Value': $scope.number.contractStart }
        var matPrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'matPrefix', 'Value': $scope.number.matPrefix }
        var matStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'matStart', 'Value': $scope.number.matStart }
        var adjPrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'adjPrefix', 'Value': $scope.number.adjPrefix }
        var adjStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'adjStart', 'Value': $scope.number.adjStart }
        var hirePrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'hirePrefix', 'Value': $scope.number.hirePrefix }
        var hireStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'hireStart', 'Value': $scope.number.hireStart }
        var salePrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'salePrefix', 'Value': $scope.number.salePrefix }
        var saleStart = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'saleStart', 'Value': $scope.number.saleStart }


        //--same prefix
        var samePrefix = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'samePrefix', 'Value': $scope.samePrefix }

        debugger
        data.push(chResetInterval);
        data.push(rateType);
        data.push(showRateOf);
        data.push(applyTax);
        data.push(allowEditChallanNumber);
        data.push(enableIssuedListPreviewCfg);
        data.push(freightTax);
        data.push(otherChargesTax);
        data.push(billPrefix);
        data.push(start);
        data.push(printratetype);
        data.push(warnchallanwithoutrate);
        data.push(diveryChallanText);
        //--rent
        data.push(rentPrefix);
        data.push(rentStart);
        data.push(contractPrefix);
        data.push(contractStart);
        // data.push(matPrefix);
        // data.push(matStart);
        data.push(adjPrefix);
        data.push(adjStart);
        data.push(hirePrefix);
        data.push(hireStart);
        data.push(salePrefix);
        data.push(saleStart);
        data.push(samePrefix);
         
        if ($scope.samePrefix == false) {
            if ($scope.number.rentPrefix && $scope.number.rentPrefix != '' && (!$scope.number.rentStart || parseInt($scope.number.rentStart, 0) < 1)) {
                alert('Please enter the rent challan start number');
                return;
            }
            if ($scope.number.contractPrefix && $scope.number.contractPrefix != '' && (!$scope.number.contractStart || parseInt($scope.number.contractStart, 0) < 1)) {
                alert('Please enter the contract challan start number');
                return;
            }
            //if ($scope.number.matPrefix && $scope.number.matPrefix != '' && (!$scope.number.matStart || parseInt($scope.number.matStart, 0) < 1)) {
            //    alert('Please enter the material transfer challan start number');
            //    return;
            //}
            if ($scope.number.adjPrefix && $scope.number.adjPrefix != '' && (!$scope.number.adjStart || parseInt($scope.number.adjStart, 0) < 1)) {
                alert('Please enter the material adjust challan start number');
                return;
            }
            if ($scope.number.hirePrefix && $scope.number.hirePrefix != '' && (!$scope.number.hireStart || parseInt($scope.number.hireStart, 0) < 1)) {
                alert('Please enter the material hire challan start number');
                return;
            }
            if ($scope.number.salePrefix && $scope.number.salePrefix != '' && (!$scope.number.saleStart || parseInt($scope.number.saleStart, 0) < 1)) {
                alert('Please enter the material sale delivery challan start number');
                return;
            }


            const prefixes = [
                $scope.number.rentPrefix,
                $scope.number.contractPrefix,
                $scope.number.adjPrefix,
                $scope.number.hirePrefix,
                $scope.number.salePrefix
            ];

            const hasDuplicates = prefixes.some((prefix, index) =>
                prefix && prefixes.indexOf(prefix) !== index
            );

            if (hasDuplicates) {
                alert('Prefixes must be unique');
                return;
            }

        }
        config.SaveConfig(data, function (e) {

            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });

    }
    $scope.saveOtherConfig = function () {
        var data = [];
        var tnc = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'tnc', 'Value': $scope.tnc }
        data.push(tnc);
        var addInfo = { 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'addInfo', 'Value': $scope.addInfo }
        data.push(addInfo);
        config.SaveConfig(data, function (e) {
            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });
    };
    $scope.savePrintConfig = function () {
        var data = [];
        var showItemSizeColumnOnPrintCfg = {
            'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'showItemSizeColumnOnPrint', 'Value': $scope.showItemSizeColumnOnPrint ? 1 : 0
        };
        data.push(showItemSizeColumnOnPrintCfg);
        data.push({ 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'printShowCartage', 'Value': $scope.printShowCartage ? 1 : 0 });
        data.push({ 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'printShowTime', 'Value': $scope.printShowTime ? 1 : 0 });
        data.push({ 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'printShowContactNo', 'Value': $scope.printShowContactNo ? 1 : 0 });
        data.push({ 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'printShowRate', 'Value': $scope.printShowRate ? 1 : 0 });
        data.push({ 'Category': 'ISSUECHALLAN', 'SubCategory': 'ISSUECHALLAN', 'Key': 'promptHeaderOriginalDuplicate', 'Value': $scope.promptHeaderOriginalDuplicate ? 1 : 0 });
        config.SaveConfig(data, function (e) {
            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });
    };
});
app.controller('ReceivingChallanSettingsController', function ($scope, $routeParams, $http, $rootScope, toaster) {
    $scope.editorOptions = {

        height: 200

    };
    var challanSettings = [];
    //billSettings.push({ 'Heading': 'NON GST Bill', 'Category': 'Invoice', 'SubCategory': 'BILLNO', 'Key': 'Prefix', 'Value': '' });
    //billSettings.push({ 'Heading': 'NON GST Bill', 'Category': 'Invoice', 'SubCategory': 'BILLNO', 'Key': 'Start', 'Value': '' });

    challanSettings.push({
        'Heading': category,
        'Category': category,
        'SubCategory': category,
        'Key': 'negativeReceiving',
        'Value': '0'
    });

    $scope.ChallanSettings = challanSettings;
    var config = new $.Config({});
    $scope.negativeReceiving = true;
    $scope.allowEditChallanNumber = false;
    $scope.enableRecvdListPreview = true;
    $scope.showAccessQuantity = false;
    $scope.disableExcess = false;
    $scope.printShowBreakageCharges = false;
    $scope.printShowContactNo = false;
    $scope.printShowCartage = false;
    $scope.printShowTime = false;
    $scope.printShowClientGSTIN = true;
    $scope.printAddBreakageQtyInQuantityColumn = false;
    $scope.promptHeaderOriginalDuplicate = false;
    $scope.returnableTextRental = "RETURNABLE";
    $scope.returnableTextSales = "RETURNABLE";
    $scope.returnableTextContract = "RETURNABLE";
    $scope.diveryChallanText = 'DELIVERY CHALLAN';

    $scope.number = {};
    $scope.number.rentPrefix = '';
    $scope.number.rentStart = '';
    //--contract challan
    $scope.number.contractPrefix = '';
    $scope.number.contractStart = '';
    //--material transfer challan
    $scope.number.matPrefix = '';
    $scope.number.matStart = '';
    //--material adjust challan
    $scope.number.adjPrefix = '';
    $scope.number.adjStart = '';
    //--hire challan
    $scope.number.hirePrefix = '';
    $scope.number.hireStart = '';
    $scope.number.hireStart = '';

    //--sale challan
    $scope.rateType = 3;
    $scope.showRateOf = 1;
    $scope.samePrefix = true;

    FormsValidation.init('frmBillConfig');

    var category = 'RECEIVINGCHALLAN';
    var subCategory = 'RECEIVINGCHALLAN';

    function GetBillingConfig() {
        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var chResetInterval = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'negativeReceiving');

                    if (chResetInterval) {
                        $scope.negativeReceiving = chResetInterval.Value == 'true';
                    }
                    var allowEditChallanNumber = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'allowEditChallanNumber');

                    if (allowEditChallanNumber) {
                        $scope.allowEditChallanNumber = allowEditChallanNumber.Value == 'true';
                    }
                    var enableRecvdListPreview = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'enableRecvdListPreview');
                    if (enableRecvdListPreview) {
                        var erp = enableRecvdListPreview.Value;
                        $scope.enableRecvdListPreview = erp == '1' || erp == 1 || erp === true || ('' + erp).toLowerCase() === 'true';
                    }
                    var showRate = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'showRate');

                    if (showRate) {
                        $scope.showRate = showRate.Value == 'true';
                    }
                    var showAccessQuantity = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'showAccessQuantity');
                    if (showAccessQuantity) {
                        var saq = showAccessQuantity.Value;
                        $scope.showAccessQuantity = saq == '1' || saq == 1 || saq === true || ('' + saq).toLowerCase() === 'true';
                    }
                    var disableExcess = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'disableExcess');
                    if (disableExcess) {
                        var dex = disableExcess.Value;
                        $scope.disableExcess = dex == '1' || dex == 1 || dex === true || ('' + dex).toLowerCase() === 'true';
                    }
                    function readBoolConfig(row) {
                        if (!row) return false;
                        var v = row.Value;
                        return v == '1' || v == 1 || v === true || ('' + v).toLowerCase() === 'true';
                    }
                    var printShowBreakageCharges = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printShowBreakageCharges');
                    if (printShowBreakageCharges) {
                        $scope.printShowBreakageCharges = readBoolConfig(printShowBreakageCharges);
                    }
                    var printShowContactNo = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printShowContactNo');
                    if (printShowContactNo) {
                        $scope.printShowContactNo = readBoolConfig(printShowContactNo);
                    }
                    var printShowCartage = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printShowCartage');
                    if (printShowCartage) {
                        $scope.printShowCartage = readBoolConfig(printShowCartage);
                    }
                    var printShowTime = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printShowTime');
                    if (printShowTime) {
                        $scope.printShowTime = readBoolConfig(printShowTime);
                    }
                    var printShowClientGSTIN = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printShowClientGSTIN');
                    if (printShowClientGSTIN) {
                        $scope.printShowClientGSTIN = readBoolConfig(printShowClientGSTIN);
                    }
                    var printAddBreakageQtyInQuantityColumn = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'printAddBreakageQtyInQuantityColumn');
                    if (printAddBreakageQtyInQuantityColumn) {
                        $scope.printAddBreakageQtyInQuantityColumn = readBoolConfig(printAddBreakageQtyInQuantityColumn);
                    }
                    var promptHeaderOriginalDuplicate = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'promptHeaderOriginalDuplicate');
                    if (promptHeaderOriginalDuplicate) {
                        $scope.promptHeaderOriginalDuplicate = readBoolConfig(promptHeaderOriginalDuplicate);
                    }
                    var tnc = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'tnc');

                    if (tnc) {
                        $scope.tnc = tnc.Value;
                    }
                    var addInfo = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'addInfo');

                    if (addInfo) {
                        $scope.addInfo = addInfo.Value;
                    }
                    // Same key, different subcategory per challan type
                    // Rental uses legacy SubCategory 'PRINT' for backward compatibility.
                    var returnableTextRental = response.Data.find(o => o.SubCategory == 'PRINT' && o.Category == category && o.Key == 'returnableText');
                    var returnableTextSales = response.Data.find(o => o.SubCategory == 'sales' && o.Category == category && o.Key == 'returnableText');
                    var returnableTextContract = response.Data.find(o => o.SubCategory == 'contract' && o.Category == category && o.Key == 'returnableText');

                    if (returnableTextRental && returnableTextRental.Value) {
                        $scope.returnableTextRental = returnableTextRental.Value;
                    }
                    if (returnableTextSales && returnableTextSales.Value) {
                        $scope.returnableTextSales = returnableTextSales.Value;
                    }
                    if (returnableTextContract && returnableTextContract.Value) {
                        $scope.returnableTextContract = returnableTextContract.Value;
                    }

                    // Backward compatibility: if sales/contract aren't configured, fall back to rental
                    if ((!returnableTextSales || !returnableTextSales.Value) && $scope.returnableTextRental) {
                        $scope.returnableTextSales = $scope.returnableTextRental;
                    }
                    if ((!returnableTextContract || !returnableTextContract.Value) && $scope.returnableTextRental) {
                        $scope.returnableTextContract = $scope.returnableTextRental;
                    }
                    var diveryChallanText = response.Data.find(o => o.SubCategory == 'PRINT' && o.Category == category && o.Key == 'hideDiveryChallanText');

                    if (diveryChallanText) {
                        $scope.diveryChallanText = diveryChallanText.Value;
                    }
                    var billPrefix = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'billPrefix');

                    if (billPrefix) {
                        $scope.number.billPrefix = billPrefix.Value;
                    }
                    var start = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'start');

                    if (start) {
                        $scope.number.start = start.Value;
                    }
                    //---rent challan
                    var rentPrefix = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'rentPrefix');
                    if (rentPrefix) {
                        $scope.number.rentPrefix = rentPrefix.Value;
                    }
                    var rentStart = response.Data.find(o => o.SubCategory == category && o.Category == category && o.Key == 'rentStart');
                    if (rentStart) {
                        $scope.number.rentStart = rentStart.Value;
                    }
                    //---contract challan
                    var contractPrefix = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'contractPrefix');
                    if (contractPrefix && contractPrefix.Value) {
                        $scope.number.contractPrefix = contractPrefix.Value;
                    }

                    var contractStart = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'contractStart');
                    if (contractStart && contractStart.Value) {
                        $scope.number.contractStart = contractStart.Value;
                    }

                    //---material adjust challan
                    var adjPrefix = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'adjPrefix');
                    if (adjPrefix) {
                        $scope.number.adjPrefix = adjPrefix.Value;
                    }
                    var adjStart = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'adjStart');
                    if (adjStart) {
                        $scope.number.adjStart = adjStart.Value;
                    }
                    //---material adjust challan
                    var hirePrefix = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'hirePrefix');
                    if (hirePrefix) {
                        $scope.number.hirePrefix = hirePrefix.Value;
                    }
                    var hireStart = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'hireStart');
                    if (hireStart) {
                        $scope.number.hireStart = hireStart.Value;
                    }
                    var samePrefix = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'samePrefix');

                    if (samePrefix && samePrefix.Value) {
                        $scope.samePrefix = samePrefix.Value == 'True' || samePrefix.Value == 'true';
                    }
                    var rateType = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'rateType');

                    if (rateType && rateType.Value) {
                        $scope.rateType = rateType.Value;
                    }

                    var showRateOf = response.Data.find(o => o.SubCategory == subCategory && o.Category == category && o.Key == 'showRateOf');

                    if (showRateOf && showRateOf.Value) {
                        $scope.showRateOf = showRateOf.Value;
                    }
                }

            }
        }, category);
    }
    GetBillingConfig();


    $scope.saveConfig = function () {

        var data = [];
        var chResetInterval = {
            'Category': category, 'SubCategory': category, 'Key': 'negativeReceiving', 'Value': $scope.negativeReceiving
        }
        data.push(chResetInterval);

        var allowEditChallanNumber = {
            'Category': category, 'SubCategory': category, 'Key': 'allowEditChallanNumber', 'Value': $scope.allowEditChallanNumber
        }

        data.push(allowEditChallanNumber);
        var enableRecvdListPreviewCfg = {
            'Category': category, 'SubCategory': category, 'Key': 'enableRecvdListPreview', 'Value': $scope.enableRecvdListPreview ? 1 : 0
        }
        data.push(enableRecvdListPreviewCfg);
        var showRate = {
            'Category': category, 'SubCategory': category, 'Key': 'showRate', 'Value': $scope.showRate
        }
        data.push(showRate);
        var showAccessQuantity = {
            'Category': category, 'SubCategory': category, 'Key': 'showAccessQuantity', 'Value': $scope.showAccessQuantity ? 1 : 0
        }
        data.push(showAccessQuantity);
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'disableExcess', 'Value': $scope.disableExcess ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printShowBreakageCharges', 'Value': $scope.printShowBreakageCharges ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printShowContactNo', 'Value': $scope.printShowContactNo ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printShowCartage', 'Value': $scope.printShowCartage ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printShowTime', 'Value': $scope.printShowTime ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printShowClientGSTIN', 'Value': $scope.printShowClientGSTIN ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'printAddBreakageQtyInQuantityColumn', 'Value': $scope.printAddBreakageQtyInQuantityColumn ? 1 : 0
        });
        data.push({
            'Category': category, 'SubCategory': category, 'Key': 'promptHeaderOriginalDuplicate', 'Value': $scope.promptHeaderOriginalDuplicate ? 1 : 0
        });
        // Store separate returnable texts per challan type using same Key with different SubCategory
        // Rental stays on SubCategory 'PRINT' (legacy).
        data.push({ 'Category': category, 'SubCategory': 'PRINT', 'Key': 'returnableText', 'Value': $scope.returnableTextRental });
        data.push({ 'Category': category, 'SubCategory': 'sales', 'Key': 'returnableText', 'Value': $scope.returnableTextSales });
        data.push({ 'Category': category, 'SubCategory': 'contract', 'Key': 'returnableText', 'Value': $scope.returnableTextContract });

        var diveryChallanText = {
            'Category': category, 'SubCategory': 'PRINT', 'Key': 'diveryChallanText', 'Value': $scope.diveryChallanText
        }
        data.push(diveryChallanText);
        var billPrefix = {
            'Category': category, 'SubCategory': category, 'Key': 'billPrefix', 'Value': $scope.number.billPrefix

        }
        var start = {
            'Category': category, 'SubCategory': category, 'Key': 'start', 'Value': $scope.number.start

        }
        var rateType = {
            'Category': category, 'SubCategory': category, 'Key': 'rateType', 'Value': parseInt($scope.rateType)
        }
        var showRateOf = {
            'Category': category, 'SubCategory': category, 'Key': 'showRateOf', 'Value': parseInt($scope.showRateOf)
        }
        data.push(billPrefix);
        data.push(start);
        data.push(rateType);
        data.push(showRateOf);

        //---rent challan
        var rentPrefix = { 'Category': category, 'SubCategory': category, 'Key': 'rentPrefix', 'Value': $scope.number.rentPrefix }
        var rentStart = { 'Category': category, 'SubCategory': category, 'Key': 'rentStart', 'Value': $scope.number.rentStart }
        var contractPrefix = { 'Category': category, 'SubCategory': category, 'Key': 'contractPrefix', 'Value': $scope.number.contractPrefix }
        var contractStart = { 'Category': category, 'SubCategory': category, 'Key': 'contractStart', 'Value': $scope.number.contractStart }
        var adjPrefix = { 'Category': category, 'SubCategory': category, 'Key': 'adjPrefix', 'Value': $scope.number.adjPrefix }
        var adjStart = { 'Category': category, 'SubCategory': category, 'Key': 'adjStart', 'Value': $scope.number.adjStart }
        var hirePrefix = { 'Category': category, 'SubCategory': category, 'Key': 'hirePrefix', 'Value': $scope.number.hirePrefix }
        var hireStart = { 'Category': category, 'SubCategory': category, 'Key': 'hireStart', 'Value': $scope.number.hireStart }
        //--same prefix
        var samePrefix = { 'Category': category, 'SubCategory': category, 'Key': 'samePrefix', 'Value': $scope.samePrefix }

        //--rent
        data.push(rentPrefix);
        data.push(rentStart);
        data.push(contractPrefix);
        data.push(contractStart);
        // data.push(matPrefix);
        // data.push(matStart);
        data.push(adjPrefix);
        data.push(adjStart);
        data.push(hirePrefix);
        data.push(hireStart);
        data.push(samePrefix);


        if ($scope.samePrefix == false) {
            if ($scope.number.rentPrefix && $scope.number.rentPrefix != '' && (!$scope.number.rentStart || parseInt($scope.number.rentStart, 0) < 1)) {
                alert('Please enter the rent challan start number');
                return;
            }
            if ($scope.number.contractPrefix && $scope.number.contractPrefix != '' && (!$scope.number.contractStart || parseInt($scope.number.contractStart, 0) < 1)) {
                alert('Please enter the contract challan start number');
                return;
            }
            //if ($scope.number.matPrefix && $scope.number.matPrefix != '' && (!$scope.number.matStart || parseInt($scope.number.matStart, 0) < 1)) {
            //    alert('Please enter the material transfer challan start number');
            //    return;
            //}
            if ($scope.number.adjPrefix && $scope.number.adjPrefix != '' && (!$scope.number.adjStart || parseInt($scope.number.adjStart, 0) < 1)) {
                alert('Please enter the material adjust challan start number');
                return;
            }
            if ($scope.number.hirePrefix && $scope.number.hirePrefix != '' && (!$scope.number.hireStart || parseInt($scope.number.hireStart, 0) < 1)) {
                alert('Please enter the material hire challan start number');
                return;
            }



            const prefixes = [
                $scope.number.rentPrefix,
                $scope.number.contractPrefix,
                $scope.number.adjPrefix,
                $scope.number.hirePrefix

            ];

            const hasDuplicates = prefixes.some((prefix, index) =>
                prefix && prefixes.indexOf(prefix) !== index
            );

            if (hasDuplicates) {
                alert('Prefixes must be unique');
                return;
            }

        }

        config.SaveConfig(data, function (e) {

            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });

    }
    $scope.saveOtherConfig = function () {
        var data = [];
        var tnc = {
            'Category': category,
            'SubCategory': category,
            'Key': 'tnc',
            'Value': $scope.tnc

        }
        data.push(tnc);
        var addInfo = {
            'Category': category,
            'SubCategory': category,
            'Key': 'addInfo',
            'Value': $scope.addInfo

        }
        data.push(addInfo);
        config.SaveConfig(data, function (e) {

            if (e.status == 200 && e.data.Code == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        });
    }
});
app.controller('ToolbarController', ['$scope', '$http', '$state', '$window', '$rootScope', 'AuthenticationService',
    'FilterService', '$mdDialog', 'tabSyncService', '$timeout',
    function ($scope, $http,
        $state, $window, $rootScope, authSerice, filterService, $mdDialog, tabSyncService, $timeout) {

        var companyDTO = new $.Company({
            CompanyId: 0
        });

        $scope.Settings = new $.Settings({
            DefaultCompanyId: 0,
            UserId: 0
        });
        var defaultCompanyId = $scope.Settings
        var token;
        $('#ddlCompany').on('change', function () {
            var compId = parseInt(this.value.split(':')[1]);
            $scope.CompanySelected(compId);
            window.setTimeout(() => {
                tabSyncService.triggerRefreshInOtherTabs();
            }, 300);
        });
        tabSyncService.init();
        function forcePurchase() {

            var div = '<div style="width:90%;height:100%"></div>';
            //debugger
            $(div).load('templ/dialogs/packageExpiredDialog.html?d=' + new Date().getTime().toString(), function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: false,
                    escapeToClose: false,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        lcd: $scope.lcd
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: PackgeExpiredPopupController
                });
            });
        }

        function getDefaultCompany() {

            GLOBAL_TOKEN = token = authSerice.getTokenInfo();
            if (token != null) {
                $('.sidebar-user-name').html(token.FullName);
                // sessionStorage x-companyId (tab-specific) overrides token.DefaultCompanyId
                var companyId = sessionStorage.getItem('x-companyId') || token.DefaultCompanyId;
                $scope.Settings.DefaultCompanyId = $rootScope.DefaultCompanyId = companyId ? parseInt(companyId, 10) : token.DefaultCompanyId;
            }
            if (token != null) {
                companyDTO.GetAll(function (e) {
                    $scope.Companies = e.data;
                });
                var companyId = sessionStorage.getItem('x-companyId') || token.DefaultCompanyId;
                $scope.Settings.DefaultCompanyId = $rootScope.DefaultCompanyId = companyId ? parseInt(companyId, 10) : token.DefaultCompanyId;
                // init sessionStorage if not set (e.g. new tab)
                if (!sessionStorage.getItem('x-companyId')) {
                    sessionStorage.setItem('x-companyId', token.DefaultCompanyId);
                }
                $rootScope.$broadcast('onCompanySelected', true);

            }
            if (token && !token.AllowSwitchCompany) {
                $('#ddlCompany').hide();
                $('#btnSaveDefaultCompany').hide();

                return
            }


        }

        // Multi-company: refresh toolbar when company changes in another tab
        $rootScope.$on('onCompanyChangedFromStorage', function () {
            getDefaultCompany();
        });

        $rootScope.$on('licensePurchased', function ($event, data) {

            token = authSerice.getTokenInfo();
            token.lcd = JSON.stringify(data);
            authSerice.setTokenInfo(token);
            setLicenseInfo();
        });

        function setLicenseInfo() {
            token = authSerice.getTokenInfo();
            var lcd = token.lcd;

            if (lcd) {
                lcd = JSON.parse(lcd);

                if (lcd.MonthlyYearly == 'monthly' && lcd.RemainingDays <= 15) {
                    $scope.showlcdMessage = true;
                } else if (lcd.MonthlyYearly == 'yearly' && lcd.RemainingDays <= 15) {
                    $scope.showlcdMessage = true;
                } else if (lcd.Demo == true) {
                    $scope.showlcdMessage = true;
                }
                if (lcd.PackageId == 0) {
                    $scope.showlcdMessage = true;
                    lcd.RemainingDays = 0;
                }
                $scope.lcd = lcd;
                if (lcd.RemainingDays <= 0) {
                    $rootScope.forceBuy = true;

                }
            }

        }
        $scope.showlcdMessage = false;
        $scope.lcd = null;
        setLicenseInfo();
        $scope.$on('forceBuy', function ($event) {

            forcePurchase();
        });


        function PackgeExpiredPopupController($scope, $mdDialog, $state, lcd) {

            $scope.buyNow = function () {
                $mdDialog.hide();
                $state.go('packages');

            }

            $scope.hideExpireDlg = function () {
                $mdDialog.hide();
                $state.go('support');
            }
        }
        $scope.$watch(function ($scope) {
            var token;
            token = $scope.getTokenInfo();
            if (token) {
                return token.DefaultCompanyId;
            }
        }, function (newVal, oldVal) {

            if (isNaN(newVal) == false) {
                getDefaultCompany();
            }

        }

        );

        $scope.getTokenInfo = function () {
            return authSerice.getTokenInfo();
        }

        $scope.CompanySelected = function (itemId) {

            $scope.Settings.DefaultCompanyId = itemId;
            $scope.Settings.SetDefaultCompany(function (e) {
                updateToken($scope.Settings.DefaultCompanyId);
                $state.reload();
            });
        }
        $scope.SetDefaultCompany = function () {
            //   alert($scope.Settings.DefaultCompanyId);
            $scope.Settings.SetDefaultCompany(function (e) {
                updateToken($scope.Settings.DefaultCompanyId);

                $state.reload();
            });
        }

        function updateToken(companyId) {
            token = token || authSerice.getTokenInfo();
            if (token) {
                token.DefaultCompanyId = companyId;
                authSerice.setTokenInfo(token);
            }
            // sessionStorage: tab-specific company for x-companyId header
            sessionStorage.setItem('x-companyId', companyId);
        }

        $scope.goBack = function () {
            $window.history.back();
        };
        $scope.OnToolClick = function (command) {

            var li = $('#toolbarCtrls li[command=' + command + ']');
            if (li) {
                if (li.hasClass('disable-ctl')) {
                    return;
                }
            }
            if (command == 'edit') {
                $('#toolbarCtrls li').addClass('disable-ctl');
                $('#toolbarCtrls li[command=undo]').removeClass('disable-ctl');
            }
            if (command == 'undo') {
                $('#toolbarCtrls li').removeClass('disable-ctl');
            }
            if (command == 'filter') {
                filterService.IssueItemFilter('');
                return;
            }
            $rootScope.$emit('onToolbarClick', command);

            //disable undo after undo last action
            if (command == 'undo') {
                $('#toolbarCtrls li').removeClass('disable-ctl');
                $('#toolbarCtrls li[command=undo]').addClass('disable-ctl');

            }
        }

        //-- tab sync service implementation
        $scope.tabInfo = {
            currentTabId: null,
            isPrimary: false,
            refreshEnabled: true
        };

        // Initialize
        $scope.init = function () {
            tabSyncService.init({
                autoRefresh: true, // We'll handle manually
                showConfirmation: false,
                debug: false
            });

            $scope.tabInfo.currentTabId = tabSyncService.getTabId();
            $scope.tabInfo.isPrimary = tabSyncService.isPrimaryTab();
        };

        // Listen for refresh events
        $scope.$on('tabSync:refreshRequired', function () {
            if ($scope.tabInfo.refreshEnabled) {
                $scope.showRefreshPrompt = false;
                $scope.$apply(); // Ensure digest cycle
            }
        });

        // Listen for new tab events
        $scope.$on('tabSync:newTabOpened', function (event, data) {
            $scope.newTabNotification = 'New tab opened: ' + data.newTabId;
            $timeout(function () {
                $scope.newTabNotification = null;
            }, 1000);
            $scope.$apply();
        });

        // Manual refresh trigger
        $scope.triggerRefreshAll = function () {
            tabSyncService.triggerRefreshInOtherTabs();
            $window.alert('Refresh signal sent to all tabs');
        };

        // Handle refresh confirmation
        $scope.confirmRefresh = function () {
            $window.location.reload();
        };

        // Cancel refresh
        $scope.cancelRefresh = function () {
            $scope.showRefreshPrompt = false;
        };

        // Toggle refresh functionality
        $scope.toggleRefresh = function () {
            $scope.tabInfo.refreshEnabled = !$scope.tabInfo.refreshEnabled;
        };

        // Clean up
        $scope.$on('$destroy', function () {
            tabSyncService.destroy();
        });

        // Initialize controller
        $scope.init();


        //end of tab sync service


    }
]);
app.controller('UserController', ['$scope', '$routeParams', '$http', '$rootScope', '$mdDialog', 'LookupService',
    function ($scope, $routeParams, $http, $rootScope, $mdDialog, LookupService) {

        if (GLOBAL_TOKEN) {
            if (GLOBAL_TOKEN.RoleId != 1) {
                $('#userList').hide();
                return;
            }
        }
        LookupService.GetAllSystemRoles(function (e) {
            $scope.Roles = e.data.Data;
        });
        var user = new $.User({});
        loadUsers();

        function loadUsers() {
            user.GetAllUsers(function (e) {
                $scope.Users = e.data;
            });
        }
        $scope.resetPwd = function (objUser) {
            event.preventDefault();

            openDialog(objUser, 'reset', 'resetpassword.html');
        }
        $scope.deactivate = function (objUser) {
            var active = objUser.Active;
            var msg = !active ? 'Are you sure to activate this user?' : 'Are you sure to de-activate this user?';
            var a = confirm(msg);
            if (a) {
                user.UserId = objUser.UserId;
                user.Active = !objUser.Active;
                user.ActivateDeActivate(function (e) {
                    if (e.status == 200 && e.data == "SUCCESS") {
                        loadUsers();
                        $scope.closeMe();
                    } else {
                        alert(e.data);
                    }
                });
            }
        }
        $scope.addEditUser = function (objUser) {
            event.preventDefault();
            openDialog(objUser, 'save', 'editUser.html?d=' + new Date().getMilliseconds());
        }

        function openDialog(user, action, url) {
            var div = '<div style="width:70%;"></div>';
            // var workOrder = $scope.WorkOrders[index];
            //   var workOrder = $scope.WorkOrders.find(o => o.Number==woNumber);
            $(div).load('templ/' + url, function () {
                var html = $(this).html();
                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        user: user,
                        action: action
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: addEditUserController
                });

            });
        }

        function addEditUserController($scope, $routeParams, $http, user, action) {

            setTimeout(() => {
                FormsValidation.init('frmUser');
            }, 500);
            $scope.TagsCompanies =
                $scope.Companies.map(function (value) {
                    return { text: value.Name, Name: value.Name, CompanyId: value.CompanyId };
                });


            $scope.User = user == undefined ? new $.User({}) : new $.User({
                UserId: user.UserId,
                FullName: user.FullName,
                UserName: user.UserName,
                Email: user.Email,
                Phone: user.Phone,
                DefaultCompanyId: user.DefaultCompanyId,
                AllowSwitchCompany: user.AllowSwitchCompany,
                Password: user.Password,
                RoleId: user.RoleId,
                Companies: []
            });
            if (user && user.Companies) {
                var _userCompanies = user.Companies.split(',');
                $.each(_userCompanies, function (v, x) {
                    var c = $scope.TagsCompanies.find(o => o.CompanyId == x);
                    $scope.User.Companies.push({ text: c.Name, CompanyId: x });
                });
            }


            $scope.closeMe = function () {
                $mdDialog.hide();
            }
            $scope.Save = function () {
                if (action == 'reset') {
                    resetPwd();
                } else if (action == 'save') {
                    addEditUser();
                }

            }
            $scope.onCompanySelected = function ($event) {

            }
            function addEditUser() {

                var m = $('#frmUser').valid();
                if (!m) {
                    alert('Please fill the form correctly');
                    return;
                }

                if (!$scope.User.Companies) {
                    alert('Please select at-least one company for the user');
                    return;
                }
                if ($scope.User.Companies.length <= 0) {
                    alert('Please select at-least one company for the user');
                    return;
                }
                var _selectedCompanies = $scope.User.Companies.map(function (val) {
                    return val.CompanyId;
                });
                $scope.User.DefaultCompanyId = _selectedCompanies[0];
                $scope.User.Companies = _selectedCompanies.flat().join();

                var model = cloneObj($scope.User);
                var user = new $.User();
                user.CreateUser(function (e) {

                    if (e.status == 200 && e.data == "SUCCESS") {
                        loadUsers();
                        $scope.closeMe();
                    } else {
                        alert(e.data);
                    }
                }, model);
            }

            function resetPwd() {

                $scope.User.ResetPassword(function (e) {
                    if (e.status == 200 && e.data == "SUCCESS") {
                        loadUsers();
                        $scope.closeMe();
                    } else {
                        alert(e.data);
                    }
                });

            }
        }


    }
]);
app.controller('RolesController', ['$scope', 'LookupService', 'UserRoleService', function ($scope, LookupService, UserRoleService) {
    $scope.client = {
        gst: '',
        pan: '',
        nogst: false
    };
    LookupService.GetAllSystemRoles(function (e) {
        $scope.Roles = e.data.Data;
    });
    $scope.RoleId = 0;
    $scope.GetRoleFunction = function (roleId) {
        $scope.SysFunctions = [];
        if (roleId == 1) {
            alert('SysAdmin role can not be configured');
            return;
        }
        $scope.RoleId = roleId;
        $scope.SysFunctions = null;
        UserRoleService.GetRoleFunctions(function (e) {
            $scope.SysFunctions = e.data.Data;
        }, {
            RoleId: roleId
        });
    }

    $scope.AddRoleFunction = function (o) {

        var data = {
            RoleId: $scope.RoleId,
            Functions: [o]
        };
        UserRoleService.AddRoleFunction(function (e) {
            if (e.data.Code == 200) {
                //   alert('Saved');
            } else {
                alert('Not saved');
            }
        }, data);
    }
}]);
app.controller('RBNClientSettingsController', function ($scope, $routeParams, $http, $rootScope, toaster) {

    //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //    e.target // newly activated tab
    //    e.relatedTarget // previous active tab
    //    if (e.target.text.trim().toLowerCase() == 'profile') {
    //        $scope.getInfo();
    //    }
    //})
    $scope.clientInfo;
    $scope.getInfo = function () {
        var user = new $.User();
        user.GetRbnClientInfo((o) => {
            if (o.status == 200) {
                $scope.clientInfo = o.data.Data;
                $scope.clientInfo.NoGst = $scope.clientInfo.NoGst == 1 ? true : false;
            }
        });
    };
    $scope.updInfo = function () {

        var m = $('#frm-rbnclientprofileSettings').valid();
        if (!m) {
            return;
        }

        var user = new $.User();
        var model = {};
        Object.assign(model, $scope.clientInfo);
        if ((model.NoGst == false || !model.NoGst) && (model.GST == null || model.GST.length < 15)) {
            toaster.pop('error', "Invalid", "Either tick I do not have GST or enter a valid GST number");
            return;
        }
        model.NoGst = model.NoGst ? 1 : 0;
        user.UpdateRbnClientInfo((o) => {
            if (o.status == 200) {
                toaster.pop('success', "Success", "Information saved.");
            }
        }, model);
        console.log(JSON.stringify($scope.clientInfo));
    };


    function loadStates() {
        stateCity.GetStates(function (e) {
            $scope.States = e.data;

        });
    }
    FormsValidation.init();
    var stateCity = new $.StateCity({});
    loadStates();
    $scope.getInfo();
});

app.controller('PrivacySettingsController', ['$scope', '$routeParams', '$http', '$rootScope', 'toaster', 'AuthenticationService',
    function ($scope, $routeParams, $http, $rootScope, toaster, authService) {

        //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        //    e.target // newly activated tab
        //    e.relatedTarget // previous active tab
        //    if (e.target.text.trim().toLowerCase() == 'profile') {
        //        $scope.getInfo();
        //    }
        //})
        var loginData = authService.getTokenInfo();
        if (loginData && loginData.ProfilePic) {
            $rootScope.profilePic = SERVER_RPT_URL + loginData.ProfilePic;
        }


        $scope.browseFile = function () {
            $('#fileProfile').click();
        };
        $scope.onFileBrowse = function (event) {
            readURL(event.currentTarget);
        };

        function readURL(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                debugger
                reader.onload = function (e) {
                    debugger
                    // $('#imgProfile').attr('src', e.target.result);
                    $scope.profilePicBlob = dataURItoBlob(e.target.result);
                    var patCanvas = document.querySelector('#snapshot');
                    if (!patCanvas) return;


                    var ctxPat = patCanvas.getContext('2d');
                    var image = new Image();

                    image.onload = function () {
                        var hRatio = patCanvas.width / image.width;
                        var vRatio = patCanvas.height / image.height;
                        var ratio = Math.min(hRatio, vRatio);
                        ctxPat.clearRect(0, 0, patCanvas.width, patCanvas.height);
                        var centerShift_x = (patCanvas.width - image.width * ratio) / 2;
                        var centerShift_y = (patCanvas.height - image.height * ratio) / 2;
                        ctxPat.drawImage(image, 0, 0, image.width, image.height,
                            centerShift_x, centerShift_y, image.width * ratio, image.height * ratio);
                    };

                    image.src = e.target.result;
                }
                debugger
                var dataUrl = reader.readAsDataURL(input.files[0]);

                // reader.readAsDataURL(input.files[0]);
                return dataUrl;
            }
        }

        $scope.clientInfo;
        $scope.getInfo = function () {
            var user = new $.User();
            user.GetRbnClientInfo((o) => {
                if (o.status == 200) {
                    $scope.clientInfo = o.data.Data;
                    $scope.clientInfo.NoGst = $scope.clientInfo.NoGst == 1 ? true : false;
                }
            });
        };
        $scope.updInfo = function () {

            var m = $('#frm-rbnclientprofileSettings').valid();
            if (!m) {
                return;
            }

            var user = new $.User();
            var model = {};
            Object.assign(model, $scope.clientInfo);
            if ((model.NoGst == false || !model.NoGst) && (model.GST == null || model.GST.length < 15)) {
                toaster.pop('error', "Invalid", "Either tick I do not have GST or enter a valid GST number");
                return;
            }
            model.NoGst = model.NoGst ? 1 : 0;
            user.UpdateRbnClientInfo((o) => {
                if (o.status == 200) {
                    toaster.pop('success', "Success", "Information saved.");
                }
            }, model);
            console.log(JSON.stringify($scope.clientInfo));
        };


        function loadStates() {
            stateCity.GetStates(function (e) {
                $scope.States = e.data;

            });
        }
        //FormsValidation.init();
        //var stateCity = new $.StateCity({});
        //loadStates();
        //$scope.getInfo();

        var _video = null,
            patData = null;
        $scope.showDemos = false;
        $scope.edgeDetection = false;
        $scope.mono = false;
        $scope.invert = false;
        $scope.profilePicBlob = null;
        $scope.patOpts = {
            x: 0,
            y: 0,
            w: 25,
            h: 25
        };


        $scope.channel = {
            // the fields below are all optional
            videoHeight: 200,
            videoWidth: 180,
            video: null // Will reference the video element on success
        };
        $scope.webcamError = false;
        $scope.onError = function (err) {
            $scope.$apply(
                function () {
                    $scope.webcamError = err;
                }
            );
        };

        $scope.onSuccess = function () {

            // The video element contains the captured camera data
            _video = $scope.channel.video;
            $scope.$apply(function () {
                $scope.patOpts.w = _video.width;
                $scope.patOpts.h = _video.height;
                $scope.showDemos = true;
            });
        };

        $scope.onStream = function (stream) {
            // You could do something manually with the stream.
        };


        /**
         * Make a snapshot of the camera data and show it in another canvas.
         */
        $scope.makeSnapshot = function makeSnapshot() {
            debugger
            if (_video) {
                var patCanvas = document.querySelector('#snapshot');
                if (!patCanvas) return;

                patCanvas.width = _video.width;
                patCanvas.height = _video.height;
                var ctxPat = patCanvas.getContext('2d');

                var idata = getVideoData($scope.patOpts.x, $scope.patOpts.y, $scope.patOpts.w, $scope.patOpts.h);
                ctxPat.putImageData(idata, 0, 0);
                $scope.profilePicBlob = dataURItoBlob(patCanvas.toDataURL());
                //   sendSnapshotToServer(patCanvas.toDataURL());

                patData = idata;
            }
        };
        var getVideoData = function getVideoData(x, y, w, h) {
            var hiddenCanvas = document.createElement('canvas');
            hiddenCanvas.width = _video.width;
            hiddenCanvas.height = _video.height;
            var ctx = hiddenCanvas.getContext('2d');
            ctx.drawImage(_video, 0, 0, _video.width, _video.height);
            return ctx.getImageData(x, y, w, h);
        };

        $scope.changeProfilePic = function () {

            var user = new $.User();
            var files = [];
            files.push($scope.profilePicBlob);
            user.changeProfilePic(function (o) {
                if (o.status == 200) {
                    $rootScope.$emit('onProfilePicChange', URL.createObjectURL($scope.profilePicBlob));
                    $scope.profilePic = URL.createObjectURL($scope.profilePicBlob);
                    $('#btnCancelProfileDialog').click();
                    $scope.showCamera = false;
                }

            }, files);
        }
        $scope.showCamera = false;
        $scope.openFileDialog = function () {
            $scope.showCamera = true;
        }
        $scope.closePicDialog = function () {
            $scope.showCamera = false;
        }
    }
]);

app.controller('ChnagePasswordController', ['$scope', '$routeParams', '$http', '$rootScope', 'toaster', 'AuthenticationService', 'UserService',
    function ($scope, $routeParams, $http, $rootScope, toaster, authService, userService) {

        $scope.pwd = {
            CurrentPassword: '',
            NewPassword: '',
            ConfirmPassword: ''
        };
        $scope.changePassword = function () {
            debugger
            var m = $('#frmChangePwd').valid();
            if (!m) {
                return;
            }
            if ($scope.pwd.NewPassword != $scope.pwd.ConfirmPassword) {
                alert('New password and confirm password must be same');
                return;
            }
            userService.ChangePassword(function (e) {
                if (e.status == 200) {
                    if (e.data.Code == 200) {
                        alert('Password changed successfully.');
                    } else {
                        alert(e.data.Message);
                    }
                } else {
                    alert(e.errorMessage);
                }
            }, $scope.pwd);
        }

        FormsValidation.init('frmChangePwd');

    }
]);
app.controller('GSTSettingsController', ['$scope', '$routeParams', '$http', '$rootScope', 'toaster', 'AuthenticationService',
    function ($scope, $routeParams, $http, $rootScope, toaster, authService) {

        $scope.GSTNo = '';
        $scope.Comp = {
            hasGST: 0,
            GSTNo: '',
            LegalName: '',
            TradeName: '',
            GSTStatus: '',
            GSTRegistrationDate: null,
            gstUpdated: 0,
            GSTStatus: ''
        };
        var loginData = authService.getTokenInfo();

        function getInfo() {

            var company = new $.Company();
            company.Props.CompanyId = loginData.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status != 200) {
                    return;
                }
                if (!e.data) {
                    return;
                }

                $scope.Comp.LegalName = e.data.LegalName;
                $scope.Comp.TradeName = e.data.TradeName;
                $scope.Comp.GSTRegistrationDate = e.data.GSTRegistrationDate;
                $scope.Comp.GSTNo = e.data.GSTNo;
                $scope.GSTNo = e.data.GSTNo;
                $scope.Comp.GSTStatus = e.data.GSTStatus;

                if (e.data.GSTRegistrationDate) {
                    $scope.Comp.gstUpdated = 1;
                    $scope.Comp.hasGST = 1;
                }
            });
        }
        getInfo();
        $scope.GetTaxPayerDetails = function () {
            var company = new $.Company();

            company.GetTaxPayerDetails(function (e) {
                var data = e.data;
                if (data.Code == 200) {
                    $scope.Comp = data.Data;
                    $scope.Comp.gstUpdated = 0;
                    $scope.Comp.hasGST = 1;
                } else {
                    alert(data.Message);
                }

            }, $scope.GSTNo);
        }
        $scope.UpdateGSTDetails = function () {
            var company = new $.Company();
            if ($scope.Comp.GSTStatus != 'Active') {
                alert('GST Status is not active');
                return;
            }
            var model = cloneObj($scope.Comp);
            model.GSTRegistrationDate = formatdate(model.GSTRegistrationDate);
            company.UpdateGSTDetails(function (e) {
                var data = e.data;
                if (data.Code == 200) {

                    $scope.Comp.gstUpdated = 1;
                    $scope.Comp.hasGST = 1;
                } else {
                    alert(data.Message);
                }

            }, model);
        }


    }
]);


app.controller('EInvoiceSettingsController', ['$scope', '$routeParams', '$http', '$rootScope', 'toaster', 'AuthenticationService', 'ModalFactory',
    function ($scope, $routeParams, $http, $rootScope, toaster, authService, modalFactory) {
        $scope.EInvoiceStartDate = convertDate(new Date());
        $scope.IRP = {
            IRPUserName: '',
            IRPPassword: ''
        };
        var loginData = authService.getTokenInfo();
        $scope.Comp = {
            EInvoiceEnabled: false
        };
        if (loginData != null) {
            $scope.MinDate = loginData.FinYearStart;
            var _d = new Date(loginData.FinYearEnd);

            var maxDate = _d.setDate(_d.getDate() + 1);
            $scope.MaxDate = formatForPicker(new Date(maxDate));
            // convertDate(token.FinYearEnd);
        }

        $scope.getInfo = function () {

            var company = new $.Company();
            company.Props.CompanyId = loginData.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status != 200) {
                    return;
                }
                if (!e.data) {
                    return;
                }
                $scope.Comp = e.data;

            });
        }
        $scope.getInfo();

        $scope.UpdateEInvoicing = function () {

            var company = new $.Company();
            var model = {
                EInvoiceStartDate: formatdate($scope.EInvoiceStartDate)
            };
            company.UpdateEInvoicing(function (e) {
                var data = e.data;
                if (data.Code == 200) {

                    alert('e-Invoicing enabled successfully.');
                    $scope.closeDialog();
                } else {
                    alert(data.Message);
                }

            }, model);
        }
        $scope.SaveAndValidateIRPUser = function () {

            var company = new $.Company();
            var model = $scope.IRP;
            company.SaveAndValidateIRPUser(function (e) {
                var data = e.data;
                if (data.Code == 200) {

                    alert('IRP Crentials Saved and Validated Successfully.');
                    $scope.closeIRPDialog();
                    $scope.getInfo();
                } else {
                    alert(data.Message);
                }

            }, model);
        }

        $scope.closeIRPDialog = function () {
            $('#irpModalDialog').modal('hide');
        }
        $scope.closeDialog = function () {
            $('#invoiceBox').modal('hide');
        }
    }
]);

app.controller('EwayBillSettingsController', ['$scope', '$routeParams', '$http', '$rootScope', 'toaster', 'AuthenticationService', 'ModalFactory',
    function ($scope, $routeParams, $http, $rootScope, toaster, authService, modalFactory) {
        $scope.EInvoiceStartDate = convertDate(new Date());
        $scope.Eway = {
            EwayUserName: '',
            EwayPassword: ''
        };
        $scope.EwaySettings = {
            printChallanWithEwabill: false
        };
        var config = new $.Config();
        var loginData = authService.getTokenInfo();
        $scope.Comp = {
            EInvoiceEnabled: false
        };
        if (loginData != null) {
            $scope.MinDate = loginData.FinYearStart;
            var _d = new Date(loginData.FinYearEnd);

            var maxDate = _d.setDate(_d.getDate() + 1);
            $scope.MaxDate = formatForPicker(new Date(maxDate));
            // convertDate(token.FinYearEnd);
        }

        $scope.getCompInfo = function () {

            var company = new $.Company();
            company.Props.CompanyId = loginData.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status != 200) {
                    return;
                }
                if (!e.data) {
                    return;
                }
                $scope.Comp = e.data;

            });
        }
        $scope.getCompInfo();

        $scope.saveEwaySettings = function () {
            var model = [{
                Category: 'ewaybill', SubCategory: 'print', Key: 'printChallanWithEwabill',
                value: $scope.EwaySettings.printChallanWithEwabill ? 'true' : 'false'
            }];

            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }

        config.GetByCategory(function (e) {
            var response = e.data;
            if (response.Data != null && response.Data && response.Data.length > 0) {
                var printChallanWithEwabill = response.Data.find(o => o.SubCategory == 'print' && o.Key == 'printChallanWithEwabill');
                if (printChallanWithEwabill) {
                    $scope.EwaySettings.printChallanWithEwabill = printChallanWithEwabill.Value == 'true';
                }
            }
        }, 'ewaybill');


        $scope.UpdateEwayCreds = function () {

            var company = new $.Company();
            var model = $scope.Eway;
            company.UpdateEwayCreds(function (e) {
                var data = e.data;
                if (data.Code == 200) {

                    alert('E-way bill Crentials Saved and Validated Successfully.');
                    $scope.closeIRPDialog();
                    $scope.getCompInfo();
                } else {
                    alert(data.Message);
                }

            }, model);
        }

        $scope.closeIRPDialog = function () {
            $('#ewayDialog').modal('hide');
        }
        $scope.closeDialog = function () {
            $('#ewayDialog').modal('hide');
        }
    }
]);
app.controller('TemplatesController', function ($state) {
    $state.go('templates.issuechallan');
});
app.controller('TemplateListingController',
    function ($scope, $state, $q) {
        var apiService = new $.ApiCaller({
            http: HTTP_SERVICE
        });
        // Default group comes from route data, but for issue challan we may switch
        // group based on selected mode (rental/contract/sales).
        $scope.GroupName = $state.current.data.group;
        // For issue challan templates, allow separate default templates per mode.
        // Rental keeps legacy 'templates' subcategory.
        $scope.SubCategory = 'templates'
        $scope.DefaultTemplateId = 0
        $scope.Config = [];
        $scope.Templates = [];
        var config = new $.Config();
        $scope.issueChallanMode = $scope.issueChallanMode || 'rental';

        function mapSubCategory(mode) {
            // Keep subcategory consistent so TemplateService can resolve pdftemplate config.
            // We separate templates by Category/GroupName (ISSUECHALLAN vs CONTRACTDELIVERYCHALLAN vs SALESDELIVERYCHALLAN, etc.).
            return 'templates';
        }

        function mapGroupName(mode) {
            // Default group comes from route data. For some pages we switch group by mode.
            var baseGroup = $state.current.data.group;
            if (baseGroup == 'ISSUECHALLAN') {
                if (mode == 'contract') return 'CONTRACTDELIVERYCHALLAN';
                if (mode == 'sales') return 'SALESDELIVERYCHALLAN';
                return baseGroup;
            }
            if (baseGroup == 'RETURNS') {
                if (mode == 'contract') return 'CONTRACTRETURNS';
                if (mode == 'sales') return 'SALESRETURNS';
                return baseGroup;
            }
            return baseGroup;
        }

        $scope.onIssueChallanModeChange = function (mode) {
            $scope.issueChallanMode = mode || 'rental';
            $scope.SubCategory = mapSubCategory($scope.issueChallanMode);
            $scope.GroupName = mapGroupName($scope.issueChallanMode);
            $scope.getListing();
        }

        $scope.getListing = function () {

            var template = new $.Template();

            var templatesRequest = apiService.prepareGet(template.GETBY_GROUP + '/' + $scope.GroupName);
            var configRequest = apiService.prepareGet(config.GET_BY_CATEGORY + '/' + $scope.GroupName + '/' + $scope.SubCategory);

            apiService.Options.http(templatesRequest).then(function (res) {

                $scope.Templates = res.data.Data;
                return apiService.Options.http(configRequest)
            }).then(function (res) {
                $scope.Config = res.data.Data;
                $scope.setDefault();
            });

        }
        $scope.setDefault = function () {

            if ($scope.Config.length > 0 && $scope.Config != null) {
                var config = $scope.Config.find(o => o.Key == 'pdftemplate');
                if (config) {
                    $scope.DefaultTemplateId = parseInt(config.Value);
                }
                return;
            }
            $scope.DefaultTemplateId = $scope.Templates.find(o => o.IsDefault == true).TemplateId;
        }
        // Initial load defaults to rental mode
        $scope.SubCategory = mapSubCategory($scope.issueChallanMode);
        $scope.GroupName = mapGroupName($scope.issueChallanMode);
        $scope.getListing();


        $scope.onUseThisClick = function (templateId) {
            var model = [{ Category: $scope.GroupName, SubCategory: $scope.SubCategory, Key: 'pdftemplate', value: templateId }];
            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                if (e.data.Data == true) {
                    $scope.DefaultTemplateId = templateId;
                }
            });

        };

        $scope.templatePreview = {};
        $scope.openTemplatePreview = function (item) {
            if (!item) return;
            $scope.templatePreview = angular.copy(item);
            $('#templateGalleryPreviewModal').modal('show');
        };
        $scope.openIssueChallanPreview = $scope.openTemplatePreview;
    });

app.controller('QuotationSettingsController', ['$scope', '$routeParams', '$http', '$rootScope', '$state', 'toaster', 'AuthenticationService',
    function ($scope, $routeParams, $http, $rootScope, $state, toaster, authService) {

        var config = new $.Config();
        var configCategory = ($state.current.data && $state.current.data.settingsConfigCategory)
            ? $state.current.data.settingsConfigCategory
            : ($state.current.name === 'settings.pi' ? 'pi' : 'quotation');
        var mainSub = configCategory === 'pi' ? 'pi' : 'quotation';

        $scope.editorOptions = {

            height: 200

        };
        $scope.attachRateSheet = 2;
        $scope.contractLineTotalMode = 'quantity';

        $scope.saveSettings = function () {
            var model = [{
                Category: configCategory, SubCategory: mainSub, Key: 'prefix', value: $scope.prefix
            }];
            model.push({
                Category: configCategory, SubCategory: mainSub, Key: 'numberstart', value: $scope.numberstart
            });
            model.push({
                Category: configCategory, SubCategory: mainSub, Key: 'editnumber', value: $scope.editnumber
            });
            model.push({
                Category: configCategory, SubCategory: mainSub, Key: 'outwardfreight', value: $scope.outwardfreight
            });
            model.push({
                Category: configCategory, SubCategory: mainSub, Key: 'inwardfreight', value: $scope.inwardfreight
            });
            model.push({
                Category: configCategory, SubCategory: mainSub, Key: 'attachRateSheet', value: $scope.attachRateSheet
            });
            model.push({
                Category: configCategory, SubCategory: 'contract', Key: 'contractLineTotalMode', value: $scope.contractLineTotalMode || 'quantity'
            });
            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }
        $scope.save = function () {
            var model = [{
                Category: configCategory, SubCategory: 'rent', Key: 'addInfo', value: $scope.quotation
            }];
            model.push({
                Category: configCategory, SubCategory: 'rent', Key: 'tnc', value: $scope.tnc
            });

            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }
        $scope.saveSaleInfo = function () {
            var model = [{
                Category: configCategory, SubCategory: 'sale', Key: 'addInfo', value: $scope.saleAddInfo
            }];
            model.push({
                Category: configCategory, SubCategory: 'sale', Key: 'tnc', value: $scope.saletnc
            });

            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }
        $scope.saveContractInfo = function () {
            var model = [{
                Category: configCategory, SubCategory: 'contract', Key: 'addInfo', value: $scope.contractAddInfo
            }];
            model.push({
                Category: configCategory, SubCategory: 'contract', Key: 'tnc', value: $scope.contracttnc
            });
            model.push({
                Category: configCategory, SubCategory: 'contract', Key: 'contractLineTotalMode', value: $scope.contractLineTotalMode || 'quantity'
            });

            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }
        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var quotation = response.Data.find(o => o.SubCategory == 'rent'
                        && o.Key == 'addInfo');

                    if (quotation) {
                        $scope.quotation = quotation.Value;
                    }
                    var qtnc = response.Data.find(o => o.SubCategory == 'rent' && o.Key == 'tnc');

                    if (qtnc) {
                        $scope.tnc = qtnc.Value;

                    }
                    var attachRateSheet = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'attachRateSheet');

                    if (attachRateSheet) {
                        $scope.attachRateSheet = attachRateSheet.Value == 'true';
                    }
                    var prefix = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'prefix');

                    if (prefix) {
                        $scope.prefix = prefix.Value;
                    }
                    var numberstart = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'numberstart');

                    if (numberstart) {
                        $scope.numberstart = numberstart.Value;
                    }
                    var editnumber = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'editnumber');

                    if (editnumber) {
                        $scope.editnumber = editnumber.Value == 'true';
                    }
                    var lineModeDefault = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'contractLineTotalMode');
                    if (lineModeDefault && lineModeDefault.Value) {
                        $scope.contractLineTotalMode = lineModeDefault.Value === 'area' ? 'area' : 'quantity';
                    }
                    //var outwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'outwardfreight');

                    //if (outwardfreight) {
                    //    $scope.outwardfreight = outwardfreight.Value == 'true';
                    //}
                    //var inwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'inwardfreight');

                    //if (inwardfreight) {
                    //    $scope.inwardfreight = inwardfreight.Value == 'true';
                    //}
                    var quotation = response.Data.find(o => o.SubCategory == 'sale'
                        && o.Key == 'addInfo');

                    if (quotation) {
                        $scope.saleAddInfo = quotation.Value;
                    }
                    var qtnc = response.Data.find(o => o.SubCategory == 'sale' && o.Key == 'tnc');

                    if (qtnc) {
                        $scope.saletnc = qtnc.Value;

                    }
                    quotation = response.Data.find(o => o.SubCategory == 'contract'
                        && o.Key == 'addInfo');

                    if (quotation) {
                        $scope.contractAddInfo = quotation.Value;
                    }
                    qtnc = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'tnc');

                    if (qtnc) {
                        $scope.contracttnc = qtnc.Value;

                    }
                    var lineMode = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'contractLineTotalMode');
                    if (lineMode && lineMode.Value) {
                        $scope.contractLineTotalMode = lineMode.Value === 'area' ? 'area' : 'quantity';
                    }
                }

            }
        }, configCategory);
    }
]);
app.controller('EmailSettingsController',
    function ($scope, $routeParams, $http, $rootScope, toaster, $crypto) {

        FormsValidation.init('frmEmailSetup');
        $scope.Setup = { Server: '', Port: '', UserName: '', Password: '', FromEmail: '' };
        var config = new $.Config();

        $scope.save = function () {
            var m = $('#frmEmailSetup').valid();
            if (!m) {
                return;
            }
            var model = [{
                Category: 'email', SubCategory: 'setup', Key: 'server', value: $scope.Setup.Server
            }];
            model.push({
                Category: 'email', SubCategory: 'setup', Key: 'port', value: $scope.Setup.Port
            });
            model.push({
                Category: 'email', SubCategory: 'setup', Key: 'username', value: $scope.Setup.UserName
            });
            model.push({
                Category: 'email', SubCategory: 'setup', Key: 'password', value: $crypto.encrypt($scope.Setup.Password)
            });
            model.push({
                Category: 'email', SubCategory: 'setup', Key: 'from_email', value: $scope.Setup.FromEmail
            });
            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }

        config.GetByCategory(function (e) {
            debugger
            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var server = response.Data.find(o => o.SubCategory == 'setup' && o.Category == 'email' && o.Key == 'server');
                    if (server) {
                        $scope.Setup.Server = server.Value;
                    }
                    var port = response.Data.find(o => o.SubCategory == 'setup' && o.Category == 'email' && o.Key == 'port');
                    if (port) {
                        $scope.Setup.Port = port.Value;
                    }
                    var username = response.Data.find(o => o.SubCategory == 'setup' && o.Category == 'email' && o.Key == 'username');
                    if (username) {
                        $scope.Setup.UserName = username.Value;
                    }
                    var password = response.Data.find(o => o.SubCategory == 'setup' && o.Category == 'email' && o.Key == 'password');
                    if (password) {
                        $scope.Setup.Password = $crypto.decrypt(password.Value);
                    }
                    var from_email = response.Data.find(o => o.SubCategory == 'setup' && o.Category == 'email' && o.Key == 'from_email');
                    if (from_email) {
                        $scope.Setup.FromEmail = from_email.Value;
                    }

                }

            }
        }, 'email');
    }
);
app.controller('ShareController',
    function ($scope, $routeParams, $http, $rootScope, toaster, $crypto, $mdDialog, emailMessage) {


        $scope.MailMessage = emailMessage;
        if (!emailMessage.Type) {
            $scope.MailMessage.Type = 'email';
        }
        $scope.editorOptions = {

            height: 200

        };
        $scope.sendNow = function () {

            var m = $('#frmSendEmailForm').valid();
            if (!m) {
                return;
            }
            var notification = new $.Notification();
            var model = cloneObj($scope.MailMessage);
            if ($scope.MailMessage.Type == 'email') {
                //if (model.Body == '' || model.Body == null) {
                //    alert('Please enter the email body');
                //    return;
                //}
                //if (model.Body.length > 8000) {
                //    alert('Email body is too long.');
                //    return;
                //}
            } else {
                if (model.phoneNo.length < 10) {
                    alert('Please enter a valid phone number');
                    return;
                }

                model.Receipients = model.phoneNo;
            }
            notification.Notify(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                }
                else {
                    alert('Message sent');
                    $mdDialog.hide();
                }
            }, model);

        }



        $scope.closeDialog = function () {
            $mdDialog.hide();
        }
        window.setTimeout(() => {
            FormsValidation.init('frmSendEmailForm');
        }, 100);

    }
);
app.controller('IntegrationController', function ($scope) {



});
app.controller('WhatsappSettingsController', function ($scope) {



    $scope.NewApp = { Name: '' };
    $scope.Apps = [];

    $scope.ListAllApps = function () {

        var settings = new $.Settings();
        settings.ListAllWhatsApps(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Apps = e.data.Data;
        });
    }
    $scope.createApp = function () {

        var settings = new $.Settings();
        settings.CreateWhatsApp(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            alert('Whatsapp App Created successfully.');
        }, $scope.NewApp);
    }

    $scope.activateCallback = function (app) {

        var settings = new $.Settings();
        settings.ActivateForCallback(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

            if (!app.Embed_Link) {
                $scope.GenerateSignupLink(app);
            }
            else {
                alert('Notification setup successfully.');
                $scope.ListAllApps();
            }
        }, app);
    }
    $scope.GenerateSignupLink = function (app) {

        var settings = new $.Settings();
        settings.GenerateSignupLink(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

            alert('Success.');
            $scope.ListAllApps();
        }, app);
    }
    $scope.RefreshAppDetails = function (app) {

        var settings = new $.Settings();
        settings.RefreshDetails(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

            alert('Success.');
            $scope.ListAllApps();
        }, app);
    }
    $scope.ListAllApps();
});
app.controller('GeneralSettingsController',
    function ($scope) {

        $scope.SignaturePrint = { bills: true, challans: true, quotations: true };
        $scope.PrintQrCode = { bills: true, quotations: true };

        $scope.Config = {
            allowNegativeSale: 1, partycodegenmode: 'manual', productcodegenmode: 'manual',
            productcodegenprefix: '', productcodegenstart: 1, partycodegenprefix: '', partycodegenstart: 1,
            decimalplaces: 2, printsacorhsnonchallans: 'hsn', printWeightOnChallan: 'challan',
            showNextChallanInvoiceNumbers: false, notifyPartyOnDispatchInwardConfirm: false,
            testPhoneMobile: '', testEmail: '', mode: 'Test',
            warnIfClientNameExists: false
        };
        var config = new $.Config();

        $scope.save = function () {

            var selected = $.map($scope.SignaturePrint, function (x, y) {
                return x ? y : null;
            });
            var selectedQrPrint = $.map($scope.PrintQrCode, function (x, y) {
                return x ? y : null;
            });

            var model = [{
                Category: 'general', SubCategory: 'inventory', Key: 'allowNegativeSale',
                value: $scope.Config.allowNegativeSale
            }];
            var d = {
                Category: 'general', SubCategory: 'clients', Key: 'hidQuotationClients',
                value: $scope.Config.hidQuotationClients
            };
            model.push(d);
            var warnIfClientNameExists = {
                Category: 'general', SubCategory: 'clients', Key: 'warnIfClientNameExists',
                value: $scope.Config.warnIfClientNameExists ? 'true' : 'false'
            };
            model.push(warnIfClientNameExists);
            var signature = {
                Category: 'general', SubCategory: 'print', Key: 'signature',
                value: selected ? selected.join() : ''
            };
            var qrCode = {
                Category: 'general', SubCategory: 'print', Key: 'qrcode',
                value: selectedQrPrint ? selectedQrPrint.join() : ''
            };
            model.push(signature);
            model.push(qrCode);
            var productcodegenmode = {
                Category: 'general', SubCategory: 'masters', Key: 'productcodegenmode',
                value: $scope.Config.productcodegenmode
            };
            var productcodegenprefix = {
                Category: 'general', SubCategory: 'masters', Key: 'productcodegenprefix',
                value: $scope.Config.productcodegenprefix
            };
            var productcodegenstart = {
                Category: 'general', SubCategory: 'masters', Key: 'productcodegenstart',
                value: $scope.Config.productcodegenstart
            };
            var decimalplaces = {
                Category: 'general', SubCategory: 'masters', Key: 'decimalplaces',
                value: $scope.Config.decimalplaces
            };

            var printsacorhsnonchallans = {
                Category: 'general', SubCategory: 'print', Key: 'printsacorhsnonchallans',
                value: $scope.Config.printsacorhsnonchallans
            };
            var printWeightOnChallan = {
                Category: 'general', SubCategory: 'print', Key: 'printWeightOnChallan',
                value: $scope.Config.printWeightOnChallan || 'challan'
            };
            var showNextChallanInvoiceNumbers = {
                Category: 'general', SubCategory: 'issuechallan', Key: 'showNextChallanInvoiceNumbers',
                value: $scope.Config.showNextChallanInvoiceNumbers ? 'true' : 'false'
            };
            var notifyPartyOnDispatchInwardConfirm = {
                Category: 'general', SubCategory: 'notifications', Key: 'notifyPartyOnDispatchInwardConfirm',
                value: $scope.Config.notifyPartyOnDispatchInwardConfirm ? 'true' : 'false'
            };
            var testPhoneMobile = {
                Category: 'general', SubCategory: 'notifications', Key: 'testPhoneMobile',
                value: $scope.Config.testPhoneMobile || ''
            };
            var testEmail = {
                Category: 'general', SubCategory: 'notifications', Key: 'testEmail',
                value: $scope.Config.testEmail || ''
            };
            var mode = {
                Category: 'general', SubCategory: 'notifications', Key: 'mode',
                value: ($scope.Config.mode == 'Live') ? 'Live' : 'Test'
            };
            model.push(productcodegenmode);
            model.push(productcodegenprefix);
            model.push(productcodegenstart);
            model.push(decimalplaces);
            model.push(printsacorhsnonchallans);
            model.push(printWeightOnChallan);
            model.push(showNextChallanInvoiceNumbers);
            model.push(notifyPartyOnDispatchInwardConfirm);
            model.push(testPhoneMobile);
            model.push(testEmail);
            model.push(mode);

            var partycodegenmode = {
                Category: 'general', SubCategory: 'masters', Key: 'partycodegenmode',
                value: $scope.Config.partycodegenmode
            };
            var partycodegenprefix = {
                Category: 'general', SubCategory: 'masters', Key: 'partycodegenprefix',
                value: $scope.Config.partycodegenprefix
            };
            var partycodegenstart = {
                Category: 'general', SubCategory: 'masters', Key: 'partycodegenstart',
                value: $scope.Config.partycodegenstart
            };
            model.push(partycodegenmode);
            model.push(partycodegenprefix);
            model.push(partycodegenstart);

            var _cnfig = $scope.Config;
            if (_cnfig.productcodegenmode == 'auto' && (!_cnfig.productcodegenprefix || _cnfig.productcodegenprefix.length <= 1 || _cnfig.productcodegenstart < 1)) {
                alert('Please enter prefix and start for product codes');
                return;
            }
            if (_cnfig.partycodegenmode == 'auto' && (!_cnfig.partycodegenprefix || _cnfig.partycodegenprefix.length <= 1 || _cnfig.partycodegenstart < 1)) {
                alert('Please enter prefix and start for party codes');
                return;
            }
            config.SaveConfig(model, function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                alert('Information saved successfully.');
            });
        }

        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var maintain = response.Data.find(o => o.SubCategory == 'inventory' &&
                        o.Category == 'general' && o.Key == 'allowNegativeSale');
                    if (maintain) {
                        $scope.Config.allowNegativeSale = maintain.Value;
                    }
                    var hidQuotationClients = response.Data.find(o => o.SubCategory == 'clients' &&
                        o.Category == 'general' && o.Key == 'hidQuotationClients');
                    if (hidQuotationClients) {
                        $scope.Config.hidQuotationClients = hidQuotationClients.Value == 'true';
                    }
                    var warnIfClientNameExists = response.Data.find(o => o.SubCategory == 'clients' &&
                        o.Category == 'general' && o.Key == 'warnIfClientNameExists');
                    if (warnIfClientNameExists) {
                        $scope.Config.warnIfClientNameExists = warnIfClientNameExists.Value == 'true';
                    }

                    var signature = response.Data.find(o => o.SubCategory == 'print' &&
                        o.Category == 'general' && o.Key == 'signature');

                    if (signature) {
                        $scope.SignaturePrint.challans = false;
                        $scope.SignaturePrint.bills = false;
                        $scope.SignaturePrint.quotations = false;
                        var arr = signature.Value.split(',');
                        if (arr) {


                            $.each(arr, function (x, y) {
                                if (y == "challans") {
                                    $scope.SignaturePrint.challans = true;
                                }
                                if (y == "bills") {
                                    $scope.SignaturePrint.bills = true;
                                }
                                if (y == "quotations") {
                                    $scope.SignaturePrint.quotations = true;
                                }
                            });
                        }

                    }

                    var qrcode = response.Data.find(o => o.SubCategory == 'print' &&
                        o.Category == 'general' && o.Key == 'qrcode');
                    if (qrcode) {

                        $scope.PrintQrCode.bills = false;
                        $scope.PrintQrCode.quotations = false;
                        var arr = qrcode.Value.split(',');
                        if (arr) {
                            $.each(arr, function (x, y) {

                                if (y == "bills") {
                                    $scope.PrintQrCode.bills = true;
                                }
                                if (y == "quotations") {
                                    $scope.PrintQrCode.quotations = true;
                                }
                            });
                        }

                    }


                    var productcodegenmode = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'productcodegenmode');
                    if (productcodegenmode) {
                        $scope.Config.productcodegenmode = productcodegenmode.Value;
                    }
                    var productcodegenprefix = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'productcodegenprefix');
                    if (productcodegenprefix) {
                        $scope.Config.productcodegenprefix = productcodegenprefix.Value;
                    }
                    var productcodegenstart = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'productcodegenstart');
                    if (productcodegenstart) {
                        $scope.Config.productcodegenstart = productcodegenstart.Value;
                    }
                    //-- Party code settings
                    var partycodegenmode = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'partycodegenmode');
                    if (partycodegenmode) {
                        $scope.Config.partycodegenmode = partycodegenmode.Value;
                    }
                    var partycodegenprefix = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'partycodegenprefix');
                    if (partycodegenprefix) {
                        $scope.Config.partycodegenprefix = partycodegenprefix.Value;
                    }
                    var partycodegenstart = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'partycodegenstart');
                    if (partycodegenstart) {
                        $scope.Config.partycodegenstart = partycodegenstart.Value;
                    }
                    var decimalplaces = response.Data.find(o => o.SubCategory == 'masters' &&
                        o.Category == 'general' && o.Key == 'decimalplaces');
                    if (decimalplaces) {
                        $scope.Config.decimalplaces = parseInt(decimalplaces.Value);
                    }
                     
                    var printsacorhsnonchallans = response.Data.find(o => o.SubCategory == 'print' &&
                        o.Category == 'general' && o.Key == 'printsacorhsnonchallans');
                    if (printsacorhsnonchallans) {
                        $scope.Config.printsacorhsnonchallans = printsacorhsnonchallans.Value;
                    }
                    var printWeightOnChallan = response.Data.find(o => o.SubCategory == 'print' &&
                        o.Category == 'general' && o.Key == 'printWeightOnChallan');
                    if (printWeightOnChallan && printWeightOnChallan.Value) {
                        $scope.Config.printWeightOnChallan = printWeightOnChallan.Value;
                    }

                    var showNextChallanInvoiceNumbers = response.Data.find(o => o.SubCategory == 'issuechallan' &&
                        o.Category == 'general' && o.Key == 'showNextChallanInvoiceNumbers');
                    if (showNextChallanInvoiceNumbers) {
                        $scope.Config.showNextChallanInvoiceNumbers = showNextChallanInvoiceNumbers.Value == 'true';
                    }
                    var notifyPartyOnDispatchInwardConfirm = response.Data.find(o => o.SubCategory == 'notifications' &&
                        o.Category == 'general' && o.Key == 'notifyPartyOnDispatchInwardConfirm');
                    if (notifyPartyOnDispatchInwardConfirm) {
                        $scope.Config.notifyPartyOnDispatchInwardConfirm = notifyPartyOnDispatchInwardConfirm.Value == 'true';
                    }
                    var testPhoneMobile = response.Data.find(o => o.SubCategory == 'notifications' &&
                        o.Category == 'general' && o.Key == 'testPhoneMobile');
                    if (testPhoneMobile) {
                        $scope.Config.testPhoneMobile = testPhoneMobile.Value || '';
                    }
                    var testEmail = response.Data.find(o => o.SubCategory == 'notifications' &&
                        o.Category == 'general' && o.Key == 'testEmail');
                    if (testEmail) {
                        $scope.Config.testEmail = testEmail.Value || '';
                    }
                    var mode = response.Data.find(o => o.SubCategory == 'notifications' &&
                        o.Category == 'general' && o.Key == 'mode');
                    if (mode && (mode.Value == 'Test' || mode.Value == 'Live')) {
                        $scope.Config.mode = mode.Value;
                    }

                    
                }

            }
        }, 'general');

        $scope.validateDecimal = function () {

            if ($scope.Config.decimalplaces < 2) {
                $scope.Config.decimalplaces = 2;
            }
            if ($scope.Config.decimalplaces > 3) {
                $scope.Config.decimalplaces = 3;
            }
        };
    }
);
app.controller("AlertsController",
    function ($scope, $rootScope,) {

        $rootScope.$on('onCompanySelected', function (e) {
            $scope.GetAlerts();
        });
        $scope.Alerts = [];
        $scope.GetAlerts = function () {

            var ns = new $.Notification();
            ns.GetAlerts(function (e) {

                $scope.Alerts = e.data.Data;
                $rootScope.totalAlerts = $scope.Alerts.length;
            });
        }

    });

app.controller('TemplateCustomizeController', function ($scope) {

});