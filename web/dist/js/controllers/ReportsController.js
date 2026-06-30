app.controller('DueItemsReportsController', function ($scope, $rootScope, $routeParams, $route, $window, $mdDialog,
    AuthenticationService, $http, $rootScope) {
    var reports = new $.Reports({});
    function itemsPendency() {
        var ledger = new $.Ledger({});
        var currentDate = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(currentDate) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }

        ledger.Print_StockBalance_Dashboard(function (e) {

            $scope.BalanceItems = e.data.Table;
            $scope.ClosingBalance = e.data.Table1;
            //$scope.BalanceItems  = e.data.find(o=>o.ClosingBalance>0);
            //$scope.BalanceItems =  jQuery.grep(e.data, function (n, i) {
            //    return (n.ClosingBalance > 0);
            //});

        }, $scope.Filter);
        itemsPendency();
    }
});
app.controller('ReportsController', function ($scope, $rootScope, $routeParams, $route, $window, $mdDialog,
    AuthenticationService, $http, $rootScope) {
    //AuthenticationService.validateRequest();
    var reports = new $.Reports({});
    var workOrder = new $.WorkOrder({ LedgerId: 0 });

    $scope.Report = reports;
    RefreshPayments();
    $scope.SiteClosedFrom = null;
    $scope.SiteClosedTo = null;
    $scope.RefreshPaymentReminder = function () {
        RefreshPayments();
    };
    // DashboardSummary();
    function RefreshPayments() {
        reports.PendingPayment(function (e) {
            $scope.PendingPayments = e.data;
        });
        refreshClosedSites();
    }
    reports.PaymentReceived(function (e) {

        $scope.PaymentsWidget = e.data;
    });


    var site = new $.Site({});
    var _jobNumbers = [];
    _jobNumbers.push({ JobNumber: 'All', Site: 'All' });
    var _siteNames = [];
    _siteNames.push({ JobNumber: 'All', Site: 'All' });

    $scope.CheckDate = function (item) {

        var dueDate = stringToDate(item.DueDate);

        return dueDate < new Date() ? 'cancelled' : '';
    }


    $scope.SiteWiseInventory = function () {


        //detail report has been commented for time being
        // if ($scope.ReportType == "1") {
        $scope.Report.SiteWiseInventorySummary(function (e) {
            $scope.SiteWiseDetailInventorySummary = e.data;
        });
        //}
        //else {
        //    $scope.Report.SiteWiseInventory(function (e) {
        //        $scope.SiteWiseDetailInventory = e.data;
        //    });
        //}


    }

    function DashboardDialogController($scope, $mdDialog, $routeParams, $http, data) {
        $scope.DialogData = null;
        $scope.Report = new $.Reports({ Closed: data.Closed });
        $scope.Data = data;
        if (data.Type == 0) {
            $scope.Report.ClosedSitesDialog(function (e) {
                $('#loadersite').hide();
                $scope.DialogData = e.data;
            });
        }
        else {
            $scope.Report.OpenedJobNumbers(function (e) {
                $('#loaderJob').hide();
                $scope.DialogData = e.data;
            });
        }
        $scope.closeDialog = function () {
            $mdDialog.hide();
        }
    }
    $scope.ClosedSites = function () {
        refreshClosedSites();

    }
    $scope.OpenDialg = function (type, closed) {
        var data = { 'Closed': closed, 'Type': type };
        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/closedSiteDialog.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    data: data
                },
                template: html,
                parent: angular.element(document.body),
                controller: DashboardDialogController
            });
        });
    }
    function refreshClosedSites() {
        if ($scope.Report.FromDate != null) {
            $scope.Report.FromDate = new Date($scope.Report.FromDate).toLocaleDateString();
        }
        if ($scope.Report.ToDate != null) {
            $scope.Report.ToDate = new Date($scope.Report.ToDate).toLocaleDateString();
        }
        $scope.Report.ClosedSites(function (e) {

            $scope.ClosedSites = e.data;
        });
    }
    $scope.DownloadReport = function () {

        $scope.Report.DownloadReport(function (e) {

            var filePath = SERVER_URL + 'temp/' + e.data;
            $window.location.href = filePath;
            //$scope.SiteWiseDetailInventorySummary = e.data;
        });
    }

    var journal = new $.Journal({});
    $scope.Journal = journal;

    $scope.PaymentReceived = function () {

        $scope.Journal.FromDate = new Date($scope.Journal.FromDate).toLocaleDateString();
        $scope.Journal.ToDate = new Date($scope.Journal.ToDate).toLocaleDateString();

        $scope.Journal.PaymentReceivedReport(function (e) {
            $scope.PaymentReceived = e.data;
        });
    };
    function intiReports() {
        $scope.GetWorkOrderBalance();
        $scope.WorkOrderDueDateReminder();
        $scope.WorkOrderOverDuesReminder();

    }
    function DashboardSummary() {

        $scope.Report.DashboardSummary(function (e) {
            $scope.Summary = e.data;
        });
    }

    $scope.SitePaymentSummary = function () {
        $scope.Report.SitePaymentSummary(function (e) {
            $scope.Report.SitePayments = e.data;
        });
    };
    $scope.GetWorkOrderBalance = function () {
        workOrder.GetWorkOrderBalance(function (e) {
            $scope.Balance = e.data;
        });
    }
    workOrder.WorkOrderDueDateReminder(function (e) {
        $scope.DueDateReminder = e.data;
    });
    $scope.WorkOrderDueDateReminder = function () {

        // dailyItemTransactions();
        getBills();
        itemsPendency();
    }
    function dailyItemTransactions() {

        $scope.Report.LedgerId = 0;
        var currentDate = new Date();
        $scope.Report.FromDate = convertDate(currentDate);
        $scope.Report.ToDate = convertDate(currentDate.setDate(currentDate.getDate() + 1));
        $scope.Report.DailyInventoryTransactions(function (e) {


            $scope.Issued = (e.data).filter(o => o.Mode == 1);
            $scope.Received = (e.data).filter(o => o.Mode == 2);
            $($scope.Issued).each(function () {

                $scope.TotalIssued += this.Quantity
            });
            $($scope.Received).each(function () {

                $scope.TotalReceived += this.Quantity
            });
        });
    }
    function getBills() {
        $scope.Billing = new $.Billing({ LedgreId: 0 });
        var currentDate = new Date();
        $scope.Billing.From = convertDate(currentDate);
        $scope.Billing.To = convertDate(currentDate.setDate(currentDate.getDate() + 1));
        $scope.Billing.GetBillList(function (e) {
            $scope.Bills = e.data;
        })
    }
    $scope.WorkOrderOverDuesReminder = function () {
        workOrder.WorkOrderOverDuesReminder(function (e) {
            $scope.OverDueReminder = e.data;
        });
    }
    intiReports();
    function itemsPendency() {
        var ledger = new $.Ledger({});
        var currentDate = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(currentDate) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }

        ledger.Print_StockBalance_Dashboard(function (e) {

            $scope.BalanceItems = e.data.Table;
            $scope.ClosingBalance = e.data.Table1;
            //$scope.BalanceItems  = e.data.find(o=>o.ClosingBalance>0);
            //$scope.BalanceItems =  jQuery.grep(e.data, function (n, i) {
            //    return (n.ClosingBalance > 0);
            //});

        }, $scope.Filter);
    }
});
app.controller('DailyInvTransController', ['$scope', '$routeParams', '$window', '$mdDialog', 'AuthenticationService', '$http', '$rootScope', 'LedgerFactory',
    function ($scope, $routeParams, $window, $mdDialog, AuthenticationService, $http, $rootScope, LedgerFactory) {
        $scope.Reports = new $.Reports({});
        $scope.TotalIssued = 0;
        $scope.TotalReceived = 0;
        $scope.GetTransactions = function () {
            $scope.Reports.DailyInventoryTransactions(function (e) {
                $scope.DailyData = e.data;
                $scope.Issued = (e.data).filter(o => o.Mode == 1);
                $scope.Received = (e.data).filter(o => o.Mode == 2);
                $($scope.Issued).each(function () {

                    $scope.TotalIssued += this.Quantity
                });
                $($scope.Received).each(function () {

                    $scope.TotalReceived += this.Quantity
                });
            });
        }

        if ($rootScope.LedgerId) {
            $scope.Reports.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Reports.LedgerId', function () {
            $rootScope.LedgerId = $scope.Reports.LedgerId;
            getSites();
        });
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Reports.LedgerId });
        }
        var token = $rootScope.getTokenInfo();
        var date = new Date();
        var filter = { LedgerId: 0, From: '', To: '' };
        // $scope.Report = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
        $scope.Reports.To = convertDate(date);
        if (token) {
            $scope.Reports.From = convertDate(token.FinYearStart);
        }
    }]);
