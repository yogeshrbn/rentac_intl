
/** Sum optional per-bill TDS on quick receipt / payment grids. */
function sumQuickReceiptBillTds(bills) {
    if (!bills || !bills.length) return 0;
    return bills.reduce(function (s, b) {
        return s + (parseFloat(b.TdsAmount, 10) || 0);
    }, 0);
}

app.controller('LedgerTransactionListController', ['$scope', '$location', '$stateParams', '$http', '$mdDialog',
    '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService', '$state',
    function ($scope, $location, $stateParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService, $state) {
        //receipt or payment type
        $scope.TranType = $state.current.data.type == undefined ? 1 : $state.current.data.type;
        $scope.Ledger = new $.LedgerTrasaction({});
        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, From: convertDate('01/01/2018'), To: convertDate(date), TransactionType: $scope.TranType };
        $scope.Ledger.To = convertDate(date);

        if (token) {
            $scope.Ledger.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        var lederTrans;
        //InitTransaction(null);
        $scope.TransList = [];
        //FormsValidation.init();
        $scope.CreateTransaction = function () {
            var m = $('#form-receipt').valid();
            if (m == false) {
                return;
            }
            $scope.Trans.TransactionMode = 1;
            var model = cloneObj($scope.Trans);
            model.TransactionDate = formatdate(model.TransactionDate);
            $scope.Trans.CreateTransaction(function (e) {

                var trans = $scope.Trans;
                if (e.data.Code == 500) {
                    alert(e.data.Description);
                    return;
                }
                else {
                    $scope.LedgerTransactionId = e.data.Data; //for ok button
                    $scope.Trans.LedgerTransactionId = e.data.Data;
                    $scope.Message = 'Do you want to print the note?';
                    $scope.okbutton = okbutton;
                    ModalFactory.ConfirmToPrint('DialogController', $scope);
                    addToList(trans);
                    InitTransaction(trans);
                }
            }, model);
            // InitTransaction(null);

        }
        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }

        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Filter.LedgerId = item.LedgerId;
            }
        };

        $scope.GetTransactions = function () {
            $scope.Ledger.GetDrCrNotes(function (e) {
                console.log('Transactions', e.data);
                $scope.Transactions = e.data.Data;
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
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
            if ($scope.Ledger.LedgerId == null) {
                $scope.Ledger.LedgerId = e.data[0].LedgerId;
            }
        });
        function addToList(transaction) {
            $scope.TransList.push(transaction);
        }
        function InitTransaction(trans) {
            $scope.Trans = new $.LedgerTrasaction({});
            $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
            $('#client_value').focus();
            $('#client_value').val('');
            $scope.Trans.TransactionDate = convertDate(new Date());
            if (trans != null) {
                $scope.Trans.TransactionDate = trans.TransactionDate;

                $scope.Trans.LedgerName = trans.LedgerName;
                //8 for cash receipt, 9 for cash payments


            }

        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
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

app.controller('LedgerTransactionController', ['$scope', '$location', '$stateParams', '$http', '$mdDialog',
    '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService', '$state', '$crypto',
    function ($scope, $location, $stateParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService, $state, $crypto) {
        //receipt or payment type
        
        $scope.TranType = $state.current.data.tranType;
        $scope.EntryType = $state.current.data.entryType;
        var txnId = $stateParams.key;
        var lederTrans;
        InitTransaction(null);
        $scope.TransList = [];
        FormsValidation.init();
        $scope.PaymentModes = StaicData.PAYMENT_MODES;
        $scope.Trans.TransactionMode = 1;
        $scope.Bills = [];
        $scope.RefundTrans = new $.LedgerTrasaction({});
        $scope.totalBillTds = function () {
            return sumQuickReceiptBillTds($scope.Bills);
        };
      
        if (txnId != undefined) {
            $scope.Trans.LedgerTransactionId = $crypto.decrypt(txnId);
            if ($scope.Trans.LedgerTransactionId > 0) {
                var objTxn = new $.LedgerTrasaction();
                objTxn.GetTransactionByForEditId(function (e) {

                    if (e.status == 200) {
                        $scope.Trans = e.data.txn;
                        $scope.Trans.TransactionDate = convertDate(e.data.txn.TransactionDate);
                        if (e.data.vendorBills) {
                            $scope.Bills = e.data.vendorBills;
                            $.grep($scope.Bills, function (bill, i) {
                                bill.billType = 'purchase';
                                bill.BillId = bill.PurchaseId;
                                bill.TdsAmount = bill.TdsAmount || 0;
                            });
                        }
                        // $scope.TransList.push(e.data);
                    }
                    else {
                        alert('Could not fetch data');
                        $state.go('quickReceipts');
                    }

                }, { LedgerTransactionId: $scope.Trans.LedgerTransactionId });
            }

        }
        $scope.RefAccounts = [];
        $scope.getAllByGroups = function () {
            var ledger = new $.Ledger();
            ledger.GetAllByGroups(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.RefAccounts = e.data.Data;
            }, '18,19');
        }
        $scope.getAllByGroups();


        $scope.CreateTransaction = function () {
            $scope.Trans.EntryType = $scope.EntryType;
            $scope.Trans.TransactionType = $scope.TranType;
            var model = cloneObj($scope.Trans);
            saveTxn(model);
        }
        $scope.openRefund = function () {
            $scope.RefundTrans.TransactionDate = convertDate(new Date());
            $scope.RefundTrans.TransactionMode = 1;
            $scope.RefundTrans.TransactionAmount = $scope.Trans.TransactionAmount;
            $('#refundDialog').modal('show');
        }
        $scope.refund = function () {
             
            $scope.RefundTrans.EntryType = 29;
            $scope.RefundTrans.TransactionType = 1;
            $scope.RefundTrans.LedgerId = $scope.Trans.LedgerId;
            $scope.RefundTrans.LedgerSiteId = $scope.Trans.LedgerSiteId;
            $scope.RefundTrans.RefTransactionId = $scope.Trans.LedgerTransactionId;

            var model = cloneObj($scope.RefundTrans);
            saveTxn(model);
        }

        function saveTxn(model) {
            var m = $('#form-receipt').valid();
            if (m == false) {
                return;
            }
             

            model.TransactionDate = formatdate(model.TransactionDate);
            var txn = new $.LedgerTrasaction();


            if ($scope.BillBalance > model.TransactionAmount && $scope.EntryType != 27) {
                alert('Total amount of the selected bills is greater than voucher payment amount.');
                return;
            }
            if ($scope.BillBalance < model.TransactionAmount && $scope.EntryType != 27) {
                var c = confirm('Voucher amount is greater than selected bills, this amount will be un-allocated and shown in the debit of the party.\nAre you sure to proceed?');
                if (!c) {
                    return;
                }
            }
            model.TxnDetails = cloneObj($scope.Bills)

            model.TotalTds = sumQuickReceiptBillTds($scope.Bills);

            txn.CreateTransaction(function (e) {

                var trans = $scope.Trans;
                if (e.data.Code == 500) {
                    alert(e.data.Description);
                    return;
                }
                else {
                    $scope.LedgerTransactionId = e.data.Data; //for ok button
                    $scope.Trans.LedgerTransactionId = e.data.Data;
                    alert('Record saved');


                    $scope.Cancel();

                    //InitTransaction(trans);
                }
            }, model);
            // InitTransaction(null);
        }

        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }

        $scope.Cancel = function () {

            if ($scope.EntryType == 8)
                $state.go('quickReceipts');
            else if ($scope.EntryType == 9)
                $state.go('quickPayments');
            else if ($scope.EntryType == 27 || $scope.EntryType == 29)
                $state.go('securitydepositlist');
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
        $scope.Filter = { LedgerId: 0 };
        $scope.$watch('Trans.LedgerId', function () {
            $scope.getSites($scope.Trans.LedgerId);

            if ($scope.Trans.LedgerTransactionId == 0 || $scope.Trans.LedgerTransactionId == undefined) {
                if ($scope.EntryType == 9) {
                    $scope.getUnpaidBills();
                }
            }

            if (!$scope.Filter || !$scope.Accounts) return;

            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Trans.LedgerId);
            $scope.Trans.LedgerName = obj.Name;

            $rootScope.LedgerId = $scope.Trans.LedgerId;

        });

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {

            $scope.Accounts = e.data;
            if (!$scope.Filter) return;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
            if (obj)
                $scope.Trans.LedgerName = obj.Name;
        });

        if ($rootScope.LedgerId) {
            $scope.Trans.LedgerId = $rootScope.LedgerId;
            // $scope.Trans.CrLedgerId = $rootScope.LedgerId;

        }
        $scope.getSites = function (ledgerId) {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: ledgerId });
        }
        function addToList(transaction) {
            $scope.TransList.push(transaction);
        }
        function InitTransaction(trans) {
            $scope.Trans = new $.LedgerTrasaction({});
            $scope.Trans.LedgerTransactionId = 0;
            $scope.Trans.EntryType = $scope.EntryType;
            $scope.Trans.ReceiptStagingPath = '';
            if (typeof $scope.receiptImagePreview !== 'undefined')
                $scope.receiptImagePreview = '';
            var qrInp = document.getElementById('quickReceiptImageInput');
            if (qrInp) qrInp.value = '';
            $('#client_value').focus();
            $('#client_value').val('');
            $scope.Trans.TransactionDate = convertDate(new Date());
            if (trans != null) {
                $scope.Trans.TransactionDate = trans.TransactionDate;

                $scope.Trans.LedgerName = trans.LedgerName;
                //8 for cash receipt, 9 for cash payments


            }

        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
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

        $scope.getUnpaidBills = function () {
            var txn = new $.Transaction();
            txn.GetUnpaidPurchaseBills(function (e) {
                if (e.data.Code != 200) {
                    alert('Could not load bills');
                    return;
                }
                $scope.Bills = $.grep(e.data.Data, function (n, i) {
                    n.billType = 'purchaes';
                    n.BillId = n.PurchaseId;
                    n.AppliedAmount = 0;
                    n.TdsAmount = 0;
                    return n;
                });

            }, { LedgerId: $scope.Trans.LedgerId });
        }


        $scope.SelectedBill;
        $scope.SelectBillForPayment = function (bill) {
            $scope.Trans.EntityKeyId = bill.PurchaseId;

            $.grep($scope.Bills, function (n, i) {
                n.Selected = 0;
            });
            bill.Selected = 1;
            $scope.SelectedBill = bill;
            $scope.BillBalance = bill.Balance - $scope.Trans.TransactionAmount;
        }
        $scope.BillBalance = 0;

        $scope.onAmountChange = function (bill) {

            if (parseFloat(bill.AppliedAmount, 0) > bill.Balance) {
                bill.AppliedAmount = bill.Balance;
            }
            bill.billType = 'purchase';
            bill.BillId = bill.PurchaseId;
            $scope.BillBalance = $.map($scope.Bills, function (n, i) {
                return n.AppliedAmount == undefined ? 0 : parseFloat(n.AppliedAmount, 0);
            }).reduce((p, v) => {
                return p + v;
            });
        }

        $scope.onTdsChange = function (bill) {
            var t = parseFloat(bill.TdsAmount, 10);
            if (isNaN(t) || t < 0) {
                bill.TdsAmount = 0;
            }
        }

    }]);

