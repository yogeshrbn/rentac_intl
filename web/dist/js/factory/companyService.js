app.factory('CompanyService', function ($http) {
    var baseUrl = API_URL;

    return {
        // Existing methods...
        getCompanyDetails: function (data) {
            return $http.post(baseUrl + '/company/GetDetails', data);
        },
        searchCompany: function (data) {
            return $http.post(baseUrl + '/company/SearchCompany', data);
        },
        getAllCompanies: function () {
            return $http.get(baseUrl + '/company/GetAll');
        },
        getAllVehicle: function () {
            return $http.post(baseUrl + '/Vehicle/GetAll');
        },
        addVehicle: function (data) {
            return $http.post(baseUrl + '/Vehicle/Add', data);
        },
        getVehicleInfo: function (data) {
            return $http.post(baseUrl + '/Vehicle/GetById', data);
        }
       
    };
});