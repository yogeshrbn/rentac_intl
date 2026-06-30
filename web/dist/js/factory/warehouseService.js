app.factory('WarehouseService', function ($http) {
    var baseUrl = API_URL;

    return {
        // Existing methods...
        getWarehouses: function () {
            return $http.get(baseUrl + '/warehouse/GetAll');
        },
        createWarehouse: function (warehouse) {
            return $http.post(baseUrl + '/warehouse/Save', warehouse);
        },
        updateWarehouse: function (id, warehouse) {
            return $http.post(baseUrl + '/warehouse/Save', warehouse);
        },
        UpdateStatus: function ( warehouse) {
            return $http.post(baseUrl + '/warehouse/UpdateStatus', warehouse);
        },
    };
});