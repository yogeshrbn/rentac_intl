// create the controller and inject Angular's $scope
// set for Route Controller
app.controller('challan', function ($scope, $routeParams) {
    // create a message to display in our view 
   
    $scope.page = $routeParams.pagename;
    // $scope.message = "(',')---I am on " + $routeParams.pagename + " page---(',')";
    $scope.message = "Challan";
});