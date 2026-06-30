app.controller('AccountGroupController', function ($scope, $routeParams, $http) {
    var gId = $routeParams.gId == undefined ? 0 : $routeParams.gId;
    var groupDTO = new $.AccountGroup({ AccountGroupId: gId });
    FormsValidation.init();
    $scope.AccGroup = groupDTO;
    
    function BindList() {
        groupDTO.GetAll(function (e) {
            $scope.AccGroups = e.data;
        });
    }
    if (gId == 0) {

        BindList();

    }
    else {
        //selects all acccount groups, this is to bind the parent dropdown.
        groupDTO.GetAll(function (e) {
            $scope.AccGroups = e.data;
        });
        //Gets the Account Group details.
        groupDTO.GetInfo(function (e) {
            $scope.AccGroup.Props = e.data;
            console.log(JSON.stringify(e.data));
        });


    }
    $scope.Save = function () {
        EnableToolbar(0);
        debugger
        groupDTO.Add(function (e) {
            EnableToolbar(1);
            showMessage(MessageClass.PRODUCTCATEGORY_SAVED);
        });
    };
    $scope.Activate = function (status, accountGroupId) {
        groupDTO.Props.AccountGroupId = accountGroupId;
        groupDTO.Props.IsActive = status;
        var allow = true;
        if (status == 0) {
            if (!confirm('Are you sure to de-activate?')) {
                allow = false;
            }
        }
        if (allow) {
            groupDTO.UpdateStatus(function (e) {
                BindList();
            });
        }

    };

});