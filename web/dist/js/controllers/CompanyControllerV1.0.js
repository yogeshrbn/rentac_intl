app.controller("CompanyController", function ($scope, $routeParams, $http) {

    debugger
    var cId = $routeParams.cId == undefined ? 0 : $routeParams.cId;
    var companyDTO = new $.Company({ CompanyId: cId });
    FormsValidation.init();
    $scope.Company = companyDTO;
    function BindList() {
        companyDTO.GetAll(function (e) {
            $scope.Companies = e.data;
        });
    }
    if (cId == 0) {

        BindList();

    }
    else {
        companyDTO.GetDetails(function (e) {

            $scope.Company = companyDTO = new $.Company(e.data);
        });

    }

    $scope.Activate = function (isActive, companyId) {
        if (isActive == 0) {
            if (!confirm('Are you sure to de-activate the company?')) {
                return;
            }
        }
        companyDTO.Props.CompanyId = companyId;
        companyDTO.Props.IsActive = isActive;
        companyDTO.ActivateDeActivate(function (e) {
            BindList();
        });
    }

    $scope.Save = function () {
        debugger
        var m = $('#form-company').valid();
        if (m) {
            EnableToolbar(0);
            companyDTO.Add(function (e) {
                EnableToolbar(1);
                showMessage(MessageClass.COMPANY_SAVED);
            });
        }
    }
    $scope.RowSelected = function (index) {



    }
});
app.controller("LedgerController", function ($scope, $routeParams, $http, $window, $filter) {
    var cId = $routeParams.cId == undefined ? 0 : $routeParams.cId;
    var ledgerDTO = new $.Ledger({ LedgerId: cId });
    var groupDTO = new $.AccountGroup({ AccountGroupId: 0 });
    FormsValidation.init();
    $scope.Ledger = ledgerDTO;
    function BindList() {
        ledgerDTO.GetAll(function (e) {
            $scope.Ledgers = e.data;
        });
    }
    if (cId == 0) {
        BindList();
    }
    else {
        ledgerDTO.GetDetails(function (e) {
            $scope.Ledger = ledgerDTO = new $.Ledger(e.data);
        });
    }
    //get all ledger accounts if not in edit or add mode. This is for listing of all ledgers
  //  if ($routeParams.cId == undefined) {
        groupDTO.GetAll(function (e) {
            $scope.AccGroups = e.data;
        });
   // }
    $scope.Activate = function (isActive, ledgerId) {
        if (isActive == 0) {
            if (!confirm('Are you sure to de-activate the account?')) {
                return;
            }
        }
        ledgerDTO.Props.LedgerId = ledgerId;
        ledgerDTO.Props.IsActive = isActive;
        ledgerDTO.ActivateDeActivate(function (e) {
            BindList();
        });
    }

    //-- stock register functions and members
    //-- filter for party wise stock register
    var filter = { LedgerId: 0, From: '', To: '' };
    $scope.PartyStockData = [];
    $scope.OpeningBalance = [];
    $scope.Filter = filter;
    //gets the stock register for the party

    $scope.GetPartyStockRegister = function () {
        getPartyOpeningBalance();
        ledgerDTO.PartyStockRegister(function (e) {
            $scope.PartyStockData = e.data;
            //selects the issued items only from the dataset
            $scope.PartyIssueTans = jQuery.grep(e.data, function (n, i) {
                return (n.TranType == 1);
            });
            //selects the recevied items only from the dataset
            $scope.PartyRecTans = jQuery.grep(e.data, function (n, i) {
                return (n.TranType == 2);
            });
            //get the maximum unique products to create rows.
            $scope.MaxRows = jQuery.unique(jQuery.map(e.data, function (n, i) {
                return (n.Product);
            }));
            //gets the unique transaction dates from the dtaaset
            groupDates(e.data);
        }, $scope.Filter);
    }
    //gets the product wise opening balance as of the selected date provided in the filter
    function getPartyOpeningBalance() {
        ledgerDTO.PartyOpeningBalance(function (e) {
            $scope.OpeningBalance = e.data;
        }, $scope.Filter);
    }
    function groupDates(data) {
        $scope.StockDates = jQuery.unique(jQuery.map(data, function (n, i) {
            return n.TransDate;
        }));
    }
    $scope.PartyForStockSelect = function (obj) {
        if (obj != undefined) {
            $scope.Filter.LedgerId = obj.originalObject.LedgerId;
        }
    }
    $scope.PrintStockRegister = function () {
    }
    //gets the transaction Challans for the issue and received items
    $scope.getTransactionChallans = function (obj, transType) {
        //gets the unique challans
        var challans = jQuery.unique(jQuery.map(jQuery.grep($scope.PartyStockData, function (n, i) {

            return (n.TransDate == obj && n.TranType == transType);
        }), function (n, i) {
            return n.JobNumber;
        }));
        if (challans.length == 0) {
            challans.push("-");
        }
        return challans;
    }
    $scope.getIssuedProductQtyOnDate = function (product, date) {
        return jQuery.map(jQuery.grep($scope.PartyIssueTans, function (n, i) {
            return (n.Product == product && n.TransDate == date);
        }), function (n, i) {
            return n.Quantity;
        });
    }
    $scope.getRecvdProductQtyOnDate = function (product, date) {
        return jQuery.map(jQuery.grep($scope.PartyRecTans, function (n, i) {
            return (n.Product == product && n.TransDate == date);
        }), function (n, i) {
            return n.Quantity;
        });
    }
    $scope.dateWiseProductsQtyTotal = function (date, tranType) {
        var totalQty = 0;
        for (var i = 0; i <= $scope.PartyStockData.length - 1; i++) {
            var obj = $scope.PartyStockData[i];
            if (obj.TransDate == date && obj.TranType == tranType) {
                totalQty += parseFloat(obj.Quantity);
            }
        }

        return totalQty;
    }
    $scope.ProductsQtyTotal = function (product) {
        var totalQty = 0;
        for (var i = 0; i <= $scope.PartyStockData.length - 1; i++) {
            var obj = $scope.PartyStockData[i];
            if (obj.Product == product) {
                if (obj.TranType == 1) {
                    totalQty += parseFloat(obj.Quantity);
                }
                else {
                    totalQty -= parseFloat(obj.Quantity);
                }
               
            }
           
        }
        totalQty += calculateOpeningBalance(product)
        return totalQty;
    }
    $scope.netClosingBal = function () {
        var totalQty = 0;
        for (var i = 0; i <= $scope.PartyStockData.length - 1; i++) {
            var obj = $scope.PartyStockData[i];
            if (obj.TranType == 1) {
                totalQty += parseFloat(obj.Quantity);
            }
            else {
                totalQty -= parseFloat(obj.Quantity);
            }
            
        }
        for (var j = 0; j < $scope.OpeningBalance.length; j++) {
            var obj = $scope.OpeningBalance[j];
            var prodExists = $scope.PartyStockData.find(o=> o.Product == obj.Product);
            debugger
            if(prodExists != undefined) {
                totalQty += parseFloat(obj.Quantity);
            }
        }
        return totalQty;
    }
    $scope.ProductOpeningBalance = function (product) {
        return calculateOpeningBalance(product);
    }
    function calculateOpeningBalance(product) {
        var openingBal = 0;
        for (var i = 0; i < $scope.OpeningBalance.length; i++) {
            var obj = $scope.OpeningBalance[i];
          
            if (obj.Product == product) {
                return obj.Quantity;
            }
        }
        return openingBal;
    }
    $scope.PartyStockRegisterPrint = function() {
        ledgerDTO.PartyStockRegisterPrint(function (e) {
           
            var filePath = SERVER_URL + 'temp/' + e.data;
            // $window.target = '_blank';
            $window.open(filePath);
        },$scope.Filter);
    }
    //--- end of stock register functions.

    $scope.Save = function () {
        debugger
        var m = $('#form-client').valid();
        if (m) {
            EnableToolbar(0);
            ledgerDTO.Add(function (e) {
                EnableToolbar(1);
                showMessage(MessageClass.COMPANY_SAVED);
            });
        }
    }
    $scope.RowSelected = function (index) {



    }
});