app.controller('DueBillsController', ['$scope', '$state', 'ReportService', 'LedgerFactory', 'ReportService',
    '$window', '$crypto', 'ModalFactory', 'FileSaver',
    function ($scope, $state, ReportsService, LedgerFactory, ReportService, $window, $crypto, ModalFactory, FileSaver) {
        $scope.Filter = {};

        const date = new Date(); // Or any specific date
        const firstDay = new Date(date.getFullYear(), date.getMonth(), 1);

        $scope.Filter.From = convertDate(firstDay);
        $scope.Filter.To = convertDate(getLastDayOfMonth(date));
        $scope.DueBills = [];
        $scope.DueBillPreview = null;
        $scope.SelectedDueBill = null;
        $scope.Find = function () {
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);


            ReportsService.DueBills(function (e) {
                $scope.DueBills = e.data;
                $scope.dataToExport = $.map(e.data, function (val) {
                    return {
                        Client: val.Client, SiteAddress: val.SiteAddress, ContactPerson: val.ContactPerson,
                        ContactPersonPhone: val.ContactPersonPhone, Amount: val.Amount
                    };
                });
            }, model);
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
        });
        $scope.Print = function () {
            ReportService.PrintDueBills(function (e) {
                var filePath = SERVER_URL + 'temp/' + e.data;
                $window.open(filePath);
            }, $scope.Filter);
        };
        $scope.PrintPdf = function () {
            var model = cloneObj($scope.Filter);
            model.To = formatdate(model.To);
            ReportService.PrintDueBillsPdf(function (e) {
                FileSaver.saveAs(e.data, 'dueBills.pdf');
            }, model);
        };
        $scope.$watch('Filter.LedgerId', function () {
            getSites();
        });

        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.closeDialog = function () {
            ModalFactory.CloseDialog();
        }
        $scope.onBillSavedOnDialog = function () {
            ModalFactory.CloseDialog();
            $scope.Find();
        }
        $scope.gotoBilling = function (item) {
            var model = { LedgerId: item.LedgerId, LedgerSiteId: item.LedgerSiteId };
            var enc = $crypto.encrypt(JSON.stringify(model));
            $scope.localData = enc;
            $scope.dueBillFilter = $scope.Filter;
            //  $state.go('genDueBill', { ld: enc });
            ModalFactory.ShowBillingDialog('BillingController', $scope);

        };
        $scope.previewLastRentBill = function (item) {
            var id = item.InvoiceId;
            if (!id || id <= 0) {
                return;
            }
            $scope.SelectedDueBill = item;
            $scope.DueBillPreview = 1;
            $('#previewDialog').modal('show');
            var encrypedText = $crypto.encrypt(id);
            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.previewReportFromHtml(function (e) {
                $scope.DueBillPreview = null;
                $('#rpt').html(e.data);
            }, 'PreviewRentBill', econded);
        };
        $scope.downloadDueBillPdf = function () {
            var item = $scope.SelectedDueBill;
            if (!item || !item.InvoiceId) {
                return;
            }
            var strInput = 'rentbill,' + item.InvoiceId;
            var encrypedText = $crypto.encrypt(strInput);
            var fileName = (item.LastInvoiceNumber || item.InvoiceNumber || ('rentbill_' + item.InvoiceId)) + '.pdf';
            ReportService.printFromReportServer(encrypedText, fileName);
        };
    }]);

app.controller('LedgerAnalysisController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory) {
        //receipt or payment type
        //   debugger;
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
            var ledgers = e.data;
            console.log('Ledgers', e.data);
            ledgers.unshift({ LedgerId: 0, Name: 'All Ledgers' });
            $scope.Accounts = ledgers;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                var sites = e.data.Data;
                console.log('Sites', e.data.Data);
                sites.unshift({ LedgerSiteId: -1, SiteAddress: 'All Sites' });
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.GetAccountLedger = function () {
            ledger.GetAccountLedger(function (e) {

                $scope.TransList = e.data;
                console.log('translist', e.data);
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
app.controller('LedgerDetailsController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory) {
        //receipt or payment type

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
            var ledgers = e.data;
            console.log('Ledgers', e.data);
            ledgers.unshift({ LedgerId: 0, Name: 'All Ledgers' });
            $scope.Accounts = ledgers;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
        });
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                var sites = e.data.Data;
                console.log('Sites', e.data.Data);
                sites.unshift({ LedgerSiteId: -1, SiteAddress: 'All Sites' });
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.PreviousCB = "0 CR";
        $scope.GetAccountLedger = function () {

            if ($scope.Filter.LedgerId == 0) {
                alert('Please select party');
                return;
            }
            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);

            ledger.GetLedgerTransactions(function (e) {

                $scope.TransList = e.data;

                $scope.TransList = $.grep(e.data, function (n, i) {
                    n.Debit = 0;
                    n.Credit = 0;
                    if (n.TransactionType == 1 || n.TransactionType == 3) {
                        n.Debit = n.TransactionAmount;
                    }
                    else {
                        n.Credit = n.TransactionAmount;
                    }

                    if (i == 0) {
                        n.Closingbalance = n.ClosingBalanceBeforeFromDate + n.Credit - n.Debit;
                        $scope.PreviousCB = Math.abs(n.ClosingBalanceBeforeFromDate).toString() + (n.ClosingBalanceBeforeFromDate < 0 ? ' DR' : ' CR');
                    }
                    else {
                        n.Closingbalance = e.data[i - 1].Closingbalance + n.Credit - n.Debit;
                    }

                    return n;
                });
                console.log('translist', e.data);
                $scope.LastRow = e.data[e.data.length - 1];

            }, filter);
        }

        $scope.Abs = function (val) {
            return Math.abs(val) + (val < 0 ? ' DR' : ' CR');
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

        var x = $scope.$on('showLedger', function (event, data) {

            $scope.parentMessage = data;

        });

    }]);
app.controller('ClientWiseItemsBalanceController', ['$scope', '$location', '$routeParams',
    '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory', 'FileSaver'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory, FileSaver) {
        //receipt or payment type
        // debugger;
        var ledger = new $.Ledger({});

        $scope.ClientWiseItemsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = {
            LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date)
            , BalanceType: 'rent', PONumber: ''
        };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            var ledgers = e.data;

            //   ledgers.unshift({ LedgerId: -1, Name: 'All Ledgers' });
            $scope.Accounts = ledgers;
            $scope.Filter.LedgerId = -1;//e.data[0].LedgerId;
            //if ($scope.Filter.LedgerId == null) {
            //    $scope.Filter.LedgerId = e.data[0].LedgerId;
            //}
        });
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                var sites = e.data.Data;

                sites.unshift({ LedgerSiteId: 0, SiteAddress: 'All Sites' });
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        //$scope.GetAccountLedger = function () {
        //    ledger.GetAccountLedger(function (e) {

        //        $scope.TransList = e.data;
        //        console.log('translist', e.data);
        //        $scope.LastRow = e.data[e.data.length - 1];
        //    }, $scope.Filter);
        //}
        $scope.PrintLedger = function () {
            ledger.GetAccountLedger_rpt(function (e) {
                //debugger
                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);

            }, $scope.Filter);
        }


        $scope.Print = function () {
            //$scope.Filter.ReportName = 'ClientwiseItemBalance';
            //ledger.PrintReport(function (e) {
            //    console.log('Print report', e.data);
            //    downloadReport('ClientwiseItemBalance.pdf', e.data.Data);

            //}, $scope.Filter);
            $scope.export();
        }
        $scope.dataToExport;
        $scope.GetClientWiseItems = function () {
            $scope.ClientWiseItemsList = [];
            ledger.GetClientWiseItems(function (e) {
                 
                if (e.data.length == 0) {
                    return;
                }
                $scope.ClientWiseItemsList = e.data;

                $scope.LastRow = e.data[e.data.length - 1];


                $scope.dataToExport = $.map(e.data, function (val) {
                    return {
                        ClientName: val.ClientName, Site: val.SiteAddress, Product: val.Product, OpeningBalance: val.OpeningBalance, IssuedQty: val.IssuedQty,
                        ReceivedQty: val.ReceivedQty, ClosingBalance: val.ClosingBalance
                    };
                });
            }, $scope.Filter);
        }

        $scope.export = function () {


            var report = new $.Reports();

            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);
            filter.Print = true;
            filter.pdf = true;
            report.ClientWiseItemBalance(function (e) {
                if (filter.pdf == 'xlsx') {
                    FileSaver.saveAs(e.data, 'clientBalance.xlsx');
                }
                else {
                    FileSaver.saveAs(e.data, 'clientBalance.pdf');
                }
            }, filter);
        }

    }]);

