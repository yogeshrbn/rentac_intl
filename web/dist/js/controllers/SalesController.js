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

function applySaleBillLineDiscounts(items, config, trans) {
    var subTotal = items.reduce(function (partialSum, a) { return partialSum + a.SubTotal; }, 0);
    var totalItemDiscount = 0;
    $.each(items, function (indx, val) {
        val.Discount = 0;
        if (config.discount_type == 'itemlevel') {
            if (trans.DiscountPercent > 0) {
                val.DiscountPercent = trans.DiscountPercent;
                val.Discount = parseFloat((val.SubTotal * trans.DiscountPercent / 100));
                totalItemDiscount += round(val.Discount);
            }
            else if (trans.Discount > 0) {
                val.Discount = parseFloat((val.SubTotal / subTotal) * trans.Discount);
            }
        }
    });
    return totalItemDiscount;
}

function allocateFreightChargesToGst(target, freightTax, chargesTax, isIntraState) {
    var extra = (parseFloat(freightTax) || 0) + (parseFloat(chargesTax) || 0);
    if (extra <= 0) {
        return;
    }
    if (isIntraState) {
        var half = parseFloat((extra / 2).toFixed(2));
        target.CGST = (parseFloat(target.CGST) || 0) + half;
        target.SGST = (parseFloat(target.SGST) || 0) + (extra - half);
    } else {
        target.IGST = (parseFloat(target.IGST) || 0) + extra;
    }
}

function computeSaleBillTaxable(trans, config) {
    var itemsTaxable = 0;
    if (trans && trans.Items) {
        $.each(trans.Items, function (i, val) {
            itemsTaxable += (parseFloat(val.SubTotal) || 0) - (parseFloat(val.Discount) || 0);
        });
    }
    var taxable = itemsTaxable + (parseFloat(trans && trans.Freight) || 0) + (parseFloat(trans && trans.OtherChargeAmount) || 0);
    if (config && config.autoRoundOffTaxable) {
        taxable = round(taxable);
    }
    return taxable;
}

function applySaleBillLineGst(items, config, isIntraState) {
    var applyTaxOn = ((config && config.applyTaxOn) || 'itemlevel').toString().toLowerCase();
    if (applyTaxOn === 'subtotal') {
        applyBillSubtotalGst(items, function (val) {
            return (parseFloat(val.SubTotal) || 0) - (parseFloat(val.Discount) || 0);
        }, config, isIntraState);
    } else {
        $.each(items, function (indx, val) {
            var taxable = val.SubTotal - val.Discount;
            val.IGST = parseFloat((taxable) * val.IGSTRate / 100);
            val.SGST = parseFloat((taxable) * val.SGSTRate / 100);
            val.CGST = parseFloat((taxable) * val.CGSTRate / 100);
        });
    }
}

/** Breakage-style lines: derive Rate from Amount/Quantity when Rate is missing so SubTotal matches API amount. */
function billingLineEnsureRateAndSubTotal(val) {
    var q = parseFloat(val.Quantity) || 0;
    var r = parseFloat(val.Rate);
    if ((!(r > 0) || isNaN(r)) && q > 0 && val.Amount != null && val.Amount !== '') {
        var amt = parseFloat(val.Amount);
        if (!isNaN(amt)) {
            val.Rate = amt / q;
            r = parseFloat(val.Rate);
        }
    }
    val.SubTotal = q * (parseFloat(val.Rate) || 0);
    val.Amount = val.SubTotal;
}

