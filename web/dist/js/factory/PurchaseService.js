app.factory('PurchaseService', function ($http) {
    var baseUrl = API_URL;

    return {
        // Existing methods...
        
        save: function (data) {
            return $http.post(baseUrl + '/purchase/Save', data);
        },
       
    };
});