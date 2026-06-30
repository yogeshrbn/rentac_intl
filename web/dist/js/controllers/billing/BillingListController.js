app.controller('BillingListController', ['$scope', '$location', '$stateParams', '$state', '$mdDialog', '$rootScope', '$window', '$timeout',
    'ModalFactory', 'LedgerFactory', 'AuthenticationService', '$crypto', 'ReportService', 'FileSaver',
    function ($scope, $location, $stateParams, $state, $mdDialog, $rootScope, $window, $timeout, ModalFactory,
        LedgerFactory, authService, $crypto, ReportService, FileSaver) {

        $scope.Billing = new $.Billing({});
        $scope.Accounts = [];
        $scope.LedgerSites = [];
        var date = new Date();
        var token = $rootScope.getTokenInfo();

        $scope.Token = token;
        $scope.Billing.To = convertDate(date);
        $scope.wsApp = token.wsApp
        if (token) {
            $scope.Billing.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Billing.LedgerId = $rootScope.LedgerId;
        }
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Billing.LedgerId = item.LedgerId;
            }
        };
        $scope.$watch('Billing.LedgerId', function () {
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
        });
        $scope.PAGE_SIZE = 100;
        $scope.CurrentPage = 1;
        $scope.TotalItems = 0;
        $scope.GetBills = function (page) {
            if (page !== undefined) {
                $scope.CurrentPage = page;
            } else {
                $scope.CurrentPage = 1;
            }
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            billing.PageIndex = $scope.CurrentPage;
            billing.PageSize = $scope.PAGE_SIZE;
            $scope.Billing.GetBillList(function (e) {
                var res = e.data;
                if (res && res.Data !== undefined) {
                    $scope.Bills = res.Data;
                    $scope.TotalItems = res.TotalCount || res.Data.length;
                } else {
                    $scope.Bills = res || [];
                    $scope.TotalItems = $scope.Bills.length;
                }
                if ($scope.billListGridApi) {
                    $scope.billListGridApi.setGridOption('rowData', $scope.Bills);
                    if (typeof $scope.billListGridApi.resetRowHeights === 'function') {
                        $scope.billListGridApi.resetRowHeights();
                    }
                }
            }, billing);
        };
        $scope.onPageChange = function (newPage) {
            $scope.GetBills(newPage);
        };
        $scope.printBillListGstReport = function () {
            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            var payload = {
                From: billing.From,
                To: billing.To,
                LedgerId: billing.LedgerId ? parseInt(billing.LedgerId, 10) : 0,
                LedgerSiteId: billing.LedgerSiteId ? parseInt(billing.LedgerSiteId, 10) : 0,
                StatusId: billing.StatusId !== undefined && billing.StatusId !== null ? parseInt(billing.StatusId, 10) : 0,
                InvoiceType: billing.InvoiceType !== undefined && billing.InvoiceType !== null ? parseInt(billing.InvoiceType, 10) : 0,
                InvoiceNumber: (billing.InvoiceNumber || '').trim()
            };
            debugger
            var encrypedText = $crypto.encrypt(JSON.stringify(payload));
            var encoded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReportFromHtmlPost(function (e) {
                FileSaver.saveAs(e.data, 'BillList-GST.pdf');
            }, 'PrintBillListGstReport', { Payload: encoded });
        };
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Billing.LedgerId });
        }
        LedgerFactory.GetAllParties(function (e) {
            var data = (e.data && Array.isArray(e.data)) ? e.data : (e.data && e.data.Data) ? e.data.Data : (e.data && e.data.items) ? e.data.items : [];
            $scope.Accounts = data;
            if ($scope.Accounts) {
                $scope.Accounts.push({ LedgerId: 0, Code: '', Name: 'All Clients' });
            }
            if (data != null && data.length > 0) {
                $scope.initialValue = data[0];
            }
            if ($scope.Billing.LedgerId == null && data.length > 0) {
                $scope.Billing.LedgerId = data[0].LedgerId;
            }
        });
        var ledger = new $.Ledger({});

        function fmtDate(d) {
            if (!d) return '';
            var x = new Date(d);
            if (isNaN(x.getTime())) return String(d);
            var day = ('0' + x.getDate()).slice(-2);
            var month = ('0' + (x.getMonth() + 1)).slice(-2);
            return day + '-' + month + '-' + x.getFullYear();
        }
        /** Site line for bill list: JSON may use PascalCase or camelCase; fall back to Site. */
        function billListSiteLine(d) {
            if (!d) return '';
            function s(x) {
                if (x == null || x === '') return '';
                var t = String(x).trim();
                return t;
            }
            var v = s(d.SiteAddress) || s(d.siteAddress) || s(d.Site) || s(d.site);
            if (v) return v;
            for (var k in d) {
                if (!Object.prototype.hasOwnProperty.call(d, k) || d[k] == null) continue;
                if (k.toLowerCase() === 'siteaddress') return s(d[k]);
            }
            return '';
        }
        var invTypeLabels = { 1: 'Rent Invoice', 2: 'Breakage', 4: 'Sale Bill', 5: 'Contract Bill', 6: 'Loss Bill', 7: 'Sale Return', 8: 'Hire Bill', 9: 'Measurement Bill' };
        var billListTheme = agGrid.themeQuartz.withParams({
            borderColor: "#00000054",
            headerHeight: "30px",
            headerTextColor: "#000",
            fontSize: 13,
            headerFontSize: 13,
            wrapperBorderRadius: 0,
            headerColumnBorder: true,
            rowHeight: 40,
            headerBackgroundColor: "#d7d7d7",
            headerCellMovingBackgroundColor: "rgb(80, 40, 140)",
            headerCellHoverBackgroundColor: "#98CCF8",
        });
        $scope.billListGridOptions = {
            theme: billListTheme,
            rowData: [],
            loading: false,
            suppressRowClickSelection: true,
            getRowStyle: function (params) {
                if (params.data.StatusId == 3) return { color: '#5cb85c' };
                if (params.data.StatusId == 2) return { color: '#d9534f' };
                if (params.data.StatusId == 4) return { color: '#337ab7' };
            },
            columnDefs: [
                {
                    headerName: '', field: 'Print', width: 40, cellRenderer: function (params) {
                        var chk = document.createElement('input');
                        chk.type = 'checkbox';
                        chk.checked = params.data.Print === true;
                        chk.onchange = function () { params.data.Print = chk.checked; };
                        var wrap = document.createElement('div');
                        wrap.style.textAlign = 'center';
                        wrap.appendChild(chk);
                        return wrap;
                    }, cellStyle: { textAlign: 'center' }
                },
                { headerName: '#', width: 50, valueGetter: function (p) { var idx = (p.node && p.node.rowIndex != null) ? p.node.rowIndex : 0; return (($scope.CurrentPage - 1) * $scope.PAGE_SIZE) + idx + 1; }, cellStyle: { textAlign: 'center' } },
                {
                    headerName: 'Bill#', field: 'InvoiceNumber', width: 150, cellRenderer: function (params) {
                        var span = document.createElement('span');
                        span.className = params.data.IrnDetails ? 'label label-success' : '';
                        span.style.cursor = 'pointer';
                        span.textContent = params.data.InvoiceNumber || '';
                        span.onclick = function () { $scope.$apply(function () { $scope.showOptions(params.data); }); };
                        return span;
                    }
                },
                {
                    headerName: 'Client',
                    field: 'Client',
                    width: 300,
                    wrapText: true,
                    cellStyle: {
                        textAlign: 'left',
                        whiteSpace: 'normal',
                        lineHeight: '1.35',
                        wordBreak: 'break-word'
                    },
                    cellRenderer: function (params) {
                        var d = params.data;
                        if (!d) return '';
                        var div = document.createElement('div');
                        div.className = 'text-left';
                        div.style.whiteSpace = 'normal';
                        div.style.overflow = 'visible';
                        var c = (d.Client || d.client || '').trim();
                        var line = document.createElement('div');
                        line.textContent = c;
                        div.appendChild(line);
                        var site = billListSiteLine(d);
                        if (site) {
                            var spanSite = document.createElement('div');
                            spanSite.style.fontSize = '11px';
                            spanSite.style.marginTop = '3px';
                            spanSite.style.color = '#444';
                            spanSite.style.whiteSpace = 'normal';
                            spanSite.textContent = site;
                            div.appendChild(spanSite);
                        }
                        return div;
                    }
                },
                { headerName: 'B. Date', field: 'InvoiceDate', width: 100, valueGetter: function (p) { return fmtDate(p.data && p.data.InvoiceDate); } },
                { headerName: 'Period', width: 190, valueGetter: function (p) { var d = p.data; return (fmtDate(d && d.From) || '') + ' To ' + (fmtDate(d && d.To) || ''); }, cellStyle: { textAlign: 'center' } },
                { headerName: 'Inv Type', field: 'InvoiceType', width: 120, valueGetter: function (p) { return invTypeLabels[p.data && p.data.InvoiceType] || ''; }, cellStyle: { textAlign: 'right' } },
                //{
                //    headerName: 'Status', width: 70, cellRenderer: function (params) {
                //        var span = document.createElement('span');
                //        span.className = params.data.Balance == 0 ? 'badge badge-success' : 'badge badge-warning';
                //        span.textContent = params.data.Balance == 0 ? 'Paid' : 'Due';
                //        return span;
                //    }, cellStyle: { textAlign: 'center' }
                //},
                { headerName: 'Amount', field: 'Total', width: 100, pinned: 'right', valueFormatter: function (p) { return p.value != null ? Number(p.value).toFixed(2) : ''; }, cellStyle: { textAlign: 'right' } },
                { headerName: 'Due', field: 'Balance', width: 100, pinned: 'right', valueFormatter: function (p) { return p.value != null ? Number(p.value).toFixed(2) : ''; }, cellStyle: { textAlign: 'right' } },
                {
                    headerName: 'Actions', width: 90, pinned: 'right', cellRenderer: function (params) {
                        var div = document.createElement('div');
                        div.className = 'billlist-actions btn-group';
                        var opts = document.createElement('a');

                        opts.href = 'javascript:void(0)';
                        opts.innerHTML = '<i class="fa fa-cogs fa-2x"></i>';
                        opts.onclick = function () { $scope.$apply(function () { $scope.showOptions(params.data); }); };
                        var email = document.createElement('a');
                        email.href = 'javascript:void(0)';
                        email.innerHTML = '<i class="fa fa-share-alt fa-2x"></i>';
                        email.onclick = function () { $scope.$apply(function () { $scope.showEmailDialog(params.data); }); };
                        div.appendChild(opts);
                        div.appendChild(email);
                        return div;
                    }, cellStyle: { textAlign: 'center' }
                }
            ],
            defaultColDef: { resizable: true, sortable: true },
        };
        $timeout(function () {
            var el = document.querySelector('#billListGrid');
            if (el) {
                $scope.billListGridApi = agGrid.createGrid(el, $scope.billListGridOptions);
            }
        }, 100);

        $('#billOptionsModal').off('hidden.bs.modal.billList').on('hidden.bs.modal.billList', function () {
            $('#billPreviewRpt').empty();
            if (!$scope.$$phase) {
                $scope.$apply(function () { $scope.BillListPreview = null; });
            } else {
                $scope.BillListPreview = null;
            }
        });

        $scope.CancellBill = function () {
            var item = $scope.SelectedBill;
            if (item.StatusId == 1 || item.StatusId == 2) {
                alert('Only draft or sent for approval invoices can be cancelled');
                return;
            }

            var tr = 'tr' + item.InvoiceId;
            var confirm = window.confirm('Cancelled bill will not be revoked.\nAre you sure to cancel this bill?');
            if (!confirm) return;
            $scope.Billing.InvoiceId = item.InvoiceId;
            $(event.target).hide(); // hide cancelled button
            $scope.Billing.CancelBill(function (e) {
                $scope.GetBills();
                //if (e.data.Data == true) {
                //    $('#' + tr).addClass('cancelled'); // add cancelled class to show the row in red.
                //}
            });
        }
        //$scope.MarkSettle = function (index) {
        //    var tr = 'tr' + index.InvoiceId;
        //    var confirm = window.confirm('Settled bill will not be revoked.\nAre you sure to settle this bill?');
        //    if (!confirm) return;
        //    $scope.Billing.InvoiceId = index.InvoiceId;
        //    $(event.target).hide(); // hide cancelled button
        //    $scope.Billing.MarkSettle(function (e) {
        //        if (e.data == true) {
        //            $('#' + tr).addClass('settled'); // add cancelled class to show the row in red.
        //        }
        //    });
        //}
        $scope.SendReminder = function (item, type) {
            item.ReminderType = type;
            LedgerFactory.DueBillReminder(function (e) {

            }, item);

        }

        $scope.BillingItemsTax = function (index) {

            loadTaxes(index.InvoiceId);
        };
        function loadTaxes(invoiceId) {
            $scope.Billing.InvoiceId = invoiceId;
            $scope.Billing.BillingItemsTax(function (e) {
                $scope.AppliedTaxes = e.data;
                var div = '<div style="width:90%;height:70%"></div>';
                $(div).load('templ/taxItems.html', function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: $scope,
                        preserveScope: true,
                        locals: {
                            AppliedTaxes: $scope.AppliedTaxes
                        },
                        template: html,
                        parent: angular.element(document.body),
                        controller: function (AppliedTaxes) {
                            $scope.closeSliderModal = function () {
                                $mdDialog.hide();
                            }
                        }
                    });
                });
            });
        }
        $scope.ShowBillingItems = function (item) {

            $scope.Billing.InvoiceId = item.InvoiceId;
            // $scope.Billing.GetBillItems(function(e) {

            object = $scope.Billing
            loadBillItems($scope.Billing);
            // });
        }

        function chooseAndPrintBill(invoiceId) {
            ModalFactory.BillType(function ($scope, $mdDialog) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                },
                    $scope.OkButtonClick = function () {
                        var headerTypes = [];

                        if ($scope.HeaderType1) {
                            headerTypes.push(1);
                        }
                        if ($scope.HeaderType2) {
                            headerTypes.push(2);
                        }
                        if ($scope.HeaderType3) {
                            headerTypes.push(3);
                        }

                        $(event.currentTarget).find('.fa-spin').show();
                        loadBillPdf(headerTypes, invoiceId);
                    }
            });
        }

        $scope.printSelected = function () {
            debugger
            if ($scope.Billing.InvoiceType != 1) {
                alert('Multiple bills print feature can work only for rent bills');
                return;
            }
            var printArray = $.grep($scope.Bills, function (n, i) {
                return n.Print == true;
            });
            //    var ids = $.map(printArray,);
            var ids = printArray.map(o => o.InvoiceId);
            if (ids.length == 0) {
                alert('Please select a bill to print');
                return;
            }
            if (ids.length > 10) {
                alert('You can print only 10 bills at a time');
                return;
            }
            var strIds = ids.join('|');

            var strInput = "rentbill," + strIds;
            var encrypedText = $crypto.encrypt(strInput);
            ReportService.printBills(encrypedText).then(function (e) {
                debugger
                FileSaver.saveAs(e.data, 'bills.zip');
            });

        }

        $scope.BillListPreview = null;

        function onBillPreviewHtml(e) {
            var applyPreview = function () {
                $scope.BillListPreview = null;
                $('#billPreviewRpt').html(e.data);
            };
            if (!$scope.$$phase) {
                $scope.$apply(applyPreview);
            } else {
                applyPreview();
            }
        }

        function loadBillPreview(item) {
            debugger
            if (!item || !item.InvoiceId) {
                return;
            }
            $scope.BillListPreview = 1;
            $('#billPreviewRpt').empty();
            var invoiceId = item.InvoiceId;
            var invTypeId = item.InvoiceType;
            var encoded = btoa($crypto.encrypt(String(invoiceId)));
            var report = new $.Reports();

            if (invTypeId == 5) {
                report.previewReportFromHtml(onBillPreviewHtml, 'PreviewContractBill', encoded);
            } else if (invTypeId == 9) {
                report.previewReportFromHtml(onBillPreviewHtml, 'PreviewMeasurementBill', encoded);
            } else if (invTypeId == 4 || invTypeId == 6 || invTypeId == 2) {
                var strInput = 'salebill,' + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);

                ReportService.loadPreviewFromReportServer(onBillPreviewHtml, encrypedText);
            } else {
                report.previewReportFromHtml(onBillPreviewHtml, 'PreviewRentBill', encoded);
            }
        }
        $scope.loadBillPreview = loadBillPreview;

        function downloadBillForItem(item) {
            if (!item || !item.InvoiceId) {
                return;
            }
            var invoiceId = item.InvoiceId;
            var invTypeId = item.InvoiceType;
            var fileName = (item.InvoiceNumber || ('bill_' + invoiceId)) + '.pdf';
            $scope.Billing.InvoiceId = invoiceId;
            var headerTypes = [1];

            if (invTypeId == 5) {
                var econded = btoa($crypto.encrypt(String(invoiceId)));
                var report = new $.Reports();
                report.downloadReportFromHtml(function (e) {
                    FileSaver.saveAs(e.data, fileName);
                }, 'PrintContractBill', econded);
            } else if (invTypeId == 9) {
                var econdedMeas = btoa($crypto.encrypt(String(invoiceId)));
                var reportMeas = new $.Reports();
                reportMeas.downloadReportFromHtml(function (e) {
                    FileSaver.saveAs(e.data, fileName);
                }, 'PrintMeasurementBill', econdedMeas);
            } else if (invTypeId == 4 || invTypeId == 6 || invTypeId == 2) {
                var strInput = 'salebill,' + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);
                ReportService.printFromReportServer(encrypedText, fileName);
            } else {
                loadBillPdf(headerTypes, invoiceId);
            }
        }

        $scope.downloadBillPdf = function () {
            downloadBillForItem($scope.SelectedBill);
        };

        $scope.PrintBill = function () {
            downloadBillForItem($scope.SelectedBill);
        }

        function loadBillPdf(headerType, invoiceId) {

            $scope.Billing.InvoiceId = invoiceId;
            //$scope.Billing.BillCopyType = headerType;
            $scope.Billing.HeaderTypes = headerType;


            var encrypedText = $crypto.encrypt(invoiceId);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReportFromHtml(function (e) {
                FileSaver.saveAs(e.data, $scope.SelectedBill.InvoiceNumber + '.pdf');
            }, 'PrintRentBill', econded);
        }
        function loadBillItems(billing) {

            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/dialogs/billingItemsDialog.html?d=' + new Date().getTime().toString(), function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        billing: billing
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: BillingDialogController
                });
            });
        }
        function BillingDialogController($scope, $mdDialog, $routeParams, $http, billing) {
            $scope.DialogData = null;
            billing.GetBillItems(function (e) {

                $('#loadersite').hide();
                $scope.DialogData = e.data;

            });
            billing.GetBillLossItems(function (e) {
                $scope.LossItems = e.data;
            });
            billing.GetBreakageItems(function (e) {
                $scope.BreakageItems = e.data;
            });
            $scope.closeDialog = function () {
                $mdDialog.hide();
            }
        }

        $scope.comp = { EInvoiceEnabled: false };
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

        $scope.pushToIRP = function () {
            var item = $scope.SelectedBill;
            var billing = new $.Billing();
            billing.PushToIRP(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been pushed to IRN successfully.');

                } else {
                    alert(e.data.Message);
                }

            }, item);
        }

        $scope.createEwayBill = function () {
            var item = $scope.SelectedBill;
            var k = '0,inv,' + item.InvoiceId;
            var ec = $crypto.encrypt(k);

            $('#billOptionsModal').modal('hide');
            setTimeout(() => {
                $state.go('addEditEwayBill', { key: ec });
            }, 200);



        }
        $scope.SelectedBill = null;
        $scope.editBill = function () {

            var item = $scope.SelectedBill;

            //if (item.StatusId == 1 || item.StatusId == 2) {
            //    alert('Only draft or sent for approval invoices can be edited');
            //    return;
            //}
            if (item.EwayBillNo != null || item.IrnDetails != null) {
                alert('Can not edit after e-way bill or e-invoice prepared.');
                return;
            }
            if (item.InvoiceType == 1 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);

                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editBill', { key: key });
                }, 500);


            }
            if (item.InvoiceType == 4 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editsale', { key: key });
                }, 500);
            }
            //if (item.InvoiceType == 5 && (item.StatusId == 3 || item.StatusId == 4)) {
            //    var key = $crypto.encrypt(item.InvoiceId + "," + item.ContractId);
            //    $('#billOptionsModal').modal('hide');
            //    setTimeout(() => {
            //        $state.go('editmeasurebill', { id: key });
            //    }, 500);
            //}
            if (item.InvoiceType == 6 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editlossbill', { key: key });
                }, 500);
            }
            if (item.InvoiceType == 2 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editbreakageBill', { key: key });
                }, 500);
            }
            if ((item.InvoiceType == 9 || item.InvoiceType == 5) && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editmeasurebill', { key: key });
                }, 500);
            }
        }
        $scope.sendForApproval = function () {

            var item = $scope.SelectedBill;

            if (item.StatusId != 3) {
                alert('Only draft bills can be sent for approval');
                return;
            }
            var billing = new $.Billing();
            billing.SendForapproval(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been sent for approval.');
                    $('#billOptionsModal').modal('hide');
                    $scope.GetBills();

                } else {
                    alert(e.data.Message);
                }

            }, { InvoiceId: item.InvoiceId });

        }
        $scope.approveBill = function () {

            if ($scope.Token.RoleId != 1) {
                alert('You are not allowed to approve this bill');
                return;
            }
            var item = $scope.SelectedBill;

            if (item.StatusId != 4) {
                alert('Not ready for approval');
                return;
            }
            var billing = new $.Billing();
            billing.Approve(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been approved.');
                    $('#billOptionsModal').modal('hide');
                    $scope.GetBills();

                } else {
                    alert(e.data.Message);
                }

            }, { InvoiceId: item.InvoiceId });

        }

        $scope.showOptions = function (item) {
            $scope.SelectedBill = item;
            if ($('#billPreviewRpt').length) {
                $('#billPreviewRpt').empty();
            }
            $('#billOptionsModal').modal('show');
            if ($scope.loadBillPreview) {
                $scope.loadBillPreview(item);
            }
        }
        $scope.getClassName = function (item) {
            if (item.StatusId == 3) {
                return 'text-success';
            }
            if (item.StatusId == 2) {
                return 'text-danger';
            }
            if (item.StatusId == 4) {
                return 'text-primary';
            }
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
                            Receipients: item.ClientEmail, Subject: 'Invoice:' + item.InvoiceNumber
                            , MetaData: "1100," + item.InvoiceId + "," + item.InvoiceNumber + ".pdf"
                        }
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: 'NotificationController'
                });
            });
        }

        $scope.SettleBill = function (bill) {

            var settleController = function ($scope) {
                $scope.SettlementRemarks = '';
                $scope.closeDialog = function () {
                    ModalFactory.hideDialog();
                }
                $scope.OkButtonClick = function () {

                    if (!$scope.SettlementRemarks || $scope.SettlementRemarks.length < 10) {
                        alert('Please enter the remarks');
                        return;
                    }
                    var bill = new $.Billing();
                    var model = { InvoiceId: $scope.SelectedBill.InvoiceId, SettlementRemarks: $scope.SettlementRemarks };
                    bill.SettleBill(function (e) {
                        if (e.data.Code != 200) {
                            alert(e.data.Message);

                        }
                        else {
                            alert('Bill settled successfully.');
                            $scope.closeDialog();
                        }
                    }, model);
                }
            }

            ModalFactory.ShowDialog(settleController, 'templ/dialogs/bill.settle.html', null, $scope, $('#billOptionsModal'));

        }
    }]);

