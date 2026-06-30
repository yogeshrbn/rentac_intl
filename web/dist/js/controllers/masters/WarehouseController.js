app.controller('WarehouseController', function ($scope, WarehouseService, ModalFactory) {
    $scope.warehouses = [];
    $scope.warehouse = {};
    $scope.isEditing = false;

    // Lookup data
    $scope.users = [];
    $scope.clients = [];
    $scope.statuses = [];

    function loadWarehouses() {
        WarehouseService.getWarehouses().then(function (response) {
            $scope.warehouses = response.data.Data;
        });
    }



    $scope.showAddForm = function () {
        $scope.isEditing = false;
        $scope.warehouse = {
            StatusId: 1 // Default to Active
        };
        $('#warehouseModal').modal('show');
    };

    $scope.editWarehouse = function (warehouse) {
        $scope.isEditing = true;
        $scope.warehouse = angular.copy(warehouse);
        $('#warehouseModal').modal('show');
    };

    $scope.saveWarehouse = function () {
        var m = $('#warehouseForm').valid();
        if (!m) {
            return;
        }
        if ($scope.isEditing) {
            WarehouseService.updateWarehouse($scope.warehouse.WarehouseId, $scope.warehouse)
                .then(function () {
                    $('#warehouseModal').modal('hide');
                    loadWarehouses();
                });
        } else {
            WarehouseService.createWarehouse($scope.warehouse)
                .then(function () {
                    $('#warehouseModal').modal('hide');
                    loadWarehouses();
                });
        }
    };


    $scope.deleteWarehouse = function (warehouse) {


        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this warehouse';
            $scope.OkButtonClick = function () {


                WarehouseService.UpdateStatus({ WarehouseId: warehouse.WarehouseId, StatusId: 2 })
                    .then(function (e) {
                        debugger
                        if (e.data.Code != 200) {
                            alert(e.data.Message);
                            return;
                        }
                        ModalFactory.Dialog.hide();
                        loadWarehouses();
                    });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));
    }
    // Initialize
    loadWarehouses();

});