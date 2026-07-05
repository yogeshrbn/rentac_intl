
app.controller('BreakageBillController', function ($scope, $rootScope, $state, $q, $uibModal,
    LedgerFactory, AuthenticationService, ReportService, $crypto, ChallanTaxService, TaxService) {

    $scope.Billing = new $.Billing({ InvoiceId: 0, InvoiceType: 2 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();

    $scope.Billing.Items = [];//initializeArray();
    $scope.Config = {
        BillPrefix: '',
        FreightTax: 0,
        BrekageBill: 1,
        allowEditBillNo: false,
        discount_type: 'invoicelevel',
        applyTaxOn: 'itemlevel',
        defaultTaxRate: 0,
        autoRoundOffTaxable: false
    };
    $scope.ApplyGST = true;
    $scope.ItemTaxSettings = [];
    $scope._taxDataPromise = initSaleTaxData($scope, TaxService, ChallanTaxService);
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Billing.Items.splice(index - 1, 1);
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
        $scope.Billing.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Billing.LedgerId', function () {
        $scope.Billing.Items = [];
        $scope.getSites();
        $rootScope.LedgerId = $scope.Billing.LedgerId;
        if ($scope.SundryDebtors) {

            var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Billing.LedgerId);
            if (ledger) {
                ledgerDTO.Props.StateCode = ledger.StateCode
            }
        }
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.$watch('Billing.LedgerSiteId', function () {
        $scope.loadData();
    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, {
            LedgerId: $scope.Billing.LedgerId
        });
    }

    $scope.loadData = function () {
        var config = new $.Config();
        var billing = new $.Billing();
        $scope.Billing.Items = [];
        var req1 = billing.GetBreakageForBill(null, {
            LedgerId: $scope.Billing.LedgerId
            , LedgerSiteId: $scope.Billing.LedgerSiteId, applyTax: $scope.Billing.applyTax
        }, true);


        config.GetBillingConfig(function (e) {

            var configData = e.data.Data;
            if (configData != null && configData.length > 0) {
                applyBillingConfigFromArray(configData, $scope);
            }

            $q.all([req1]).then((res) => {
                $scope.Billing.Items = res[0].data.Data.BillingItems;
                $scope.Billing.BreakageDamageDetails = res[0].data.Data.BreakageDamageDetails;
                $scope.Billing.Challans = res[0].data.Data.Challans;
                if ($scope.Billing.applyTax === false) {
                    $scope.ApplyGST = false;
                }
                $scope._taxDataPromise.then(function () {
                    if ($scope.SubTotal) {
                        $scope.SubTotal(0);
                    }
                });
            });
        });
    }

    $scope.$watch('Billing.Items', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    }, true);
    $scope.$watch('Billing.BreakageDamageDetails', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    }, true);







    $scope.getInfo = function () {
        if (!loginData || !loginData.DefaultCompanyId) {
            return;
        }
        var company = new $.Company();
        company.Props.CompanyId = loginData.DefaultCompanyId;
        company.GetDetails(function (e) {
            if (e.status != 200 || !e.data) {
                return;
            }
            $scope.comp = e.data;
            if ($scope.SubTotal) {
                $scope.SubTotal(0);
            }
        });
    };



    $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
    $scope.Billing.FreightTax = 0;

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
        $scope.BillingItem = new $.TransItem({ PurchaseRate: $scope.DefaultRate });
        //   $scope.Billing = new $.LedgerTrasaction({});
        $scope.Billing.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Billing.PurchaseDate = convertDate(new Date());
        $scope.Billing.DiscountPercent = 0;
        $scope.Billing.Discount = 0;

        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Billing.Items.filter(function (v) {
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


    function addWorkOrder(fileList) {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
        var model = cloneObj($scope.Billing);

        var billing = new $.Billing();
        model.Tnc = htmlEncode(model.Tnc);
        model.InvoiceDate = formatdate(model.InvoiceDate);
        billing.SaveBill(function (e) {
            if (e.data.Code == 200) {

                // alert('saved');
                var invoiceId = e.data.Data.InvoiceId;
                $scope.warnOnLeave = false;

                //  var encrypedText = $crypto.encrypt(invoiceId);

                //var econded = btoa(encrypedText);
                var strInput = "salebill," + + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);
                ReportService.printFromReportServer(encrypedText);
                $state.go('billList');
                //var report = new $.Reports();
                //ReportService.printFromReportServer(encrypedText);
                //report.downloadReportFromHtml(function (e) {
                //    FileSaver.saveAs(e.data, e.data.Data.InvoiceNumber + '.pdf');
                //    $state.go('billList');
                //}, 'salebill', econded);

                //   $state.reload();
            } else {
                showMessage(e.Message);
            }

        }, model, fileList);
    }


    $scope.SubTotal = function (_total) {
        return runBreakageBillSubtotal($scope, ChallanTaxService);
    };

    $scope.getInfo();

    $scope.DefaultRate = 0.0;

    FormsValidation.init('frmPurchase');

    init();
    $scope.$watch('ApplyGST', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.onDiscountPercentChange = function () {
        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {
        $scope.Billing.DiscountPercent = 0;
        $scope.SubTotal(0);
    }
});
app.controller('EditBreakageBillController', function ($scope, $rootScope, $q, $stateParams, $state, $uibModal
    , LedgerFactory, AuthenticationService, $crypto, ReportService, ChallanTaxService, TaxService) {

    $scope.Billing = new $.Billing({ InvoiceId: 0, InvoiceType: 2 });
    $scope.Billing.InvoiceId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });

    var loginData = AuthenticationService.getTokenInfo();

    $scope.Billing.Items = [];//initializeArray();
    $scope.Config = {
        BillPrefix: '',
        FreightTax: 0,
        BrekageBill: 1,
        allowEditBillNo: false,
        discount_type: 'invoicelevel',
        applyTaxOn: 'itemlevel',
        defaultTaxRate: 0,
        autoRoundOffTaxable: false
    };
    $scope.ApplyGST = true;
    $scope.ItemTaxSettings = [];
    $scope._taxDataPromise = initSaleTaxData($scope, TaxService, ChallanTaxService);
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Billing.Items.splice(index - 1, 1);
        });
    }

    LedgerFactory.GetAllParties(function (e) {

        $scope.SundryDebtors = e.data;

    });

    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Billing.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Billing.LedgerId', function () {

        $scope.getSites();
        $rootScope.LedgerId = $scope.Billing.LedgerId;
        if ($scope.SundryDebtors) {

            var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Billing.LedgerId);
            if (ledger) {
                ledgerDTO.Props.StateCode = ledger.StateCode
            }
        }
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    //$scope.$watch('Billing.LedgerSiteId', function () {
    //    $scope.loadData();
    //});
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, {
            LedgerId: $scope.Billing.LedgerId
        });
    }

    $scope.loadData = function () {
        var config = new $.Config();
        var billing = new $.Billing();
        $scope.Billing.Items = [];
        var req1 = billing.ById(null, { InvoiceId: $scope.Billing.InvoiceId, applyTax: $scope.Billing.applyTax }, true);
        //var req1 = billing.GetLossItemsToBill(null, {
        //    LedgerId: $scope.Billing.LedgerId
        //    , LedgerSiteId: $scope.Billing.LedgerSiteId, applyTax: $scope.Billing.applyTax
        //}, true);


        config.GetBillingConfig(function (e) {

            var configData = e.data.Data;
            if (configData != null && configData.length > 0) {
                applyBillingConfigFromArray(configData, $scope);
            }

            $q.all([req1]).then((res) => {
                var bill = res[0].data.Data;
                $scope.Billing.Items = res[0].data.Data.BillableItems;
                $scope.Billing.BreakageDamageDetails = res[0].data.Data.BreakageDamageDetails;
                $scope.Billing.Challans = res[0].data.Data.Challans;
                $scope.Billing.LedgerId = bill.LedgerId;
                $scope.Billing.LedgerSiteId = bill.LedgerSiteId;
                $scope.Billing.InvoiceId = bill.InvoiceId;
                $scope.Billing.InvoiceNumber = bill.InvoiceNumber;
                $scope.Billing.Discount = bill.Discount;
                $scope.Billing.DiscountPercent = bill.DiscountPercent;
                $scope.Billing.InvoiceDate = convertDate(bill.InvoiceDate);
                if ($scope.Billing.applyTax === false) {
                    $scope.ApplyGST = false;
                }
                if (bill.AppliedTaxes && bill.AppliedTaxes.length) {
                    $scope.Billing.AppliedTaxes = bill.AppliedTaxes;
                }

                $scope._taxDataPromise.then(function () {
                    loadSavedBillingInvoiceTaxes($scope.Billing.InvoiceId, $scope, ChallanTaxService, function () {
                        if ($scope.SubTotal) {
                            $scope.SubTotal(0);
                        }
                    });
                });

            });
        });
    }
    $scope.loadData();
    $scope.$watch('Billing.Items', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    }, true);
    $scope.$watch('Billing.BreakageDamageDetails', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    }, true);







    $scope.getInfo = function () {
        if (!loginData || !loginData.DefaultCompanyId) {
            return;
        }
        var company = new $.Company();
        company.Props.CompanyId = loginData.DefaultCompanyId;
        company.GetDetails(function (e) {
            if (e.status != 200 || !e.data) {
                return;
            }
            $scope.comp = e.data;
            if ($scope.SubTotal) {
                $scope.SubTotal(0);
            }
        });
    };



    $scope.AppliedTaxes = $scope.Billing.AppliedTaxes = [];
    $scope.Billing.FreightTax = 0;

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
        $scope.BillingItem = new $.TransItem({ PurchaseRate: $scope.DefaultRate });
        //   $scope.Billing = new $.LedgerTrasaction({});
        $scope.Billing.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Billing.PurchaseDate = convertDate(new Date());
        $scope.Billing.DiscountPercent = 0;
        $scope.Billing.Discount = 0;

        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Billing.Items.filter(function (v) {
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


    function addWorkOrder(fileList) {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
        var model = cloneObj($scope.Billing);

        var billing = new $.Billing();
        model.Tnc = htmlEncode(model.Tnc);
        model.InvoiceDate = formatdate(model.InvoiceDate);
        billing.SaveBill(function (e) {
             
            if (e.data.Code == 200) {

               // alert('saved');
                var invoiceId = e.data.Data.InvoiceId;
                $scope.warnOnLeave = false;
       
              //  var encrypedText = $crypto.encrypt(invoiceId);

                //var econded = btoa(encrypedText);
                var strInput = "salebill," + + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);
                ReportService.printFromReportServer(encrypedText);
                $state.go('billList');
                //var report = new $.Reports();
                //ReportService.printFromReportServer(encrypedText);
                //report.downloadReportFromHtml(function (e) {
                //    FileSaver.saveAs(e.data, e.data.Data.InvoiceNumber + '.pdf');
                //    $state.go('billList');
                //}, 'salebill', econded);

                //   $state.reload();
            } else {
                showMessage(e.Message);
            }

        }, model, fileList);
    }


    $scope.SubTotal = function (_total) {
        return runBreakageBillSubtotal($scope, ChallanTaxService);
    };

    $scope.getInfo();

    $scope.DefaultRate = 0.0;

    FormsValidation.init('frmPurchase');

    init();
    $scope.$watch('ApplyGST', function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.onDiscountPercentChange = function () {
        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {
        $scope.Billing.DiscountPercent = 0;
        $scope.SubTotal(0);
    }


});