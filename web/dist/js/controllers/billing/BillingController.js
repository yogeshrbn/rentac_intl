/** Main rent grid only — invoice breakage lines must not appear as rent line items. */
function rentBillItemsOnly(items) {
    if (!items || !items.length) {
        return items || [];
    }
    return items.filter(function (it) {
        return !(it && (it.IsBreakage === true || it.isBreakage === true));
    });
}

function applyRentBillLineDiscounts(items, config, billing) {
    var subTotal = items.reduce(function (partialSum, a) { return partialSum + a.Amount; }, 0);
    var totalItemDiscount = 0;
    $.each(items, function (indx, val) {
        val.Discount = 0;
        if (config.discount_type == 'itemlevel') {
            if (billing.DiscountPercent > 0) {
                val.DiscountPercent = billing.DiscountPercent;
                val.Discount = parseFloat((val.Amount * billing.DiscountPercent / 100));
                totalItemDiscount += round(val.Discount);
            }
            else if (billing.Discount > 0) {
                val.Discount = parseFloat((val.Amount / subTotal) * billing.Discount);
            }
        }
    });
    return totalItemDiscount;
}

function resolveDefaultGstRates(defaultTaxRate, isIntraState) {
    var rate = parseFloat(defaultTaxRate) || 0;
    if (rate <= 0) {
        return { igstRate: 0, cgstRate: 0, sgstRate: 0 };
    }
    if (isIntraState) {
        var half = rate / 2;
        return { igstRate: 0, cgstRate: half, sgstRate: half };
    }
    return { igstRate: rate, cgstRate: 0, sgstRate: 0 };
}

function applyBillSubtotalGst(items, getLineTaxable, config, isIntraState) {
    var taxableSum = 0;
    $.each(items, function (i, val) {
        taxableSum += getLineTaxable(val);
    });
    if (taxableSum <= 0) {
        $.each(items, function (i, val) {
            val.IGST = 0;
            val.SGST = 0;
            val.CGST = 0;
            val.IGSTRate = 0;
            val.CGSTRate = 0;
            val.SGSTRate = 0;
        });
        return;
    }
    var taxableBase = taxableSum;
    if (config && config.autoRoundOffTaxable) {
        taxableBase = round(taxableSum);
    }
    var rates = resolveDefaultGstRates(config.defaultTaxRate, isIntraState);
    var gIGST = parseFloat((taxableBase * rates.igstRate / 100).toFixed(2));
    var gCGST = parseFloat((taxableBase * rates.cgstRate / 100).toFixed(2));
    var gSGST = parseFloat((taxableBase * rates.sgstRate / 100).toFixed(2));
    $.each(items, function (j, val) {
        var taxable = getLineTaxable(val);
        var ratio = taxable / taxableSum;
        val.IGST = parseFloat((gIGST * ratio).toFixed(2));
        val.CGST = parseFloat((gCGST * ratio).toFixed(2));
        val.SGST = parseFloat((gSGST * ratio).toFixed(2));
        val.IGSTRate = rates.igstRate;
        val.CGSTRate = rates.cgstRate;
        val.SGSTRate = rates.sgstRate;
    });
}

function resolveBillIsIntraState(comp, accounts, ledgerId) {
    if (!comp || !accounts || !ledgerId) {
        return false;
    }
    var party = accounts.find(function (o) { return o.LedgerId == ledgerId; });
    if (!party || !party.StateCode || !comp.StateCode) {
        return false;
    }
    return comp.StateCode == party.StateCode;
}

/** Rent items subtotal + freight + other charges + GRN damage + lost subtotal (pre-tax). */
function computeRentBillTaxable(billing, config, totalOtherCharges, lossCharges) {
    var itemsSub = parseFloat(billing && billing.SubTotal) || 0;
    var freight = parseFloat(billing && billing.Freight) || 0;
    var other = parseFloat(totalOtherCharges) || 0;
    var lossSub = 0;
    if (lossCharges && lossCharges.length) {
        for (var li = 0; li < lossCharges.length; li++) {
            lossSub += parseFloat(lossCharges[li].Amount) || 0;
        }
    }
    var damage = parseFloat(billing && billing.BreakageAmount) || 0;
    if (!damage) {
        var dds = billing && billing.BreakageDamageDetails;
        if (dds && dds.length) {
            for (var dj = 0; dj < dds.length; dj++) {
                var row = dds[dj];
                var c = row.Cost != null ? row.Cost : row.cost;
                damage += parseFloat(c) || 0;
            }
        }
    }
    var taxable = itemsSub + freight + other + damage + lossSub;
    if (config && config.autoRoundOffTaxable) {
        taxable = round(taxable);
    }
    return taxable;
}

function loadBillingTaxConfigExtras(response, config) {
    if (!response || !response.Data) {
        return;
    }
    var defaultTaxRate = response.Data.find(function (o) { return o.SubCategory == 'Tax' && o.Key == 'defaultTaxRate'; });
    if (defaultTaxRate && defaultTaxRate.Value != null && defaultTaxRate.Value !== '') {
        config.defaultTaxRate = +defaultTaxRate.Value;
    }
    var autoRoundOffTaxable = response.Data.find(function (o) { return o.SubCategory == 'Tax' && o.Key == 'autoRoundOffTaxable'; });
    if (autoRoundOffTaxable) {
        config.autoRoundOffTaxable = autoRoundOffTaxable.Value == '1' || autoRoundOffTaxable.Value === true || autoRoundOffTaxable.Value === 'true';
    }
    var applyTaxOnCfg = response.Data.find(function (o) { return o.SubCategory == 'Tax' && (o.Key == 'applyTaxOn' || o.Key.toLowerCase() == 'applytaxon'); });
    if (applyTaxOnCfg && applyTaxOnCfg.Value) {
        var ato = ('' + applyTaxOnCfg.Value).toLowerCase();
        if (ato === 'subtotal' || ato === 'itemlevel') {
            config.applyTaxOn = ato;
        }
    }
}

function applyRentBillLineGst(items, config, applyTax, isIntraState) {
    var applyTaxOn = ((config && config.applyTaxOn) || 'itemlevel').toString().toLowerCase();
    if (!applyTax) {
        $.each(items, function (i, val) {
            val.IGST = 0;
            val.SGST = 0;
            val.CGST = 0;
        });
        return;
    }
    if (applyTaxOn === 'subtotal') {

        applyBillSubtotalGst(items, function (val) {
            return (parseFloat(val.Amount) || 0) - (parseFloat(val.Discount) || 0);
        }, config, isIntraState);
    } else {
        $.each(items, function (indx, val) {
            var taxable = val.Amount - val.Discount;
            val.IGST = parseFloat((taxable) * val.IGSTRate / 100);
            val.SGST = parseFloat((taxable) * val.SGSTRate / 100);
            val.CGST = parseFloat((taxable) * val.CGSTRate / 100);
        });
    }
}