app.controller('ReceiptTransactionController', ['$scope', '$location', '$stateParams', '$http', '$mdDialog',
    '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService', '$state', '$crypto', '$timeout',
    function ($scope, $location, $stateParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService, $state, $crypto, $timeout) {
        //receipt or payment type

        $scope.TranType = $state.current.data.tranType;
        $scope.EntryType = $state.current.data.entryType;
        $scope.lockPartySite = false;
        $scope.lockSiteSelect = false;
        var txnId = $stateParams.key;
        var lederTrans;
        InitTransaction(null);
        $scope.TransList = [];
        FormsValidation.init();
        $scope.PaymentModes = StaicData.PAYMENT_MODES;
        $scope.Trans.TransactionMode = 1;
        $scope.Bills = [];
        $scope.receiptImagePreview = '';
        $scope.Trans.ReceiptStagingPath = '';

        $scope.totalBillTds = function () {
            return sumQuickReceiptBillTds($scope.Bills);
        };

        $scope.receiptSavedImageUrl = function () {
            var p = $scope.Trans && $scope.Trans.ReceiptDocumentPath;
            if (!p) return '';
            if (p.indexOf('http') === 0) return p;
            var base = (typeof API_URL !== 'undefined') ? API_URL.replace(/\/?api\/?$/i, '') : '';
            return base +'/' + p;
        };

        $scope.onQuickReceiptFileChange = function (fileInput) {
            if (!fileInput || !fileInput.files || !fileInput.files.length) {
                $scope.Trans.ReceiptStagingPath = '';
                $scope.receiptImagePreview = '';
                return;
            }
            var f = fileInput.files[0];
            if (!f.type || f.type.indexOf('image/') !== 0) {
                alert('Please choose an image file (JPEG, PNG, etc.).');
                fileInput.value = '';
                return;
            }
            var reader = new FileReader();
            reader.onload = function (ev) {
                $timeout(function () {
                    $scope.receiptImagePreview = ev.target.result;
                });
            };
            reader.readAsDataURL(f);
            var stageApi = new $.LedgerTrasaction({});
            stageApi.StageQuickReceiptImage(function (e) {
                $timeout(function () {
                    var d = e.data;
                    if (!d || d.Code === 500 || d.Code === '500') {
                        alert(d && d.Description ? d.Description : 'Could not upload receipt image.');
                        $scope.Trans.ReceiptStagingPath = '';
                        $scope.receiptImagePreview = '';
                        fileInput.value = '';
                        return;
                    }
                    $scope.Trans.ReceiptStagingPath = d.Data;
                });
            }, [f]);
        };

        $scope.clearQuickReceiptImage = function () {
            $scope.Trans.ReceiptStagingPath = '';
            $scope.receiptImagePreview = '';
            var el = document.getElementById('quickReceiptImageInput');
            if (el) el.value = '';
        };

        $scope.removeSavedReceiptImage = function () {
            if (!$scope.Trans.LedgerTransactionId || $scope.Trans.LedgerTransactionId <= 0) {
                return;
            }
            if (!$scope.Trans.ReceiptDocumentPath) {
                return;
            }
            if (!window.confirm('Remove the saved receipt image from this voucher?')) {
                return;
            }
            var api = new $.LedgerTrasaction({});
            var filter = { LedgerTransactionId: $scope.Trans.LedgerTransactionId };
            api.ClearQuickReceiptDocument(function (e) {
                $timeout(function () {
                    var d = e.data;
                    if (!d || d.Code === 500 || d.Code === '500') {
                        alert(d && d.Description ? d.Description : 'Could not remove receipt image.');
                        return;
                    }
                    $scope.Trans.ReceiptDocumentPath = null;
                    $scope.clearQuickReceiptImage();
                });
            }, filter);
        };

        if (txnId != undefined) {
            $scope.Trans.LedgerTransactionId = $crypto.decrypt(txnId);
            if ($scope.Trans.LedgerTransactionId > 0) {
                var objTxn = new $.LedgerTrasaction();
                objTxn.GetTransactionByForEditId(function (e) {

                    if (e.status == 200) {
                        $scope.Trans = e.data.txn;
                        $scope.Trans.TransactionDate = convertDate(e.data.txn.TransactionDate);
                        if (e.data.invoices) {
                            $scope.Bills = e.data.invoices;
                            $.grep($scope.Bills, function (bill, i) {
                                bill.billType = 'invoice';
                                bill.BillId = bill.InvoiceId;
                                bill.TdsAmount = bill.TdsAmount || 0;
                            });
                        }
                        // $scope.TransList.push(e.data);
                    }
                    else {
                        alert('Could not fetch data');
                        $state.go('quickReceipts');
                    }

                }, { LedgerTransactionId: $scope.Trans.LedgerTransactionId });
            }

        }
        $scope.RefAccounts = [];
        $scope.getAllByGroups = function () {
            var ledger = new $.Ledger();
            ledger.GetAllByGroups(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.RefAccounts = e.data.Data;
            }, '18,19');
        }
        $scope.getAllByGroups();


        $scope.CreateTransaction = function () {

            var m = $('#form-receipt').valid();
            if (m == false) {
                return;
            }
            $scope.Trans.EntryType = $scope.EntryType;

            $scope.Trans.TransactionType = $scope.TranType;
            var model = cloneObj($scope.Trans);

            var bills = $scope.Bills || [];
            var appliedAmount = bills.reduce(function (partialSum, a) {
                return partialSum + (parseFloat(a.AppliedAmount, 10) || 0);
            }, 0);
            //if (appliedAmount != model.TransactionAmount && model.Advance == 0) {
            //    alert('Total paid amount of the selected bills must match voucher payment amount.');
            //    return;
            //}

            model.TransactionDate = formatdate(model.TransactionDate);
            var txn = new $.LedgerTrasaction();
            //if ($scope.BillBalance > model.TransactionAmount) {
            //    alert('Total amount of the selected bills is greater than voucher payment amount.');
            //    return;
            //}
            //if ($scope.BillBalance < model.TransactionAmount) {
            //    var c = confirm('Voucher amount is greater than selected bills, this amount will be un-allocated and shown in the debit of the party.\nAre you sure to proceed?');
            //    if (!c) {
            //        return;
            //    }
            //}
            model.TxnDetails = cloneObj($scope.Bills || [])
            model.TDS = sumQuickReceiptBillTds($scope.Bills || []);

            txn.CreateTransaction(function (e) {

                var trans = $scope.Trans;
                if (e.data.Code == 500) {
                    alert(e.data.Description);
                    return;
                }
                else {
                    $scope.LedgerTransactionId = e.data.Data; //for ok button
                    $scope.Trans.LedgerTransactionId = e.data.Data;
                    $scope.Trans.ReceiptStagingPath = '';
                    $scope.receiptImagePreview = '';
                    var elInp = document.getElementById('quickReceiptImageInput');
                    if (elInp) elInp.value = '';
                    alert('Record saved');

                    if ($scope.fromContractModal) {
                        var p = $scope.$parent;
                        while (p) {
                            if (typeof p.loadContractLedger === 'function') {
                                p.loadContractLedger();
                                break;
                            }
                            p = p.$parent;
                        }
                        $('#contractQuickReceiptModal').modal('hide');
                        return;
                    }

                    $scope.Cancel();

                    //InitTransaction(trans);
                }
            }, model);
            // InitTransaction(null);

        }
        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }

        $scope.Cancel = function () {

            if ($scope.fromContractModal) {
                $('#contractQuickReceiptModal').modal('hide');
                return;
            }
            if ($scope.EntryType == 8)
                $state.go('quickReceipts');
            else if ($scope.EntryType == 9)
                $state.go('quickPayments');
            else if ($scope.EntryType == 18)
                $state.go('securitydepositlist');
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
        $scope.Filter = { LedgerId: 0 };
        $scope.$watch('Trans.LedgerId', function () {
            $scope.getSites($scope.Trans.LedgerId);

            if (!$scope.Filter || !$scope.Accounts) return;

            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Trans.LedgerId);
            $scope.Trans.LedgerName = obj.Name;

            $rootScope.LedgerId = $scope.Trans.LedgerId;

        });
        $scope.$watch('Trans.LedgerSiteId', function () {
            if ($scope.Trans.LedgerTransactionId == 0 || $scope.Trans.LedgerTransactionId == undefined) {
                if ($scope.EntryType == 8) {
                    $scope.getUnpaidBills();
                }
            }
        });
        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {

            $scope.Accounts = e.data;
            if (!$scope.Filter) return;
            if ($scope.Filter.LedgerId == null) {
                $scope.Filter.LedgerId = e.data[0].LedgerId;
            }
            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
            if (obj)
                $scope.Trans.LedgerName = obj.Name;
        });

        if ($rootScope.LedgerId) {
            $scope.Trans.LedgerId = $rootScope.LedgerId;
            // $scope.Trans.CrLedgerId = $rootScope.LedgerId;

        }
        $scope.getSites = function (ledgerId) {
            LedgerFactory.GetMasterSites(function (e) {
                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: ledgerId });
        }
        function addToList(transaction) {
            $scope.TransList.push(transaction);
        }
        function InitTransaction(trans) {
            $scope.Trans = new $.LedgerTrasaction({});
            $scope.Trans.LedgerTransactionId = 0;
            $scope.Trans.EntryType = $scope.EntryType;
            $scope.Trans.ReceiptStagingPath = '';
            if (typeof $scope.receiptImagePreview !== 'undefined')
                $scope.receiptImagePreview = '';
            var qrInp = document.getElementById('quickReceiptImageInput');
            if (qrInp) qrInp.value = '';
            $('#client_value').focus();
            $('#client_value').val('');
            $scope.Trans.TransactionDate = convertDate(new Date());
            if (trans != null) {
                $scope.Trans.TransactionDate = trans.TransactionDate;

                $scope.Trans.LedgerName = trans.LedgerName;
                //8 for cash receipt, 9 for cash payments


            }

        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
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

        $scope.getUnpaidBills = function () {
            var bill = new $.Billing();
            bill.GetUnpaidInvoices(function (e) {
                if (e.data.Code != 200) {
                    alert('Could not load bills');
                    return;
                }
                $scope.Bills = $.grep(e.data.Data, function (n, i) {
                    n.billType = 'invoice';
                    n.BillId = n.InvoiceId;
                    n.AppliedAmount = 0;
                    n.TdsAmount = 0;
                    return n;
                });

            }, { LedgerId: $scope.Trans.LedgerId, LedgerSiteId: $scope.Trans.LedgerSiteId });
        }


        $scope.SelectedBill;
        $scope.SelectBillForPayment = function (bill) {
            $scope.Trans.EntityKeyId = bill.PurchaseId;

            $.grep($scope.Bills, function (n, i) {
                n.Selected = 0;
            });
            bill.Selected = 1;
            $scope.SelectedBill = bill;
            $scope.BillBalance = bill.Balance - $scope.Trans.TransactionAmount;
        }
        $scope.BillBalance = 0;

        $scope.onAmountChange = function (bill) {

            if (parseFloat(bill.AppliedAmount, 0) > bill.Balance) {
                bill.AppliedAmount = bill.Balance;
            }
            bill.billType = 'invoice';
            bill.BillId = bill.InvoiceId;
            //$scope.BillBalance = $.map($scope.Bills, function (n, i) {
            //    return n.AppliedAmount == undefined ? 0 : parseFloat(n.AppliedAmount, 0);
            //}).reduce((p, v) => {
            //    return p + v;
            //});
        }

        $scope.onTdsChange = function (bill) {
            var t = parseFloat(bill.TdsAmount, 10);
            if (isNaN(t) || t < 0) {
                bill.TdsAmount = 0;
            }
        }

        $scope.onPaidAmountChange = function () {
            distributeAmount($scope.Trans.TransactionAmount, $scope.Bills);
        }
        function distributeAmount(totalAmount, bills) {
            const allocations = [];
            for (const bill of bills) {
                bill.AppliedAmount = 0;
                bill.TdsAmount = 0;
                bill.billType = 'invoice';
                bill.BillId = bill.InvoiceId;
            }
            for (const bill of bills) {
                if (totalAmount <= 0) break;

                bill.AppliedAmount = Math.min(bill.Balance, totalAmount);


                totalAmount -= bill.AppliedAmount;
            }

            return allocations;
        }


        $scope.onAdvanceChange = function () {

            $scope.SelectedBill = [];
            $scope.BillBalance = 0;
            $scope.Bills = [];
            if ($scope.Trans.Advance == 0) {
                $scope.getUnpaidBills();
            }
        }
    }]);

