function quotationLineDurationFactor(quotationType, duration) {
    if (quotationType == 15)
        return parseFloat(duration) || 1;
    return 1;
}

/** Inclusive rent days for quantity/rent line mode: line period, else header period, else 1. */
function contractLineRentDayCount(item, headerTrans) {
    if (!item) return 1;
    var d = contractLinePeriodDaysInclusive(item.From, item.To);
    if (d != null && d >= 1) return d;
    if (item.Days != null && parseFloat(item.Days) >= 1) return parseFloat(item.Days);
    if (headerTrans && headerTrans.From && headerTrans.To) {
        var h = contractLinePeriodDaysInclusive(headerTrans.From, headerTrans.To);
        if (h != null && h >= 1) return h;
    }
    return 1;
}

/** quantity = Rent (Qty×Rate×Days); area = Qty×Area×Rate. */
function contractLineSubTotal(mode, qty, rate, area, headerArea, item, headerTrans) {
    var q = parseFloat(qty) || 0, r = parseFloat(rate) || 0;
    if (mode === 'area') {
        var a = parseFloat(area) || parseFloat(headerArea) || 0;
        return q * a * r;
    }
    var rentDays = contractLineRentDayCount(item, headerTrans);
    return q * r * rentDays;
}

function contractLineQtyArea(qty, area, headerArea) {
    var q = parseFloat(qty) || 0;
    var a = parseFloat(area) || parseFloat(headerArea) || 0;
    return q * a;
}

function totalContractQtyArea(items, headerArea) {
    if (!items || !items.length)
        return 0;
    return items.reduce(function (sum, item) {
        return sum + contractLineQtyArea(item.Quantity, item.Area, headerArea);
    }, 0);
}

function getContractLineMode(trans, config) {
    if (trans && (trans.LineTotalMode === 'area' || trans.LineTotalMode === 'quantity'))
        return trans.LineTotalMode;
    if (config && config.contractLineTotalMode === 'area')
        return 'area';
    return 'quantity';
}

function normalizeLineTotalMode(mode) {
    return mode === 'area' ? 'area' : 'quantity';
}

/** Per-line contract (type 16) subtotal mode; empty/null uses header then company config. */
function getEffectiveLineTotalMode(line, trans, config) {
    if (line && (line.LineTotalMode === 'area' || line.LineTotalMode === 'quantity'))
        return normalizeLineTotalMode(line.LineTotalMode);
    return getContractLineMode(trans, config);
}

function copyContractLineDefaultsFromHeader(line, trans) {
    if (!line.Area && trans.Area)
        line.Area = trans.Area;
    if (!line.From && trans.From)
        line.From = trans.From;
    if (!line.To && trans.To)
        line.To = trans.To;
}

/** Parse line From/To (Date, dd/MM/yyyy, or formatdate MM/dd/yyyy) at local midnight. */
function parseQuotationLineDate(val) {
    if (val == null || val === '')
        return null;
    if (Object.prototype.toString.call(val) === '[object Date]') {
        var ddt = new Date(val.getTime());
        ddt.setHours(0, 0, 0, 0);
        return isValidDate(ddt) ? ddt : null;
    }
    var s = String(val).trim();
    var parts = s.split('/');
    if (parts.length !== 3) {
        var dAlt = new Date(s);
        if (!isValidDate(dAlt))
            return null;
        dAlt.setHours(0, 0, 0, 0);
        return dAlt;
    }
    var p0 = parseInt(parts[0], 10), p1 = parseInt(parts[1], 10), y = parseInt(parts[2], 10);
    if (isNaN(p0) || isNaN(p1) || isNaN(y) || y < 1900)
        return null;
    var dEu = new Date(y, p1 - 1, p0);
    var dUs = new Date(y, p0 - 1, p1);
    dEu.setHours(0, 0, 0, 0);
    dUs.setHours(0, 0, 0, 0);
    if (p0 > 12 && isValidDate(dEu))
        return dEu;
    if (p1 > 12 && isValidDate(dUs))
        return dUs;
    if (isValidDate(dEu))
        return dEu;
    if (isValidDate(dUs))
        return dUs;
    return null;
}

/** Inclusive calendar days (To − From + 1), aligned with DATEDIFF(Day,…)+1 style billing. */
function contractLinePeriodDaysInclusive(fromVal, toVal) {
    var d0 = parseQuotationLineDate(fromVal);
    var d1 = parseQuotationLineDate(toVal);
    if (!d0 || !d1 || d1 < d0)
        return null;
    return Math.floor((d1 - d0) / 86400000) + 1;
}

function contractLinesMatch(a, b) {
    return a.ProductId === b.ProductId
        && String(a.From || '') === String(b.From || '')
        && String(a.To || '') === String(b.To || '')
        && (parseFloat(a.Area) || 0) === (parseFloat(b.Area) || 0);
}

function prepareContractQuotationForSave(trans, config) {
    if (trans.QuotationType != 16)
        return true;

    if (!trans.Area || parseFloat(trans.Area) <= 0) {
        alert('Please enter contract area');
        return false;
    }
    if (!trans.MeasureType || parseInt(trans.MeasureType, 10) <= 0)
        trans.MeasureType = 1;
    if (!trans.From) {
        alert('Please enter period From date');
        return false;
    }
    if (!trans.To) {
        alert('Please enter period To date');
        return false;
    }
    trans.From = formatdate(trans.From);
    trans.To = formatdate(trans.To);
    if (!isValidDate(new Date(trans.From)) || !isValidDate(new Date(trans.To))) {
        alert('Please enter valid period dates');
        return false;
    }
    if (new Date(trans.From) > new Date(trans.To)) {
        alert('Period To must be on or after From');
        return false;
    }
    var items = trans.Items || trans.BillableItems || [];
    for (var i = 0; i < items.length; i++) {
        var line = items[i];
        copyContractLineDefaultsFromHeader(line, trans);
        var eff = getEffectiveLineTotalMode(line, trans, config);
        if (eff === 'area') {
            var a = parseFloat(line.Area) || parseFloat(trans.Area) || 0;
            if (a <= 0) {
                alert('Please enter area on each line (or header) for area-wise calculation');
                return false;
            }
        }
        if (line.From)
            line.From = formatdate(line.From);
        else
            line.From = trans.From;
        if (line.To)
            line.To = formatdate(line.To);
        else
            line.To = trans.To;
        if (!isValidDate(new Date(line.From)) || !isValidDate(new Date(line.To))) {
            alert('Please enter valid period dates on all lines');
            return false;
        }
        if (new Date(line.From) > new Date(line.To)) {
            alert('Line period To must be on or after From');
            return false;
        }
        //debugger
        //var daysInc = contractLinePeriodDaysInclusive(line.From, line.To);
        //line.Days = daysInc == null ? 0 : daysInc;
    }
    return true;
}

function clearLineGstRates(line) {
    line.IGSTRate = 0;
    line.CGSTRate = 0;
    line.SGSTRate = 0;
    line.TaxRate = 0;
}

function setLineGstRatesFromTaxCategory(line, tax, isIntraState) {
    clearLineGstRates(line);
    if (!tax)
        return;
    if (isIntraState) {
        line.CGSTRate = parseFloat(tax.CGST) || 0;
        line.SGSTRate = parseFloat(tax.SGST) || 0;
        line.TaxRate = line.CGSTRate + line.SGSTRate;
    } else {
        line.IGSTRate = parseFloat(tax.IGST) || 0;
        line.TaxRate = line.IGSTRate;
    }
}

function setLineGstRatesUnregistered(line, trans) {
    clearLineGstRates(line);
    var rate = parseFloat(trans.GstRate) || 0;
    if (rate <= 0)
        return;
    if (trans.IGST)
        line.IGSTRate = rate;
    if (trans.CGST)
        line.CGSTRate = rate;
    if (trans.SGST)
        line.SGSTRate = rate;
    line.TaxRate = (line.IGSTRate || 0) + (line.CGSTRate || 0) + (line.SGSTRate || 0);
}

/** Product slab from AllSizes + TAX_CATEGORY; manual unregistered GST only when slab is not fully zero-rated (aligned with registered party logic). */
function quotationProductGstSlabForManualUnregistered(allSizes, productId, taxes) {
    if (!allSizes || !taxes || !productId)
        return { tax: null, eligible: false };
    var item = allSizes.find(function (o) { return o.ProductId == productId; });
    if (!item)
        return { tax: null, eligible: false };
    var tax = taxes.find(function (o) { return o.TaxId == item.TaxCategoryId; });
    if (!tax)
        return { tax: null, eligible: false };
    var sum = (parseFloat(tax.CGST) || 0) + (parseFloat(tax.SGST) || 0) + (parseFloat(tax.IGST) || 0);
    return { tax: tax, eligible: sum > 0 };
}

function lineItemGstPercentLabel(item) {
    if (!item)
        return '';
    var parts = [];
    var igst = parseFloat(item.IGSTRate) || 0;
    var cgst = parseFloat(item.CGSTRate) || 0;
    var sgst = parseFloat(item.SGSTRate) || 0;
    if (igst > 0)
        parts.push('IGST ' + igst + '%');
    if (cgst > 0)
        parts.push('CGST ' + cgst + '%');
    if (sgst > 0)
        parts.push('SGST ' + sgst + '%');
    if (parts.length)
        return parts.join(', ');
    var tr = parseFloat(item.TaxRate) || 0;
    if (tr > 0)
        return tr + '%';
    return '';
}

