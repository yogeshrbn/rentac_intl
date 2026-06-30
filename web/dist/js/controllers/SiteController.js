

app.controller('WorkOrderInfoController', ['$scope', '$rootScope', '$stateParams', '$http',
    '$location', '$window', '$mdDialog', 'LedgerFactory', 'ModalFactory',
    function ($scope, $rootScope, $stateParams, $http, $location, $window, $mdDialog, LedgerFactory, ModalFactory) {
        var sId = $stateParams.wId == undefined ? 0 : $stateParams.wId;
        var worder = new $.WorkOrder({ WorkOrderId: 0 });
        var site = new $.Site({});
        site.LedgerSiteId = sId;
        LedgerFactory.GetMasterSiteById(function (e) {

            $scope.SiteInfo = e.data.Data;
        }, { LedgerSiteId: sId });
        $scope.NewSiteJob = function () {

            ModalFactory.AddEditSiteJob('SiteJobController', site);
        }
        site.GetSiteJobs(function (e) {

            $scope.Jobs = e.data;
        });

        worder.GetDetail(function (e) {

            $scope.WorkOrder = e.data;
            //getTransList();
        });

        worder.GetItems(function (e) {
            $scope.Items = e.data;
        });
        worder.GetSites(function (e) {

            $scope.Sites = e.data;
            $($scope.Sites).each(function (i) {
                loadTaxes(i);
            });
            // $scope.ParentSites = $($scope.Sites).each(o=>o.ParentWorkOrderId == 0);
        });

        $scope.AddSite = function () {
            $location.path('/addSiteBill').search({ wId: wId });
            $scope.$apply();
        };
        $scope.AddSiteItems = function (wId) {
            $location.path('/challanEntry').search({ wId: wId });
            $scope.$apply();
        };
        $scope.EditJob = function (site, event) {
            event.preventDefault();
            debugger
            site.ChallanType = 8; // site job
            site.LedgerId = $scope.SiteInfo.LedgerId;
            ModalFactory.AddEditSiteJob('SiteJobController', site);
        }
        $scope.RowSelected = function (index) {

        };
        $scope.LoadItems = function (index, evt) {
            var e = evt || window.event;

            loadSiteDetails($scope.Jobs[index].SiteId, index);
            toggleIcon(e);
        };

        function loadSiteDetails(siteId, index) {

            site.SiteId = siteId;
            //   var e = evt || window.event;

            if ($scope.Jobs[index].Items == undefined || $scope.Jobs[index].Items == null) {
                //refreshPayments(site.SiteId);
                var job = $scope.Jobs[index];
                getTransList(job);
                site.GetItems(function (e) {
                    $scope.Jobs[index].Items = e.data;
                    // loadTaxes(index);
                    loadGRN(index);



                    //load the taxes

                });
                GetSiteChallans(index);
            }
        }

        function GetSiteChallans(siteIndex) {
            worder.WorkOrderId = $scope.Jobs[siteIndex].WorkOrderId;
            worder.GetSiteChallans(function (e) {
                //  $scope.Challans = e.data;
                $scope.Jobs[siteIndex].Challans = e.data;
                // $scope.Challans = e.data;
            });
        }

        $scope.loadTaxes = function (jobEstimate) {
            console.log(jobEstimate.SiteId);
            if (jobEstimate.Taxes) return;
            jobEstimate.Taxes = [];
            site.SiteId = jobEstimate.SiteId;// $scope.Jobs[siteIndex].SiteId;
            site.GetTaxes(function (e) {
                //  $scope.Jobs[siteIndex].Taxes = e.data;
                jobEstimate.Taxes = e.data;
            });
            return true;
        }
        function refreshPayments(siteId) {
            debugger
            //$scope.Pay = new $.Journal({SiteId:siteId});
            //var _siteIndex =  $scope.Jobs.indexOf($scope.Jobs.find(o=>o.SiteId==siteId));
            //$scope.Pay.GetJournals(function(e){
            //    $scope.Jobs[_siteIndex].Payments = e.data;
            //});
            //  getTransList();
        }
        function loadGRN(index) {

            //  site.JobNumber =   $scope.Sites[index].JobNumber
            site.WorkOrderId = $scope.Jobs[index].WorkOrderId
            site.GetSiteGRN(function (e) {
                debugger
                $scope.Jobs[index].GRNItems = e.data;
            });
        }
        $scope.filterDates = function (item) {
            var isNew = indexedDates.indexOf(item.StartDate) == -1;
            if (isNew) {
                indexedDates.push(event.date);
            }
            return isNew;
        }
        function toggleIcon(evt) {

            var obj = evt.target;
            if ($(obj).hasClass('panel-title')) {
                obj = $(evt.target).parent();
            }

            // $(obj).toggleClass('fa-plus fa-minus');

        }

        $scope.NewEntry = function (siteId) {

            $scope.Pay = new $.Journal({ SiteId: siteId });

            var div = '<div style="width:70%;height:60%"></div>';
            //var workOrder = $scope.WorkOrders[index];
            $(div).load('templ/newPayEntry.html', function () {
                var html = $(this).html();
                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    //locals: {
                    //    workOrder: workOrder
                    //},
                    template: html,
                    parent: angular.element(document.body)
                    //   controller: editController
                });

            });
            $scope.closeDialog = function () {
                $mdDialog.hide();
            }
            $scope.createEntry = function () {

                $scope.Pay.EntryDate = new Date($scope.Pay.EntryDate).toLocaleDateString();
                $scope.Pay.CreateEntry(function (e) {
                    $mdDialog.hide();
                    refreshPayments($scope.Pay.SiteId);
                });


            }
        }

        function PrintReceipt(item, headerType) {
            debugger

            var challan = new $.Challan({ WorkOrderId: item.WorkOrderId, ChallanHeaderType: headerType });
            challan.WorkOrderId = item.WorkOrderId;
            challan.PrintDeliveryChallanReceipt(function (e) {

                var filePath = SERVER_URL + 'temp/' + e.data;
                // $window.target = '_blank';
                $window.open(filePath);
            });
        }

        $scope.PrintReceipt = function (item, event) {

            event.preventDefault();
            openChallanHeaderSelectinDialog(function (e) {
                PrintReceipt(item, e);
            });
            //  var siteInfo =   $scope.WorkOrders.find(o => o.SiteId==siteId)
            //  var siteInfo = $scope.WorkOrders[index];//this.site;

        }
        function openChallanHeaderSelectinDialog(okButton) {
            var div = '<div style="width:50%;height:10%"></div>';

            $(div).load('templ/dialogs/challanHeaderType.html?d=' + new Date().getTime(), function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    height: '200px',
                    template: html,
                    parent: angular.element(document.body),
                    controller: function ($scope, $mdDialog) {
                        $scope.OkButtonClick = function () {
                            $mdDialog.hide();
                            okButton($scope.HeaderType);
                        }
                        $scope.closeDialog = function () {
                            $mdDialog.hide();
                        }
                    }
                });
            });
        }

        function getTransList(job) {

            var date = new Date();
            var token = $rootScope.getTokenInfo();
            var filter = { LedgerId: job.LedgerId, From: convertDate(token.FinYearStart), To: convertDate(date), WorkOrderId: job.WorkOrderId };
            var ledger = new $.Ledger({});
            ledger.SiteWiseLedger(function (e) {
                // $scope.Jobs[jobIndex].TransList = e.data;
                job.TransList = e.data;
                job.LastRow = e.data[e.data.length - 1];
            }, filter);
        }

    }]);
