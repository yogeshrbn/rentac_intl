app.controller("TransporterListController", ['$scope', '$rootScope', '$state', '$stateParams', '$http', 'CompanyService',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService) {


        var transDto = new $.Transporter();
        $scope.Transporters = [];
        $scope.TP = { Name: '', GST: '', Email: '', Phone: '' };

        var token = $rootScope.getTokenInfo();

        $scope.Search = { Name: '' }

        $scope.GetAll = function () {
            
            transDto.GetAll(function (e) {
                $scope.Transporters = e.data;
            });
        }
        $scope.GetAll();
        $scope.Save = function () {
            var m = $('#frmTransporter').valid();
            if (m) {
                transDto.Save(function (e) {
                    if (e.data.Code == 200) {
                        showMessage(MessageClass.COMPANY_SAVED);
                        $('#addTransModel').modal('hide');
                        $scope.GetAll();
                    } else {
                        showMessage(e.data.Message);
                    }

                }, $scope.TP);
            }
        }
        FormsValidation.init('frmTransporter');

        $scope.edit = function (item) {
            $scope.TP = item;
            $('#addTransModel').modal('show');
        }
        $scope.closeModal = function () {
            $('#addTransModel').modal('hide');
        }
    }]);

