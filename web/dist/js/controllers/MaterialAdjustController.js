app.controller('MaterialAdjustListController', ['$scope', '$rootScope', '$state', 'LedgerFactory',
    'EmployeeService', 'CompanyService', '$crypto', 'ModalFactory', 'ReportService',
    function ($scope, $rootScope, $state, LedgerFactory, EmployeeService, CompanyService, $crypto, ModalFactory, ReportService) {
        init();
        function init() {
            var ledger = new $.Ledger({});
            $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: '', To: convertDate(new Date()) };
            FormsValidation.init();
            var token = $rootScope.getTokenInfo();
            if (token) {
                $scope.Filter.From = convertDate(token.FinYearStart);
            }
            ledger.GetAll(function (e) {
                $scope.Accounts = e.data;
                if (e.data != null && e.data.length > 0) {
                    $scope.initialValue = e.data[0];
                }
            });
        }
        $scope.$watch('Filter.LedgerId', function () {
            getSites();
        });
        $scope.selectedParty = function (selected) {

            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Filter.LedgerId = item.LedgerId;

            }
        };
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }

        $scope.Find = function () {
            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);

            var wo = new $.WorkOrder({});
            wo.AdjustMaterialList(function (e) {

                $scope.List = e.data;
            }, filter);
        }
        $scope.edit = function (item) {

            var key = item.WorkOrderId.toString();
            var _encKey = $crypto.encrypt(key);
            $state.go('matadjustedit', { key: _encKey });
        };

        $scope.delete = function (item) {
            $scope.Message = 'Are you sure to delete this material adjustment?';
            ModalFactory.ConfirmDelete($scope, function () {
                var wd = new $.WorkOrder();
                var payload = {
                    WorkOrderId: item.WorkOrderId,
                    SiteId: item.SiteId,
                    LedgerId: item.LedgerId,
                    LedgerSiteId: item.LedgerSiteId
                };
                wd.DeleteChallan(function (e) {
                    if (e.data && e.data.Code != 200) {
                        alert(e.data.Message || 'Delete failed');
                        return;
                    }
                    ModalFactory.hideDialog();
                    $scope.Find();
                }, payload);
            });
        };

        $scope.PrintReceipt = function (item) {
            if (!item || !item.WorkOrderId) {
                return;
            }
            var strInput = 'materialadjust,' + item.WorkOrderId;
            var encrypedText = $crypto.encrypt(strInput);
            var safeNo = (item.Number || String(item.WorkOrderId)).replace(/[^\w\-]+/g, '_');
            ReportService.printFromReportServer(encrypedText, 'mat-adjust-' + safeNo + '.pdf');
        };

        $scope.PrintSelected = function () {
            if (!$scope.List || $scope.List.length === 0) {
                return;
            }
            var selected = $.grep($scope.List, function (n) { return n.Print === true; });
            if (selected.length === 0) {
                alert('Please select at least one row to print.');
                return;
            }
            for (var i = 0; i < selected.length; i++) {
                $scope.PrintReceipt(selected[i]);
            }
        };
    }]);
