app.controller('ZoneListController', function ($scope, ModalFactory) {
    var zone = new $.Zone();
    $scope.Zones = [];
    $scope.list = function () {
        zone.zoneList(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Zones = e.data.Data;
        }, {});
    }
    $scope.delete = function (item) {
        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this record?';
            $scope.OkButtonClick = function () {

                var z = new $.Zone();
                z.deleteZone(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    $scope.list();
                }, item);
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));

    }
    $scope.edit = function (item) {
        zone.zoneById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Zone = e.data.Data;
            $('#addEditZone').modal('show');
        }, { ZoneId: item.ZoneId });
    }
    $scope.list();
    $scope.Zone = { ZoneId: 0, Name: '', Localities: [] };
    $scope.Locality = '';
    $scope.zoneById = function () {
        zone.zoneById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }

        }, { ZoneId: zId });
    }
   
    $scope.addLocality = function () {
        var model = { LocalityId: 0, Name: $scope.Locality, Deleted:0 };
        $scope.Zone.Localities.push(model);
        $scope.Locality = '';
    }

    $scope.save = function () {

        var model = cloneObj($scope.Zone);
        zone.save(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#addEditZone').modal('hide');
            $scope.list();
        }, model);
    }

    $scope.deleteLocality = function(item){
        item.Deleted = 1;
    }
});
 