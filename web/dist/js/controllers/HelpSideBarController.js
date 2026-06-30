
app.controller("HelpSideBarController", ['$scope', '$stateParams', '$rootScope',
    function ($scope, $stateParams, $rootScope) {
        $rootScope.$on('$stateChangeSuccess', function (event, current, previous) {
            //if (angular.isDefined(current.$$state)) {
           
           
            //    //  $rootScope.title = Utility.parseTitle(current.$$route.title, $routeParams);
            //    $scope.title = current.$$state().title
            //}
        });

    }]);