app.directive('ejMultiselectCheckbox', ['$timeout', '$parse', function ($timeout, $parse) {
    return {
        restrict: 'A',
        scope: {
            options: '=?',
            dataSource: '=?',
            fields: '=?',
            selectedOptions: '=?',
            onChange: '&?',
            placeholder: '@'
        },
        link: function (scope, element, attrs) {
            if (!ej || !ej.dropdowns || !ej.dropdowns.MultiSelect) {
                console.error('Syncfusion ej2 script not loaded before this directive ran. Check script order.');
                return;
            }

            element.css({ width: '100%', minWidth: '220px' });

            var MultiSelect = ej.dropdowns.MultiSelect;
            // Only two-way write-back when the bound expression is assignable.
            // A constant expression (e.g. selected="true") is not assignable, so we
            // never mutate scope.selected for it and thus never trigger $compile:nonassign.
            var isAssignable = !!$parse(attrs.selectedOptions).assign;

            function getDataSource() {
                return scope.options || scope.dataSource || [];
            }

            function getFields() {
                return scope.fields || { text: 'text', value: 'value' };
            }

            var multiSelectObj = new MultiSelect({
                dataSource: getDataSource(),
                fields: getFields(),
                mode: 'CheckBox',
                showSelectAll: true,
                showDropDownIcon: true,
                allowFiltering: true,
                filterBarPlaceholder: 'Search',
                closePopupOnSelect: false,
                placeholder: scope.placeholder || 'Select options',
                value: scope.selectedOptions || [],
                change: function (args) {
                    scope.$applyAsync(function () {
                        var val = args.value || [];

                        if (isAssignable) {
                            // Two-way binding propagates this back to the correct
                            // origin scope/property, even through md-dialog wrapper scopes.
                            scope.selectedOptions = val;
                        }
                        if (attrs.onChange) {
                            scope.onChange({ selectedOptions: val });
                        }
                    });
                }
            });

            multiSelectObj.appendTo(element[0]);

            function refreshDataSource(newVal) {
                var data = angular.isArray(newVal) ? newVal : getDataSource();
                multiSelectObj.dataSource = data;
                multiSelectObj.fields = getFields();
                multiSelectObj.dataBind();

                if (scope.selected && scope.selected.length) {
                    multiSelectObj.value = scope.selected;
                    multiSelectObj.dataBind();
                }
            }

            function refreshSelected(newVal) {
                 
                if (!angular.isArray(newVal)) {
                    return;
                }
                multiSelectObj.value = newVal;
                multiSelectObj.dataBind();
            }

            $timeout(function () {
                refreshDataSource(getDataSource());
            }, 0);

            scope.$watchCollection('options', refreshDataSource);
         //   scope.$watchCollection('dataSource', refreshDataSource);

            scope.$watch('fields', function (newVal) {
                if (newVal) {
                    multiSelectObj.fields = newVal;
                    multiSelectObj.dataBind();
                }
            }, true);

            scope.$watchCollection('selectedOptions', refreshSelected);

            scope.$on('$destroy', function () {
                multiSelectObj.destroy();
            });
        }
    };
}]);
