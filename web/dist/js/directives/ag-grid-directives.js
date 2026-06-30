app.factory('myCustomEditor', function ($compile, $rootScope) {
    return function () {
        let scope;
        let element;
    
        function isCharNumeric(charStr) {
            return !!/\d/.test(charStr);
        }

        // Editor component definition
        this.init = function (params) {
          
            // Create a new scope for the editor and attach to root scope
            scope = $rootScope.$new();
            scope.value = params.value;

            // Use the provided $compile to link your template
            element = angular.element(
                '<input type="text" class="form-control" ng-model="value" ng-keydown="onKeyDown($event)">'
            );
            $compile(element)(scope);

            // Function to handle key presses
            scope.onKeyDown = function (event) {
                if (!isCharNumeric(event.key)) {
                    event.stopPropagation();
                }
                alert('hi');
            };
        };

        // Return the element to AG Grid
        this.getGui = function () {
            return element[0];
        };

        // Method to return the new value to AG Grid
        this.getValue = function () {
            return scope.value;
        };
    };
});