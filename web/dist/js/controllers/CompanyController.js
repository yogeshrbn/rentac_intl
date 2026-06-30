app.controller("CompanyController", ['$scope', '$rootScope', '$state', '$stateParams', '$http',
    'CompanyService', 'ModalFactory', 'WarehouseService', 'CompanyService',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, ModalFactory, WarehouseService, CompanyService
    ) {


        var cId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
        var companyDTO = new $.Company({ CompanyId: cId });
        $scope.isBranch = $state.current.data.isBranch;
        FormsValidation.init();
        $scope.Companies = companyDTO;
        var token = $rootScope.getTokenInfo();

        $scope.Search = { Name: '' }

        function BindList() {
            if ($scope.isBranch == true) {
                companyDTO.GetBranches(function (e) {
                    $scope.Companies = e.data;
                }, token.DefaultCompanyId);

            }

            else {
                CompanyService.getAllCompanies().then(function (e) {
                    $scope.Companies = e.data;
                });
            }
        }

        $scope.selectedWarehouses = [];

        WarehouseService.getWarehouses().then(function (response) {
            $scope.warehouses = response.data.Data;
            if (cId == 0) {
                BindList();
            }
            else {
                CompanyService.getCompanyDetails({ CompanyId: cId }).then(function (e) {
                    $scope.Company = companyDTO = new $.Company(e.data);

                    $scope.Signature = ''
                    $scope.QrCode = ''
                    $scope.Logo = ''

                    if (e.data.Signature && e.data.Signature.length > 3) {
                        $scope.Signature = e.data.Signature;
                    }
                    if (e.data.QrCode && e.data.QrCode.length > 3) {
                        $scope.QrCode = e.data.QrCode;
                    }
                    if (e.data.Logo && e.data.Logo.length > 3) {
                        $scope.Logo = e.data.Logo;
                    }
                    var warehouseIds = e.data.Warehouses;
                    if (warehouseIds) {

                        var selectedWarehouses = warehouseIds.split(',').map(item => item.trim());
                        if ($scope.warehouses) {
                            angular.forEach(selectedWarehouses, function (value, index) {
                                var wh = $scope.warehouses.find(o => o.WarehouseId == value);
                                if (wh) {
                                    $scope.selectedWarehouses.push(wh);
                                }
                            });
                        }
                    }

                });


            }
        });
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

            if ($scope.logoChanged == false) {
                companyDTO.Props.Logo = 'no';
            }
            if ($scope.SignatureChanged == false) {
                companyDTO.Props.Signature = 'no';
            }
            if ($scope.QrCodeChanged == false) {
                companyDTO.Props.QrCode = 'no';
            }
            var m = $('#form-company').valid();
            var data = cloneObj($scope.Company.Props);
            if ($scope.selectedWarehouses) {
                data.Warehouses = $.map($scope.selectedWarehouses, function (val) {
                    return val.WarehouseId;
                }).join(',');

            }


            if (m) {
                EnableToolbar(0);
                companyDTO.Add(function (e) {
                    EnableToolbar(1);

                    if (e.data.Code == 200) {
                        showMessage(MessageClass.COMPANY_SAVED);
                        $state.go('companylist');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, data);
            }
        }
        $scope.RowSelected = function (index) {



        }
        $scope.find = function () {

            CompanyService.SearchCompany(function (e) {
                if (e.status == 200) {
                    $scope.Companies = e.data.items;
                }
                else {
                    alert(e.data.Description);
                }

            }, $scope.Search);
        }


        $scope.browseFile = function (id) {

            $(id).click();
        };

        $scope.logoChanged = false;
        $scope.Logo = '';
        $scope.SignatureChanged = false;
        $scope.Signature = '';
        $scope.QrCode = '';
        $scope.QrCodeChanged = false;
        $scope.onFileBrowse = function (event) {

            readURL(event.currentTarget, function (e) {

                if (e._target.id == 'fileProfile') {
                    $scope.logoChanged = true;

                    $scope.Company.Props.Logo = e.data;
                    $scope.Logo = e.data;
                }
                else if (e._target.id == 'fileSignature') {
                    $scope.SignatureChanged = true;
                    $scope.Company.Props.Signature = e.data;
                    $scope.Signature = e.data;
                }
                else if (e._target.id == 'qrCode') {
                    $scope.QrCodeChanged = true;
                    $scope.Company.Props.QrCode = e.data;
                    $scope.QrCode = e.data;
                }
                $scope.$apply();

            });
        };
        function readURL(input, onRead) {

            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {

                    // $('#imgProfile').attr('src', e.target.result);
                    onRead.call(null, { _target: input, data: e.target.result });

                }

                if (input.files[0].size > 200000) {
                    alert('Uploaded File Size can not exceed 200 KB');
                    return;
                }
                var fileType = input.files[0].type;
                if (fileType == "image/png" || fileType == "image/jpeg" || fileType == "image/jpg") {
                    var dataUrl = reader.readAsDataURL(input.files[0]);
                    return dataUrl;
                }
                else {
                    alert('Uploaded File must be either png, or jpeg or jpg');
                    return;
                }


            }
        }

        $scope.removeLogo = function (fileId) {
            var deleteController = function ($scope) {
                $scope.ShowRemarks = false;
                $scope.Message = 'Are you sure to remove';
                $scope.OkButtonClick = function () {
                    if (fileId == 'fileSignature') {
                        $scope.SignatureChanged = true;
                        $scope.Company.Props.Signature = 'delete';
                        $scope.Signature = '';
                    }
                    if (fileId == 'fileProfile') {
                        $scope.logoChanged = true;
                        $scope.Company.Props.Logo = 'delete';
                        $scope.Logo = '';
                    }
                    if (fileId == 'qrCode') {
                        $scope.QrCodeChanged = true;
                        $scope.Company.Props.QrCode = 'delete';
                        $scope.QrCode = '';
                    }
                    ModalFactory.Dialog.hide();
                };
                $scope.closeDialog = function () {
                    ModalFactory.Dialog.hide();
                };
            }
            ModalFactory.Confirm(deleteController, $scope, $('body'));


        }

        $scope.verifyGST = function () {
            var company = new $.Company();

            var test = REGEX.GST.test($scope.Company.Props.GSTNo);
            if (!test) {
                alert('Please enter a valid GSTIN');
                return;
            }
            company.GetTaxPayerDetails(function (e) {
                var res = e.data;;

                if (res.Code == 200) {
                    var data = res.Data;
                    alert('GST Status: ' + data.GSTStatus);
                    $scope.Company.Props.Name = data.LegalName;
                    $scope.Company.Props.TradeName = data.TradeName;

                    $scope.Company.Props.Address1 = data.Address1;
                    $scope.Company.Props.Address2 = data.Address2;
                    $scope.Company.Props.City = data.City;
                    $scope.Company.Props.StateId = data.StateId;
                    $scope.Company.Props.ZipCode = data.ZipCode;





                } else {
                    alert(res.Message);
                }

            }, $scope.Company.Props.GSTNo);
        }

    }]);

