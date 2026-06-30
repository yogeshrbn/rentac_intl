app.controller('ItemsIssuedRegister', ['$scope', '$stateParams', '$state', '$rootScope', '$http', '$location', '$window', '$mdDialog', 'LedgerFactory',
    'WorkOrderFactory', '$crypto', 'AuthenticationService', 'ModalFactory', '$timeout', 'ReportService', 'FileSaver'
    , function ($scope, $stateParams, $state, $rootScope, $http, $location, $window, $mdDialog,
        LedgerFactory, WorkOrderFactory, $crypto, authService, ModalFactory, $timeout, ReportService, FileSaver) {
        $scope.ChallanType = $state.current.data.challanType;
        var wOrder = new $.WorkOrder({});
        $scope.Billing = new $.Billing({});
        $scope.Billing.IssuedListStatusFilter = 'All';
        var date = new Date();
        var token = $rootScope.getTokenInfo();

        var ISSUED_LIST_FILTER_PREFIX = 'rentacIssuedListFilter_v1_';

        function issuedListStorageKey() {
            return ISSUED_LIST_FILTER_PREFIX + ($state.current.name || 'issuedList');
        }

        function persistIssuedListFilter() {
            try {
                var payload = {
                    LedgerId: $scope.Billing.LedgerId,
                    LedgerSiteId: $scope.Billing.LedgerSiteId,
                    From: $scope.Billing.From,
                    To: $scope.Billing.To,
                    ChallanNo: $scope.Billing.ChallanNo,
                    IssuedListStatusFilter: $scope.Billing.IssuedListStatusFilter,
                    ChallanType: $scope.ChallanType,
                    CurrentPage: $scope.CurrentPage
                };
                sessionStorage.setItem(issuedListStorageKey(), JSON.stringify(payload));
            } catch (ex) { }
        }

        function persistIssuedListWithScrollTarget(workOrderId) {
            try {
                var payload = {
                    LedgerId: $scope.Billing.LedgerId,
                    LedgerSiteId: $scope.Billing.LedgerSiteId,
                    From: $scope.Billing.From,
                    To: $scope.Billing.To,
                    ChallanNo: $scope.Billing.ChallanNo,
                    IssuedListStatusFilter: $scope.Billing.IssuedListStatusFilter,
                    ChallanType: $scope.ChallanType,
                    CurrentPage: $scope.CurrentPage,
                    pendingScrollToWorkOrderId: workOrderId
                };
                sessionStorage.setItem(issuedListStorageKey(), JSON.stringify(payload));
            } catch (ex) { }
        }

        function restoreIssuedListFilter() {
            try {
                var raw = sessionStorage.getItem(issuedListStorageKey());
                return raw ? JSON.parse(raw) : null;
            } catch (ex) {
                return null;
            }
        }

        var savedIssuedFilter = restoreIssuedListFilter();

        function afterIssuedListLoaded() {
            var pendingId = null;
            try {
                var st = restoreIssuedListFilter();
                if (st && st.pendingScrollToWorkOrderId) {
                    pendingId = st.pendingScrollToWorkOrderId;
                }
            } catch (ex) { }
            persistIssuedListFilter();
            if (pendingId) {
                $timeout(function () {
                    var el = document.getElementById('issued-row-' + pendingId);
                    if (el && typeof el.scrollIntoView === 'function') {
                        el.scrollIntoView({ block: 'center', behavior: 'smooth' });
                    }
                }, 150);
            }
        }

        $scope.comp = { EwayBillEnabled: false };
        $scope.enableIssuedListPreview = true;
        $scope.promptHeaderOriginalDuplicate = false;
        var delChIssueConfig = new $.Config({});
        delChIssueConfig.GetByCategory(function (e) {
            var response = e.data;
            if (response && response.Data && response.Data.length > 0) {
                var cfg = response.Data.find(function (o) {
                    return o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'enableIssuedListPreview';
                });
                if (cfg) {
                    var v = cfg.Value;
                    $scope.enableIssuedListPreview = (v == '1' || v == 1 || v === true || ('' + v).toLowerCase() === 'true');
                }
                var ph = response.Data.find(function (o) {
                    return o.SubCategory == 'ISSUECHALLAN' && o.Category == 'ISSUECHALLAN' && o.Key == 'promptHeaderOriginalDuplicate';
                });
                if (ph) {
                    var pv = ph.Value;
                    $scope.promptHeaderOriginalDuplicate = (pv == '1' || pv == 1 || pv === true || ('' + pv).toLowerCase() === 'true');
                }
            }
        }, 'issuechallan');
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


        $scope.Billing.To = convertDate(date);
        if (token) {
            $scope.Billing.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        $scope.PAGE_SIZE = 20;
        $scope.CurrentPage = 1;
        $scope.TotalItems = 0;
        if (savedIssuedFilter) {
            if (savedIssuedFilter.From) {
                $scope.Billing.From = savedIssuedFilter.From;
            }
            if (savedIssuedFilter.To) {
                $scope.Billing.To = savedIssuedFilter.To;
            }
            if (savedIssuedFilter.LedgerId !== undefined && savedIssuedFilter.LedgerId !== null) {
                $scope.Billing.LedgerId = savedIssuedFilter.LedgerId;
            }
            if (savedIssuedFilter.LedgerSiteId !== undefined && savedIssuedFilter.LedgerSiteId !== null) {
                $scope.Billing.LedgerSiteId = savedIssuedFilter.LedgerSiteId;
            }
            if (savedIssuedFilter.ChallanNo !== undefined) {
                $scope.Billing.ChallanNo = savedIssuedFilter.ChallanNo;
            }
            if (savedIssuedFilter.IssuedListStatusFilter) {
                $scope.Billing.IssuedListStatusFilter = savedIssuedFilter.IssuedListStatusFilter;
            }
            if (savedIssuedFilter.ChallanType !== undefined && savedIssuedFilter.ChallanType !== null) {
                $scope.ChallanType = savedIssuedFilter.ChallanType;
            }
            if (savedIssuedFilter.CurrentPage > 0) {
                $scope.CurrentPage = savedIssuedFilter.CurrentPage;
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
            billing.ChallanType = $scope.ChallanType;
            billing.PageIndex = $scope.CurrentPage;
            billing.PageSize = $scope.PAGE_SIZE;
            wOrder.ItemsIssued(billing, function (e) {
                var res = e.data;
                if (res && res.Data !== undefined) {
                    $scope.IssuedItems = res.Data;
                    $scope.TotalItems = res.TotalCount || res.Data.length;
                } else {
                    $scope.IssuedItems = res || [];
                    $scope.TotalItems = $scope.IssuedItems.length;
                }
                afterIssuedListLoaded();
            });
        };
        $scope.getIssuedChallanTypeLabel = function (challanType) {
            if (challanType == 2) return 'Rent Delivery Challan';
            if (challanType == 12) return 'Transfer Challan';
            if (challanType == 1) return 'Contract Delivery Challan';
            if (challanType == 10) return 'Un-Hire Delivery Challan';
            return '-';
        };
        $scope.onPageChange = function (newPage) {
            $scope.ItemsIssued(newPage);
        };
        $scope.LoadItems = function (index) {
            wOrder.WorkOrderId = $scope.IssuedItems[index].WorkOrderId;
            wOrder.GetItems(function (e) {

                $scope.IssuedItems[index].Items = e.data;
            });

        }
        $scope.ExpandAll = function () {
            if (!$scope.IssuedItems || $scope.IssuedItems.length === 0) return;
            for (var i = 0; i < $scope.IssuedItems.length; i++) {
                $scope.LoadItems(i);
            }
            $timeout(function () {
                angular.element('.row-detail-collapse').addClass('in');
            }, 50);
        }
        $scope.CollapseAll = function () {
            angular.element('.row-detail-collapse').removeClass('in');
        }

        var ledger = new $.Ledger({});
        ledger.GetAll(function (e) {
            $scope.Accounts = e.data;
            if (($scope.Billing.LedgerId == null || $scope.Billing.LedgerId === 0) && e.data && e.data.length) {
                $scope.Billing.LedgerId = e.data[0].LedgerId;
            }
        });
        if ($rootScope.LedgerId && !savedIssuedFilter) {
            $scope.Billing.LedgerId = $rootScope.LedgerId;
        }
        $scope.$watch('Billing.LedgerId', function () {
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
        });
        $scope.ChangePartyModel = { LedgerId: 0, LedgerSiteId: 0, Sites: [] };

        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
                $scope.ChangePartySites = e.data.Data;
            }, { LedgerId: $scope.Billing.LedgerId });
        }
        $scope.PrintReceipt = function (item) {
            $scope.SelectedIssueItem = item;
            $scope.printIssueChallanPdf();
        }
        function Print(data) {
            wOrder.PrintIssuedReceipt(function (e) {
                var filePath = SERVER_RPT_URL + 'temp/' + e.data;
                $window.open(filePath);
            }, data);
        }
        $scope.PrintSelected = function () {
            var printArray = $.grep($scope.IssuedItems, function (n, i) {
                return n.Print == true;
            });
            var ids = $.map(printArray, function (n, i) {
                return { WorkOrderId: n.WorkOrderId };
            });
            if (ids.length > 0) {
                Print(ids);
            }
        }

        $scope.PrintIssuedRegisterReport = function () {
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            billing.ChallanType = $scope.ChallanType;
            billing.PageIndex = 0;
            billing.PageSize = 0;
            var report = new $.Reports();
            report.issuedChallansRegisterReport(function (e) {
                if (e && e.data) {
                    FileSaver.saveAs(e.data, 'issued-challans-register.pdf');
                }
            }, billing);
        };

        $scope.$watch('ChangePartyModel.LedgerId', function () {
            if ($scope.ChangePartyModel.LedgerId == 0) return;
            LedgerFactory.GetMasterSites(function (e) {
                $scope.ChangePartySites = e.data.Data;
            }, { LedgerId: $scope.ChangePartyModel.LedgerId });
        });


        $scope.changePartyDialog = function () {
            var printArray = $.grep($scope.IssuedItems, function (n, i) {
                return n.Print == true;
            });
            var ids = $.map(printArray, function (n, i) {
                return { WorkOrderId: n.WorkOrderId };
            });
            if (ids.length == 0) {
                alert('Please select a maximum of 10 challans');
                return;
            }
            if (ids.length > 10) {
                alert('Maximum 10 challans can be selected');
                return;
            }
            $('#dlgChangeParty').modal('show');

        }
        $scope.changeParty = function () {
            var printArray = $.grep($scope.IssuedItems, function (n, i) {
                return n.Print == true;
            });
            var ids = $.map(printArray, function (n, i) {
                return { WorkOrderId: n.WorkOrderId };
            });
            if (ids.length == 0) {
                alert('Please select a maximum of 10 challans');
                return;
            }
            if (ids.length > 10) {
                alert('Maximum 10 challans can be selected');
                return;
            }


        }
        $scope.SendEmail = function (workOrderId) {
            WorkOrderFactory.EmailIssueChallan(workOrderId, function (e) {
            });
        }
        $scope.SendSMS = function (workOrderId) {
            WorkOrderFactory.SMSIssueChallan(workOrderId, function (e) {
            });
        }

        $scope.BrowseFile = function (file, image, item) {
            $('#' + file).click();
            $('#newfile').bind('change', function () {
                readURL(this, function (e) {

                    uploadDocument(item.WorkOrderId, file);
                });
                $('#newfile').unbind('change');
            });

        }
        $scope.ChallanDocs = [{ ChallanDocumentId: 0, FilePath: '../img/add.png' }, { ChallanDocumentId: 0, FilePath: '../img/add.png' }];
        $scope.GetChallanDocuments = function (item) {

            if (item.WorkOrderId > 0) {
                WorkOrderFactory.GetChallanDocuments(function (e) {
                    $.map(e.data.Data, function (n, i) {
                        n.FilePath = SERVER_RPT_URL + n.FilePath;
                        // $('#img'+ i).css('padding','0px');
                        // $('#img0').css('padding', '0px');
                    });

                    item.ChallanDocs = e.data.Data;
                }, { WorkOrderId: item.WorkOrderId });
            }
        }
        //$scope.BrowseFile = function (file, image, workOrderId) {

        //    $('#' + file).click();
        //    $('#' + file).bind('change', function () {
        //        readURL(this, image, function (e) {
        //            uploadDocument(workOrderId, file);
        //        });

        //    });
        //}
        function readURL(input, success) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    // $('#' + image).attr('src', e.target.result);
                    //$('#' + image).css('padding','0px');
                    success.call();
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        $scope.RemoveFile = function (wOrderItem, file, image, documentId) {

            var conf = confirm('Are you sure to remove this file ?');

            if (!conf)
                return;

            // $('#' + image).attr('src', '../img/add.png');
            //  $('#' + image).css('padding','30px');
            // $('#' + image).removeClass('nopadding');
            // $('#' + file).value('');
            if (documentId > 0) {
                // var wOrder = $scope.IssuedItems.find(o => o.WorkOrderId == workOrderId);
                WorkOrderFactory.DeleteChallanDocument(function (e) {
                    if (e.data.Code == 200) {
                        $scope.GetChallanDocuments(wOrderItem);
                    }
                    //   $('#liDocs').click();
                }, { ChallanDocumentId: documentId });
            }
        }
        $scope.IssuePreview = null;
        $scope.SelectedIssueItem = null;
        $scope.printHeaderType = 'none';
        $scope.previewIssueChallan = function (item) {
            if (!$scope.enableIssuedListPreview) {
                return;
            }
            $scope.SelectedIssueItem = item;
            $scope.IssuePreview = 1;
            $('#previewDialog').modal('show');
            var strInput = 'issuechallan,' + item.WorkOrderId;
            var encrypedText = $crypto.encrypt(strInput);
            ReportService.loadPreviewFromReportServer(function (e) {
                $scope.IssuePreview = null;
                $('#rpt').html(e.data);
            }, encrypedText);
        };
        $scope.printIssueChallanPdf = function () {
            var item = $scope.SelectedIssueItem;
            if (!item) {
                return;
            }

            var safeClient = (item.Client || 'challan').replace(/[^\w\- ]/g, '').trim() || 'challan';
            var safeNo = (item.ChallanNumber || item.WorkOrderId).toString().replace(/[^\w\-]/g, '_');

            var strInput = 'issuechallan,' + item.WorkOrderId + ',' + $scope.printHeaderType;
            var encrypedText = $crypto.encrypt(strInput);
            ReportService.printFromReportServer(encrypedText, safeClient + '-' + safeNo + '.pdf');
        };
        $scope.edit = function (worder) {
            $('#previewDialog').modal('hide');
            setTimeout(() => {
                persistIssuedListWithScrollTarget(worder.WorkOrderId);

                //  $scope.params = worder;
                var worderId = $crypto.encrypt(worder.WorkOrderId.toString());
                if (worder.ChallanType == 2 || worder.ChallanType == 1) {
                    $state.go('editIsCh', { WorkOrderId: worderId });
                }
                if (worder.ChallanType == 10) {
                    $state.go('editunhirechallan', { WorkOrderId: worderId });
                }
                if (worder.ChallanType == 12) {
                    $state.go('editMatTransfer', { WorkOrderId: worderId });
                }
            }, 200);
        }

        $scope.delete = function (worder) {



            $scope.Message = 'Are you sure to delete this challan record';

            ModalFactory.ConfirmDelete($scope, function () {

                var wd = new $.WorkOrder();
                wd.DeleteChallan(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    ModalFactory.hideDialog();
                    $scope.ItemsIssued();

                }, worder);

            });

            //  $scope.params = worder;

        }

        function uploadDocument(workOrderId, fileId) {
            if (workOrderId == 0) {
                return;
            }
            var fileList = [];
            if ($('#' + fileId)[0].files.length > 0) {
                fileList.push($('#' + fileId)[0].files[0]);
            }

            WorkOrderFactory.AddChallanDocument({ WorkOrderId: workOrderId }, fileList, function (e) {
                if (e.data.Code == 200) {
                    $scope.ChallanDocs = [];

                    var wOrder = $scope.IssuedItems.find(o => o.WorkOrderId == workOrderId);
                    $scope.GetChallanDocuments(wOrder);

                }
            });
        }

        $scope.createEwayBill = function (item) {
            persistIssuedListFilter();
            var k = '0,chl,' + item.WorkOrderId + ',del';
            var ec = $crypto.encrypt(k);

            $state.go('addEditEwayBill', { key: ec });

        }
        $scope.markDispatched = function (item) {
            if (!item || !item.WorkOrderId) {
                return;
            }
            if (!confirm('Mark this challan as dispatched?')) {
                return;
            }
            var wd = new $.WorkOrder();
            wd.UpdateStatus(function (e) {
                if (!e || !e.data || e.data.Code != 200) {
                    alert((e && e.data && e.data.Message) ? e.data.Message : 'Could not update dispatched status');
                    return;
                }
                $scope.ItemsIssued($scope.CurrentPage);
            }, { WorkOrderId: item.WorkOrderId });
        }

        $scope.goIssueChallan = function ($event) {
            if ($event) {
                $event.preventDefault();
            }
            persistIssuedListFilter();
            if ($scope.ChallanType == 12) {
                $state.go('matTransfer');
                return;
            }
            $state.go('issueChallan');
        }

        if (savedIssuedFilter) {
            $scope.ItemsIssued($scope.CurrentPage > 0 ? $scope.CurrentPage : 1);
        }
        $scope.showEmailDialog = function (item) {
            var div = '<div style="width:90%;height:700px"></div>';
            var _d = new Date().getMilliseconds();
            $(div).load('templ/dialogs/sendemail-dialog.html?d=' + _d, function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: false,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        emailMessage: {
                            Receipients: '', Subject: 'Delivery Challan:' + item.ChallanNumber
                            , MetaData: "1103," + item.WorkOrderId + "," + item.ChallanNumber + ".pdf"
                        }
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: 'ShareController'
                });
            });
        }
    }]);