app.controller('RoleCostingController', function ($scope, ModalFactory) {
    var roleCosting = new $.RoleCosting();
    var lookup = new $.Lookup();

    $scope.RoleCosts = [];
    $scope.Roles = [];
    $scope.Model = { RoleCostingId: 0, RoleId: 0, PerHourCost: 0, PerDayCost: 0 };

    $scope.list = function () {
        roleCosting.list(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.RoleCosts = e.data.Data || [];
        });
    };

    $scope.loadRoles = function () {
        lookup.GetAllEmployeeRoles(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Roles = e.data.Data || [];
        });
    };

    $scope.New = function () {
        $scope.Model = { RoleCostingId: 0, RoleId: 0, PerHourCost: 0, PerDayCost: 0 };
    };

    $scope.edit = function (item) {
        roleCosting.byId(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Model = e.data.Data;
            $('#addEditRoleCosting').modal('show');
        }, { RoleCostingId: item.RoleCostingId });
    };

    $scope.save = function () {
        if (!$scope.Model.RoleId || $scope.Model.RoleId <= 0) {
            alert('Please select role.');
            return;
        }
        if (!$scope.Model.PerHourCost || Number($scope.Model.PerHourCost) <= 0) {
            alert('Please enter per hour cost.');
            return;
        }
        if (!$scope.Model.PerDayCost || Number($scope.Model.PerDayCost) <= 0) {
            alert('Please enter per day cost.');
            return;
        }

        roleCosting.save(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $('#addEditRoleCosting').modal('hide');
            $scope.list();
        }, $scope.Model);
    };

    $scope.delete = function (item) {
        var deleteController = function ($scope) {
            $scope.Message = 'Are you sure to delete this role costing record?';
            $scope.OkButtonClick = function () {
                roleCosting.deleteItem(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    $scope.list();
                }, { RoleCostingId: item.RoleCostingId });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        };
        ModalFactory.Confirm(deleteController, $scope, $('body'));
    };

    $scope.loadRoles();
    $scope.list();
});