app.controller("LedgerListController", ['$scope', '$http', '$location', 'LedgerFactory', 'AccountService', '$state', '$timeout',
    function ($scope, $http, $location, LedgerFactory, AccountService, $state, $timeout) {
        var ledgerDTO = new $.Ledger({ LedgerId: 0 });
        var CLIENT_LIST_FILTER_KEY = 'rentacClientListFilter_v1';

        function persistClientListFilter() {
            try {
                sessionStorage.setItem(CLIENT_LIST_FILTER_KEY, JSON.stringify({
                    Search: angular.copy($scope.Search),
                    CurrentPage: $scope.CurrentPage
                }));
            } catch (ex) { }
        }

        function persistClientListWithScrollTarget(ledgerId) {
            try {
                sessionStorage.setItem(CLIENT_LIST_FILTER_KEY, JSON.stringify({
                    Search: angular.copy($scope.Search),
                    CurrentPage: $scope.CurrentPage,
                    pendingScrollToLedgerId: ledgerId
                }));
            } catch (ex) { }
        }

        function restoreClientListFilter() {
            try {
                var raw = sessionStorage.getItem(CLIENT_LIST_FILTER_KEY);
                return raw ? JSON.parse(raw) : null;
            } catch (ex) {
                return null;
            }
        }

        function afterClientListLoaded() {
            var pendingId = null;
            try {
                var st = restoreClientListFilter();
                if (st && st.pendingScrollToLedgerId) {
                    pendingId = st.pendingScrollToLedgerId;
                }
            } catch (ex) { }
            persistClientListFilter();
            if (pendingId) {
                $timeout(function () {
                    var el = document.getElementById('client-row-' + pendingId);
                    if (el && typeof el.scrollIntoView === 'function') {
                        el.scrollIntoView({ block: 'center', behavior: 'smooth' });
                    }
                }, 150);
            }
        }

        var savedClientFilter = restoreClientListFilter();
        $scope.Search = { Name: '', Code: '', Phone1: '', AccountGroup: 0 };
        if (savedClientFilter && savedClientFilter.Search) {
            angular.extend($scope.Search, savedClientFilter.Search);
        }
        $scope.CurrentPage = 1;
        if (savedClientFilter && savedClientFilter.CurrentPage > 0) {
            $scope.CurrentPage = savedClientFilter.CurrentPage;
        }
        $scope.Ledgers = [];
        $scope.onSortChange = function (e) {

        }
        AccountService.getAllAccountGroups().then((e) => {
            $scope.AccGroups = e.data;
        });
        function BindList() {
            var leder = new $.Ledger();
            leder.SearchClient(function (e) {
                $scope.Ledgers = e.data.items;
                afterClientListLoaded();
            }, { Name: '' });
        }

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
        $scope.Remove = function (ledgerId) {
            var confMsg = 'All the data and information will be removed for this party\n and this action is not revokeable\n\n';
            confMsg += ' Are you sure to remove this party?';
            var conf = confirm(confMsg);
            if (!conf) {
                return;
            }

            LedgerFactory.Remove(function (e) {
                if (e.data.Code == 200) {
                    BindList();
                }
                else {
                    alert(e.data.Description);
                }

            }, { LedgerId: ledgerId });
        }
        $scope.find = function (page) {
            if (page == null || page === undefined) {
                $scope.CurrentPage = 1;
            } else {
                $scope.CurrentPage = page;
            }
            LedgerFactory.SearchClient(function (e) {
                if (e.status == 200) {
                    $scope.Ledgers = e.data.items;
                    afterClientListLoaded();
                }
                else {
                    alert(e.data.Description);
                }

            }, $scope.Search);
        };

        $scope.onPageChange = function () {
            persistClientListFilter();
        };

        $scope.goNewAccount = function ($event) {
            if ($event) {
                $event.preventDefault();
            }
            persistClientListFilter();
            $state.go('addeditclient');
        };

        $scope.goEditAccount = function ($event, ledgerId) {
            if ($event) {
                $event.preventDefault();
            }
            persistClientListWithScrollTarget(ledgerId);
            $state.go('addeditclient/:cId', { cId: ledgerId });
        };

        if (savedClientFilter) {
            $scope.find($scope.CurrentPage > 0 ? $scope.CurrentPage : 1);
        } else {
            BindList();
        }
    }]);