/** Receive payment from contract info modal — reuses ReceiptTransactionController with fake $state. */
app.controller('ContractQuickReceiptController', ['$scope', '$controller', '$rootScope', '$timeout',
    function ($scope, $controller, $rootScope, $timeout) {
        var savedRootLedgerId = $rootScope.LedgerId;
        $rootScope.LedgerId = null;
        var fakeState = { current: { data: { tranType: 2, entryType: 8 } }, go: angular.noop };
        $controller('ReceiptTransactionController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: {},
            $timeout: $timeout
        });
        $rootScope.LedgerId = savedRootLedgerId;

        $scope.fromContractModal = true;
        $scope.lockPartySite = false;
        $scope.lockSiteSelect = false;

        function getContractInfoScope() {
            var s = $scope.$parent;
            while (s) {
                if (s.Contract && s.Contract.ContractId !== undefined && s.loadContractLedger) {
                    return s;
                }
                s = s.$parent;
            }
            return null;
        }

        function applyContractPartySite() {
            var parentScope = getContractInfoScope();
            var c = parentScope && parentScope.Contract;
            if (!c || !$scope.Trans) {
                return;
            }
            $scope.lockPartySite = (c.LedgerId > 0);
            $scope.lockSiteSelect = (c.LedgerSiteId > 0);
            if (c.LedgerId > 0) {
                $scope.Trans.LedgerId = c.LedgerId;
            }
            if (c.LedgerSiteId > 0) {
                $scope.Trans.LedgerSiteId = c.LedgerSiteId;
            }
            $scope.Trans.ContractId = c.ContractId || 0;
            if (c.LedgerId > 0 && $scope.getSites) {
                $scope.getSites(c.LedgerId);
            }
        }

        $scope.$watch(function () {
            var parentScope = getContractInfoScope();
            var c = parentScope && parentScope.Contract;
            return c ? c.ContractId + ':' + c.LedgerId + ':' + c.LedgerSiteId : '';
        }, function () {
            applyContractPartySite();
        });

        $timeout(function () {
            applyContractPartySite();
        }, 0);
    }]);

