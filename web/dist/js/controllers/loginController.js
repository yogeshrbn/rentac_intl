app.controller('loginController', ['$scope', '$rootScope', 'LoginService', 'StorageService', '$location', 'AuthenticationService',
    function ($scope, $rootScope, loginService, storageService, $location, authenticationService) {

        //console.log(JSON.stringify(authenticationService));

        //console.log(authenticationService.isAuthenticated);
        // authenticationService.removeToken();
        if (authenticationService.isAuthenticated() == true) {
            window.location.href = '/pages/index.html#/home';

            return;
        }

        $rootScope.showNav = $scope.showNav = 0;
        FormsValidation.init();
        $scope.loginData = {
            userName: "",
            password: "",
            finYear: 0
        };
        var config = new $.Config({});
         
        storageService.clearAll();
        localStorage.clear();
        config.GetFinYearList(function (e) {
            $scope.FinYearList = e.data;
            storageService.setFinYear(e.data);
        });
      
        // clearSessionSlider(); // clears the sessionSliderTimeInterval
        // resetSlider();
        $scope.login = function () {

            var m = $('#form-login').valid();
            //if (!m) {
            //    return;
            //}

            loginService.login($scope.loginData.userName, $scope.loginData.password, $scope.loginData.finYear).then(function (response) {
                 
                //if (response != null && response.error != undefined) {
                //    $scope.message = response.error_description;
                //}
                if (response != null && response.Message != undefined) {
                    $scope.message = response.Message;
                }
                else {
                    $('#page-container').addClass('sidebar-partial sidebar-visible-lg sidebar-no-animations');
                    // $location.path('/home');
                    window.location.href = '/pages/index.html#/home';
                }
            });
        }
        $('#page-container').removeClass('sidebar-partial sidebar-visible-lg sidebar-no-animations');
        // var token = $rootScope.getTokenInfo();
    }]);
app.controller('SignUpController', ['$scope', '$rootScope', 'LoginService', 'StorageService', '$location', '$state',
    function ($scope, $rootScope, loginService, storageService, $location, $state) {
        FormsValidation.init();
        $scope.signUpData = {
            name: "",
            email: "",

            mobile: "",
            company: ""
        };
        //var config = new $.Config({});
        //config.GetFinYearList(function (e) {
        //    $scope.FinYearList = e.data;
        //    storageService.setFinYear(e.data);

        //});


        // clearSessionSlider(); // clears the sessionSliderTimeInterval
        // resetSlider();
        $scope.register = function () {

            var m = $('#form-signup').valid();
            if (!m) {
                return;
            }
            var subscription = new $.Subscription();
            var model = $scope.signUpData;
            subscription.registerClient(function (e) {
                if (e.data.Code != 200) {
                    $scope.message = e.data.Message;
                } else {
                    //  $location.path('/signupconfirm');
                    $state.go('signupconfirm');
                }
            }, model);
        }
        $('#page-container').removeClass('sidebar-partial sidebar-visible-lg sidebar-no-animations');
        //var token = $rootScope.getTokenInfo();
    }]);
app.controller('ForgotPasswordController', ['$scope', '$rootScope', 'UserService', 'StorageService', '$location', '$state',
    function ($scope, $rootScope, userService, storageService, $location, $state) {

        $scope.email = "";
        $scope.emailSent = false;
        $scope.sendVerificationLink = function () {

            var m = $('#frmForgotPwd').valid();
            if (!m) {
                return;
            }

            userService.sendForgotPwdLink(function (e) {
                if (e.data.Code != 200) {
                    alert(e.data.Message);
                } else {
                    $scope.emailSent = true;
                }
            }, $scope.email);
        }
        FormsValidation.init('frmForgotPwd');
        $('#page-container').removeClass('sidebar-partial sidebar-visible-lg sidebar-no-animations');
    }]);
app.controller('ForgotPasswordLinkVericationController', ['$scope', '$rootScope', 'UserService', 'StorageService', '$stateParams', '$state', 'toaster',
    function ($scope, $rootScope, userService, storageService, $stateParams, $state, toaster) {
        FormsValidation.init('frmForgotResetPwd');

        $scope.guId = $stateParams.guId;
        $scope.pwd = { guid: $stateParams.guId, Password: '', ConfirmPassword: '' };
        $scope.pwdChanged = false;
        $scope.linkStatus = 1;
        $scope.verifyLink = function () {


            userService.validateForgotPwdLink(function (e) {
                if (e.data.Code != 200) {
                    $scope.linkStatus = e.data.Data;

                }
            }, $scope.guId);
        }
        $scope.verifyLink();
        $scope.resetPassword = function () {

            var m = $('#frmForgotResetPwd').valid();
            if (!m) {
                return;
            }


            userService.resetPasswordByGuId(function (e) {

                if (e.data.Code != 200) {
                    $scope.message = e.data.Message;
                    toaster.pop('Error', "Error", e.data.Message);

                } else {
                    $scope.pwdChanged = true;

                }
            }, $scope.pwd);
        }
        $('#page-container').removeClass('sidebar-partial sidebar-visible-lg sidebar-no-animations');
    }]);


