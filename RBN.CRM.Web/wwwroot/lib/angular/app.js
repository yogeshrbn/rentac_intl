

var app = angular.module("crmApp", ['ui.router', 'ct.ui.router.extras']);


//app.config(function ($routeProvider) {
//    // Create a reference to the route-provider to use it later on
//    $routeProviderReference = $routeProvider;
//   // $cryptoProvider.setCryptographyKey('mysupersecretkey');
//});
app.config(function ($httpProvider) {
    // $httpProvider.interceptors.push('authInterceptorService');
    $httpProvider.defaults.withCredentials = false;
});
app.config(function ($locationProvider) {
    // $httpProvider.interceptors.push('authInterceptorService');
   // $locationProvider.html5Mode(true);
});

app.config(['$stateProvider', '$futureStateProvider',  
    function ($sp, $fsp) {

        var futureStateResolve = function ($http) {

            return $http.get("../lib/angular/routes.json?d=" + new Date()).then(function (response) {

                angular.forEach(response.data, function (currentRoute) {
                    $sp.state({
                        name: currentRoute.name,
                        url: currentRoute.url ? currentRoute.url : '/' + currentRoute.name,
                        templateUrl: '../' + currentRoute.templateUrl + '?d=' + new Date().getTime(),
                        title: currentRoute.title,
                     controller: currentRoute.controller,
                        toolbar: currentRoute.toolbar,
                        newitem: currentRoute.newitem,
                        data: currentRoute.data,
                        params: currentRoute.params,
                        authorize: currentRoute.authorize,
                        warnOnLeave: currentRoute.warnOnLeave,
                        reload: true
                    });
                })
            })
        }
        $fsp.addResolve(futureStateResolve);
      
    }]);