app.controller('ItemWiseClientsBalanceController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory', 'FileSaver'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory, FileSaver) {
        //receipt or payment type
        //  debugger;
        var ledger = new $.Ledger({});

        $scope.ItemWiseClientsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = {
            ProductId: 0, LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date)
            , BalanceType: 'rent'
        };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        //ledger.GetAll(function (e) {
        //    var ledgers = e.data;
        //    console.log('Ledgers', e.data);
        //    ledgers.unshift({ LedgerId: -1, Name: 'All Ledgers' });
        //    $scope.Accounts = ledgers;
        //    if ($scope.Filter.LedgerId == null) {
        //        $scope.Filter.LedgerId = e.data[0].LedgerId;
        //    }
        //});
        getAllProductSizesByCompany();
        function getAllProductSizesByCompany() {
            var product = new $.Product();
            product.GetSizeListByCompany(function (e) {
                //  debugger
                console.log('AllSizes', e.data);
                $scope.AllSizes = e.data;

            });
        }
        $scope.selectedProduct = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                $scope.Filter.ProductId = item.ProductId;

                //$scope.IssueItem.Product = item.Product;
                //findDefaultRate($scope.IssueItem.ProductId);
                ////   alert(item.ProductSizeId);
                //$scope.IssueItem.ProductSizeId = item.ProductSizeId;
                //$scope.ItemInStock = $filter('sumByKey')($filter('filter')($scope.ItemStock, { ProductId: $scope.IssueItem.ProductId }), 'Quantity');
                //$scope.PartyItemInStock = $filter('sumByKey')($filter('filter')($scope.PartyBalance, { ProductId: $scope.IssueItem.ProductId }), 'ClosingBalance');
            }
        };
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            // getSites();
        });
        //function getSites() {
        //    LedgerFactory.GetMasterSites(function (e) {
        //        var sites = e.data.Data;
        //        console.log('Sites', e.data.Data);
        //        sites.unshift({ LedgerSiteId: -1, SiteAddress: 'All Sites' });
        //        $scope.LedgerSites = e.data.Data;
        //    }, { LedgerId: $scope.Filter.LedgerId });
        //}
        //$scope.GetAccountLedger = function () {
        //    ledger.GetAccountLedger(function (e) {

        //        $scope.TransList = e.data;
        //        console.log('translist', e.data);
        //        $scope.LastRow = e.data[e.data.length - 1];
        //    }, $scope.Filter);
        //}

        $scope.Print = function () {
            var report = new $.Reports();
            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);
            filter.Print = true;
            filter.Pdf = true;
            report.ItemWiseClientBalance(function (e) {
                FileSaver.saveAs(e.data, 'itemWiseClientBalance.pdf');
            }, filter);
        }
        $scope.GetItemWiseClients = function () {
            ledger.GetItemWiseClients(function (e) {
                var list = e.data || [];
                list.forEach(function (item) {
                    item.GroupKey = item.ClientName + '|' + (item.SiteAddress || 'Default');
                });
                $scope.ItemWiseClientsList = list;
                console.log('GetItemWiseClients', e.data);
                $scope.LastRow = list[list.length - 1];
            }, $scope.Filter);
        }

        //$scope.SubTotal = function () {
        //    var total = 0;
        //    if ($scope.TransList != undefined) {
        //        for (var i = 0; i <= $scope.TransList.length - 1; i++) {
        //            total += parseFloat($scope.TransList[i].TransactionAmount);
        //        }
        //    }
        //    return total;
        //}

    }]);

app.controller('CashbookController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory) {
        //receipt or payment type
        //    debugger;
        var ledger = new $.Ledger({});

        $scope.ItemsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { AccountGroupId: 17, LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            // getSites();
        });

        $scope.Print = function () {
            $scope.Filter.ReportName = 'Cashbook';
            ledger.PrintReport(function (e) {
                console.log('Print report', e.data);
                downloadReport('Cashbook.pdf', e.data.Data);

            }, $scope.Filter);
        }

        $scope.GetCashBook = function () {
            ledger.GetCashBook(function (e) {

                $scope.ItemsList = e.data;
                console.log('GetCashBook', e.data);
                $scope.LastRow = e.data[e.data.length - 1];
            }, $scope.Filter);
        }



    }]);

app.controller('StockInhandController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window',
    'LedgerFactory', 'WarehouseService'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory, WarehouseService) {
        //receipt or payment type
        //  debugger;
        var ledger = new $.Ledger({});

        $scope.ItemsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, OnDate: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            // getSites();
        });

        $scope.Print = function () {
            $scope.Filter.ReportName = 'StockInhand';
            ledger.PrintReport(function (e) {
                console.log('Print report', e.data);
                downloadReport('StockInhand.pdf', e.data.Data);

            }, $scope.Filter);
        }
        $scope.Warehouses = [];
        WarehouseService.getWarehouses().then(function (e) {

            if (e.data.Code != 200) {
                aelrt(e.data.Message);
                return;
            }
            $scope.Warehouses = e.data.Data;
        });

        $scope.GetStockInhand = function () {

            ledger.GetStockInhand(function (e) {

                $scope.ItemsList = e.data;
                console.log('GetStockInhand', e.data);
                $scope.LastRow = e.data[e.data.length - 1];
            }, $scope.Filter);
        }



    }]);

app.controller('StockSummaryController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog',
    '$window', 'LedgerFactory', 'WarehouseService'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory, WarehouseService) {
        //receipt or payment type
        // debugger;
        var ledger = new $.Ledger({});

        $scope.ItemsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, OnDate: convertDate(date) };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
        }
        var ledger = new $.Ledger({});
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            // getSites();
        });

        $scope.Print = function () {
            $scope.Filter.ReportName = 'StockSummary';
            ledger.PrintReport(function (e) {
                console.log('Print report', e.data);
                downloadReport('StockSummary.pdf', e.data.Data);

            }, $scope.Filter);
        }
        $scope.Warehouses = [];
        WarehouseService.getWarehouses().then(function (e) {

            if (e.data.Code != 200) {
                aelrt(e.data.Message);
                return;
            }
            $scope.Warehouses = e.data.Data;
        });
        $scope.GetStockSummary = function () {
            ledger.GetStockSummary(function (e) {

                $scope.ItemsList = e.data;
                console.log('GetStockSummary', e.data);
                $scope.LastRow = e.data[e.data.length - 1];
            }, $scope.Filter);
        }



    }]);