function quotationSummaryGstRate(items, component) {
    if (!items || !items.length)
        return null;
    var field = component === 'IGST' ? 'IGSTRate' : (component === 'CGST' ? 'CGSTRate' : 'SGSTRate');
    for (var i = 0; i < items.length; i++) {
        var r = parseFloat(items[i][field]) || 0;
        if (r > 0)
            return r;
    }
    return null;
}

//--sales register
app.controller('QuotationListController', function ($scope, $rootScope, $state, $crypto, $mdDialog, ReportService, ModalFactory, $timeout, LedgerFactory) {
    $scope._stateData = $state.current.data;
    var sales = new $.Transaction({ SalesId: 0 });
    var date = new Date();
    var token = $rootScope.getTokenInfo();
    $scope.QuotationStatuses = StaicData.QUOTATIONT_STATUSES;
    $scope.QuotationTypes = StaicData.QUOTATIONT_TYPES;

    var QUOTATION_LIST_FILTER_KEY = 'rentacQuotationListFilter_v1';

    function quotationListStorageKey() {
        return $scope._stateData.category === 'pi'
            ? QUOTATION_LIST_FILTER_KEY + '_pi'
            : QUOTATION_LIST_FILTER_KEY + '_quotation';
    }

    function persistQuotationListFilter() {
        try {
            var payload = {
                LedgerId: $scope.Filter.LedgerId,
                QuotationType: $scope.Filter.QuotationType,
                StatusId: $scope.Filter.StatusId,
                From: $scope.Filter.From,
                To: $scope.Filter.To,
                Category: $scope.Filter.Category,
                CurrentPage: $scope.CurrentPage
            };
            sessionStorage.setItem(quotationListStorageKey(), JSON.stringify(payload));
        } catch (ex) { }
    }

    function persistQuotationListWithScrollTarget(quotationId) {
        try {
            var payload = {
                LedgerId: $scope.Filter.LedgerId,
                QuotationType: $scope.Filter.QuotationType,
                StatusId: $scope.Filter.StatusId,
                From: $scope.Filter.From,
                To: $scope.Filter.To,
                Category: $scope.Filter.Category,
                CurrentPage: $scope.CurrentPage,
                pendingScrollToQuotationId: quotationId
            };
            sessionStorage.setItem(quotationListStorageKey(), JSON.stringify(payload));
        } catch (ex) { }
    }

    function restoreQuotationListFilter() {
        try {
            var raw = sessionStorage.getItem(quotationListStorageKey());
            return raw ? JSON.parse(raw) : null;
        } catch (ex) {
            return null;
        }
    }

    function afterQuotationListLoaded() {
        var pendingId = null;
        try {
            var st = restoreQuotationListFilter();
            if (st && st.pendingScrollToQuotationId) {
                pendingId = st.pendingScrollToQuotationId;
            }
        } catch (ex) { }
        persistQuotationListFilter();
        if (pendingId) {
            $timeout(function () {
                var el = document.getElementById('quotation-row-' + pendingId);
                if (el && typeof el.scrollIntoView === 'function') {
                    el.scrollIntoView({ block: 'center', behavior: 'smooth' });
                }
            }, 150);
        }
    }

    var savedListFilter = restoreQuotationListFilter();

    $scope.Filter = { LedgerId: 0, From: convertDate('01/01/2018'), To: convertDate(date), Category: $scope._stateData.category };
    if ($scope._stateData.category === 'quotation') {
        var toDate = new Date();
        var fromDate = new Date(toDate);
        fromDate.setMonth(fromDate.getMonth() - 1);
        $scope.Filter.To = convertDate(toDate);
        $scope.Filter.From = convertDate(fromDate);
    } else if (token) {
        $scope.Filter.From = convertDate(token.FinYearStart);
    }

    $scope.PAGE_SIZE = 20;
    $scope.CurrentPage = 1;
    if (savedListFilter) {
        if (savedListFilter.From) {
            $scope.Filter.From = savedListFilter.From;
        }
        if (savedListFilter.To) {
            $scope.Filter.To = savedListFilter.To;
        }
        if (savedListFilter.LedgerId !== undefined && savedListFilter.LedgerId !== null) {
            $scope.Filter.LedgerId = savedListFilter.LedgerId;
        }
        if (savedListFilter.QuotationType !== undefined && savedListFilter.QuotationType !== null) {
            $scope.Filter.QuotationType = savedListFilter.QuotationType;
        }
        if (savedListFilter.StatusId !== undefined && savedListFilter.StatusId !== null) {
            $scope.Filter.StatusId = savedListFilter.StatusId;
        }
        if (savedListFilter.CurrentPage > 0) {
            $scope.CurrentPage = savedListFilter.CurrentPage;
        }
    }

    var ledger = new $.Ledger({});
    ledger.GetAll(function (e) {
        $scope.Accounts = e.data;
        if (($scope.Filter.LedgerId == null || $scope.Filter.LedgerId === 0) && e.data && e.data.length) {
            $scope.Filter.LedgerId = e.data[0].LedgerId;
        }
    });
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId && !savedListFilter) {
        $scope.Filter.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Filter.LedgerId', function () {
        $rootScope.LedgerId = $scope.Filter.LedgerId;
    });

    $scope.showItems = function (index) {

        loadItems(index.SalesId);
    };
    $scope.SalesItemsTax = function (index) {

        loadTaxes(index.SalesId);
    };
    function loadTaxes(salesId) {
        sales.SalesId = salesId;
        sales.SalesItemsTax(function (e) {
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

    function loadItems(salesId) {

        var div = '<div style="width:90%;height:70%"></div>';

        $(div).load('templ/dialogs/purchaseItems.html', function () {
            var html = $(this).html();

            $mdDialog.show({
                clickOutsideToClose: true,
                scope: $scope,
                preserveScope: true,
                locals: {
                    salesId: salesId
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'SalesItemsController'
            });
        });
    }

    //$scope.print = function (item, format) {
    //    var filter = { 'QuotationId': item.QuotationId, 'FileFormat': format };
    //    //  purchase.PurchaseId = item.PurchaseId;
    //    sales.PrintQuotation(function (e) {
    //        //debugger
    //        var filePath = SERVER_RPT_URL + 'temp/' + e.data;
    //        // $window.target = '_blank';
    //        $window.open(filePath);
    //    }, filter);
    //}

    $scope.edit = function (item) {
        persistQuotationListWithScrollTarget(item.QuotationId);
        var key = $crypto.encrypt(item.QuotationId);
        $('#previewDialog').modal('hide');
        setTimeout(() => {
            if (item.Category == 'pi') {
                $state.go('editpinvoice', { key: key });
            }
            else {
                $state.go('editquotation', { key: key });
            }
        }, 500);
    }

    $scope.goNewQuotation = function ($event) {
        if ($event) {
            $event.preventDefault();
        }
        persistQuotationListFilter();
        if ($scope._stateData.category === 'pi') {
            $state.go('pinvoice');
        } else {
            $state.go('quotation');
        }
    };

    $scope.Preview = 0;
    $scope.SelectedItem;
    $scope.StatusToUpdate = 0;
    $scope.preview = function (item) {
        $scope.SelectedItem = item;
        $scope.Preview = 1;
        $scope.StatusToUpdate = 0;
        $('#previewDialog').modal('show');

        var strInput = "quotation," + item.QuotationId
        var encrypedText = $crypto.encrypt(strInput);


        ReportService.loadPreviewFromReportServer(function (e) {
            $scope.Preview = null;
            $('#rpt').html(e.data);

        }, encrypedText);
    }

    $scope.printPdf = function () {

        var item = $scope.SelectedItem;
        var strInput = "quotation," + + item.QuotationId;
        var encrypedText = $crypto.encrypt(strInput);

        var fileName = item.Client + "-" + item.QuotationNumber + ".pdf";
        ReportService.printFromReportServer(encrypedText, fileName);
    }

    $scope.showEmailDialog = function (quotation) {
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
                        Receipients: quotation.ClientEmail, Subject: 'Quotation:' + quotation.QuotationNumber
                        , MetaData: "1102," + quotation.QuotationId + "," + quotation.QuotationNumber + ".pdf"

                    }
                },
                template: html,
                parent: angular.element(document.body),
                controller: 'ShareController'
            });
        });
    }
    $scope.qToSale = function (item) {

        $('#previewDialog').modal('hide');

        var strInput = item.QuotationId
        var encrypedText = $crypto.encrypt(strInput);
        setTimeout(() => {
            $state.go('qtsale', { qId: encrypedText });
        }, 500);
    }
    $scope.createContract = function (item) {

        $('#previewDialog').modal('hide');

        var strInput = item.QuotationId
        var encrypedText = $crypto.encrypt(strInput);
        setTimeout(() => {
            $state.go('contract', { id: 0, qId: encrypedText });
        }, 500);
    }

    $scope.createPartyFromPreview = function () {
        var item = $scope.SelectedItem;
        if (!item || item.LedgerId > 0) {
            return;
        }
        $('#previewDialog').modal('hide');
        function openModal(unm, uad, uph) {
            ModalFactory.AddEditLedger('AddEditLedgerController', {
                LedgerId: 0,
                forQuotation: 1,
                quotationIdForLink: item.QuotationId,
                presetPartyName: (unm || '').trim(),
                presetPartyAddress: (uad || '').trim(),
                presetPartyPhone: (uph || '').trim()
            });
        }
        if ((item.UnregisteredPartyName && item.UnregisteredPartyName.trim()) ||
            (item.UnregisteredPartyAddress && item.UnregisteredPartyAddress.trim()) ||
            (item.UnregisteredPartyPhone && item.UnregisteredPartyPhone.trim())) {
            openModal(item.UnregisteredPartyName, item.UnregisteredPartyAddress, item.UnregisteredPartyPhone);
            return;
        }
        var tx = new $.Transaction({});
        tx.GetQutotationById(function (e) {
            var d = e.data && e.data.Data;
            if (d) {
                openModal(d.UnregisteredPartyName, d.UnregisteredPartyAddress, d.UnregisteredPartyPhone);
            } else {
                openModal('', '', '');
            }
        }, item.QuotationId);
    };

    $scope.UpdateStatus = function (item) {

        //if ($scope.StatusToUpdate == 0) {
        //    alert('Please selec the status');
        //    return;
        //}


        sales.UpdatedQuotationStatus(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            ModalFactory.Dialog.hide();
            $('#previewDialog').modal('hide');
        }, { QuotationId: item.QuotationId, statusId: $scope.SelectedItem.StatusId });
    }
    $scope.delete = function (item) {

        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this quotation record?';
            $scope.OkButtonClick = function () {

                sales.UpdatedQuotationStatus(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    ModalFactory.Dialog.hide();
                    $('#previewDialog').modal('hide');
                }, { QuotationId: item.QuotationId, statusId: 3 });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        };
        ModalFactory.Confirm(deleteController, $scope, $('#previewDialog'));
        //ModalFactory.ConfirmBT($scope.OkButtonClick, $scope.OkButtonClick, 'Are you sure to delete this quotation record?');

    }

    $scope.TotalItems = 0;
    $scope.find = function (page) {
        if (page == null || page === undefined) {
            $scope.CurrentPage = 1;
        } else {
            $scope.CurrentPage = page;
        }
        var filter = cloneObj($scope.Filter);
        filter.From = formatdate(filter.From);
        filter.To = formatdate(filter.To);
        filter.PageIndex = $scope.CurrentPage;
        filter.PageSize = $scope.PAGE_SIZE;
        sales.QuotationsList(function (e) {
            var res = e.data;
            if (res && res.Data !== undefined) {
                $scope.Register = res.Data;
                $scope.TotalItems = res.TotalCount || res.Data.length;
            } else {
                $scope.Register = res || [];
                $scope.TotalItems = $scope.Register.length;
            }
            afterQuotationListLoaded();
        }, filter);
    };
    $scope.onPageChange = function (newPage) {
        $scope.find(newPage);
    };
    if (savedListFilter && savedListFilter.CurrentPage > 0) {
        $scope.find(savedListFilter.CurrentPage);
    } else {
        $scope.find();
    }

    var onLedgerAddedForQuotationParty = $rootScope.$on('OnLedgerAdded', function (evt, data) {
        if (!data || !data.quotationIdForLink) {
            return;
        }
        function runLink(ledgerId, partyName) {
            var tid = data.quotationIdForLink;
            var tx = new $.Transaction({});
            tx.LinkQuotationToLedger(function (e) {
                if (e.data.Code !== 200 && e.data.Code !== '200') {
                    alert(e.data.Message || 'Failed to link party to quotation');
                    return;
                }
                $scope.$apply(function () {
                    if ($scope.SelectedItem && $scope.SelectedItem.QuotationId === tid) {
                        $scope.SelectedItem.LedgerId = ledgerId;
                        if (partyName) {
                            $scope.SelectedItem.Client = partyName;
                        }
                    }
                    if ($scope.Register) {
                        var row = $scope.Register.find(function (o) { return o.QuotationId === tid; });
                        if (row) {
                            row.LedgerId = ledgerId;
                            if (partyName) {
                                row.Client = partyName;
                            }
                        }
                    }
                });
            }, { QuotationId: tid, LedgerId: ledgerId });
        }
        var lid = data.LedgerId;
        var pname = data.partyName;
        if (lid && lid > 0) {
            runLink(lid, pname);
            return;
        }
        if (data.Code) {
            LedgerFactory.GetAllParties(function (e) {
                var nl = e.data.find(function (x) { return x.Code == data.Code; });
                if (nl && nl.LedgerId) {
                    runLink(nl.LedgerId, nl.Name || nl.TradeName || pname);
                } else {
                    alert('Party was saved but could not be linked to the quotation.');
                }
            });
        } else {
            alert('Party was saved but ledger id was not returned.');
        }
    });
    $scope.$on('$destroy', function () {
        onLedgerAddedForQuotationParty();
    });
    //PurchaseItemsList
});
//-end of sales register
app.controller('SalesItemsController', function ($scope, $mdDialog, salesId) {

    var sales = new $.Transaction({ SalesId: salesId });
    sales.SalesItemsList(function (e) {
        $('#loadersite').hide();
        $scope.Items = e.data;
    });

    $scope.closeDialog = function () {
        $mdDialog.hide();
    }
});
//---Sale entry
app.controller('QuotationController', function ($scope, $rootScope, $stateParams, $state, $http,
    ModalFactory, $uibModal, LedgerFactory, AuthenticationService, $timeout) {

    $scope._stateData = $state.current.data;

    var sale = new $.Transaction({ PurchaseId: 0, QuotationId: 0 });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var config = new $.Config();
    var loginData = AuthenticationService.getTokenInfo();

    $scope.Trans = sale;
    $scope.Trans.Category = $scope._stateData.category;
    $scope.Trans.PartyType = 1;
    $scope.Trans.UnregisteredPartyName = '';
    $scope.Trans.UnregisteredPartyAddress = '';
    $scope.Trans.UnregisteredPartyPhone = '';
    $scope.Trans.GstRate = 0;
    $scope.Trans.IGST = false;
    $scope.Trans.CGST = false;
    $scope.Trans.SGST = false;
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.QuotationType = 15;
    $scope.Trans.MeasureType = 1;
    $scope.Trans.LineTotalMode = 'quantity';
    $scope.Trans.Charge5 = 0;
    $scope.Trans.Items = [];//initializeArray();
    $scope.Trans.FreightTaxRate = 0;
    $scope.lineQtyArea = function (item) {
        return contractLineQtyArea(item.Quantity, item.Area, $scope.Trans.Area);
    };
    $scope.linePeriodDaysLabel = function (item) {
        var d = contractLinePeriodDaysInclusive(item.From, item.To);
        return d == null ? '—' : String(d);
    };
    $scope.totalContractQtyArea = function () {
        return totalContractQtyArea($scope.Trans.Items, $scope.Trans.Area);
    };
    $scope.lineItemGstPercentLabel = lineItemGstPercentLabel;
    $scope.quotationSummaryGstRate = function (component) {
        return quotationSummaryGstRate($scope.Trans.Items, component);
    };
    $scope.ApplyGST = true;
    //$scope.ApplyCGST = false;
    //$scope.ApplySGST = false;
    $scope.ApplyFreightGST = false;
    $scope.ApplyOtherChargeGST = false;
    $scope.Trans.QuotationDate = convertDate(new Date());

    var PI_VARIANCE_PREFILL_KEY = 'rentacPiVariancePrefill';

    var leder = new $.Ledger();
    leder.SearchClient(function (e) {
        $scope.SundryDebtors = e.data.items;
        //   $scope.SundryDebtors.splice(0, 0, { LedgerId: -1, Name: 'Other' });
        init();
        tryApplyVariancePrefill();

    }, { Page: 'quotation' });
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }

    $scope.$watch('Trans.LedgerId', function () {

        if ($scope.Trans.LedgerId == 0) return;
        $scope.getSites();
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        if (!$scope.SundryDebtors) return;
        var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode
            $scope.SelectedLedger = ledger;
        }
    });
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }
    $scope.NewSite = function () {
        if ($scope.Trans.LedgerId) {
            if ($scope.Trans.LedgerId > 0) {
                ModalFactory.AddEditClientSite('AddEditClientSiteController', {
                    LedgerId: $scope.Trans.LedgerId,
                    LedgerSiteId: 0
                });

            }
        }
    };
    var onSiteAdded = $rootScope.$on("OnSiteAdded", function (evt, data) {
        $scope.getSites();
    });
    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }




    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ PurchaseRate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmQuotation').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }
        EnableToolbar(0);
        //   var reader = new FileReader();
        var fileList = [];
        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }
        addWorkOrder(fileList);
    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    function addWorkOrder(fileList) {

        var model = cloneObj($scope.Trans);
        var quto = new $.Transaction();
        //model.AddInfo = $scope.RentAddInfo;
        //model.Tnc = $scope.Renttnc;

        var pt = parseInt(model.PartyType, 10) || 1;
        model.PartyType = pt;
        if (pt === 1) {
            if (!model.LedgerId || model.LedgerId <= 0) {
                alert('Please select the party');
                return;
            }
        } else {
            model.LedgerId = 0;
            model.LedgerSiteId = 0;
            var unm = (model.UnregisteredPartyName || '').trim();
            var uad = (model.UnregisteredPartyAddress || '').trim();
            if (!unm || !uad) {
                alert('Please enter party name and address');
                return;
            }
            model.UnregisteredPartyName = unm;
            model.UnregisteredPartyAddress = uad;
            model.UnregisteredPartyPhone = (model.UnregisteredPartyPhone || '').trim();
        }
        //if (!model.LedgerSiteId || model.LedgerSiteId <= 0) {
        //    alert('Please select the party site');
        //    return;
        //}
        model.BillableItems = model.Items;
        var qNumber = model.QuotationNumber;
        if ($scope.Config.editnumber == true && (!qNumber || qNumber.length < 1)) {
            alert('Please enter a quotation number');
            return;
        }
        if (!model.PoDate) {
            model.PoDate = '';
        }
        model.PoDate = formatdate(model.PoDate);
        var _poDate = isValidDate(new Date(model.PoDate));
        if (model.PoNumber && model.PoNumber != '' && !_poDate) {
            alert('Please enter a valid PO Date');
            return;
        }

        if ((!model.PoNumber || model.PoNumber == '') && _poDate) {
            alert('Please enter a PO Number');
            return;
        }

        model.LineTotalMode = normalizeLineTotalMode(getContractLineMode(model, $scope.Config));
        if (!prepareContractQuotationForSave(model, $scope.Config))
            return;

        if (!model.ValidUntil) {
            model.ValidUntil = '';
        }
        model.ValidUntil = formatdate(model.ValidUntil);
        model.QuotationDate = formatdate(model.QuotationDate);
        if (model.QuotationType != 16) {
            model.From = model.QuotationDate;
            model.To = model.QuotationDate;
            model.LineTotalMode = 'quantity';
        }
        if (model.BillableItems) {
            for (var li = 0; li < model.BillableItems.length; li++) {
                var bl = model.BillableItems[li];
                if (model.QuotationType != 15)
                    bl.Duration = 1;
                else if (!bl.Duration || bl.Duration <= 0)
                    bl.Duration = 1;
                if (model.QuotationType == 16) {
                    copyContractLineDefaultsFromHeader(bl, model);
                    var effBl = getEffectiveLineTotalMode(bl, model, $scope.Config);
                    bl.SubTotal = contractLineSubTotal(effBl, bl.Quantity, bl.Rate, bl.Area, model.Area, bl, model);
                }
            }
        }
        quto.SaveQuotation(function (e) {

            if (e.statusText == 'OK' || e.data == true) {

                alert('saved');
                $scope.warnOnLeave = false;
                if ($scope.fromContractInfoModal) {
                    $('#contractNewQuotationModal').modal('hide');
                } else if ($scope._stateData.category == 'pi') {
                    try {
                        sessionStorage.removeItem(PI_VARIANCE_PREFILL_KEY);
                    } catch (ex) { }
                    $state.go('pinvoices');
                }
                else
                    $state.go('quotations');
                //   $state.reload();
            } else {
                showMessage(e.data);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        calculateDiscount();
        //$scope.SubTotal(0);
    });

    function calculateDiscount() {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        // $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountPercent) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }
    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);
    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }
        if ($scope.Trans.QuotationType != 15)
            $scope.TransItem.Duration = 1;
        else if (!$scope.TransItem.Duration || $scope.TransItem.Duration <= 0)
            $scope.TransItem.Duration = 1;

        if ($scope.Trans.QuotationType == 16)
            copyContractLineDefaultsFromHeader($scope.TransItem, $scope.Trans);

        //var itemExist = $scope.Trans.QuotationType == 16
        //    ? $scope.Trans.Items.find(o => contractLinesMatch(o, $scope.TransItem))
        //    : $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId);
        //if (itemExist) {
        //    itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
        //    if ($scope.Trans.QuotationType == 16)
        //        itemExist.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode(itemExist, $scope.Trans, $scope.Config), itemExist.Quantity, itemExist.Rate, itemExist.Area, $scope.Trans.Area, itemExist, $scope.Trans);
        //    else
        //        itemExist.SubTotal = itemExist.Quantity * itemExist.Rate * quotationLineDurationFactor($scope.Trans.QuotationType, itemExist.Duration);
        //} else {

        if ($scope.Trans.QuotationType == 16)
            $scope.TransItem.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode($scope.TransItem, $scope.Trans, $scope.Config), $scope.TransItem.Quantity, $scope.TransItem.Rate, $scope.TransItem.Area, $scope.Trans.Area, $scope.TransItem, $scope.Trans);
        else
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate * quotationLineDurationFactor($scope.Trans.QuotationType, $scope.TransItem.Duration);
        $scope.Trans.Items.push($scope.TransItem);
        //  }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    function applyDiscount(item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    function getAllProductSizesByCompany() {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {

            for (var i = 0; i < $scope.Trans.Items.length; i++) {
                var item = $scope.Trans.Items[i];
                if (item.Quantity != null) {
                    if ($scope.Trans.QuotationType == 16) {
                        item.QtyArea = contractLineQtyArea(item.Quantity, item.Area, $scope.Trans.Area);
                         
                        var _days = contractLinePeriodDaysInclusive(item.From, item.To);
                        item.Days = _days == null ? 0 : _days;
                        item.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode(item, $scope.Trans, $scope.Config), item.Quantity, item.Rate, item.Area, $scope.Trans.Area, item, $scope.Trans);
                    } else {
                        item.SubTotal = parseFloat(item.Quantity) * parseFloat(item.Rate)
                            * quotationLineDurationFactor($scope.Trans.QuotationType, item.Duration);
                    }
                }
                applyDiscount(item);
                applyTaxRate(item.ProductId);
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);


            //if ($scope.Trans.Items) {
            //    for (var i = 0; i < $scope.Trans.Items.length; i++) {
            //        var item = $scope.Trans.Items[i];
            //        applyDiscount(item);
            //        applyTaxRate(item.ProductId);
            //    }
            //}
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        calculateDiscount();

        //}
        if ($scope.Config) {
            if ($scope.ApplyFreightGST == true) {
                $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.FreightTaxRate = 0;
            }
            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.ChargesTaxRate = 0;
            }

            if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
                var freight = parseFloat($scope.Trans.Freight) + parseFloat($scope.Trans.FreightIn);
                $scope.Trans.FreightTax = (freight * $scope.Trans.FreightTaxRate) / 100
            }

            $scope.Trans.OtherChargeAmount = parseFloat($scope.Trans.Charge1) + parseFloat($scope.Trans.Charge2) + parseFloat($scope.Trans.Charge3)
                + parseFloat($scope.Trans.Charge4) + parseFloat($scope.Trans.Charge5);
            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * $scope.Trans.ChargesTaxRate) / 100
            }
        }


        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight) + parseFloat($scope.Trans.FreightTax) +
            $scope.Trans.OtherChargeAmount + $scope.Trans.ChargesTax;

        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Description = selected.originalObject.Description;
            if (selected.originalObject.BOMDescription != '') {
                $scope.TransItem.Description = selected.originalObject.BOMDescription;
            }

        }
    };

    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', editnumber: false,
        outwardfreight: true, inwardfreight: false, attachRateSheet: false, rentAddInfo: '', renttnc: '',
        saleAddInfo: '', saletnc: '', contractAddInfo: '', contracttnc: '', contractLineTotalMode: 'quantity'
    };

    $scope.GetBillingConfig = function () {
        config.GetBillingConfig(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {
                    var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                    if (freightTax) {
                        $scope.Config.FreightTax = $scope.Trans.FreightTaxRate = freightTax.Value
                        $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate = freightTax.Value;
                        $scope.ApplyFreightGST = $scope.ApplyOtherChargeGST = parseInt($scope.Config.FreightTax) > 0;
                    }


                }
                // 
            }
        });
    }

    $scope.GetBillingConfig();

    $scope.DefaultRate = 0.0;

    $scope.$watch('Trans.QuotationType', function () {
        // $scope.Config.ChargesTaxRate = 0;
        //$scope.Trans.ChargesTaxRate = 0;
        $scope.setAddInfoAndTnc();
        if ($scope.Trans.QuotationType == 16) {
            if (!$scope.Trans.LineTotalMode || ($scope.Trans.LineTotalMode !== 'area' && $scope.Trans.LineTotalMode !== 'quantity'))
                $scope.Trans.LineTotalMode = getContractLineMode($scope.Trans, $scope.Config);
        }
    }, true);
    $scope.$watch('Trans.LineTotalMode', function (n, o) {
        if (o === undefined || n === o || $scope.Trans.QuotationType != 16)
            return;
        $scope.Trans.LineTotalMode = normalizeLineTotalMode(n);
        $scope.SubTotal(0);
    });
    $scope.$watchGroup(['Trans.Area', 'Trans.From', 'Trans.To'], function () {
        if ($scope.Trans.QuotationType == 16 && $scope.TransItem) {
            if (!$scope.TransItem.Area && $scope.Trans.Area)
                $scope.TransItem.Area = $scope.Trans.Area;
            if (!$scope.TransItem.From && $scope.Trans.From)
                $scope.TransItem.From = $scope.Trans.From;
            if (!$scope.TransItem.To && $scope.Trans.To)
                $scope.TransItem.To = $scope.Trans.To;
        }
        if ($scope.Trans.QuotationType == 16)
            $scope.SubTotal(0);
    });
    $scope.setAddInfoAndTnc = function () {
        if ($scope.Trans.QuotationType == 15) {
            $scope.Trans.AddInfo = $scope.RentAddInfo;
            $scope.Trans.Tnc = $scope.Renttnc;

        }
        if ($scope.Trans.QuotationType == 16) {
            $scope.Trans.AddInfo = $scope.ContractAddInfo;
            $scope.Trans.Tnc = $scope.Contracttnc;
        }
        if ($scope.Trans.QuotationType == 17) {
            $scope.Trans.AddInfo = $scope.SaleAddInfo;
            $scope.Trans.Tnc = $scope.Saletnc;
        }
    }
    $scope.$watch('ApplyFreightGST', function () {
        //  $scope.Config.FreightTax = 0;

        if ($scope.ApplyFreightGST == false) {
            $scope.Trans.FreightTaxRate = 0;
            //   $scope.Trans.FreightTax = 0;
        }
        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        }

        $scope.SubTotal(0);
    }, true);

    $scope.$watch('ApplyOtherChargeGST', function () {
        // $scope.Config.ChargesTaxRate = 0;
        //$scope.Trans.ChargesTaxRate = 0;
        if ($scope.ApplyOtherChargeGST == false) {
            $scope.Trans.ChargesTaxRate = 0;
            //$scope.Trans.FreightTax = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate;
        }
        $scope.SubTotal(0);
    }, true);

    $scope.$watch('ApplyGST', function () {

        $scope.SubTotal(0);

    }, true);

    $scope.$watch('Trans.PartyType', function (n, o) {
        if (o === undefined || n === o) return;
        if (n === 2) {
            $scope.Trans.LedgerId = 0;
            $scope.Trans.LedgerSiteId = 0;
            $scope.LedgerSites = [];
            $scope.ApplyGST = false;
        } else if (n === 1) {
            $scope.Trans.UnregisteredPartyName = '';
            $scope.Trans.UnregisteredPartyAddress = '';
            $scope.Trans.UnregisteredPartyPhone = '';
            $scope.Trans.GstRate = 0;
            //$scope.Trans.IGST = false;
            //$scope.Trans.CGST = false;
            //$scope.Trans.SGST = false;
            $scope.ApplyGST = false;
        }
        $scope.SubTotal(0);
    });

    $scope.$watchGroup(['Trans.GstRate', 'Trans.IGST', 'Trans.CGST', 'Trans.SGST'], function () {
        $scope.SubTotal(0);
    });

    function applyTaxRate(productId) {
        var taxes = StaicData.TAX_CATEGORY;
        var pt = parseInt($scope.Trans.PartyType, 10) || 1;
        var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);
        if (!lineItems.length) {
            return;
        }

      //  if (pt === 2) {
            var rate = parseFloat($scope.Trans.GstRate) || 0;
            for (var i = 0; i < lineItems.length; i++) {
                if (!lineItems[i].DiscountAmount) {
                    lineItems[i].DiscountAmount = 0;
                }
                var slab = quotationProductGstSlabForManualUnregistered($scope.AllSizes, lineItems[i].ProductId, taxes);
                if (!slab.eligible) {
                    lineItems[i].CGST = 0;
                    lineItems[i].SGST = 0;
                    lineItems[i].IGST = 0;
                    clearLineGstRates(lineItems[i]);
                    lineItems[i].TaxName = slab.tax ? slab.tax.TaxName : '';
                    continue;
                }
                var taxable = lineItems[i].SubTotal - lineItems[i].DiscountAmount;
                lineItems[i].CGST = 0;
                lineItems[i].SGST = 0;
                lineItems[i].IGST = 0;
                if (rate > 0) {
                    if ($scope.Trans.IGST) {
                        lineItems[i].IGST = taxable * rate / 100;
                    }
                    if ($scope.Trans.CGST) {
                        lineItems[i].CGST = taxable * rate / 100;
                    }
                    if ($scope.Trans.SGST) {
                        lineItems[i].SGST = taxable * rate / 100;
                    }
                }
                lineItems[i].TaxName = 'GST';
                setLineGstRatesUnregistered(lineItems[i], $scope.Trans);
            }
            return;
       // }

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var isIntraState = $scope.comp && ledgerDTO.Props && $scope.comp.StateCode == ledgerDTO.Props.StateCode;

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = 0;
                        lineItems[i].SGST = 0;
                        lineItems[i].IGST = 0;
                        setLineGstRatesFromTaxCategory(lineItems[i], tax, isIntraState);
                        lineItems[i].TaxName = tax.TaxName;
                        if ($scope.ApplyGST == true) {
                            if (isIntraState) {
                                lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                                lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                            }
                            else {
                                lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;
                            }
                        }
                    }
                } else {
                    for (var j = 0; j < lineItems.length; j++) {
                        clearLineGstRates(lineItems[j]);
                    }
                }
            }
        }

    }

    function unwrapGrnLineItems(res) {
        var body = res && res.data;
        if (!body) return [];
        if (angular.isArray(body)) return body;
        if (body.Data != null && angular.isArray(body.Data)) return body.Data;
        if (body.data != null && angular.isArray(body.data)) return body.data;
        return [];
    }

    function mergeVarianceLinesIntoTrans(lines, mode, hints) {
        var hintMap = {};
        if (hints && hints.length) {
            angular.forEach(hints, function (h) {
                var key = (h.grnId || 0) + '_' + (h.productId || 0);
                hintMap[key] = h;
            });
        }
        angular.forEach(lines, function (line) {
            var key = (line.GRNId || 0) + '_' + (line.ProductId || 0);
            var hint = hintMap[key];
            var qty;
            var rate;
            if (mode === 'lost') {
                qty = hint ? (parseFloat(hint.shortQty) || 0) : (parseFloat(line.ShortQty) || 0);
                if (qty <= 0) return;
                rate = parseFloat(line.Rate) || 0;
            } else {
                qty = hint ? (parseFloat(hint.breakage) || 0) : (parseFloat(line.Breakage) || 0);
                if (qty <= 0) return;
                var br = parseFloat(line.BreakageRate) || 0;
                rate = br > 0 ? br : (parseFloat(line.Rate) || 0);
            }
            if (!line.ProductId || line.ProductId <= 0 || qty <= 0) return;

            var itemExist = $scope.Trans.Items.find(o => o.ProductId == line.ProductId);
            if (itemExist) {
                itemExist.Quantity = parseFloat(itemExist.Quantity) + qty;
                itemExist.SubTotal = itemExist.Quantity * itemExist.Rate * (itemExist.Duration ? itemExist.Duration : 1);
            } else {
                var ti = angular.extend(new $.TransItem({}), {
                    ProductId: line.ProductId,
                    Item: line.Item,
                    Quantity: qty,
                    Rate: rate,
                    Duration: 1,
                    QuotationType: $scope.Trans.QuotationType
                });
                ti.SubTotal = qty * rate * (ti.Duration ? ti.Duration : 1);
                $scope.Trans.Items.push(ti);
            }
        });
        $scope.SubTotal(0);
    }

    function tryApplyVariancePrefill() {
        var cat = ($state.current.data && $state.current.data.category) || ($scope._stateData && $scope._stateData.category);
        if (cat !== 'pi') return;
        var raw;
        try {
            raw = sessionStorage.getItem(PI_VARIANCE_PREFILL_KEY);
        } catch (ex) {
            return;
        }
        if (!raw) return;
        var payload;
        try {
            payload = JSON.parse(raw);
        } catch (ex2) {
            return;
        }
        if (!payload || !payload.grnIds || !payload.grnIds.length) return;
        var ledgerId = parseInt(payload.ledgerId, 10) || 0;
        var ledgerSiteId = parseInt(payload.ledgerSiteId, 10) || 0;
        var mode = payload.mode === 'lost' ? 'lost' : 'breakage';
        if (ledgerId <= 0) return;

        if ($scope.Trans.Items && $scope.Trans.Items.length > 0) return;

        $scope.Trans.LedgerId = ledgerId;

        LedgerFactory.GetMasterSites(function (e) {
            $scope.LedgerSites = e.data.Data;
            $timeout(function () {
                $scope.Trans.LedgerSiteId = ledgerSiteId;
            }, 0);

            var pending = payload.grnIds.slice();
            var collected = [];

            function loadNext() {
                if (!pending.length) {
                    mergeVarianceLinesIntoTrans(collected, mode, payload.hints);
                    return;
                }
                var gid = pending.shift();
                var grn = new $.GRN({});
                grn.GRNId = gid;
                grn.GetItemsByGRNId(function (res) {
                    var items = unwrapGrnLineItems(res);
                    for (var i = 0; i < items.length; i++) {
                        collected.push(items[i]);
                    }
                    loadNext();
                });
            }
            loadNext();
        }, { LedgerId: ledgerId });
    }

    getAllProductSizesByCompany();

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

    $scope.getConfig = function () {
        var config = new $.Config();
        var configCategory = ($scope._stateData && $scope._stateData.category === 'pi') ? 'pi' : 'quotation';
        var mainSub = configCategory === 'pi' ? 'pi' : 'quotation';
        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var qAddInfo = response.Data.find(o => o.SubCategory == 'rent' && o.Key == 'addInfo');

                    if (qAddInfo) {
                        $scope.RentAddInfo = $scope.Config.rentAddInfo = qAddInfo.Value;
                        $('#divAddInfo').html($scope.RentAddInfo);
                    }
                    var qtnc = response.Data.find(o => o.SubCategory == 'rent' && o.Key == 'tnc');

                    if (qtnc) {
                        $scope.Renttnc = $scope.Config.renttnc = qtnc.Value;
                        $('#divtnc').html($scope.Renttnc);
                    }

                    var saleAddinfo = response.Data.find(o => o.SubCategory == 'sale' && o.Key == 'addInfo');

                    if (saleAddinfo) {
                        $scope.SaleAddInfo = $scope.Config.saleAddInfo = saleAddinfo.Value;
                        $('#divAddInfo').html($scope.SaleAddInfo);
                    }
                    var saletnc = response.Data.find(o => o.SubCategory == 'sale' && o.Key == 'tnc');

                    if (saletnc) {
                        $scope.Saletnc = $scope.Config.saletnc = saletnc.Value;
                        $('#divtnc').html($scope.Saletnc);
                    }

                    var contractAddInfo = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'addInfo');

                    if (contractAddInfo) {
                        $scope.ContractAddInfo = $scope.Config.contractAddInfo = contractAddInfo.Value;
                        $('#divAddInfo').html($scope.ContractAddInfo);
                    }
                    var contracttnc = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'tnc');

                    if (contracttnc) {
                        $scope.Contracttnc = $scope.Config.contracttnc = contracttnc.Value;
                        $('#divtnc').html($scope.Contracttnc);
                    }
                    var contractLineMode = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'contractLineTotalMode');
                    if (contractLineMode && contractLineMode.Value) {
                        $scope.Config.contractLineTotalMode = contractLineMode.Value === 'area' ? 'area' : 'quantity';
                    } else {
                        $scope.Config.contractLineTotalMode = 'quantity';
                    }
                    if (!$scope.Trans.QuotationId && $scope.Trans.QuotationType == 16) {
                        $scope.Trans.LineTotalMode = $scope.Config.contractLineTotalMode;
                    }
                    if ($scope.Trans.QuotationType == 16)
                        $scope.SubTotal(0);

                    var prefix = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'prefix');
                    if (prefix) {
                        $scope.Config.prefix = prefix.Value;
                    }
                    var numberstart = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'numberstart');
                    if (numberstart) {
                        $scope.Config.numberstart = numberstart.Value;
                    }
                    var editnumber = response.Data.find(o => o.SubCategory == mainSub && o.Key == 'editnumber');
                    if (editnumber) {
                        $scope.Config.editnumber = editnumber.Value == 'true';
                    }
                    //var outwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'outwardfreight');
                    //if (outwardfreight) {
                    //    $scope.Config.outwardfreight = outwardfreight.Value == 'true';
                    //}
                    //var inwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'inwardfreight');
                    //if (inwardfreight) {
                    //    $scope.Config.inwardfreight = inwardfreight.Value == 'true';
                    //}
                    $scope.setAddInfoAndTnc();
                }

            }
            FormsValidation.init('frmQuotation');
        }, configCategory);
    }
    $scope.getConfig();
    var onLedgerAdded = $rootScope.$on("OnLedgerAdded", function (evt, data) {
        if (data && data.quotationIdForLink) {
            return;
        }
        LedgerFactory.GetAllParties(function (e) {

            $scope.SundryDebtors = e.data;
            var newLedger = e.data.find(x => x.Code == data.Code);
            $scope.Trans.LedgerId = newLedger.LedgerId;
        });
    });
    $scope.NewLedger = function () {

        ModalFactory.AddEditLedger('AddEditLedgerController', {
            LedgerId: 0, forQuotation: 1
        });
    }

    $scope.OnLossQtyChange = function (item) {

    }
});
app.controller('QuotationEditController', function ($scope, $rootScope, $stateParams, $state, $http, $crypto,
    $uibModal, LedgerFactory, AuthenticationService) {

    $scope._stateData = $state.current.data;

    if ($stateParams.key == undefined) {
        window.histor.go(-1);
        return;
    }
    var quoteId = $crypto.decrypt($stateParams.key);
    var sale = new $.Transaction({ PurchaseId: 0, QuotationId: quoteId });
    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var config = new $.Config();
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.Trans.Category = $scope._stateData.category;
    $scope.Trans.PartyType = 1;
    $scope.Trans.UnregisteredPartyName = '';
    $scope.Trans.UnregisteredPartyAddress = '';
    $scope.Trans.UnregisteredPartyPhone = '';
    $scope.Trans.GstRate = 0;
    $scope.Trans.IGST = false;
    $scope.Trans.CGST = false;
    $scope.Trans.SGST = false;
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.Charge5 = 0;
    $scope.Trans.Items = [];//initializeArray();
    $scope.Trans.FreightTaxRate = 0;
    $scope.lineQtyArea = function (item) {
        return contractLineQtyArea(item.Quantity, item.Area, $scope.Trans.Area);
    };
    $scope.linePeriodDaysLabel = function (item) {
        var d = contractLinePeriodDaysInclusive(item.From, item.To);
        return d == null ? '—' : String(d);
    };
    $scope.totalContractQtyArea = function () {
        return totalContractQtyArea($scope.Trans.Items, $scope.Trans.Area);
    };
    $scope.lineItemGstPercentLabel = lineItemGstPercentLabel;
    $scope.quotationSummaryGstRate = function (component) {
        return quotationSummaryGstRate($scope.Trans.Items, component);
    };
    $scope.ApplyGST = false;
    $scope.ApplyFreightGST = false;
    $scope.ApplyOtherChargeGST = false;


    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    //LedgerFactory.GetAccountsByGroup(function (e) {
    //    $scope.Banks = e.data;

    //}, { AccountGroupId: Enums.PURCHASE_ACCOUNT });


    //select default ledger if it selected on some other screen

    //if ($rootScope.LedgerId) {
    //    $scope.Trans.LedgerId = $rootScope.LedgerId;
    //}
    $scope.$watch('Trans.LedgerId', function () {
        if ($scope.Trans.LedgerId == 0) return;
        $scope.getSites();
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        if (!$scope.SundryDebtors) return;
        var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode
            $scope.SelectedLedger = ledger;
        }
        
    });
    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();
    $scope.getSites = function () {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }

    $scope.Find = function () {

    }




    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        console.log(selected);
    };

    $scope.RowSelected = function (index) {
        if (!$scope.Trans.Items) {
            return;
        }
        if ($scope.Trans.Items[index] != undefined) {
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



    $scope.AppliedTaxes = $scope.Trans.AppliedTaxes = [];
    $scope.Trans.FreightTax = 0;

    var modal
    $scope.showTaxItems = function () {

        modal = $uibModal.open({
            windowClass: 'right',
            templateUrl: 'templ/taxItems.html',
            scope: $scope, //passed current scope to the modal
            size: 'lg'

        });

    }
    $scope.closeSliderModal = function () {

        modal.close({});
    }
    //-- taxes

    function init() {
        $scope.TransItem = new $.TransItem({ PurchaseRate: $scope.DefaultRate });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.PurchaseDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.Save = function () {
        var res = $scope.Trans.Items.filter(function (v) {
            if (v != undefined) {
                return v.ProductId > 0;
            }
            else
                return false;
        });

        var m = $('#frmQuotation').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }

        addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    function addWorkOrder(fileList) {

        var model = cloneObj($scope.Trans);
        var quto = new $.Transaction();
        var pt = parseInt(model.PartyType, 10) || 1;
        model.PartyType = pt;
        if (pt === 1) {
            if (!model.LedgerId || model.LedgerId <= 0) {
                alert('Please select the party');
                return;
            }
        } else {
            model.LedgerId = 0;
            model.LedgerSiteId = 0;
            var unm = (model.UnregisteredPartyName || '').trim();
            var uad = (model.UnregisteredPartyAddress || '').trim();
            if (!unm || !uad) {
                alert('Please enter party name and address');
                return;
            }
            model.UnregisteredPartyName = unm;
            model.UnregisteredPartyAddress = uad;
            model.UnregisteredPartyPhone = (model.UnregisteredPartyPhone || '').trim();
        }
        //if (!model.LedgerSiteId || model.LedgerSiteId <= 0) {
        //    alert('Please select the party site');
        //    return;
        //}

        //model.AddInfo = $scope.RentAddInfo;
        //model.Tnc = $scope.Renttnc;
        model.BillableItems = model.Items;
        if (!model.PoDate) {
            model.PoDate = '';
        }
        model.PoDate = formatdate(model.PoDate);
        var _poDate = isValidDate(new Date(model.PoDate));
        if (model.PoNumber && model.PoNumber != '' && !_poDate) {
            alert('Please enter a valid PO Date');
            return;
        }

        if ((!model.PoNumber || model.PoNumber == '') && _poDate) {
            alert('Please enter a PO Number');
            return;
        }
        model.LineTotalMode = normalizeLineTotalMode(getContractLineMode(model, $scope.Config));
        if (!prepareContractQuotationForSave(model, $scope.Config))
            return;

        if (!model.ValidUntil) {
            model.ValidUntil = '';
        }
        model.ValidUntil = formatdate(model.ValidUntil);
        model.QuotationDate = formatdate(model.QuotationDate);
        if (model.QuotationType != 16) {
            model.From = model.QuotationDate;
            model.To = model.QuotationDate;
            model.LineTotalMode = 'quantity';
        }
        if (model.BillableItems) {
            for (var li = 0; li < model.BillableItems.length; li++) {
                var bl = model.BillableItems[li];
                if (model.QuotationType != 15)
                    bl.Duration = 1;
                else if (!bl.Duration || bl.Duration <= 0)
                    bl.Duration = 1;
                if (model.QuotationType == 16) {
                    copyContractLineDefaultsFromHeader(bl, model);
                    var effBlEd = getEffectiveLineTotalMode(bl, model, $scope.Config);
                    bl.SubTotal = contractLineSubTotal(effBlEd, bl.Quantity, bl.Rate, bl.Area, model.Area, bl, model);
                }
            }
        }
        quto.SaveQuotation(function (e) {
            if (e.statusText == 'OK' || e.data == true) {

                alert('saved');
                $scope.warnOnLeave = false;

                if ($scope.fromContractInfoModal) {
                    $('#contractEditQuotationModal').modal('hide');
                } else if ($scope._stateData.category == 'pi') {
                    $state.go('pinvoices');
                } else {
                    $state.go('quotations');
                }
                //   $state.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        calculateDiscount();
        //$scope.SubTotal(0);
    });

    function calculateDiscount() {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        // $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountPercent) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }
    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);
    //add new item to be issued
    $scope.addItem = function () {
        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }
        if ($scope.Trans.QuotationType != 15)
            $scope.TransItem.Duration = 1;
        else if (!$scope.TransItem.Duration || $scope.TransItem.Duration <= 0)
            $scope.TransItem.Duration = 1;

        if ($scope.Trans.QuotationType == 16)
            copyContractLineDefaultsFromHeader($scope.TransItem, $scope.Trans);

        var itemExist = $scope.Trans.QuotationType == 16
            ? $scope.Trans.Items.find(o => contractLinesMatch(o, $scope.TransItem))
            : $scope.Trans.Items.find(o => o.ProductId == $scope.TransItem.ProductId);
        //if (itemExist) {
        //    itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt($scope.TransItem.Quantity);
        //    if ($scope.Trans.QuotationType == 16)
        //        itemExist.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode(itemExist, $scope.Trans, $scope.Config), itemExist.Quantity, itemExist.Rate, itemExist.Area, $scope.Trans.Area, itemExist, $scope.Trans);
        //    else
        //        itemExist.SubTotal = itemExist.Quantity * itemExist.Rate * quotationLineDurationFactor($scope.Trans.QuotationType, itemExist.Duration);
        //} else {

        if ($scope.Trans.QuotationType == 16)
            $scope.TransItem.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode($scope.TransItem, $scope.Trans, $scope.Config), $scope.TransItem.Quantity, $scope.TransItem.Rate, $scope.TransItem.Area, $scope.Trans.Area, $scope.TransItem, $scope.Trans);
        else
            $scope.TransItem.SubTotal = $scope.TransItem.Quantity * $scope.TransItem.Rate * quotationLineDurationFactor($scope.Trans.QuotationType, $scope.TransItem.Duration);
        $scope.Trans.Items.push($scope.TransItem);
        // }



        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }

    function applyDiscount(item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    function getAllProductSizesByCompany() {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;

        });
    }
    $scope.SubTotal = function (_total) {
        $scope.Trans.Total = 0;
        $scope.Trans.SubTotal = 0;
        $scope.Trans.DiscountAmount = 0;
        $scope.Trans.TaxAmount = 0;
        $scope.Trans.FreightTax = 0;
        $scope.Trans.ChargesTax = 0;
        $scope.Trans.OtherChargeAmount = 0;

        if ($scope.Trans.Items) {

            for (var i = 0; i < $scope.Trans.Items.length; i++) {
                var item = $scope.Trans.Items[i];
                if (item.Quantity != null) {
                    if ($scope.Trans.QuotationType == 16) {
                        item.QtyArea = contractLineQtyArea(item.Quantity, item.Area, $scope.Trans.Area);
                        debugger
                        var _daysEd = contractLinePeriodDaysInclusive(item.From, item.To);
                        item.Days = _daysEd == null ? 0 : _daysEd;
                        item.SubTotal = contractLineSubTotal(getEffectiveLineTotalMode(item, $scope.Trans, $scope.Config), item.Quantity, item.Rate, item.Area, $scope.Trans.Area, item, $scope.Trans);
                    } else {
                        item.SubTotal = parseFloat(item.Quantity) * parseFloat(item.Rate)
                            * quotationLineDurationFactor($scope.Trans.QuotationType, item.Duration);
                    }
                }
                applyDiscount(item);
                applyTaxRate(item.ProductId);
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.SubTotal, 0);


            //if ($scope.Trans.Items) {
            //    for (var i = 0; i < $scope.Trans.Items.length; i++) {
            //        var item = $scope.Trans.Items[i];
            //        applyDiscount(item);
            //        applyTaxRate(item.ProductId);
            //    }
            //}
            $scope.Trans.TaxAmount = $scope.Trans.Items.reduce(function (partialSum, a) {
                if (!a.IGST) {
                    a.IGST = 0;
                }
                if (!a.CGST) {
                    a.CGST = 0;
                }
                if (!a.SGST) {
                    a.SGST = 0;
                }
                return partialSum + a.IGST + a.CGST + a.SGST;
            }, 0)

        }
        // if ($scope.Trans.SubTotal == 0) {
        calculateDiscount();

        //}
        if ($scope.Config) {
            if ($scope.ApplyFreightGST == true) {
                $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.FreightTaxRate = 0;
            }
            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTaxRate = $scope.Config.FreightTax;
            } else {
                $scope.Trans.ChargesTaxRate = 0;
            }

            if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
                var freight = parseFloat($scope.Trans.Freight)
                $scope.Trans.FreightTax = (freight * $scope.Trans.FreightTaxRate) / 100
            }

            $scope.Trans.OtherChargeAmount = parseFloat($scope.Trans.Charge1) + parseFloat($scope.Trans.Charge2) + parseFloat($scope.Trans.Charge3)
                + parseFloat($scope.Trans.Charge4) + parseFloat($scope.Trans.Charge5);
            if ($scope.ApplyOtherChargeGST == true) {
                $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * $scope.Trans.ChargesTaxRate) / 100
            }
        }


        $scope.Trans.Total = $scope.Trans.SubTotal - parseInt($scope.Trans.DiscountAmount, 0) + $scope.Trans.TaxAmount
            + parseFloat($scope.Trans.Freight) + parseFloat($scope.Trans.FreightTax) +
            $scope.Trans.OtherChargeAmount + $scope.Trans.ChargesTax;

        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;

        }
    };


    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', editnumber: false,
        outwardfreight: true, inwardfreight: false, attachRateSheet: false, contractLineTotalMode: 'quantity'
    };
    $scope.GetBillingConfig = function () {
        config.GetBillingConfig(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                    var breakageBill = response.Data.find(o => o.Key == 'breakageBill');

                    if (freightTax) {

                        $scope.Config.FreightTax = $scope.Trans.FreightTaxRate = freightTax.Value
                        $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate = freightTax.Value;
                    }


                }
                // 
            }
        });
    }
    $scope.getQuotationById = function () {
        if (quoteId > 0) {
            var txn = new $.Transaction();
            txn.GetQutotationById(function (e) {

                if (e.data.Code != 200) {
                    alert(e.data.Message);
                    return;
                }
                $scope.Trans = e.data.Data;
                $scope.Trans.QuotationDate = convertDate($scope.Trans.QuotationDate);
                $scope.Trans.PoDate = convertDate($scope.Trans.PoDate);
                if ($scope.Trans.ValidUntil)
                    $scope.Trans.ValidUntil = convertDate($scope.Trans.ValidUntil);
                if ($scope.Trans.From)
                    $scope.Trans.From = convertDate($scope.Trans.From);
                if ($scope.Trans.To)
                    $scope.Trans.To = convertDate($scope.Trans.To);
                if (!$scope.Trans.MeasureType || parseInt($scope.Trans.MeasureType, 10) <= 0)
                    $scope.Trans.MeasureType = 1;
                $scope.Trans.LineTotalMode = normalizeLineTotalMode($scope.Trans.LineTotalMode || getContractLineMode($scope.Trans, $scope.Config));
                if ($scope.Trans.Items) {
                    for (var ix = 0; ix < $scope.Trans.Items.length; ix++) {
                        if ($scope.Trans.Items[ix].From)
                            $scope.Trans.Items[ix].From = convertDate($scope.Trans.Items[ix].From);
                        if ($scope.Trans.Items[ix].To)
                            $scope.Trans.Items[ix].To = convertDate($scope.Trans.Items[ix].To);
                    }
                }

                $scope.Trans.PartyType = $scope.Trans.LedgerId > 0 ? 1 : 2;

                $scope.Trans.IGST = !!$scope.Trans.IGST;
                $scope.Trans.CGST = !!$scope.Trans.CGST;
                $scope.Trans.SGST = !!$scope.Trans.SGST;
                if (!$scope.Trans.UnregisteredPartyName) {
                    $scope.Trans.UnregisteredPartyName = '';
                }
                if (!$scope.Trans.UnregisteredPartyAddress) {
                    $scope.Trans.UnregisteredPartyAddress = '';
                }
                if (!$scope.Trans.UnregisteredPartyPhone) {
                    $scope.Trans.UnregisteredPartyPhone = '';
                }

                //$scope.RentAddInfo = $scope.Trans.AddInfo;
                //$scope.Renttnc = $scope.Trans.Tnc;
                $scope.Trans.FreightTaxRate = 0;
                var taxGSTAmount = $scope.Trans.Items.reduce((partialSum, a) => partialSum + a.IGST + a.CGST + a.SGST, 0);
                if ($scope.Trans.PartyType == 2) {
                    $scope.ApplyGST = false;
                } else if (taxGSTAmount > 0) {
                    $scope.ApplyGST = true;
                }
                if ($scope.Trans.ChargesTaxRate > 0) {

                    $scope.ApplyOtherChargeGST = true;
                }

                if ($scope.Trans.FreightTax > 0) {
                    $scope.ApplyFreightGST = true;

                }
                $scope.GetBillingConfig();
                if ($scope.Trans.QuotationType == 16)
                    $scope.SubTotal(0);
            }, quoteId);
        }
    }


    $scope.DefaultRate = 0.0;

    $scope.$watch('Trans.QuotationType', function () {
        if ($scope.Trans.QuotationType == 16) {
            if (!$scope.Trans.LineTotalMode || ($scope.Trans.LineTotalMode !== 'area' && $scope.Trans.LineTotalMode !== 'quantity'))
                $scope.Trans.LineTotalMode = getContractLineMode($scope.Trans, $scope.Config);
        }
    }, true);
    $scope.$watch('Trans.LineTotalMode', function (n, o) {
        if (o === undefined || n === o || $scope.Trans.QuotationType != 16)
            return;
        $scope.Trans.LineTotalMode = normalizeLineTotalMode(n);
        $scope.SubTotal(0);
    });
    $scope.$watchGroup(['Trans.Area', 'Trans.From', 'Trans.To'], function () {
        if ($scope.Trans.QuotationType == 16)
            $scope.SubTotal(0);
    });

    $scope.$watch('ApplyFreightGST', function () {
        //  $scope.Config.FreightTax = 0;

        if ($scope.ApplyFreightGST == false) {
            $scope.Trans.FreightTaxRate = 0;
            //   $scope.Trans.FreightTax = 0;
        }
        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        }

        $scope.SubTotal(0);
    }, true);

    $scope.$watch('ApplyOtherChargeGST', function () {
        // $scope.Config.ChargesTaxRate = 0;
        //$scope.Trans.ChargesTaxRate = 0;
        if ($scope.ApplyOtherChargeGST == false) {
            $scope.Trans.ChargesTaxRate = 0;
            //$scope.Trans.FreightTax = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate;
        }
        $scope.SubTotal(0);
    }, true);

    $scope.$watch('ApplyGST', function () {

        $scope.SubTotal(0);

    }, true);

    $scope.$watch('Trans.PartyType', function (n, o) {
        if (o === undefined || n === o) return;
        if (n === 2) {
            $scope.Trans.LedgerId = 0;
            $scope.Trans.LedgerSiteId = 0;
            $scope.LedgerSites = [];
            $scope.ApplyGST = false;
        } else if (n === 1) {
            $scope.Trans.UnregisteredPartyName = '';
            $scope.Trans.UnregisteredPartyAddress = '';
            $scope.Trans.UnregisteredPartyPhone = '';
            $scope.Trans.GstRate = 0;
            //$scope.Trans.IGST = false;
            //$scope.Trans.CGST = false;
            //$scope.Trans.SGST = false;
            $scope.ApplyGST = true;
        }
        $scope.SubTotal(0);
    });

    $scope.$watchGroup(['Trans.GstRate', 'Trans.IGST', 'Trans.CGST', 'Trans.SGST'], function () {
        $scope.SubTotal(0);
    });

    function applyTaxRate(productId) {
        var taxes = StaicData.TAX_CATEGORY;
        var pt = parseInt($scope.Trans.PartyType, 10) || 1;
        var lineItems = $scope.Trans.Items.filter(o => o.ProductId == productId);
        if (!lineItems.length) {
            return;
        }

       // if (pt === 2) {
            var rate = parseFloat($scope.Trans.GstRate) || 0;
            for (var i = 0; i < lineItems.length; i++) {
                if (!lineItems[i].DiscountAmount) {
                    lineItems[i].DiscountAmount = 0;
                }
                var slab = quotationProductGstSlabForManualUnregistered($scope.AllSizes, lineItems[i].ProductId, taxes);
                if (!slab.eligible) {
                    lineItems[i].CGST = 0;
                    lineItems[i].SGST = 0;
                    lineItems[i].IGST = 0;
                    clearLineGstRates(lineItems[i]);
                    lineItems[i].TaxName = slab.tax ? slab.tax.TaxName : '';
                    continue;
                }
                var taxable = lineItems[i].SubTotal - lineItems[i].DiscountAmount;
                lineItems[i].CGST = 0;
                lineItems[i].SGST = 0;
                lineItems[i].IGST = 0;
                if (rate > 0) {
                    if ($scope.Trans.IGST) {
                        lineItems[i].IGST = taxable * rate / 100;
                    }
                    if ($scope.Trans.CGST) {
                        lineItems[i].CGST = taxable * rate / 100;
                    }
                    if ($scope.Trans.SGST) {
                        lineItems[i].SGST = taxable * rate / 100;
                    }
                }
                lineItems[i].TaxName = 'GST';
                setLineGstRatesUnregistered(lineItems[i], $scope.Trans);
            }
            return;
      //  }

        if ($scope.AllSizes) {
            var item = $scope.AllSizes.find(o => o.ProductId == productId);
            if (item) {
                var tax = taxes.find(o => o.TaxId == item.TaxCategoryId);
                var isIntraState = $scope.comp && ledgerDTO.Props && $scope.comp.StateCode == ledgerDTO.Props.StateCode;

                if (tax) {
                    for (var i = 0; i < lineItems.length; i++) {
                        if (!lineItems[i].DiscountAmount) {
                            lineItems[i].DiscountAmount = 0;
                        }
                        lineItems[i].CGST = 0;
                        lineItems[i].SGST = 0;
                        lineItems[i].IGST = 0;
                        setLineGstRatesFromTaxCategory(lineItems[i], tax, isIntraState);
                        lineItems[i].TaxName = tax.TaxName;
                        if ($scope.ApplyGST == true) {
                            if (isIntraState) {
                                lineItems[i].CGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.CGST / 100;
                                lineItems[i].SGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.SGST / 100;
                            }
                            else {
                                lineItems[i].IGST = (lineItems[i].SubTotal - lineItems[i].DiscountAmount) * tax.IGST / 100;
                            }
                        }
                    }
                } else {
                    for (var j = 0; j < lineItems.length; j++) {
                        clearLineGstRates(lineItems[j]);
                    }
                }
            }
        }

    }

    getAllProductSizesByCompany();
    FormsValidation.init('frmQuotation');
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

    var leder = new $.Ledger();
    leder.SearchClient(function (e) {


        $scope.SundryDebtors = e.data.items;
        $scope.getQuotationById();
        //   $scope.SundryDebtors.splice(0, 0, { LedgerId: -1, Name: 'Other' });
        init();

    }, { Page: 'quotation' });

    //LedgerFactory.GetAllParties(function (e) {
    //    $scope.SundryDebtors = e.data;
    //    $scope.getQuotationById();
    //    init();

    //});
    $scope.getConfig = function () {
        var config = new $.Config();
        var configCategory = ($scope._stateData && $scope._stateData.category === 'pi') ? 'pi' : 'quotation';
        var mainSub = configCategory === 'pi' ? 'pi' : 'quotation';
        config.GetByCategory(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    //var qAddInfo = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'addInfo');

                    //if (qAddInfo) {
                    //    $scope.RentAddInfo = qAddInfo.Value;
                    //    $('#divAddInfo').html($scope.RentAddInfo);
                    //}
                    //var qtnc = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'tnc');

                    //if (qtnc) {
                    //    $scope.Renttnc = qtnc.Value;
                    //    $('#divtnc').html($scope.Renttnc);
                    //}
                    var prefix = response.Data.find(o => o.SubCategory == mainSub && o.Category == configCategory && o.Key == 'prefix');
                    if (prefix) {
                        $scope.Config.prefix = prefix.Value;
                    }
                    var numberstart = response.Data.find(o => o.SubCategory == mainSub && o.Category == configCategory && o.Key == 'numberstart');
                    if (numberstart) {
                        $scope.Config.numberstart = numberstart.Value;
                    }
                    var editnumber = response.Data.find(o => o.SubCategory == mainSub && o.Category == configCategory && o.Key == 'editnumber');
                    if (editnumber) {
                        $scope.Config.editnumber = editnumber.Value == 'true';
                    }
                    var contractLineModeEdit = response.Data.find(o => o.SubCategory == 'contract' && o.Key == 'contractLineTotalMode');
                    if (contractLineModeEdit && contractLineModeEdit.Value) {
                        $scope.Config.contractLineTotalMode = contractLineModeEdit.Value === 'area' ? 'area' : 'quantity';
                    } else if (!$scope.Config.contractLineTotalMode) {
                        $scope.Config.contractLineTotalMode = 'quantity';
                    }
                    if ($scope.Trans && $scope.Trans.QuotationType == 16) {
                        if (!$scope.Trans.LineTotalMode || ($scope.Trans.LineTotalMode !== 'area' && $scope.Trans.LineTotalMode !== 'quantity'))
                            $scope.Trans.LineTotalMode = $scope.Config.contractLineTotalMode;
                        $scope.SubTotal(0);
                    }
                    //var outwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'outwardfreight');
                    //if (outwardfreight) {
                    //    $scope.Config.outwardfreight = outwardfreight.Value == 'true';
                    //}
                    //var inwardfreight = response.Data.find(o => o.SubCategory == 'quotation' && o.Category == 'quotation' && o.Key == 'inwardfreight');
                    //if (inwardfreight) {
                    //    $scope.Config.inwardfreight = inwardfreight.Value == 'true';
                    //}

                }

            }
        }, configCategory);
    }
    $scope.getConfig();
});