function applyBillingConfigFromArray(configData, scope) {
    if (!configData || configData.length === 0) {
        return;
    }
    var Billing = scope.Billing;
    var Config = scope.Config;
    var applyTax = configData.find(function (o) { return o.SubCategory == 'Tax' && o.Key == 'applyTax'; });
    var allowEditBillNo = configData.find(function (o) { return o.Key == 'allowEditBillNo'; });
    var billPrefix = configData.find(function (o) { return o.Key == 'Prefix' && o.SubCategory == 'BILLNO' && o.Category == 'InvoiceGST'; });
    var tnc = configData.find(function (o) { return o.Key == 'tnc' && o.SubCategory == 'Other' && o.Category == 'Invoice'; });
    var discount_type = configData.find(function (o) { return o.Key && o.Key.toLowerCase() == 'discount_type'; });
    if (applyTax) {
        Billing.applyTax = applyTax.Value == 'true';
    }
    if (allowEditBillNo) {
        Config.allowEditBillNo = allowEditBillNo.Value == 'true';
    }
    if (billPrefix) {
        Config.billPrefix = billPrefix.Value;
    }
    if (tnc) {
        Billing.Tnc = tnc.Value;
    }
    if (discount_type) {
        Config.discount_type = discount_type.Value;
    }
    if (typeof loadBillingTaxConfigExtras === 'function') {
        loadBillingTaxConfigExtras({ Data: configData }, Config);
    }
}

/** Sale-style invoice totals for Billing + Items (loss bill, breakage bill). Freight/charges optional on Billing. */
function runBillingTotalsLikeSale(scope, ledgerDTO) {
    var Billing = scope.Billing;
    var Config = scope.Config || {};
    var items = Billing.Items;
    if (!items) {
        Billing.Total = 0;
        Billing.Taxable = 0;
        Billing.SubTotal = 0;
        return 0;
    }

    Billing.Freight = parseFloat(Billing.Freight) || 0;
    Billing.Charge1 = parseFloat(Billing.Charge1) || 0;
    Billing.Charge2 = parseFloat(Billing.Charge2) || 0;
    Billing.Charge3 = parseFloat(Billing.Charge3) || 0;
    Billing.Charge4 = parseFloat(Billing.Charge4) || 0;
    Billing.Charge5 = parseFloat(Billing.Charge5) || 0;
    Billing.OtherChargeAmount =
        Billing.Charge1 + Billing.Charge2 + Billing.Charge3 + Billing.Charge4 + Billing.Charge5;

    if (Billing.applyTax === false) {
        $.each(items, function (indx, val) {
            billingLineEnsureRateAndSubTotal(val);
            val.IGST = 0;
            val.SGST = 0;
            val.CGST = 0;
        });
        var totalItemDiscountNoTax = applySaleBillLineDiscounts(items, Config, Billing);
        Billing.SubTotal = items.reduce(function (partialSum, a) {
            return partialSum + (parseFloat(a.SubTotal) || 0);
        }, 0);
        var preDiscountTotalNoTax =
            parseFloat(Billing.SubTotal) + Billing.Freight + Billing.OtherChargeAmount;
        if (Config.discount_type != 'itemlevel') {
            if (Billing.DiscountPercent > 0) {
                Billing.Discount = round((preDiscountTotalNoTax * Billing.DiscountPercent) / 100);
            }
        } else if (Billing.DiscountPercent > 0) {
            Billing.Discount = totalItemDiscountNoTax;
        }
        Billing.IGST = Billing.CGST = Billing.SGST = 0;
        Billing.TaxAmount = 0;
        Billing.Total =
            parseFloat(Billing.SubTotal) -
            parseFloat(Billing.Discount || 0) +
            parseFloat(Billing.Freight || 0) +
            Billing.OtherChargeAmount;
        Billing.Taxable =
            typeof computeSaleBillTaxable === 'function'
                ? computeSaleBillTaxable(Billing, Config)
                : Billing.SubTotal;
        return Billing.SubTotal;
    }

    var applyTaxOnSubtotal =
        ((Config.applyTaxOn || 'itemlevel') + '').toLowerCase() === 'subtotal';
    $.each(items, function (indx, val) {
        if (!applyTaxOnSubtotal && scope.applyTaxRate && typeof scope.applyTaxRate === 'function') {
            scope.applyTaxRate(val.ProductId);
        }
        billingLineEnsureRateAndSubTotal(val);
    });

    var totalItemDiscount = applySaleBillLineDiscounts(items, Config, Billing);
    var isIntraState =
        scope.comp &&
        ledgerDTO.Props &&
        ledgerDTO.Props.StateCode &&
        scope.comp.StateCode == ledgerDTO.Props.StateCode;
    applySaleBillLineGst(items, Config, isIntraState);

    Billing.SubTotal = items.reduce(function (partialSum, a) {
        return partialSum + (parseFloat(a.SubTotal) || 0);
    }, 0);
    Billing.CGST = items.reduce(function (partialSum, a) {
        return partialSum + (parseFloat(a.CGST) || 0);
    }, 0);
    Billing.SGST = items.reduce(function (partialSum, a) {
        return partialSum + (parseFloat(a.SGST) || 0);
    }, 0);
    Billing.IGST = items.reduce(function (partialSum, a) {
        return partialSum + (parseFloat(a.IGST) || 0);
    }, 0);

    var preDiscountTotal =
        parseFloat(Billing.SubTotal) +
        Billing.IGST +
        Billing.SGST +
        Billing.CGST +
        parseFloat(Billing.Freight || 0) +
        Billing.OtherChargeAmount;

    if (Config.discount_type != 'itemlevel') {
        if (Billing.DiscountPercent > 0) {
            Billing.Discount = round((preDiscountTotal * Billing.DiscountPercent) / 100);
        }
    } else if (Billing.DiscountPercent > 0) {
        Billing.Discount = totalItemDiscount;
    }

    Billing.TaxAmount = Billing.IGST + Billing.SGST + Billing.CGST;
    Billing.Total =
        parseFloat(Billing.SubTotal) -
        parseFloat(Billing.Discount || 0) +
        Billing.TaxAmount +
        parseFloat(Billing.Freight || 0) +
        Billing.OtherChargeAmount;

    Billing.Taxable =
        typeof computeSaleBillTaxable === 'function'
            ? computeSaleBillTaxable(Billing, Config)
            : Billing.SubTotal;

    return Billing.SubTotal;
}

