app.factory('AccountService', function ($http) {
    var baseUrl = API_URL;

    return {
        // Existing methods...
        initChallan: function (data) {
            return $http.post(baseUrl + '/workorder/InitChallan');
        },

        getAllAccountGroups: function (o) {
            return $http.post(baseUrl + '/Account/GetAllGroups');
        },
    };
});