

app.controller('BankEntryListController', ['$scope', '$location', '$routeParams', '$http', '$mdDialog',
    '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService',
    function ($scope, $location, $routeParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService) {
        //receipt or payment type
        $scope.TranType = $routeParams.type == undefined ? 1 : $routeParams.type;
        var accGroup = new $.AccountGroup({});
        var recordSet = new $.RecordSet({});
        $scope.Ledger = new $.Ledger({});
        $scope.EntryTypes = [];
        var date = new Date();
        var ledger = new $.Ledger({});
        accGroup.GetBankEntryTypes(function (e) {
            $scope.EntryTypes = e.data;
        });
        accGroup.GetBanks(function (e) {
            $scope.Banks = e.data;
            GetTranLookup();
        });
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Ledger.LedgerId = item.LedgerId;
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };
        $scope.bankLoaded = function (e) {
            GetTranLookup();
        }

        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
        });

        var token = $rootScope.getTokenInfo();

        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, BankId: 0, EntryType: 0, From: convertDate('01/01/2018'), To: convertDate(date), TransactionType: $scope.TranType };
        $scope.Ledger.To = convertDate(date);

        if (token) {
            $scope.Ledger.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        var lederTrans;
        //InitTransaction(null);
        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }

        $scope.GetBankEntries = function () {
            accGroup.GetBankEntries(function (e) {
                console.log('Bank entries', e.data);
                $scope.Transactions = e.data;
            }, $scope.Filter);
        }
        $scope.Print = function (transactionId) {
            LedgerFactory.PrintTransaction(function (e) {
                if (e.data.Code != '500') {
                    ReportService.Print(e.data.Data);
                }
                else {
                    $scope.Message = e.data.Description;
                    ModalFactory.Info('DialogController', $scope);
                }
            }, { LedgerTransactionId: transactionId });
        }
        $scope.$watch('Filter.LedgerId', function () {
            if (!$scope.Filter) return;
            //var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
            //$scope.Trans.LedgerName = obj.Name;
            //$scope.Trans.CrLedgerId = $scope.Filter.LedgerId;
            ////quick receipt. Customer is paying to company
            //if ($scope.Trans.EntryType == 8) {
            //    $scope.Trans.CrLedgerId = obj.LedgerId;
            //} // quick payment. Company is paying to customer
            //else if ($scope.Trans.EntryType == 9) {
            //    $scope.Trans.DrLedgerId = obj.LedgerId;
            //}
            $rootScope.LedgerId = $scope.Filter.LedgerId;
            getSites($scope.Filter.LedgerId);
        });


        //ledger.GetAll(function (e) {
        //    $scope.Accounts = e.data;
        //    if (!$scope.Filter) return;
        //    if ($scope.Filter.LedgerId == null) {
        //        $scope.Filter.LedgerId = e.data[0].LedgerId;
        //    }
        //    var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
        //    //$scope.Trans.LedgerName = obj.Name;
        //});

        if ($rootScope.LedgerId) {
            $scope.Filter.LedgerId = $rootScope.LedgerId;
            //$scope.Trans.CrLedgerId = $rootScope.LedgerId;

        }
        //function getSites(ledgerId) {
        //    LedgerFactory.GetMasterSites(function (e) {
        //        $scope.LedgerSites = e.data.Data;
        //    }, { LedgerId: ledgerId });
        //}
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Ledger.LedgerId });
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Ledger.LedgerId == null) {
                $scope.Ledger.LedgerId = e.data[0].LedgerId;
            }
        });
        function GetTranLookup() {
            //$scope.Filter.LedgerId = $scope.Filter.CrLedgerId;
            //$scope.Trans.GetLookup(function (e) {
            //    recordSet.LookupData = e.data;
            //    if (recordSet.LookupData.length > 0) {
            //        $scope.OnNavClick(3);// move to last;
            //    }
            //}, filter);
        }

        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
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

