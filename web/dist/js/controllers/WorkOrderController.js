app.controller('PartyStockBalanceController', ['$scope', '$rootScope', '$window', '$routeParams', '$http', '$mdDialog', '$rootScope', 'LedgerFactory',
    function ($scope, $rootScope, $window, $routeParams, $http, $mdDialog, $rootScope, LedgerFactory) {
        var date = new Date();
        var ledger = new $.Ledger({});

        var token = $rootScope.getTokenInfo();

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
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });


        $scope.GetBalance = function () {
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            ledger.StockBalance(function (e) {
                $scope.BalanceItems = e.data;
                //var sites = $.map(e.data, function (index, val) {
                //    return new { LedgerSiteId: val.LedgerSiteId, SiteAddress: val.SiteAddress };
                //});
                //$scope.Sites = uniqueObjects(sites, 'LedgerSiteId');
            }, model);
        }

        $scope.Print = function () {
            ledger.Print_StockBalance_report(function (e) {
                //  debugger
                var filePath = SERVER_URL + 'temp/' + e.data;

                $window.open(filePath);

            }, $scope.Filter);
        }


    }]);



app.controller('AccountLedger', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory) {
        //receipt or payment type
        debugger;
        var ledger = new $.Ledger({});

        $scope.TransList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.GetAccountLedger = function () {
            ledger.GetAccountLedger(function (e) {

                $scope.TransList = e.data;

                $scope.LastRow = e.data[e.data.length - 1];
            }, $scope.Filter);
        }
        $scope.PrintLedger = function () {
            ledger.GetAccountLedger_rpt(function (e) {
                //debugger
                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);

            }, $scope.Filter);
        }


        $scope.SubTotal = function () {
            var total = 0;
            if ($scope.TransList != undefined) {
                for (var i = 0; i <= $scope.TransList.length - 1; i++) {
                    total += parseFloat($scope.TransList[i].TransactionAmount);
                }
            }
            return total;
        }

    }]);

//--DEbit/Credit Notes List Controller
app.controller('DrCrNotesListController', ['$scope', '$location', '$state', '$http', '$mdDialog', '$rootScope',
    '$window',
    'ModalFactory', 'LedgerFactory', 'ReportService', '$crypto',
    function ($scope, $location, $state, $http, $mdDialog, $rootScope, $window, ModalFactory, LedgerFactory, ReportService, $crypto) {

        $scope.TranType = $state.current.data.txnType;
        $scope.EntryType = $state.current.data.entryType;
        //  $scope.TranType = $routeParams.type == undefined ? 3 : parseInt($routeParams.type);
        $scope.Ledger = new $.LedgerTrasaction({});
        var date = new Date();
        var token = $rootScope.getTokenInfo();

        if (token != null)
            $scope.MinDate = token.FinYearStart;

        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, EntryType: $scope.EntryType, From: convertDate($scope.MinDate), To: convertDate(date), TransactionType: $scope.TranType };
        $scope.Ledger.To = convertDate(date);
        if (token) {
            $scope.Ledger.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Ledger.LedgerId = $rootScope.LedgerId;
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Ledger.LedgerId', function () {
            $rootScope.LedgerId = $scope.Ledger.LedgerId;
            getSites();
        });
        $scope.GetNotes = function () {
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);

            $scope.Ledger.GetDrCrNotes(function (e) {
                if ($scope.EntryType == 10) {
                    $scope.Notes = e.data.Data.filter(o => o.EntryType == 10 || o.EntryType == 15);
                }
                if ($scope.EntryType == 13) {
                    $scope.Notes = e.data.Data.filter(o => o.EntryType == 13 || o.EntryType == 16);
                }
            }, model);
        }

        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Ledger.LedgerId = item.LedgerId;
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Ledger.LedgerId });
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            //if (e.data != null && e.data.length > 0) {
            //    $scope.initialValue = e.data[0];
            //}
            //if ($scope.Ledger.LedgerId == null) {
            //    $scope.Ledger.LedgerId = e.data[0].LedgerId;
            //}
        });

        function okbutton(modalScope) {
            $scope.printNote(modalScope.LedgerTransactionId);
        }

        $scope.printNote = function (transactionId) {

            LedgerFactory.PrintTransaction(function (e) {
                if (e.data.Code != '500') {
                    ReportService.Print(e.data.Data);
                }
                else {
                    // $scope.printNote($scope.LedgerTransactionId);
                    $scope.Message = e.data.Description;
                    //ModalFactory.Info('DialogController', $scope);
                }
            }, { LedgerTransactionId: transactionId });
        }

        $scope.edit = function (txnId) {
            var _key = $crypto.encrypt(txnId);
            if ($scope.EntryType == 10) {

                $state.go('editdrNote', { key: _key });

            }
            else {
                $state.go('editcrNote', { key: _key });
            }

        }

    }]);