app.controller("LedgerController", ['$scope', '$stateParams', '$rootScope', '$location', '$http', '$window', '$filter', 'LedgerFactory',
    function ($scope, $stateParams, $rootScope, $location, $http, $window, $filter, LedgerFactory) {

        var cId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
        var ledgerDTO = new $.Ledger({ LedgerId: cId });
        var groupDTO = new $.AccountGroup({ AccountGroupId: 0 });
        var billingAddress = new $.Address({});
        var shippingAddress = new $.Address({});
        var objUOM = new $.UOM({ UOMId: 0 });
        FormsValidation.init();
        $scope.Ledger = ledgerDTO;
        $scope.Ledger.Props.BillingAddress = billingAddress;
        $scope.Ledger.Props.ShippingAddress = shippingAddress;
        $scope.Ledger.Props.CreditDays = 0;



        ledgerDTO.GetDetails(function (e) {

            $scope.Ledger = ledgerDTO = new $.Ledger(e.data);
            if (e.data.BillingAddress == null) {
                $scope.Ledger.Props.BillingAddress = billingAddress;
            }
            if (e.data.ShippingAddress == null) {
                $scope.Ledger.Props.ShippingAddress = shippingAddress;
            }
            getAllProducts();
            objUOM.GetAllUOM(function (e) {
                $scope.UOM = e.data;
            });
        });
        // }
        //get all ledger accounts if not in edit or add mode. This is for listing of all ledgers
        //  if ($routeParams.cId == undefined) {
        groupDTO.GetAll(function (e) {
            $scope.AccGroups = e.data;
        });
        // }

        //list of all products of the company selected
        function getAllProducts() {
            ledgerDTO.GetProductRates(function (e) {

                $scope.Ledger.Products = e.data;
            });
        }

        //-- stock register functions and members
        //-- filter for party wise stock register
        var token = $rootScope.getTokenInfo();
        var date = new Date();
        var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }


        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {

            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.PartyStockData = [];
        $scope.OpeningBalance = [];
        // $scope.Filter = filter;
        //gets the stock register for the party

        $scope.GetPartyStockRegister = function () {

            getPartyOpeningBalance();
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);

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
                //debugger
                ////get the maximum unique products to create rows.
                //$scope.MaxRows = jQuery.unique(jQuery.map(e.data, function (n, i) {
                //    return (n.Product);
                //}));
                $scope.MaxRows = [];
                $.grep(e.data, function (el, index) {
                    if ($.inArray(el.Product, $scope.MaxRows) < 0) {
                        $scope.MaxRows.push(el.Product);
                    }
                    return true;
                });

                //gets the unique transaction dates from the dtaaset
                groupDates(e.data);
            }, model);
        }
        //gets the product wise opening balance as of the selected date provided in the filter
        function getPartyOpeningBalance() {
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            ledgerDTO.PartyOpeningBalance(function (e) {
                $scope.OpeningBalance = e.data;
            }, model);
        }
        function groupDates(data) {

            $scope.IssuedDates = uniqueItems(jQuery.map($scope.PartyIssueTans, function (n, i) {
                return n.TransDate;
            }));

            $scope.RecvedDates = uniqueItems(jQuery.map($scope.PartyRecTans, function (n, i) {
                return n.TransDate;
            }));
        }
        //$scope.PartyForStockSelect = function (obj) {
        //    if (obj != undefined) {
        //        $scope.Filter.LedgerId = obj.originalObject.LedgerId;
        //    }
        //}
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });

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
            var date = convertDate(date);

            return jQuery.map(jQuery.grep($scope.PartyIssueTans, function (n, i) {
                var tDate = convertDate(n.TransDate);
                return (n.Product == product && tDate == date);
            }), function (n, i) {
                return n;
            });
        }
        $scope.getRecvdProductQtyOnDate = function (product, date) {
            return jQuery.map(jQuery.grep($scope.PartyRecTans, function (n, i) {
                return (n.Product == product && n.TransDate == date);
            }), function (n, i) {
                return n;
            });
        }
        $scope.ProductWiseProductsQtyTotal = function (product, tranType) {
            var totalQty = 0;

            for (var i = 0; i <= $scope.PartyStockData.length - 1; i++) {
                var obj = $scope.PartyStockData[i];
                if (obj.Product == product && obj.TranType == tranType) {

                    totalQty += parseFloat(obj.Quantity) + parseFloat(obj.Breakage);
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
                var prodExists = $scope.PartyStockData.find(o => o.Product == obj.Product);

                if (prodExists != undefined) {
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
        $scope.PartyStockRegisterPrint = function () {
            ledgerDTO.PartyStockRegisterPrint(function (e) {

                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);
            }, $scope.Filter);
        }
        //--- end of stock register functions.

        $scope.Save = function () {

            var m = $('#form-client').valid();
            if (m) {
                EnableToolbar(0);
                ledgerDTO.Add(function (e) {
                    if (e.data.Code == "500") {
                        showMessage(e.data.Description);
                        return
                    }
                    EnableToolbar(1);
                    showMessage(MessageClass.ACCOUNT_SAVED);
                    $location.path('/clientlist');
                });
            }
        }
        $scope.RowSelected = function (index) {



        }

        $scope.SaveRates = function () {
            ;
            //    var s = JSON.stringify($scope.Ledger);
            $scope.Ledger.AddProductRates(function (e) {

                if (e.data == true) {
                    showMessage('Rates has been updated.');
                    // $location.path('/clientlist');
                }
            });

        }
        //used to copy billing address to the shipping address
        //used in add/edit client page.

        $scope.copyBillingAddress = function () {

            $scope.Ledger.Props.ShippingAddress.Address1 = $scope.Ledger.Props.BillingAddress.Address1;
            $scope.Ledger.Props.ShippingAddress.Address2 = $scope.Ledger.Props.BillingAddress.Address2;
            $scope.Ledger.Props.ShippingAddress.Phone1 = $scope.Ledger.Props.BillingAddress.Phone1;
            $scope.Ledger.Props.ShippingAddress.Phone2 = $scope.Ledger.Props.BillingAddress.Phone2;
            $scope.Ledger.Props.ShippingAddress.City = $scope.Ledger.Props.BillingAddress.City;
            $scope.Ledger.Props.ShippingAddress.Email = $scope.Ledger.Props.BillingAddress.Email;
            $scope.Ledger.Props.ShippingAddress.Fax = $scope.Ledger.Props.BillingAddress.Fax;
            $scope.Ledger.Props.ShippingAddress.Web = $scope.Ledger.Props.BillingAddress.Web;

            $scope.Ledger.Props.ShippingAddress.State = $scope.Ledger.Props.BillingAddress.State;

        }

        $scope.ChangeDefaultRentRate = function () {

            if (isNaN(parseInt($scope.Ledger.Props.DefaultRate, 0))) {
                $scope.Ledger.Props.DefaultRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].RentRate = $scope.Ledger.Props.DefaultRate;
            }

        }
        $scope.ChangeDefaultLossRate = function () {
            if (isNaN(parseInt($scope.Ledger.Props.DefaultLossRate, 0))) {
                $scope.Ledger.Props.DefaultLossRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].LossRate = $scope.Ledger.Props.DefaultLossRate;
            }

        }
        $scope.ChangeDefaultDmgRate = function () {
            if (isNaN(parseInt($scope.Ledger.Props.DefaultDmgRate, 0))) {
                $scope.Ledger.Props.DefaultDmgRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].DamageRate = $scope.Ledger.Props.DefaultDmgRate;
            }

        }

       

    }]);
//app.controller("LedgerListController", ['$scope', '$http', '$location', 'LedgerFactory', function ($scope, $http, $location, LedgerFactory) {
//    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
//    BindList();
//    function BindList() {
//        ledgerDTO.GetAll(function (e) {
//            $scope.Ledgers = e.data;
//        });
//    }
//    $scope.Activate = function (isActive, ledgerId) {
//        if (isActive == 0) {
//            if (!confirm('Are you sure to de-activate the account?')) {
//                return;
//            }
//        }
//        ledgerDTO.Props.LedgerId = ledgerId;
//        ledgerDTO.Props.IsActive = isActive;
//        ledgerDTO.ActivateDeActivate(function (e) {
//            BindList();
//        });
//    }
//    $scope.Remove = function (ledgerId) {
//        var confMsg = 'All the data and information will be removed for this party\n and this action is not revokeable\n\n';
//        confMsg += ' Are you sure to remove this party?';
//        var conf = confirm(confMsg);
//        if (!conf) {
//            return;
//        }

//        LedgerFactory.Remove(function (e) {
//            if (e.data.Code == 200) {
//                BindList();
//            }
//            else {
//                alert(e.data.Description);
//            }

