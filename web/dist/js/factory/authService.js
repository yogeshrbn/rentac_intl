'use strict';
angular.module('medRack').factory('authInterceptorService', ['$q', '$location', '$window', 'localStorageService', '$cookies',
    '$injector', '$cacheFactory', 'loaderService',
    function ($q, $location, $window, localStorageService,
        $cookies, $injector, $cacheFactory, loaderService) {

        var authInterceptorServiceFactory = {}
        var cache = undefined;
        if (!$cacheFactory.info().rentacCache)
            cache = $cacheFactory('rentacCache');
        else {
            cache = $cacheFactory.get('rentacCache');
        };

        var _request = function (config) {

            var $route = $injector.get('$route');
            var $state = $injector.get('$state');
            if (!config.url.endsWith('.html') && !config.skipLoader) {
                loaderService.show();
            }
            //     var accessToken = $cookies.get("TokenInfo");// $window.sessionStorage["TokenInfo"];//sessionStorage.getItem('TokenInfo');
            //var authorize = $route.current.authorize;
            var accessToken = localStorage.getItem("TokenInfo")
            if (accessToken == null || accessToken == undefined || accessToken == "undefined" || accessToken == "null") {

                if ($state.current) {
                    if (($state.current.$$state && $state.current.authorize == false) || $state.current.url == "^") {
                        return config;
                    }


                    //  $location.path('../index.html#login');
                    window.location.href = '../index.html#login';
                }

            }
            else {

                //if (!config.headrs['skip-token'])
                //    debugger
                var tokenData = JSON.parse(accessToken);
                config.headers["Authorization"] = "bearer " + tokenData.accessToken;
                // x-companyId: sessionStorage (tab-specific) overrides TokenInfo.DefaultCompanyId
                var companyId = sessionStorage.getItem('x-companyId') || tokenData.DefaultCompanyId;
                if (companyId) {
                    config.headers["x-companyId"] = companyId;
                }
                //  if (config.data.contains('refresh_token') == false)
                var cached = cache.get(config.url);
                if (cached) {
                    var deferred = $q.defer();
                    deferred.resolve(cached);
                    cached.cached = true;
                    return $q.reject(cached);

                }

                //if (SLIDING_TIMER <= 1) {
                //    SLIDING_TIMER = SESSION_DURATION;
                //    $location.path('/login');
                //}
                //else {
                //    SLIDING_TIMER = SESSION_DURATION;
                //}
            }
            //debugger
            return config;
        }
        var _response = function (response) {
            loaderService.hide();
            if (response && response.status == 200) {
                var cacheable = response.config.url.includes('ccache=true');
                if (cacheable) {
                    cache.put(response.config.url, response);
                    console.log('cached:{0}', response.config.url);
                }

            }
            else {

            }
            return response;
        }
        var _responseError = function (rejection) {
            loaderService.hide();
            if (rejection.status === 401) {


                //$window.sessionStorage.clear();
                //$window.sessionStorage["TokenInfo"] = null;
                // delete $cookies['TokenInfo'];

                localStorage.clear();
                sessionStorage.removeItem('x-companyId');
                $cookies.remove('TokenInfo', { path: '/' });
                const cookies = document.cookie.split(";");

                for (let i = 0; i < cookies.length; i++) {
                    const cookie = cookies[i];
                    const eqPos = cookie.indexOf("=");
                    const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
                }

                console.log('response error');
                //  $location.path('../index.html#login');
                if (rejection.status == 401) {
                    window.location.href = '../index.html#login';
                }



            }
            else if (rejection.status == 500) {
                loaderService.hide();
                // alert(rejection.statusText);
                return rejection;

            }
            if (rejection && rejection.cached == true) {
                var deferred = $q.defer();
                rejection.status = 200;
                deferred.resolve(rejection);
                return deferred.promise;
            }
            return $q.reject(rejection);
        }

        authInterceptorServiceFactory.request = _request;
        authInterceptorServiceFactory.responseError = _responseError;
        authInterceptorServiceFactory.response = _response;
        return authInterceptorServiceFactory;
    }]);


//-------------------