//--Debit/Credit Notes controller
app.controller('DrCrNotesController', ['$scope', '$state', '$stateParams', 'ModalFactory', 'LedgerFactory', 'ReportService', '$crypto',
    function ($scope, $state, $stateParams, ModalFactory, LedgerFactory, ReportService, $crypto) {

        //  $scope.TranType = $routeParams.type == undefined ? 3 : parseInt($routeParams.type);
        $scope.TranType = $state.current.data.txnType;
        $scope.EntryType = $state.current.data.entryType;
        var txnId = $stateParams.key;

        $scope.Items = [];
        init();
        if (txnId != undefined) {
            $scope.Trans.LedgerTransactionId = $crypto.decrypt(txnId);
            if ($scope.Trans.LedgerTransactionId > 0) {
                var objTxn = new $.LedgerTrasaction();
                objTxn.GetTransactionByForEditId(function (e) {

                    if (e.status == 200) {
                        $scope.Trans = e.data.txn;
                        $scope.Trans.TransactionDate = convertDate(e.data.txn.TransactionDate);
                        // $scope.TransList.push(e.data);
                    }
                    else {
                        alert('Could not fetch data');
                        $state.go('quickReceipts');
                    }

                }, { LedgerTransactionId: $scope.Trans.LedgerTransactionId });
            }

        }
        $scope.addItem = function () {
            $scope.Trans.EntryType = $scope.EntryType;

            switch ($scope.TranType) {
                case 3: //Debit note
                    $scope.Trans.DrLedgerId = $scope.Trans.LedgerId;
                    $scope.Trans.TransactionType = $scope.TranType;
                    break;
                case 4: //Credit note
                    $scope.Trans.CrLedgerId = $scope.Trans.LedgerId;
                    $scope.Trans.TransactionType = $scope.TranType;
                    break;
            }
            var model = cloneObj($scope.Trans);
            model.TransactionDate = formatdate(model.TransactionDate);
            var txn = new $.LedgerTrasaction();
            txn.CreateTransaction(function (e) {

                var trans = $scope.Trans;
                if (e.data.Code == "500") {
                    // alert(e.data.Description);
                    var localData = { Message: e.data.Description, Title: 'Error' };
                    $scope.Message = e.data.Description;
                    ModalFactory.Info('DialogController', $scope);
                    return;
                }
                else {
                    alert('Record saved');
                    $scope.Cancel();
                    //$scope.LedgerTransactionId = e.data.Data; //for ok button
                    //$scope.Trans.LedgerTransactionId = e.data.Data;
                    //$scope.Message = 'Do you want to print the note?';
                    //$scope.okbutton = okbutton;
                    //ModalFactory.ConfirmToPrint('DialogController', $scope);
                    //$scope.Items.push($scope.Trans);
                    //init();
                }
            }, model);



        }
        function okbutton(modalScope) {
            $scope.printNote(modalScope.LedgerTransactionId);
        }


        $scope.printNote = function (transactionId) {

            LedgerFactory.PrintTransaction(function (e) {

                if (e.data.Code != '500') {
                    ReportService.Print(e.data.Data);
                }
                else {
                    $scope.Message = e.data.Description;
                    //ModalFactory.Info('DialogController', $scope);
                    //ReportService.Print(e.data.Data);
                }
            }, { LedgerTransactionId: transactionId });
        }


        function init() {
            $scope.Trans = new $.LedgerTrasaction({});
            var ledger = new $.Ledger({});
            $scope.Trans.TransactionDate = convertDate(new Date());
            ledger.GetAll(function (e) {
                $scope.Accounts = e.data;
                $scope.Trans.LedgerId = e.data[0].LedgerId;
            });


        }


        $scope.$watch('Trans.LedgerId', function () {
            $scope.Items = [];
            if (!$scope.Trans) return;
            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Trans.LedgerId);
            $scope.Trans.LedgerName = obj.Name;
            getExistingNotes();
        });

        function getExistingNotes() {

            LedgerFactory.GetDrCrNotes(function (e) {

                $scope.Items = jQuery.grep(e.data.Data, function (n, j) {
                    return (n.TransactionType == $scope.TranType);
                });

            }, { LedgerId: $scope.Trans.LedgerId });
        }
        $scope.DeleteItem = function (index) {
            $scope.$apply(function () {
                $scope.Items.splice(index - 1, 1);
            });
        }
        $scope.SendReminder = function (transactionId, remType) {
            window.event.stopPropagation();
            LedgerFactory.EmailDrCrNote(function (e) {

            }, { LedgerTransactionId: transactionId, ReminderType: remType });
        }

        $scope.Cancel = function () {
            if ($scope.EntryType == 10) {
                $state.go('drNotesList');
            }
            else {
                $state.go('crNotesList');
            }
        }
    }]);

