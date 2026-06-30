

/** Measure bill line area: UI uses Area + Unit; API persists Height/Width (Height=Area, Width=1 on save). */
function measureBillEffectiveArea(item) {
    if (!item) return 0;
    var a = parseFloat(item.Area);
    if (!isNaN(a) && a > 0) return a;
    var h = parseFloat(item.Height) || 0;
    var w = parseFloat(item.Width) || 0;
    return h * w;
}

function measureBillNormalizeItemsArea(items) {
    if (!items || !items.length) return;
    var defaultUnit = (StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT';
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        var eff = measureBillEffectiveArea(it);
        var cur = parseFloat(it.Area);
        if (isNaN(cur) || cur === 0) {
            if (eff > 0) it.Area = eff;
        }
        var u = it.Unit != null ? String(it.Unit).trim() : '';
        if (!u) it.Unit = defaultUnit;
    }
}

function measureBillApplyApiDimensionsForSave(items) {
    if (!items || !items.length) return;
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        var a = parseFloat(it.Area);
        if (!isNaN(a) && a > 0) {
            it.Height = a;
            it.Width = 1;
        }
    }
}

/** Inclusive rent days for contract measure bill quantity/rent mode (matches quotation contract helpers). */
function measureBillInclusiveRentDays(trans, item) {
    if (!item) return 1;
    var d = parseFloat(item.Days);
    if (!isNaN(d) && d >= 1) return d;
    if (item.From && item.To) {
        var d0 = new Date(item.From), d1 = new Date(item.To);
        if (isValidDate(d0) && isValidDate(d1)) {
            d0.setHours(0, 0, 0, 0);
            d1.setHours(0, 0, 0, 0);
            if (d1 >= d0) return Math.floor((d1 - d0) / 86400000) + 1;
        }
    }
    if (trans && trans.From && trans.To) {
        var f = new Date(trans.From), t = new Date(trans.To);
        if (isValidDate(f) && isValidDate(t)) {
            f.setHours(0, 0, 0, 0);
            t.setHours(0, 0, 0, 0);
            if (t >= f) return Math.floor((t - f) / 86400000) + 1;
        }
    }
    return 1;
}

/** Line subtotal for measure UI: contract bill (type 5 + quotation 16) matches server; quantity = Rent (Qty×Rate×Days). */
function measureBillLineSubTotal(trans, item) {
    var qty = parseFloat(item.Quantity) || 0;
    var rate = parseFloat(item.Rate) || 0;
    var effA = measureBillEffectiveArea(item);
    if (trans && trans.InvoiceType === 5 && trans.QuotationType === 16) {
        var mode = (trans.LineTotalMode || 'area').toLowerCase();
        if (mode === 'area') {
            var lineA = parseFloat(item.Area);
            if (isNaN(lineA) || lineA <= 0) {
                lineA = parseFloat(trans.Area) || 0;
            }
            if (lineA <= 0) {
                lineA = effA;
            }
            return qty * rate * lineA;
        }
        return qty * rate * measureBillInclusiveRentDays(trans, item);
    }
    return qty * rate * effA;
}

/** Contract measure bill (type 5 + quotation 16) may save a narrative line with ProductId 0 when no product is on the contract. */
function measureBillItemEligibleForSave(trans, item) {
    if (item == null || item === undefined) return false;
    if (item.ProductId > 0) return true;
    if (trans && trans.InvoiceType === 5 && trans.QuotationType === 16) {
        var text = String(item.Description || item.Item || item.Product || '').trim();
        var q = parseInt(item.Quantity, 10);
        return text.length > 0 && !isNaN(q) && q > 0;
    }
    return false;
}

