app.controller('SaltController', function ($scope, $routeParams, $http) {
    var sId = $routeParams.sId == undefined ? 0 : $routeParams.sId;

    var salt = new $.Salt({ SaltId: sId, StoreId: STOREID });
    $scope.Salt = salt;
    function BindList() {
        salt.GetAll(function (e) {
            $scope.Salts = e.data;
        });
    }
    if (sId == 0) {
        BindList();
    }
    else {
        salt.GetDetails(function (e) {
            console.log(JSON.stringify(e.data));
            $scope.Salt = salt = new $.Salt(e.data);
            console.log(JSON.stringify($scope.Salt));
        });
       
    }
    FormsValidation.init();
    $scope.Activate = function (status, saltId) {
        debugger
        if (status == 2) {
            if (!confirm('Are you sure to de-activate the salt?')) {
                return;
            }
        }
        salt.Props.SaltId = saltId;
        salt.Props.Status = status;
        salt.UpdateStatus(function (e) {
            BindList();
        });
    }
    $scope.Save = function () {
         debugger
        var m = $('#form-salt').valid();
        if (m) {
            salt.Add(function (e) {
                showMessage(MessageClass.SALT_SAVED);
            });
        }
    }
});