app.service('LoginService', ['$http', '$q', '$rootScope', 'AuthenticationService', 'authData', 'StorageService',
    function ($http, $q, $rootScope, authenticationService, authData, storage) {
        var userInfo;
        //  var loginServiceURL = API_URL + 'token';
        var loginServiceURL = AUTH_SERVER + 'api/auth/Login';


        var deviceInfo = [];
        var deferred;
        this.login = function (userName, password, finYear,userId) {

            deferred = $q.defer();
            //var data = "grant_type=password&username=" + userName + "&password=" + password + "&FinYear=" + finYear;
            //if (password == 'refresh_token') {
            //    var tokenIfno = authenticationService.getTokenInfo();
            //    var data = 'grant_type=refresh_token&refresh_token=' + tokenIfno.refresh_token + '&FinYear=' + finYear;
            //}
            var data = { userName: userName, password: password, FinYear: finYear, UserId: 0 };
            if (userId) {
                data.UserId = userId;
            }
            //if (password == 'refresh_token') {
            //    var tokenIfno = authenticationService.getTokenInfo();
            //    var data = 'grant_type=refresh_token&refresh_token=' + tokenIfno.refresh_token + '&FinYear=' + finYear;
            //}

            //$http.post(loginServiceURL, data, {
            //    headers:
            //    {
            //        'Content-Type': 'application/x-www-form-urlencoded', 'Access-Control-Allow-Origin': window.location.origin
            //    }
            $http.post(loginServiceURL, data, {
                headers:
                {
                    'Content-Type': 'application/json', 'Access-Control-Allow-Origin': window.location.origin
                }
            }).success(function (e) {
                if (e.Code != 200) {

                    authData.authenticationData.IsAuthenticated = false;
                    authData.authenticationData.userName = "";
                    deferred.resolve(e);

                } else {
                    //var o = response;
                    var response = e.Data._extra;
                    userInfo = {
                        accessToken: e.Data.access_token,
                        userName: response.userName,
                        UserId: response.UserId,
                        DefaultCompanyId: response.DefaultCompanyId,
                        ClientId: response.ClientId,
                        AllowSwitchCompany: response.AllowSwitchCompany,
                        RoleId: response.RoleId,
                        FullName: response.FullName,
                        FinYear: response.FinYearId,
                        FinYearStart: response.FinYearStart,
                        FinYearEnd: response.FinYearEnd,
                        refresh_token: response.refresh_token,
                        ProfilePic: response.ProfilePic,
                        lcd: e.Data.lcd,
                        CompanyStateId: response.CompanyStateId,
                        wsApp: e.Data.wsApp,
                        DefaultWareHouseId: response.DefaultWarehouseId
                    };
                    authenticationService.removeToken();
                    authenticationService.setTokenInfo(userInfo);
                    sessionStorage.setItem('x-companyId', response.DefaultCompanyId);
                    authData.authenticationData.IsAuthenticated = true;
                    authData.authenticationData.userName = response.userName;
                    storage.setData('menus', e.Data.menus);
                    //StartSessionSlider();

                    $rootScope.$emit('onLogin', '1');
                    // $rootScope.$broadcast('onLogin', 1);
                    deferred.resolve(null);
                }
            })
                .error(function (err, status) {

                    authData.authenticationData.IsAuthenticated = false;
                    authData.authenticationData.userName = "";
                    deferred.resolve(err);
                });
            return deferred.promise;
        }
        this.logOut = function () {

            authenticationService.removeToken();
            authData.authenticationData.IsAuthenticated = false;
            authData.authenticationData.userName = "";
        }
    }
]);

//----------------
app.factory('authData', [function () {
    var authDataFactory = {};

    var _authentication = {
        IsAuthenticated: false,
        userName: ""
    };
    authDataFactory.authenticationData = _authentication;

    return authDataFactory;
}]);