/** First non-zero line rate for a GST component (same idea as quotationSummaryGstRate). */
function measureBillSummaryGstRate(items, component) {
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

/** Persist manual GST options at end of AddInfo (hidden from user after strip on load). */
var MEASURE_MANUAL_GST_RE = /\r?\n\[\[MEASURE_MANUAL_GST\]\]([\s\S]*?)\[\[\/MEASURE_MANUAL_GST\]\]\s*$/;

function measureBillStripManualGstFromAddInfo(addInfo) {
    var s = addInfo != null ? String(addInfo) : '';
    return s.replace(MEASURE_MANUAL_GST_RE, '').replace(/\s+$/, '');
}

function measureBillParseManualGstFromAddInfo(addInfo) {
    var s = addInfo != null ? String(addInfo) : '';
    var m = MEASURE_MANUAL_GST_RE.exec(s);
    if (!m || !m[1]) {
        return null;
    }
    try {
        return JSON.parse(m[1].trim());
    } catch (e) {
        return null;
    }
}

function measureBillMergeManualGstIntoAddInfo(addInfo, scope) {
    var clean = measureBillStripManualGstFromAddInfo(addInfo);
    var payload = {
        rate: parseFloat(scope.measureBillGstRate) || 0,
        igst: !!scope.measureBillGstApplyIgst,
        cgst: !!scope.measureBillGstApplyCgst,
        sgst: !!scope.measureBillGstApplySgst
    };
    return clean + '\n[[MEASURE_MANUAL_GST]]' + JSON.stringify(payload) + '[[/MEASURE_MANUAL_GST]]';
}

function measureBillApplyManualGstPayloadToScope(scope, data) {
    if (!data) {
        return;
    }
    scope.measureBillSuppressManualGstTouch = true;
    scope.measureBillGstRate = parseFloat(data.rate) || 0;
    scope.measureBillGstApplyIgst = !!data.igst;
    scope.measureBillGstApplyCgst = !!data.cgst;
    scope.measureBillGstApplySgst = !!data.sgst;
    scope.measureBillSuppressManualGstTouch = false;
}

function measureBillClearLineGstRates(line) {
    line.IGSTRate = 0;
    line.CGSTRate = 0;
    line.SGSTRate = 0;
    line.TaxRate = 0;
}

function measureBillSetLineGstRatesUnregistered(line, gstTrans) {
    measureBillClearLineGstRates(line);
    var rate = parseFloat(gstTrans.GstRate) || 0;
    if (rate <= 0) {
        return;
    }
    if (gstTrans.IGST) {
        line.IGSTRate = rate;
    }
    if (gstTrans.CGST) {
        line.CGSTRate = rate;
    }
    if (gstTrans.SGST) {
        line.SGSTRate = rate;
    }
    line.TaxRate = (line.IGSTRate || 0) + (line.CGSTRate || 0) + (line.SGSTRate || 0);
}

function measureBillProductGstSlabForManualUnregistered(allSizes, productId, taxes) {
    if (!allSizes || !taxes || !productId) {
        return { tax: null, eligible: false };
    }
    var item = allSizes.find(function (o) { return o.ProductId == productId; });
    if (!item) {
        return { tax: null, eligible: false };
    }
    var tax = taxes.find(function (o) { return o.TaxId == item.TaxCategoryId; });
    if (!tax) {
        return { tax: null, eligible: false };
    }
    var sum = (parseFloat(tax.CGST) || 0) + (parseFloat(tax.SGST) || 0) + (parseFloat(tax.IGST) || 0);
    return { tax: tax, eligible: sum > 0 };
}

function measureBillGstTransFromScope(scope) {
    return {
        GstRate: parseFloat(scope.measureBillGstRate) || 0,
        IGST: !!scope.measureBillGstApplyIgst,
        CGST: !!scope.measureBillGstApplyCgst,
        SGST: !!scope.measureBillGstApplySgst
    };
}

function measureBillApplyTaxRateForScope(scope, productId) {
    var taxes = StaicData.TAX_CATEGORY;
    var gstTrans = measureBillGstTransFromScope(scope);
    var rate = gstTrans.GstRate;
    var lineItems = scope.Trans.Items.filter(function (o) { return o.ProductId == productId; });
    if (!lineItems.length) {
        return;
    }
    for (var i = 0; i < lineItems.length; i++) {
        if (!lineItems[i].DiscountAmount) {
            lineItems[i].DiscountAmount = 0;
        }
        var slab = measureBillProductGstSlabForManualUnregistered(scope.AllSizes, productId, taxes);
        if (!slab.eligible) {
            lineItems[i].CGST = 0;
            lineItems[i].SGST = 0;
            lineItems[i].IGST = 0;
            measureBillClearLineGstRates(lineItems[i]);
            lineItems[i].TaxName = slab.tax ? slab.tax.TaxName : '';
            continue;
        }
        var taxable = lineItems[i].SubTotal - lineItems[i].DiscountAmount;
        lineItems[i].CGST = 0;
        lineItems[i].SGST = 0;
        lineItems[i].IGST = 0;
        if (rate > 0) {
            if (gstTrans.IGST) {
                lineItems[i].IGST = taxable * rate / 100;
            }
            if (gstTrans.CGST) {
                lineItems[i].CGST = taxable * rate / 100;
            }
            if (gstTrans.SGST) {
                lineItems[i].SGST = taxable * rate / 100;
            }
        }
        lineItems[i].TaxName = 'GST';
        measureBillSetLineGstRatesUnregistered(lineItems[i], gstTrans);
    }
}

function measureBillApplyTaxAllProductGroups(scope) {
    if (!scope.Trans.Items || !scope.Trans.Items.length) {
        return;
    }
    var seen = {};
    for (var i = 0; i < scope.Trans.Items.length; i++) {
        var pid = scope.Trans.Items[i].ProductId;
        if (seen[pid]) {
            continue;
        }
        seen[pid] = true;
        measureBillApplyTaxRateForScope(scope, pid);
    }
}

/** Default IGST/CGST/SGST + rate from first line product and company vs party state (no PartyType). */
function measureBillRefreshManualGstDefaultsFromMaster(scope, options) {
    options = options || {};
    if (scope.measureBillManualGstUserTouched && !options.force) {
        return;
    }
    if (!scope.comp || !scope.comp.StateCode) {
        return;
    }
    if (!scope.ledgerDTO || !scope.ledgerDTO.Props || scope.ledgerDTO.Props.StateCode == null || scope.ledgerDTO.Props.StateCode === '') {
        return;
    }
    if (!scope.AllSizes || !scope.Trans.Items || !scope.Trans.Items.length) {
        return;
    }
    var firstPid = 0;
    for (var i = 0; i < scope.Trans.Items.length; i++) {
        if (scope.Trans.Items[i].ProductId > 0) {
            firstPid = scope.Trans.Items[i].ProductId;
            break;
        }
    }
    var taxes = StaicData.TAX_CATEGORY;
    scope.measureBillSuppressManualGstTouch = true;
    if (!firstPid) {
        scope.measureBillGstRate = 0;
        scope.measureBillGstApplyIgst = false;
        scope.measureBillGstApplyCgst = false;
        scope.measureBillGstApplySgst = false;
        scope.measureBillSuppressManualGstTouch = false;
        return;
    }
    var item = scope.AllSizes.find(function (o) { return o.ProductId == firstPid; });
    if (!item) {
        scope.measureBillSuppressManualGstTouch = false;
        return;
    }
    var tax = taxes.find(function (o) { return o.TaxId == item.TaxCategoryId; });
    if (!tax) {
        scope.measureBillSuppressManualGstTouch = false;
        return;
    }
    var isIntra = scope.comp.StateCode == scope.ledgerDTO.Props.StateCode;
    if (isIntra) {
        scope.measureBillGstApplyIgst = false;
        scope.measureBillGstApplyCgst = true;
        scope.measureBillGstApplySgst = true;
        scope.measureBillGstRate = parseFloat(tax.CGST) || parseFloat(tax.SGST) || 0;
    } else {
        scope.measureBillGstApplyIgst = true;
        scope.measureBillGstApplyCgst = false;
        scope.measureBillGstApplySgst = false;
        scope.measureBillGstRate = parseFloat(tax.IGST) || 0;
    }
    scope.measureBillSuppressManualGstTouch = false;
}

/** When contract quotation has manual GST fields, use them as initial values. */
function measureBillTryApplyQuotationGstDefaults(scope, quotation) {
    if (!quotation) {
        return false;
    }
    var ig = quotation.IGST === true || quotation.IGST === 1;
    var cg = quotation.CGST === true || quotation.CGST === 1;
    var sg = quotation.SGST === true || quotation.SGST === 1;
    var has = (quotation.GstRate != null && parseFloat(quotation.GstRate) > 0) || ig || cg || sg;
    if (!has) {
        return false;
    }
    scope.measureBillSuppressManualGstTouch = true;
    scope.measureBillGstRate = parseFloat(quotation.GstRate) || 0;
    scope.measureBillGstApplyIgst = ig;
    scope.measureBillGstApplyCgst = cg;
    scope.measureBillGstApplySgst = sg;
    scope.measureBillSuppressManualGstTouch = false;
    scope.measureBillGstDefaultsApplied = true;
    return true;
}

/** Build contract bill lines from contract header when no quotation items exist. */
function measureBillBuildContractFallbackItems(cd, measureUnits) {
    var pid = 0;
    if (cd.Details && cd.Details.length) {
        for (var di = 0; di < cd.Details.length; di++) {
            if (cd.Details[di].ProductId > 0) {
                pid = cd.Details[di].ProductId;
                break;
            }
        }
    }
    var areaNum = parseFloat(cd.Area) || 0;
    var cv = parseFloat(cd.ContractValue) || 0;
    var title = (cd.Title || 'Contract').trim();
    var ltm = (cd.LineTotalMode && String(cd.LineTotalMode).trim().toLowerCase() === 'area') ? 'area' : 'quantity';
    var rateVal = (ltm === 'area')
        ? (areaNum > 0 ? (cv / areaNum) : cv)
        : cv;
    var mu = (StaicData.CONTRACT_MEASUREMENTS && cd.MeasureType &&
        StaicData.CONTRACT_MEASUREMENTS.filter(function (m) { return m.Id == cd.MeasureType; })[0]);
    var units = measureUnits || (StaicData.CONTRACT_MEASUREMENTS || []);
    return [{
        ProductId: pid,
        GroupItemId: 0,
        Quantity: 1,
        Rate: rateVal,
        Area: areaNum,
        Height: 0,
        Width: 0,
        TaxCategoryId: cd.TaxCategoryId || 0,
        IGST: 0,
        CGST: 0,
        SGST: 0,
        IGSTRate: 0,
        CGSTRate: 0,
        SGSTRate: 0,
        DiscountPercent: 0,
        Discount: 0,
        Item: title,
        Description: title,
        SubTotal: cv,
        Total: cv,
        ChallanId: 0,
        Unit: (mu && mu.Name) ? mu.Name : ((units[0] && units[0].Name) || 'SQFT')
    }];
}

function measureBillApplyContractHeaderToTrans(scope, cd) {
    scope.Trans.LedgerId = cd.Ledger.LedgerId;
    scope.Trans.LedgerSiteId = cd.LedgerSiteId;
    scope.Trans.InvoiceDate = convertDate(new Date());
    scope.Trans.QuotationType = 16;
    scope.Trans.Area = parseFloat(cd.Area) || 0;
    scope.Trans.MeasureType = cd.MeasureType != null ? cd.MeasureType : 1;
    scope.Trans.LineTotalMode = 'area';
    if (cd.LineTotalMode) {
        scope.Trans.LineTotalMode = cd.LineTotalMode;
    }
    scope.Trans.From = convertDate(cd.EffectiveFrom);
    scope.Trans.To = convertDate(cd.ValidTill);
}

/**
 * Load measure-bill lines from selected contract quotations (contractInfo picker).
 * quotationIds: array of QuotationId; empty uses contract-value fallback.
 */
function measureBillLoadContractQuotationItems(scope, contractId, quotationIds, done) {
    var contract = new $.Contract();
    contract.GetById(function (e) {
        if (!e.data || !e.data.Data) {
            if (done) done();
            return;
        }
        var cd = e.data.Data;
        scope.$evalAsync(function () {
            measureBillApplyContractHeaderToTrans(scope, cd);
            var ids = quotationIds || [];
            if (!ids.length) {
                scope.Trans.Items = measureBillBuildContractFallbackItems(cd, scope.MeasureUnits);
                measureBillTryInitGstDefaults(scope);
                if (scope.SubTotal) scope.SubTotal(0);
                if (done) done();
                return;
            }
            var txn = new $.Transaction();
            var loadedQuotations = [];
            var mergedItems = [];
            var pending = ids.length;
            var mergeMultiple = ids.length > 1;
            var primaryId = cd.QuotationId || 0;

            function finishLoad() {
                if (mergedItems.length) {
                    scope.Trans.Items = mergedItems;
                    measureBillNormalizeItemsArea(scope.Trans.Items);
                } else {
                    scope.Trans.Items = measureBillBuildContractFallbackItems(cd, scope.MeasureUnits);
                }
                var gstQuotation = null;
                if (primaryId > 0) {
                    for (var pi = 0; pi < loadedQuotations.length; pi++) {
                        if (loadedQuotations[pi].QuotationId === primaryId) {
                            gstQuotation = loadedQuotations[pi];
                            break;
                        }
                    }
                }
                if (!gstQuotation && loadedQuotations.length) {
                    gstQuotation = loadedQuotations[0];
                }
                if (gstQuotation) {
                    measureBillTryApplyQuotationGstDefaults(scope, gstQuotation);
                }
                measureBillTryInitGstDefaults(scope);
                if (scope.SubTotal) scope.SubTotal(0);
                if (done) done();
            }

            angular.forEach(ids, function (qid) {
                txn.GetQutotationById(function (resp) {
                    pending--;
                    if (resp.data && resp.data.Code == 200 && resp.data.Data) {
                        var q = resp.data.Data;
                        loadedQuotations.push(q);
                        var vch = q.QuotationNumber || ('Q' + q.QuotationId);
                        if (q.Items && q.Items.length) {
                            angular.forEach(q.Items, function (value) {
                                var row = angular.copy(value);
                                row.AddInfo = '';
                                row.Tnc = '';
                                var desc = (row.Description || row.Item || row.Product || '').trim();
                                if (mergeMultiple && desc) {
                                    desc = '[' + vch + '] ' + desc;
                                }
                                if (desc) {
                                    row.Description = desc;
                                }
                                mergedItems.push(row);
                            });
                        }
                    }
                    if (pending === 0) {
                        finishLoad();
                    }
                }, qid);
            });
        });
    }, { ContractId: contractId });
}

function measureBillTryInitGstDefaults(scope) {
    if (scope.measureBillManualGstUserTouched || scope.measureBillGstDefaultsApplied) {
        return;
    }
    if (!scope.comp || !scope.comp.StateCode) {
        return;
    }
    if (!scope.ledgerDTO || !scope.ledgerDTO.Props || scope.ledgerDTO.Props.StateCode == null || scope.ledgerDTO.Props.StateCode === '') {
        return;
    }
    if (!scope.AllSizes || !scope.Trans.Items || !scope.Trans.Items.length) {
        return;
    }
    measureBillRefreshManualGstDefaultsFromMaster(scope, { force: true });
    scope.measureBillGstDefaultsApplied = true;
}

app.controller('EditMeasureBillController', function ($scope, $rootScope, $stateParams, $state, $crypto,
    $q, $uibModal, LedgerFactory, AuthenticationService) {


    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 9, InvoiceId: 0 });

    sale.InvoiceId = $stateParams.key == undefined ? 0 : $crypto.decrypt($stateParams.key);

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Trans = sale;
    $scope.MeasureUnits = StaicData.CONTRACT_MEASUREMENTS;
    $scope.Trans.Items = [];//initializeArray();
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.Charge5 = 0;
    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', allowEditBillNo: false,
        discount_type: 'invoicelevel', applyTaxOn: 'itemlevel', defaultTaxRate: 0, autoRoundOffTaxable: false
    };
    $scope.editorOptions = {
        height: 200
    };
    $scope.measureBillSummaryGstRate = function (component) {
        return measureBillSummaryGstRate($scope.Trans.Items, component);
    };
    $scope.measureBillGstRate = 0;
    $scope.measureBillGstApplyIgst = false;
    $scope.measureBillGstApplyCgst = false;
    $scope.measureBillGstApplySgst = false;
    $scope.measureBillManualGstUserTouched = false;
    $scope.measureBillGstDefaultsApplied = false;
    $scope.measureBillSuppressManualGstTouch = false;
    $scope.measureBillManualGstChanged = function () {
        if (!$scope.measureBillSuppressManualGstTouch) {
            $scope.measureBillManualGstUserTouched = true;
        }
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    };
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    //LedgerFactory.GetAccountsByGroup(function (e) {
    //    $scope.Banks = e.data;

    //}, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    LedgerFactory.GetAllParties(function (e) {

        $scope.SundryDebtors = e.data;
        $scope.getById();
    });
    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {
        if ($scope.Trans.LedgerId == 0) return;
        $rootScope.LedgerId = $scope.Trans.LedgerId;
        var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
        if (ledger) {
            ledgerDTO.Props.StateCode = ledger.StateCode
        }
        getSites();
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.getById = function () {

        var billing = new $.Billing();
        var config = new $.Config();
        var billConfig = config.GetBillingConfig(null, true);
        var billById = billing.ById(null, { InvoiceId: $scope.Trans.InvoiceId }, true);

        var allPromises = [billConfig, billById];
        $q.all(allPromises).then(function (responses) {
            var configResponse = responses[0];
            var billingResponse = responses[1];
            if (configResponse) {
                $scope.SetBillingConfig(configResponse.data);
            }
            if (billingResponse) {
                if (billingResponse.data.Code != 200) {
                    alert('Could not load invoice' + billingResponse.data.Message);
                    return;
                }
                $scope.Trans = billingResponse.data.Data;
                $scope.Trans.Items = billingResponse.data.Data.BillableItems;
                measureBillNormalizeItemsArea($scope.Trans.Items);
                $scope.Trans.InvoiceDate = convertDate(billingResponse.data.Data.InvoiceDate);

                if ($scope.Trans.Recurring == true) {
                    $scope.Trans.StartsOn = convertDate(billingResponse.data.Data.StartsOn);
                    $scope.Trans.EndsOn = convertDate(billingResponse.data.Data.EndsOn);
                }

                $scope.AddInfo = $scope.Trans.AddInfo;
                var mbGst = measureBillParseManualGstFromAddInfo($scope.Trans.AddInfo);
                if (mbGst) {
                    measureBillApplyManualGstPayloadToScope($scope, mbGst);
                    $scope.measureBillManualGstUserTouched = true;
                    $scope.measureBillGstDefaultsApplied = true;
                    $scope.Trans.AddInfo = measureBillStripManualGstFromAddInfo($scope.Trans.AddInfo);
                    $scope.AddInfo = $scope.Trans.AddInfo;
                }
                if ($scope.defaultBillTnc && (!$scope.Trans.Tnc || !String($scope.Trans.Tnc).trim())) {
                    $scope.Trans.Tnc = $scope.defaultBillTnc;
                }
                $scope.qtnc = $scope.Trans.Tnc;

                if ($scope.Trans.ChargesTax > 0) {

                    $scope.ApplyOtherChargeGST = true;
                }

                if ($scope.Trans.FreightTax > 0) {
                    $scope.ApplyFreightGST = true;

                }
                $scope.$evalAsync(function () {
                    if ($scope.SundryDebtors && $scope.Trans.LedgerId) {
                        var lg = $scope.SundryDebtors.find(function (o) { return o.LedgerId == $scope.Trans.LedgerId; });
                        if (lg) {
                            ledgerDTO.Props.StateCode = lg.StateCode;
                        }
                    }
                    if (!mbGst) {
                        measureBillTryInitGstDefaults($scope);
                    }
                    if ($scope.SubTotal) {
                        $scope.SubTotal(0);
                    }
                });
            }


        }).catch(function (error) {
            // Handle any errors that occurred in any of the requests
            console.error('One or more requests failed:', error);
        });
    }



    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        //console.log(selected);
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

    $scope.init = function () {
        $scope.TransItem = new $.TransItem();
        $scope.TransItem.Area = 0;
        $scope.TransItem.Unit = (StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT';
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.InvoiceDate = convertDate(new Date());
        // getNextWorkOrderNumber();
    }
    $scope.init();
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            return measureBillItemEligibleForSave($scope.Trans, v);
        });

        var m = $('#frmPurchase').valid();

        if (!m) {
            // alert('Please provide all values');
            return;
        }


        //   EnableToolbar(0);
        //   var reader = new FileReader();

        var fileList = [];


        if (res.length < 1) {
            alert('Please add items to save.');
            return;
        }
        for (var ai = 0; ai < res.length; ai++) {
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16) {
                var ltm = ($scope.Trans.LineTotalMode || 'quantity').toLowerCase();
                if (ltm === 'area' && !measureBillEffectiveArea(res[ai])) {
                    alert('Each line item must have Area greater than zero.');
                    return;
                }
            } else if (!measureBillEffectiveArea(res[ai])) {
                alert('Each line item must have Area greater than zero.');
                return;
            }
        }

        $scope.addWorkOrder(fileList);



    };

    function addFileToList(fileList, inputTypeId) {
        if ($('#' + inputTypeId)[0].files.length > 0) {
            fileList.push($('#' + inputTypeId)[0].files[0]);
        }
        return fileList;
    }

    $scope.addWorkOrder = function (fileList) {

        var model = cloneObj($scope.Trans);
        model.AddInfo = measureBillMergeManualGstIntoAddInfo(model.AddInfo || '', $scope);
        if (model.Items && model.Items.length) {
            if (!(model.InvoiceType === 5 && model.QuotationType === 16)) {
                measureBillApplyApiDimensionsForSave(model.Items);
            }
        }
        model.Tnc = htmlEncode(model.Tnc);
        if ($scope.Config.allowEditBillNo == true) {
            if (!model.InvoiceNumber) {
                alert('Please enter the invoice number.');
                return;
            }
            if (trimAll(model.InvoiceNumber).length <= 1) {
                alert('Please enter a valid invoice number.');
                return;
            }
        }
        if (model.Recurring == true) {

            model.StartsOn = formatdate(model.StartsOn);
            model.EndsOn = formatdate(model.EndsOn);
            var invDate = formatdate(model.InvoiceDate);
            if (dateDiff(model.StartsOn, model.EndsOn) < 1) {
                alert('Ends on date must be ahead of Starts on date');
                return;
            }
            if (dateDiff(invDate, model.StartsOn) < 0) {
                alert('Start on date must be ahead of invoice date');
                return;
            }
            if (!model.Iteration) {
                alert('Iteration must be selected');
                return;
            }
        }

        var txn = new $.Transaction();

        if (model.LedgerSiteId > 0) {
            if (model.ShipTo && model.ShipTo.length > 10) {
                alert('Either select site or enter custom ship to address, can not enter both');
                return;
            }
        }
        if (model.LedgerSiteId == 0 && model.ShipTo.length < 3) {
            alert('Either select shipping site or enter custom ship to address');
            return;
        }
        txn.SaveSales(function (e) {
            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                $state.go('billList');
                //   $state.reload();
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

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
        var contractQtyMode = $scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16 &&
            ($scope.Trans.LineTotalMode || 'quantity').toLowerCase() !== 'area';
        if (!contractQtyMode && (!parseFloat($scope.TransItem.Area) || parseFloat($scope.TransItem.Area) <= 0)) {
            alert("Please enter a valid Area greater than zero.");
            return;
        }
        var billItem = cloneObj($scope.TransItem);
        $scope.addItemToBill(billItem);




        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0, Area: 0, Unit: (StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT' });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
    }
    $scope.addItemToBill = function (billItem) {
        var itemExist = $scope.Trans.Items.find(o => o.ProductId == billItem.ProductId && o.Rate == billItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt(billItem.Quantity);
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16)
                itemExist.SubTotal = measureBillLineSubTotal($scope.Trans, itemExist);
            else
                itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16)
                billItem.SubTotal = measureBillLineSubTotal($scope.Trans, billItem);
            else
                billItem.SubTotal = billItem.Quantity * billItem.Rate;

            $scope.Trans.Items.push(billItem);
        }

        //if (billItem.BOM != null) {
        //    $.each(billItem.BOM, function (index, value) {

        //        value.Quantity = value.Quantity * billItem.Quantity;
        //        value.Rate = 0;
        //        value.Item = value.Product;
        //        value.GroupItemId = billItem.ProductId;
        //        $scope.addItemToBill(value);
        //    });
        //}
    }
    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            //console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;
            measureBillTryInitGstDefaults($scope);
            if ($scope.SubTotal) {
                $scope.SubTotal(0);
            }

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

        if ($scope.Trans.Items && $scope.Trans.Items.length > 0) {
            var applyTaxOnSubtotal = (($scope.Config && $scope.Config.applyTaxOn || 'itemlevel') + '').toLowerCase() === 'subtotal';
            for (var i = 0; i < $scope.Trans.Items.length; i++) {
                var item = $scope.Trans.Items[i];
                if ($scope.Trans.Items[i].Quantity != null) {
                    $scope.Trans.Items[i].SubTotal = measureBillLineSubTotal($scope.Trans, $scope.Trans.Items[i]);
                }
                $scope.applyDiscount(item);
            }
            if (!applyTaxOnSubtotal) {
                measureBillApplyTaxAllProductGroups($scope);
            }
            if (typeof applySaleBillLineGst === 'function') {
                $.each($scope.Trans.Items, function (idx, val) {
                    val.Discount = parseFloat(val.DiscountAmount) || 0;
                });
                var isIntraState = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
                var gstCfg = $scope.Config;
                if (applyTaxOnSubtotal) {
                    measureBillApplyTaxAllProductGroups($scope);
                    gstCfg = angular.extend({}, $scope.Config, { applyTaxOn: 'itemlevel' });
                }
                applySaleBillLineGst($scope.Trans.Items, gstCfg, isIntraState);
            } else {
                measureBillApplyTaxAllProductGroups($scope);
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + a.SubTotal; }, 0);
            $scope.Trans.CGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.CGST) || 0); }, 0);
            $scope.Trans.SGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.SGST) || 0); }, 0);
            $scope.Trans.IGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.IGST) || 0); }, 0);
        } else {
            $scope.Trans.CGST = $scope.Trans.SGST = $scope.Trans.IGST = 0;
        }

        $scope.calculateDiscount();

        $scope.Trans.OtherChargeAmount = (parseFloat($scope.Trans.Charge1) || 0) + (parseFloat($scope.Trans.Charge2) || 0) + (parseFloat($scope.Trans.Charge3) || 0)
            + (parseFloat($scope.Trans.Charge4) || 0) + (parseFloat($scope.Trans.Charge5) || 0);

        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        } else {
            $scope.Trans.FreightTaxRate = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate || $scope.Config.FreightTax;
        } else {
            $scope.Trans.ChargesTaxRate = 0;
        }

        if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
            var freight = parseFloat($scope.Trans.Freight);
            $scope.Trans.FreightTax = (freight * parseFloat($scope.Trans.FreightTaxRate)) / 100;
        } else {
            $scope.Trans.FreightTax = 0;
        }

        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * parseFloat($scope.Trans.ChargesTaxRate)) / 100;
        } else {
            $scope.Trans.ChargesTax = 0;
        }


        var isIntraStateBill = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;

        if ($scope.Trans.IGST == 0 && ($scope.Trans.SGST > 0 || $scope.Trans.CGST > 0)) {
            isIntraStateBill = true;
        }


        if (typeof allocateFreightChargesToGst === 'function') {
            allocateFreightChargesToGst($scope.Trans, $scope.Trans.FreightTax, $scope.Trans.ChargesTax, isIntraStateBill);
        }

        $scope.Trans.TaxAmount = (parseFloat($scope.Trans.IGST) || 0) + (parseFloat($scope.Trans.SGST) || 0) + (parseFloat($scope.Trans.CGST) || 0);

        $scope.Trans.Total = (parseFloat($scope.Trans.SubTotal) || 0) - (parseFloat($scope.Trans.DiscountAmount) || 0) + $scope.Trans.TaxAmount
            + (parseFloat($scope.Trans.Freight) || 0) + $scope.Trans.OtherChargeAmount;

        if (typeof computeSaleBillTaxable === 'function') {
            $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
        }

        return $scope.Trans.SubTotal;

    };

    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.SalePrice;
            $scope.TransItem.TaxCategoryId = selected.originalObject.TaxCategoryId;
            var pu = selected.originalObject.Unit != null ? String(selected.originalObject.Unit).trim() : '';
            $scope.TransItem.Unit = pu || ((StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT');

            $scope.getBomItems($scope.TransItem.ProductId);
        }
    };

    $scope.getBomItems = function (productId) {
        var p = new $.Product();
        p.BOMByProductId(function (e) {
            if (e.data.Code != 200) {
                alert('Could not featch BOM details');
                return;
            }
            $scope.TransItem.BOM = e.data.Data.Details;
        }, { ProductId: productId });
    }
    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {
        measureBillApplyTaxRateForScope($scope, productId);
    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');

    $scope.getInfo = function () {

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
            measureBillTryInitGstDefaults($scope);
            if ($scope.Trans && $scope.Trans.Items && $scope.Trans.Items.length && $scope.SubTotal) {
                $scope.SubTotal(0);
            }

        });
    }
    $scope.getInfo();

    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);

    $scope.$watchGroup(['Trans.Freight', 'Trans.Charge1', 'Trans.Charge2', 'Trans.Charge3', 'Trans.Charge4', 'Trans.Charge5'], function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });

    $scope.$watchGroup(['measureBillGstRate', 'measureBillGstApplyIgst', 'measureBillGstApplyCgst', 'measureBillGstApplySgst', 'Trans.LedgerId'], function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });

    $scope.SetBillingConfig = function (response) {
        if (response.Data != null && response.Data) {
            if (response.Data.length > 0) {

                var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');
                var tnc = response.Data.find(o => o.Key == 'tnc' && o.SubCategory == 'Other' && o.Category == 'Invoice');
                var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');

                if (freightTax) {
                    $scope.Config.FreightTax = freightTax.Value;
                    $scope.Config.ChargesTaxRate = freightTax.Value;
                    $scope.Trans.FreightTaxRate = freightTax.Value;
                }
                if (allowEditBillNo) {
                    $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                }
                if (discount_type) {
                    $scope.Config.discount_type = discount_type.Value;
                }
                if (typeof loadBillingTaxConfigExtras === 'function') {
                    loadBillingTaxConfigExtras(response, $scope.Config);
                }
                if (tnc && tnc.Value) {
                    $scope.defaultBillTnc = tnc.Value;
                    if (!$scope.Trans.InvoiceId || $scope.Trans.InvoiceId === 0) {
                        $scope.Trans.Tnc = tnc.Value;
                    }
                }
            }
            // 
        }

    }
});
app.controller('MeasureBillController', function ($scope, $rootScope, $stateParams, $state, $stateParams, $crypto, $uibModal, LedgerFactory, AuthenticationService) {
    debugger
    var sale = new $.Transaction({ PurchaseId: 0, InvoiceType: 9 });
    var cId = $stateParams.cId == undefined ? 0 : $stateParams.cId;
    $scope.contractId = 0;

    $scope.Trans = sale;
    $scope.MeasureUnits = StaicData.CONTRACT_MEASUREMENTS;
    $scope.Trans.Charge1 = 0;
    $scope.Trans.Charge2 = 0;
    $scope.Trans.Charge3 = 0;
    $scope.Trans.Charge4 = 0;
    $scope.Trans.Charge5 = 0;
    $scope.Trans.Items = [];
    $scope.measureBillSummaryGstRate = function (component) {
        return measureBillSummaryGstRate($scope.Trans.Items, component);
    };
    $scope.measureBillGstRate = 0;
    $scope.measureBillGstApplyIgst = false;
    $scope.measureBillGstApplyCgst = false;
    $scope.measureBillGstApplySgst = false;
    $scope.measureBillManualGstUserTouched = false;
    $scope.measureBillGstDefaultsApplied = false;
    $scope.measureBillSuppressManualGstTouch = false;
    $scope.measureBillManualGstChanged = function () {

        if (!$scope.measureBillSuppressManualGstTouch) {
            $scope.measureBillManualGstUserTouched = true;
        }
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    };

    if (cId) {
        var contractId = $scope.contractId = $crypto.decrypt(cId);
        $scope.Trans.InvoiceType = 5;
        $scope.Trans.ContractId = contractId;
        $scope.Trans.QuotationType = 16;

        if ($scope.fromContractInfoModal) {
            var quoteIds = ($scope.billQuotationIds && $scope.billQuotationIds.length)
                ? angular.copy($scope.billQuotationIds)
                : [];
            measureBillLoadContractQuotationItems($scope, contractId, quoteIds);
        } else {
            var contract = new $.Contract();
            contract.GetById(function (e) {
                if (!e.data || !e.data.Data) {
                    return;
                }
                var cd = e.data.Data;
                $scope.$evalAsync(function () {
                    measureBillApplyContractHeaderToTrans($scope, cd);
                    if (cd.Quotation && cd.Quotation.Items && cd.Quotation.Items.length) {
                        $scope.Trans.Items = angular.copy(cd.Quotation.Items);
                        angular.forEach($scope.Trans.Items, function (value) {
                            value.AddInfo = '';
                            value.Tnc = '';
                            var desc = (value.Description || value.Item || value.Product || '').trim();
                            if (desc) {
                                value.Description = desc;
                            }
                        });
                        measureBillNormalizeItemsArea($scope.Trans.Items);
                    } else {
                        $scope.Trans.Items = measureBillBuildContractFallbackItems(cd, $scope.MeasureUnits);
                    }
                    if (cd.Quotation) {
                        measureBillTryApplyQuotationGstDefaults($scope, cd.Quotation);
                    }
                    measureBillTryInitGstDefaults($scope);
                    if ($scope.SubTotal) {
                        $scope.SubTotal(0);
                    }
                });
            }, { ContractId: contractId });
        }
    }

    var ledgerDTO = new $.Ledger({ LedgerId: 0 });
    var accGroup = new $.AccountGroup({});
    var loginData = AuthenticationService.getTokenInfo();
    $scope.Config = {
        FreightTax: 0, ChargesTaxRate: 0, prefix: '', numberstart: '', allowEditBillNo: false,
        discount_type: 'invoicelevel', applyTaxOn: 'itemlevel', defaultTaxRate: 0, autoRoundOffTaxable: false
    };
    $scope.editorOptions = {
        height: 200
    };

    var quoteId = 0;
    if ($stateParams.qId) {
        quoteId = $crypto.decrypt($stateParams.qId);
    }
    //debugger
    $scope.onCancelled = function () {
      //  debugger
        if ($scope.fromContractInfoModal) {
            $('#contractMeasureBillModal').modal('hide');
        }
        else {
            $state.go('billList');
        }
    }
    $scope.loadQuotation = function (intQuoteId) {

        var txn = new $.Transaction();
        txn.GetQutotationById(function (e) {

            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Trans = e.data.Data;
            $scope.Trans.InvoiceDate = convertDate($scope.Trans.QuotationDate);
            $scope.Trans.InvoiceType = sale.InvoiceType;

            $.each($scope.Trans.Items, function (index, value) {

                value.AddInfo = "";
                value.Tnc = "";
            });
            measureBillNormalizeItemsArea($scope.Trans.Items);

            $scope.Trans.AddInfo = "";
            $scope.Trans.Tnc = "";
            debugger
            measureBillTryApplyQuotationGstDefaults($scope, $scope.Trans);
            measureBillTryInitGstDefaults($scope);
            if ($scope.SubTotal) {
                $scope.SubTotal(0);
            }

        }, intQuoteId);
    };
    if (quoteId > 0) {
        $scope.loadQuotation(quoteId);
    }
    $scope.DeleteItem = function (index) {

        $scope.$apply(function () {
            $scope.Trans.Items.splice(index - 1, 1);
        });
    }
    //LedgerFactory.GetAccountsByGroup(function (e) {
    //    $scope.Banks = e.data;

    //}, { AccountGroupId: Enums.PURCHASE_ACCOUNT });
    LedgerFactory.GetAllParties(function (e) {

        $scope.SundryDebtors = e.data;

    });
    function getSites() {
        LedgerFactory.GetMasterSites(function (e) {

            $scope.LedgerSites = e.data.Data;
        }, { LedgerId: $scope.Trans.LedgerId });
    }
    //select default ledger if it selected on some other screen

    if ($rootScope.LedgerId) {
        $scope.Trans.LedgerId = $rootScope.LedgerId;
    }
    $scope.$watch('Trans.LedgerId', function () {

        $rootScope.LedgerId = $scope.Trans.LedgerId;
        if ($scope.SundryDebtors) {
            var ledger = $scope.SundryDebtors.find(o => o.LedgerId == $scope.Trans.LedgerId);
            if (ledger) {
                ledgerDTO.Props.StateCode = ledger.StateCode
            }
        }
        getSites();
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });

    //  $scope.WorkOrder.SiteInfo = new $.Site({ WorkOrderId: $scope.WorkOrderId, SiteId: sId });
    var selectedWorkOrderItemIndex = -1;
    FormsValidation.init();
    //GetTaxes();


    $scope.Find = function () {

    }

    $scope.focusIn = function (selected) {
        // $scope.Product = this.item.Props.Item.Props;
        // console.log(selected);
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

        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0 });
        //   $scope.Trans = new $.LedgerTrasaction({});
        $scope.TransItem.Area = 0;
        $scope.TransItem.Unit = (StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT';

        $scope.Trans.EntryType = $scope.TranType == 1 ? 8 : 9;
        $scope.Trans.InvoiceDate = convertDate(new Date());
        $scope.Trans.Recurring = false;
        $scope.Trans.StartsOn = convertDate(new Date());
        $scope.Trans.EndsOn = convertDate(addMonths(new Date(), 6));
        $scope.Trans.Iteration = 'monthly';
        // getNextWorkOrderNumber();
    }
    init();
    $scope.Save = function () {

        var res = $scope.Trans.Items.filter(function (v) {
            return measureBillItemEligibleForSave($scope.Trans, v);
        });

        var m = $('#frmPurchase').valid();

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
        for (var ai = 0; ai < res.length; ai++) {
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16) {
                var ltm = ($scope.Trans.LineTotalMode || 'quantity').toLowerCase();
                if (ltm === 'area' && !measureBillEffectiveArea(res[ai])) {
                    alert('Each line item must have Area greater than zero.');
                    return;
                }
            } else if (!measureBillEffectiveArea(res[ai])) {
                alert('Each line item must have Area greater than zero.');
                return;
            }
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
        model.AddInfo = measureBillMergeManualGstIntoAddInfo(model.AddInfo || '', $scope);
        if (model.Items && model.Items.length) {
            if (!(model.InvoiceType === 5 && model.QuotationType === 16)) {
                measureBillApplyApiDimensionsForSave(model.Items);
            }
        }
        model.Tnc = htmlEncode(model.Tnc);
        if ($scope.Config.allowEditBillNo == true) {
            if (!model.InvoiceNumber) {
                alert('Please enter the invoice number.');
                return;
            }
            if (trimAll(model.InvoiceNumber).length <= 1) {
                alert('Please enter a valid invoice number.');
                return;
            }
        }
        if (model.Recurring == true) {
            var cDate = new Date();
            model.StartsOn = formatdate(model.StartsOn);
            model.EndsOn = formatdate(model.EndsOn);
            var invDate = formatdate(model.InvoiceDate);
            if (dateDiff(model.StartsOn, model.EndsOn) < 1) {
                alert('Ends on date must be ahead of Starts on date');
                return;
            }
            if (dateDiff(invDate, model.StartsOn) < 1) {
                alert('Start on date must be ahead of today');
                return;
            }
            if (!model.Iteration) {
                alert('Iteration must be selected');
                return;
            }
        }
        var txn = new $.Transaction();
        if (model.LedgerSiteId > 0) {
            if (model.ShipTo && model.ShipTo.length > 10) {
                alert('Either select site or enter custom ship to address, can not enter both');
                return;
            }
        }
        if (model.LedgerSiteId == 0 && model.ShipTo.length < 3) {
            alert('Either select shipping site or enter custom ship to address');
            return;
        }
        if ($scope.fromContractInfoModal && $scope.billQuotationIds && $scope.billQuotationIds.length) {
            model.BillQuotationIds = angular.copy($scope.billQuotationIds);
        }
        txn.SaveSales(function (e) {
            if (e.status == 200) {

                alert('saved');
                $scope.warnOnLeave = false;
                if ($scope.fromContractInfoModal) {
                    $('#contractMeasureBillModal').modal('hide');
                } else {
                    $state.go('billList');
                }
            } else {
                showMessage(MessageClass.SAVED);
            }

        }, model, fileList);
    }
    $scope.$watch('Trans.DiscountValue', function () {

        $scope.calculateDiscount();
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });

    $scope.calculateDiscount = function () {
        //if ($scope.Trans.DiscountPercent && $scope.Trans.DiscountPercent > 100) {
        //    alert('Discount can not be more than 100%');
        //   $scope.Trans.DiscountValue = 0;
        //    return;
        //}
        $scope.Trans.DiscountPercent = $scope.Trans.DiscountValue;
        $scope.Trans.DiscountAmount = ($scope.Trans.SubTotal * $scope.Trans.DiscountValue) / 100;
        if ($scope.Trans.DiscountAmount == 0) {
            $scope.Trans.DiscountValue = 0;
        }
        if (!$scope.Trans.DiscountAmount) {
            $scope.Trans.DiscountAmount = 0;
        }
    }

    //add new item to be issued
    $scope.addItem = function ($event) {

        if (!$scope.TransItem.ProductId || $scope.TransItem.ProductId <= 0 || $scope.TransItem.Quantity == 0 || !$scope.TransItem.Rate) {
            alert("Please provide all details.");
            return;
        }

        if ($scope.TransItem.Rate <= 0) {
            alert("Rate can't be 0 or less.");
            return;
        }
        var contractQtyMode = $scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16 &&
            ($scope.Trans.LineTotalMode || 'quantity').toLowerCase() !== 'area';
        if (!contractQtyMode && (!parseFloat($scope.TransItem.Area) || parseFloat($scope.TransItem.Area) <= 0)) {
            alert("Please enter a valid Area greater than zero.");
            return;
        }
        var billItem = cloneObj($scope.TransItem);
        $scope.addItemToBill(billItem);




        $scope.TransItem = new $.TransItem({ Rate: $scope.DefaultRate, TaxCategoryId: 0, Area: 0, Unit: (StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT' });
        //$scope.SubTotal(0);
        $('#itemSelect_value').focus();
        $('#itemSelect_value').val('');
        $event.stoppropagation();
    }
    $scope.addItemToBill = function (billItem) {
        var itemExist = $scope.Trans.Items.find(o => o.ProductId == billItem.ProductId && o.Rate == billItem.Rate);
        if (itemExist) {
            itemExist.Quantity = parseInt(itemExist.Quantity) + parseInt(billItem.Quantity);
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16)
                itemExist.SubTotal = measureBillLineSubTotal($scope.Trans, itemExist);
            else
                itemExist.SubTotal = itemExist.Quantity * itemExist.Rate;

        } else {
            if ($scope.Trans.InvoiceType === 5 && $scope.Trans.QuotationType === 16)
                billItem.SubTotal = measureBillLineSubTotal($scope.Trans, billItem);
            else
                billItem.SubTotal = billItem.Quantity * billItem.Rate;

            $scope.Trans.Items.push(billItem);
        }

        //if (billItem.BOM != null) {
        //    $.each(billItem.BOM, function (index, value) {

        //        value.Quantity = value.Quantity * billItem.Quantity;
        //        value.Rate = 0;
        //        value.Item = value.Product;
        //        value.GroupItemId = billItem.ProductId;
        //        $scope.addItemToBill(value);
        //    });
        //}
    }
    $scope.applyDiscount = function (item) {
        item.DiscountPercent = $scope.Trans.DiscountValue;
        item.DiscountAmount = (item.SubTotal * item.DiscountPercent) / 100;

    }
    $scope.getAllProductSizesByCompany = function () {
        var product = new $.Product();
        product.GetAll(function (e) {
            //debugger
            // console.log('AllSizes', e.data);
            $scope.AllSizes = e.data;
            measureBillTryInitGstDefaults($scope);
            if ($scope.SubTotal) {
                $scope.SubTotal(0);
            }

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

        if ($scope.Trans.Items && $scope.Trans.Items.length > 0) {
            var applyTaxOnSubtotal = (($scope.Config && $scope.Config.applyTaxOn || 'itemlevel') + '').toLowerCase() === 'subtotal';
            for (var i = 0; i < $scope.Trans.Items.length; i++) {
                var item = $scope.Trans.Items[i];
                if ($scope.Trans.Items[i].Quantity != null) {
                    $scope.Trans.Items[i].SubTotal = measureBillLineSubTotal($scope.Trans, $scope.Trans.Items[i]);
                }
                $scope.applyDiscount(item);
            }

            if (!applyTaxOnSubtotal) {
                measureBillApplyTaxAllProductGroups($scope);
            }
            if (typeof applySaleBillLineGst === 'function') {
                $.each($scope.Trans.Items, function (idx, val) {
                    val.Discount = parseFloat(val.DiscountAmount) || 0;
                });
                var isIntraState = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;
                var gstCfg = $scope.Config;
                if (applyTaxOnSubtotal) {
                    measureBillApplyTaxAllProductGroups($scope);
                    gstCfg = angular.extend({}, $scope.Config, { applyTaxOn: 'itemlevel' });
                }
                applySaleBillLineGst($scope.Trans.Items, gstCfg, isIntraState);
            } else {
                measureBillApplyTaxAllProductGroups($scope);
            }
            $scope.Trans.SubTotal = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + a.SubTotal; }, 0);
            $scope.Trans.CGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.CGST) || 0); }, 0);
            $scope.Trans.SGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.SGST) || 0); }, 0);
            $scope.Trans.IGST = $scope.Trans.Items.reduce(function (partialSum, a) { return partialSum + (parseFloat(a.IGST) || 0); }, 0);
        } else {
            $scope.Trans.CGST = $scope.Trans.SGST = $scope.Trans.IGST = 0;
        }

        $scope.calculateDiscount();

        $scope.Trans.OtherChargeAmount = (parseFloat($scope.Trans.Charge1) || 0) + (parseFloat($scope.Trans.Charge2) || 0) + (parseFloat($scope.Trans.Charge3) || 0)
            + (parseFloat($scope.Trans.Charge4) || 0) + (parseFloat($scope.Trans.Charge5) || 0);

        if ($scope.ApplyFreightGST == true) {
            $scope.Trans.FreightTaxRate = $scope.Config.FreightTax;
        } else {
            $scope.Trans.FreightTaxRate = 0;
        }
        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate || $scope.Config.FreightTax;
        } else {
            $scope.Trans.ChargesTaxRate = 0;
        }

        if ($scope.Trans.Freight && $scope.ApplyFreightGST == true) {
            var freight = parseFloat($scope.Trans.Freight);
            $scope.Trans.FreightTax = (freight * parseFloat($scope.Trans.FreightTaxRate)) / 100;
        } else {
            $scope.Trans.FreightTax = 0;
        }

        if ($scope.ApplyOtherChargeGST == true) {
            $scope.Trans.ChargesTax = ($scope.Trans.OtherChargeAmount * parseFloat($scope.Trans.ChargesTaxRate)) / 100;
        } else {
            $scope.Trans.ChargesTax = 0;
        }

        var isIntraStateBill = $scope.comp && ledgerDTO.Props.StateCode && $scope.comp.StateCode == ledgerDTO.Props.StateCode;

        if ($scope.Trans.IGST == 0 && ($scope.Trans.SGST > 0 || $scope.Trans.CGST > 0)) {
            isIntraStateBill = true;
        }

        if (typeof allocateFreightChargesToGst === 'function') {
            allocateFreightChargesToGst($scope.Trans, $scope.Trans.FreightTax, $scope.Trans.ChargesTax, isIntraStateBill);
        }

        $scope.Trans.TaxAmount = (parseFloat($scope.Trans.IGST) || 0) + (parseFloat($scope.Trans.SGST) || 0) + (parseFloat($scope.Trans.CGST) || 0);

        $scope.Trans.Total = (parseFloat($scope.Trans.SubTotal) || 0) - (parseFloat($scope.Trans.DiscountAmount) || 0) + $scope.Trans.TaxAmount
            + (parseFloat($scope.Trans.Freight) || 0) + $scope.Trans.OtherChargeAmount;

        if (typeof computeSaleBillTaxable === 'function') {
            $scope.Trans.Taxable = computeSaleBillTaxable($scope.Trans, $scope.Config);
        }

        return $scope.Trans.SubTotal;

    };
    $scope.selectedProduct = function (selected) {

        if (selected != undefined) {
            $scope.TransItem.ProductId = selected.originalObject.ProductId;//this.$parent.item.Props.Item.Props;
            $scope.TransItem.Item = selected.originalObject.Name;
            $scope.TransItem.Rate = selected.originalObject.SalePrice;
            $scope.TransItem.TaxCategoryId = selected.originalObject.TaxCategoryId;
            var pu = selected.originalObject.Unit != null ? String(selected.originalObject.Unit).trim() : '';
            $scope.TransItem.Unit = pu || ((StaicData.CONTRACT_MEASUREMENTS && StaicData.CONTRACT_MEASUREMENTS[0] && StaicData.CONTRACT_MEASUREMENTS[0].Name) || 'SQFT');

            $scope.getBomItems($scope.TransItem.ProductId);
        }
    };

    $scope.getBomItems = function (productId) {
        var p = new $.Product();
        p.BOMByProductId(function (e) {
            if (e.data.Code != 200) {
                alert('Could not featch BOM details');
                return;
            }
            $scope.TransItem.BOM = e.data.Data.Details;
        }, { ProductId: productId });
    }

    $scope.DefaultRate = 0.0;
    $scope.applyTaxRate = function (productId) {
        measureBillApplyTaxRateForScope($scope, productId);
    }

    $scope.getAllProductSizesByCompany();
    FormsValidation.init('frmPurchase');

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
            measureBillTryInitGstDefaults($scope);
            if ($scope.Trans && $scope.Trans.Items && $scope.Trans.Items.length && $scope.SubTotal) {
                $scope.SubTotal(0);
            }

        });
    }
    getInfo();

    $scope.$watch('Trans.Items', function () {

        $scope.SubTotal(0);
    }, true);
    $scope.$watchGroup(['Trans.Freight', 'Trans.Charge1', 'Trans.Charge2', 'Trans.Charge3', 'Trans.Charge4', 'Trans.Charge5'], function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.$watchGroup(['measureBillGstRate', 'measureBillGstApplyIgst', 'measureBillGstApplyCgst', 'measureBillGstApplySgst', 'Trans.LedgerId'], function () {
        if ($scope.SubTotal) {
            $scope.SubTotal(0);
        }
    });
    $scope.GetBillingConfig = function () {
        var config = new $.Config();
        config.GetBillingConfig(function (e) {

            var response = e.data;
            if (response.Data != null && response.Data) {
                if (response.Data.length > 0) {

                    var freightTax = response.Data.find(o => o.SubCategory == 'Tax' && o.Key == 'freightTax');
                    var allowEditBillNo = response.Data.find(o => o.Key == 'allowEditBillNo');
                    var tnc = response.Data.find(o => o.Key == 'tnc' && o.SubCategory == 'Other' && o.Category == 'Invoice');
                    var discount_type = response.Data.find(o => o.Key.toLowerCase() == 'discount_type');

                    if (freightTax) {

                        $scope.Config.FreightTax = freightTax.Value;
                        $scope.Config.ChargesTaxRate = freightTax.Value;
                        $scope.Trans.FreightTaxRate = freightTax.Value;

                    }
                    if (allowEditBillNo) {
                        $scope.Config.allowEditBillNo = allowEditBillNo.Value == 'true';
                    }
                    if (discount_type) {
                        $scope.Config.discount_type = discount_type.Value;
                    }
                    if (typeof loadBillingTaxConfigExtras === 'function') {
                        loadBillingTaxConfigExtras(response, $scope.Config);
                    }
                    if (tnc && tnc.Value && !quoteId && (!$scope.Trans.Tnc || !String($scope.Trans.Tnc).trim())) {
                        $scope.Trans.Tnc = tnc.Value;
                    }

                }
                // 
            }
        });
    }
    $scope.$watch('ApplyFreightGST', function () {
        //  $scope.Config.FreightTax = 0;
        //debugger
        if ($scope.ApplyFreightGST == false) {
            $scope.Trans.FreightTaxRate = 0;
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
            $scope.Trans.ChargesTaxRate = $scope.Config.ChargesTaxRate || $scope.Config.FreightTax;
        }
        $scope.SubTotal(0);
    }, true);
    $scope.GetBillingConfig();


});

