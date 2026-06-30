app.factory('BackupService', function ($http) {
    var baseUrl = API_URL;

    return {
        // Existing methods...
        list: function () {
            return $http.post(baseUrl + '/backup/list');
        },
        backupdb: function () {
            return $http.post(baseUrl + '/backup/backupdb');
        },
        downloadBackup: function (log) {
            return $http({
                method: 'POST',
                data:log,
                responseType: 'blob',
                url: baseUrl + '/backup/downloadBackup'
            }, log);
        }
    };
});