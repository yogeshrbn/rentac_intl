(function () {
    'use strict';
    app.controller('indexController', ['$scope', '$rootScope', '$location', '$route', 'authData', 'LoginService', 'StorageService',
        'AuthenticationService',
        function ($scope, $rootScope, $location, $route, authData, loginService, storageService, authService) {
             
            $rootScope.$on('onLogin', function (e, args) {
                 
                $scope.showNav = '1'

                storageService.setData('showLogin', '1');

                
            });
            $scope.logOut = function () {
             
                storageService.clearAll();
                loginService.logOut();
                window.location.href = '../index.html#/login';
                //$location.path('../index.htmll#login');
            }
            $scope.authentication = authData.authenticationData;

            var loginData = authService.getTokenInfo();
            if (loginData && loginData.ProfilePic) {
                $scope.profilePic = IMAGE_LOCATION + loginData.ProfilePic;
            }
            if (loginData) {
                 
                var menus = storageService.getData('menus');
                if (menus) {
                    $scope.menus = JSON.parse(menus);
                    window.setTimeout(() => {
                         
                        App.init();
                    }, 100);
                    //init prime-ui to open and close menus.
                  

                }
            }

            $scope.loadDefault = function () {
               
                return '../img/userPic.JPG';
            }
            $rootScope.$on('onProfilePicChange', function (e, args) {
                
                $scope.profilePic = args;;
            });
        }]);
})();
app.controller('EmptyController', function () {

});