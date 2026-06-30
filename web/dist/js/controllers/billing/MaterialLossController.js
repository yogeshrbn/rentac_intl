app.controller('MatLossListController',
    function ($scope, $state, $crypto, $rootScope, LedgerFactory, ReportService, $crypto) {
        //
        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: '', To: '' };

        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Token = token;
        $scope.Filter.To = convertDate(date);
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }

        $scope.getSites = function () {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, {
                LedgerId: $scope.Filter.LedgerId
            });
        }
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            $scope.getSites();
        })

        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            //if ($scope.Filter.LedgerId == null) {
            //    $scope.Filter.LedgerId = e.data[0].LedgerId;
            //}
        });
        $scope.Data = [];
        $scope.find = function () {

            var wo = new $.WorkOrder();
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            wo.MatLossList(function (e) {

                if (e.data.Code != 200) {
                    alert(e.data.Message);
                }
                else
                    $scope.Data = e.data.Data;

            }, model);

        }

        //$scope.edit = function (item) {

        //    var key = $crypto.encrypt(item.MatLossId);
        //    $state.go('lossedit', { key: key });
        //}

        $scope.find();

        $scope.edit = function () {
            var key = $crypto.encrypt($scope.SelectedItem.MatLossId);
            $('#previewDialog').modal('hide');
            setTimeout(() => {
                $state.go('lossedit', { key: key });
            }, 500);
        }
        $scope.delete = function () {
            var cnf = confirm('Are you sure you want to delete this record? Press ok to delete and cancel to close this window');
            if (!cnf) {
                return;
            }
            var wo = new $.WorkOrder();
            wo.DeleteMatLoss(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }

                alert('Record has been deleted successfully.');
                $scope.find();
                $('#previewDialog').modal('hide');
            }, { MatLossId: $scope.SelectedItem.MatLossId });
        }
        $scope.Preview = 0;
        $scope.SelectedItem;
        $scope.preview = function (item) {
            $scope.SelectedItem = item;
            $scope.Preview = 1;
            $('#previewDialog').modal('show');

            var strInput = "matlossreceipt," + item.MatLossId
            var encrypedText = $crypto.encrypt(strInput);


            ReportService.loadPreviewFromReportServer(function (e) {
                $scope.Preview = null;
                $('#rpt').html(e.data);

            }, encrypedText);
        }

        $scope.printPdf = function () {

            var item = $scope.SelectedItem;
            var strInput = "matlossreceipt," + + item.MatLossId;
            var encrypedText = $crypto.encrypt(strInput);


            ReportService.printFromReportServer(encrypedText);
        }



    });
app.controller('MatLossEntryController', function ($scope, $stateParams, LedgerFactory, AuthenticationService, $rootScope, $crypto, toaster) {

    $scope.Trans = { LedgerId: 0, LedgerSiteId: 0, EntryDate: '', To: '', Items: [] };
    $scope.TransItem = { ProductId: 0, Name: '', Rate: 0, Quantity: 0 };
    var date = new Date();
    var token = $rootScope.getTokenInfo();
    $scope.Token = token;
    FormsValidation.init('frmLossEntry');
    $scope.Trans.To = convertDate(date);
    if (token) {
        $scope.Trans.EntryDate = convertDate(date);
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }

    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, {
            LedgerId: $scope.Trans.LedgerId
        });
    }
    $scope.$watch('Trans.LedgerId', function () {
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        $scope.getSites();
    })

    LedgerFactory.GetAllParties(function (e) {
        $scope.Accounts = e.data;
        //if ($scope.Trans.Trans == null) {
        //    $scope.Trans.LedgerId = e.data[0].LedgerId;
        //}
    });
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Product = selected.originalObject.Name;

        }
    };
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.Amount = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.Amount = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    $scope.AllProducts = function () {
        var product = new $.Product();
        product.GetAll(function (e) {

            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;
        });
    }

    $scope.AllProducts();
    $scope.Save = function () {

        var m = $('#frmLossEntry').valid();
        if (!$scope.Trans.Items) {
            alert('Please enter line items');
            return;
        }
        if ($scope.Trans.Items.length <= 0) {
            alert('Please enter line items');
            return;
        }

        if (!m) {
            // alert('Please provide all values');
            return;
        }
        var wo = new $.WorkOrder();
        var data = cloneObj($scope.Trans);
        data.EntryDate = formatdate(data.EntryDate);
        wo.AddMatLoss(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
            }
            else {
                alert('Record added successfully.');
                $state.go('losslist');
            }

        }, data);
    }

});
app.controller('MatLossEditController', function ($scope, $stateParams, LedgerFactory, $state, $rootScope, $crypto, toaster) {

    $scope.Trans = { MatLossId: 0, LedgerId: 0, LedgerSiteId: 0, EntryDate: '', To: '', Items: [] };
    $scope.TransItem = { ProductId: 0, Name: '', Rate: 0, Quantity: 0 };

    $scope.Trans.MatLossId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);
    if (!$scope.Trans.MatLossId || $scope.Trans.MatLossId == 0) {
        alert('Invalid key provided');
        $state.go('losslist');
    }

    var date = new Date();
    var token = $rootScope.getTokenInfo();
    $scope.Token = token;
    FormsValidation.init('frmLossEntry');
    $scope.Trans.To = convertDate(date);
    if (token) {
        $scope.Trans.EntryDate = convertDate(date);
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }
    $scope.getById = function () {

        if ($scope.Trans.MatLossId > 0) {
            var wo = new $.WorkOrder();
            wo.MatLossById(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                }
                else {
                    $scope.Trans = e.data.Data;
                    $scope.Trans.EntryDate = convertDate(e.data.Data.EntryDate);
                }
            }, { MatLossId: $scope.Trans.MatLossId });
        }
    }
    $scope.getById();
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, {
            LedgerId: $scope.Trans.LedgerId
        });
    }
    $scope.$watch('Trans.LedgerId', function () {
        // console.log($scope.Trans.LedgerId);
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        $scope.getSites();
    })

    LedgerFactory.GetAllParties(function (e) {
        $scope.Accounts = e.data;

    });
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Product = selected.originalObject.Name;

        }
    };
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }

        var itemExist = $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId && o.Rate == $scope.TransItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
            itemExist.Amount = itemExist.Quantity * itemExist.Rate;

        } else {
            $scope.TransItem.Amount = $scope.TransItem.Quantity * $scope.TransItem.Rate;

            $scope.Trans.Items.push($scope.TransItem);
        }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    $scope.AllProducts = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;
        });
    }

    $scope.AllProducts();
    $scope.Save = function () {

        var m = $('#frmLossEntry').valid();
        if (!$scope.Trans.Items) {
            alert('Please enter line items');
            return;
        }
        if ($scope.Trans.Items.length <= 0) {
            alert('Please enter line items');
            return;
        }

        if (!m) {
            // alert('Please provide all values');
            return;
        }
        var wo = new $.WorkOrder();
        var data = cloneObj($scope.Trans);
        data.EntryDate = formatdate(data.EntryDate);
        wo.AddMatLoss(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
            }
            else {
                alert('Saved successfully');
                $state.go('losslist');
            }

        }, data);
    }




});

