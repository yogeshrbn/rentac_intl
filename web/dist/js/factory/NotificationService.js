app.factory('NotificationSerivice', function ($http) {
    var baseUrl = API_URL;
    var base_REP_URL = REPORT_SERVER;
    return {
        // Existing methods...
        getAlerts: function (data) {
            return $http.post(base_REP_URL + '/api/notify/GetMyAlerts',data);
        },
        updateStatus: function (data) {
            return $http.post(base_REP_URL + '/api/notify/UpdateStatus', data);
        },

    };
});