//--sales register
app.controller('SalesRegisterController', function ($scope, $rootScope, $rootScope, $route, $window, $location, $mdDialog) {

    var sales = new $.Transaction({ SalesId: 0 });
    $scope.Billing = new $.Billing({});
    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    var ledger = new $.Ledger({});
    ledger.GetAll(function (e) {
        $scope.Accounts = e.data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }
    });
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });
    //$scope.SalesRegister = sales.SalesRegister(function (e) {
    //    $scope.Register = e.data;
    //});
    $scope.GetBills = function () {

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.InvoiceType = 4;
        $scope.Billing.GetBillList(function (e) {
            $scope.Register = e.data;
        }, filter);
    }
    $scope.GetBills();
    $scope.ShowSalesItems = function (index) {

        loadItems(index.SalesId);
    };
    $scope.SalesItemsTax = function (index) {

        loadTaxes(index.SalesId);
    };
    function loadTaxes(salesId) {
        sales.SalesId = salesId;
        sales.SalesItemsTax(function (e) {
            $scope.AppliedTaxes = e.data;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/taxItems.html', function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        AppliedTaxes: $scope.AppliedTaxes
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: function (AppliedTaxes) {
                        $scope.closeSliderModal = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        });
    }

    function loadItems(salesId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    salesId: salesId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'SalesItemsController'
            });
        });
    }

    $scope.print = function (item, format) {
        var filter = { 'SalesId': item.SalesId, 'FileFormat': format };
        //  purchase.PurchaseId = item.PurchaseId;
        sales.PrintSalesReceipt(function (e) {
            //debugger
            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        }, filter);
    }
    //PurchaseItemsList
});
//-end of sales register
app.controller('SalesItemsController', function ($scope, $mdDialog, salesId) {

    var sales = new $.Transaction({ SalesId: salesId });
    sales.SalesItemsList(function (e) {
        $('#loadersite').hide();
        $scope.Items = e.data;
    });

    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
});
//---Sale entry
app.controller('EditSaleBillController', function ($scope, $rootScope, $stateParams, $state, $crypto,
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
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', allowEditBillNo: false, discount_type: 'invoicelevel', applyTaxOn: 'itemlevel', defaultTaxRate: 0, autoRoundOffTaxable: false
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
                $scope.Trans.InvoiceDate = convertDate(billingResponse.data.Data.InvoiceDate);
                $scope.AddInfo = $scope.Trans.AddInfo;
                $scope.qtnc = $scope.Trans.Tnc;
                $scope.Trans.FreightTaxRate = 0;
                if ($scope.Trans.Recurring == true) {
                    $scope.Trans.StartsOn = convertDate(billingResponse.data.Data.StartsOn);
                    $scope.Trans.EndsOn = convertDate(billingResponse.data.Data.EndsOn);
                }
                if ($scope.Trans.ChargesTax > 0) {

                    $scope.ApplyOtherChargeGST = true;
                }

                if ($scope.Trans.FreightTax > 0) {
                    $scope.ApplyFreightGST = true;

                }
                $scope.Trans.PODate = convertDate(billingResponse.data.Data.PODate);
                $scope.SubTotal(0);
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
        if ($scope.Config.allowEditBillNo == true) {
            if (!model.InvoiceNumber) {
                alert('Please enter the invoice number.');
                return;
            }
            if (trimAll(model.InvoiceNumber).length <= 1) {
                alert('Please enter a valid invoice number.');
                return;
            }
        }
        var invDate = formatdate(model.InvoiceDate);
        if (model.PODate) {
            model.PODate = formatdate(model.PODate);
        }
        if (model.Recurring == true) {
            var cDate = new Date();
            model.StartsOn = formatdate(model.StartsOn);
            model.EndsOn = formatdate(model.EndsOn);
            if (dateDiff(model.StartsOn, model.EndsOn) < 1) {
                alert('Ends on date must be ahead of Starts on date');
                return;
            }
            if (dateDiff(invDate, model.StartsOn) < 0) {
                alert('Start on date must be ahead of bill date');
                return;
            }
            if (!model.Iteration) {
                alert('Iteration must be selected');
                return;
            }
        }
        else {
            model.StartsOn = null;
            model.EndsOn = null;

        }
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
        model.Discount = model.Discount || model.DiscountAmount || 0;
        model.DiscountPercent = model.DiscountPercent || model.DiscountValue || 0;

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


    $scope.onDiscountPercentChange = function () {

        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {

        $scope.Trans.DiscountPercent = 0;

        $scope.SubTotal(0);
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
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {

            var applyTaxOnSubtotal = (($scope.Config.applyTaxOn || 'itemlevel') + '').toLowerCase() === 'subtotal';
            $.each($scope.Trans.Items, function (indx, val) {
                if (!applyTaxOnSubtotal) {
                    $scope.applyTaxRate(val.ProductId);
                }
                val.SubTotal = parseFloat(val.Quantity) * parseFloat(val.Rate);
            });
            var totalItemDiscount = applySaleBillLineDiscounts($scope.Trans.Items, $scope.Config, $scope.Trans);
            var isIntraState = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
            applySaleBillLineGst($scope.Trans.Items, $scope.Config, isIntraState);

            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            $scope.Trans.CGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.CGST, 0);
            $scope.Trans.SGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SGST, 0);
            $scope.Trans.IGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.IGST, 0);

            //$scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
            //    if (!a.IGST) {
            //        a.IGST = 0;
            //    }
            //    if (!a.CGST) {
            //        a.CGST = 0;
            //    }
            //    if (!a.SGST) {
            //        a.SGST = 0;
            //    }
            //    return partialSum + a.IGST + a.CGST + a.SGST;
            //}, 0)

        }



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
        var isIntraStateBill = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
        allocateFreightChargesToGst($scope.Trans, $scope.Trans.FreightTax, $scope.Trans.ChargesTax, isIntraStateBill);
        var preDiscountTotal = parseFloat($scope.Trans.SubTotal) + $scope.Trans.IGST + $scope.Trans.SGST + $scope.Trans.CGST
            + parseFloat($scope.Trans.Freight || 0) + $scope.Trans.OtherChargeAmount;
        if ($scope.Config.discount_type != 'itemlevel') {
            if ($scope.Trans.DiscountPercent > 0) {
                $scope.Trans.Discount = round((preDiscountTotal * $scope.Trans.DiscountPercent) / 100);
            }
        }
        else {
            if ($scope.Trans.DiscountPercent > 0) {
                $scope.Trans.Discount = totalItemDiscount;
            }

        }
        $scope.Trans.TaxAmount = $scope.Trans.IGST + $scope.Trans.SGST + $scope.Trans.CGST;
        //   }
        $scope.Trans.Total = $scope.Trans.SubTotal - parseFloat($scope.Trans.Discount || 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight || 0) + $scope.Trans.OtherChargeAmount;

        $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
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
                        if ($scope.Config.discount_type != 'itemlevel') {
                            lineItems[i].DiscountAmount = 0;
                        }
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
            if ($scope.Trans && $scope.Trans.Items && $scope.Trans.Items.length) {
                $scope.SubTotal(0);
            }

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
                var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');


                if (allowEditBillNo) {
                    $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                }

                var showDiscriptionColumn = response.Data.find(o => o.Key == 'showDiscriptionColumn');
                if (showDiscriptionColumn) {
                    $scope.Config.showDiscriptionColumn = showDiscriptionColumn.Value == 1;
                }
                var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');


                if (discount_type) {
                    $scope.Config.discount_type = discount_type.Value;
                }
                loadBillingTaxConfigExtras(response, $scope.Config);
            }
            // 
        }

    }
});
app.controller('SaleController', function ($scope, $rootScope, $stateParams, $state, $http, $crypto, $uibModal, LedgerFactory, AuthenticationService) {

    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 4 });
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
        $scope.Trans.Discount = 0;
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.InvoiceDate = convertDate(new Date());
        $scope.Trans.Recurring = false;
        $scope.Trans.StartsOn = convertDate(new Date());
        $scope.Trans.EndsOn = convertDate(addMonths(new Date(), 6));
        $scope.Trans.Iteration = 'monthly';
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
        if ($scope.Config.allowEditBillNo == true) {
            if (!model.InvoiceNumber) {
                alert('Please enter the invoice number.');
                return;
            }
            if (trimAll(model.InvoiceNumber).length <= 1) {
                alert('Please enter a valid invoice number.');
                return;
            }
        }



        if (model.Recurring == true) {
            var cDate = new Date();
            model.StartsOn = formatdate(model.StartsOn);
            model.EndsOn = formatdate(model.EndsOn);
            var invDate = formatdate(model.InvoiceDate);

            if (dateDiff(model.StartsOn, model.EndsOn) < 1) {
                alert('Ends on date must be ahead of Starts on date');
                return;
            }
            if (dateDiff(invDate, model.StartsOn) < 0) {
                alert('Start on date must be ahead of bill date');
                return;
            }
            if (!model.Iteration) {
                alert('Iteration must be selected');
                return;
            }
        }
        else {
            model.StartsOn = null;
            model.EndsOn = null;

        }

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
        model.Discount = model.Discount || model.DiscountAmount || 0;
        model.DiscountPercent = model.DiscountPercent || model.DiscountValue || 0;
        if (model.PODate) {
            model.PODate = formatdate(model.PODate);
        }

        txn.SaveSales(function (e) {

            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billList');
            } else {
                showMessage(e.data);
            }

        }, model, fileList);
    }
    //$scope.$watch('Trans.DiscountValue', function () {

    //    $scope.calculateDiscount();
    //    //$scope.SubTotal(0);
    //});

    $scope.calculateDiscount = function () {

        var discountBase = getDiscountBase();
        $scope.Trans.DiscountAmount = (discountBase * $scope.Trans.DiscountPercent) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.Discount = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }

    }
    $scope.onDiscountPercentChange = function () {

        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {

        $scope.Trans.DiscountPercent = 0;

        $scope.SubTotal(0);
    }
    function getDiscountBase() {
        if ($scope.Config.discount_type == 'itemlevel') {
            return parseFloat($scope.Trans.SubTotal || 0);
        }
        return parseFloat($scope.Trans.SubTotal || 0) + parseFloat($scope.Trans.TaxAmount || 0)
            + parseFloat($scope.Trans.Freight || 0) + parseFloat($scope.Trans.OtherChargeAmount || 0);
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
        item.DiscountAmount = 0;
        item.DiscountPercent = 0;
        if ($scope.Config.discount_type != 'itemlevel') {
            return;
        }
        if ($scope.Trans.DiscountPercent > 0) {
            item.DiscountPercent = $scope.Trans.DiscountPercent;
            item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;
        } else if ($scope.Trans.Discount > 0 && $scope.Trans.SubTotal > 0) {
            item.DiscountAmount = (item.SubTotal / $scope.Trans.SubTotal) * $scope.Trans.Discount;
        }
    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            // console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {

        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;

        $scope.Trans.TaxAmount = 0;
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {

            var applyTaxOnSubtotalSale = (($scope.Config.applyTaxOn || 'itemlevel') + '').toLowerCase() === 'subtotal';
            $.each($scope.Trans.Items, function (indx, val) {
                if (!applyTaxOnSubtotalSale) {
                    $scope.applyTaxRate(val.ProductId);
                }
                val.SubTotal = parseFloat(val.Quantity) * parseFloat(val.Rate);
            });
            var totalItemDiscount = applySaleBillLineDiscounts($scope.Trans.Items, $scope.Config, $scope.Trans);
            var isIntraStateSale = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
            applySaleBillLineGst($scope.Trans.Items, $scope.Config, isIntraStateSale);

            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
            $scope.Trans.CGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.CGST, 0);
            $scope.Trans.SGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SGST, 0);
            $scope.Trans.IGST = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.IGST, 0);

            //$scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
            //    if (!a.IGST) {
            //        a.IGST = 0;
            //    }
            //    if (!a.CGST) {
            //        a.CGST = 0;
            //    }
            //    if (!a.SGST) {
            //        a.SGST = 0;
            //    }
            //    return partialSum + a.IGST + a.CGST + a.SGST;
            //}, 0)

        }



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
        var isIntraStateBillSale = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
        allocateFreightChargesToGst($scope.Trans, $scope.Trans.FreightTax, $scope.Trans.ChargesTax, isIntraStateBillSale);
        var preDiscountTotal = parseFloat($scope.Trans.SubTotal) + $scope.Trans.IGST + $scope.Trans.SGST + $scope.Trans.CGST
            + parseFloat($scope.Trans.Freight || 0) + $scope.Trans.OtherChargeAmount;
        if ($scope.Config.discount_type != 'itemlevel') {
            if ($scope.Trans.DiscountPercent > 0) {
                $scope.Trans.Discount = round((preDiscountTotal * $scope.Trans.DiscountPercent) / 100);
            }
        }
        else {
            if ($scope.Trans.DiscountPercent > 0) {
                $scope.Trans.Discount = totalItemDiscount;
            }

        }
        $scope.Trans.TaxAmount = $scope.Trans.IGST + $scope.Trans.SGST + $scope.Trans.CGST;
        //   }
        $scope.Trans.Total = $scope.Trans.SubTotal - parseFloat($scope.Trans.Discount || 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight || 0) + $scope.Trans.OtherChargeAmount;

        $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
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
        debugger
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
                        if ($scope.Config.discount_type != 'itemlevel') {
                            lineItems[i].DiscountAmount = 0;
                        }
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
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', allowEditBillNo: false, showDiscriptionColumn: false, discount_type: 'invoicelevel', applyTaxOn: 'itemlevel', defaultTaxRate: 0, autoRoundOffTaxable: false
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

                    var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');
                    var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');


                    if (allowEditBillNo) {
                        $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                    }
                    if (discount_type) {
                        $scope.Config.discount_type = discount_type.Value;
                    }
                    loadBillingTaxConfigExtras(response, $scope.Config);

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
//Sale return controllers
app.controller('SaleReturnListController', function ($scope, $rootScope, $state, $crypto, $window, FileSaver) {

    var sales = new $.Transaction({ SalesId: 0 });
    $scope.Billing = new $.Billing({});
    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    var ledger = new $.Ledger({});
    ledger.GetAll(function (e) {
        $scope.Accounts = e.data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }
    });
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });
    //$scope.SalesRegister = sales.SalesRegister(function (e) {
    //    $scope.Register = e.data;
    //});
    $scope.GetBills = function () {

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.InvoiceType = 7;

        $scope.Billing.GetBillList(function (e) {
            $scope.Register = e.data.filter(o => o.StatusId != 2);
        }, filter);
    }
    $scope.GetBills();
    $scope.ShowSalesItems = function (index) {

        loadItems(index.SalesId);
    };
    $scope.SalesItemsTax = function (index) {

        loadTaxes(index.SalesId);
    };
    function loadTaxes(salesId) {
        sales.SalesId = salesId;
        sales.SalesItemsTax(function (e) {
            $scope.AppliedTaxes = e.data;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/taxItems.html', function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        AppliedTaxes: $scope.AppliedTaxes
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: function (AppliedTaxes) {
                        $scope.closeSliderModal = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        });
    }

    function loadItems(salesId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    salesId: salesId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'SalesItemsController'
            });
        });
    }

    $scope.print = function (item, format) {
        var filter = { 'SalesId': item.SalesId, 'FileFormat': format };
        //  purchase.PurchaseId = item.PurchaseId;
        sales.PrintSalesReceipt(function (e) {
            //debugger
            var filePath = SERVER_RPT_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        }, filter);
    }
    $scope.edit = function (item) {
        var key = $crypto.encrypt(item.InvoiceId);
        $('#previewDialog').modal('hide');
        setTimeout(() => {
            $state.go('editsalereturn', { key: key });
        }, 500);
    }
    $scope.Preview = 0;
    $scope.SelectedItem;
    $scope.preview = function (item) {
        $scope.SelectedItem = item;
        $scope.Preview = 1;
        $('#previewDialog').modal('show');

        var strInput = "salereturn-invoice," + item.InvoiceId
        var encrypedText = $crypto.encrypt(strInput);

        var econded = btoa(encrypedText);
        var report = new $.Reports();

        report.previewReport(function (e) {
            $scope.Preview = null;
            $('#rpt').html(e.data);

        }, econded);
    }

    $scope.printPdf = function () {

        var item = $scope.SelectedItem;
        var strInput = "salereturn-invoice," + + item.InvoiceId;
        var encrypedText = $crypto.encrypt(strInput);

        var econded = btoa(encrypedText);
        var report = new $.Reports();
        report.downloadReport(function (e) {

            FileSaver.saveAs(e.data, 'text.pdf');
        }, econded);
    }
    $scope.delete = function () {
        var item = $scope.SelectedItem;


        var tr = 'tr' + item.InvoiceId;
        var confirm = window.confirm('Deleted items will not be revoked.\nAre you sure to delete?');
        if (!confirm) return;
        $scope.Billing.InvoiceId = item.InvoiceId;
        $(event.target).hide(); // hide cancelled button
        $scope.Billing.CancelBill(function (e) {

            if (e.data.Data == true) {
                alert('Record deleted successfully.');
                $('#previewDialog').modal('hide');
                $scope.GetBills();
            }
        });
    }
    //PurchaseItemsList
});

