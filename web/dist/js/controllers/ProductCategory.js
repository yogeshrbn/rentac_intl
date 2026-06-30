app.controller('ProductCategoryController', function ($scope, $routeParams, $http) {
    debugger
    var cId = $routeParams.cId == undefined ? 0 : $routeParams.cId;
    var category = new $.ProductCategory({ CategoryId: cId });
    $scope.Category = category;
    if (cId == 0) {
        GetList();
    }
    else {
        category.GetInfo(function (e) {
            
            $scope.Category.Props = e.data;
        });
    }
    function GetList() {
        category.GetAll(function (e) {
            $scope.Categories = e.data;
        });
    }
    $scope.Activate = function (status, categoryId) {
        debugger
        category.Props.CategoryId = categoryId;
        category.Props.Status = status;
        category.ChangeStatus(function (e) {
            GetList();
        });
    }
    $scope.Save = function () {
        EnableToolbar(0);
        debugger
        category.Add(function (e) {
            EnableToolbar(1);
            showMessage(MessageClass.PRODUCTCATEGORY_SAVED);
        });
    };
    $scope.RowSelected = function (index) {



    }
});