/** Contract info modal: measure bill — reuses MeasureBillController with synthetic route params. */
app.controller('ContractMeasureBillModalController', ['$scope', '$controller', '$crypto',
    function ($scope, $controller, $crypto) {
        var parent = $scope.$parent;
        while (parent && typeof parent.getBills !== 'function') {
            parent = parent.$parent;
        }
        if (!parent || !parent.Contract || !parent.Contract.ContractId) {
            return;
        }
        var cIdEnc = $crypto.encrypt(parent.Contract.ContractId);
        $scope.fromContractInfoModal = true;
        $scope.billQuotationIds = angular.isArray(parent.billQuotationIds)
            ? angular.copy(parent.billQuotationIds)
            : [];
        var fakeState = {
            current: { data: { authorizedRoles: ['ADMIN'], billType: 1, menuCluster: 'admin-users' } },
            go: angular.noop
        };
        $controller('MeasureBillController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: { cId: cIdEnc }
        });
    }]);

/** Contract info modal: delivery challan — reuses RentChallanController (challanType 1). */
app.controller('ContractDeliveryChallanModalController', ['$scope', '$controller', '$crypto',
    function ($scope, $controller, $crypto) {
        var parent = $scope.$parent;
        while (parent && typeof parent.getBills !== 'function') {
            parent = parent.$parent;
        }
        if (!parent || !parent.SelectedJob || !parent.Contract || !parent.Contract.ContractId) {
            return;
        }
        var jcKey = parent.SelectedJob.JobCardId + ',' + parent.Contract.ContractId;
        var encKey = $crypto.encrypt(jcKey);
        $scope.fromContractInfoModal = true;
        var fakeState = {
            current: { data: { challanType: 1, authorizedRoles: ['ADMIN'], menuCluster: 'admin-users' } },
            go: angular.noop
        };
        $controller('RentChallanController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: { JobCardId: encKey, WorkOrderId: 0 }
        });
    }]);

