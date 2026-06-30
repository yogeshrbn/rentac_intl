app.factory('FileUploadService', function ($http) {
    var baseUrl = API_URL;
    var _baseLocation = STORAGE_LOC;
    return {
        // Existing methods...

        upload: function (file) {

            var formData = new FormData();
            formData.append('file', file);

            return $http.post(baseUrl + '/Fileupload/upload', formData, {
                transformRequest: angular.identity, // Prevent Angular from serializing FormData
                headers: { 'Content-Type': undefined } // Let the browser set Content-Type for FormData
            });

        },
        downloadAsBlob: function (data) {
             
            return $http.post(baseUrl + '/Fileupload/download', data, {
               
                responseType: 'blob'
            });

        },
        getDocs: function (compId,fileName) {
            if (fileName) {
                return _baseLocation +   '0/' + compId + '/docs/' + fileName;
            }
            else return "../img/no-img.svg";
        }
    };
});