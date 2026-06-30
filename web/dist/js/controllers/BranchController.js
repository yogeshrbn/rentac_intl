app.controller("BranchListController", ['$scope', '$rootScope', '$state', '$stateParams', '$http', 'CompanyService',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService) {


        var cId = $stateParams.key == undefined ? 0 : $stateParams.key;
        var companyDTO = new $.Company({ CompanyId: cId });
        $scope.isBranch = $state.current.data.isBranch;
        FormsValidation.init();
        $scope.Companies = companyDTO;
        var token = $rootScope.getTokenInfo();

        $scope.Search = { Name: '' }

        function BindList() {
            if ($scope.isBranch == true) {
                companyDTO.GetBranches(function (e) {
                    $scope.Companies = e.data;
                }, token.DefaultCompanyId);

            }

            else {
                companyDTO.GetAll(function (e) {
                    $scope.Companies = e.data;
                });
            }
        }

        if (cId == 0) {

            BindList();

        }
        else {
            companyDTO.GetDetails(function (e) {

                $scope.Company = companyDTO = new $.Company(e.data);
                $scope.Logo = e.data.Logo;

            });

        }

        $scope.Activate = function (isActive, companyId) {
            if (isActive == 0) {
                if (!confirm('Are you sure to de-activate the company?')) {
                    return;
                }
            }
            companyDTO.Props.CompanyId = companyId;
            companyDTO.Props.IsActive = isActive;
            companyDTO.ActivateDeActivate(function (e) {
                BindList();
            });
        }

        $scope.Save = function () {
            debugger
            if ($scope.logoChanged == false) {
                companyDTO.Props.Logo = 'no';
            }
            var m = $('#form-company').valid();
            var data = cloneObj($scope.Company.Props);
            if (m) {
                EnableToolbar(0);
                companyDTO.Add(function (e) {
                    EnableToolbar(1);

                    if (e.data.Code == 200) {
                        showMessage(MessageClass.COMPANY_SAVED);
                        $state.go('companylist');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, data);
            }
        }
        $scope.RowSelected = function (index) {



        }
        $scope.find = function () {

            CompanyService.SearchCompany(function (e) {
                if (e.status == 200) {
                    $scope.Companies = e.data.items;
                }
                else {
                    alert(e.data.Description);
                }

            }, $scope.Search);
        }


        $scope.browseFile = function () {

            $('#fileProfile').click();
        };

        $scope.logoChanged = false;
        $scope.Logo = '';
        $scope.onFileBrowse = function (event) {

            readURL(event.currentTarget, function (e) {
                $scope.logoChanged = true;
                $scope.Company.Props.Logo = e;
                $scope.Logo = e;
                $scope.$apply();

            });
        };
        function readURL(input, onRead) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {

                    // $('#imgProfile').attr('src', e.target.result);
                    onRead.call(null, e.target.result);

                }

                if (input.files[0].size > 100000) {
                    alert('Logo size can not exceed 100 KB');
                    return;
                }
                var fileType = input.files[0].type;
                if (fileType == "image/png" || fileType == "image/jpeg" || fileType == "image/jpg") {
                    var dataUrl = reader.readAsDataURL(input.files[0]);
                    return dataUrl;
                }
                else {
                    alert('Logo must be either png, or jpeg or jpg');
                    return;
                }


            }
        }


    }]);

app.controller("BranchController", ['$scope', '$rootScope', '$state', '$stateParams', '$http', 'CompanyService',
    function ($scope, $rootScope, $state, $stateParams, $http, CompanyService) {


        var cId = $stateParams.key == undefined ? 0 : $stateParams.key;
        var companyDTO = new $.Company({ CompanyId: cId });
        $scope.isBranch = $state.current.data.isBranch;
        FormsValidation.init();
        $scope.Companies = companyDTO;
        var token = $rootScope.getTokenInfo();

        $scope.Search = { Name: '' }

        function BindList() {
            if ($scope.isBranch == true) {
                companyDTO.GetBranches(function (e) {
                    $scope.Companies = e.data;
                }, token.DefaultCompanyId);

            }

            else {
                companyDTO.GetAll(function (e) {
                    $scope.Companies = e.data;
                });
            }
        }

        if (cId == 0) {

            BindList();

        }
        else {
            companyDTO.GetDetails(function (e) {

                $scope.Company = companyDTO = new $.Company(e.data);
                $scope.Logo = e.data.Logo;

            });

        }

        $scope.Activate = function (isActive, companyId) {
            if (isActive == 0) {
                if (!confirm('Are you sure to de-activate the branch?')) {
                    return;
                }
            }
            companyDTO.Props.CompanyId = companyId;
            companyDTO.Props.IsActive = isActive;
            companyDTO.ActivateDeActivate(function (e) {
                BindList();
            });
        }

        $scope.Save = function () {
          
            if ($scope.logoChanged == false) {
                companyDTO.Props.Logo = 'no';
            }
            var m = $('#form-company').valid();
            var data = cloneObj($scope.Company.Props);
            data.isBranch = 1;
            data.ParentCompanyId = token.DefaultCompanyId;
            if (m) {
                EnableToolbar(0);
                companyDTO.Add(function (e) {
                    EnableToolbar(1);

                    if (e.data.Code == 200) {
                        showMessage('Branch saved successfully.');
                        $state.go('branches');
                    } else {
                        showMessage(e.data.Message);
                    }

                }, data);
            }
        }
        $scope.RowSelected = function (index) {



        }
        $scope.find = function () {

            CompanyService.SearchCompany(function (e) {
                if (e.status == 200) {
                    $scope.Companies = e.data.items;
                }
                else {
                    alert(e.data.Description);
                }

            }, $scope.Search);
        }


        $scope.browseFile = function () {

            $('#fileProfile').click();
        };

        $scope.logoChanged = false;
        $scope.Logo = '';
        $scope.onFileBrowse = function (event) {

            readURL(event.currentTarget, function (e) {
                $scope.logoChanged = true;
                $scope.Company.Props.Logo = e;
                $scope.Logo = e;
                $scope.$apply();

            });
        };
        function readURL(input, onRead) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {

                    // $('#imgProfile').attr('src', e.target.result);
                    onRead.call(null, e.target.result);

                }

                if (input.files[0].size > 100000) {
                    alert('Logo size can not exceed 100 KB');
                    return;
                }
                var fileType = input.files[0].type;
                if (fileType == "image/png" || fileType == "image/jpeg" || fileType == "image/jpg") {
                    var dataUrl = reader.readAsDataURL(input.files[0]);
                    return dataUrl;
                }
                else {
                    alert('Logo must be either png, or jpeg or jpg');
                    return;
                }


            }
        }


    }]);