//        }, { LedgerId: ledgerId });
//    }
//}]);
app.controller("AddEditClientController", ['$scope', '$stateParams', '$rootScope', '$location', '$http', '$window',
    '$filter', 'LedgerFactory',
    function ($scope, $stateParams, $rootScope, $location, $http, $window, $filter, LedgerFactory) {

        var cId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
        var ledgerDTO = new $.Ledger({ LedgerId: cId });
        var groupDTO = new $.AccountGroup({ AccountGroupId: 0 });
        var billingAddress = new $.Address({});
        var shippingAddress = new $.Address({});
        var objUOM = new $.UOM({ UOMId: 0 });
        FormsValidation.init();
        $scope.Ledger = ledgerDTO;
        $scope.Ledger.Props.BillingAddress = billingAddress;
        $scope.Ledger.Props.ShippingAddress = shippingAddress;

        $scope.AccountGroup = '';
        $scope.QrCode = '';

        if (cId > 0) {
            ledgerDTO.GetDetails(function (e) {

                if (e.data == null) {
                    return;
                }
                $scope.Ledger = ledgerDTO = new $.Ledger(e.data);
                if (e.data.BillingAddress == null) {
                    $scope.Ledger.Props.BillingAddress = billingAddress;
                }
                if (e.data.ShippingAddress == null) {
                    $scope.Ledger.Props.ShippingAddress = shippingAddress;
                }
                $scope.AccountGroup = e.data.GroupName;
            });
        }
        // }
        //get all ledger accounts if not in edit or add mode. This is for listing of all ledgers
        //  if ($routeParams.cId == undefined) {
        groupDTO.GetAll(function (e) {
            $scope.AccGroups = e.data;
        });
        // }

        //list of all products of the company selected
        //function getAllProducts() {
        //    ledgerDTO.GetProductRates(function (e) {

        //        $scope.Ledger.Products = e.data;
        //    });
        //}

        //-- stock register functions and members
        //-- filter for party wise stock register
        var token = $rootScope.getTokenInfo();
        var date = new Date();
        var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }

        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            if ($scope.Filter.LedgerId > 0)
                getSites();
        });
        function getSites() {

            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.PartyStockData = [];
        $scope.OpeningBalance = [];
        // $scope.Filter = filter;
        //gets the stock register for the party

        function groupDates(data) {
            $scope.IssuedDates = jQuery.unique(jQuery.map($scope.PartyIssueTans, function (n, i) {
                return n.TransDate;
            }));
            $scope.RecvedDates = jQuery.unique(jQuery.map($scope.PartyRecTans, function (n, i) {
                return n.TransDate;
            }));
        }
        //$scope.PartyForStockSelect = function (obj) {
        //    if (obj != undefined) {
        //        $scope.Filter.LedgerId = obj.originalObject.LedgerId;
        //    }
        //}
        var ledger = new $.Ledger({});

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

        $scope.Save = function () {

            var m = $('#form-client').valid();
            if (m) {
                EnableToolbar(0);
                ledgerDTO.Add(function (e) {
                    if (e.data.Code == "500") {
                        showMessage(e.data.Message);
                        return
                    }
                    EnableToolbar(1);
                    showMessage(MessageClass.ACCOUNT_SAVED);
                    $location.path('/clientlist');
                });
            }
        }
        $scope.RowSelected = function (index) {



        }

        //used to copy billing address to the shipping address
        //used in add/edit client page.

        $scope.copyBillingAddress = function () {

            $scope.Ledger.Props.ShippingAddress.Address1 = $scope.Ledger.Props.BillingAddress.Address1;
            $scope.Ledger.Props.ShippingAddress.Address2 = $scope.Ledger.Props.BillingAddress.Address2;
            $scope.Ledger.Props.ShippingAddress.Phone1 = $scope.Ledger.Props.BillingAddress.Phone1;
            $scope.Ledger.Props.ShippingAddress.Phone2 = $scope.Ledger.Props.BillingAddress.Phone2;
            $scope.Ledger.Props.ShippingAddress.City = $scope.Ledger.Props.BillingAddress.City;
            $scope.Ledger.Props.ShippingAddress.Email = $scope.Ledger.Props.BillingAddress.Email;
            $scope.Ledger.Props.ShippingAddress.Fax = $scope.Ledger.Props.BillingAddress.Fax;
            $scope.Ledger.Props.ShippingAddress.Web = $scope.Ledger.Props.BillingAddress.Web;
            $scope.Ledger.Props.ShippingAddress.ZipCode = $scope.Ledger.Props.BillingAddress.ZipCode;

            $scope.Ledger.Props.ShippingAddress.StateId = $scope.Ledger.Props.BillingAddress.StateId;

        }

        $scope.ChangeDefaultRentRate = function () {

            if (isNaN(parseInt($scope.Ledger.Props.DefaultRate, 0))) {
                $scope.Ledger.Props.DefaultRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].RentRate = $scope.Ledger.Props.DefaultRate;
            }

        }
        $scope.ChangeDefaultLossRate = function () {
            if (isNaN(parseInt($scope.Ledger.Props.DefaultLossRate, 0))) {
                $scope.Ledger.Props.DefaultLossRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].LossRate = $scope.Ledger.Props.DefaultLossRate;
            }

        }
        $scope.ChangeDefaultDmgRate = function () {
            if (isNaN(parseInt($scope.Ledger.Props.DefaultDmgRate, 0))) {
                $scope.Ledger.Props.DefaultDmgRate = 0;
            }
            for (var i = 0; i < $scope.Ledger.Products.length; i++) {
                $scope.Ledger.Products[i].DamageRate = $scope.Ledger.Props.DefaultDmgRate;
            }

        }


        $scope.verifyGST = function () {
            var company = new $.Company();

            var test = REGEX.GST.test($scope.Ledger.Props.GSTNo);
            if (!test) {
                alert('Please enter a valid GSTIN');
                return;
            }
            company.GetTaxPayerDetails(function (e) {
                var res = e.data;;

                if (res.Code == 200) {
                    var data = res.Data;
                    alert('GST Status: ' + data.GSTStatus);
                    $scope.Ledger.Props.Name = data.LegalName;
                    $scope.Ledger.Props.TradeName = data.TradeName;
                    if (!$scope.Ledger.Props.BillingAddress) {
                        $scope.Ledger.Props.BillingAddress = billingAddress;
                    }
                    $scope.Ledger.Props.BillingAddress.Address1 = data.Address1;
                    $scope.Ledger.Props.BillingAddress.Address2 = data.Address2;
                    $scope.Ledger.Props.BillingAddress.City = data.City;
                    $scope.Ledger.Props.BillingAddress.StateId = data.StateId;
                    $scope.Ledger.Props.BillingAddress.ZipCode = data.ZipCode;





                } else {
                    alert(res.Message);
                }

            }, $scope.Ledger.Props.GSTNo);
        }
         
        $scope.Config = {
            partycodegenmode: 'manual'
        };
        var config = new $.Config();
        config.GetByCategory(function (e) {
            var res = e.data;
            if (res && res.Data) {
                var partycodegenmode = res.Data.find(o => o.SubCategory == 'masters' &&
                    o.Category == 'general' && o.Key == 'partycodegenmode');
                if (partycodegenmode) {
                    $scope.Config.partycodegenmode = partycodegenmode.Value;
                }
            }
        }, 'general');

    }]);