app.controller('BankEntryController', ['$scope', '$location', '$routeParams', '$http', '$mdDialog', '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService',
    function ($scope, $location, $routeParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService) {
        //receipt or payment type
        $scope.TranType = $routeParams.type == undefined ? 1 : $routeParams.type;
        var accGroup = new $.AccountGroup({});
        var recordSet = new $.RecordSet({});
        var ledger = new $.Ledger({});
        $scope.EntryTypes = [];
        var lederTrans;
        InitTransaction(null);
        $scope.TransList = [];
        FormsValidation.init();
        var date = new Date();
        var filter = { LedgerId: 0, TransactionDate: convertDate(date) };
        var ledger = new $.Ledger({});
        $scope.LedgerId = 0;
        accGroup.GetBankEntryTypes(function (e) {
            $scope.EntryTypes = e.data;
        });
        accGroup.GetBanks(function (e) {
            $scope.Banks = e.data;
            GetTranLookup();
        });
        $scope.bankLoaded = function (e) {
            GetTranLookup();
        }

        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;

        });
        $scope.CreateTransaction = function () {
            //var m =$('#form-receipt').valid();
            //if(m==false) {
            //    return;
            //}

            if ($scope.Trans.CrLedgerId == 0) {
                return;
            }
            $scope.Trans.CreateTransaction(function (e) {
                var trans = $scope.Trans;
                if (e.data.Code == 500) {
                    alert(e.data.Description);
                    return;
                }
                else {
                    // showMessage(MessageClass.SAVED);
                    $scope.LedgerTransactionId = e.data.Data; //for ok button
                    $scope.Trans.LedgerTransactionId = e.data.Data;
                    $scope.Message = 'Do you want to print the note?';
                    $scope.okbutton = okbutton;
                    ModalFactory.ConfirmToPrint('DialogController', $scope);
                    loadTransactions();
                    // addToList(trans);
                    InitTransaction(trans);
                }
            });
            // InitTransaction(null);

        }
        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }


        $scope.Print = function (transactionId) {
            LedgerFactory.PrintTransaction(function (e) {
                if (e.data.Code != '500') {
                    ReportService.Print(e.data.Data);
                }
                else {
                    $scope.Message = e.data.Description;
                    ModalFactory.Info('DialogController', $scope);
                }
            }, { LedgerTransactionId: transactionId });
        }

        $scope.$watch('LedgerId', function () {

            if ($scope.LedgerId > 0)
                ledgerSelect($scope.LedgerId);
        });
        function ledgerSelect(ledgerId) {

            //credit the giver and debit the receiver
            $scope.Trans.DrLedgerId = ledgerId;
            $scope.Trans.CrLedgerId = $scope.BankId;
            $scope.Trans.BankId = $scope.BankId;
            switch (parseInt($scope.Trans.EntryType, 0)) {
                case 1:
                case 2:
                case 5:
                case 6:
                case 7:
                    $scope.Trans.CrLedgerId = $scope.BankId; //party was paid by the bank via cheque/draft so debit the party.
                    $scope.Trans.DrLedgerId = ledgerId;//bank will be debited
                    break;
                case 3:
                case 4:

                    $scope.Trans.CrLedgerId = ledgerId; //party has paid and deposited in the bank.
                    $scope.Trans.DrLedgerId = $scope.BankId;//bank will be debited
                    break;

            }
            var obj = $scope.Accounts.find(o => o.LedgerId == ledgerId);
            if (obj != undefined) {
                $scope.Trans.LedgerName = obj.Name;
            }
            getClientSites(ledgerId);

        }
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
            $scope.Trans.TransactionDate = convertDate(new Date());
        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
            $scope.TransList = [];
        }
        function GetTranLookup() {
            filter.LedgerId = $scope.Trans.CrLedgerId;
            $scope.Trans.GetLookup(function (e) {
                recordSet.LookupData = e.data;
                if (recordSet.LookupData.length > 0) {
                    $scope.OnNavClick(3);// move to last;
                }
            }, filter);
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

        $scope.OnNavClick = function (id) {
            var newFilter;
            switch (parseInt(id)) {
                case 0:
                    newFilter = recordSet.MoveFirst();
                    break;
                case 1:
                    newFilter = recordSet.MovePrev();
                    break;
                case 2:
                    newFilter = recordSet.MoveNext();
                    break;
                case 3:
                    newFilter = recordSet.MoveLast();
                    break;
            }
            //debugger

            if (newFilter) {
                $scope.Trans.TransactionDate = convertDate(newFilter.TransactionDate);
            }
            loadTransactions();
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
        $scope.DeleteTran = function (item) {
            var d = { ledgerTransactionId: item.LedgerTransactionId };
            ledger.DeleteTransaction(function (e) {
                item.TransactionStatus = 2;
            }, d);
        }
        //----sites
        function getClientSites(ledgerId) {
            //var ledger = new $.Ledger({});
            //ledger.GetSites(function(e){
            //    $scope.Sites = e.data;
            //},{LedgerId:ledgerId});

            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: ledgerId });

        }
    }]);