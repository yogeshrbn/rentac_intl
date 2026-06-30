
app.controller('BankEntryRegisterController', ['$scope', '$location', '$routeParams', '$rootScope', '$http', '$mdDialog', '$window',
    'ModalFactory', 'LedgerFactory', 'ReportService',
    function ($scope, $location, $routeParams, $rootScope, $http, $mdDialog, $window, ModalFactory, LedgerFactory, ReportService) {

        var accGroup = new $.AccountGroup({});
        var recordSet = new $.RecordSet({});
        var ledger = new $.Ledger({});
        $scope.EntryTypes = [];
        var lederTrans;
        InitTransaction(null);
        $scope.TransList = [];
        FormsValidation.init();


        var token = $rootScope.getTokenInfo();
        var date = new Date();

        $scope.Filter = { 'CrLedgerId': 0, To: convertDate(date), 'From': '', 'EntryType': 0, 'TranRefNumber': '', 'DrLedgerId': 0 };
        //  $scope.Filter = { CrLedgerId: 0, From: convertDate('01/01/2018'),TranRefNumber:'', To: convertDate(date),EntryType:0 };
        if (token) {
            $scope.Filter.From = convertDate(token.FinYearStart);
        }

        accGroup.GetBankEntryTypes(function (e) {
            e.data.splice(0, 0, { 'Name': 'All', 'EntryTypeId': 0 });
            $scope.EntryTypes = e.data;
        });
        accGroup.GetBanks(function (e) {
            $scope.Banks = e.data;
            $scope.Banks.splice(0, 0, { 'Name': 'All', 'LedgerId': 0 });
            //    GetTranLookup();
        });
        $scope.bankLoaded = function (e) {
            //   GetTranLookup();
        }
        $scope.FindEntryies = function () {
            ////debugger
            ledger.BankEntryRegister(function (e) {
                $scope.TransList = e.data;

            }, $scope.Filter);
        }
        $scope.PrintRegister = function () {
            ledger.BankEntryRegister_rpt(function (e) {
                var filePath = SERVER_URL + 'temp/' + e.data;

                $window.open(filePath);

            }, $scope.Filter);
        }

        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            // if($scope.Filter.CredgerId == null) {
            $scope.Filter.CrLedgerId = e.data[0].LedgerId;
            //  }
        });
        $scope.$watch('Filter.CrLedgerId', function () {

            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.CrLedgerId });
        });
        //$scope.LedgerSelect = function(obj) {
        //    if (obj != undefined) {
        //        //debugger
        //        $scope.Filter.DrLedgerId = obj.originalObject.LedgerId;
        //    }
        //}
        function addToList(transaction) {
            $scope.TransList.push(transaction);
        }
        function InitTransaction(trans) {
            $scope.Trans = new $.LedgerTrasaction({});
            $('#client_value').focus();
            $('#client_value').val('');
            if (trans != null) {
                $scope.Trans.TransactionDate = trans.TransactionDate;
                $scope.Trans.CrLedgerId = trans.CrLedgerId;
                $scope.Trans.LedgerName = trans.LedgerName;
            }

        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
            $scope.TransList = [];
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


        function loadTransactions() {

            filter.TransactionDate = $scope.Trans.TransactionDate;

            ledger.GetTranDetails(function (e) {
                debugger
                $scope.TransList = e.data;
            }, filter);
        }
        //function convertDate(inputFormat) {
        //    function pad(s) { return (s < 10) ? '0' + s : s; }
        //    var d = new Date(inputFormat);
        //    return [pad(d.getDate()), pad(d.getMonth()+1), d.getFullYear()].join('/');
        //}

        $scope.NewTransaction = function () {
            ClearTransaction();
        }
        $scope.PrintTransaction = function (tranId) {

            LedgerFactory.PrintTransaction(function (e) {
                if (e.data.Code != '500') {
                    ReportService.Print(e.data.Data);
                }
                else {
                    $scope.Message = e.data.Description;
                    ModalFactory.Info('DialogController', $scope);
                }
            }, { LedgerTransactionId: tranId });

        }
        $scope.SendReminder = function (transactionId, remType) {
            //window.event.stopPropagation();
            LedgerFactory.EmailDrCrNote(function (e) {

            }, { LedgerTransactionId: transactionId, ReminderType: remType });
        }
    }]);