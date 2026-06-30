// Manufacturing Service
app
    .factory('ProductionService', ['$http', function ($http) {
        var _baseApi = API_URL
        return {

            // Client methods
            getClients: function () {
                return $http.get(_baseApi + '/Ledger/GetAllLedger?name=');
            },

            // Item methods
            getManufacturableItems: function () {
                return $http.post(_baseApi + '/product/GetProductSizeListByCompany');
            },

            // BOM methods
            getBOM: function (model) {
                return $http.post(_baseApi + '/product/BOMByProductId', model);
            },

            // Sales Order methods
            getSalesOrders: function () {
                return $http.get(_baseApi + '/sales-orders');
            },

            // Save manufacturing order
            saveManufacturingOrder: function (orderData) {
                return $http.post(_baseApi + '/Production/Create', orderData);
            },
            getOperations: function () {
                return $http.get(_baseApi + '/Production/Operations');
            }
        };
    }]);