app.controller('BillingController', ['$scope', '$rootScope', '$stateParams', 'FileSaver', '$state', '$crypto', '$mdDialog',
    '$window', '$uibModal', 'ModalFactory', 'LedgerFactory', 'WorkOrderFactory', 'ChallanTaxService', 'TaxService',
    function ($scope, $rootScope, $stateParams, FileSaver, $state, $crypto, $mdDialog, $window, $uibModal,
        ModalFactory, LedgerFactory, WorkOrderFactory, ChallanTaxService, TaxService) {

        function applyInvoiceTotalRoundOff(cfg, billing, total) {
            var method = (cfg.roundOffTotalMethod || 'none').toString().toLowerCase();
            var autoRound = method !== 'none';
            if (!autoRound && billing.RoundOff !== true) {
                return total;
            }
            var kind = autoRound ? method : 'nearest';
            var t = Number(total);
            if (isNaN(t)) {
                return total;
            }
            if (kind === 'up') {
                return Math.ceil(t);
            }
            if (kind === 'down') {
                return Math.floor(t);
            }
            return Math.round(t);
        }

        var lederDetails = $scope.localData;
        var config = new $.Config({});
        $scope.Config = {
            BillPrefix: '',
            FreightTax: 0,
            allowRateEdits: false
            , BrekageBill: 1
            , allowEditBillNo: false,
            discount_type: 'invoicelevel',
            applyTaxOn: 'itemlevel',
            defaultTaxRate: 0,
            autoRoundOffTaxable: false,
            roundOffTotalMethod: 'none',
            defaultRoundOffTotal: false
        };
        $scope.ApplyGST = true;
        $scope.ApplyFreightGST = true;
        $scope.ApplyOtherChargeGST = true;
        $scope.ItemTaxSettings = [];
        $scope._taxDataPromise = initSaleTaxData($scope, TaxService, ChallanTaxService);
        $scope.editorOptions = {

            height: 200

        };
        $scope.AllSizes = [];
        $scope.getAllProductSizesByCompany = function () {
            var product = new $.Product();
            product.GetAll(function (e) {
                //debugger
                // console.log('AllSizes', e.data);
                $scope.AllSizes = e.data;

            });
        }
        $scope.getAllProductSizesByCompany();

        function loadCompanyForTax() {
            var token = $rootScope.getTokenInfo();
            if (!token || !token.DefaultCompanyId) {
                return;
            }
            var company = new $.Company();
            company.Props.CompanyId = token.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status == 200 && e.data) {
                    $scope.comp = e.data;
                }
            });
        }
        loadCompanyForTax();

        function GetBillingConfig() {
            config.GetBillingConfig(function (e) {

                var response = e.data;
                if (response.Data != null && response.Data) {
                    if (response.Data.length > 0) {

                        var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                        var applyTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'applyTax');
                        // var breakageBill = response.Data.find(o => o.Key == 'breakageBill');
                        var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');
                        var billPrefix = response.Data.find(o => o.Key == 'Prefix' && o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST');
                        var tnc = response.Data.find(o => o.Key == 'tnc' && o.SubCategory == 'Other' && o.Category == 'Invoice');
                        var defaultRateType = response.Data.find(o => o.Key == 'defaultRateType');
                        var billReceivedDate = response.Data.find(o => o.Key == 'billReceivedDate');
                        var adjustAdvance = response.Data.find(o => o.Key == 'adjustAdvance');
                        var applydiscounton = response.Data.find(o => o.Key == 'applydiscounton');
                        if (applydiscounton) {
                            $scope.Config.applydiscounton = applydiscounton.Value;
                        }
                        if (billReceivedDate)
                            $scope.Billing.ChargeReturnDay = billReceivedDate.Value == 'true' ? 1 : 0;

                        if (defaultRateType) {
                            $scope.Billing.RateCalcType = defaultRateType.Value == 'day' ? 1 : 2;
                        }

                        if (applyTax) {
                            $scope.Billing.applyTax = applyTax.Value == 'true';
                        }
                        if (freightTax) {

                            $scope.Config.FreightTax = freightTax.Value
                            if (applyTax) {
                                if (applyTax.Value != 'true') {
                                    $scope.Config.FreightTax = 0;
                                }
                            }
                        }
                        //if (breakageBill) {
                        //    $scope.Config.BrekageBill = breakageBill.Value == 'true' ? 1 : 0;
                        //}
                        if (allowEditBillNo) {
                            $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                        }
                        if (billPrefix) {
                            $scope.Config.billPrefix = billPrefix.Value;
                        }
                        if (tnc) {
                            $scope.Billing.Tnc = tnc.Value;
                        }
                        if (adjustAdvance) {
                            $scope.Config.adjustAdvance = adjustAdvance.Value;
                        }
                        var allowRateEdits = response.Data.find(o => o.Key.toLowerCase() == 'allowrateedits');
                        if (allowRateEdits) {
                            $scope.Config.allowRateEdits = allowRateEdits.Value == 'true' || allowRateEdits.Value == '1';
                        }
                        var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');
                        if (discount_type) {
                            $scope.Config.discount_type = discount_type.Value;
                        }

                        loadBillingTaxConfigExtras(response, $scope.Config);
                        var roundOffTotalMethod = response.Data.find(o => o.Key == 'roundOffTotalMethod');
                        if (roundOffTotalMethod && roundOffTotalMethod.Value) {
                            var rm = ('' + roundOffTotalMethod.Value).toLowerCase();
                            if (rm === 'none' || rm === 'nearest' || rm === 'up' || rm === 'down') {
                                $scope.Config.roundOffTotalMethod = rm;
                            }
                        }
                        var defaultRoundOffTotal = response.Data.find(o => o.Key == 'defaultRoundOffTotal');
                        if (defaultRoundOffTotal) {
                            $scope.Config.defaultRoundOffTotal = defaultRoundOffTotal.Value == '1' || defaultRoundOffTotal.Value === true || defaultRoundOffTotal.Value === 'true';
                        }
                        if ($scope.Billing && (!$scope.Billing.InvoiceId || $scope.Billing.InvoiceId === 0)) {
                            $scope.Billing.RoundOff = !!$scope.Config.defaultRoundOffTotal;
                        }

                        if ($scope.Billing && $scope.Billing.IsCashBill) {
                            $scope.Billing.applyTax = false;
                        }
                        $scope.ApplyGST = $scope.Billing.applyTax !== false;

                    }
                }
            });
        }
        GetBillingConfig();
        FormsValidation.init();

        function displaySave(display) {
            if (display) {
                $('#btnSave').show();
            } else {
                $('#btnSave').hide();
            }
        }


        var ledger = new $.Ledger({});
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Billing.LedgerId == null) {
                $scope.Billing.LedgerId = e.data[0].LedgerId;
            }
        });


        $scope.NewBill = function () {
            newBill();
        }
        newBill();

        $scope.$watch('Billing.IsCashBill', function (cash) {
            if (cash) {
                $scope.Billing.applyTax = false;
                $scope.ApplyGST = false;
            }
        });

        $scope.$watch('ApplyGST', function () {
            if ($scope.CalSubTotal) {
                $scope.CalSubTotal();
            }
        });

        $scope.GenBill = function () {

            var isValid = validateForm();
            if (!isValid) {
                return;
            }
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            if (billing.FilterChallansByPO && (!billing.PONumber || billing.PONumber.trim() === '')) {
                alert('Please enter PO# to filter challans by PO.');
                return;
            }
            $scope.BillData = [];
            $scope.Billing.GenerateBill(function (e) {

                //if (e.data.Code != undefined) {
                //    alert(e.data.Message);
                //    return;
                //}
                if (e.data.Code && e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.BillData = $scope.Billing.Items = rentBillItemsOnly(e.data.BillingItems);
                $scope.Billing.Challans = e.data.Challans;
                $scope.Billing.Breakage = e.data.Breakage;
                $scope.Billing.BreakageDamageDetails = e.data.BreakageDamageDetails || e.data.breakageDamageDetails || [];
                $scope.BillGenerated = true;

                $scope.Billing.applyTax = e.data.ApplyTax;
                if ($scope.Billing.IsCashBill) {
                    $scope.Billing.applyTax = false;
                }
                $scope.ApplyGST = $scope.Billing.applyTax !== false;

                $scope.LossCharges = e.data.LostItems;
                $scope.ClosingBalance = e.data.StockBalanceAfterBill;
                // applyAllTaxes();
                if ($scope.BillData.length > 0) {
                    $scope.Billing.Freight = $scope.BillData[0].Freight;
                    displaySave(true);
                    //$scope.getBreakageForBill();

                }
                $scope.Billing.AccountLedger = e.data.AccountLedger;
                $scope.Billing.OtherCharges = e.data.OtherCharges;
                $scope.Billing.PO = e.data.PO || [];
                debugger
                $scope._taxDataPromise.then(function () {
                    $scope.CalSubTotal();
                });

            }, billing);
            // getStockBalance();
            //   LedgerFactory.GetAccountBalanceForBill($scope.Billing, getLedger);


        }

        $scope.removeGenBillPo = function (index) {
            if (!$scope.Billing || !$scope.Billing.PO || !angular.isArray($scope.Billing.PO)) {
                return;
            }
            if (index < 0 || index >= $scope.Billing.PO.length) {
                return;
            }
            $scope.Billing.PO.splice(index, 1);
        };

        //function applyAllTaxes() {

        //    for (var i = 0; i < $scope.Billing.Taxes.length; i++) {

        //        var tax = $scope.Billing.Taxes[i];
        //        if (tax.Rate > 0) {
        //            tax.Applicable = true;
        //        }
        //        $scope.ApplyTax(tax);
        //    }

        //}

        function newBill() {
            $scope.Billing = new $.Billing({});
            //GetTaxes();
            $scope.CurrentBill = new $.Billing({
                BillNumber: ''
            });
            $scope.Billing.InvoiceType = 1; //default rent type bill
            $scope.Billing.RateCalcType = 1;
            $scope.BillData = null;
            $scope.Breakages = null;
            $scope.AllTaxes = null;
            $scope.Billing.IGST = 0;
            $scope.Billing.SGST = 0;
            $scope.Billing.CGST = 0;

            $scope.Billing.DiscountPercent = 0;
            $scope.Billing.BreakageDiscountPercent = 0;
            $scope.Billing.LossDiscountPercent = 0;
            $scope.Billing.BreakageDiscount = 0;
            $scope.Billing.LossDiscount = 0;

            $scope.Billing.TotalLossAmount = 0;
            $scope.Billing.TotalBreakageAmount = 0;
            $scope.Billing.IsCashBill = false;
            $scope.Billing.FilterChallansByPO = false;

            displaySave(false);
            var date = new Date();
            var token = $rootScope.getTokenInfo();
            $scope.Billing.To = convertDate(date);
            $scope.Billing.InvoiceDate = convertDate(date);

            if (token) {
                $scope.Billing.From = convertDate(token.FinYearStart);
                $scope.MinDate = token.FinYearStart;
                $scope.MaxDate = token.FinYearEnd; // convertDate(token.FinYearEnd);
            }
            $scope.LastBill = {};

            if (lederDetails) {
                var ld = JSON.parse($crypto.decrypt(lederDetails));
                console.log(ld);
                if (ld) {
                    $scope.Billing.LedgerId = ld.LedgerId;
                    $scope.Billing.LedgerSiteId = ld.LedgerSiteId;

                }
            }
            $scope.Billing.RoundOff = !!$scope.Config.defaultRoundOffTotal;
            $scope.BillGenerated = false;
        }
        $scope.showBreakageBalanceTab = function () {
            if (!$scope.Billing) return false;
            var inc = $scope.Billing.IncludeBreakageItems === true || $scope.Billing.IncludeBreakageItems === 1;
            return inc && !!$scope.BillGenerated;
        };
        // $scope.CurrentBill = new $.Billing({BillNumber:'<NEW>'});
        $scope.SaveBill = function () {
            if (!validateForm()) {
                return;
            }
            if (!$scope.Billing.Items) {
                alert('No line items found for billing');
                return;
            }
            if ($scope.Billing.Items.length <= 0) {
                alert('No line items found for billing');
                return;
            }

            $scope.CalSubTotal();
            $scope.Billing.LossItems = $scope.LossCharges;
            if ($scope.Billing.BreakageDamageDetails && $scope.Billing.BreakageDamageDetails.length) {
                $scope.Billing.BreakageItems = $scope.Billing.Breakage || $scope.Billing.BreakageItems || [];
            }

            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            billing.InvoiceDate = formatdate(billing.InvoiceDate);
            billing.Tnc = htmlEncode(billing.Tnc);

            if (billing.PODate)
                billing.PODate = formatdate(billing.PODate);


            if ($scope.Config.allowEditBillNo == true) {
                if (!billing.InvoiceNumber) {
                    alert('Please enter the invoice number.');
                    return;
                }
                if (trimAll(billing.InvoiceNumber).length <= 1) {
                    alert('Please enter a valid invoice number.');
                    return;
                }
            }

            if ($scope.Payments) {
                var payments = cloneObj($scope.Payments.filter(o => o.PaidAmount > 0));
                $.each(payments, function (index, value) {
                    if (value.PaidAmount > 0) {
                        value.TransactionAmount = value.PaidAmount;
                    }
                });
                billing.Payments = payments;
            }

            $scope.Billing.SaveBill(function (e) {

                if (e.data.Code == 200) {
                    $scope.CurrentBill = e.data;
                    alert('Bill generated successfully');
                    if ($scope.localData) {
                        $scope.onBillSavedOnDialog();
                        return;
                    }

                    chooseAndPrintBill(e.data.Data.InvoiceId);
                } else {
                    alert(e.data.Message);
                }

            }, billing);
        }

        function chooseAndPrintBill(invoiceId) {
            //ModalFactory.BillType(function ($scope, $mdDialog) {
            //    $scope.closeDialog = function () {
            //        $mdDialog.hide();
            //    },
            //        $scope.OkButtonClick = function () {
            //            $(event.currentTarget).find('.fa-spin').show();
            //            printBill($scope.HeaderType, invoiceId);
            //        }
            //});

            var encrypedText = $crypto.encrypt(invoiceId);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReportFromHtml(function (e) {
                FileSaver.saveAs(e.data, $scope.CurrentBill.Data.InvoiceNumber + '.pdf');
                $state.go('billList');
            }, 'PrintRentBill', econded);

        }

        function printBill(headerType, invoiceId) {

            //$scope.Billing.InvoiceId = invoiceId;
            //$scope.Billing.BillCopyType = headerType;
            ////  $scope.Billing.InvoiceNumber = e.data.InvoiceNumber;
            //$scope.Billing.PrintBill(function (e) {
            //    $mdDialog.hide();
            //    var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            //    // $window.target = '_blank';
            //    $window.open(filePath);
            //});

            //$route.reload();
        }
        //--Loading site information
        var _siteNames = [];
        _siteNames.push({
            JobNumber: '',
            SiteId: 0
        });
        var site = new $.Site({});


        $scope.CalculateSum = function () {
            var total = 0;
            if ($scope.BillData != undefined) {
                for (var i = 0; i <= $scope.BillData.length - 1; i++) {
                    var v = $scope.BillData[i];
                    total += parseFloat($scope.BillData[i].Amount);
                    //   total+=(v.ClosingBalance * (v.ChargeReturnedDate? (v.Days + 1) : v.Days) * v.Rate) ;
                }
            }
            //var otherCharge = 0;
            //if ($scope.Billing.OtherCharges != null) {
            //    $.each($scope.Billing.OtherCharges, function () {
            //        otherCharge += this.Amount;
            //    });
            //}
            $scope.Billing.SubTotal = total;
            // $scope.Billing.Total1 = parseFloat($scope.Billing.SubTotal) + parseFloat($scope.Billing.Freight) + $scope.totalBreakageAmount() + $scope.TotalLossCharges() + $scope.fnOtherChargesTotal();
            return total;
        }

        $scope.fnOtherChargesTotal = function () {

            var otherCharge = 0;
            if ($scope.Billing.OtherCharges != null) {
                $.each($scope.Billing.OtherCharges, function () {
                    otherCharge += this.Amount;
                });
            }
            $scope.TotalOtherCharges = otherCharge;
            return otherCharge;
        }
        $scope.CalSubTotal = function () {
            runGenBillSubtotal($scope, ChallanTaxService);
            $scope.Billing.Total = applyInvoiceTotalRoundOff($scope.Config, $scope.Billing, $scope.Billing.Total);
            if (($scope.Config.roundOffTotalMethod || 'none').toString().toLowerCase() !== 'none') {
                $scope.Billing.RoundOff = true;
            }
            return $scope.Billing.Total;
        }

        //function GetTaxes() {
        //    var Tax = new $.Tax({
        //        ItemId: 0
        //    });
        //    $scope.WorkOrder = new $.WorkOrder({});
        //    //Tax.GetTaxes($scope.WorkOrder, function (e) {

        //    //    $scope.Billing.Taxes = e.data;
        //    //});
        //    if ($scope.Billing.LedgerSiteId > 0) {
        //        //LedgerFactory.GetSiteTaxes(function (e) {
        //        //    $scope.Billing.Taxes = e.data.Data;
        //        //}, {
        //        //    LedgerSiteId: $scope.Billing.LedgerSiteId
        //        //});
        //    }
        //    //Tax.GetAllTaxes(function (e) {
        //    //    $scope.AllTaxes = e.data;
        //    //});
        //}
        $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
        $scope.Billing.FreightTax = 0;
        $scope.Billing.BreakageTax = 0;

        $scope.totalBreakageAmount = function () {
            var bAmount = 0;
            var dds = $scope.Billing.BreakageDamageDetails;
            if (dds && dds.length) {
                for (var i = 0; i < dds.length; i++) {
                    var row = dds[i];
                    bAmount += parseFloat(row.Cost != null ? row.Cost : row.cost) || 0;
                }
            }
            return bAmount;
        }
        $scope.TotalLossCharges = function () {
            var bAmount = 0;
            if ($scope.LossCharges) {

                for (var i = 0; i <= $scope.LossCharges.length - 1; i++) {
                    bAmount += ($scope.LossCharges[i].Rate * $scope.LossCharges[i].Quantity);
                }
            }
            //  $scope.Billing.TotalLossAmount = bAmount;
            return bAmount;
        }
        $scope.taxableValueBase = function () {
            $scope.Billing.Taxable = computeRentBillTaxable($scope.Billing, $scope.Config, $scope.TotalOtherCharges, $scope.LossCharges);
        };
        //$scope.onBreakageDiscountPercentChange = function () {
        //    var breakageAmount = $scope.totalBreakageAmount();
        //    $scope.Billing.BreakageDiscount = 0;
        //    $scope.Billing.TotalBreakageAmount = 0;
        //    if (breakageAmount > 0) {
        //        $scope.Billing.BreakageDiscount = (breakageAmount * $scope.Billing.BreakageDiscountPercent / 100)

        //    }
        //    $scope.calcBreakageTotal();
        //}
        //$scope.onBreakageDiscountChange = function () {
        //    var breakageAmount = $scope.totalBreakageAmount();
        //    $scope.Billing.BreakageDiscountPercent = 0;
        //    $scope.calcBreakageTotal();
        //}
        //$scope.onLossDiscountPercentChange = function () {
        //    var lossAmount = $scope.TotalLossCharges();
        //    $scope.Billing.LossDiscount = 0;
        //    //   $scope.Billing.TotalLossAmount = 0;
        //    if (lossAmount > 0) {
        //        $scope.Billing.LossDiscount = (lossAmount * $scope.Billing.LossDiscountPercent / 100)
        //        // $scope.Billing.TotalLossAmount = lossAmount - $scope.Billing.LossDiscount;
        //    }
        //    $scope.calcLossTotal();
        //}
        //$scope.onLossDiscountChange = function () {

        //    var lossAmount = $scope.TotalLossCharges();
        //    $scope.Billing.LossDiscountPercent = 0;
        //    $scope.calcLossTotal();
        //    //  $scope.Billing.TotalLossAmount = 0;
        //    //if (lossAmount > 0) {

        //    //    $scope.Billing.TotalLossAmount = lossAmount - $scope.Billing.LossDiscount;
        //    //}
        //}
        $scope.onDiscountPercentChange = function () {

            $scope.CalSubTotal();
        }
        $scope.onDiscountChange = function () {

            $scope.Billing.DiscountPercent = 0;
            $scope.CalSubTotal();
        }
        //$scope.isDisplayed = false;
        $scope.$watch('Billing.LedgerId', function () {

            $scope.BillData = [];
            $scope.Billing.Items = [];
            ledger.Props.LedgerId = ledger.LedgerId = $scope.Billing.LedgerId;
            if (ledger.LedgerId == 0) return;
            //  getStockBalance();
            //  LedgerFactory.GetAccountBalanceForBill(getLedger, ledger);
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
            getAllProducts();
            getLastBill();
        });



        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, {
                LedgerId: $scope.Billing.LedgerId
            });
        }
        $scope.$watch('Billing.LedgerSiteId', function () {
            // getStockBalance();
            getLastBill();
            //    GetTaxes();
            getSiteBalanceOtherCharges();

            if ($scope.Config.adjustAdvance == 1) {
                var model = cloneObj($scope.Billing);
                ledger.getAdvanceReceipts(function (e) {
                    if (e.data.Code == 200) {
                        $scope.Payments = e.data.Data;
                    }
                }, model);
            }
        })


        function getAllProducts() {
            ledger.GetProductRates(function (e) {
                $scope.ProductRates = e.data;
            });

        }

        function getLastBill() {

            // alert($scope.dueBillFilter);
            LedgerFactory.GetLastBill(function (e) {
                if (e.data.Data) {
                    $scope.LastBill = e.data.Data;

                    if ($scope.LastBill) {
                        $scope.Billing.From = convertDate($scope.LastBill.To)
                    }
                }
                if ($scope.dueBillFilter) {
                    $scope.Billing.From = $scope.dueBillFilter.From;
                    $scope.Billing.To = $scope.dueBillFilter.To;

                }

            }, {
                LedgerId: $scope.Billing.LedgerId,
                LedgerSiteId: $scope.Billing.LedgerSiteId
            });
        }

        function getSiteBalanceOtherCharges() {
            //WorkOrderFactory.GeSiteOtherBalanceCharges($scope.Billing.LedgerSiteId, function (e) {
            //    $scope.Billing.OtherCharges = e.data;
            //});
        }

        $scope.LossCharges = [];

        $scope.getItemRate = function (productId) {
            if ($scope.ProductRates) {
                return $scope.ProductRates.find(o => o.ProductId == productId);
            }
        }

        $scope.DeleteLossItem = function (index) {
            $scope.LossCharges.splice(index, 1);
        }
        var modal
        $scope.showTaxItems = function () {
            debugger
            modal = $uibModal.open({
                windowClass: 'right',
                templateUrl: 'templ/taxItems.html',
                scope: $scope, //passed current scope to the modal
                size: 'lg'

            });

        }
        $scope.closeSliderModal = function () {

            modal.close({});
        }

        $scope.ApplyExtraDays = function (item) {

            if (item.ChargeReturnedDate) {
                item.Days++;
            } else {
                item.Days--;
            }
            if (item.ItemCategory == 1012) {
                item.Amount = (item.ClosingBalance * item.Days * item.Rate);
            } else {

                item.Amount = item.ClosingBalance * item.Rate;

            }
        }
        $scope.ApplyRate = function (item) {


            if (item.ItemCategory == 1012) {
                item.Amount = (item.ClosingBalance * item.Days * item.Rate);
            } else { //consumeables

                item.Amount = item.ClosingBalance * item.Rate;

            }
        }

        function validateForm() {
            return $('#form-bill').valid();
        }

        $scope.openBillTaxSettings = function () {
            $scope.ApplyGST = $scope.Billing.applyTax !== false;
            var div = '<div></div>';
            $(div).load('templ/dialogs/billTaxSettings.dialog.html?d=' + new Date().getTime(), function () {
                var html = $(this).html();
                $mdDialog.show({
                    //  clickOutsideToClose: true,
                    skipHide: true,
                    scope: $scope,
                    preserveScope: true,
                    template: html,
                    parent: angular.element(document.body),
                    controller: ['$scope', '$mdDialog', function ($dialogScope, $mdDialog) {
                        $dialogScope.closeBillTaxSettingsDialog = function () {
                            $mdDialog.hide();
                        };
                        $dialogScope.confirmBillTaxSettings = function () {
                            if ($dialogScope.Billing && $dialogScope.Billing.IsCashBill) {
                                $dialogScope.Billing.applyTax = false;
                            }
                            $dialogScope.ApplyGST = $dialogScope.Billing.applyTax !== false;
                            if (typeof $dialogScope.CalSubTotal === 'function') {
                                $dialogScope.CalSubTotal();
                            }
                            $mdDialog.hide();
                        };
                    }]
                });
            });
        };

        $scope.cancel = function () {
            if ($scope.localData) {
                $scope.onBillSavedOnDialog();
                return;
            }
            else {
                $state.go('billList');
            }
        }

        $scope.$watch('LossCharges', function (e, y) {
            if (!e) {
                return;
            }
            $.each(e, function (index, value) {
                value.Amount = parseFloat(value.Quantity) * parseFloat(value.Rate);
            });
            if ($scope.LossCharges) {
                $scope.Billing.LossIGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.IGST) || 0), 0);
                $scope.Billing.LossSGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.SGST) || 0), 0);
                $scope.Billing.LossCGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.CGST) || 0), 0);
            }

            $scope.calcLossTotal();
            $scope.CalSubTotal();

        }, true);
        $scope.calcLossTotal = function () {
            if (!$scope.LossCharges) {
                return;
            }
            //  $scope.Billing.TotalLossAmount = $scope.LossCharges.reduce((partialSum, a) => partialSum + a.Amount + a.CGST + a.SGST + a.IGST, 0);
            $scope.Billing.TotalLossAmount = $scope.LossCharges.reduce((partialSum, a) => partialSum + a.Amount, 0);

            if ($scope.Billing.LossDiscount) {
                $scope.Billing.TotalLossAmount = $scope.Billing.TotalLossAmount - $scope.Billing.LossDiscount;
            }
        }
        $scope.calcBreakageTotal = function () {
            if (!$scope.Billing.Breakage) {
                return;
            }
            $scope.Billing.TotalBreakageAmount = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + a.Amount + a.CGST + a.SGST + a.IGST, 0);
            if ($scope.Billing.BreakageDiscount) {
                $scope.Billing.TotalBreakageAmount = $scope.Billing.TotalBreakageAmount - $scope.Billing.BreakageDiscount;
            }
        }
        $scope.$watch('Billing.Breakage', function (e, y) {
            if (!e) {
                return;
            }
            var useDamageDetails = $scope.Billing.BreakageDamageDetails && $scope.Billing.BreakageDamageDetails.length;
            $.each(e, function (index, value) {
                value.Amount = parseFloat(value.Quantity) * parseFloat(value.Rate);
                if (!useDamageDetails) {
                    value.IGST = value.IGSTRate * value.Amount / 100;
                    value.CGST = value.CGSTRate * value.Amount / 100;
                    value.SGST = value.SGSTRate * value.Amount / 100;
                }
            });
            if ($scope.Billing.Breakage) {
                $scope.Billing.Breakage.IGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.IGST) || 0), 0);
                $scope.Billing.Breakage.SGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.SGST) || 0), 0);
                $scope.Billing.Breakage.CGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.CGST) || 0), 0);
            }
            $scope.calcBreakageTotal();
            $scope.CalSubTotal();

        }, true);
    }



]);
app.controller('EditBillController', ['$scope', '$stateParams', '$location', 'FileSaver', '$state', '$crypto', '$mdDialog',
    '$q', '$rootScope', '$uibModal', 'ModalFactory', 'LedgerFactory', 'WorkOrderFactory', 'ChallanTaxService', 'TaxService',
    function ($scope, $stateParams, $location, FileSaver, $state, $crypto, $mdDialog, $q, $rootScope, $uibModal,
        ModalFactory, LedgerFactory, WorkOrderFactory, ChallanTaxService, TaxService) {

        function applyInvoiceTotalRoundOff(cfg, billing, total) {
            var method = (cfg.roundOffTotalMethod || 'none').toString().toLowerCase();
            var autoRound = method !== 'none';
            if (!autoRound && billing.RoundOff !== true) {
                return total;
            }
            var kind = autoRound ? method : 'nearest';
            var t = Number(total);
            if (isNaN(t)) {
                return total;
            }
            if (kind === 'up') {
                return Math.ceil(t);
            }
            if (kind === 'down') {
                return Math.floor(t);
            }
            return Math.round(t);
        }

        var billKey = $stateParams.key == undefined ? 0 : $stateParams.key;
        $scope.ExistingBill = null;
        $scope.init = function () {
            $scope.Billing = new $.Billing({});

            $scope.Billing.DiscountPercent = 0;
            $scope.Billing.BreakageDiscountPercent = 0;
            $scope.Billing.LossDiscountPercent = 0;
            $scope.Billing.BreakageDiscount = 0;
            $scope.Billing.LossDiscount = 0;

            $scope.Billing.TotalLossAmount = 0;
            $scope.Billing.TotalBreakageAmount = 0;
            $scope.Billing.IsCashBill = false;
            $scope.Billing.FilterChallansByPO = false;
            $scope.BillData = null;
            $scope.Breakages = null;
            $scope.AllTaxes = null;
            // displaySave(false);
            var date = new Date();
            var token = $rootScope.getTokenInfo();

            $scope.LastBill = {};
        }
        $scope.init();
        // $scope.Billing = new $.Billing({});
        $scope.Billing.InvoiceId = $crypto.decrypt(billKey);

        var config = new $.Config({});
        $scope.Config = {
            BillPrefix: '',
            FreightTax: 0
            , allowRateEdits: false
            , BrekageBill: 1
            , discount_type: 'invoicelevel'
            , applyTaxOn: 'itemlevel'
            , defaultTaxRate: 0
            , autoRoundOffTaxable: false
            , allowEditBillNo: false
            , roundOffTotalMethod: 'none'
            , defaultRoundOffTotal: false
        };
        $scope.ApplyGST = true;
        $scope.ApplyFreightGST = true;
        $scope.ApplyOtherChargeGST = true;
        $scope.ItemTaxSettings = [];
        $scope._taxDataPromise = initSaleTaxData($scope, TaxService, ChallanTaxService);

        function loadCompanyForTaxEdit() {
            var token = $rootScope.getTokenInfo();
            if (!token || !token.DefaultCompanyId) {
                return;
            }
            var company = new $.Company();
            company.Props.CompanyId = token.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status == 200 && e.data) {
                    $scope.comp = e.data;
                }
            });
        }
        loadCompanyForTaxEdit();

        $scope.getById = function (e) {
            //$scope.Billing.ById(function (e) {
            //    if (e.data.Code != 200) {
            //        alert('Could not load invoice' + e.data.Message);
            //        return;
            //    }
            $scope.Billing = e.data.Data;
            $scope.BillData = $scope.Billing.Items = e.data.Data.BillableItems;
            var breakageFromServer = e.data.Data.BreakageItems || [];
            $scope.Billing.BreakageItems = breakageFromServer;
            $scope.Billing.Breakage = breakageFromServer;
             
            $scope.Billing.BreakageDamageDetails = e.data.Data.BreakageDamageDetails || e.data.Data.breakageDamageDetails || [];
            if ((breakageFromServer && breakageFromServer.length) || ($scope.Billing.BreakageDamageDetails && $scope.Billing.BreakageDamageDetails.length)) {
                $scope.Billing.IncludeBreakageItems = true;
            }
            $scope.Billing.From = convertDate(new Date(moment(e.data.Data.From)));
            $scope.Billing.To = convertDate(e.data.Data.To);
            $scope.ExistingBill = cloneObj($scope.Billing);
            $scope.ClosingBalance = e.data.Data.StockBalanceAfterBill;
            $scope.Billing.InvoiceDate = convertDate($scope.Billing.InvoiceDate);

            $scope.LossCharges = $scope.Billing.LostItems;
            $scope.Payments = $scope.Billing.Payments;

            $scope.Billing.PODate = convertDate($scope.Billing.PODate);
            $scope.Billing.PO = e.data.PO || $scope.Billing.PO || [];
            //}, { InvoiceId: $scope.Billing.InvoiceId });
        }
        //$scope.getById();


        function loadDetails() {
            var billing = new $.Billing();

            var cfg = config.GetBillingConfig(null, true);
            var bill = billing.ById(null, { InvoiceId: $scope.Billing.InvoiceId }, true);
            $q.all([cfg, bill])
                .then((e) => {
                    var response = e[0].data;
                    $scope.getById(e[1]);
                    if (response.Data != null && response.Data) {
                        if (response.Data.length > 0) {

                            var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                            var applyTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'applyTax');
                            // var breakageBill = response.Data.find(o => o.Key == 'breakageBill');
                            var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');
                            var billPrefix = response.Data.find(o => o.Key == 'Prefix' && o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST');
                            var tnc = response.Data.find(o => o.Key == 'tnc' && o.SubCategory == 'Other' && o.Category == 'Invoice');
                            var defaultRateType = response.Data.find(o => o.Key == 'defaultRateType');
                            var billReceivedDate = response.Data.find(o => o.Key == 'billReceivedDate');


                            if (billReceivedDate)
                                $scope.Billing.ChargeReturnDay = billReceivedDate.Value == 'true' ? 1 : 0;

                            if (defaultRateType) {
                                $scope.Billing.RateCalcType = defaultRateType.Value == 'day' ? 1 : 2;
                            }

                            if (applyTax) {
                                $scope.Billing.applyTax = applyTax.Value == 'true';
                            }

                            if (freightTax) {

                                $scope.Config.FreightTax = freightTax.Value
                                if (applyTax) {
                                    if (applyTax.Value != 'true') {
                                        $scope.Config.FreightTax = 0;
                                    }
                                }
                            }
                            //if (breakageBill) {
                            //    $scope.Config.BrekageBill = breakageBill.Value == 'true' ? 1 : 0;
                            //}
                            if (allowEditBillNo) {
                                $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                            }
                            if (billPrefix) {
                                $scope.Config.billPrefix = billPrefix.Value;
                            }
                            if (tnc) {
                                $scope.Billing.Tnc = tnc.Value;
                            }
                            var allowRateEdits = response.Data.find(o => o.Key.toLowerCase() == 'allowrateedits');
                            if (allowRateEdits) {
                                $scope.Config.allowRateEdits = allowRateEdits.Value == 'true' || allowRateEdits.Value == '1';
                            }
                            var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');
                            if (discount_type) {
                                $scope.Config.discount_type = discount_type.Value;
                            }
                            loadBillingTaxConfigExtras(response, $scope.Config);
                            var roundOffTotalMethod = response.Data.find(o => o.Key == 'roundOffTotalMethod');
                            if (roundOffTotalMethod && roundOffTotalMethod.Value) {
                                var rm = ('' + roundOffTotalMethod.Value).toLowerCase();
                                if (rm === 'none' || rm === 'nearest' || rm === 'up' || rm === 'down') {
                                    $scope.Config.roundOffTotalMethod = rm;
                                }
                            }
                            var defaultRoundOffTotal = response.Data.find(o => o.Key == 'defaultRoundOffTotal');
                            if (defaultRoundOffTotal) {
                                $scope.Config.defaultRoundOffTotal = defaultRoundOffTotal.Value == '1' || defaultRoundOffTotal.Value === true || defaultRoundOffTotal.Value === 'true';
                            }
                        }
                    }

                    if ($scope.Billing && $scope.Billing.IsCashBill) {
                        $scope.Billing.applyTax = false;
                    }
                    $scope.ApplyGST = $scope.Billing.applyTax !== false;

                    $scope.BillGenerated = true;
                    $scope._taxDataPromise.then(function () {
                        loadSavedGenBillInvoiceTaxes($scope.Billing.InvoiceId, $scope, ChallanTaxService, function () {
                            $scope.CalSubTotal();
                        });
                    });
                    if ($scope.BillData && $scope.BillData.length > 0) {
                        displaySave(true);
                    }
                });
        }
        loadDetails();
        FormsValidation.init();

        $scope.$watch('Billing.IsCashBill', function (cash) {
            if (cash) {
                $scope.Billing.applyTax = false;
                $scope.ApplyGST = false;
            }
        });

        $scope.$watch('ApplyGST', function () {
            if ($scope.CalSubTotal) {
                $scope.CalSubTotal();
            }
        });

        function displaySave(display) {
            if (display) {
                $('#btnSave').show();
            } else {
                $('#btnSave').hide();
            }
        }


        var ledger = new $.Ledger({});
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Billing.LedgerId == null) {
                $scope.Billing.LedgerId = e.data[0].LedgerId;
            }
        });



        //select default ledger if it selected on some other screen
        //if ($rootScope.LedgerId) {
        //    $scope.Billing.LedgerId = $rootScope.LedgerId;
        //}
        $scope.BillGenerated = false;

        $scope.showBreakageBalanceTab = function () {
            if (!$scope.Billing) return false;
            var inc = $scope.Billing.IncludeBreakageItems === true || $scope.Billing.IncludeBreakageItems === 1;
            return inc && !!$scope.BillGenerated;
        };

        $scope.GenBill = function () {


            if (!validateForm()) {
                return;
            }
            var billing = new $.Billing();

            var modal = cloneObj($scope.Billing);
            modal.From = formatdate(modal.From);
            modal.To = formatdate(modal.To);
            if (modal.FilterChallansByPO && (!modal.PONumber || modal.PONumber.trim() === '')) {
                alert('Please enter PO# to filter challans by PO.');
                return;
            }


            billing.GenerateBill(function (e) {
                if (e.data.Code != undefined) {
                    alert(e.data.Description);
                    return;
                }

                $scope.BillGenerated = true;

                $scope.BillData = $scope.Billing.Items = rentBillItemsOnly(e.data.BillingItems);
                $scope.Billing.Challans = e.data.Challans;
                $scope.Billing.Breakage = e.data.Breakage;
                $scope.Billing.BreakageDamageDetails = e.data.BreakageDamageDetails || e.data.breakageDamageDetails || [];
                $scope.ClosingBalance = e.data.StockBalanceAfterBill;
                $scope.Billing.applyTax = e.data.ApplyTax;
                if ($scope.Billing.IsCashBill) {
                    $scope.Billing.applyTax = false;
                }
                $scope.ApplyGST = $scope.Billing.applyTax !== false;
                $scope.Billing.AccountLedger = e.data.AccountLedger;
                $scope.LossCharges = e.data.LostItems;
                $scope.Billing.OtherCharges = e.data.OtherCharges;
                $scope.Billing.PO = e.data.PO || [];
                // applyAllTaxes();
                if ($scope.BillData.length > 0) {
                    $scope.Billing.Freight = $scope.BillData[0].Freight;
                    displaySave(true);
                    //$scope.getBreakageForBill();
                }
                $scope._taxDataPromise.then(function () {
                    $scope.CalSubTotal();
                });
            }, modal);
            // getStockBalance();
            //   LedgerFactory.GetAccountBalanceForBill($scope.Billing, getLedger);
        }

        $scope.removeGenBillPo = function (index) {
            if (!$scope.Billing || !$scope.Billing.PO || !angular.isArray($scope.Billing.PO)) {
                return;
            }
            if (index < 0 || index >= $scope.Billing.PO.length) {
                return;
            }
            $scope.Billing.PO.splice(index, 1);
        };

        //$scope.applyAllTaxes = function () {
        //    for (var i = 0; i < $scope.Billing.Taxes.length; i++) {

        //        var tax = $scope.Billing.Taxes[i];
        //        if (tax.Rate > 0) {
        //            tax.Applicable = true;
        //        }
        //        $scope.ApplyTax(tax);
        //    }

        //}


        // $scope.CurrentBill = new $.Billing({BillNumber:'<NEW>'});
        $scope.SaveBill = function () {
            if ($scope.BillGenerated == false) {
                alert('Please generate the bill to save');
                return;
            }
            //   $('#billChangesDialog').modal('show');
            var c = confirm('Are you sure you want to update the invoice');
            if (!c) {
                return;
            }
            if (!validateForm()) {
                return;
            }
            $scope.CalSubTotal();
            $scope.Billing.LossItems = $scope.LossCharges;
            if ($scope.Billing.BreakageDamageDetails && $scope.Billing.BreakageDamageDetails.length) {
                $scope.Billing.BreakageItems = $scope.Billing.Breakage || $scope.Billing.BreakageItems || [];
            }
            var billing = new $.Billing();
            var model = cloneObj($scope.Billing);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            model.InvoiceDate = formatdate(model.InvoiceDate);
            model.Tnc = htmlEncode(model.Tnc);
            if (model.PODate)
                model.PODate = formatdate(model.PODate);

            if ($scope.Payments) {
                var payments = cloneObj($scope.Payments.filter(o => o.PaidAmount > 0));
                $.each(payments, function (index, value) {
                    if (value.PaidAmount > 0) {
                        value.TransactionAmount = value.PaidAmount;
                    }
                });
                model.Payments = payments;
            }

            billing.SaveBill(function (e) {

                if (e.data.Code == 200) {
                    $scope.CurrentBill = e.data;
                    alert('Bill generated successfully');

                    chooseAndPrintBill(e.data.Data.InvoiceId);
                    $state.go('billList');
                } else {
                    alert(e.data.Message);
                }

            }, model);
        }



        function chooseAndPrintBill(invoiceId) {
            //ModalFactory.BillType(function ($scope, $mdDialog) {
            //    $scope.closeDialog = function () {
            //        $mdDialog.hide();
            //    },
            //        $scope.OkButtonClick = function () {
            //            $(event.currentTarget).find('.fa-spin').show();
            //            printBill($scope.HeaderType, invoiceId);
            //        }
            //});
            var encrypedText = $crypto.encrypt(invoiceId);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReportFromHtml(function (e) {
                FileSaver.saveAs(e.data, $scope.CurrentBill.Data.InvoiceNumber + '.pdf');
                $state.go('billList');
            }, 'PrintRentBill', econded);
        }

        function printBill(headerType, invoiceId) {

            //$scope.Billing.InvoiceId = invoiceId;
            //$scope.Billing.BillCopyType = headerType;
            ////  $scope.Billing.InvoiceNumber = e.data.InvoiceNumber;
            //$scope.Billing.PrintBill(function (e) {
            //    $mdDialog.hide();
            //    var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            //    // $window.target = '_blank';
            //    $window.open(filePath);
            //});

            //$route.reload();
        }
        //--Loading site information
        var _siteNames = [];
        _siteNames.push({
            JobNumber: '',
            SiteId: 0
        });
        var site = new $.Site({});



        $scope.CalculateSum = function () {
            var total = 0;
            if ($scope.BillData != undefined) {
                for (var i = 0; i <= $scope.BillData.length - 1; i++) {
                    var v = $scope.BillData[i];
                    total += parseFloat($scope.BillData[i].Amount);
                    //   total+=(v.ClosingBalance * (v.ChargeReturnedDate? (v.Days + 1) : v.Days) * v.Rate) ;
                }
            }

            $scope.Billing.SubTotal = total;
            return total;
        }

        $scope.fnOtherChargesTotal = function () {

            var otherCharge = 0;
            if ($scope.Billing.OtherCharges != null) {
                $.each($scope.Billing.OtherCharges, function () {
                    otherCharge += this.Amount;
                });
            }
            $scope.TotalOtherCharges = otherCharge;
            return otherCharge;
        }
        $scope.CalSubTotal = function () {
            if (!$scope.Billing.Items) {
                return;
            }
            runGenBillSubtotal($scope, ChallanTaxService);
            $scope.Billing.Total = applyInvoiceTotalRoundOff($scope.Config, $scope.Billing, $scope.Billing.Total);
            if (($scope.Config.roundOffTotalMethod || 'none').toString().toLowerCase() !== 'none') {
                $scope.Billing.RoundOff = true;
            }
            return $scope.Billing.Total;
        }

        //$scope.GetTaxes = function () {
        //    var Tax = new $.Tax({
        //        ItemId: 0
        //    });
        //    $scope.WorkOrder = new $.WorkOrder({});
        //    //Tax.GetTaxes($scope.WorkOrder, function (e) {

        //    //    $scope.Billing.Taxes = e.data;
        //    //});
        //    if ($scope.Billing.LedgerSiteId > 0) {
        //        LedgerFactory.GetSiteTaxes(function (e) {
        //            $scope.Billing.Taxes = e.data.Data;
        //        }, {
        //            LedgerSiteId: $scope.Billing.LedgerSiteId
        //        });
        //    }
        //    Tax.GetAllTaxes(function (e) {
        //        $scope.AllTaxes = e.data;
        //    });
        //}
        $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
        $scope.Billing.FreightTax = 0;
        $scope.Billing.BreakageTax = 0;
        //$scope.ApplyTax = function (o) {
        //    var taxId = o.TaxId;

        //    //var itemsToApplyTax = $scope.AllTaxes.find(n=>n.TaxId == o.tax.TaxId);
        //    var itemsToApplyTax = jQuery.grep($scope.AllTaxes, function (n, i) {
        //        return (n.TaxId == taxId);
        //    });
        //    o.TaxAmount = 0;
        //    if (!o.Applicable) {

        //        $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = jQuery.grep($scope.Billing.AppliedTaxes, function (n, i) {
        //            return (n.TaxId != taxId);
        //        });
        //        return;
        //    }
        //    for (var i = 0; i < itemsToApplyTax.length; i++) {
        //        var billingItems = jQuery.grep($scope.BillData, function (n, j) {
        //            return (n.ProductId == itemsToApplyTax[i].ItemValue);
        //        });
        //        if (billingItems && billingItems.length > 0) {
        //            for (var j = 0; j < billingItems.length; j++) {
        //                var txAmount = (billingItems[j].Amount * itemsToApplyTax[i].Rate / 100.0);
        //                o.TaxAmount += txAmount
        //                var appliedTax = {
        //                    'TaxId': taxId,
        //                    'TaxName': o.Name,
        //                    'Item': billingItems[j].Item,
        //                    'ProductId': billingItems[j].ProductId,
        //                    'Amount': billingItems[j].Amount,
        //                    'TaxRate': itemsToApplyTax[i].Rate,
        //                    'TaxAmount': txAmount
        //                };
        //                $scope.addToAppliedTaxes(appliedTax);
        //            }
        //        }
        //    }



        //};

        //$scope.addToAppliedTaxes = function (o) {
        //    var taxes = $scope.Billing.AppliedTaxes;
        //    var updated = false;

        //    //debugger
        //    for (var i = 0; i < taxes.length; i++) {
        //        if (o.ProductId == taxes[i].ProductId && o.TaxId == parseInt(taxes[i].TaxId)) {
        //            taxes[i].Amount += o.Amount;
        //            taxes[i].TaxAmount += o.TaxAmount;
        //            updated = true;
        //        }

        //    }
        //    if (!updated) {
        //        taxes.push(o);
        //    }
        //    $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = taxes;
        //}


        $scope.getBreakageForBill = function () {

            var billing = new $.Billing();
            var model = cloneObj($scope.Billing);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            var filter = {
                From: model.From, To: model.To, LedgerId: model.LedgerId, LedgerSiteId: model.LedgerSiteId,
                FinYearId: model.FinYearId, InvoiceId: model.InvoiceId, CompanyId: model.CompanyId
            };
            billing.GetBreakageForBill(function (e) {
                $scope.Billing.Breakage = e.data;
            }, filter);
        }
        $scope.totalBreakageAmount = function () {
            var bAmount = 0;
            if ($scope.Billing.Breakage) {
                for (var i = 0; i <= $scope.Billing.Breakage.length - 1; i++) {
                    bAmount += ($scope.Billing.Breakage[i].Rate * $scope.Billing.Breakage[i].Quantity);
                }
            }
            return bAmount;
        }
        $scope.TotalLossCharges = function () {
            var bAmount = 0;
            if ($scope.LossCharges) {
                for (var i = 0; i <= $scope.LossCharges.length - 1; i++) {
                    bAmount += ($scope.LossCharges[i].Rate * $scope.LossCharges[i].Quantity);
                }
            }
            return bAmount;
        }
        $scope.taxableValueBase = function () {
            $scope.Billing.Taxable = computeRentBillTaxable($scope.Billing, $scope.Config, $scope.TotalOtherCharges, $scope.LossCharges);
        };
        $scope.onBreakageDiscountPercentChange = function () {
            var breakageAmount = $scope.totalBreakageAmount();
            $scope.Billing.BreakageDiscount = 0;
            $scope.Billing.TotalBreakageAmount = 0;
            if (breakageAmount > 0) {
                $scope.Billing.BreakageDiscount = (breakageAmount * $scope.Billing.BreakageDiscountPercent / 100)

            }
            $scope.calcBreakageTotal();
        }
        $scope.onBreakageDiscountChange = function () {
            var breakageAmount = $scope.totalBreakageAmount();
            $scope.Billing.BreakageDiscountPercent = 0;
            $scope.calcBreakageTotal();
        }
        $scope.onLossDiscountPercentChange = function () {
            var lossAmount = $scope.TotalLossCharges();
            $scope.Billing.LossDiscount = 0;
            //   $scope.Billing.TotalLossAmount = 0;
            if (lossAmount > 0) {
                $scope.Billing.LossDiscount = (lossAmount * $scope.Billing.LossDiscountPercent / 100)
                // $scope.Billing.TotalLossAmount = lossAmount - $scope.Billing.LossDiscount;
            }
            $scope.calcLossTotal();
        }
        $scope.onLossDiscountChange = function () {

            var lossAmount = $scope.TotalLossCharges();
            $scope.Billing.LossDiscountPercent = 0;
            $scope.calcLossTotal();
            //  $scope.Billing.TotalLossAmount = 0;
            //if (lossAmount > 0) {

            //    $scope.Billing.TotalLossAmount = lossAmount - $scope.Billing.LossDiscount;
            //}
        }
        $scope.onDiscountPercentChange = function () {
            $scope.CalSubTotal();
        }
        $scope.onDiscountChange = function () {
            $scope.Billing.DiscountPercent = 0;
            $scope.CalSubTotal();
        }
        $scope.$watch('Billing.LedgerId', function () {
            if ($scope.Billing.LedgerId == 0)
                return;

            ledger.Props.LedgerId = ledger.LedgerId = $scope.Billing.LedgerId;
            if (ledger.LedgerId == 0) return;
            // getStockBalance();
            //  LedgerFactory.GetAccountBalanceForBill(getLedger, ledger);
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
            getAllProducts();
            getLastBill();
        });

        //function getStockBalance() {
        //    if ($scope.Billing.LedgerId <= 0) {
        //        return;
        //    }
        //    ledger.StockBalance(function (e) {
        //        $scope.ClosingBalance = e.data;
        //    }, $scope.Billing);
        //}
        function getStockBalance() {
            if ($scope.Billing.LedgerId <= 0) {
                return;
            }
            var model = cloneObj($scope.Billing);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);

            if ($scope.Billing.LedgerId <= 0) {
                return;
            }
            ledger.StockBalance(function (e) {
                $scope.ClosingBalance = e.data;
            }, model);
        }

        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, {
                LedgerId: $scope.Billing.LedgerId
            });
        }
        $scope.$watch('Billing.LedgerSiteId', function () {
            if ($scope.Billing.LedgerSiteId == 0 || $scope.Billing.LedgerSiteId == undefined) {
                return;
            }
            //  getStockBalance();
            getLastBill();
            // $scope.GetTaxes();
            //$scope.getSiteBalanceOtherCharges();
        })

        function getLedger(e) {
            $scope.Billing.OutStanding = 0;
            $scope.Billing.OutStandingType = 'Cr';
            if (e.data != null) {
                //$scope.TransList = e.data;
                //  $scope.LastTransRow = e.data[e.data.length - 1];
                $scope.Billing.OutStanding = e.data.Closingbalance;
                $scope.Billing.OutStandingType = $scope.Billing.OutStanding >= 0 ? 'Cr' : 'Dr';
            }
        }

        function getAllProducts() {
            ledger.GetProductRates(function (e) {
                $scope.ProductRates = e.data;
            });

        }

        function getLastBill() {
            LedgerFactory.GetLastBill(function (e) {
                $scope.LastBill = e.data.Data;

            }, {
                LedgerId: $scope.Billing.LedgerId,
                LedgerSiteId: $scope.Billing.LedgerSiteId
            });
        }

        $scope.getSiteBalanceOtherCharges = function () {
            WorkOrderFactory.GeSiteOtherBalanceCharges($scope.Billing.LedgerSiteId, function (e) {
                $scope.Billing.OtherCharges = e.data;
            });
        }

        $scope.LossCharges = [];
        $scope.$watch('LossItem.ProductId', function () {
            if (!$scope.LossItem) {
                return;
            }
            var rate = $scope.getItemRate($scope.LossItem.ProductId);
            if (rate) {
                $scope.LossItem.Rate = rate.LossRate;
            }
        });
        $scope.getItemRate = function (productId) {
            if ($scope.ProductRates) {
                return $scope.ProductRates.find(o => o.ProductId == productId);
            }
        }
        $scope.AddLossItem = function () {
            var item = $scope.ProductRates.find(o => o.ProductId == $scope.LossItem.ProductId);
            if (item) {
                $scope.LossItem.Product = item.Product;
            }
            $scope.LossCharges.push($scope.LossItem);
            $scope.LossItem = {};
        }
        $scope.DeleteLossItem = function (index) {
            $scope.LossCharges.splice(index, 1);
        }
        var modal;
        $scope.showTaxItems = function () {
            modal = $uibModal.open({
                windowClass: 'right',
                templateUrl: 'templ/taxItems.html',
                scope: $scope, //passed current scope to the modal
                size: 'lg'

            });
        }
        $scope.closeSliderModal = function () {
            modal.close({});
        }

        $scope.ApplyExtraDays = function (item) {
            if (item.ChargeReturnedDate) {
                item.Days++;
            } else {
                item.Days--;
            }
            if (item.ItemCategory == 1012) {
                item.Amount = (item.ClosingBalance * item.Days * item.Rate);
            } else {
                item.Amount = item.ClosingBalance * item.Rate;
            }
        }
        $scope.ApplyRate = function (item) {

            if (item.ItemCategory == 1012) {
                item.Amount = (item.ClosingBalance * item.Days * item.Rate);
            } else { //consumeables
                item.Amount = item.ClosingBalance * item.Rate;
            }
        }
        function validateForm() {
            return $('#form-bill').valid();
        }

        $scope.openBillTaxSettings = function () {
            $scope.ApplyGST = $scope.Billing.applyTax !== false;
            var div = '<div></div>';
            $(div).load('templ/dialogs/billTaxSettings.dialog.html?d=' + new Date().getTime(), function () {
                var html = $(this).html();
                $mdDialog.show({
                    clickOutsideToClose: true,
                    skipHide: true,
                    scope: $scope,
                    preserveScope: true,
                    template: html,
                    parent: angular.element(document.body),
                    controller: ['$scope', '$mdDialog', function ($dialogScope, $mdDialog) {
                        $dialogScope.closeBillTaxSettingsDialog = function () {
                            $mdDialog.hide();
                        };
                        $dialogScope.confirmBillTaxSettings = function () {
                            if ($dialogScope.Billing && $dialogScope.Billing.IsCashBill) {
                                $dialogScope.Billing.applyTax = false;
                            }
                            $dialogScope.ApplyGST = $dialogScope.Billing.applyTax !== false;
                            if (typeof $dialogScope.CalSubTotal === 'function') {
                                $dialogScope.CalSubTotal();
                            }
                            $mdDialog.hide();
                        };
                    }]
                });
            });
        };

        $scope.cancel = function () {
            if ($scope.localData) {
                $scope.onBillSavedOnDialog();
                return;
            }
            else {
                $state.go('billList');
            }
        }
        $scope.$watch('LossCharges', function (e, y) {
            if (!e) {
                return;
            }
            $.each(e, function (index, value) {
                value.Amount = parseFloat(value.Quantity) * parseFloat(value.Rate);
            });
            if ($scope.LossCharges) {
                $scope.Billing.LossIGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.IGST) || 0), 0);
                $scope.Billing.LossSGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.SGST) || 0), 0);
                $scope.Billing.LossCGST = $scope.LossCharges.reduce((partialSum, a) => partialSum + (parseFloat(a.CGST) || 0), 0);
            }

            $scope.calcLossTotal();
            $scope.CalSubTotal();

        }, true);
        $scope.calcLossTotal = function () {
            if (!$scope.LossCharges) {
                return;
            }
            // $scope.Billing.TotalLossAmount = $scope.LossCharges.reduce((partialSum, a) => partialSum + a.Amount + a.CGST + a.SGST + a.IGST, 0);
            $scope.Billing.TotalLossAmount = $scope.LossCharges.reduce((partialSum, a) => partialSum + a.Amount, 0);

            if ($scope.Billing.LossDiscount) {
                $scope.Billing.TotalLossAmount = $scope.Billing.TotalLossAmount - $scope.Billing.LossDiscount;
            }
        }
        $scope.calcBreakageTotal = function () {
            if (!$scope.Billing.Breakage) {
                return;
            }
            $scope.Billing.TotalBreakageAmount = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + a.Amount + a.CGST + a.SGST + a.IGST, 0);
            if ($scope.Billing.BreakageDiscount) {
                $scope.Billing.TotalBreakageAmount = $scope.Billing.TotalBreakageAmount - $scope.Billing.BreakageDiscount;
            }
        }
        $scope.$watch('Billing.Breakage', function (e, y) {
            if (!e) {
                return;
            }
            var useDamageDetails = $scope.Billing.BreakageDamageDetails && $scope.Billing.BreakageDamageDetails.length;
            $.each(e, function (index, value) {
                value.Amount = parseFloat(value.Quantity) * parseFloat(value.Rate);
                if (!useDamageDetails) {
                    value.IGST = value.IGSTRate * value.Amount / 100;
                    value.CGST = value.CGSTRate * value.Amount / 100;
                    value.SGST = value.SGSTRate * value.Amount / 100;
                }
            });
            if ($scope.Billing.Breakage) {
                $scope.Billing.Breakage.IGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.IGST) || 0), 0);
                $scope.Billing.Breakage.SGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.SGST) || 0), 0);
                $scope.Billing.Breakage.CGST = $scope.Billing.Breakage.reduce((partialSum, a) => partialSum + (parseFloat(a.CGST) || 0), 0);
            }
            $scope.calcBreakageTotal();
            $scope.CalSubTotal();

        }, true);
    }
]);
app.controller('POSBillignContainerController', function ($scope) {
    var vm = this;
    vm.Tabs = [{ name: 'Billing', id: crypto.randomUUID() }]
    vm.activeTab = 0;
    vm.newBilling = function () {
        if (vm.Tabs.length == 5) {
            alert('Only 5 screens can be added');
            return;
        }
        var id = vm.Tabs.length + 1;
        vm.Tabs.push({ name: 'Billing', id: crypto.randomUUID() });
    };
    vm.delBilling = function (index) {
        vm.Tabs.splice(index, 1);
    }
});
app.controller('POSBillignController', function ($scope, myCustomEditor) {

    var vm = this;
    vm.uniqueId = crypto.randomUUID().replace(/-/g, '');
    $scope.gridId = vm.gridId = vm.uniqueId;
    vm.itemsPopUpId = 'billPopup' + vm.uniqueId;

    vm.getAllProducts = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            vm.Items = e.data;
        });
    }
    vm.productConfig = {
        headers: [
            { key: 'Name', label: 'Name', sortable: true },
            { key: 'Code', label: 'Product Code', sortable: true },

        ],
        inputId: 'input' + vm.uniqueId,
        searchPlaceholder: 'Search products...',
        enableSearch: true,
        enableSort: true,
        enableActions: false,
        defaultSort: 'Name',
        defaultSortDirection: 'asc'
    };

    vm.getAllProducts();
    // Sample cart items
    vm.cartItems = [
        { id: 1, name: 'Laptop Dell XPS', code: 'ITM001', mbp: 1200, price: 999.99, quantity: 1 },
        { id: 2, name: 'Wireless Mouse', code: 'ITM002', mbp: 30, price: 25.99, quantity: 2 },
        { id: 3, name: 'Mechanical Keyboard', code: 'ITM003', mbp: 60, price: 45.99, quantity: 1 }
    ];

    vm.searchText = '';
    vm.amountReceived = 0;
    vm.BillItems = [];
    vm.selelectedItem;
    vm.SubTotal = 0;
    vm.TaxAmount = 0;
    vm.Total = 0;

    vm.onItemSelected = function (item) {
        vm.selelectedItem = item;
        $('#' + vm.itemsPopUpId).modal('hide');
        // vm.pushNewRow();
        // $scope.gridApi.setGridOption("rowData", vm.BillItems);
        //vm.focusGridCell(vm.selectedRowEvent.rowIndex, 'Name');
    }

    // Calculate subtotal
    vm.getSubtotal = function () {
        return vm.BillItems.reduce(function (total, item) {
            if (item && item.Quantity > 0 && item.ProductId > 0 && item.SalePrice > 0) {
                return total + (item.SalePrice * item.Quantity);
            }
            return total + 0;
        }, 0);
    };

    // Calculate tax (assuming 8% tax rate)
    vm.getTax = function () {
        return vm.getSubtotal() * 0.08;
    };

    // Calculate total
    vm.getTotal = function () {
        return vm.getSubtotal() + vm.getTax();
    };

    // Update item quantity
    vm.updateQuantity = function (item, newQuantity) {
        if (newQuantity >= 1) {
            item.quantity = newQuantity;
        } else if (newQuantity < 1) {
            // Remove item if quantity is 0
            var index = vm.cartItems.indexOf(item);
            if (index !== -1) {
                vm.cartItems.splice(index, 1);
            }
        }
    };



    // Keyboard shortcuts
    //document.addEventListener('keydown', function (event) {
    //    // CTRL+1 - Focus on search
    //    if (event.ctrlKey && event.key === '1') {
    //        event.preventDefault();
    //        document.querySelector('input[ng-model="posCtrl.searchText"]').focus();
    //    }

    //    // CTRL+8 - Hold bill
    //    if (event.ctrlKey && event.key === '8') {
    //        event.preventDefault();
    //        alert('Bill held successfully!');
    //    }

    //    // F2 - Add discount
    //    if (event.key === 'F2') {
    //        event.preventDefault();
    //        alert('Add Discount dialog would open here');
    //    }

    //    // F3 - Add additional charge
    //    if (event.key === 'F3') {
    //        event.preventDefault();
    //        alert('Add Additional Charge dialog would open here');
    //    }

    //    // F4 - Focus on received amount
    //    if (event.key === 'F4') {
    //        event.preventDefault();
    //        document.querySelector('input[ng-model="posCtrl.amountReceived"]').focus();
    //    }

    //    // F5 - Save and print
    //    if (event.key === 'F5') {
    //        event.preventDefault();
    //        alert('Bill saved and sent to printer!');
    //    }

    //    // F7 - Save bill
    //    if (event.key === 'F7') {
    //        event.preventDefault();
    //        alert('Bill saved successfully!');
    //    }
    //});


    const myTheme = agGrid.themeQuartz.withParams({
        borderColor: "#00000054",
        headerHeight: "30px",
        headerTextColor: "#000",
        fontSize: 13,
        headerFontSize: 13,
        wrapperBorderRadius: 0,
        headerColumnBorder: true,
        rowHeight: 30,
        headerBackgroundColor: "#d7d7d7",

        headerCellMovingBackgroundColor: "rgb(80, 40, 140)",
        headerCellHoverBackgroundColor: "#98CCF8",
    });
    var gridApi;
    setTimeout(() => {

        $('#' + vm.itemsPopUpId).on('shown.bs.modal', function () {
            // Do something after the modal is fully visible


            $('#' + vm.productConfig.inputId).focus();

        });
        $('#' + vm.itemsPopUpId).on('hidden.bs.modal', function () {
            var rowIndex = vm.selectedRowEvent.rowIndex;
            var colKey = vm.selectedRowEvent.colDef.field;
            $scope.gridApi.setFocusedCell(rowIndex, colKey);
            vm.pushNewRow();
        });

    }, 300);
    //Grid Options: Contains all of the grid configurations
    vm.selectedRowEvent = 0;

    $scope.gridOptions = {
        theme: myTheme,

        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        onCellValueChanged: function (event) {

            // alert(event);
            // Update your underlying data array or object
            // For example: event.data[event.colDef.field] = event.newValue;
            event.data[event.colDef.field] = event.newValue
            // Optionally, if you have specific rendering logic or row styles,
            // you might need to refresh the specific cell or row.
            event.api.refreshCells({ force: true, rowNodes: [event.node] });


            vm.calculateTotal();
            $scope.$apply();


        },
        tabToNextCell: function (params) {
            const displayedColumns = params.api.getAllDisplayedColumns();
            const lastColIndex = displayedColumns.length - 1;
            const currentColumn = params.previousCellPosition.column;
            const currentRowIndex = params.previousCellPosition.rowIndex;
            const nextRowIndex = currentRowIndex + 1;

            // Check if it's the last column in the current row
            if (currentColumn.getColId() === displayedColumns[lastColIndex].getColId()) {

                // Check if there's a next row
                if (nextRowIndex < params.api.getDisplayedRowCount()) {
                    const firstColumn = displayedColumns[2];
                    return {
                        rowIndex: nextRowIndex,
                        column: firstColumn,
                        floating: params.floating
                    };
                } else {
                    // If it's the last cell of the last row, allow default browser tab behavior
                    return null;
                }
            } else {
                // Default AG Grid behavior for tabbing within the same row
                return params.nextCellPosition;
            }
        },
        //autoSizeStrategy: {
        //    type: 'fitCellContents',
        //},
        getRowStyle: params => {
            if (params.data.Status == 'Completed') {
                return { color: 'green', background: '#e3ffe3' };
            }
            if (params.data.Status == 'Delayed') {
                return { color: 'orange', background: '#fff5c49e' };
            }
        },
        loading: false,
        rowData: null,
        onCellKeyDown: (event) => {
            console.log('Key down on cell:', event.event.key);
            console.log('Cell details:', event.rowIndex, event.colDef.field, event.value);
            vm.selectedRowEvent = event;
            if (event.colDef.field == 'Name') {

                if ([9, 37, 38, 39, 40, 13, 27].includes(event.event.keyCode)) return;
                $('#' + vm.itemsPopUpId).modal('show');


            }
            // Example: Prevent default behavior for a specific key
            if (event.event.key === 'Enter') {
                event.event.preventDefault(); // Stop default Enter key behavior
                // Implement custom logic here, e.g., navigate to a specific cell
            }
        },
        // Columns to be displayed (Should match rowData properties)
        alwaysShowHorizontalScroll: true,
        columnDefs: [
            /* { headerName: "S.No", field: "no",width:60 },*/

            { field: "Code", width: 100, maxWidth: 100 },
            {
                field: "Name", width: 280, maxWidth: 280,
                editable: true
                //     cellEditor: myCustomEditor // Your custom cell editor component

            },
            {
                field: "Quantity", headerName: 'QTY', width: 70, editable: true,
                cellEditor: 'agNumberCellEditor',
                cellEditorParams: {
                    min: 0, // Optional: Set minimum allowed value
                    max: 999999,// Optional: Set maximum allowed value
                    precision: 2 // Optional: Set number of decimal places allowed
                },
                valueFormatter: (params) => {
                    if (typeof params.value === 'number') {
                        return params.value.toFixed(2); // Formats the number to two decimal places
                    }
                    return params.value; // Returns the value as is if not a number
                },
                valueSetter: params => {

                    // Update the underlying data
                    params.data[params.colDef.field] = params.newValue;
                    // Notify the grid that the row data has changed
                    params.node.setData(params.data);
                    return true; // Indicate that the value was set successfully
                }
            },
            {
                field: "free", headerName: 'FREE', width: 70, editable: true,
                valueFormatter: (params) => {
                    if (typeof params.value === 'number') {
                        return params.value.toFixed(2); // Formats the number to two decimal places
                    }
                    return params.value; // Returns the value as is if not a number
                },
                cellEditorParams: {
                    min: 0, // Optional: Set minimum allowed value
                    max: 999999,// Optional: Set maximum allowed value
                    precision: 2 // Optional: Set number of decimal places allowed
                },
            },
            {
                field: "SalePrice", headerName: 'RATE', width: 80, editable: true,
                valueFormatter: (params) => {
                    if (typeof params.value === 'number') {
                        return params.value.toFixed(2); // Formats the number to two decimal places
                    }
                    return params.value; // Returns the value as is if not a number
                },
                cellEditorParams: {
                    min: 0, // Optional: Set minimum allowed value
                    max: 999999,// Optional: Set maximum allowed value
                    precision: 2 // Optional: Set number of decimal places allowed
                },
            },
            { field: "Discount", width: 120, editable: true },

            {
                field: "Amount", headerName: "AMOUNT", width: 100,
                valueGetter: params => {
                    var val = parseFloat(params.data.Quantity) * parseFloat(params.data.SalePrice);
                    if (isNaN(val)) {
                        return 0;
                    }
                    return parseFloat(params.data.Quantity) * parseFloat(params.data.SalePrice);
                },
                valueFormatter: (params) => {
                    if (typeof params.value === 'number') {
                        return params.value.toFixed(2); // Formats the number to two decimal places
                    }
                    return params.value; // Returns the value as is if not a number
                }
            }


        ],

        defaultColDef: {
            enterNavigatesVertically: false,
            enterNavigatesVerticallyAfterEdit: false,

            resizable: false,
            sortable: false,
            domLayout: null,
        },
    };
    vm.pushNewRow = function () {
        var existingItem;
        if (vm.selelectedItem) {
            existingItem = vm.BillItems.find(function (cartItem) {
                return cartItem.ProductId === vm.selelectedItem.ProductId;
            });
            if (existingItem) {
                existingItem.Quantity++;
            } else {
                vm.selelectedItem.Quantity = 1;
                vm.BillItems[vm.selectedRowEvent.rowIndex] = vm.selelectedItem;
            }
        }
        vm.emptyRow = {};
        var totalRows = vm.BillItems.length;
        var lastRow = vm.BillItems[vm.BillItems.length - 1];
        if (totalRows == 0) {
            vm.BillItems.push(vm.emptyRow);
        }
        else if (lastRow && lastRow.ProductId > 0) {
            //validate last row properly          
            vm.BillItems.push(vm.emptyRow);
        }
        $scope.gridApi.setGridOption("rowData", vm.BillItems);
        var lastRowIndex = vm.BillItems.length - 1;
        vm.focusGridCell(lastRowIndex, 'Name');
        vm.calculateTotal();
        $scope.$apply();
    }
    setTimeout(() => {
        $scope.gridApi = agGrid.createGrid(document.querySelector("#_" + vm.gridId), $scope.gridOptions);
        vm.pushNewRow();
    }, 200);

    vm.focusGridCell = function (rowIndex, colKey) {

        $scope.gridApi.setFocusedCell(rowIndex, colKey);
        //$scope.gridApi.startEditingCell({
        //    rowIndex: rowIndex,
        //    colKey: colKey,
        //});
    }

    vm.calculateTotal = function () {
        vm.SubTotal = vm.getSubtotal();
    }
});

