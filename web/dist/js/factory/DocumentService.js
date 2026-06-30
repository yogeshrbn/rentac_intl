// Document Service
app.factory('ProductionService', ['$http', function ($http) {
    var _baseLocation = STORAGE_LOC
    return {
        getDocs: function (fileName) {
            return _baseLocation + '/0/1034/docs/' + fileName; 
        }
    });