/** Contract info modal: return challan — reuses InventoryController (challanType 11). */
app.controller('ContractReturnChallanModalController', ['$scope', '$controller', '$crypto',
    function ($scope, $controller, $crypto) {
        var parent = $scope.$parent;
        while (parent && typeof parent.getBills !== 'function') {
            parent = parent.$parent;
        }
        if (!parent || !parent.SelectedJob || !parent.Contract || !parent.Contract.ContractId) {
            return;
        }
        var jcKey = parent.SelectedJob.JobCardId + ',' + parent.Contract.ContractId;
        var encKey = $crypto.encrypt(jcKey);
        $scope.fromContractInfoModal = true;
        var fakeState = {
            current: { data: { challanType: 11, authorizedRoles: ['ADMIN'], menuCluster: 'admin-users' } },
            go: angular.noop
        };
        $controller('InventoryController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: { JobCardId: encKey, GRNId: 0 }
        });
    }]);

/** Contract info modal: edit quotation — reuses QuotationEditController. */
app.controller('ContractEditQuotationModalController', ['$scope', '$controller', '$crypto',
    function ($scope, $controller, $crypto) {
        var parent = $scope.$parent;
        while (parent && typeof parent.refreshContractInfoLists !== 'function') {
            parent = parent.$parent;
        }
        if (!parent || !parent.Contract) {
            return;
        }
        var qid = parent.editQuotationTargetId || parent.Contract.QuotationId;
        if (!qid) {
            return;
        }
        $scope.fromContractInfoModal = true;
        var fakeState = {
            current: {
                data: {
                    category: 'quotation',
                    authorizedRoles: ['ADMIN'],
                    menuCluster: 'admin-users'
                }
            },
            go: angular.noop
        };
        $controller('QuotationEditController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: { key: $crypto.encrypt(qid) }
        });
    }]);