app.controller("ClientSiteController", ['$scope', '$rootScope', '$stateParams', 'ModalFactory', 'LedgerFactory', 'FileSaver', '$http', function ($scope, $rootScope, $stateParams, ModalFactory, LedgerFactory, FileSaver, $http) {

    var ledgerId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
    var filter = { LedgerId: +ledgerId };

    $scope.siteDocName = function (site, slot) {
        if (!site) return '';
        switch (slot) {
            case 1: return site.Document1FileName;
            case 2: return site.Document2FileName;
            case 3: return site.Document3FileName;
            case 4: return site.Document4FileName;
            case 5: return site.Document5FileName;
            default: return '';
        }
    };

    $scope.downloadSiteDocument = function (site, slot) {
        var fn = $scope.siteDocName(site, slot);
        if (!fn) return;
        var url = API_URL + 'Ledger/DownloadSiteDocument?ledgerId=' + encodeURIComponent(ledgerId) + '&ledgerSiteId=' + encodeURIComponent(site.LedgerSiteId) + '&slot=' + encodeURIComponent(slot);
        $http.get(url, { responseType: 'blob' }).then(function (res) {
            FileSaver.saveAs(res.data, fn || 'document');
        }, function (err) {
            alert(err && err.data && err.data.Message ? err.data.Message : 'Download failed');
        });
    };


    $scope.NewSite = function () {
        openSiteDialog({ LedgerId: ledgerId, LedgerSiteId: 0, Ledger: $scope.Ledger });
    };
    function openSiteDialog(data) {
        ModalFactory.AddEditClientSite('AddEditClientSiteController', data);
    }

    var onSiteAdded = $rootScope.$on("OnSiteAdded", function (evt, data) {

        $scope.getSites();
    });
    $scope.$on('$destroy', function () {
        onSiteAdded();
    });

    $scope.getSites = function () {
        filter.Closed = 1;
        LedgerFactory.GetMasterSites(function (e) {

            $scope.Sites = e.data.Data;
            $scope.Ledger = e.data.Extra;
        }, filter);
    }
    $scope.getSites();
    $scope.EditSite = function (site) {

        openSiteDialog({ LedgerId: ledgerId, LedgerSiteId: site.LedgerSiteId });

    }
    $scope.CloseSite = function (site) {

        var model = { LedgerId: ledgerId, LedgerSiteId: site.LedgerSiteId, Remarks: '' };

        var closeSiteController = function ($scope) {
            $scope.dlg = { Remarks: '' };
            $scope.ShowRemarks = true;
            $scope.dlg.Remarks = '';
            if (site.Closed == 1) {
                $scope.Message = 'Are you sure to open this site';
            }
            else {
                $scope.Message = 'Are you sure to Close this site';
            }
            $scope.OkButtonClick = function () {

                if ($scope.dlg.Remarks.length < 5) {
                    alert('Please enter remrkas');
                    return;
                }
                model.ClosedRemarks = $scope.dlg.Remarks;
                var ledger = new $.Ledger();
                ledger.CloseSite(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    
                    $scope.closeDialog();
                    $scope.getSites();
                }, model);
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(closeSiteController, $scope);

    }
}]);
app.controller("AddEditClientSiteController", ['$scope', '$mdDialog', 'LedgerFactory', 'localData', 'EmployeeService', 'FileSaver', '$http',
    function ($scope, $mdDialog, LedgerFactory, localData, EmployeeService, FileSaver, $http) {
    $scope.TAXES = StaicData.TAX_CATEGORY;
    $scope.AllBillingSites = [];
    $scope.removeDoc = [false, false, false, false, false];
    $scope.siteDocName = function (slot) {
        var site = $scope.Site;
        if (!site) return '';
        switch (slot) {
            case 1: return site.Document1FileName;
            case 2: return site.Document2FileName;
            case 3: return site.Document3FileName;
            case 4: return site.Document4FileName;
            case 5: return site.Document5FileName;
            default: return '';
        }
    };
    $scope.downloadSiteDocument = function (slot) {
        var fn = $scope.siteDocName(slot);
        if (!fn || !localData.LedgerId || !($scope.Site && $scope.Site.LedgerSiteId)) return;
        var url = API_URL + 'Ledger/DownloadSiteDocument?ledgerId=' + encodeURIComponent(localData.LedgerId) + '&ledgerSiteId=' + encodeURIComponent($scope.Site.LedgerSiteId) + '&slot=' + encodeURIComponent(slot);
        $http.get(url, { responseType: 'blob' }).then(function (res) {
            FileSaver.saveAs(res.data, fn || 'document');
        }, function () {
            alert('Download failed');
        });
    };
    console.log(localData);
    window.setTimeout(() => {
        FormsValidation.init('form-clientsite');
    }, 100);
    $scope.OkButtonClick = function () {
        saveSite();
    }
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    var filter = { LedgerSiteId: localData.LedgerSiteId };
    $scope.Site = { PrintLastBillDetails: 0, PrintBalanceMaterial: 0, UseForBilling: 0, BillingToSiteId: 0 };
    function getTaxes() {
        LedgerFactory.GetSiteTaxes(function (e) {
            $scope.Site.Taxes = e.data.Data;
        }, filter);
    }

    if (filter.LedgerSiteId > 0) {
        LedgerFactory.GetMasterSiteById(function (e) {
            $scope.Site = e.data.Data;
            $scope.Site.BillingToSiteId = $scope.Site.BillingToSiteId || 0;
            $scope.Site.PODate = convertDate($scope.Site.PODate);
            getTaxes();
        }, filter);
    }
    else {
        getTaxes();

        if (localData.Ledger) {
            $scope.Site.ContactPerson = localData.Ledger.ContactPersonName;
            $scope.Site.ContactPersonPhone = localData.Ledger.ContactPersonMobile;

            $scope.Site.City = localData.Ledger.City;

        }

    }
    function saveSite() {
        var m = $('#form-clientsite').valid();
        if (!m) {
            return;
        }

        $scope.Site.LedgerId = localData.LedgerId;
        $scope.Site.LedgerSiteId = localData.LedgerSiteId;
        var model = cloneObj($scope.Site);
        model.BillingToSiteId = model.BillingToSiteId > 0 ? model.BillingToSiteId : null;
        if (model.PODate) {
            model.PODate = formatdate(model.PODate);
        }
        if ($scope.removeDoc) {
            for (var ri = 0; ri < 5; ri++) {
                if ($scope.removeDoc[ri]) {
                    if (ri === 0) model.Document1FileName = null;
                    else if (ri === 1) model.Document2FileName = null;
                    else if (ri === 2) model.Document3FileName = null;
                    else if (ri === 3) model.Document4FileName = null;
                    else if (ri === 4) model.Document5FileName = null;
                }
            }
        }
        var docFiles = [];
        for (var di = 1; di <= 5; di++) {
            var fel = document.getElementById('siteDocFile' + di);
            docFiles.push(fel && fel.files && fel.files[0] ? fel.files[0] : null);
        }
        LedgerFactory.AddMasterSite(function (e) {
            if (e.data.Code == "200") {
                $scope.$emit('OnSiteAdded', { D: 1 });
                $mdDialog.hide();
            }
            else {
                alert(e.data.Message);
            }
        }, model, docFiles);
    }
  

    function getAllEmployees() {
        EmployeeService.GetAllEmployees(function (e) {
            $scope.Employees = e.data.Data;
        });
    }
    function getAllBillingSites() {
        var ledger = new $.Ledger({});
        ledger.AllClientsWithSites(function (e) {
            var sites = (e && e.data && e.data.Data) ? e.data.Data : [];
            $scope.AllBillingSites = [{
                LedgerSiteId: 0,
                DisplaySite: '- Select -'
            }];
            for (var i = 0; i < sites.length; i++) {
                var s = sites[i];
                var name = s.Name || '';
                var project = s.Project || '';
                var address = s.SiteAddress || '';
                var parts = [];
                if (name) {
                    parts.push(name);
                }
                if (project) {
                    parts.push(project);
                }
                if (address) {
                    parts.push(address);
                }
                s.DisplaySite = parts.join(' - ');
                $scope.AllBillingSites.push(s);
            }
        });
    }
    getAllEmployees();
    getAllBillingSites();

}]);

app.controller("AddEditLedgerController", ['$scope', '$mdDialog', 'LedgerFactory', 'localData', function ($scope, $mdDialog, LedgerFactory, localData) {

    $scope.OkButtonClick = function () {
        saveLedger();
    }
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    var filter = { LedgerId: localData.LedgerId };
    $scope.Ledger = {
        LedgerId: localData.LedgerId, ShippingAddress: {}, forQuotation: localData.forQuotation
        , BillingAddress: {}
    };

    if (localData.presetPartyName) {
        $scope.Ledger.Name = localData.presetPartyName;
    }
    if (localData.presetPartyAddress) {
        $scope.Ledger.BillingAddress = $scope.Ledger.BillingAddress || {};
        $scope.Ledger.BillingAddress.Address1 = localData.presetPartyAddress;
    }
    if (localData.presetPartyPhone) {
        $scope.Ledger.Phone1 = localData.presetPartyPhone;
    }

    if (filter.LedgerId > 0) {
        //TODO: Edit Ledger case
        //LedgerFactory.GetMasterSiteById(function (e) {
        //    $scope.Site = e.data.Data;
        //    getTaxes();
        //}, filter);
    }

    function saveLedger() {
        var m = $('#form-client').valid();
        if (!m) {
            return;
        }
        LedgerFactory.AddLedger(function (e) {
            var newId = null;
            if (typeof e.data === 'number' && e.data > 0) {
                newId = e.data;
            } else if (e.data && (e.data.Code === 200 || e.data.Code === '200')) {
                newId = e.data.Data;
            }
            if (newId) {
                $scope.$root.$broadcast('OnLedgerAdded', {
                    Code: $scope.Ledger.Code,
                    LedgerId: newId,
                    partyName: $scope.Ledger.Name,
                    quotationIdForLink: localData.quotationIdForLink
                });
                $mdDialog.hide();
            }
            else {
                alert((e.data && e.data.Message) ? e.data.Message : 'Could not save party');
            }
        }, $scope.Ledger);
    }
    $scope.copyBillingAddress = function () {

        $scope.Ledger.ShippingAddress.Address1 = $scope.Ledger.BillingAddress.Address1;
        $scope.Ledger.ShippingAddress.Address2 = $scope.Ledger.BillingAddress.Address2;
        $scope.Ledger.ShippingAddress.Phone1 = $scope.Ledger.BillingAddress.Phone1;
        $scope.Ledger.ShippingAddress.Phone2 = $scope.Ledger.BillingAddress.Phone2;
        $scope.Ledger.ShippingAddress.City = $scope.Ledger.BillingAddress.City;
        $scope.Ledger.ShippingAddress.Email = $scope.Ledger.BillingAddress.Email;
        $scope.Ledger.ShippingAddress.Fax = $scope.Ledger.BillingAddress.Fax;
        $scope.Ledger.ShippingAddress.Web = $scope.Ledger.BillingAddress.Web;
        $scope.Ledger.ShippingAddress.ZipCode = $scope.Ledger.BillingAddress.ZipCode;

        $scope.Ledger.ShippingAddress.StateId = $scope.Ledger.BillingAddress.StateId;

    }

    FormsValidation.init('form-client');

    $scope.Config = {
        partycodegenmode: 'manual'
    };
    var config = new $.Config();
    config.GetByCategory(function (e) {
        var res = e.data;
        if (res && res.Data) {
            var partycodegenmode = res.Data.find(o => o.SubCategory == 'masters' &&
                o.Category == 'general' && o.Key == 'partycodegenmode');
            if (partycodegenmode) {
                $scope.Config.partycodegenmode = partycodegenmode.Value;
            }
        }
    }, 'general');
}]);


app.controller("EmployeeListController", ['$scope', '$rootScope', 'EmployeeService', 'ModalFactory', function ($scope, $rootScope, EmployeeService, ModalFactory) {
    $scope.New = function () {
        openDialog({ EmployeeId: 0 });
    }
    $scope.Edit = function (item) {
        openDialog({ EmployeeId: item.EmployeeId });
    }
    function getAll() {
        EmployeeService.GetAllEmployees(function (e) {
            debugger
            if (e.data.Code == 200) {
                $scope.Employees = e.data.Data;
            }
            else {
                alert(e.data.Message);
            }
        });
    }
    function openDialog(data) {
        ModalFactory.AddEditEmployee('AddEditEmployeeController', data);
    }
    var onEmpAdded = $rootScope.$on("OnEmployeeAdded", function (evt, data) {
        getAll();
    });
    $scope.$on('$destroy', function () {
        onEmpAdded();
    });
    getAll();
}]);
app.controller("AddEditEmployeeController", ['$scope', '$mdDialog', 'LookupService', 'EmployeeService', 'localData', function ($scope, $mdDialog, LookupService, EmployeeService, localData) {
    window.setTimeout(() => {
        FormsValidation.init('form-employee');
    }, 100);


    $scope.OkButtonClick = function () {
        save();
    }
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    function save() {

        var m = $('#form-employee').valid();
        if (!m) {
            return;
        }
        EmployeeService.Add(function (e) {
            if (e.data.Code == "200") {
                $scope.$emit('OnEmployeeAdded', { D: 1 });
                $mdDialog.hide();
            }
            else {
                alert(e.data.Message);
            }
        }, $scope.Employee);

    }
    LookupService.GetAllEmployeeRoles(function (e) {
        $scope.Roles = e.data.Data;
    });

    if (localData.EmployeeId > 0) {
        EmployeeService.GetInfo(function (e) {
            $scope.Employee = e.data.Data;
        }, localData);
    }
}]);
app.controller("VehicleListController", ['$scope', '$rootScope', 'CompanyService', 'ModalFactory',
    function ($scope, $rootScope, CompanyService, ModalFactory) {
        $scope.New = function () {
            openDialog({ VehicleId: 0 });
        }
        $scope.Edit = function (item) {
            openDialog({ VehicleId: item.VehicleId });
        }
        function getAll() {
            CompanyService.getAllVehicle().then(function (e) {
                $scope.Vehicles = e.data.Data;
            });
        }
        function openDialog(data) {
            ModalFactory.AddEditVehicle('AddEditVehicleController', data);
        }
        var onAdded = $rootScope.$on("OnAdded", function (evt, data) {
            getAll();
        });
        $scope.$on('$destroy', function () {
            onAdded();
        });
        getAll();
    }]);
app.controller("AddEditVehicleController", ['$scope', '$mdDialog', 'LookupService', 'CompanyService', 'localData', function ($scope, $mdDialog, LookupService, CompanyService, localData) {
    window.setTimeout(() => {
        FormsValidation.init('form-vehicle');
    }, 100);
    $scope.OkButtonClick = function () {
        save();
    }
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    function save() {
        var m = $('#form-vehicle').valid();
        if (!m) {
            return;
        }
        CompanyService.addVehicle($scope.Vehicle).then(function (e) {
            if (e.data.Code == "200") {
                $scope.$emit('OnAdded', { D: 1 });
                $mdDialog.hide();
            }
            else {
                alert(e.data.Message);
            }
        });

    }


    if (localData.VehicleId > 0) {
        CompanyService.getVehicleInfo(localData).then(function (e) {
            $scope.Vehicle = e.data.Data;
        });
    }
}]);
app.controller("PartyWiseRateListController", ['$scope', '$rootScope', 'LedgerFactory', function ($scope, $rootScope, LedgerFactory) {

    var ledger = new $.Ledger({});
    $scope.Ledger = ledger;
    $scope.Accounts = [];
    $scope.copyToSelected = [];
    $scope.CopyRatesModel = { Rent: true, Loss: true, Breakge: true, RentRatePerSize: true, CopyToAll: false, CopyFromSiteId: 0, CopyTo: [] };
    $scope.itemListFilter = '';
    $scope.changeRatesForm = {
        mode: 'percent',
        direction: 'increase',
        value: '',
        applyRent: true,
        applyLoss: true,
        applyBreakage: true,
        applySizeRate: true
    };
    $scope.fileName = 'No file choosen';
    getAllProductSizesByCompany();
    // var newRates;
    ledger.AllClientsWithSites(function (e) {
 
        $scope.Accounts = e.data.Data;
    
        if (e.data && e.data.length > 0) {
            $scope.GetRates(e.data[0].LedgerId, e.data[0].LedgerSiteId);
        }

    });
    
 
   
    $scope.onRatesPartySelected = function (e) {
        $scope.GetRates(e.LedgerId, e.LedgerSiteId);
    }
    $scope.onCopyPartySelected = function (e) {
        $scope.CopyRatesModel.CopyFromSiteId = e.LedgerSiteId;
    }
    $scope.copyRates = function () {

        $scope.CopyRatesModel.CopyTo = $scope.copyToSelected.map((o) => {
            return { LedgerSiteId: o.LedgerSiteId, LedgerId: o.LedgerId };
        });
        var model = cloneObj($scope.CopyRatesModel);
        if (model.CopyTo.length == 0) {
            alert('Please select copy to parties.');
            return;
        }
        if (model.CopyFromSiteId == 0) {
            alert('Please select copy from party.');
            return;
        }
        ledger.CopyProductRates(function (e) {
            if (e.data.Code == 200) {
                alert('Rates copied successfully.');
                $('#copyModal').modal('hide');
                return;
            }
            alert(e.data.Message);
        }, model);
    }

    $scope.resetChangeRatesForm = function () {
        $scope.changeRatesForm = {
            mode: 'percent',
            direction: 'increase',
            value: '',
            applyRent: true,
            applyLoss: true,
            applyBreakage: true,
            applySizeRate: true
        };
    };

    $scope.changeRatesSelectAllTypes = function () {
        $scope.changeRatesForm.applyRent = true;
        $scope.changeRatesForm.applyLoss = true;
        $scope.changeRatesForm.applyBreakage = true;
        $scope.changeRatesForm.applySizeRate = true;
    };

    $scope.applyBulkRateChange = function () {
        var rates = $scope.Rates;
        if (!rates || !rates.length) {
            alert('No rates loaded to adjust.');
            return;
        }
        var rawStr = $scope.changeRatesForm.value;
        if (rawStr === null || rawStr === undefined || String(rawStr).trim() === '') {
            alert('Please enter a value.');
            return;
        }
        var raw = parseFloat(String(rawStr).replace(/,/g, '').trim());
        if (isNaN(raw) || raw < 0) {
            alert('Please enter a valid non-negative number.');
            return;
        }
        var mode = $scope.changeRatesForm.mode;
        var dir = $scope.changeRatesForm.direction;
        var form = $scope.changeRatesForm;
        var fieldDefs = [
            { prop: 'applyRent', field: 'RentRate' },
            { prop: 'applyLoss', field: 'LossRate' },
            { prop: 'applyBreakage', field: 'DamageRate' },
            { prop: 'applySizeRate', field: 'UnitSizeRate' }
        ];
        var fields = [];
        angular.forEach(fieldDefs, function (d) {
            if (form[d.prop]) {
                fields.push(d.field);
            }
        });
        if (!fields.length) {
            alert('Please select at least one rate type to change (or use Select all).');
            return;
        }

        function adjustBase(baseVal) {
            var b = parseFloat(baseVal);
            if (isNaN(b)) {
                b = 0;
            }
            var r;
            if (mode === 'percent') {
                if (dir === 'increase') {
                    r = b * (1 + raw / 100);
                } else {
                    r = b * (1 - raw / 100);
                }
            } else {
                if (dir === 'increase') {
                    r = b + raw;
                } else {
                    r = b - raw;
                }
            }
            if (r < 0) {
                r = 0;
            }
            return Math.round(r * 100) / 100;
        }

        angular.forEach(rates, function (item) {
            angular.forEach(fields, function (f) {
                item[f] = adjustBase(item[f]);
            });
        });

        $('#changeRatesModal').modal('hide');
        $scope.resetChangeRatesForm();
    };

    $scope.RowSelected = function (index) {
        if ($scope.Accounts) {
            var ledgerId = $scope.Accounts[index].LedgerId;
            //   $scope.GetRates(ledgerId);
        }
    }
    $scope.browseRateFile = function () {

        $('#rateFile').click();
    }


    $scope.itemSelected = function (itemId) {
        //here itemId is selected index


        if (itemId <= 0) return;
        var index = itemId - 1; //-- subtract the index of first element in the dropdown which is "Please select";
        //  $scope.IssueItem.ProductId = itemId;
        //var item = $scope.ProductRates.find(o=>o.ProductId == itemId);
        var item = $scope.AllSizes[index];
        $scope.newitem.ProductId = item.ProductId;

        $scope.newitem.Product = item.Product;
        // findDefaultRate($scope.IssueItem.ProductId);
        //   alert(item.ProductSizeId);
        $scope.newitem.ProductSizeId = item.ProductSizeId;
        //$scope.ProductSizes=[{}];
        //$scope.ProductSizes =  $.map($scope.AllSizes,function(value,key) {
        //    if(value.ProductId ==itemId) {
        //        return value;
        //    }
        //});

        //$scope.ItemInStock = $filter('sumByKey')($filter('filter')($scope.ItemStock, { ProductId: $scope.IssueItem.ProductId }), 'Quantity');
        //$scope.PartyItemInStock = $filter('sumByKey')($filter('filter')($scope.PartyBalance, { ProductId: $scope.IssueItem.ProductId }), 'ClosingBalance');

    };
    $scope.GetRates = function (ledgerId, ledgerSiteId,) {
        $scope.itemListFilter = '';
        $scope.Ledger.LedgerId = ledgerId;
        $scope.Ledger.LedgerSiteId = ledgerSiteId;
        $scope.Ledger.Props.LedgerId = ledgerId;
        //var ledgerId  = 0;
        //if($scope.Filter != undefined){
        //    ledgerId = $scope.Filter.LedgerId;
        //}
        LedgerFactory.PartyWiseRates(function (e) {
            $scope.Rates = e.data;

            //if (e.data && e.data.length > 0) {
            //    $scope.hasData = true;
            //}
            $scope.hasData = e.data && e.data.length > 0;
            //if ($scope.hasData) {
            //    angular.forEach(e.data, function (value, key) {
            //        $scope.Rates[key].selectedProduct = $scope.AllSizes.find(x => x.ProductId == $scope.Rates[key].ProductId); 
            //    });
            //}
            // $scope.NewRates = e.data;
        }, { LedgerId: ledgerId, LedgerSiteId: ledgerSiteId });

    }
    $scope.Export = function () {
        console.log($scope.Rates);
    };
    $scope.SaveRates = function () {

        //$scope.Ledger.Rates = $scope.Rates;
        $scope.Ledger.Products = $scope.Rates;
        var model = cloneObj($scope.Ledger.Props);

        model.LedgerId = $scope.Ledger.Props.LedgerId;
        model.LedgerSiteId = $scope.Ledger.LedgerSiteId;
        model.Rates = $scope.Rates;
        //   model.Products = $scope.Rates;

        //    var s = JSON.stringify($scope.Ledger);
        $scope.Ledger.UpdateProductRates(function (e) {

            if (e.data == true) {
                showMessage('Rates has been updated.');
                //  $location.path('/clientlist');
            }
        }, $scope.Ledger);

    }
    setNewItem();
    function setNewItem() {
        $scope.newitem = {
            RentRate: 0, LossRate: 0, DamageRate: 0, ProductId: 0, ProductSizeId: 0, Product: "", ProductCode: null, selectedProduct: null
        };
    }

    $scope.AddNew = function () {
        $scope.Rates = ($scope.Rates != null) ? $scope.Rates : [];
        if ($scope.newitem.selectedProduct != null) {
            //$scope.newitem.ProductCode = $scope.newitem.selectedProduct.originalObject.Code;
            //$scope.newitem.ProductId = $scope.newitem.selectedProduct.originalObject.ProductId;
            //$scope.newitem.Product = $scope.newitem.selectedProduct.originalObject.Product;
            //$scope.newitem.ProductSizeId = $scope.newitem.selectedProduct.originalObject.ProductSizeId;
            pushNewRate();
            if ($scope.newitem != null) {
                $scope.Rates.push($scope.newitem);
                setNewItem();
                $scope.SaveRates();
            }

        } else {
            alert('Please select Product !');
        }


    };
    function pushNewRate() {
        if ($scope.newitem.selectedProduct != null) {
            $scope.newitem.ProductCode = $scope.newitem.selectedProduct.originalObject.Code;
            $scope.newitem.ProductId = $scope.newitem.selectedProduct.originalObject.ProductId;
            $scope.newitem.Product = $scope.newitem.selectedProduct.originalObject.Product;
            $scope.newitem.ProductSizeId = $scope.newitem.selectedProduct.originalObject.ProductSizeId;
            //  $scope.Rates.push($scope.newitem);
        }
    }
    $scope.processImportData = function (exceljson) {


        $scope.Rates = ($scope.Rates != null) ? $scope.Rates : [];

        for (var i = 0; i < exceljson.length; i++) {
            var existProd = $scope.Rates.find(x => x.ProductCode == exceljson[i].ProductCode);
            var rentRate = Math.trunc(parseFloat(exceljson[i].RentRate) * 100) / 100;
            var lossRate = Math.trunc(parseFloat(exceljson[i].LossRate) * 100) / 100;;
            var damageRate = Math.trunc(parseFloat(exceljson[i].DamageRate) * 100) / 100;

            if (existProd) {
                existProd.RentRate = rentRate;
                existProd.LossRate = lossRate;
                existProd.DamageRate = damageRate;

            } else {
                var prod = $scope.AllSizes.find(x => x.Code == exceljson[i].ProductCode);
                $scope.Rates.push({
                    RentRate: rentRate, LossRate: lossRate, DamageRate: damageRate,
                    ProductId: prod.ProductId, Product: prod.Product, ProductCode: prod.ProductCode,
                    LedgerId: $scope.Ledger.LedgerId, Unit: 0
                });
            }
            //$scope.students.push(exceljson[i]);

        }
        // $scope.$apply();

        $scope.SaveRates();
    }
    $scope.FileSelected = function (file) {
        // alert('File selected');
        $scope.fileName = file.name;
        $scope.ReadExcelData(file);
        angular.element("input[type='file']").val(null);

    };

    $scope.ReadExcelData = function (file) {
        var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.xlsx|.xls)$/;
        /*Checks whether the file is a valid excel file*/
        if (file.name.toLowerCase()) {
            var xlsxflag = false; /*Flag for checking whether excel is .xls format or .xlsx format*/
            if (file.name.toLowerCase().indexOf(".xlsx") > 0) {
                xlsxflag = true;
            }
            /*Checks whether the browser supports HTML5*/
            if (typeof (FileReader) != "undefined") {
                var reader = new FileReader();
                reader.onload = function (e) {
                    // debugger;
                    var data = e.target.result;
                    /*Converts the excel data in to object*/
                    if (xlsxflag) {
                        var workbook = XLSX.read(data, { type: 'binary' });
                    }
                    else {
                        var workbook = XLS.read(data, { type: 'binary' });
                    }
                    /*Gets all the sheetnames of excel in to a variable*/
                    var sheet_name_list = workbook.SheetNames;
                    var cnt = 0; /*This is used for restricting the script to consider only first sheet of excel*/
                    sheet_name_list.forEach(function (y) { /*Iterate through all sheets*/
                        /*Convert the cell value to Json*/
                        if (xlsxflag) {
                            var exceljson = XLSX.utils.sheet_to_json(workbook.Sheets[y]);
                        }
                        else {
                            var exceljson = XLS.utils.sheet_to_row_object_array(workbook.Sheets[y]);
                        }
                        if (exceljson.length > 0) {
                            $scope.processImportData(exceljson);

                        }
                    });
                }
                if (xlsxflag) {/*If excel file is .xlsx extension than creates a Array Buffer from excel*/
                    reader.readAsArrayBuffer(file);
                }
                else {
                    reader.readAsBinaryString(file);
                }
            }
            else {
                alert("Sorry! Your browser does not support HTML5!");
            }
        }
        else {
            alert("Please upload a valid Excel file!");
        }
    }


    function getAllProductSizesByCompany() {
        var product = new $.Product();
        product.GetSizeListByCompany(function (e) {

            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
}]);
app.controller("EstimatedRentController", ['$scope', '$rootScope', 'LedgerFactory', function ($scope, $rootScope, LedgerFactory) {

    var ledger = new $.Ledger({});
    var date = new Date();
    var token = $rootScope.getTokenInfo();
    // debugger
    $scope.Filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter.To = convertDate(date);
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    ledger.GetAll(function (e) {
        $scope.Accounts = e.data;

    });
    $scope.GetRent = function () {

        var ledgerId = 0;
        if ($scope.Filter != undefined) {
            ledgerId = $scope.Filter.LedgerId;
        }
        LedgerFactory.EstimatedRentPerDay(function (e) {
            $scope.Rent = e.data;
        }, $scope.Filter);

    }

}]);