app.controller('WoSettingsController', function ($scope, $routeParams, $http) {
    var sId = $routeParams.sId == undefined ? 0 : $routeParams.sId;
    var site = new $.Site({ SiteId: sId });
    var payReminder = new $.PayReminder({ SiteId: sId });
    $scope.payReminder = payReminder;
    site.GetSiteInfo(function (e) {
        //debugger
        $scope.Site = e.data;
    });
    $scope.Add = function () {
        payReminder.Add(function (e) {
            refresh();
        });
    }
    refresh();
    function refresh() {
        payReminder.GetAll(function (e) {
            $scope.Reminders = e.data;
        });
    }
    $scope.addChallanToSite = function () {
        var workOrder = new $.WorkOrder({ WorkOrderId: $scope.Site.WorkOrderId, Number: $scope.NewWorkOrderNumber });
        workOrder.AddChallanToSite(function (e) {
            showMessage(MessageClass.SAVED);
        });
    }
    $scope.Delete = function (reminderId) {
        // alert(reminderId);
        payReminder.PayReminderId = reminderId;
        payReminder.Delete(function (e) {
            refresh();
        });
    }
});
app.controller('SiteController', function ($scope, $location, $routeParams, $http, $mdDialog, $rootScope) {

    var siteId = $routeParams.siteId == undefined ? 0 : $routeParams.siteId;
    var siteInfo = $scope.currentSite;
    if (siteInfo != null && siteInfo != undefined) {
        //var site = siteInfo;
        $scope.SiteInfo = new $.Site({
            SiteId: siteInfo.SiteId, JobNumber: siteInfo.JobNumber, ChallanNumber: siteInfo.ChallanNumber
            , SiteEng: siteInfo.SiteEng, StartDate: siteInfo.StartDate, Duration: siteInfo.Duration,
            ShaftHeight: siteInfo.ShaftHeight, ShaftSize: siteInfo.ShaftSize, Site: siteInfo.Site
        });

        //    $scope.SiteInfo = site;
        $scope.SiteInfo.StartDate = (new Date(siteInfo.StartDate));
        $scope.SiteInfo.WorkOrderNumber = siteInfo.WorkOrderNumber;
        $scope.SiteInfo.Vehicle = siteInfo.Vehicle;
        $scope.SiteInfo.Driver = siteInfo.Driver;
        $scope.SiteInfo.State = siteInfo.State;

    }
    var siteObj = new $.Site({ SiteId: siteId });
    if (siteId != null && siteId != undefined) {

        siteObj.GetSiteInfo(function (e) {

            $scope.site = e.data;
            siteObj.JobNumber = $scope.site.JobNumber;
        });

    }
    loadSiteDetails();

    function loadSiteDetails() {

        //debugger

        refreshPayments();
        siteObj.GetItems(function (e) {

            $scope.site.Items = e.data;
            //load the taxes
            loadTaxes();
            loadGRN();
        });


    }
    function loadTaxes() {
        //debugger
        siteObj.GetTaxes(function (e) {

            $scope.site.Taxes = e.data;
        });
    }
    function refreshPayments() {

        $scope.Pay = new $.Journal({ SiteId: siteId });
        // var _siteIndex =  $scope.Sites.indexOf($scope.Sites.find(o=>o.SiteId==siteId));
        $scope.Pay.GetJournals(function (e) {
            $scope.site.Payments = e.data;
        });
    }
    function loadGRN() {
        // site.JobNumber =   $scope.Sites[index].JobNumber

        siteObj.GetSiteGRN(function (e) {

            $scope.site.GRNItems = e.data;
        });
    }
    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
    $scope.NewEntry = function () {

        $scope.Pay = new $.Journal({ SiteId: siteId });

        var div = '<div style="width:70%;height:60%"></div>';
        //var workOrder = $scope.WorkOrders[index];
        $(div).load('templ/newPayEntry.html', function () {
            var html = $(this).html();
            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                //locals: {
                //    workOrder: workOrder
                //},
                template: html,
                parent: angular.element(document.body)
                //   controller: editController
            });

        });

        $scope.createEntry = function () {

            $scope.Pay.EntryDate = new Date($scope.Pay.EntryDate).toLocaleDateString();
            $scope.Pay.CreateEntry(function (e) {
                $mdDialog.hide();
                refreshPayments($scope.Pay.SiteId);
            });


        }
    }
    $scope.AddSiteItems = function () {
        //debugger
        $location.path('/workorder').search({ wId: $scope.site.WorkOrderId, sId: siteId });
        $scope.$apply();
    };
    $scope.update = function () {

        $scope.SiteInfo.StartDate = new Date($scope.SiteInfo.StartDate).toLocaleDateString();
        $scope.SiteInfo.Update(function (e) {

            $mdDialog.hide();
            $rootScope.$emit("LoadSites", siteInfo.WorkOrderId);
        });
    }
    var stateCity = new $.StateCity({});
    loadStates();

    function loadStates() {
        stateCity.GetStates(function (e) {
            $scope.States = e.data;

        });
    }
});