/** Contract info modal: new quotation — reuses QuotationController. */
app.controller('ContractNewQuotationModalController', ['$scope', '$controller', '$timeout',
    function ($scope, $controller, $timeout) {
        var parent = $scope.$parent;
        while (parent && typeof parent.refreshContractInfoLists !== 'function') {
            parent = parent.$parent;
        }
        if (!parent || !parent.Contract || !parent.Contract.ContractId) {
            return;
        }
        $scope.fromContractInfoModal = true;
        var fakeState = {
            current: {
                data: {
                    category: 'quotation',
                    authorizedRoles: ['ADMIN'],
                    menuCluster: 'admin-users'
                }
            },
            go: angular.noop
        };
        $controller('QuotationController', {
            $scope: $scope,
            $state: fakeState,
            $stateParams: {}
        });
        $timeout(function () {
            var c = parent.Contract;
            $scope.Trans.ContractId = c.ContractId;
            $scope.Trans.LedgerId = c.LedgerId || 0;
            $scope.Trans.LedgerSiteId = c.LedgerSiteId || 0;
            $scope.Trans.QuotationType = 16;
            $scope.Trans.Area = c.Area != null ? c.Area : 0;
            $scope.Trans.MeasureType = c.MeasureType != null ? c.MeasureType : 1;
            if (c.EffectiveFrom) {
                $scope.Trans.From = convertDate(c.EffectiveFrom);
            }
            if (c.ValidTill) {
                $scope.Trans.To = convertDate(c.ValidTill);
            }
        }, 200);
    }]);