app.controller('EditSaleReturnBillController', function ($scope, $rootScope, $stateParams, $state,
    $crypto, $q, $uibModal, LedgerFactory, AuthenticationService) {


    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 7, InvoiceId: 0 });

    sale.InvoiceId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.Trans.Items = [];//initializeArray();

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
    $scope.$watch('Trans.LedgerId', function () {
        if ($scope.Trans.LedgerId == 0) return;
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode
        }
    });
    $scope.getById = function () {
        debugger
        var billing = new $.Billing();
        var config = new $.Config();
        var billConfig = config.GetBillingConfig(null, true);
        var billById = billing.ById(function (e) {
            if (e.data.Code != 200) {
                alert('Could not load invoice' + e.data.Message);
                return;
            }
            $scope.Trans = e.data.Data;
            $scope.Trans.Items = e.data.Data.BillableItems
            $scope.Trans.InvoiceDate = convertDate(e.data.Data.InvoiceDate);


        }, { InvoiceId: $scope.Trans.InvoiceId }, true);

        var allPromises = [billConfig, billById];
        $q.all(allPromises).then(function (responses) {
            // responses is an array containing the results of each promise
            var response1 = responses[0].data; // Data from /api/data1
            var response2 = responses[1].data; // Data from /api/data2


            // Process the combined data here
            console.log('All requests completed successfully!', response1, response2, response3);
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
                $state.go('saleretlist');
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
        // $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountPercent) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountPercent = 0;
        } else {
            $scope.Trans.Discount = 0;
        }
        if (!$scope.Trans.Discount && $scope.Trans.Discount > 0) {
            $scope.Trans.DiscountAmount = $scope.Trans.Discount;
        }


    }
    $scope.onDiscountChange = function () {
        $scope.Billing.DiscountPercent = 0;
        //if (!$scope.Trans.Discount) {
        //    $scope.Trans.Discount = 0;
        //}
        //if ($scope.Trans.SubTotal > 0) {
        //    $scope.Trans.DiscountAmount = ($scope.Trans.Discount * 100) / $scope.Trans.SubTotal;
        //} else {
        //    $scope.Trans.DiscountAmount = 0;
        //}
        //$scope.Trans.DiscountAmount = $scope.Trans.Discount;
        $scope.SubTotal(0);
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
                $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
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
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
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
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        if ($scope.comp.StateCode == ledgerDTO.Props.StateCode) {
                            lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                            lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        }
                        else
                            lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;

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

    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', editnumber: false, applyTaxOn: 'itemlevel'
    };
    $scope.SetBillingConfig = function (response) {
        if (response.Data != null && response.Data) {
            if (response.Data.length > 0) {

                var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                if (freightTax) {
                    $scope.Config.FreightTax = $scope.Trans.FreightTaxRate = freightTax.Value
                }
                var applyTaxOnCfg = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'applyTaxOn');
                if (applyTaxOnCfg && applyTaxOnCfg.Value) {
                    var ato = ('' + applyTaxOnCfg.Value).toLowerCase();
                    if (ato === 'subtotal' || ato === 'itemlevel') {
                        $scope.Config.applyTaxOn = ato;
                    }
                }
            }
            // 
        }

    }

});
app.controller('SaleReturnController', function ($scope, $rootScope, $routeParams, $state, $http, $location, $uibModal, LedgerFactory, AuthenticationService) {

    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 7 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.Trans.Items = [];//initializeArray();
    init();
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
    $scope.$watch('Trans.LedgerId', function () {

        $rootScope.LedgerId = $scope.Trans.LedgerId;
        if ($scope.SundryDebtors) {
            var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
            if (ledger) {
                ledgerDTO.Props.StateCode = ledger.StateCode
            }
        }
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
        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
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



        model.Discount = model.Discount || model.DiscountAmount || 0;
        model.DiscountPercent = model.DiscountPercent || model.DiscountValue || 0;
        $scope.Trans.SaveSales(function (e) {
            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('saleretlist');
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
        $scope.Trans.Discount = $scope.Trans.DiscountAmount;
    }
    $scope.onDiscountChange = function () {
        if (!$scope.Trans.Discount) {
            $scope.Trans.Discount = 0;
        }
        if ($scope.Trans.SubTotal > 0) {
            $scope.Trans.DiscountValue = ($scope.Trans.Discount * 100) / $scope.Trans.SubTotal;
        } else {
            $scope.Trans.DiscountValue = 0;
        }
        $scope.Trans.DiscountAmount = $scope.Trans.Discount;
        $scope.SubTotal(0);
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
            // console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;

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
                $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);
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
        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount;
        $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
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
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        if ($scope.comp.StateCode == ledgerDTO.Props.StateCode) {
                            lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                            lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                        }
                        else
                            lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;

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

});