app.controller('BankEntryController1', function ($scope, $location, $routeParams, $http, $mdDialog, $rootScope) {
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
    var filter = { LedgerId: 0, TransactionDate: '' };
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
    $scope.CreateTransaction = function () {
        //var m =$('#form-receipt').valid();
        //if(m==false) {
        //    return;
        //}

        if ($scope.Trans.CrLedgerId == 0) {
            return;
        }
        $scope.Trans.CreateTransaction(function (e) {
            debugger
            var trans = $scope.Trans;
            if (e.data.Code != undefined) {
                alert(e.data.Description);
                return;
            }
            else {
                loadTransactions();
                // addToList(trans);
                InitTransaction(trans);
            }
        });
        // InitTransaction(null);

    }
    $scope.BankId = 0;
    $scope.LedgerSelect = function (obj) {
        if (obj != undefined) {

            $scope.Trans.DrLedgerId = obj.originalObject.LedgerId;
            $scope.Trans.CrLedgerId = $scope.BankId;
            switch (parseInt($scope.Trans.EntryType, 0)) {
                case 3:
                case 4:

                    $scope.Trans.CrLedgerId = obj.originalObject.LedgerId; //party has paid and deposited in the bank.
                    $scope.Trans.DrLedgerId = $scope.BankId; //bank will be debited
                    break;

            }

            $scope.Trans.LedgerName = obj.originalObject.Name;
            getClientSites(obj.originalObject.LedgerId);
        }
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

            $scope.TransList = e.data;
        }, filter);
    }
    function convertDate(inputFormat) {
        function pad(s) { return (s < 10) ? '0' + s : s; }
        var d = new Date(inputFormat);
        return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
    }
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
        var ledger = new $.Ledger({});
        ledger.GetSites(function (e) {
            $scope.Sites = e.data;
        }, { LedgerId: ledgerId });
    }
});