app.controller('ReceiptRegisterController', ['$scope', '$state', '$stateParams', '$rootScope', '$http', '$mdDialog',
    '$rootScope', '$window', 'LedgerFactory', 'ReportService', '$crypto', 'ModalFactory',
    function ($scope, $state, $stateParams, $rootScope, $http, $mdDialog, $rootScope, $window, LedgerFactory,
        ReportService, $crypto, ModalFactory) {

        // $scope.TranType = $state.current.data.type;
        $scope.EntryType = $state.current.data.entryType;
        //-- 8 is for cash receipt and 9 is for cash payment.
        // var filter = {LedgerId :0, From :'', To : '',EntryType: $scope.TranType == 1 ? 8 : 9};

        // $scope.Filter = filter;
        var token = $rootScope.getTokenInfo();
        var date = new Date();
        var filter = { LedgerId: 0, From: '', To: '' };
        $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date), EntryType: $scope.EntryType };
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
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Filter.LedgerId });
        }
        $scope.Trans = new $.LedgerTrasaction({});
        $scope.registerReceiptImageUrl = function (item) {
              
            if (!item || !item.ReceiptDocumentPath) return '';
            var p = item.ReceiptDocumentPath;
            if (p.indexOf('http') === 0) return p;
            var base = (typeof API_URL !== 'undefined') ? API_URL.replace(/\/?api\/?$/i, '') : '';
            return base + '/'+ p;
        };
        $scope.GetReceiptRegister = function () {
            //debugger
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);
            //  model.EntryType = $scope.TranType;
            $scope.Trans.GetReceiptRegister(function (e) {
                $scope.Receipts = e.data;

            }, model);
        }
        $scope.TotalAmount = function () {
            var total = 0;
            if ($scope.Receipts != undefined) {
                for (var i = 0; i <= $scope.Receipts.length - 1; i++) {
                    total += parseFloat($scope.Receipts[i].TransactionAmount);
                }
            }
            return total;
        }
        //$scope.LedgerSelect = function(obj) {
        //    if (obj != undefined) {
        //        //debugger
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
        $scope.PrintReceipts = function () {


            $scope.Trans.PrintReceipts(function (e) {
                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);


            }, $scope.Filter);
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

        $scope.edit = function (txnId) {
            var _key = $crypto.encrypt(txnId);
            if ($scope.EntryType == 8) {

                $state.go('editquickReceipt', { key: _key });
            }
            else if ($scope.EntryType == 9) {
                $state.go('editquickPayment', { key: _key });
            }
            else if ($scope.EntryType == 27) {
                $state.go('editsecuritydeposit', { key: _key });
            }
        }
        //Currently used only for security deposits
        $scope.DeleteTran = function (item) {
             
            var deleteController = function ($scope) {
                var d = { ledgerTransactionId: item.LedgerTransactionId };
                $scope.Message = 'Are you sure to delete this record?';
                $scope.OkButtonClick = function () {
                    ModalFactory.Dialog.hide();
                    ledger.DeleteTransaction(function (e) {
                        item.TransactionStatus = 2;
                        $scope.GetReceiptRegister();
                    }, d);
                };
                $scope.closeDialog = function () {
                    ModalFactory.Dialog.hide();
                };
            }
            ModalFactory.Confirm(deleteController, $scope, $('body'));


        }
    }]);