app.controller('RecurringInvoicesController', ['$scope', '$location', '$stateParams', '$state', '$mdDialog', '$rootScope', '$window',
    'ModalFactory', 'LedgerFactory', 'AuthenticationService', '$crypto', 'ReportService', 'FileSaver',
    function ($scope, $location, $stateParams, $state, $mdDialog, $rootScope, $window, ModalFactory,
        LedgerFactory, authService, $crypto, ReportService, FileSaver) {

        $scope.Billing = new $.Billing({});
        var date = new Date();
        var token = $rootScope.getTokenInfo();

        $scope.Token = token;
        $scope.Billing.To = convertDate(date);
        $scope.wsApp = token.wsApp
        if (token) {
            $scope.Billing.From = convertDate(token.FinYearStart);
            $scope.MinDate = token.FinYearStart;
            $scope.MaxDate = token.FinYearEnd;// convertDate(token.FinYearEnd);
        }
        //select default ledger if it selected on some other screen
        if ($rootScope.LedgerId) {
            $scope.Billing.LedgerId = $rootScope.LedgerId;
        }
        $scope.selectedParty = function (selected) {
            if (selected != undefined) {
                var item = selected.originalObject;
                console.log('$scope.selectedParty', selected);
                $scope.Billing.LedgerId = item.LedgerId;
            }
        };
        $scope.$watch('Billing.LedgerId', function () {
            $rootScope.LedgerId = $scope.Billing.LedgerId;
            getSites();
        });
        $scope.GetBills = function () {

            var billing = cloneObj($scope.Billing);
            billing.From = formatdate(billing.From);
            billing.To = formatdate(billing.To);
            $scope.Billing.GetBillList(function (e) {
                $scope.Bills = e.data;
            }, billing);
        }
        function getSites() {
            LedgerFactory.GetMasterSites(function (e) {

                $scope.LedgerSites = e.data.Data;
            }, { LedgerId: $scope.Billing.LedgerId });
        }
        LedgerFactory.GetAllParties(function (e) {
            $scope.Accounts = e.data;
            if ($scope.Accounts) {
                $scope.Accounts.push({ LedgerId: 0, Code: '', Name: 'All Clients' });
            }
            if (e.data != null && e.data.length > 0) {
                $scope.initialValue = e.data[0];
            }
            if ($scope.Billing.LedgerId == null) {
                $scope.Billing.LedgerId = e.data[0].LedgerId;
            }
        });
        var ledger = new $.Ledger({});


        $scope.CancellBill = function () {
            var item = $scope.SelectedBill;
            if (item.StatusId == 1 || item.StatusId == 2) {
                alert('Only draft or sent for approval invoices can be cancelled');
                return;
            }

            var tr = 'tr' + item.InvoiceId;
            var confirm = window.confirm('Cancelled bill will not be revoked.\nAre you sure to cancel this bill?');
            if (!confirm) return;
            $scope.Billing.InvoiceId = item.InvoiceId;
            $(event.target).hide(); // hide cancelled button
            $scope.Billing.CancelBill(function (e) {
                $scope.GetBills();
                //if (e.data.Data == true) {
                //    $('#' + tr).addClass('cancelled'); // add cancelled class to show the row in red.
                //}
            });
        }
        //$scope.MarkSettle = function (index) {
        //    var tr = 'tr' + index.InvoiceId;
        //    var confirm = window.confirm('Settled bill will not be revoked.\nAre you sure to settle this bill?');
        //    if (!confirm) return;
        //    $scope.Billing.InvoiceId = index.InvoiceId;
        //    $(event.target).hide(); // hide cancelled button
        //    $scope.Billing.MarkSettle(function (e) {
        //        if (e.data == true) {
        //            $('#' + tr).addClass('settled'); // add cancelled class to show the row in red.
        //        }
        //    });
        //}
        $scope.SendReminder = function (item, type) {
            item.ReminderType = type;
            LedgerFactory.DueBillReminder(function (e) {

            }, item);

        }

        $scope.BillingItemsTax = function (index) {

            loadTaxes(index.InvoiceId);
        };
        function loadTaxes(invoiceId) {
            $scope.Billing.InvoiceId = invoiceId;
            $scope.Billing.BillingItemsTax(function (e) {
                $scope.AppliedTaxes = e.data;
                var div = '<div style="width:90%;height:70%"></div>';
                $(div).load('templ/taxItems.html', function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: $scope,
                        preserveScope: true,
                        locals: {
                            AppliedTaxes: $scope.AppliedTaxes
                        },
                        template: html,
                        parent: angular.element(document.body),
                        controller: function (AppliedTaxes) {
                            $scope.closeSliderModal = function () {
                                $mdDialog.hide();
                            }
                        }
                    });
                });
            });
        }
        $scope.ShowBillingItems = function (item) {

            $scope.Billing.InvoiceId = item.InvoiceId;
            // $scope.Billing.GetBillItems(function(e) {

            object = $scope.Billing
            loadBillItems($scope.Billing);
            // });
        }

        function chooseAndPrintBill(invoiceId) {
            ModalFactory.BillType(function ($scope, $mdDialog) {
                $scope.closeDialog = function () {
                    $mdDialog.hide();
                },
                    $scope.OkButtonClick = function () {
                        var headerTypes = [];

                        if ($scope.HeaderType1) {
                            headerTypes.push(1);
                        }
                        if ($scope.HeaderType2) {
                            headerTypes.push(2);
                        }
                        if ($scope.HeaderType3) {
                            headerTypes.push(3);
                        }

                        $(event.currentTarget).find('.fa-spin').show();
                        loadBillPdf(headerTypes, invoiceId);
                    }
            });
        }

        $scope.PrintBill = function () {
            var invoiceId, invTypeId;
            invoiceId = $scope.SelectedBill.InvoiceId;
            invTypeId = $scope.SelectedBill.InvoiceType;
            $scope.Billing.InvoiceId = invoiceId;///$scope.Bills[index].InvoiceId;
            // chooseAndPrintBill($scope.Billing.InvoiceId);
            var headerTypes = [];


            headerTypes.push(1);
            if (invTypeId == 5) {
                //$scope.Billing.InvoiceId = invoiceId;
                var encrypedText = $crypto.encrypt(invoiceId);

                var econded = btoa(encrypedText);
                var report = new $.Reports();
                report.downloadReportFromHtml(function (e) {
                    FileSaver.saveAs(e.data, $scope.SelectedBill.InvoiceNumber + '.pdf');
                }, 'PrintContractBill', econded);
                //$scope.Billing.PrintContractBill(function (e) {

                //    var filePath = SERVER_RPT_URL + 'temp/' + e.data;
                //    // $window.target = '_blank';
                //    $window.open(filePath);
                //});
            }
            else if (invTypeId == 4) {
                var strInput = "salebill," + + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);
                ReportService.printFromReportServer(encrypedText);
            }
            else if (invTypeId == 6 || invTypeId == 2) {
                var strInput = "salebill," + + invoiceId;
                var encrypedText = $crypto.encrypt(strInput);
                ReportService.printFromReportServer(encrypedText);
            }
            else
                loadBillPdf(headerTypes, invoiceId);


        }

        function loadBillPdf(headerType, invoiceId) {

            $scope.Billing.InvoiceId = invoiceId;
            //$scope.Billing.BillCopyType = headerType;
            $scope.Billing.HeaderTypes = headerType;


            var encrypedText = $crypto.encrypt(invoiceId);

            var econded = btoa(encrypedText);
            var report = new $.Reports();
            report.downloadReportFromHtml(function (e) {
                FileSaver.saveAs(e.data, $scope.SelectedBill.InvoiceNumber + '.pdf');
            }, 'PrintRentBill', econded);
        }
        function loadBillItems(billing) {

            var div = '<div style="width:90%;height:70%"></div>';

            $(div).load('templ/dialogs/billingItemsDialog.html?d=' + new Date().getTime().toString(), function () {
                var html = $(this).html();

                $mdDialog.show({
                    clickOutsideToClose: true,
                    scope: $scope,
                    preserveScope: true,
                    locals: {
                        billing: billing
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: BillingDialogController
                });
            });
        }
        function BillingDialogController($scope, $mdDialog, $routeParams, $http, billing) {
            $scope.DialogData = null;
            billing.GetBillItems(function (e) {

                $('#loadersite').hide();
                $scope.DialogData = e.data;

            });
            billing.GetBillLossItems(function (e) {
                $scope.LossItems = e.data;
            });
            billing.GetBreakageItems(function (e) {
                $scope.BreakageItems = e.data;
            });
            $scope.closeDialog = function () {
                $mdDialog.hide();
            }
        }

        $scope.comp = { EInvoiceEnabled: false };
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

        $scope.pushToIRP = function () {
            var item = $scope.SelectedBill;
            var billing = new $.Billing();
            billing.PushToIRP(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been pushed to IRN successfully.');

                } else {
                    alert(e.data.Message);
                }

            }, item);
        }

        $scope.createEwayBill = function () {
            var item = $scope.SelectedBill;
            var k = '0,inv,' + item.InvoiceId;
            var ec = $crypto.encrypt(k);

            $('#billOptionsModal').modal('hide');
            setTimeout(() => {
                $state.go('addEditEwayBill', { key: ec });
            }, 200);



        }
        $scope.SelectedBill = null;
        $scope.editBill = function () {

            var item = $scope.SelectedBill;

            //if (item.StatusId == 1 || item.StatusId == 2) {
            //    alert('Only draft or sent for approval invoices can be edited');
            //    return;
            //}
            if (item.EwayBillNo != null || item.IrnDetails != null) {
                alert('Can not edit after e-way bill or e-invoice prepared.');
                return;
            }
            if (item.InvoiceType == 1 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);

                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editBill', { key: key });
                }, 500);


            }
            if (item.InvoiceType == 4 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editsale', { key: key });
                }, 500);
            }
            if (item.InvoiceType == 5 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId + "," + item.ContractId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editbillContract', { id: key });
                }, 500);
            }
            if (item.InvoiceType == 6 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editlossbill', { key: key });
                }, 500);
            }
            if (item.InvoiceType == 2 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editbreakageBill', { key: key });
                }, 500);
            }
            if (item.InvoiceType == 9 && (item.StatusId == 3 || item.StatusId == 4)) {
                var key = $crypto.encrypt(item.InvoiceId);
                $('#billOptionsModal').modal('hide');
                setTimeout(() => {
                    $state.go('editmeasurebill', { key: key });
                }, 500);
            }
        }
        $scope.sendForApproval = function () {

            var item = $scope.SelectedBill;

            if (item.StatusId != 3) {
                alert('Only draft bills can be sent for approval');
                return;
            }
            var billing = new $.Billing();
            billing.SendForapproval(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been sent for approval.');
                    $('#billOptionsModal').modal('hide');
                    $scope.GetBills();

                } else {
                    alert(e.data.Message);
                }

            }, { InvoiceId: item.InvoiceId });

        }
        $scope.approveBill = function () {

            if ($scope.Token.RoleId != 1) {
                alert('You are not allowed to approve this bill');
                return;
            }
            var item = $scope.SelectedBill;

            if (item.StatusId != 4) {
                alert('Not ready for approval');
                return;
            }
            var billing = new $.Billing();
            billing.Approve(function (e) {

                if (e.data.Code == 200) {

                    alert('INVOICE has been approved.');
                    $('#billOptionsModal').modal('hide');
                    $scope.GetBills();

                } else {
                    alert(e.data.Message);
                }

            }, { InvoiceId: item.InvoiceId });

        }

        $scope.showOptions = function (item) {
            $scope.SelectedBill = item;
            if ($('#billPreviewRpt').length) {
                $('#billPreviewRpt').empty();
            }
            $('#billOptionsModal').modal('show');
            if ($scope.loadBillPreview) {
                $scope.loadBillPreview(item);
            }
        }
        $scope.getClassName = function (item) {
            if (item.StatusId == 3) {
                return 'text-success';
            }
            if (item.StatusId == 2) {
                return 'text-danger';
            }
            if (item.StatusId == 4) {
                return 'text-primary';
            }
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
                            Receipients: item.ClientEmail, Subject: 'Invoice:' + item.InvoiceNumber
                            , MetaData: "1100," + item.InvoiceId + "," + item.InvoiceNumber + ".pdf"
                        }
                    },
                    template: html,
                    parent: angular.element(document.body),
                    controller: 'ShareController'
                });
            });
        }

        $scope.SettleBill = function (bill) {

            var settleController = function ($scope) {
                $scope.SettlementRemarks = '';
                $scope.closeDialog = function () {
                    ModalFactory.hideDialog();
                }
                $scope.OkButtonClick = function () {

                    if (!$scope.SettlementRemarks || $scope.SettlementRemarks.length < 10) {
                        alert('Please enter the remarks');
                        return;
                    }
                    var bill = new $.Billing();
                    var model = { InvoiceId: $scope.SelectedBill.InvoiceId, SettlementRemarks: $scope.SettlementRemarks };
                    bill.SettleBill(function (e) {
                        if (e.data.Code != 200) {
                            alert(e.data.Message);

                        }
                        else {
                            alert('Bill settled successfully.');
                            $scope.closeDialog();
                        }
                    }, model);
                }
            }

            ModalFactory.ShowDialog(settleController, 'templ/dialogs/bill.settle.html', null, $scope, $('#billOptionsModal'));

        }
    }]);

