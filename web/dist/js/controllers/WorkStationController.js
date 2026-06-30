app.controller("WorkStationListController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {


        var workStation = new $.WorkStation();
        $scope.WorkStations = [];

        $scope.Search = { Name: '' }

        $scope.GetAll = function () {

            workStation.WorkStationList(function (e) {

                $scope.WorkStations = e.data.Data;
            });
        }
        $scope.edit = function (item) {
            var key = $crypto.encrypt(item.WorkStationId);
            $state.go('editworkstation', { key: key });
        }

        $scope.GetAll();

    });
app.controller("WorkStationController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {

        var _workStationId = $stateParams.key ? $crypto.decrypt($stateParams.key) : 0;
        var workStation = new $.WorkStation();

        var token = $rootScope.getTokenInfo();
        $scope.WorkStation = { Name: '', Description: '', WorkStationTypeId: 0, WorkStationId: _workStationId };

        $scope.GetAllTypes = function () {

            workStation.TypeList(function (e) {

                $scope.WorkStationTypes = e.data.Data;
            });
        }
        $scope.GetAllTypes();
        $scope.Save = function () {
            var m = $('#frmWorkStation').valid();
            if (m) {
                workStation.SaveWorkStation(function (e) {
                    if (e.data.Code == 200) {
                        /* showMessage(MessageClass.COMPANY_SAVED);
                         $('#addTransModel').modal('hide');
                         $scope.GetAll(); */
                        alert('Record saved successfully.');
                        $state.go('workstations');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, $scope.WorkStation);
            }
        }
        FormsValidation.init('frmWorkStation');



        if ($scope.WorkStation.WorkStationId > 0) {
            workStation.WorkStationDetails(function (e) {
                $scope.WorkStation = e.data.Data;
            }, { WorkStationId: $scope.WorkStation.WorkStationId });
        }




    });

app.controller("WorkStationTypeListController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {


        var workStation = new $.WorkStation();
        $scope.WorkStationTypes = [];

        var token = $rootScope.getTokenInfo();

        $scope.Search = { Name: '' }

        $scope.GetAll = function () {

            workStation.TypeList(function (e) {

                $scope.WorkStationTypes = e.data.Data;
            });
        }
        $scope.edit = function (item) {
            var key = $crypto.encrypt(item.WorkStationTypeId);
            $state.go('editworkstationtype', { key: key });
        }

        $scope.GetAll();

    });
app.controller("WorkStationTypeController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {

        var _typeId = $stateParams.key ? $crypto.decrypt($stateParams.key) : 0;
        var workStation = new $.WorkStation();

        var token = $rootScope.getTokenInfo();
        $scope.WorkStationType = { Name: '', Description: '', WorkStationTypeId: _typeId };

        $scope.Save = function () {
            var m = $('#frmWorkStationType').valid();
            if (m) {
                workStation.SaveType(function (e) {
                    if (e.data.Code == 200) {
                        /* showMessage(MessageClass.COMPANY_SAVED);
                         $('#addTransModel').modal('hide');
                         $scope.GetAll(); */
                        alert('Record saved successfully.');
                        $state.go('workstationtypes');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, $scope.WorkStationType);
            }
        }
        FormsValidation.init('frmWorkStationType');



        if ($scope.WorkStationType.WorkStationTypeId > 0) {
            workStation.TypeDetails(function (e) {
                $scope.WorkStationType = e.data.Data;
            }, { WorkStationTypeId: $scope.WorkStationType.WorkStationTypeId });
        }
    });
//Operations
app.controller("OperationsListController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {


        var operation = new $.Operation();
        $scope.Operations = [];

        $scope.Search = { Name: '' }

        $scope.GetAll = function () {

            operation.OperationsList(function (e) {

                $scope.Operations = e.data.Data;
            });
        }
        $scope.edit = function (item) {
            var key = $crypto.encrypt(item.OperationId);
            $state.go('editoperation', { key: key });
        }

        $scope.GetAll();

    });
app.controller("OperationController",
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService, $crypto) {

        var _operationId = $stateParams.key ? $crypto.decrypt($stateParams.key) : 0;
        var operation = new $.Operation();

        var token = $rootScope.getTokenInfo();
        $scope.Operation = { Name: '', Description: '', ParentOperationId: 0, OperationId: _operationId };

        $scope.GetAll = function () {

            operation.OperationsList(function (e) {
                $scope.Operations = e.data.Data.filter(o => o.OperationId != _operationId);
            });
        }
        $scope.GetAll();
        $scope.Save = function () {
            var m = $('#frmOperation').valid();
            if (m) {
                operation.SaveOperation(function (e) {
                    if (e.data.Code == 200) {
                        /* showMessage(MessageClass.COMPANY_SAVED);
                         $('#addTransModel').modal('hide');
                         $scope.GetAll(); */
                        alert('Record saved successfully.');
                        $state.go('operations');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, $scope.Operation);
            }
        }
        FormsValidation.init('frmOperation');



        if ($scope.Operation.OperationId > 0) {
            operation.OperationDetails(function (e) {
                $scope.Operation = e.data.Data;
            }, { OperationId: $scope.Operation.OperationId });
        }




    });

  