app.controller('JournalListController', ['$scope', '$location', '$state', '$http', '$mdDialog', '$rootScope',
    '$window',
    'ModalFactory', 'LedgerFactory', 'ReportService', '$crypto',
    function ($scope, $location, $state, $http, $mdDialog, $rootScope, $window, ModalFactory, LedgerFactory, ReportService, $crypto) {


        $scope.EntryType = $state.current.data.entryType;

        $scope.Ledger = new $.LedgerTrasaction({});
        var date = new Date();
        var token = $rootScope.getTokenInfo();

        if (token != null)
            $scope.MinDate = token.FinYearStart;

        $scope.Filter = { LedgerId: 0, LedgerSiteId: 0, EntryType: $scope.EntryType, From: convertDate($scope.MinDate), To: convertDate(date) };
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

        $scope.GetTransactions = function () {
            var model = cloneObj($scope.Filter);
            model.From = formatdate(model.From);
            model.To = formatdate(model.To);

            $scope.Ledger.GetAllTransactions(function (e) {

                $scope.data = e.data;

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

        LedgerFactory.GetAllParties(function (e) {
            if ($scope.EntryType == 25)
                $scope.Accounts = e.data.filter(o => o.AccountGroup == 18 || o.AccountGroup == 19);
            else
                $scope.Accounts = e.data;
        });



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

            if ($scope.EntryType == 24)
                $state.go('editjournal', { key: _key });
            else
                $state.go('editcontra', { key: _key });



        }

    }]);
app.controller('JournalController', ['$scope', '$location', '$stateParams', '$http', '$mdDialog',
    '$rootScope', 'LedgerFactory', 'ModalFactory', 'ReportService', '$state', '$crypto',
    function ($scope, $location, $stateParams, $http, $mdDialog, $rootScope, LedgerFactory, ModalFactory, ReportService, $state, $crypto) {
        //receipt or payment type


        $scope.EntryType = $state.current.data.entryType;
        var txnId = $stateParams.key;
        var lederTrans;
        InitTransaction(null);
        $scope.TransList = [];
        FormsValidation.init();

        $scope.Trans.TransactionMode = 1;
        $scope.Bills = [];

        if (txnId != undefined) {
            $scope.Trans.LedgerTransactionId = $crypto.decrypt(txnId);
            if ($scope.Trans.LedgerTransactionId > 0) {
                var objTxn = new $.LedgerTrasaction();
                objTxn.GetTransactionByForEditId(function (e) {

                    if (e.status == 200) {
                        $scope.Trans = e.data.txn;
                        $scope.Trans.TransactionDate = convertDate(e.data.txn.TransactionDate);
                        if (e.data.vendorBills) {
                            $scope.Bills = e.data.vendorBills;
                            $.grep($scope.Bills, function (bill, i) {
                                bill.billType = 'purchase';
                                bill.BillId = bill.PurchaseId;
                                bill.TdsAmount = bill.TdsAmount || 0;
                            });
                        }

                    }
                    else {
                        alert('Could not fetch data');
                        $state.go('journal');
                    }

                }, { LedgerTransactionId: $scope.Trans.LedgerTransactionId });
            }

        }


        $scope.CreateTransaction = function () {

            //var m = $('#form-receipt').valid();
            //if (m == false) {
            //    return;
            //}
            $scope.Trans.EntryType = $scope.EntryType;




            var model = cloneObj($scope.Trans);
            model.TransactionDate = formatdate(model.TransactionDate);
            var txn = new $.LedgerTrasaction();


            if (model.LedgerId == model.RefLedgerId) {
                alert('Dr and Cr accoiunts can not be same.');
                return;
            }
            if (model.LedgerId < 1 || model.RefLedgerId < 1) {
                alert('Please select both accounts.');
                return;
            }


            txn.CreateTransaction(function (e) {

                var trans = $scope.Trans;
                if (e.data.Code == 500) {
                    alert(e.data.Description);
                    return;
                }
                else {
                    $scope.LedgerTransactionId = e.data.Data; //for ok button
                    $scope.Trans.LedgerTransactionId = e.data.Data;
                    alert('Record saved');


                    $scope.Cancel();


                }
            }, model);


        }
        function okbutton(modalScope) {
            $scope.Print(modalScope.LedgerTransactionId);
        }

        $scope.Cancel = function () {
            if ($scope.EntryType == 24)
                $state.go('journal');
            else
                $state.go('contra');
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
        $scope.Filter = { LedgerId: 0 };
        $scope.$watch('Trans.LedgerId', function () {
            //  $scope.getSites($scope.Trans.LedgerId);
            if (!$scope.Filter || !$scope.Accounts) return;
            debugger
            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Trans.LedgerId);


        });
        $scope.$watch('Trans.RefLedgerId', function () {
            //  $scope.getSites($scope.Trans.LedgerId);
            if (!$scope.Filter || !$scope.Accounts) return;

            var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Trans.LedgerId);


        });



        var ledger = new $.Ledger({});
        debugger
        if ($scope.EntryType == 25) {
            ledger.GetAllByGroups(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.Accounts = e.data.Data;
                var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
                if (obj)
                    $scope.Trans.LedgerName = obj.Name;
            }, '18,19');
        }
        else {
            ledger.GetAll(function (e) {

                $scope.Accounts = e.data;
                if (!$scope.Filter) return;
                if ($scope.Filter.LedgerId == null) {
                    $scope.Filter.LedgerId = e.data[0].LedgerId;
                }
                var obj = $scope.Accounts.find(o => o.LedgerId == $scope.Filter.LedgerId);
                if (obj)
                    $scope.Trans.LedgerName = obj.Name;
            });
        }
        if ($rootScope.LedgerId) {
            $scope.Trans.LedgerId = $rootScope.LedgerId;
            // $scope.Trans.CrLedgerId = $rootScope.LedgerId;

        }


        function InitTransaction(trans) {
            $scope.Trans = new $.LedgerTrasaction({});
            $scope.Trans.LedgerTransactionId = 0;
            $scope.Trans.EntryType = $scope.EntryType;
            $scope.Trans.ReceiptStagingPath = '';
            if (typeof $scope.receiptImagePreview !== 'undefined')
                $scope.receiptImagePreview = '';
            var qrInp = document.getElementById('quickReceiptImageInput');
            if (qrInp) qrInp.value = '';
            $('#client_value').focus();
            $('#client_value').val('');
            $scope.Trans.TransactionDate = convertDate(new Date());
            if (trans != null) {
                $scope.Trans.TransactionDate = trans.TransactionDate;

                $scope.Trans.LedgerName = trans.LedgerName;
                //8 for cash receipt, 9 for cash payments


            }

        }
        function ClearTransaction() {
            $scope.Trans.Trans = 0;
            $scope.Trans.Desc = '';
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




        $scope.BillBalance = 0;


    }]);
