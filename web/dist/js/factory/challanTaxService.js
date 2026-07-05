/**
 * Tax calculation from preloaded tax categories (issue challan, quotation, etc.).
 */
app.factory('ChallanTaxService', [function () {

    function normalizeCode(code) {
        return (code || '').toString().trim().toUpperCase();
    }

    function calculateTaxAmount(taxableAmount, rate, rateType) {
        var base = parseFloat(taxableAmount) || 0;
        var taxRate = parseFloat(rate) || 0;
        if (base <= 0 || taxRate <= 0) {
            return 0;
        }
        if ((rateType || 'Percentage').toLowerCase() === 'fixed') {
            return taxRate;
        }
        return (base * taxRate) / 100;
    }

    function buildCategoryMap(taxCategories) {
        var map = {};
        (taxCategories || []).forEach(function (category) {
            map[category.TaxCategoryId] = category.Mappings || [];
        });
        return map;
    }

    function getLineCategoryId(item, allSizes) {
        if (item.TaxCategoryId) {
            return item.TaxCategoryId;
        }
        var product = (allSizes || []).find(function (p) {
            return p.ProductId === item.ProductId;
        });
        return product && product.TaxCategoryId ? product.TaxCategoryId : 0;
    }

    function getLineSubTotal(item, options) {
        if (options.getTaxableAmount) {
            return options.getTaxableAmount(item);
        }
        return parseFloat(item.SubTotal) || 0;
    }

    function isActiveLine(item, options) {
        if (options.itemFilter) {
            return options.itemFilter(item);
        }
        return item.Deleted !== 1 && parseFloat(item.SentQty) > 0;
    }

    function buildLineTaxes(taxableAmount, taxCategoryId, categoryMappings) {
        var lineTaxes = [];

        (categoryMappings || []).forEach(function (mapping) {
            var amount = calculateTaxAmount(taxableAmount, mapping.Rate, mapping.RateType);
            if (amount <= 0) {
                return;
            }

            lineTaxes.push({
                TaxCategoryId: taxCategoryId,
                TaxId: mapping.TaxId,
                TaxName: mapping.TaxName || mapping.Name,
                TaxCode: mapping.TaxCode || mapping.Code,
                Rate: mapping.Rate,
                RateType: mapping.RateType || 'Percentage',
                Amount: amount
            });
        });

        return lineTaxes;
    }

    function getLineMappings(categoryId, categoryMap, manualTaxMappings) {
        if (manualTaxMappings && manualTaxMappings.length) {
            return manualTaxMappings;
        }
        return categoryMap[categoryId] || [];
    }

    function aggregateUniqueTaxes(allLineTaxes) {
        var taxMap = {};

        (allLineTaxes || []).forEach(function (lineTax) {
            var key = String(lineTax.TaxId);
            if (!taxMap[key]) {
                taxMap[key] = angular.copy(lineTax);
                taxMap[key].Amount = 0;
            }
            taxMap[key].Amount += parseFloat(lineTax.Amount) || 0;
        });

        return Object.keys(taxMap).map(function (key) {
            return taxMap[key];
        }).sort(function (a, b) {
            return (a.TaxName || '').localeCompare(b.TaxName || '');
        });
    }

    function buildLegacyTaxSummary(appliedTaxes) {
        var summary = {
            IGST: 0, IGSTAmount: 0,
            SGST: 0, SGSTAmount: 0,
            CGST: 0, CGSTAmount: 0
        };

        (appliedTaxes || []).forEach(function (tax) {
            var code = normalizeCode(tax.TaxCode);
            var amount = parseFloat(tax.Amount) || 0;
            var rate = parseFloat(tax.Rate) || 0;

            if (code === 'IGST') {
                summary.IGST = rate;
                summary.IGSTAmount += amount;
            } else if (code === 'CGST') {
                summary.CGST = rate;
                summary.CGSTAmount += amount;
            } else if (code === 'SGST') {
                summary.SGST = rate;
                summary.SGSTAmount += amount;
            }
        });

        return summary;
    }

    function calculateChallanTaxes(options) {
        options = options || {};
        var items = options.items || [];
        var allSizes = options.allSizes || [];
        var categoryMap = buildCategoryMap(options.taxCategories);
        var manualTaxMappings = options.manualTaxMappings || null;
        var applyGst = options.applyGst !== false;

        if (!applyGst || !items.length) {
            return {
                appliedTaxes: [],
                taxAmount: 0,
                legacyTaxes: buildLegacyTaxSummary([])
            };
        }

        var activeItems = items.filter(function (item) {
            return isActiveLine(item, options);
        });

        var subTotal = activeItems.reduce(function (sum, item) {
            return sum + getLineSubTotal(item, options);
        }, 0);

        var extraTaxable = 0;
        if (options.freightTax) {
            extraTaxable += parseFloat(options.freight) || 0;
        }
        if (options.otherChargesTax) {
            extraTaxable += parseFloat(options.otherCharges) || 0;
        }

        var allLineTaxes = [];
        activeItems.forEach(function (item) {
            var lineSubTotal = getLineSubTotal(item, options);
            var shareOfExtra = subTotal > 0 ? (lineSubTotal / subTotal) * extraTaxable : 0;
            var taxableAmount = lineSubTotal + shareOfExtra;

            var categoryId = getLineCategoryId(item, allSizes);
            item.TaxCategoryId = categoryId;
            var mappings = getLineMappings(categoryId, categoryMap, manualTaxMappings);
            item.LineTaxes = buildLineTaxes(taxableAmount, categoryId, mappings);
            allLineTaxes = allLineTaxes.concat(item.LineTaxes);
        });

        var appliedTaxes = aggregateUniqueTaxes(allLineTaxes);
        var taxAmount = appliedTaxes.reduce(function (sum, tax) {
            return sum + (parseFloat(tax.Amount) || 0);
        }, 0);

        return {
            appliedTaxes: appliedTaxes,
            taxAmount: taxAmount,
            legacyTaxes: buildLegacyTaxSummary(appliedTaxes)
        };
    }

    return {
        calculateChallanTaxes: calculateChallanTaxes,
        aggregateUniqueTaxes: aggregateUniqueTaxes,
        buildLegacyTaxSummary: buildLegacyTaxSummary
    };
}]);
