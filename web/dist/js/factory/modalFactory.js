//----------------
angular.module('medRack').factory('ModalFactory', ['$window', '$mdDialog',
    function ($window, $mdDialog) {
        return {
            Dialog: $mdDialog,
            BillType: function (controller) {
                var div = '<div style="width:40%;height:10%"></div>';

                $(div).load('templ/dialogs/challanHeaderType.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            ConfirmToPrint: function (controller, scope) {
                var div = '<div style="width:50%;height:10%"></div>';

                $(div).load('templ/dialogs/messagewithConfirm.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();
                    // $scope.Message = message;
                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: scope,

                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            ConfirmDelete: function (scope, okClick, closeClick, parent) {

                var deleteController = function ($scope) {

                    //   $scope.Message = 'Are you sure to delete this challan record?';
                    $scope.OkButtonClick = function () {

                        okClick.call();
                    };
                    $scope.closeDialog = function () {
                        //  closeClick();
                        $mdDialog.hide();
                    };
                }
                var div = '<div style="min-width:20%;height:10%"></div>';
                var _parent = angular.element(document.body);
                if (parent) {
                    _parent = parent;
                }
                $(div).load('templ/dialogs/confirmDialog.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();
                    //$scope.Message = message;
                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: scope,
                        preserveScope: true,
                        height: '200px',
                        width: '300px',
                        template: html,
                        parent: _parent,
                        controller: deleteController

                    });
                });
            },
            Confirm: function (controller, scope, parent) {
                var div = '<div style="min-width:20%;height:10%"></div>';
                var _parent = angular.element(document.body);
                if (parent) {
                    _parent = parent;
                }
                $(div).load('templ/dialogs/confirmDialog.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();
                    //$scope.Message = message;
                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: scope,
                        preserveScope: true,
                        height: '200px',
                        width: '300px',
                        template: html,
                        parent: _parent,
                        controller: controller

                    });
                });
            },
            //confirm dialog from bootstrap

            Info: function (controller, localData) {
                var div = '<div style="width:20%;height:10%"></div>';

                $(div).load('templ/dialogs/infoDialog.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        locals: { localData: localData },
                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            ShowGRNItems: function (grnId, controller) {

                var div = '<div style="width:90%;height:70%"></div>';
                //debugger
                $(div).load('templ/dialogs/grn_dialog.html?d=' + new Date().getTime().toString(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        // scope: $scope,
                        preserveScope: true,
                        locals: {
                            grnId: grnId
                        },
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller
                    });
                });
            },
            AddEditClientSite: function (controller, data) {
                var div = '<div style="width:50%;height:80% !important"></div>';

                $(div).load('templ/dialogs/addEditClientSite.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            AddEditSiteJob: function (controller, data) {
                var div = '<div style="width:50%;height:90% !important"></div>';

                $(div).load('templ/dialogs/siteJob.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            AddEditLedger: function (controller, data) {
                var div = '<div style="width:50%;height:80% !important"></div>';

                $(div).load('templ/dialogs/addEditLedger.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            AddEditEmployee: function (controller, data) {
                var div = '<div ></div>';
                $(div).load('templ/dialogs/addEditEmployee.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200px',
                        width: '50%',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            AddEditVehicle: function (controller, data) {
                var div = '<div style="width:30%;max-height:400px !important;"></div>';

                $(div).load('templ/dialogs/addEditVehicle.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        //  scope: $scope,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            AddEditTaxCategory: function (controller, data) {
                var div = '<div style="width:50%;max-height:500px !important;"></div>';

                $(div).load('templ/dialogs/addEditTaxCategory.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '200',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller
                    });
                });
            },
            AddEditTax: function (controller, data) {
                var div = '<div style="width:60%;max-height:600px !important;"></div>';

                $(div).load('templ/dialogs/addEditTax.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        clickOutsideToClose: true,
                        locals: {
                            localData: data
                        },
                        preserveScope: true,
                        height: '400',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller
                    });
                });
            },
            ShowBillingDialog: function (controller, $scope) {
                var div = '<div ></div>';
                debugger
                $(div).load('templ/dialogs/genbillDialog.html?d=' + new Date().getTime(), function () {
                    var html = $(this).html();

                    $mdDialog.show({
                        fullscreen: true,
                        clickOutsideToClose: true,
                        scope: $scope,
                        //locals: {
                        //    localData: data
                        //},
                        preserveScope: true,
                        height: '200px',
                        width: '800px',
                        template: html,
                        parent: angular.element(document.body),
                        controller: controller

                    });
                });
            },
            ShowDialog: function (controller, template, data, $scope, parent) {
                var div = '<div style="width:50%;height:auto !important"></div>';

                $(div).load(template + '?d=' + new Date().getTime(), function () {
                    var html = $(this).html();
                    var _parent = angular.element(document.body);
                    if (parent) {
                        _parent = parent;
                    }
                    $mdDialog.show({
                        clickOutsideToClose: true,
                        scope: $scope,
                        locals: {
                            localData: data
                        },

                        preserveScope: true,
                        //  height: '200',
                        template: html,
                        parent: _parent,
                        controller: controller

                    });
                });
            },
            hideDialog: function () {
                $mdDialog.hide();
            },
            CloseDialog: function () {
                $mdDialog.hide();
            }
        }
    }
]);