app.controller('SiteJobController', ['$scope', '$route', 'ModalFactory', 'localData', function ($scope, $route, ModalFactory, localData) {

    $scope.WorkOrder = new $.WorkOrder({ LedgerId: localData.LedgerId });
    var ledger = new $.Ledger({});

    if (localData.WorkOrderId > 0) {

        $scope.WorkOrder.WorkOrderId = localData.WorkOrderId;
        $scope.WorkOrder.WorkOrderDate = convertDate(localData.StartDate);
        $scope.WorkOrder.SiteInfo = localData;
        $scope.WorkOrder.Number = localData.Number;
    }

    ledger.GetAll(function (e) {
        $scope.Accounts = e.data;
        if ($scope.WorkOrder.LedgerId == null) {

            $scope.WorkOrder.LedgerId = e.data[0].LedgerId;
        }
    });
    $scope.Save = function () {
        //Site job
        if (!localData.WorkOrderId) {
            addNew();
        }
        else {
            update();
        }
    }

    function addNew() {
        $scope.WorkOrder.SiteInfo.LedgerSiteId = localData.LedgerSiteId;
        $scope.WorkOrder.ChallanType = 8;
        $scope.WorkOrder.Add(function (e) {
            if (e.statusText == 'OK') {
                //$location.path('/woInfo').search({ wId: e.data.WorkOrderId });
                //$scope.$apply();

                $scope.warnOnLeave = false;
                $route.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, null);
    }

    function update() {
        debugger
        $scope.WorkOrder.ChallanType = 8;
        $scope.WorkOrder.Update(function (e) {
            alert('updated');

        }, null);
    }
    $scope.CloseDialog = function () {
        ModalFactory.CloseDialog();
    }
}]);
//-- end of sale entry