app.controller('Gstr1Controller', function ($scope, ExcelService, $rootScope, $window, ReportService) {

    $scope.gstrData = [];
    var _d = new Date();
    _d.setDate(_d.getMonth() - 1);
    $scope.filter = {
        month: _d.getMonth() + 1, year: _d.getFullYear()
    };


    $scope.show = function () {
        ReportService.gstr1($scope.filter).then(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.gstrData = e.data.Data;

        });
    };
    $scope.exportToExcel = function () {

        //const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        //const columns = getColumns(data);
        //columns.splice(columns.length - 1, 1);
        //const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        //const workbook = XLSX.utils.book_new();
        //XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        //XLSX.writeFile(workbook, 'gstr1.xlsx');
        var _headers = ['Description', 'Count', 'Taxable', 'IGST', 'SGST', 'CGST', 'CESS', 'Total Tax', 'Invoice Value'];
        var _data = [];
        _data.push({
            'Description': 'B2b', 'Count': $scope.gstrData.b2b.Vouchers, 'Taxable': $scope.gstrData.b2b.TaxAbleAmount, 'IGST': $scope.gstrData.b2b.IGSTAmount,
            'SGST': $scope.gstrData.b2b.SGSTAmount, 'CGST': $scope.gstrData.b2b.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.b2b.TaxAmount, 'Invoice Value': $scope.gstrData.b2b.TaxAbleAmount + $scope.gstrData.b2b.TaxAmount
        });
        _data.push({
            'Description': 'B2C (Large) Invoice', 'Count': $scope.gstrData.b2c_largeInvoice.Vouchers, 'Taxable': $scope.gstrData.b2c_largeInvoice.TaxAbleAmount, 'IGST': $scope.gstrData.b2c_largeInvoice.IGSTAmount,
            'SGST': $scope.gstrData.b2c_largeInvoice.SGSTAmount, 'CGST': $scope.gstrData.b2c_largeInvoice.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.b2c_largeInvoice.TaxAmount, 'Invoice Value': $scope.gstrData.b2c_largeInvoice.TaxAbleAmount + $scope.gstrData.b2c_largeInvoice.TaxAmount
        });
        _data.push({
            'Description': 'B2C (Small) Invoice', 'Count': $scope.gstrData.b2c_smallInvoice.Vouchers, 'Taxable': $scope.gstrData.b2c_smallInvoice.TaxAbleAmount, 'IGST': $scope.gstrData.b2c_smallInvoice.IGSTAmount,
            'SGST': $scope.gstrData.b2c_smallInvoice.SGSTAmount, 'CGST': $scope.gstrData.b2c_smallInvoice.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.b2c_smallInvoice.TaxAmount, 'Invoice Value': $scope.gstrData.b2c_smallInvoice.TaxAbleAmount + $scope.gstrData.b2c_smallInvoice.TaxAmount
        });
        _data.push({
            'Description': 'Nill rated', 'Count': $scope.gstrData.nillRated.Vouchers, 'Taxable': $scope.gstrData.nillRated.TaxAbleAmount, 'IGST': $scope.gstrData.nillRated.IGSTAmount,
            'SGST': $scope.gstrData.nillRated.SGSTAmount, 'CGST': $scope.gstrData.nillRated.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.nillRated.TaxAmount, 'Invoice Value': $scope.gstrData.nillRated.TaxAbleAmount + $scope.gstrData.nillRated.TaxAmount
        });

        _data.push({
            'Description': '-Exempted', 'Count': $scope.gstrData.nillRated.Vouchers, 'Taxable': $scope.gstrData.nillRated.TaxAbleAmount, 'IGST': $scope.gstrData.nillRated.IGSTAmount,
            'SGST': $scope.gstrData.nillRated.SGSTAmount, 'CGST': $scope.gstrData.nillRated.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.nillRated.TaxAmount, 'Invoice Value': $scope.gstrData.nillRated.TaxAbleAmount + $scope.gstrData.nillRated.TaxAmount
        });

        _data.push({
            'Description': 'Export Invoices', 'Count': $scope.gstrData.exportInvoices.Vouchers, 'Taxable': $scope.gstrData.exportInvoices.TaxAbleAmount, 'IGST': $scope.gstrData.exportInvoices.IGSTAmount,
            'SGST': $scope.gstrData.exportInvoices.SGSTAmount, 'CGST': $scope.gstrData.exportInvoices.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.exportInvoices.TaxAmount, 'Invoice Value': $scope.gstrData.exportInvoices.TaxAbleAmount + $scope.gstrData.exportInvoices.TaxAmount
        });

        _data.push({
            'Description': 'Tax Liability on Advance', 'Count': $scope.gstrData.advanceReceived.Vouchers, 'Taxable': $scope.gstrData.advanceReceived.TaxAbleAmount, 'IGST': $scope.gstrData.advanceReceived.IGSTAmount,
            'SGST': $scope.gstrData.advanceReceived.SGSTAmount, 'CGST': $scope.gstrData.advanceReceived.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.advanceReceived.TaxAmount, 'Invoice Value': $scope.gstrData.advanceReceived.TaxAbleAmount + $scope.gstrData.advanceReceived.TaxAmount
        });

        _data.push({
            'Description': 'Set/Off Tax on Advance Of Prior Period', 'Count': $scope.gstrData.advanceAdjustment.Vouchers, 'Taxable': $scope.gstrData.advanceAdjustment.TaxAbleAmount, 'IGST': $scope.gstrData.advanceAdjustment.IGSTAmount,
            'SGST': $scope.gstrData.advanceAdjustment.SGSTAmount, 'CGST': $scope.gstrData.advanceAdjustment.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.advanceAdjustment.TaxAmount, 'Invoice Value': $scope.gstrData.advanceAdjustment.TaxAbleAmount + $scope.gstrData.advanceAdjustment.TaxAmount
        });
        _data.push({
            'Description': 'Credit/Debit Note (Registered)', 'Count': $scope.gstrData.notesRegistered.Vouchers, 'Taxable': $scope.gstrData.notesRegistered.TaxAbleAmount, 'IGST': $scope.gstrData.notesRegistered.IGSTAmount,
            'SGST': $scope.gstrData.notesRegistered.SGSTAmount, 'CGST': $scope.gstrData.notesRegistered.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.notesRegistered.TaxAmount, 'Invoice Value': $scope.gstrData.notesRegistered.TaxAbleAmount + $scope.gstrData.notesRegistered.TaxAmount
        });
        _data.push({
            'Description': 'Credit/Debit Note (Un-Registered)', 'Count': $scope.gstrData.notesUnRegistered.Vouchers, 'Taxable': $scope.gstrData.notesUnRegistered.TaxAbleAmount, 'IGST': $scope.gstrData.notesUnRegistered.IGSTAmount,
            'SGST': $scope.gstrData.notesUnRegistered.SGSTAmount, 'CGST': $scope.gstrData.notesUnRegistered.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.notesUnRegistered.TaxAmount, 'Invoice Value': $scope.gstrData.notesUnRegistered.TaxAbleAmount + $scope.gstrData.notesUnRegistered.TaxAmount
        });

        _data.push({
            'Description': 'Total', 'Count': $scope.gstrData.total.Vouchers, 'Taxable': $scope.gstrData.total.TaxAbleAmount, 'IGST': $scope.gstrData.total.IGSTAmount,
            'SGST': $scope.gstrData.total.SGSTAmount, 'CGST': $scope.gstrData.total.CGSTAmount, 'CESS': 0, 'Total Tax': $scope.gstrData.total.TaxAmount, 'Invoice Value': $scope.gstrData.total.TaxAbleAmount + $scope.gstrData.total.TaxAmount
        });

        var _sheet1 = {};
        _sheet1.data = _data;
        _sheet1.headers = _headers;
        _sheet1.name = 'gstr1';
        var _sheets = [];
        _sheets.push(_sheet1);
        // ExcelService.exportToExcel(_data, 'gstr1.xlsx', _headers, 'sheet1');
        var _hsnSheetHeaders = ['HSN/SAC Code', 'Description', 'UQC', 'Quantity', 'Tax Rate', 'Taxable',
            'IGST', 'SGST', 'CGST', 'CESS', 'Total Tax', 'Invoice Value'];
        var _b2bData = $scope.gstrData.hsnSummary.filter(o => o.B2B == 1);
        var _b2cData = $scope.gstrData.hsnSummary.filter(o => o.B2B == 0);

        var _b2bSummary = _b2bData.map(function (val, index) {

            return {
                'HSN/SAC Code': val.HSNCode, 'Description': '', 'UQC': val.Unit, 'Quantity': val.Quantity,
                'Tax Rate': val.TaxRate, 'Taxable': val.TaxAbleAmount, 'IGST': val.IGSTAmount, 'SGST': val.SGSTAmount,
                'CGST': val.CGSTAmount, 'Total Tax': val.TaxAmount, 'Invoice Value': val.TaxAmount + val.TaxAbleAmount
            };
        });
        var _b2cSummary = _b2cData.map(function (val, index) {

            return {
                'HSN/SAC Code': val.HSNCode, 'Description': '', 'UQC': val.Unit, 'Quantity': val.Quantity,
                'Tax Rate': val.TaxRate, 'Taxable': val.TaxAbleAmount, 'IGST': val.IGSTAmount, 'SGST': val.SGSTAmount,
                'CGST': val.CGSTAmount, 'Total Tax': val.TaxAmount, 'Invoice Value': val.TaxAmount + val.TaxAbleAmount
            };
        });

        var _hsnB2CHeaders = [{
            'HSN/SAC Code': 'HSN/SAC Code', 'Description': 'Description', 'UQC': 'UQC', 'Quantity': 'Quantity',
            'Tax Rate': 'Tax Rate', 'Taxable': 'Taxable', 'IGST': 'IGST', 'SGST': 'SGST',
            'CGST': 'CGST', 'CESS': 'CESS', 'Total Tax': 'Total Tax', 'Invoice Value': 'Invoice Value'
        }];
        var _hsnSummaryData = [..._b2bSummary, ...[{}, {}], ..._hsnB2CHeaders, ..._b2cSummary];
        



        

        var _hsnSummarySheet = {};
        _hsnSummarySheet.data = _hsnSummaryData;
        _hsnSummarySheet.headers = _hsnSheetHeaders;
        _hsnSummarySheet.name = 'hsn-sac summary';
        _sheets.push(_hsnSummarySheet);
         
        var _b2bHeadersStrings = [
            'Party',
            'GSTIN', 'Voucher', 'Taxable', 'IGST', 'SGST',
            'CGST', 'CESS', 'Total Tax', 'Invoice Value'
        ];
         
        // Build clean data array  one object per B2B entry
        var _b2bDetailsdata = $scope.gstrData.b2b.Details.map(function (val) {
            return {
                'Party': val.Party,
                'GSTIN': val.PartyGST,
                'Voucher': val.VoucherNumber,
                'Taxable': val.TaxAbleAmount,
                'IGST': val.IGSTAmount,
                'SGST': val.SGSTAmount,
                'CGST': val.CGSTAmount,
                'CESS': 0,   // if CESS is always 0 as per other sheets
                'Total Tax': val.TaxAmount,
                'Invoice Value': val.TaxAmount + val.TaxAbleAmount
            };
        });

        // Create sheet object
        var _b2bDetailsSheet = {
            data: _b2bDetailsdata,
            headers: _b2bHeadersStrings,
            name: 'details'
        };

        _sheets.push(_b2bDetailsSheet);

        ExcelService.exportMultipleSheets(_sheets, 'gstr1.xlsx');


    }
    $scope.showDetails = function (details) {
        $scope.details = details;
        $('#gstrDetialDialog').modal('show');
    }

    $scope.showHSNDetails = function (details) {
        $scope.hsnSummary_b2b = details.filter(o => o.B2B == 1);
        $scope.hsnSummary_b2c = details.filter(o => o.B2B == 0);

        $('#gstrHSNSummaryDialog').modal('show');

    }
});
app.controller('GstSalesTaxReportController', function ($scope, $location, $rootScope, $window, ReportService) {

    $scope.gstrData = [];
    debugger
    var token = $rootScope.getTokenInfo();
    var date = new Date();
    // var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { From: convertDate(date), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);

        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }
    $scope.show = function () {

        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);

        ReportService.gstSalesTaxReport(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.gstrData = e.data.Data;

        }, model);
    };
    $scope.exportToExcel = function () {
        const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        const columns = getColumns(data);
        columns.splice(columns.length - 1, 1);
        const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        XLSX.writeFile(workbook, 'gstr1.xlsx');
    }
    function getColumns(data) {
        const columns = [];
        data.forEach(row => {
            Object.keys(row).forEach(col => {
                if (!columns.includes(col)) {
                    columns.push(col);
                }
            });
        });
        return columns;
    }
});
app.controller('GstPurchaseTaxReportController', function ($scope, $location, $rootScope, $window, ReportService) {

    $scope.gstrData = [];

    var token = $rootScope.getTokenInfo();
    var date = new Date();
    // var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { From: convertDate(date), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);

        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }
    $scope.show = function () {

        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);

        ReportService.gstPurchaseTaxReport(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.gstrData = e.data.Data;

        }, model);
    };
    $scope.exportToExcel = function () {
        const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        const columns = getColumns(data);
        columns.splice(columns.length - 1, 1);
        const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        XLSX.writeFile(workbook, 'gstr1.xlsx');
    }
    function getColumns(data) {
        const columns = [];
        data.forEach(row => {
            Object.keys(row).forEach(col => {
                if (!columns.includes(col)) {
                    columns.push(col);
                }
            });
        });
        return columns;
    }
});
app.controller('TrialBalanceController', function ($scope, $location, $rootScope, $window, ReportService) {

    $scope.gstrData = [];

    var token = $rootScope.getTokenInfo();
    var date = new Date();
    // var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { To: convertDate(date), Format: 1, ShowGroup: false, IncludeZeroBalance: true };

    $scope.MinDate = token.FinYearStart;
    $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);

    $scope.show = function () {

        var model = cloneObj($scope.Filter);

        model.To = formatdate(model.To);

        ReportService.trialBalance(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            var _data = e.data.Data;

            if ($scope.Filter.IncludeZeroBalance == false) {
                _data = _data.filter(item => item.OpeningBalance > 0 || item.Debit > 0 || item.Credit > 0)
            }
            /*  if ($scope.Filter.Format == 1) {*/
            $scope.ReportData = $.grep(_data, function (item, i) {

                var amount = item.Credit - item.Debit;
                if (item.OBType == 1) {
                    amount = amount - item.OpeningBalance
                }
                else if (item.OBType == 2) {
                    amount = amount + item.OpeningBalance
                }
                item.CBCredit = amount > 0 ? amount : 0;
                item.CBDebit = amount > 0 ? 0 : Math.abs(amount);
                return item;
            });
            // }

            //if ($scope.Filter.Format == 2) {
            //    $scope.ReportData = $.grep(_data, function (item, i) {
            //        var amount = item.Credit - item.Debit;
            //        item.Credit = amount > 0 ? amount : 0;
            //        item.Debit = amount > 0 ? 0 : Math.abs(amount);
            //        return item;
            //    });
            //}
        }, model);
    };
    $scope.exportToExcel = function () {
        const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        const columns = getColumns(data);
        columns.splice(columns.length - 1, 1);
        const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        XLSX.writeFile(workbook, 'gstr1.xlsx');
    }
    function getColumns(data) {
        const columns = [];
        data.forEach(row => {
            Object.keys(row).forEach(col => {
                if (!columns.includes(col)) {
                    columns.push(col);
                }
            });
        });
        return columns;
    }
});
app.controller('PnlStatementController', function ($scope, $location, $rootScope, $mdDialog, ReportService) {


    var token = $rootScope.getTokenInfo();
    var date = new Date();
    // var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { To: convertDate(date), Format: 1, ShowGroup: false, IncludeZeroBalance: true };


    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);

        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }
    $scope.show = function () {

        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);

        model.To = formatdate(model.To);

        ReportService.pnlStatement(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

            $scope.ReportData = e.data.Data;


        }, model);
    };
    $scope.exportToExcel = function () {
        const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        const columns = getColumns(data);
        columns.splice(columns.length - 1, 1);
        const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        XLSX.writeFile(workbook, 'gstr1.xlsx');
    }
    function getColumns(data) {
        const columns = [];
        data.forEach(row => {
            Object.keys(row).forEach(col => {
                if (!columns.includes(col)) {
                    columns.push(col);
                }
            });
        });
        return columns;
    }

    $scope.showLedgerDetails = function (item) {
        debugger

        $('#dlgLedgerDetails').modal('show');
        $scope.GetAccountLedger(item.AccountGroupId);

    }
    var ledger = new $.Ledger();
    $scope.GetAccountLedger = function (accountGroupId) {

        if ($scope.Filter.LedgerId == 0) {
            alert('Please select party');
            return;
        }
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.AccountGroupId = accountGroupId;
        ledger.LedgerAccGroupTransactions(function (e) {

            $scope.TransList = e.data;

            $scope.TransList = $.grep(e.data, function (n, i) {
                n.Debit = 0;
                n.Credit = 0;
                if (n.TransactionType == 1 || n.TransactionType == 3) {
                    n.Debit = n.TransactionAmount;
                }
                else {
                    n.Credit = n.TransactionAmount;
                }

                if (i == 0) {
                    n.Closingbalance = n.ClosingBalanceBeforeFromDate + n.Credit - n.Debit;
                    $scope.PreviousCB = Math.abs(n.ClosingBalanceBeforeFromDate).toString() + (n.ClosingBalanceBeforeFromDate < 0 ? ' DR' : ' CR');
                }
                else {
                    n.Closingbalance = e.data[i - 1].Closingbalance + n.Credit - n.Debit;
                }

                return n;
            });
            console.log('translist', e.data);
            $scope.LastRow = e.data[e.data.length - 1];

        }, filter);
    }
    $scope.Abs = function (val) {
        return Math.abs(val) + (val < 0 ? ' DR' : ' CR');
    }

});
app.controller('BalanceSheetController', function ($scope, $location, $rootScope, $window, ReportService, $filter) {


    var token = $rootScope.getTokenInfo();
    var date = new Date();
    // var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { To: convertDate(date), Format: 1, ShowGroup: false, IncludeZeroBalance: true };


    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);

        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
    }
    $scope.show = function () {

        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);

        model.To = formatdate(model.To);

        ReportService.balanceSheet(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

            $scope.ReportData = e.data.Data;
            var liabilities = e.data.Data.Groups.filter(o => o.AccType == 'Liability');
            $scope.ReportData.TotalLiability = $filter('sumByKey')(liabilities, 'Amount');
            var assets = e.data.Data.Groups.filter(o => o.AccType == 'Asset');
            $scope.ReportData.TotalAsset = $filter('sumByKey')(assets, 'Amount');

        }, model);
    };
    $scope.exportToExcel = function () {
        const data = $scope.gstrData;// JSON.parse($scope.gstrData);
        const columns = getColumns(data);
        columns.splice(columns.length - 1, 1);
        const worksheet = XLSX.utils.json_to_sheet(data, { header: columns });
        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'gstr1');
        XLSX.writeFile(workbook, 'gstr1.xlsx');
    }
    function getColumns(data) {
        const columns = [];
        data.forEach(row => {
            Object.keys(row).forEach(col => {
                if (!columns.includes(col)) {
                    columns.push(col);
                }
            });
        });
        return columns;
    }
});
app.controller("PartyStockRegisterCrossTab", function ($scope, $routeParams, $rootScope, FileSaver, $window, $filter, LedgerFactory) {
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

    var token = $rootScope.getTokenInfo();
    var date = new Date();
    var filter = { LedgerId: 0, From: '', To: '' };
    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    $scope.PartyStockData = [];
    $scope.OpeningBalance = [];
    $scope.reportType = 'crossTab';
    $scope.challanWiseRows = [];

    function processPartyStockRegisterResponse(resp) {
        var allData = (resp && resp.allData) ? resp.allData : [];
        $scope.PartyStockData = allData;
        $scope.Balanace = resp ? resp.balance : null;
        $scope.PartyIssueTans = jQuery.grep(allData, function (n, i) {
            return (n.TranType == 1);
        });
        $scope.PartyRecTans = jQuery.grep(allData, function (n, i) {
            return (n.TranType == 2);
        });
        $scope.MaxRows = uniqueItems(jQuery.map(allData, function (n, i) {
            return (n.Product);
        }));
        groupDates(allData);

        if ($scope.reportType === 'challanWise') {
            $scope.challanWiseRows = buildChallanWiseLedger(allData);
            if ($scope.gridApi) {
                $scope.gridApi.setGridOption('rowData', []);
            }
            return;
        }
        $scope.challanWiseRows = [];
        var allProducts = jQuery.map(allData, function (n, i) {
            return { Product: n.Product, ProductId: n.ProductId, Unit: n.Unit };
        });
        const prods = uniqueObjects(allProducts, 'Product');
        var sets = prods.filter(o => o.Unit == 'Set');
        if (sets && sets.length > 0) {
            sets.push({});
        }
        var others = prods.filter(o => o.Unit != 'Set');
        const combined = [...sets, ...others];
        if ($scope.gridApi) {
            $scope.gridApi.setGridOption('rowData', combined);
        }
        $scope.updateGridCols();
    }

    $scope.GetPartyStockRegister = function () {
        getPartyOpeningBalance();
        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);
        ledgerDTO.PartyStockRegister(function (e) {
            processPartyStockRegisterResponse(e.data || {});
        }, model);
    }

    $scope.updateGridCols = function () {
        var cols = [{
            field: 'Product', minWidth: 200, autoHeaderHeight: true, sortable: false,
            pinned: 'left', resizable: true
        }];
        cols.push({
            field: 'Unit', headerName: 'UOM', minWidth: 80, maxWidth: 80, suppressSizeToFit: true, headerClass: 'headerRow',
            autoHeaderHeight: true, sortable: false
        });
        var delColumn = {
            headerName: 'Delivery',
            cellStyle: { 'text-align': 'center' },
            children: []
        }

        var issueDates = uniqueItems($scope.PartyIssueTans.map(o => o.TransDate));
        for (var i = 0; i < issueDates.length; i++) {
            var d = $filter('date')(issueDates[i], 'dd/MM/yyyy');
            var challans = $scope.getTransactionChallans(issueDates[i], 1);
            delColumn.children.push({
                field: d, minWidth: 110, suppressSizeToFit: true, headerClass: 'headerRow', transType: 1, date: issueDates[i],
                autoHeaderHeight: true, sortable: false, valueGetter: getDeliveryQty, challans: challans,
                headerComponent: StockRegisterHeaderComponent,
                headerClass: 'crossTabreport-subheader',
                cellStyle: { 'background-color': '#b6edd7', 'text-align': 'center' }
            });
        }

        delColumn.children.push({
            field: 'Total Delivery', minWidth: 110, maxWidth: 140, suppressSizeToFit: true, headerClass: 'headerRow',
            headerClass: 'crossTabreport-subheader',
            headerHeight: 80, sortable: false, valueGetter: (p) => {
                if (!p.data.Product) {
                    return '';
                }
                var initialValue = 0;
                return $scope.PartyIssueTans.filter(o => o.ProductId == p.data.ProductId).reduce(
                    (x, y) => x + y.Quantity,
                    initialValue,
                );

            },

            cellStyle: { 'background-color': '#fff2df', 'text-align': 'right', }
        });
        cols.push(delColumn);
        var retColumn = {
            headerName: 'Return',


            children: []
        }
        // returning items
        var recDates = uniqueItems($scope.PartyRecTans.map(o => o.TransDate));
        for (var i = 0; i < recDates.length; i++) {
            var d = $filter('date')(recDates[i], 'dd/MM/yyyy');
            var challans = $scope.getTransactionChallans(recDates[i], 2);
            retColumn.children.push({
                field: d, minWidth: 110, suppressSizeToFit: true, headerClass: 'headerRow', transType: 2, date: recDates[i],
                headerHeight: 80, sortable: false, valueGetter: getReturnQty, challans: challans,
                headerComponent: StockRegisterHeaderComponent,
                headerClass: 'crossTabreport-subheader',
                cellStyle: { 'background-color': '#d2d9f5', 'text-align': 'center' }
            });
        }
        retColumn.children.push({
            field: 'Total Pickup', minWidth: 110, suppressSizeToFit: true, headerClass: 'headerRow',
            headerClass: 'crossTabreport-subheader', headerHeight: 80, sortable: false,
            valueGetter: (p) => {
                if (!p.data.Product) {
                    return '';
                }
                var initialValue = 0;
                return $scope.PartyRecTans.filter(o => o.ProductId == p.data.ProductId).reduce(
                    (x, y) => x + y.Quantity,
                    initialValue,
                );
            }, cellStyle: { 'background-color': '#fff2df', 'text-align': 'right' }
        });
        cols.push(retColumn);
        var balanceColumn = {
            headerName: 'Balance',
            headerClass: 'headerRow',
            suppressSizeToFit: true,
            maxWidth: 210,
            pinned: 'right',
            children: []
        }
        balanceColumn.children.
            push({
                field: 'Qty',
                pinned: 'right',
                width: 70,
                headerClass: 'crossTabreport-subheader',
                valueGetter: (p) => {
                    if (!p.data.Product) {
                        return '';
                    }
                    var initialValue = 0;
                    return $scope.ProductsQtyTotal(p.data.Product);
                }, cellStyle: { 'text-align': 'center' }
            });
        balanceColumn.children.push({
            field: 'Size', pinned: 'right', width: 70,
            headerClass: 'crossTabreport-subheader',
            valueGetter: (p) => {
                if (!p.data.Product) {
                    return '';
                }
                if ($scope.Balanace) {
                    return $scope.Balanace.find(o => o.ProductId == p.data.ProductId).SizeBalance;
                }
                return 0;
            }, cellStyle: { 'text-align': 'center' }
        });
        balanceColumn.children.push({
            field: 'Weight', pinned: 'right', width: 70,
            headerClass: 'crossTabreport-subheader',
            valueGetter: (p) => {
                if (!p.data.Product) {
                    return '';
                }
                if ($scope.Balanace) {
                    return $scope.Balanace.find(o => o.ProductId == p.data.ProductId).WeightBalance;
                }
                return 0;
            }, cellStyle: { 'text-align': 'center' }
        });
        cols.push(balanceColumn);
        $scope.gridApi.setGridOption("columnDefs", cols);

    }

    function getDeliveryQty(p) {

        if (!p.data.Product) {
            return '';
        }
        var d = $scope.getIssuedProductQtyOnDate(p.data.Product, p.colDef.date);
        return sum(d);


    }
    function getReturnQty(p) {
        if (!p.data.Product) {
            return '';
        }
        var d = $scope.getRecvdProductQtyOnDate(p.data.Product, p.colDef.date);
        return sum(d);

    }
    //gets the product wise opening balance as of the selected date provided in the filter
    function getPartyOpeningBalance() {
        ledgerDTO.PartyOpeningBalance(function (e) {
            $scope.OpeningBalance = e.data;
        }, $scope.Filter);
    }
    function groupDates(data) {
        $scope.StockDates = uniqueItems(jQuery.map(data, function (n, i) {
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
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {
            var sites = e.data.Data;

            sites.unshift({ LedgerSiteId: 0, SiteAddress: 'All Sites' });
            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.PrintStockRegister = function () {
    }
    //gets the transaction Challans for the issue and received items
    $scope.getTransactionChallans = function (obj, transType) {
        //gets the unique challans
        var challans = uniqueItems(jQuery.map(jQuery.grep($scope.PartyStockData, function (n, i) {

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
            return n.Quantity + n.Breakage;
        });
    }
    $scope.dateWiseProductsQtyTotal = function (date, tranType) {
        var totalQty = 0;
        for (var i = 0; i <= $scope.PartyStockData.length - 1; i++) {
            var obj = $scope.PartyStockData[i];
            if (obj.TransDate == date && obj.TranType == tranType) {
                totalQty += parseFloat(obj.Quantity + obj.Breakage);
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
                    totalQty -= parseFloat(obj.Quantity + obj.Breakage);
                }
            }

        }
        // totalQty += calculateOpeningBalance(product)
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
                totalQty -= parseFloat(obj.Quantity + obj.Breakage);
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

    function getOpeningBalanceEntry(product, productId) {
        var list = $scope.OpeningBalance || [];
        for (var i = 0; i < list.length; i++) {
            var obj = list[i];
            if (productId != null && productId !== undefined && productId !== '' && obj.ProductId != null
                && parseInt(obj.ProductId, 10) === parseInt(productId, 10)) {
                return obj;
            }
            if (obj.Product == product) {
                return obj;
            }
        }
        return null;
    }

    /** Aggregate movement lines for one product by date + challan + tran type, then sort. */
    function aggregateProductMovements(lines) {
        var map = {};
        for (var i = 0; i < lines.length; i++) {
            var n = lines[i];
            var key = String(n.TransDate) + '\n' + String(n.JobNumber == null ? '' : n.JobNumber) + '\n' + String(n.TranType);
            if (!map[key]) {
                map[key] = {
                    TransDate: n.TransDate,
                    JobNumber: n.JobNumber,
                    TranType: n.TranType,
                    issue: 0,
                    receive: 0
                };
            }
            var qty = parseFloat(n.Quantity) || 0;
            var br = parseFloat(n.Breakage) || 0;
            if (n.TranType == 1) {
                map[key].issue += qty;
            } else {
                map[key].receive += qty + br;
            }
        }
        var agg = [];
        for (var k in map) {
            if (Object.prototype.hasOwnProperty.call(map, k)) {
                agg.push(map[k]);
            }
        }
        agg.sort(function (a, b) {
            var ta = new Date(a.TransDate).getTime();
            var tb = new Date(b.TransDate).getTime();
            if (isNaN(ta)) {
                ta = 0;
            }
            if (isNaN(tb)) {
                tb = 0;
            }
            if (ta !== tb) {
                return ta - tb;
            }
            var ja = String(a.JobNumber || '');
            var jb = String(b.JobNumber || '');
            if (ja !== jb) {
                return ja.localeCompare(jb);
            }
            return (parseInt(a.TranType, 10) || 0) - (parseInt(b.TranType, 10) || 0);
        });
        return agg;
    }

    /** Per-item challan-wise rows: OB on first row from PartyOpeningBalance; ClosingBalance on last row. */
    function buildChallanWiseLedger(allData) {
        if (!allData || !allData.length) {
            return [];
        }
        var byKey = {};
        for (var i = 0; i < allData.length; i++) {
            var n = allData[i];
            var pk = (n.ProductId != null && n.ProductId !== undefined && n.ProductId !== '')
                ? ('id:' + n.ProductId)
                : ('p:' + String(n.Product || ''));
            if (!byKey[pk]) {
                byKey[pk] = [];
            }
            byKey[pk].push(n);
        }
        var rows = [];
        var usedPk = {};

        function appendProductBucket(lines) {
            if (!lines || !lines.length) {
                return;
            }
            var productName = lines[0].Product;
            var productId = lines[0].ProductId;
            var itemOpening = parseFloat(calculateOpeningBalance(productName)) || 0;
            var obEntry = getOpeningBalanceEntry(productName, productId);
            var closingVal = null;
            if (obEntry != null && obEntry.ClosingBalance != null && obEntry.ClosingBalance !== '') {
                var c = parseFloat(obEntry.ClosingBalance);
                if (!isNaN(c)) {
                    closingVal = c;
                }
            }
            var agg = aggregateProductMovements(lines);
            if (!agg.length) {
                return;
            }
            var balanceBefore = itemOpening;
            for (var j = 0; j < agg.length; j++) {
                var r = agg[j];
                var issue = r.issue || 0;
                var receive = r.receive || 0;
                var balanceAfter = balanceBefore + issue - receive;
                rows.push({
                    item: productName,
                    productId: productId,
                    openingBalance: j === 0 ? itemOpening : null,
                    showOpening: j === 0,
                    displayDate: $filter('date')(r.TransDate, 'dd/MM/yyyy') || String(r.TransDate || ''),
                    challanNo: r.JobNumber == null || r.JobNumber === '' ? '-' : r.JobNumber,
                    issue: issue,
                    receive: receive,
                    balance: balanceAfter,
                    isLastForItem: (j === agg.length - 1),
                    closingBalance: (j === agg.length - 1) ? closingVal : null
                });
                balanceBefore = balanceAfter;
            }
        }

        var mr = $scope.MaxRows || [];
        for (var mi = 0; mi < mr.length; mi++) {
            var pnm = mr[mi];
            for (var pk in byKey) {
                if (!Object.prototype.hasOwnProperty.call(byKey, pk) || usedPk[pk]) {
                    continue;
                }
                var bucket = byKey[pk];
                if (!bucket.length || bucket[0].Product !== pnm) {
                    continue;
                }
                usedPk[pk] = true;
                appendProductBucket(bucket);
            }
        }
        for (var pk2 in byKey) {
            if (!Object.prototype.hasOwnProperty.call(byKey, pk2) || usedPk[pk2]) {
                continue;
            }
            appendProductBucket(byKey[pk2]);
        }
        return rows;
    }

    $scope.export = function () {


        var report = new $.Reports();

        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);

        report.printPartyStockRegister(function (e) {

            FileSaver.saveAs(e.data, 'text.xlsx');
        }, filter);
    };

    /** Export challan-wise ledger to PDF (server builds HTML via XSLT, then PDF). */
    $scope.exportChallanWisePdf = function () {
        if ($scope.reportType !== 'challanWise' || !$scope.challanWiseRows || !$scope.challanWiseRows.length) {
            return;
        }
        var report = new $.Reports();
        var payload = {
            LedgerId: $scope.Filter.LedgerId,
            LedgerSiteId: $scope.Filter.LedgerSiteId || 0,
            From: formatdate($scope.Filter.From),
            To: formatdate($scope.Filter.To),
            Print: true,
            Pdf: true,
            Rows: angular.copy($scope.challanWiseRows)
        };
        report.printPartyStockChallanWisePdf(function (e) {
            FileSaver.saveAs(e.data, 'partyStockChallanWise.pdf');
        }, payload);
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
                EnableToolbar(1);
                showMessage(MessageClass.COMPANY_SAVED);
            });
        }
    }
    $scope.RowSelected = function (index) {



    }
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

    //Grid Options: Contains all of the grid configurations
    $scope.gridOptions = {
        theme: myTheme,
        rowNumbers: true,
        loading: false,
        rowData: null,
        domLayout: 'autoHeight',

        // Columns to be displayed (Should match rowData properties)
        alwaysShowHorizontalScroll: true,
        columnDefs: [
            { field: "Product", pinned: 'left' },

        ],

        defaultColDef: {
            flex: 1,
            autoHeaderHeight: true,
            resizable: false,


        },
    };
    // Create Grid: Create new grid within the #myGrid div, using the Grid Options object
    $scope.gridApi = agGrid.createGrid(document.querySelector("#myGrid"), $scope.gridOptions);

    $scope.$watch('OpeningBalance', function () {
        if ($scope.reportType !== 'challanWise' || !$scope.PartyStockData || !$scope.PartyStockData.length) {
            return;
        }
        $scope.challanWiseRows = buildChallanWiseLedger($scope.PartyStockData);
    }, true);

    $scope.$watch('reportType', function (nv, ov) {
        if (ov === undefined || nv === ov) {
            return;
        }
        if ($scope.PartyStockData && $scope.PartyStockData.length) {
            processPartyStockRegisterResponse({ allData: $scope.PartyStockData, balance: $scope.Balanace });
        }
    });
});

app.controller('BillPaymentReportController', function ($scope, $rootScope, LedgerFactory, FileSaver) {
    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    var ledger = new $.Ledger({});

    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }

    $scope.getReport = function (print) {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.Print = print;
        report.billPaymentSummary(function (e) {

            if (print == true) {
                FileSaver.saveAs(e.data, "test.pdf");

                if (e.data.Code != 200) {
                    //  alert(e.data.Message);
                    return;
                }
            }
            else {
                $scope.ReportData = e.data.Data;
            }

        }, filter);
    }

});

app.controller('PartyPaymentsReportController', function ($scope, $rootScope, $state, LedgerFactory, FileSaver) {
    var date = new Date();
    var token = $rootScope.getTokenInfo();
    var entryType = $state.current.data.entryType || 8;
    $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
        $scope.MinDate = token.FinYearStart;
        $scope.MaxDate = token.FinYearEnd;
    } else {
        $scope.MinDate = '01/01/2018';
        $scope.MaxDate = '31/12/2030';
    }
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    var ledger = new $.Ledger({});
    ledger.GetAllByGroups(function (e) {
        $scope.Accounts = e.data.Data;
    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data || [];
        }, { LedgerId: $scope.Filter.LedgerId || 0 });
    }

    $scope.ReportData = [];
    $scope.getReport = function (print) {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.entryType = entryType;
        filter.Print = !!print;
        report.partyPayments(function (e) {
            if (print) {
                FileSaver.saveAs(e.data, 'partyPayments.pdf');
                return;
            }
            var raw = (e.data && e.data.Data) ? e.data.Data : [];
            raw.forEach(function (item) { item.PartyDisplay = item.CrLedger || item.Name || '-'; });
            $scope.ReportData = raw;
        }, filter);
    };

    getSites();
});

