app.controller('ItemsReceivedRegister', ['$scope', '$rootScope',
    'WorkOrderFactory', 'ModalFactory', 'AuthenticationService',
    'LedgerFactory', '$state', '$crypto', '$timeout', 'ReportService', 'FileSaver',
    function ($scope, $rootScope, WorkOrderFactory, ModalFactory, authService,

        LedgerFactory, $state, $crypto, $timeout, ReportService, FileSaver) {
        var wOrder = new $.WorkOrder({});
        $scope.ChallanType = $state.current.data.challanType;
        $scope.Billing = { 'From': '', 'To': '', LedgerId: 0, 'Print': false, ChallanType: $scope.ChallanType, ReceivedListStatusFilter: 'All' };

        var date = new Date();
        var token = $rootScope.getTokenInfo();
        $scope.Billing.To = convertDate(date);
        $scope.printHeaderType = 'none';
        var RECVD_LIST_FILTER_PREFIX = 'rentacRecvdListFilter_v1_';

        function recvdListStorageKey() {
            return RECVD_LIST_FILTER_PREFIX + ($state.current.name || 'recvdList');
        }

        function persistRecvdListFilter() {
            try {
                var payload = {
                    LedgerId: $scope.Billing.LedgerId,
                    LedgerSiteId: $scope.Billing.LedgerSiteId,
                    From: $scope.Billing.From,
                    To: $scope.Billing.To,
                    ChallanNo: $scope.Billing.ChallanNo,
                    ReceivedListStatusFilter: $scope.Billing.ReceivedListStatusFilter,
                    ChallanType: $scope.ChallanType,
                    CurrentPage: $scope.CurrentPage
                };
                sessionStorage.setItem(recvdListStorageKey(), JSON.stringify(payload));
            } catch (ex) { }
        }

        function persistRecvdListWithScrollTarget(grnId) {
            try {
                var payload = {
                    LedgerId: $scope.Billing.LedgerId,
                    LedgerSiteId: $scope.Billing.LedgerSiteId,
                    From: $scope.Billing.From,
                    To: $scope.Billing.To,
                    ChallanNo: $scope.Billing.ChallanNo,
                    ReceivedListStatusFilter: $scope.Billing.ReceivedListStatusFilter,
                    ChallanType: $scope.ChallanType,
                    CurrentPage: $scope.CurrentPage,
                    pendingScrollToGrnId: grnId
                };
                sessionStorage.setItem(recvdListStorageKey(), JSON.stringify(payload));
            } catch (ex) { }
        }

        function restoreRecvdListFilter() {
            try {
                var raw = sessionStorage.getItem(recvdListStorageKey());
                return raw ? JSON.parse(raw) : null;
            } catch (ex) {
                return null;
            }
        }

        var savedRecvdFilter = restoreRecvdListFilter();

        function afterRecvdListLoaded() {
            var pendingId = null;
            try {
                var st = restoreRecvdListFilter();
                if (st && st.pendingScrollToGrnId) {
                    pendingId = st.pendingScrollToGrnId;
                }
            } catch (ex) { }
            persistRecvdListFilter();
            if (pendingId) {
                $timeout(function () {
                    var el = document.getElementById('recvd-row-' + pendingId);
                    if (el && typeof el.scrollIntoView === 'function') {
                        el.scrollIntoView({ block: 'center', behavior: 'smooth' });
                    }
                }, 150);
            }
        }

        $scope.comp = { EwayBillEnabled: false };
        $scope.enableRecvdListPreview = true;
        $scope.promptHeaderOriginalDuplicate = false;

        var recChConfig = new $.Config({});
        recChConfig.GetByCategory(function (e) {
            var response = e.data;
            if (response && response.Data && response.Data.length > 0) {
                var cfg = response.Data.find(function (o) {
                    return o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'enableRecvdListPreview';
                });
                if (cfg) {
                    var v = cfg.Value;
                    $scope.enableRecvdListPreview = (v == '1' || v == 1 || v === true || ('' + v).toLowerCase() === 'true');
                }
                var ph = response.Data.find(function (o) {
                    return o.SubCategory == 'RECEIVINGCHALLAN' && o.Category == 'RECEIVINGCHALLAN' && o.Key == 'promptHeaderOriginalDuplicate';
                });
                if (ph) {
                    var pv = ph.Value;
                    $scope.promptHeaderOriginalDuplicate = (pv == '1' || pv == 1 || pv === true || ('' + pv).toLowerCase() === 'true');
                }
            }
        }, 'RECEIVINGCHALLAN');
        var loginData = authService.getTokenInfo();
        function getInfo() {

            var company = new $.Company();
            company.Props.CompanyId = loginData.DefaultCompanyId;
            company.GetDetails(function (e) {
                if (e.status != 200) {
                    return;
                }
                if (!e.data) {
                    return;
                }
                $scope.comp = e.data;

            });
        }
        getInfo();

        if (token) {
            $scope.Billing.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        $scope.PAGE_SIZE = 20;
        $scope.CurrentPage = 1;
        $scope.TotalItems = 0;
        if (savedRecvdFilter) {
            if (savedRecvdFilter.From) {
                $scope.Billing.From = savedRecvdFilter.From;
            }
            if (savedRecvdFilter.To) {
                $scope.Billing.To = savedRecvdFilter.To;
            }
            if (savedRecvdFilter.LedgerId !== undefined && savedRecvdFilter.LedgerId !== null) {
                $scope.Billing.LedgerId = savedRecvdFilter.LedgerId;
            }
            if (savedRecvdFilter.LedgerSiteId !== undefined && savedRecvdFilter.LedgerSiteId !== null) {
                $scope.Billing.LedgerSiteId = savedRecvdFilter.LedgerSiteId;
            }
            if (savedRecvdFilter.ChallanNo !== undefined) {
                $scope.Billing.ChallanNo = savedRecvdFilter.ChallanNo;
            }
            if (savedRecvdFilter.ReceivedListStatusFilter) {
                $scope.Billing.ReceivedListStatusFilter = savedRecvdFilter.ReceivedListStatusFilter;
            }
            if (savedRecvdFilter.ChallanType !== undefined && savedRecvdFilter.ChallanType !== null) {
                $scope.ChallanType = savedRecvdFilter.ChallanType;
            }
            if (savedRecvdFilter.CurrentPage > 0) {
                $scope.CurrentPage = savedRecvdFilter.CurrentPage;
            }
        }
        $scope.ItemsIssued = function (page) {
            if (page == null || page === undefined) {
                $scope.CurrentPage = 1;
            } else {
                $scope.CurrentPage = page;
            }
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            billing.Print = false;
            billing.ChallanType = $scope.ChallanType;
            billing.PageIndex = $scope.CurrentPage;
            billing.PageSize = $scope.PAGE_SIZE;
            wOrder.ItemsReceived(billing, function (e) {
                var res = e.data;
                if (res && res.Data !== undefined) {
                    $scope.ReceivedItems = res.Data;
                    $scope.TotalItems = res.TotalCount || res.Data.length;
                } else {
                    $scope.ReceivedItems = res || [];
                    $scope.TotalItems = $scope.ReceivedItems.length;
                }
                afterRecvdListLoaded();
            });
        };
        $scope.getRecvdChallanTypeLabel = function (challanType) {
            if (challanType == 2) return 'Rent Challan';
            if (challanType == 12) return 'Transfer Challan';
            if (challanType == 11) return 'Contract Challan';
            if (challanType == 10) return 'Hire Challan';
            return '-';
        };
        $scope.onPageChange = function (newPage) {
            $scope.ItemsIssued(newPage);
        };

        $scope.PrintReceivedRegisterReport = function () {
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            billing.ChallanType = $scope.ChallanType;
            billing.PageIndex = 0;
            billing.PageSize = 0;
            var report = new $.Reports();
            report.receivedChallansRegisterReport(function (e) {
                if (e && e.data) {
                    FileSaver.saveAs(e.data, 'received-challans-register.pdf');
                }
            }, billing);
        };

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if (($scope.Billing.LedgerId == null || $scope.Billing.LedgerId === 0) && e.data && e.data.length) {
                $scope.Billing.LedgerId = e.data[0].LedgerId;
            }
        });
        //function receivingSlipHeaderArg() {
        //    var h = $scope.printHeaderType;
        //    if (h === 'original' || h === 'duplicate') {
        //        return h;
        //    }
        //    return null;
        //}
        $scope.Print = function (o) {
            var data = [{ GRNId: o.GRNId }];
            WorkOrderFactory.PrintItemReceivingSlip(data, null);
            //$scope.SelectedReceivedItem = { GRNId: o.GRNId };
            //$scope.printReceivedChallanPdf();
        }

        $scope.ReceivedPreview = null;
        $scope.SelectedReceivedItem = null;

        $scope.previewReceivedChallan = function (item) {
            if (!$scope.enableRecvdListPreview) {
                return;
            }
            $scope.SelectedReceivedItem = item;
            $scope.ReceivedPreview = 1;
            $('#previewDialog').modal('show');
            var strInput = 'receivedchallan,' + item.GRNId;
            var encrypedText = $crypto.encrypt(strInput);
            ReportService.loadPreviewFromReportServer(function (e) {
                $scope.ReceivedPreview = null;
                $('#rpt').html(e.data);
            }, encrypedText);
        };

        $scope.printReceivedChallanPdf = function () {
            var item = $scope.SelectedReceivedItem;
            if (!item) {
                return;
            }
            var strInput = 'receivedchallan,' + item.GRNId + ',' + $scope.printHeaderType;
            var encrypedText = $crypto.encrypt(strInput);
            var safeClient = (item.Client || 'receipt').replace(/[^\w\- ]/g, '').trim() || 'receipt';
            var safeNo = (item.GRN || item.GRNId).toString().replace(/[^\w\-]/g, '_');

            ReportService.printFromReportServer(encrypedText, safeClient + '-' + safeNo + '.pdf');
        };

        $scope.edit = function (o) {
            $('#previewDialog').modal('hide');
            setTimeout(() => {
                persistRecvdListWithScrollTarget(o.GRNId);
                var grnId = $crypto.encrypt(o.GRNId.toString());
                if ($scope.ChallanType == 2 || $scope.ChallanType == 11) {
                    $state.go('editgrn', { GRNId: grnId });
                }

                else if ($scope.ChallanType == 10) {
                    $state.go('edithirechallan', { GRNId: grnId });
                }
            }, 200);
        }
        $scope.PrintSelected = function () {

            var printArray = $.grep($scope.ReceivedItems, function (n, i) {

                return n.Print == true;
            });
            var ids = $.map(printArray, function (n, i) {
                return { GRNId: n.GRNId };
            });
            if (ids.length > 0) {
                WorkOrderFactory.PrintItemReceivingSlip(ids, null, receivingSlipHeaderArg());
            }
        }
        $scope.ShowItems = function (grnId) {
            ModalFactory.ShowGRNItems(grnId, modalController);
        };
        function modalController($scope, $mdDialog, grnId) {
            $scope.closeDialog = function () {
                $mdDialog.hide();
            };
            WorkOrderFactory.GetItemsByGrnId(grnId, function (e) {
                $scope.DialogData = e.data;
            });
        }
        if ($rootScope.LedgerId && !savedRecvdFilter) {
            $scope.Billing.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Billing.LedgerId', function () {
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
        });
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Billing.LedgerId });
        }
        $scope.LoadItems = function (index) {

            var grn = new $.GRN({});
            grn.GRNId = $scope.ReceivedItems[index].GRNId;

            grn.GetItemsByGRNId(function (e) {
                $scope.ReceivedItems[index].Items = e.data;
            });

        }

        $scope.SendEmail = function (grnId) {
            WorkOrderFactory.EmailReceivedReceipt(grnId, function (e) {
            });
        }
        $scope.SendSMS = function (grnId) {
            WorkOrderFactory.SMSReceivedReceipt(grnId, function (e) {
            });
        }
        $scope.deleteChallan = function (grn) {

            $scope.Message = 'Are you sure to delete this challan record';
            $scope.GrnToDel = grn;
            ModalFactory.ConfirmDelete($scope, function () {

                var grn = new $.GRN({});
                grn.DeleteChallan(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    ModalFactory.hideDialog();
                    alert('Challan deleted successfully');
                    $scope.ItemsIssued();
                }, $scope.GrnToDel);

            });
        }
        $scope.createEwayBill = function (item) {
            persistRecvdListFilter();
            var k = '0,chl,' + item.GRNId + ',ret';
            var ec = $crypto.encrypt(k);

            $state.go('addEditEwayBill', { key: ec });

        }
        $scope.inwardConfirm = function (item) {
            if (!item || !item.GRNId) {
                return;
            }
            if (!confirm('Mark this challan as inward confirmed?')) {
                return;
            }
            var grn = new $.GRN({});
            grn.InwardConfirm(function (e) {
                if (!e || !e.data || e.data.Code != 200) {
                    alert((e && e.data && e.data.Message) ? e.data.Message : 'Could not update inward confirm status');
                    return;
                }
                $scope.ItemsIssued($scope.CurrentPage);
            }, { GRNId: item.GRNId });
        }

        $scope.goNewReceivedChallan = function ($event) {
            if ($event) {
                $event.preventDefault();
            }
            persistRecvdListFilter();
            $state.go('inv_grn');
        }

        if (savedRecvdFilter) {
            $scope.ItemsIssued($scope.CurrentPage > 0 ? $scope.CurrentPage : 1);
        }
    }]);