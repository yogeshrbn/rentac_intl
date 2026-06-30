app.controller('ItemGroupController', function ($scope, ModalFactory) {
    $scope.Group = { GroupName: '', GroupCode: '', ItemGroupId: 0 };
    var product = new $.Product();
    $scope.GroupList = [];
    $scope.save = function () {
        product.SaveItemGroup(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            
            $('#addEditGroup').modal('hide');
            $scope.Group.ItemGroupId = 0;
            $scope.list();
        }, $scope.Group);
    }

    $scope.list = function () {
        product.ListItemGroup(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.GroupList = e.data.Data;
        });
    }
    $scope.list();
    $scope.edit = function (group) {

        $scope.byId(group.ItemGroupId);
    }
    $scope.byId = function (groupId) {
        product.ItemGroupById(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Group = e.data.Data;
            $('#addEditGroup').modal('show');
        }, { ItemGroupId: groupId });
    }
    $scope.delete = function (group) {
        var deleteController = function ($scope) {

            $scope.Message = 'Are you sure to delete this group record?';
            $scope.OkButtonClick = function () {

                var pd = new $.Product();
                pd.DelteItemGroup(function (e) {
                    if (e.data.Code != 200) {
                        alert(e.data.Message);
                        return;
                    }
                    ModalFactory.Dialog.hide();
                    $scope.list();
                }, { ItemGroupId: group.ItemGroupId });
            };
            $scope.closeDialog = function () {
                ModalFactory.Dialog.hide();
            };
        }
        ModalFactory.Confirm(deleteController, $scope, $('body'));



    }
});