app.controller('AmountReceiveableReportController', function ($scope, $rootScope, LedgerFactory, FileSaver) {
    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    var ledger = new $.Ledger({});

    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.ReportData = [];
    $scope.getReport = function (print) {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.Print = print;
        report.amountReceiveable(function (e) {

            if (print == true) {
                FileSaver.saveAs(e.data, "amountReceiveable.pdf");

                if (e.data.Code != 200) {
                    //  alert(e.data.Message);
                    return;
                }
            }
            else {
                $scope.ReportData = e.data.Data.reportData;
            }

        }, filter);
    }

    $scope.getClientTotal = function (clientGroup, field) {
        debugger
        if (!$scope.ReportData) {
            return 0;
        }
        debugger
        return clientGroup.reduce(function (sum, item) {
            return sum + (parseFloat(item[field]) || 0);
        }, 0);
    };

    $scope.getGrandTotal = function (field) {
        if (!$scope.ReportData) {
            return 0;
        }
        return $scope.ReportData.reduce(function (sum, item) {
            return sum + (parseFloat(item[field]) || 0);
        }, 0);
    };
});


app.controller('PartyStockRegisterController', function ($scope, $rootScope, LedgerFactory, FileSaver) {
    var date = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    var ledger = new $.Ledger({});

    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }

    $scope.getReport = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);

        report.billPaymentSummary(function (e) {

            FileSaver.saveAs(e.data, "test.pdf");

            if (e.data.Code != 200) {
                //  alert(e.data.Message);
                return;
            }
            $scope.ReportData = e.data.Data;
        }, filter);
    }
});
app.controller('PartyReportsController', function ($scope, $rootScope, LedgerFactory, FileSaver, $filter, $state) {
    var date = $scope.Today = new Date();
    var token = $rootScope.getTokenInfo();

    $scope.Filter = { ReportType: 1, LedgerId: 0, LedgerSiteId: 0, ChallanType: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    var ledger = new $.Ledger({});

    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }

    $scope.getReport = function (print) {


        if ($scope.Filter.From == '' || $scope.Filter.To == '') {
            alert('Please from and to date');
            return;
        }

        $scope.Filter.Print = print;
        var filter = cloneObj($scope.Filter);

        if (filter.ReportType == 1) {
            if ($scope.Filter.LedgerId <= 0) {
                alert('Please select party');
                return;
            }
            $scope.partyRegister();
        }
        if (filter.ReportType == 2) {
            $scope.deliveryChallans();
        }
        if (filter.ReportType == 3) {
            $scope.returnChallans();
        }
        if (filter.ReportType == 5) {
            $scope.breakageReport();
        }
        if (filter.ReportType == 6) {
            $scope.lostReport();
        }
        if (filter.ReportType == 7) {
            $scope.excessReport();
        }
        //if (print == true) {
        //    FileSaver.saveAs(e.data, "test.pdf");

        //    if (e.data.Code != 200) {
        //        //  alert(e.data.Message);
        //        return;
        //    }
        //}
        //else {
        //    $scope.ReportData = e.data.Data;
        //}
    }

    $scope.deliveryChallans = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.deliveryChallans(function (e) {
            if (filter.Print) {
                FileSaver.saveAs(e.data, "party delivery challans.pdf");
            }
            if (e.data.Code != 200) {
                return;
            }
            $scope.ReportData = e.data.Data;

        }, filter);
    }
    $scope.returnChallans = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.returnChallans(function (e) {

            if (filter.Print) {
                FileSaver.saveAs(e.data, "party return challans.pdf");
            }
            if (e.data.Code != 200) {
                return;
            }
            $scope.ReportData = e.data.Data;

        }, filter);
    }
    $scope.partyRegister = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.partyRegister(function (e) {


            if (filter.Print) {
                FileSaver.saveAs(e.data, "party register.pdf");
                return;
            }
            if (e.data.Code != 200) {
                return;
            }

            $scope.ReportData = e.data.Data;

        }, filter);
    };

    $scope.varianceReportGroups = [];
    $scope.varianceGrandTotal = 0;

    function rebuildVarianceGroups(measureField) {
        $scope.varianceReportGroups = [];
        $scope.varianceGrandTotal = 0;
        if (!$scope.ReportData || !$scope.ReportData.length) {
            return;
        }
        var ordered = $filter('orderBy')($scope.ReportData, ['Client', 'SiteName', 'ReceivingDate']);
        var groups = [];
        var curKey = null;
        var curGroup = null;
        angular.forEach(ordered, function (row) {
            var key = (row.Client || '') + '\t' + (row.SiteName || '');
            if (key !== curKey) {
                curKey = key;
                curGroup = {
                    Client: row.Client,
                    SiteName: row.SiteName,
                    rows: [],
                    subtotal: 0
                };
                groups.push(curGroup);
            }
            curGroup.rows.push(row);
            var v = parseFloat(row[measureField]) || 0;
            curGroup.subtotal += v;
            $scope.varianceGrandTotal += v;
        });
        $scope.varianceReportGroups = groups;
    }

    $scope.breakageReport = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.breakageReport(function (e) {
            if (filter.Print) {
                FileSaver.saveAs(e.data, 'breakage-report.pdf');
            }
            if (e.data.Code != 200) {
                return;
            }
            $scope.ReportData = e.data.Data;
            rebuildVarianceGroups('Breakage');
        }, filter);
    };

    $scope.lostReport = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.lostReport(function (e) {
            if (filter.Print) {
                FileSaver.saveAs(e.data, 'lost-report.pdf');
            }
            if (e.data.Code != 200) {
                return;
            }
            $scope.ReportData = e.data.Data;
            rebuildVarianceGroups('ShortQty');
        }, filter);
    };

    $scope.excessReport = function () {
        var report = new $.Reports();
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        report.excessReport(function (e) {
            if (filter.Print) {
                FileSaver.saveAs(e.data, 'excess-report.pdf');
            }
            if (e.data.Code != 200) {
                return;
            }
            $scope.ReportData = e.data.Data;
            rebuildVarianceGroups('ExcessQty');
        }, filter);
    };

    var PI_VARIANCE_PREFILL_KEY = 'rentacPiVariancePrefill';

    $scope.generatePiFromVarianceGroup = function (g, mode) {
        if (!g || !g.rows || !g.rows.length) {
            return;
        }
        var seen = {};
        var grnIds = [];
        angular.forEach(g.rows, function (row) {
            var id = row.GRNId;
            if (id && !seen[id]) {
                seen[id] = true;
                grnIds.push(id);
            }
        });
        var row0 = g.rows[0];
        if (!grnIds.length || !row0.LedgerId) {
            alert('No challan data found for this group.');
            return;
        }
        var payload = {
            ledgerId: row0.LedgerId,
            ledgerSiteId: row0.LedgerSiteId || 0,
            grnIds: grnIds,
            mode: mode === 'lost' ? 'lost' : 'breakage',
            hints: g.rows.map(function (r) {
                return {
                    grnId: r.GRNId,
                    productId: r.ProductId,
                    breakage: r.Breakage,
                    shortQty: r.ShortQty
                };
            })
        };
        try {
            sessionStorage.setItem(PI_VARIANCE_PREFILL_KEY, JSON.stringify(payload));
        } catch (ex) { }
        $state.go('pinvoice');
    };

});
app.controller('ItemBalanceCostReportController', ['$scope', '$location', '$routeParams',
    '$rootScope', '$http', '$mdDialog', '$window', 'LedgerFactory', 'FileSaver'
    , function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, LedgerFactory, FileSaver) {
        //receipt or payment type
        // debugger;
        var ledger = new $.Ledger({});

        $scope.ClientWiseItemsList = [];
        FormsValidation.init();

        var token = $rootScope.getTokenInfo();
        var date = new Date();
        // var filter = { LedgerId: 0, From: '', To: '' };
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
            var ledgers = e.data;

            //   ledgers.unshift({ LedgerId: -1, Name: 'All Ledgers' });
            $scope.Accounts = ledgers;
            $scope.Filter.LedgerId = -1;//e.data[0].LedgerId;
            //if ($scope.Filter.LedgerId == null) {
            //    $scope.Filter.LedgerId = e.data[0].LedgerId;
            //}
        });
        $scope.$watch('Filter.LedgerId', function () {
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {
                var sites = e.data.Data;

                sites.unshift({ LedgerSiteId: 0, SiteAddress: 'All Sites' });
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }


        $scope.Print = function () {
            //$scope.Filter.ReportName = 'ClientwiseItemBalance';
            //ledger.PrintReport(function (e) {
            //    console.log('Print report', e.data);
            //    downloadReport('ClientwiseItemBalance.pdf', e.data.Data);

            //}, $scope.Filter);
            $scope.export();
        }
        $scope.dataToExport;
        $scope.GetClientWiseItems = function () {

            //ledger.GetClientWiseItems(function (e) {
            //    $scope.Filter.Print = true;
            //    $scope.ClientWiseItemsList = e.data;

            //    $scope.LastRow = e.data[e.data.length - 1];


            //    //$scope.dataToExport = $.map(e.data, function (val) {
            //    //    return {
            //    //        ClientName: val.ClientName, Site: val.SiteAddress, Product: val.Product, OpeningBalance: val.OpeningBalance, IssuedQty: val.IssuedQty,
            //    //        ReceivedQty: val.ReceivedQty, ClosingBalance: val.ClosingBalance
            //    //    };
            //    //});
            //}, $scope.Filter);
            var report = new $.Reports();


            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);
            /// filter.Print = false;
            report.ClientWiseItemBalance(function (e) {
                $scope.ClientWiseItemsList = e.data;
                $scope.LastRow = e.data[e.data.length - 1];
            }, filter);
        }

        $scope.export = function () {


            var report = new $.Reports();

            var filter = cloneObj($scope.Filter);
            filter.From = formatdate(filter.From);
            filter.To = formatdate(filter.To);
            filter.Print = true;
            report.ClientWiseItemBalance(function (e) {

                FileSaver.saveAs(e.data, 'clientBalance.xlsx');
            }, filter);
        }

    }]);