//---Sale entry
app.controller('MatLossBillController', function ($scope, $rootScope, $state, $q, $uibModal, LedgerFactory, AuthenticationService) {

    $scope.Billing = new $.Billing({ InvoiceId: 0, InvoiceType: 6 });
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
        var req1 = billing.GetLossItemsToBill(null, {
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
                $scope.SubTotal(0);

            });
        });
    }

    $scope.$watch('Billing.Items', function () {
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
    $scope.getInfo();




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
        var model = cloneObj($scope.Billing);

        var billing = new $.Transaction();
        model.Tnc = htmlEncode(model.Tnc);
        billing.SaveSales(function (e) {
            if (e.statusText == 'OK') {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billlist');
                //   $state.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }


    $scope.SubTotal = function (_total) {
        return runBillingTotalsLikeSale($scope, ledgerDTO);
    };

    $scope.DefaultRate = 0.0;

    FormsValidation.init('frmPurchase');

    init();
    $scope.onDiscountPercentChange = function () {
        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {
        $scope.Billing.DiscountPercent = 0;
        $scope.SubTotal(0);
    }
});
app.controller('EditMatLossBillController', function ($scope, $rootScope, $q, $stateParams, $state, $uibModal, LedgerFactory, AuthenticationService, $crypto) {

    $scope.Billing = new $.Billing({ InvoiceId: 0, InvoiceType: 6 });
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
                $scope.Billing.Items = res[0].data.Data.LostItems;
                $scope.Billing.LedgerId = bill.LedgerId;
                $scope.Billing.LedgerSiteId = bill.LedgerSiteId;
                $scope.Billing.InvoiceId = bill.InvoiceId;
                $scope.Billing.InvoiceNumber = bill.InvoiceNumber;
                $scope.Billing.Discount = bill.Discount;
                $scope.Billing.DiscountPercent = bill.DiscountPercent;
                $scope.Billing.InvoiceDate = convertDate(bill.InvoiceDate);

                $scope.SubTotal(0);

            });
        });
    }
    $scope.loadData();
    $scope.$watch('Billing.Items', function () {
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
    $scope.getInfo();






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
        var model = cloneObj($scope.Billing);

        var billing = new $.Transaction();
        model.Tnc = htmlEncode(model.Tnc);
        billing.SaveSales(function (e) {
            if (e.statusText == 'OK') {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billlist');
                //   $state.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }


    $scope.SubTotal = function (_total) {
        return runBillingTotalsLikeSale($scope, ledgerDTO);
    };

    $scope.DefaultRate = 0.0;

    FormsValidation.init('frmPurchase');

    init();
    $scope.onDiscountPercentChange = function () {
        $scope.SubTotal(0);
    }
    $scope.onDiscountChange = function () {
        $scope.Billing.DiscountPercent = 0;
        $scope.SubTotal(0);
    }


});