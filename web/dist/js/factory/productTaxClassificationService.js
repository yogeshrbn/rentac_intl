/**
 * Loads product tax classification (e.g. Rental) used on issue challan lines.
 */
app.factory('ProductTaxClassificationService', ['$http', '$q', function ($http, $q) {
    var rentalCategoryCache = {};
    var baseUrl = API_URL + '/ProductTaxClassification';
    function getRentalTaxCategoryId(productId) {
        if (!productId) {
            return $q.resolve(0);
        }

        if (rentalCategoryCache[productId]) {
            return $q.resolve(rentalCategoryCache[productId]);
        }

        return $http.get(baseUrl + '/ByTransaction', {
            params: {
                productId: productId,
                transactionType: 'Rental'
            }
        }).then(function (response) {
            var categoryId = 0;
            if (response.data && response.data.Code === 200 && response.data.Data) {
                categoryId = response.data.Data.TaxCategoryId || 0;
            }
            rentalCategoryCache[productId] = categoryId;
            return categoryId;
        }).catch(function () {
            return 0;
        });
    }

    function clearCache() {
        rentalCategoryCache = {};
    }

    return {
        getRentalTaxCategoryId: getRentalTaxCategoryId,
        clearCache: clearCache
    };
}]);