app.controller('VehicleTravelSummaryController', function ($scope, $rootScope, LedgerFactory) {
    var date = $scope.Today = new Date();
    var token = $rootScope.getTokenInfo();
    var ledger = new $.Ledger({});

    $scope.Filter = { ReportType: 1, LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });


    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.ReprotData = [];
    $scope.getReport = function () {
        var reports = new $.Reports();
        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);

        reports.vehicleTravelSummary(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.ReprotData = e.data.Data;
        }, model);
    }
});

app.controller('TransporterReportController', function ($scope, $rootScope, LedgerFactory) {
    var date = $scope.Today = new Date();
    var token = $rootScope.getTokenInfo();
    var ledger = new $.Ledger({});

    $scope.Filter = { ReportType: 1, LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date) };
    if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }
    //select default ledger if it selected on some other screen
    if ($rootScope.LedgerId) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
        getSites();
    });
    $scope.GetAllTransporters = function () {
        var transDto = new $.Transporter();
        transDto.GetAll(function (e) {
            $scope.Transporters = e.data;
        });
    }
    $scope.GetAllTransporters();
    ledger.GetAllByGroups(function (e) {

        $scope.Accounts = e.data.Data;
        if ($scope.Filter.LedgerId == null) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }

    }, DEBTORS_AND_CREDITORS);

    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Filter.LedgerId });
    }
    $scope.ReprotData = [];
    $scope.getReport = function () {
        var reports = new $.Reports();
        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);

        reports.TransporterReport(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.ReprotData = e.data.Data;
        }, model);
    }
});