app.controller('MaterialAdjustController', ['$scope', '$rootScope', '$stateParams', 'LedgerFactory',
    'EmployeeService', '$state', '$crypto', '$q',
    function ($scope, $rootScope, $stateParams, LedgerFactory, EmployeeService, $state, $crypto, $q) {

        var tId = $stateParams.key == undefined ? 0 : $stateParams.key;
        var workOrderId = 0, siteId = 0;
        if (tId) {

            workOrderId = $crypto.decrypt(tId);
        }
        var _date = new Date();
        var edit = false;
        $scope.edit = edit;

        $scope.Adjust = { WorkOrderDate: convertDate(_date), AdjType: 1 };
        $scope.IssueItem = { ClosingBalance: 0, ProductId: 0, SentQty: 0 };
        $scope.RecItem = {};
        $scope.IssueList = [];
        $scope.ReceiveList = [];

        var token = $rootScope.getTokenInfo();
        if (token != null) {
            $scope.MinDate = token.FinYearStart;

            // convertDate(token.FinYearEnd);
        }
        init();

        $rootScope.$on('onToolbarClick', function (e, args) {

            nav_onToolbarClick(args);
        });
        nav_Items = [];

        // nav_init();
        function nav_onToolbarClick(args) {
            switch (args) {
                case 'first': nav_first(); nav_onItemSelection(); break;
                case 'previous': nav_previous(); nav_onItemSelection(); break;
                case 'next': nav_next(); nav_onItemSelection(); break;
                case 'last': nav_last(); nav_onItemSelection(); break;
                case 'search': nav_search(); break;
                case 'print': nav_print(); break;
                case 'edit': edit = !edit;
                    $scope.edit = edit;
                    setInterval(function () {
                        $scope.$apply()
                    }, 50); break;
            }

            console.log('Navigation command', args, nav_selected);

        }
        function nav_init() {

            // feth list set nav_items
            //
            var w = new $.WorkOrder();
            w.GetWorkOrdersByCompany(function (e) {
                console.log('Material Adjust', e);
                nav_Items = e.data;
                nav_count = nav_Items.length;
                //nav_first();
                //nav_onItemSelection();
            }, $scope.Filter);


            //nav_first();
            //nav_onItemSelection();
            nav_initialized = true;

        }
        //nav_init();

        function nav_search() {
            alert('nav_search');
        }
        function nav_print() {
            alert('nav_print');
        }
        $scope.getAllProductSizesByCompany = function () {
            var product = new $.Product();
            product.GetSizeListByCompany(function (e) {
                //debugger
                console.log('AllSizes', e.data);
                $scope.AllSizes = e.data;

            });
        }
        $scope.getAllProductSizesByCompany();
        function nav_onItemSelection() {

            if (nav_selectedIndex > -1) {

                //$scope.WorkOrder.LedgerId = nav_selected.LedgerId;
                //$scope.WorkOrder.SiteInfo.LedgerSiteId = nav_selected.LedgerSiteId;
                //$scope.GRN.WorkOrderId = nav_selected.WorkOrderId;
                //grnId = nav_selected.GrnId;
                //  tid = nav_selected.WorkOrderId;

                edit = false;
                $scope.edit = false;
                setInterval(function () {
                    init();
                    $scope.$apply()
                }, 200);
            }
        }
        $scope.IssueList = [];
        $scope.ReceiveList = [];
        $scope.Balance = [];
        $scope.GetClientWiseItems = function (cb) {

            if ($scope.Filter.LedgerSiteId == 0) {
                return;
            }
            $scope.ClientWiseItemsList = [];
            var ledger = new $.Ledger();
            $scope.Filter.To = convertDate(new Date())
            ledger.GetClientWiseItems(function (e) {
                if (e.data.length == 0) {
                    return;
                }
                $scope.Balance = e.data;
                if ($scope.Adjust.WorkOrderId == 0) {

                    $scope.ReceiveList = e.data.filter(o => o.ClosingBalance > 0);
                    $scope.IssueList = e.data.filter(o => o.ExcessQty > 0);
                }

            }, $scope.Filter);
        }

        nav_init();




        function init() {
            var ledger = new $.Ledger({});
            $scope.Filter = {
                LedgerId: 0, LedgerSiteId: 0, From: '', To: convertDate(new Date()),
                code: 'MaterialAdjust'
            };
            FormsValidation.init();
            var token = $rootScope.getTokenInfo();
            if (token) {
                $scope.Filter.From = convertDate(token.FinYearStart);
            }
            ledger.GetAll(function (e) {
                $scope.Accounts = e.data;
            });
            getById();
        }
        function getById() {
            var wo = new $.WorkOrder();
            var ledger = new $.Ledger({});
            //wo.TransactionId = tId;          
            wo.WorkOrderId = workOrderId;
            $scope.Adjust.WorkOrderId = workOrderId;
            if (workOrderId > 0) {

                wo.AdjustMaterialById(function (e) {
                    var data = e.data;
                    if (data.Code == 200) {
                        if (data.Data != null && data.Data.length > 0) {
                            $scope.Filter.LedgerId = data.Data[0].LedgerId;
                            $scope.Filter.LedgerSiteId = data.Data[0].LedgerSiteId;

                            $scope.Adjust = data.Data[0];
                            var _at = parseInt($scope.Adjust.AdjType, 10);
                            $scope.Adjust.AdjType = (!isNaN(_at) && _at >= 1 && _at <= 2) ? _at : 1;
                            $scope.Adjust.WorkOrderDate = convertDate(data.Data[0].WorkOrderDate);
                            var newList = $.grep(data.Data, function (n, i) {
                                n.SentQty = n.Quantity;
                                return n.WorkOrderItemId > 0;
                            });
                            $scope.IssueList = JSON.parse(JSON.stringify(newList));
                            var newRecList = $.grep(data.Data, function (n, i) {
                                return n.GRNItemId > 0;
                            });
                            $scope.ReceiveList = newRecList;
                            $scope.GetClientWiseItems();
                        }
                    }
                }, $scope.Adjust);

            }
        }

        $scope.$watch('Filter.LedgerId', function () {
            $scope.Adjust.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };

        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.$watch('Filter.LedgerSiteId', function () {
            $scope.Adjust.LedgerSiteId = $scope.Filter.LedgerSiteId;
            //getSiteBalance();
            if ($scope.Adjust.WorkOrderId == 0)
                $scope.GetClientWiseItems();

        });
        function getSiteBalance() {

            LedgerFactory.PartyStockBalance_BySize($scope.Filter, function (e) {
                $scope.Balance = e.data;
            });
        }


        $scope.RecitemSelected = function (index) {

            if (index == undefined) {
                return;
            }
            var item = index.originalObject;
            if (item) {
                var bItem = $scope.Balance.find(o => o.ProductId == item.ProductId);
                //if (bItem == null) {
                //    $scope.RecItem = {};
                //    alert('Item not found');
                //    return;
                //}
                //if (bItem.ClosingBalance <= 0) {
                //    $scope.RecItem = {};
                //    alert('Item balance is 0. Can not receive.');
                //    return;
                //}
                var cb = 0;
                if (bItem) {
                    $scope.IssueItem.ClosingBalance = cb = bItem.ClosingBalance;
                }
                var toAdd = {
                    ProductId: item.ProductId, Product: item.Product,
                    ProductSizeId: item.ProductSizeId, ClosingBalance: cb, Quantity: ''
                };
                $scope.RecItem = toAdd;

            }
        }

        $scope.addToRecList = function (index) {

            if ($scope.RecItem.Quantity > 0 && $scope.RecItem.Product) {
                var found;
                if ($scope.ReceiveList.length > 0) {
                    found = $scope.ReceiveList.find(o => o.ProductId == $scope.RecItem.ProductId &&
                        o.ProductSizeId == $scope.RecItem.ProductSizeId);
                }
                if (found) {
                    found.Quantity += $scope.RecItem.Quantity;
                }
                else {
                    $scope.ReceiveList.push($scope.RecItem);
                }
                $scope.RecItem = {};
                $('#itemRec_value').val('');
                $('#itemRec_value').focus();
                //$scope.RecItem.ProductId = 0;
            }
        };
        $scope.remove = function (index) {
            $scope.ReceiveList.splice(index, 1);
        }

        //----Issue
        $scope.IssueitemSelected = function (index) {

            if (index == undefined) {
                return;
            }
            var item = index.originalObject;
            if (item) {
                var bItem = $scope.Balance.find(o => o.ProductId == item.ProductId);
                //if (bItem == null) {
                //    $scope.IssueItem = {};
                //    alert('Item not found');
                //    return;
                //}
                var cb = 0;
                if (bItem) {
                   $scope.IssueItem.ClosingBalance = cb = bItem.ClosingBalance;
                }
                var toAdd = {
                    ProductId: item.ProductId, Product: item.Product, ProductSizeId: item.ProductSizeId,
                    ClosingBalance: cb, Quantity: ''
                };
                $scope.IssueItem = toAdd;

            }
        }
        $scope.addToIssueList = function (index) {

            if ($scope.IssueItem.SentQty > 0 && $scope.IssueItem.Product) {
                var found;
                if ($scope.IssueList.length > 0) {
                    found = $scope.IssueList.find(o => o.ProductId == $scope.IssueItem.ProductId &&
                        o.ProductSizeId == $scope.IssueItem.ProductSizeId);
                }
                if (found) {
                    found.SentQty += $scope.IssueItem.Quantity;
                }
                else {
                    $scope.IssueList.push($scope.IssueItem);
                }
                $scope.IssueItem = {};
                $('#itemIssue_value').val('');
                $('#itemIssue_value').focus();
                //$scope.RecItem.ProductId = 0;
            }
        };
        $scope.removeIssue = function (index) {
            $scope.IssueList.splice(index, 1);
        }
        //--- end of issue
        $scope.SaveAdjustment = function () {

            var m = $('#form-mat').valid();
            if (!m) return;
            $scope.Adjust.IssueList = $scope.IssueList;
            $scope.Adjust.ReceiveList = $scope.ReceiveList;

            //if ($scope.Adjust.IssueList == null || $scope.Adjust.IssueList.length == 0 ||
            //    $scope.Adjust.ReceiveList == null || $scope.Adjust.ReceiveList.length == 0) {
            //    alert('Please provide issue and receive items');
            //    return;
            //}
            //either items can be issued or received
            if (($scope.Adjust.IssueList == null && $scope.Adjust.IssueList.length == 0) &&
                $scope.Adjust.ReceiveList == null && $scope.Adjust.ReceiveList.length == 0) {
                alert('Please provide issue or receive items');
                return;
            }
            var wo = new $.WorkOrder({});
            var model = cloneObj($scope.Adjust);
            model.WorkOrderDate = formatdate(model.WorkOrderDate);
            wo.AdjustMaterial(function (e) {
                if (e.data.Code == 200) {
                    alert('Material Adjusted');
                    $state.go('matAdjstList');
                }
                else if (e.data.Code == 500) {
                    alert(e.data.Message);
                }
            }, model);
        }
    }]);