app.controller('DialogController', ['$scope', 'ModalFactory', function ($scope, ModalFactory) {

    //$scope.Message = localData.Message;
    //$scope.Title = localData.Title;
    $scope.OkButtonClick = function (data) {

        if ($scope.okbutton) {
            $scope.okbutton($scope);
        }
        ModalFactory.Dialog.hide();
    }
    $scope.closeDialog = function () {
        ModalFactory.Dialog.hide();
    }
}]);

app.controller('PartyBalanceController', ['$scope', '$rootScope', 'LedgerFactory', function ($scope, $rootScope, LedgerFactory) {

    var date = new Date();
    var ledger = new $.Ledger({});

    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: '', To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    loadPartyBalance();
    $scope.LoadPartyBalance = function () {
        //loadPartyBalance();
    }
    function loadPartyBalance() {
        LedgerFactory.GetPartyBalance(function (e) {

            $scope.Balance = e.data;
        }, $scope.Filter);
    }

}]);
app.controller('UnbilledSitesController', ['$scope', '$rootScope', 'LedgerFactory', function ($scope, $rootScope, LedgerFactory) {

    var date = new Date();
    var ledger = new $.Ledger({});

    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: '', To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    getUnbilledSites();
    $scope.GetUnbilledSites = function () {
        // getUnbilledSites();
    }
    function getUnbilledSites() {
        LedgerFactory.GetUnbilledSites(function (e) {

            $scope.UnbilledSites = e.data;
        }, $scope.Filter);
    }
}]);
app.controller('PendingChallanAcknowledgmentController', ['$scope', '$rootScope', 'WorkOrderFactory', function ($scope, $rootScope, WorkOrderFactory) {

    var date = new Date();
    // var workOrder = new $.WorkOrder({});     
    //var token = $rootScope.getTokenInfo();   
    $scope.Filter = { LedgerId: 0, From: '', To: '' };
    //if(token) {
    //    $scope.Filter.From =   convertDate( token.FinYearStart);
    //}
    getUnbilledSites();
    $scope.GetUnbilledSites = function () {
        // getUnbilledSites();
    }
    function getUnbilledSites() {
        WorkOrderFactory.PendingChallanAcknowldgements(function (e) {

            $scope.PendingAcknowledgements = e.data.Data;
        }, $scope.Filter);
    }
}]);
app.controller('WorkOrderListController', ['$scope', '$rootScope', '$filter', '$state', '$crypto', '$stateParams',
    '$mdDialog', 'ModalFactory', 'LedgerFactory'
    , function ($scope, $rootScope, $filter, $state, $crypto, $stateParams, $mdDialog, ModalFactory, LedgerFactory) {

        var worder = new $.WorkOrder({ WorkOrderId: 0 });


        $scope.worder = worder;
        $scope.search = { JobNumber: '', Site: '', Client: '', JobNumber: '', SiteEng: '', Closed: 0 };
        $scope.Filter = { LedgerId: 0, Site: '', JobNumber: '' };
        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Token = token;
        $scope.Filter.EndDate = convertDate(date);

        if (token) {
            $scope.Filter.WorkOrderDate = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;

        }
        $scope.NewSiteJob = function (site, event) {
            event.preventDefault();
            ModalFactory.AddEditSiteJob('SiteJobController', site);
        }
        LedgerFactory.GetAllParties(function (e) {
            if (e.status != 200) {
                alert('Could not load parties');
                return;
            }

            $scope.Accounts = e.data.filter(o => o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_CREDITORS
                || o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_DEBTORS);

        });
        $scope.GetAll = function () {

            //worder.Number = $scope.search.Number;
            //worder.JobNumber = $scope.search.JobNumber;
            //worder.Site = $scope.search.Site;
            //worder.Client = $scope.search.Client;
            //worder.Closed = $scope.search.Closed;
            //worder.SiteEng = $scope.search.SiteEng
            var filter = cloneObj($scope.Filter);
            filter.WorkOrderDate = formatdate(filter.WorkOrderDate);
            filter.EndDate = formatdate(filter.EndDate);
            filter.ChallanType = CHALLAN_TYPE.WORKORDER;
            worder.WorkOrders(function (e) {
                $scope.WorkOrders = e.data.Data;
            }, filter);
            //LedgerFactory.GetAllClientSites(function (e) {
            //    $scope.Sites = e.data.Data;
            //}, $scope.search);
        }

        $scope.RowSelected = function (index) {

        };
        $scope.Find = function () {
            $scope.GetAll();
        };
        $scope.GetAll();
        //$rootScope.$on("LoadSites", function (evt, index) {

        //    var arrIndex = $scope.WorkOrders.indexOf($scope.WorkOrders.find(o => o.WorkOrderId == index));
        //    $scope.LoadSites(arrIndex, true);
        //});
        $scope.CloseSite = function (index, siteId, event) {

            event.preventDefault();
            var siteInfo = $scope.Sites[index];;
            var site = new $.Site({ SiteId: siteId });

            site.CloseSite(function (e) {
                $('#tr' + siteInfo.SiteId).addClass('closedSite');
                //  refreshSites(index,true);
            });
        }
        $scope.CloseSitePayment = function (index, siteId) {
            var siteInfo = $scope.WorkOrders[index];;
            event.preventDefault()
            //  var site = new $.Site({SiteId: siteInfo.SiteId});
            var site = $scope.WorkOrders.find(o => o.SiteId == siteId)
            site.CloseSitePayment(function (e) {
                $('#tr' + siteInfo.SiteId).addClass('paymentClosedSite');
            });
        }
        $scope.LoadSites = function (index, refresh) {
            refreshSites(index, refresh);

        }
        function refreshSites(index, refresh) {
            var indx = index;
            var wOrder = new $.WorkOrder({ WorkOrderId: $scope.WorkOrders[indx].WorkOrderId });
            var workOrderId = wOrder.WorkOrderId;
            var rowToShow = $('tr[id=' + workOrderId + ']');
            var plusToggle = rowToShow.prev().find('i').not('.fa-edit');
            $('.childRows').not(rowToShow).hide();

            if ($scope.WorkOrders[indx].Sites.length == 0 || refresh) {
                wOrder.GetSites(function (e) {
                    $scope.WorkOrders[indx].Sites = e.data;

                });
            }
            if (refresh != true) {
                plusToggle.toggleClass('fa-chevron-down fa-chevron-up');
                rowToShow.toggle(500);
            }
            $('.fa-chevron-up').not(plusToggle).toggleClass('fa-chevron-down fa-chevron-up')


        }
        $scope.edit = function (wo) {
            var key = $crypto.encrypt(wo.WorkOrderId);
            $state.go('editwo', { key: key });
        }
        $scope.editSite = function (index, siteInfo) {

            event.preventDefault();
            //  var siteInfo =   $scope.WorkOrders.find(o => o.SiteId==siteId)
            //  var siteInfo = $scope.WorkOrders[index];//this.site;
            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/editSite.html?d=' + new Date().getTime(), function () {
                var html = $(this).html();
                $scope.currentSite = siteInfo;
                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        siteInfo: siteInfo
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: 'SiteController'
                });
            });
        }

        $scope.editWorOrder = function (index, woNumber) {
            event.preventDefault();
            var div = '<div style="width:90%;height:70%"></div>';
            // var workOrder = $scope.WorkOrders[index];
            var workOrder = $scope.WorkOrders.find(o => o.Number == woNumber);
            $(div).load('templ/editWorkOrder.html', function () {
                var html = $(this).html();
                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        workOrder: workOrder
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: editController
                });

            });
        }
        function editController($scope, $routeParams, $http, workOrder) {
            var ledger = new $.Ledger({});
            var company = new $.Company({});
            var wOrder = new $.WorkOrder({});
            wOrder.Number = workOrder.Number;
            wOrder.WorkOrderDate = workOrder.WorkOrderDate;
            wOrder.LedgerId = workOrder.LedgerId;
            wOrder.CompanyId = workOrder.CompanyId;
            wOrder.ClientAmount = workOrder.ClientAmount;
            wOrder.WorkOrderId = workOrder.WorkOrderId;

            wOrder.WorkOrderDate = new Date(workOrder.WorkOrderDate);
            $scope.workOrder = wOrder;
            $scope.closeDialog = function () {
                $mdDialog.hide();
            }
            $scope.updateWorkOrder = function () {
                var fileList = [];
                $scope.workOrder.Update(function (e) {

                    $mdDialog.hide();
                }, fileList);


            }
            $scope.LedgerList = [];
            $scope.LedgerList.push(ledger);
            //debugger
            ledger.GetAll(function (e) {

                $scope.LedgerList = e.data;
            });
            company.GetAll(function (e) {
                $scope.Companies = e.data;
            });

        }
        $scope.PrintReceipt = function (item) {
            event.preventDefault();

            worder.WorkOrderNumber = item.Number;
            worder.PrintIssuedReceipt(function (e) {

                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);
            });
        }

    }]);