//----------------
app.service('AuthenticationService', ['$http', '$q', '$window', '$cookies',
    function ($http, $q, $window, $cookies) {
        var tokenInfo;

        this.setTokenInfo = function (data) {
            tokenInfo = data;

            // $window.sessionStorage["TokenInfo"] = JSON.stringify(tokenInfo);
            //  $cookies.put('TokenInfo', JSON.stringify(tokenInfo));

            localStorage.setItem('TokenInfo', JSON.stringify(tokenInfo));
        }
        //this.setFinYear = function (data) {

        //    // $window.sessionStorage["TokenInfo"] = JSON.stringify(tokenInfo);
        //    $cookies.put('finYear', JSON.stringify(tokenInfo));
        //}
        //this.getFinYear = function (data) {
        //    var finYear = $cookies.get("finYear")
        //    return JSON.parse(finYear);
        //}

        this.getTokenInfo = function () {

            if (tokenInfo) {
                MinDate = tokenInfo.FinYearStart;
                MaxDate = tokenInfo.FinYearEnd;

            }
            return tokenInfo;
        }

        this.removeToken = function () {
            tokenInfo = null;
            sessionStorage.removeItem('x-companyId');
            //  localStorage.clear();
            localStorage.removeItem('TokenInfo');
            //$window.sessionStorage.clear();
            //$window.sessionStorage["TokenInfo"] = null;
            // delete $cookies['TokenInfo'];
            $cookies.remove('TokenInfo', { path: '/' });
            const cookies = document.cookie.split(";");

            for (let i = 0; i < cookies.length; i++) {
                const cookie = cookies[i];
                const eqPos = cookie.indexOf("=");
                const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
            }
        }

        this.isAuthenticated = function () {
            this.init();
            if (tokenInfo) {
                return true;
            }
            return false;
        }
        this.init = function () {
            //if ($window.sessionStorage["TokenInfo"]) {
            //    tokenInfo = JSON.parse($window.sessionStorage["TokenInfo"]);
            //}
            tokenInfo = null;
            //if (!$cookies.get("TokenInfo")) {
            //    return;
            //}

            var tokn = localStorage.getItem('TokenInfo')
            if (!tokn) {
                return;
            }
            //tokenInfo = JSON.parse($cookies.get("TokenInfo"));
            tokenInfo = JSON.parse(localStorage.getItem('TokenInfo'));
            if (tokenInfo) {
                //   tokenInfo = JSON.parse($cookies.get("TokenInfo"));
                if (tokenInfo) {
                    MinDate = tokenInfo.FinYearStart;
                    MaxDate = tokenInfo.FinYearEnd;


                }
            }
        }

        // Re-sync token from localStorage (e.g. when another tab changes company)
        this.refreshFromStorage = function () {
            this.init();
        }

        this.setHeader = function (http) {
            delete http.defaults.headers.common['X-Requested-With'];
            if ((tokenInfo != undefined) && (tokenInfo.accessToken != undefined) && (tokenInfo.accessToken != null) && (tokenInfo.accessToken != "")) {
                http.defaults.headers.common['Authorization'] = 'Bearer ' + tokenInfo.accessToken;
                http.defaults.headers.common['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
            }
        }
        this.validateRequest = function () {
            var url = serviceBase + 'api/home';
            var deferred = $q.defer();
            $http.get(url).then(function () {
                deferred.resolve(null);
            }, function (error) {
                deferred.reject(error);
            });
            return deferred.promise;
        }
        this.init();
    }
]);
app.service('StorageService', ['localStorageService', '$q', '$window', '$cookies', '$cacheFactory',
    function (localStorageService, $q, $window, $cookies, $cacheFactory) {

        var cache = undefined;
        if (!$cacheFactory.info().rentacCache)
            cache = $cacheFactory('rentacCache');
        else {
            cache = $cacheFactory.get('rentacCache');
        }

        this.putCache = function (key, value) {
            cache.put(key, value);
        }
        this.getCache = function (key) {
            return cache.get(key);
        }
        this.setData = function (key, value) {
            localStorage.setItem(key, value);
        }
        this.setCacheData = function (key, value) {
            localStorage.setItem(key, value);
        }
        this.getData = function (key) {
            return localStorage.getItem(key);
        }
        this.clearAll = function () {
            return localStorage.clear();
        }
        this.setFinYear = function (value) {

            this.setData('finyear', JSON.stringify(value));
        }
        this.getFinYear = function () {

            var finYear = this.getData('finyear');
            return JSON.parse(finYear);
        }
        this.selectFinYear = function (finYear) {

            var allFinyeaar = this.getFinYear();
            var o = allFinyeaar.filter(o => o.FinYear == finYear);
            if (o.length > 0) {
                allFinyeaar.map(o => o.Selected = false);
                o[0].Selected = true;
            }
            this.setFinYear(allFinyeaar);

        }
        this.getSelectedFinYear = function () {

            var finYear = this.getFinYear();
            if (!finYear) {
                return;
            }
            var o = finYear.filter(o => o.Selected == true);
            if (o.length > 0) {
                return o[0];
            }
            else return finYear[0];

        }
    }]);