//recurring invoices
app.controller('RecurringInvoiceController', function ($scope, $rootScope, $stateParams, $state, $http, $crypto, $uibModal, LedgerFactory, AuthenticationService) {

    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 10 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.Charge5 = 0;
    $scope.Trans.Items = [];//initializeArray();
    init();
    var quoteId = 0;
    if ($stateParams.qId) {
        quoteId = $crypto.decrypt($stateParams.qId);
    }
    if (quoteId > 0) {
        var txn = new $.Transaction();
        txn.GetQutotationById(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Trans = e.data.Data;
            $scope.Trans.InvoiceDate = convertDate($scope.Trans.QuotationDate);
            $scope.Trans.InvoiceType = sale.InvoiceType;
            $.each($scope.Trans.Items, function (index, value) {
                value.AddInfo = "";
                value.Tnc = "";
            });

            $scope.Trans.AddInfo = "";
            $scope.Trans.Tnc = "";

        }, quoteId);
    }
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    //LedgerFactory.GetAccountsByGroup(function (e) {
    //    $scope.Banks = e.data;

    //}, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    LedgerFactory.GetAllParties(function (e) {

        $scope.SundryDebtors = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }
    $scope.$watch('Trans.LedgerId', function () {

        $rootScope.LedgerId = $scope.Trans.LedgerId;
        if ($scope.SundryDebtors) {
            var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
            if (ledger) {
                ledgerDTO.Props.StateCode = ledger.StateCode
            }
        }
        getSites();
    });

    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        // console.log(selected);
    };

    $scope.RowSelected = function (index) {
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
            //  $scope.$digest(function () {
            selectedWorkOrderItemIndex = index;
            //  $scope.Product = $scope.WorkOrder.Items[index].Item.Props;
            // });
        }
        //LoadSites(index, 0);

    };

    $scope.GetSelected = function () {
        var v = $scope[selectedProject];
    };



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0 });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.InvoiceDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    function addWorkOrder(fileList) {
        var model = cloneObj($scope.Trans);
        var txn = new $.Transaction();
        if (model.LedgerSiteId > 0) {
            if (model.ShipTo && model.ShipTo.length > 10) {
                alert('Either select site or enter custom ship to address, can not enter both');
                return;
            }
        }
        if (model.LedgerSiteId == 0 && model.ShipTo.length < 3) {
            alert('Either select shipping site or enter custom ship to address');
            return;
        }
        txn.SaveSales(function (e) {
            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billList');
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {

        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }
        var billItem = cloneObj($scope.TransItem);
        $scope.addItemToBill(billItem);




        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0 });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }
    $scope.addItemToBill = function (billItem) {
        var itemExist = $scope.Trans.Items.find(o => o.ProductId == billItem.ProductId && o.Rate == billItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt(billItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            billItem.SubTotal = billItem.Quantity * billItem.Rate;

            $scope.Trans.Items.push(billItem);
        }

        if (billItem.BOM != null) {
            $.each(billItem.BOM, function (index, value) {

                value.Quantity = value.Quantity * billItem.Quantity;
                value.Rate = 0;
                value.Item = value.Product;
                value.GroupItemId = billItem.ProductId;
                $scope.addItemToBill(value);
            });
        }
    }
    $scope.applyDiscount = function (item) {
        debugger
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            // console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    /*
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {
            //  $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    if ($scope.Trans.Items[i].Quantity != null) {
                        $scope.Trans.Items[i].SubTotal = parseFloat($scope.Trans.Items[i].Quantity) * parseFloat($scope.Trans.Items[i].Rate);
                    }
                    debugger
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
                $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0)

            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.OtherChargeAmount = parseFloat($scope.Trans.Charge1) + parseFloat($scope.Trans.Charge2) + parseFloat($scope.Trans.Charge3)
            + parseFloat($scope.Trans.Charge4) + parseFloat($scope.Trans.Charge5);

        // if ($scope.Config) {
        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        } else {
            $scope.Trans.FreightTaxRate = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.FreightTax;
        } else {
            $scope.Trans.ChargesTaxRate = 0;
        }

        if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
            var freight = parseFloat($scope.Trans.Freight);
            $scope.Trans.FreightTax = (freight * $scope.Trans.FreightTaxRate) / 100
        }


        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * $scope.Trans.ChargesTaxRate) / 100
        }
        //   }
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight) + parseFloat($scope.Trans.FreightTax) +
            $scope.Trans.OtherChargeAmount + $scope.Trans.ChargesTax;

        // $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };

    */
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.SalePrice;
            $scope.TransItem.TaxCategoryId = selected.originalObject.TaxCategoryId;

            $scope.getBomItems($scope.TransItem.ProductId);
        }
    };

    $scope.getBomItems = function (productId) {
        var p = new $.Product();
        p.BOMByProductId(function (e) {
            if (e.data.Code != 200) {
                alert('Could not featch BOM details');
                return;
            }
            $scope.TransItem.BOM = e.data.Data.Details;
        }, { ProductId: productId });
    }

    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        // $scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {

                    for (var i = 0; i < lineItems.length; i++) {
                        lineItems[i].IGST = 0;
                        lineItems[i].CGST = 0;
                        lineItems[i].SGST = 0;
                        lineItems[i].CGSTRate = 0;
                        lineItems[i].SGSTRate = 0;
                        lineItems[i].IGSTRate = 0;
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        if ($scope.comp.StateCode == ledgerDTO.Props.StateCode) {
                            lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                            lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                            lineItems[i].CGSTRate = tax.CGST;
                            lineItems[i].SGSTRate = tax.SGST;

                        }
                        else {
                            lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;
                            lineItems[i].IGSTRate = tax.IGST;
                        }

                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');

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
            $scope.comp = e.data;

        });
    }
    getInfo();

    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);
    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', editnumber: false, showDiscriptionColumn: false, showChallans: true
    };
    $scope.GetBillingConfig = function () {
        var config = new $.Config();
        config.GetBillingConfig(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                    if (freightTax) {
                        $scope.Config.FreightTax = $scope.Trans.FreightTaxRate = freightTax.Value
                    }

                    var showDiscriptionColumn = response.Data.find(o => o.Key == 'showDiscriptionColumn');
                    if (showDiscriptionColumn) {
                        $scope.Config.showDiscriptionColumn = showDiscriptionColumn.Value == 1;
                    }
                    var showChallans = response.Data.find(o => o.Key == 'showChallans');
                    if (showChallans) {
                        $scope.Config.showChallans = showChallans.Value == 1 || showChallans.Value == '1';
                    }
                }
            }
        });
    }
    $scope.$watch('ApplyFreightGST', function () {
        //  $scope.Config.FreightTax = 0;
        // debugger
        if ($scope.ApplyFreightGST == false) {
            $scope.Trans.FreightTaxRate = 0;
        }
        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        }

        $scope.SubTotal(0);
    }, true);

    $scope.$watch('ApplyOtherChargeGST', function () {
        // $scope.Config.ChargesTaxRate = 0;
        //$scope.Trans.ChargesTaxRate = 0;
        if ($scope.ApplyOtherChargeGST == false) {
            $scope.Trans.ChargesTaxRate = 0;
            //$scope.Trans.FreightTax = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate;
        }
        $scope.SubTotal(0);
    }, true);
    $scope.GetBillingConfig();


});
app.controller('EditRecurringInvoiceController', function ($scope, $rootScope, $stateParams, $state, $crypto,
    $q, $uibModal, LedgerFactory, AuthenticationService) {


    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 4, InvoiceId: 0 });

    sale.InvoiceId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.Trans.Items = [];//initializeArray();
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.Charge5 = 0;
    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', editnumber: false
    };
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    //LedgerFactory.GetAccountsByGroup(function (e) {
    //    $scope.Banks = e.data;

    //}, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    LedgerFactory.GetAllParties(function (e) {

        $scope.SundryDebtors = e.data;
        $scope.getById();
    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }
    $scope.$watch('Trans.LedgerId', function () {
        if ($scope.Trans.LedgerId == 0) return;
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode
        }
        getSites();
    });
    $scope.getById = function () {

        var billing = new $.Billing();
        var config = new $.Config();
        var billConfig = config.GetBillingConfig(null, true);
        var billById = billing.ById(null, { InvoiceId: $scope.Trans.InvoiceId }, true);

        var allPromises = [billConfig, billById];
        $q.all(allPromises).then(function (responses) {
            var configResponse = responses[0];
            var billingResponse = responses[1];
            if (configResponse) {
                $scope.SetBillingConfig(configResponse.data);
            }
            if (billingResponse) {
                if (billingResponse.data.Code != 200) {
                    alert('Could not load invoice' + billingResponse.data.Message);
                    return;
                }
                $scope.Trans = billingResponse.data.Data;
                $scope.Trans.Items = billingResponse.data.Data.BillableItems
                $scope.Trans.InvoiceDate = convertDate(billingResponse.data.Data.InvoiceDate)

                $scope.AddInfo = $scope.Trans.AddInfo;
                $scope.qtnc = $scope.Trans.Tnc;
                $scope.Trans.FreightTaxRate = 0;

                if ($scope.Trans.ChargesTax > 0) {

                    $scope.ApplyOtherChargeGST = true;
                }

                if ($scope.Trans.FreightTax > 0) {
                    $scope.ApplyFreightGST = true;

                }
            }


        }).catch(function (error) {
            // Handle any errors that occurred in any of the requests
            console.error('One or more requests failed:', error);
        });
    }



    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        //console.log(selected);
    };

    $scope.RowSelected = function (index) {
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
            //  $scope.$digest(function () {
            selectedWorkOrderItemIndex = index;
            //  $scope.Product = $scope.WorkOrder.Items[index].Item.Props;
            // });
        }
        //LoadSites(index, 0);

    };

    $scope.GetSelected = function () {
        var v = $scope[selectedProject];
    };



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    $scope.init = function () {
        $scope.TransItem = new $.TransItem();
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.InvoiceDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.init();
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        //   EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {

        var model = cloneObj($scope.Trans);
        var txn = new $.Transaction();
        if (model.LedgerSiteId > 0) {
            if (model.ShipTo && model.ShipTo.length > 10) {
                alert('Either select site or enter custom ship to address, can not enter both');
                return;
            }
        }
        if (model.LedgerSiteId == 0 && model.ShipTo.length < 3) {
            alert('Either select shipping site or enter custom ship to address');
            return;
        }
        txn.SaveSales(function (e) {
            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billList');
                //   $state.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        //$scope.SubTotal(0);
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function () {

        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }
        var billItem = cloneObj($scope.TransItem);
        $scope.addItemToBill(billItem);




        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0 });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }
    $scope.addItemToBill = function (billItem) {
        var itemExist = $scope.Trans.Items.find(o => o.ProductId == billItem.ProductId && o.Rate == billItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt(billItem.Quantity);
            itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            billItem.SubTotal = billItem.Quantity * billItem.Rate;

            $scope.Trans.Items.push(billItem);
        }

        if (billItem.BOM != null) {
            $.each(billItem.BOM, function (index, value) {

                value.Quantity = value.Quantity * billItem.Quantity;
                value.Rate = 0;
                value.Item = value.Product;
                value.GroupItemId = billItem.ProductId;
                $scope.addItemToBill(value);
            });
        }
    }
    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            //console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;
        $scope.Trans.TaxAmount = 0;
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {
            //  $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            //for (var i = 0; i < $scope.Trans.Items; i++) {
            //    var item = $scope.Trans.Items[i];

            //}
            if ($scope.Trans.Items) {
                for (var i = 0; i < $scope.Trans.Items.length; i++) {
                    var item = $scope.Trans.Items[i];
                    if ($scope.Trans.Items[i].Quantity != null) {
                        $scope.Trans.Items[i].SubTotal = parseFloat($scope.Trans.Items[i].Quantity) * parseFloat($scope.Trans.Items[i].Rate);
                    }
                    $scope.applyDiscount(item);
                    $scope.applyTaxRate(item.ProductId);
                }
                $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0)

            }
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        $scope.calculateDiscount();
        //}
        $scope.Trans.OtherChargeAmount = parseFloat($scope.Trans.Charge1) + parseFloat($scope.Trans.Charge2) + parseFloat($scope.Trans.Charge3)
            + parseFloat($scope.Trans.Charge4) + parseFloat($scope.Trans.Charge5);

        if ($scope.Config) {
            if ($scope.ApplyFreightGST == true) {
                $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.FreightTaxRate = 0;
            }
            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.ChargesTaxRate = 0;
            }

            if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
                var freight = parseFloat($scope.Trans.Freight);
                $scope.Trans.FreightTax = (freight * $scope.Trans.FreightTaxRate) / 100
            }


            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * $scope.Trans.ChargesTaxRate) / 100
            }
        }
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight) + parseFloat($scope.Trans.FreightTax) +
            $scope.Trans.OtherChargeAmount + $scope.Trans.ChargesTax;

        // $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        return $scope.Trans.SubTotal;

    };

    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.SalePrice;
            $scope.TransItem.TaxCategoryId = selected.originalObject.TaxCategoryId;

            $scope.getBomItems($scope.TransItem.ProductId);
        }
    };

    $scope.getBomItems = function (productId) {
        var p = new $.Product();
        p.BOMByProductId(function (e) {
            if (e.data.Code != 200) {
                alert('Could not featch BOM details');
                return;
            }
            $scope.TransItem.BOM = e.data.Data.Details;
        }, { ProductId: productId });
    }
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {

        // $scope.TransItem.Rate = 0;
        var taxes = StaicData.TAX_CATEGORY;

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);

                if (tax) {

                    for (var i = 0; i < lineItems.length; i++) {
                        lineItems[i].IGST = 0;
                        lineItems[i].CGST = 0;
                        lineItems[i].SGST = 0;
                        lineItems[i].CGSTRate = 0;
                        lineItems[i].SGSTRate = 0;
                        lineItems[i].IGSTRate = 0;
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        if ($scope.comp.StateCode == ledgerDTO.Props.StateCode) {
                            lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                            lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                            lineItems[i].CGSTRate = tax.CGST;
                            lineItems[i].SGSTRate = tax.SGST;

                        }
                        else {
                            lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;
                            lineItems[i].IGSTRate = tax.IGST;
                        }

                        lineItems[i].TaxName = tax.TaxName;
                    }
                }
            }
        }

    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');

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
            $scope.comp = e.data;

        });
    }
    $scope.getInfo();

    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);

    $scope.SetBillingConfig = function (response) {
        if (response.Data != null && response.Data) {
            if (response.Data.length > 0) {

                var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                if (freightTax) {
                    $scope.Config.FreightTax = $scope.Trans.FreightTaxRate = freightTax.Value
                }
            }
            // 
        }

    }
});