app.controller('WorkOrderController', function ($scope, $rootScope, $route, $stateParams, LedgerFactory, $state, $crypto) {

    var wId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);
    var sId = $stateParams.sId == undefined ? 0 : $stateParams.sId;
    var worder = new $.WorkOrder({ WorkOrderId: wId });
    var operation = new $.Operation();
    var product = new $.Product();
    worder.ChallanType = CHALLAN_TYPE.WORKORDER;
    worder.WorkOrderDate = convertDate(new Date());
    $scope.Operation = { OperationId: 0, Quantity: 0, Name: '' };
    $scope.Operations = [];
    $scope.BOMList = [];
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    $scope.WorkOrderId = 0;
    $scope.WorkOrder = worder;
    var newItem = { ProductId: 0, Name: '', Quantity: 0, BOMId: 0 };
    $scope.NewItem = newItem;
    $scope.WorkOrder.Items = [];
    $scope.WorkOrder.Operations = [];

    init();
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.WorkOrder.Items.splice(index - 1, 1);
        });
    }
    $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();

    operation.OperationsList(function (e) {
        if (e.data.Code == 200) {
            $scope.Operations = e.data.Data;
        }
    });
    product.BOMList(function (e) {
        if (e.data.Code == 200) {
            $scope.BOMList = e.data.Data;
        }
    });
    $scope.SubTotal = function (_total) {

        var subTotal = 0;

        for (var i = 0; i < Object.keys($scope.WorkOrder.Items).length; i++) {
            if ($scope.WorkOrder.Items[i].SentQty != null) {
                subTotal += parseFloat($scope.WorkOrder.Items[i].SentQty) * parseFloat($scope.WorkOrder.Items[i].Rate);
            }
        }
        $scope.WorkOrder.SubTotal = $scope.WorkOrder.SiteInfo.SubTotal = subTotal;
        $scope.WorkOrder.SubTotal1 = $scope.WorkOrder.SubTotal + parseFloat($scope.WorkOrder.SiteInfo.Freight);
        var taxAmount = 0;

        if (_total == 1) {

            if ($scope.AppliedTaxes != null) {
                $scope.FreightTax = 0;
                for (var i = 0; i < $scope.AppliedTaxes.length; i++) {
                    taxAmount += $scope.AppliedTaxes[i].TaxAmount;
                    //   $scope.FreightTax += $scope.AppliedTaxes[i].TaxRate/100.0 * $scope.WorkOrder.SiteInfo.Freight;
                }
            }
            if ($scope.WorkOrder.Taxes) {
                for (var i = 0; i < $scope.WorkOrder.Taxes.length; i++) {
                    var tax = $scope.WorkOrder.Taxes[i];
                    if (tax.Applicable) {
                        $scope.FreightTax += ($scope.WorkOrder.SiteInfo.Freight) * $scope.WorkOrder.Taxes[i].DefaultRate / 100.00;
                    }
                }
            }
            $scope.WorkOrder.Total = $scope.WorkOrder.SiteInfo.Total = $scope.WorkOrder.SubTotal1 + taxAmount + $scope.FreightTax;
            return $scope.WorkOrder.Total;
        }
        else {
            return subTotal;
        }


    };

    $scope.itemSelected = function (itemId) {

        debugger
        $scope.IssueItem.ProductId = itemId;
        var item = $scope.ProductRates.find(o => o.ProductId == itemId);
        $scope.IssueItem.Name = item.Product;
        findDefaultRate($scope.IssueItem.ProductId);
        $scope.ProductSizes = [{}];
        $scope.ProductSizes = $.map($scope.AllSizes, function (value, key) {
            if (value.ProductId == itemId) {
                return value;
            }
        });

    };
    $scope.DefaultRate = 0.0;
    // $scope.FreightTax=0.0;
    function findDefaultRate(productId) {
        $scope.IssueItem.Rate = 0;
        if ($scope.ProductRates) {
            var item = $scope.ProductRates.find(o => o.ProductId == productId);
            if (item) {
                $scope.IssueItem.Rate = item.RentRate;
            }
        }
    }

    $scope.$watch('WorkOrder.BOMId', function () {

        if ($scope.WorkOrder.BOMId != null) {


            product.BOMDetails(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }

                var _data = e.data.Data;

                $scope.WorkOrder.Items.filter(o => o.BOMId > 0).forEach(function (i, j) {

                    $scope.WorkOrder.Items.splice(j - 1, 1);

                });

                _data.Details.forEach(function (i, j) {
                    var c = { ProdutId: i.ProductId, Name: i.Product, Quantity: i.Quantity, BOMId: i.BomId };
                    $scope.WorkOrder.Items.push(c);
                });
            }, { BOMId: $scope.WorkOrder.BOMId });
        }
    });

    $scope.$watch('WorkOrder.LedgerId', function () {

        if ($scope.WorkOrder.LedgerId != null) {
            ledgerDTO.Props.LedgerId = $scope.WorkOrder.LedgerId;
            $scope.getSites();


            $scope.ProductRates = [];
            $rootScope.LedgerId = $scope.WorkOrder.LedgerId;

        }
    });

    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;

        }, {
            LedgerId: $scope.WorkOrder.LedgerId
        });
    }

    function getAllProducts() {
        $scope.ProductRates = [];
        ledgerDTO.GetProductRates(function (e) {
            $scope.ProductRates = e.data;
        });
    }
    $scope.CompanySelection = function (obj) {
        if (obj != undefined) {
            $scope.WorkOrder.CompanyId = obj.originalObject.CompanyId;
        }
    };

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {

        if ($scope.WorkOrder.Items[index] != undefined) {
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



    var modal

    $scope.closeSliderModal = function () {

        modal.close({});
    }

    function init() {
        $scope.IssueItem = new $.WorkOrderItem({ Rate: $scope.DefaultRate });
        $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = $scope.Accounts = e.data.filter(o => o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_CREDITORS || o.AccountGroup == StaicData.SYS_ACCOUNT_GROUPS.SUNDRY_DEBTORS);

            if ($scope.WorkOrder.LedgerId == null) {
                $scope.WorkOrder.LedgerId = e.data[0].LedgerId;
            }
        });
        debugger
        if (worder.WorkOrderId > 0) {
            worder.JobWoById(function (e) {
                if (e.data) {
                    $scope.WorkOrder = e.data;
                }
            });
        }
    }
    $scope.Save = function () {
        //var res = $scope.WorkOrder.Items.filter(function (v) {
        //    if (v != undefined) {
        //        return v.ProductId > 0;
        //    }
        //    else
        //        return false;
        //});
        debugger

        var m = $('#form-workorder').valid();
        // var site = $('#form-site').valid();

        //if (site && sId == 0) {
        //    if ($scope.WorkOrder.SiteInfo.JobNumber == null || $scope.WorkOrder.SiteInfo.JobNumber == '') {
        //        alert('Job number is required');
        //        return;
        //    }
        //}
        if (m) {
            EnableToolbar(0);
            var reader = new FileReader();

            var fileList = [];
            //fileList = addFileToList(fileList, 'fileSitePic');
            //fileList = addFileToList(fileList, 'Doc1');
            //fileList = addFileToList(fileList, 'Doc2');
            //fileList = addFileToList(fileList, 'Doc3');

            if ($scope.WorkOrder.Items.length < 1) {
                alert('Work order can not be empty.');
                return;
            }
            //debugger
            addWorkOrder(fileList);


        }
    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    function addWorkOrder(fileList) {

        //  $scope.WorkOrder.Transaction = $scope.Trans;
        //$scope.WorkOrder.SiteInfo.FreightTax = $scope.FreightTax;
        // $scope.WorkOrder.LedgerSiteId = sId;
        $scope.WorkOrder.ChallanType = CHALLAN_TYPE.WORKORDER; // for rent purpose
        var wo = new $.WorkOrder();
        var model = cloneObj($scope.WorkOrder);
        model.PlanStartDate = formatdate(model.PlanStartDate);
        model.PlanEndDate = formatdate(model.PlanEndDate);
        model.WorkOrderDate = formatdate(model.WorkOrderDate);
        model.EndDate = formatdate(model.EndDate);

        wo.AddProductionWo(function (e) {
            if (e.statusText == 'OK') {
                //$location.path('/woInfo').search({ wId: e.data.WorkOrderId });
                //$scope.$apply();
                alert('Success');
                $scope.warnOnLeave = false;
                $route.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }

    function hideWorkOrderTab() {
        $scope.HideWorkOrder = 0;
        if (wId > 0) {


            $('#tbWorkOrder').addClass('disabled');
            //$('#tbWorkOrder').next().addClass('active');
            $('#tbWorkOrder a').removeAttr('data-toggle');

            $('a[href="#siteInfo"]').tab('show');
        }
    }

    //add new item to be issued
    $scope.addItem = function () {
        debugger
        var item = cloneObj($scope.IssueItem);
        var exist = $scope.WorkOrder.Items.find(o => o.ProductId == item.ProductId);
        if (exist) {
            exist.Quantity += item.Quantity;
            return;
        }
        $scope.WorkOrder.Items.push(item);

        $scope.IssueItem = new $.WorkOrderItem({ ProductId: lastProductId, Product: lastProductName, Rate: $scope.DefaultRate });


    }
    $scope.onItemSelected = function (value) {
        $scope.IssueItem.Name = value.Name;
        $scope.IssueItem.ProductId = value.ProductId;
    }
    $scope.addOperation = function () {
        if ($scope.Operation.OperationId == 0 || $scope.Operation.Quantity <= 0) {
            alert('Please select operation and quantity');
            return;
        }
        var op = $scope.Operations.find(o => o.OperationId == $scope.Operation.OperationId);
        var cOperation = cloneObj($scope.Operation);
        cOperation.Name = op.Name;
        $scope.WorkOrder.Operations.push(cOperation);
        $scope.Operation.OperationId = 0;
        $scope.Operation.Quantity = 0;
    }

    var stateCity = new $.StateCity({});
    loadStates();

    function loadStates() {
        stateCity.GetStates(function (e) {
            $scope.States = e.data;

        });
    }

});
