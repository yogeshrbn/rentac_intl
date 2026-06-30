var app = angular.module("medRack", ['ngRoute', 'ui.router', 'ct.ui.router.extras', 'ngAnimate', 'angular-loading-bar', '720kb.datepicker', 'LocalStorageModule',
    'cfp.loadingBar', 'angularUtils.directives.dirPagination', 'angucomplete-alt', 'angular.filter', 'ngMaterial', 'chart.js',
    'ui.bootstrap', 'ngCookies', 'ngFileSaver', 'toaster', 'webcam', 'mdo-angular-cryptography', 'ngCkeditor', 'ngSanitize', 'ngTagsInput'
]);


CKEDITOR.config.customConfig = '/dist/js/helpers/ckEditor/config.js';
//'angular-loading-bar', 'cfp.loadingBar', 'ngToast'
app.config(function ($routeProvider, $cryptoProvider) {
    // Create a reference to the route-provider to use it later on
    $routeProviderReference = $routeProvider;
    $cryptoProvider.setCryptographyKey('mysupersecretkey');
});
app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
    $httpProvider.defaults.withCredentials = false;

});
app.factory('httpInterceptor', function ($q) {

    return {
        responseError: function (response) {

            if (response.status === -1) {
                alert("Request failed before reaching server:", response.config.url);
            }
            return $q.reject(response);
        }
    };
});

app.config(function ($httpProvider) {

    $httpProvider.interceptors.push('httpInterceptor');
});

app.config(['$stateProvider', '$futureStateProvider',
    function ($sp, $fsp) {
        var futureStateResolve = function ($http) {
            return $http.get("../config/routes.json?d=" + new Date()).then(function (response) {
                angular.forEach(response.data, function (currentRoute) {
                    $sp.state({
                        name: currentRoute.name,
                        url: currentRoute.url ? currentRoute.url : '/' + currentRoute.name,
                        templateUrl: currentRoute.templateUrl + '?d=' + new Date().getTime(),
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
        console.log($fsp);

    }]);

document.write('<script src="../dist/js/controllers/controllerLoader.js"></script>');

app.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {

    // cfpLoadingBarProvider.includeSpinner = false;
}]);

app.filter('formatDate', function ($filter) {
    return function (_date) {
        var d = $filter('date')(_date, "dd-MM-yyyy");
        if (d == '01-01-0001') {
            return '';
        }
        if (d != undefined) {
            if (d.length > 10) {
                d = d.substring(0, 10);
            }
        }
        if (d) {
            return d.split(' ')[0];//$filter('date')(_date, "dd/MM/yyyy");
        }
        else return "";
    }

});

/**
 * Actions that should be invoken, once the application is loaded and runable.
 */
app.run(['$rootScope', '$window', 'AuthenticationService', function ($rootScope, $window, AuthenticationService) {
    // Multi-company: expose getTokenInfo globally so controllers always get current company
    $rootScope.getTokenInfo = function () {
        return AuthenticationService.getTokenInfo();
    };
    // Sync token across tabs when company changes in another tab
    angular.element($window).on('storage', function (e) {
        if (e.originalEvent && e.originalEvent.key === 'TokenInfo') {
            AuthenticationService.refreshFromStorage();
            $rootScope.$broadcast('onCompanyChangedFromStorage');
            $rootScope.$broadcast('onCompanySelected');
        }
    });
}]);

app.run(function ($rootScope, $templateCache, $location, $routeParams, $http, $route, $state, $cacheFactory) {

    // Get routes from file, iterate them and load the $routeProvider-reference
    //set the rootscope and global http service to the global variables. These variables will be accessed from 
    //all controllers and data objects.
    ROOT_SCOPE = $rootScope;
    HTTP_SERVICE = $http;
    if (!$cacheFactory.info().rentacCache)
        CACHE_FACTORY = $cacheFactory('rentacCache');


    $rootScope.PAGE_SIZE = PAGE_SIZE;
    $rootScope.API_URL = API_URL;
    $rootScope.SERVER_URL = SERVER_URL;
    $rootScope.Message = new $.Message();
    $rootScope.SMS_REMINDER = 1;
    $rootScope.EMAIL_REMINDER = 2;
    $rootScope.route = $route;
    var d = convertDate(new Date()).split('/');
    $rootScope.Today = d[2] + '-' + d[1] + '-' + d[0] + "T00:00:00";
    $rootScope.IRPCONSULTANT = 'BVM IT Consulting Services India Private Limited'
    $http.get('../config/routes.json').success(function (routes) {

        for (var j = 0; j < routes.length; j++) {
            var currentRoute = routes[j];

            //$routeProviderReference.when(currentRoute.name, {
            //    templateUrl: currentRoute.templateUrl + '?d=' + new Date().getTime(),
            //    title: currentRoute.title,
            //    controller: currentRoute.controller,
            //    toolbar: currentRoute.toolbar,
            //    newitem: currentRoute.newitem,
            //    data: currentRoute.data,
            //    authorize: currentRoute.authorize,
            //    warnOnLeave: currentRoute.warnOnLeave

            //});
        }

        //$state.reload();
        // $routeProviderReference.otherwise({ templateUrl: 'pages/templ/login.html' });

        //reloads the browser after loading the configuration. It will help to show the header title of each page
        // $route.reload();
    });
    $rootScope.$on('$stateChangeStart', function (event, current, previous) {

        if (previous != undefined) {

            var scopeWarning
            if (previous.scope) {
                scopeWarning = previous.scope.warnOnLeave;
            }
            if (previous.warnOnLeave && (scopeWarning == undefined || scopeWarning == true)) {
                var c = confirm('You have un-saved data on the current screen!\nAre you sure to leave?');
                if (!c) {
                    event.preventDefault();
                }
            }
        }
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });
    /**
     * On successful route-change, set the title of the current route in the root-scope.
     */
    var PREVIOUS_ROUTE;
    $rootScope.$on('$stateChangeSuccess', function (event, current, previous) {
        //to go to the previous route or screen/page.
        PREVIOUS_ROUTE = previous;

        if (current.data.forceBuy == undefined || current.data.forceBuy == true) {
            if ($rootScope.forceBuy) {
                $rootScope.$broadcast('forceBuy', true);
            }
        }
        if (angular.isDefined(current.$$state)) {
            $templateCache.remove(current.templateUrl);
            //  $rootScope.title = Utility.parseTitle(current.$$route.title, $routeParams);
            $rootScope.title = current.$$state().title

            // $('#toolbar-section').empty();
            //$('.content-header').hide();
            // alert(current.$$route.toolbar);
            // alert($(".page-header-text"));
            $(".page-header-text").html($rootScope.title);
            if (current.$$state().toolbar == "yes") {
                //loads the toolbar based on the template and passes the last route object to it.
                //  $('#toolbar-section').load('templ/toolbar.html?d' + new Date().getTime(), function () { (new $.Toolbar({ Scope: $rootScope, Location: $location, Current: current, Previous: previous }).intit()); });
                // $('#toolbar-section').html('<toolbar />');
                // $('.content-header').show();
                $('#toolbarCtrls').show();
            } else {
                $('#toolbarCtrls').hide();
            }

        }
        // debugger
        $rootScope.MinDate = MinDate;
        $rootScope.MaxDate = MaxDate;
        // alert($rootScope.MinDate);
        var config = new $.Config();
        if (current.authorize != false) {
            config.GetByCategory(function (e) {
                var response = e.data;
                if (response.Data != null && response.Data) {
                    if (response.Data.length > 0) {

                        var decimalplaces = response.Data.find(o => o.SubCategory == 'masters' &&
                            o.Category == 'general' && o.Key == 'decimalplaces');
                        if (decimalplaces) {
                            DECIMAL_PLACES = parseInt(decimalplaces.Value);
                        }
                    }
                }
            }, 'general');
        }
        if (current.url) {
            var user = new $.User({});
            var route = current.url.replace('/', '');

            if (GLOBAL_TOKEN && GLOBAL_TOKEN.RoleId == 1) {
                //no check for superadmin
                event.targetScope.AccessData = { Add: true, Edit: true, View: true, Delete: true };
            }
            else {

                if ((current.authorize != false || current.authorize == undefined) && route != 'home' && route != 'signup') {
                    user.GetRouteAccess(function (e) {
                        if (e.data.Data == null) {
                            $location.path('/access');
                        }
                        else {
                            event.targetScope.AccessData = e.data.Data;
                        }
                    }, { Route: route });
                }
            }

            var tokenInfo = event.targetScope.getTokenInfo();
            var validDate = true;
            if (tokenInfo) {

                var cDate = new Date(new Date().toDateString());
                var start = new Date(tokenInfo.FinYearStart);
                var end = new Date(tokenInfo.FinYearEnd);
                // validDate = cDate >= start && cDate <= end

            }
            if (validDate)
                event.targetScope.AccessData = { Add: true, Edit: true, View: true, Delete: true };
            else {
                event.targetScope.AccessData = { Add: false, Edit: false, View: true, Delete: false };
            }

        }
    });
});

//app.directive('medKey', function () {

//    var index = 1;
//    return {
//        restrict: 'A',
//        link: function (scope, elem, atr) {
//            elem.bind('load', function () {

//            });
//        }
//    }
//});
app.run(['$rootScope', 'FirebaseService', function ($rootScope, FirebaseService) {
    // Initialize Firebase when app starts
    FirebaseService.initialize();

    // Listen for FCM messages globally
    $rootScope.$on('fcmMessageReceived', function (event, payload) {
        console.log('Global FCM message:', payload);
    });

    // Listen for token updates
    $rootScope.$on('fcmTokenUpdated', function (event, token) {
        console.log('FCM token updated:', token);
    });
}]);

app.directive('medKey', function () {

    var index = 1;
    var KEY_DOWN = 40, KEY_UP = 38, KEY_DEL = 46;
    return {
        restrict: 'A',

        link: function (scope, elem, attr) {
            //  var elmentScope = scope;
            //set a default index to the table to support keyboard shortcuts
            $(elem).attr('tabindex', 100);
            //set the focus to the table element. the keyboard event will work only when the element has focus.
            $(elem).focus();
            scope.$watch(function (scope) { return $(elem).html(); }, function (newval, oldval) {
                //select the first row of the table (exclude the header row. header row index is 0)
                selectRow();
                //handles the onmouse down event on the row to select the row on mouse click
                $(elem).find('tr').bind('mousedown', function (event) {

                    //  alert(event);
                    //if header row then increament by one.
                    index = $(this).index();
                    var isTbody = this.parentElement.nodeName.toUpperCase() === 'TBODY';
                    if (index == 0 && !isTbody) {
                        index = 1;
                    }
                    else {
                        selectRow();
                    }
                });

                //handles the onmouse down event on the row to select the row on mouse click
                $(elem).find('tr').bind('dblclick', function (event) {

                    //if header row then increament by one.
                    index = $(this).index();
                    // $($(elem)[0].rows[index]).find('.fa-edit').click();
                    //edit on double click
                    edit(elem, index);
                    event.stopImmediatePropagation();
                });

                $(elem).find('input').bind('focus', function (event) {
                    index = $(this).closest('tr').index();
                    console.log(index);
                    selectRow();
                    //  alert(index);
                });
            });
            elem.bind("keydown", function (event, scope) {

                var $focused = $(':focus');
                var tdIndex = $focused.parent().index();
                var isAutoComplete = false;
                var empty = isEmpty($focused);

                // var isInputText = $focused[0] instanceof HTMLInputElement && $focused[0].type == 'text'
                if ($focused.attr('type') == 'text') {
                    $focusssedTd = $focused.parent();
                }
                else {
                    isAutoComplete = $focused.hasClass('autocomplete');
                }

                //if autocomplete then get the rowIndex of the currently focused textbox 
                if (isAutoComplete) {
                    index = $focused.parent().parent().index();
                }

                //if TD contains autocomplete textbox then don't move to next row.
                if (isAutoComplete && event.keyCode == KEY_DOWN && !empty) return;

                //handles up and down arrow keys
                if (event.keyCode == KEY_UP || event.keyCode == KEY_DOWN) {

                    //if up and index is greater then the first row (exclude the header row)
                    if (event.keyCode == KEY_UP && index > 1) {
                        index--;
                    }
                    else if (event.keyCode == KEY_DOWN) {
                        //if last row then set to first row.
                        if (index == $(elem)[0].rows.length - 1) {
                            index = 1;
                        }
                        else
                            index++;
                    }
                    //highlight the row
                    selectRow();
                    $($($(elem)[0].rows[index]).find('td')[tdIndex]).find('input').focus();
                }

                else if (event.keyCode == 13) {//enter key
                    //edit the current selected row.
                    edit(elem, index);
                    $($($(elem)[0].rows[index]).find('td')[tdIndex]).find('input').focus();
                }
                else if (event.keyCode == KEY_DEL) {
                    deleteSelectedRow(elem, index);
                    //   $($(elem)[0].rows[index]).find('.fa-trash').click();
                }


                // $($($(elem)[0].rows[index]).find('td')[tdIndex]).find('input').focus();

            });
            //edits the current selected item.
            function edit(elem, index) {

                //check if the the edit icon is hidden or not. it will be hidden if the item is not editable or user 
                //does not have edit permissions.
                var editIcon = $($(elem)[0].rows[index]).find('.fa-edit').parent().not("[ng-show='false']");
                if (editIcon != undefined) {

                    $($(elem)[0].rows[index]).find('.fa-edit').parent().not("[ng-show='false']").click();
                }
                ;

                var id = $($(elem)[0].rows[index])[0].attributes['id'];

                if (id) {


                    scope.editRow(id.value);
                }

                if (scope != undefined && scope.edit) {
                    scope.edit(elem, index);
                }

            }

            function deleteSelectedRow(elem, index) {

                var onDelete = $($(elem)[0].rows[index]).attr('ng-delete');

                var fn_delete = scope.$eval(onDelete, { index: index });
                // alert(fn_delete);
            }

            function deleteRow() {
                var rowDeleted = $($(elem)[0].rows[index]).attr('rowDeleted');
                alert(rowDeleted);
            }

            function selectRow() {

                $(elem).find('tr').removeClass('selected');
                $($(elem)[0].rows[index]).addClass('selected');
                if (scope != undefined) {
                    if (scope.RowSelected != undefined) {
                        scope.RowSelected(index - 1);
                    }
                }
            }
        }
    };
});
app.filter('parseDate', function () {
    return function (input) {

        var d = Date.parse(input);

        return new Date(input);
    };
});
//--------select dropdown
app.directive('searchableselect', function ($parse) {

    return {
        restrict: 'E',
        templateUrl: 'directive/multiselect.html?d=' + new Date().getTime(),
        link: function (scope, elem, attr) {
            //var ItemSelected = scope.ItemSelected();
            //debugger
            elem.addClass('searchableselect');
            if (attr.tabindex) {
                //  $(elem).find('select').attr('tabindex', attr.tabIndex);
            }

            $(elem).find('select').attr('id', $(elem).attr('ng-id'));
            $(elem).find('select').attr('name', $(elem).attr('ng-name'));
            if (attr.ngDisabled) {
                var disabledGetter = $parse(attr.ngDisabled);
                scope.$watch(function () {
                    try { return !!disabledGetter(scope); } catch (ex) { return false; }
                }, function (disabled) {
                    $(elem).find('button').prop('disabled', disabled);
                    $(elem).find('select').prop('disabled', disabled);
                });
            }
            scope.$watch(function (newVal, oldVal) {

                var dataField = $(elem).attr('data-field');

                return scope[dataField]
            },
                function (newval, oldval) {
                    // debugger
                    //if (newval == undefined || newval.length == 0) {
                    //    return;
                    //}
                    //select the first row of the table (exclude the header row. header row index is 0)
                    var valueField = $(elem).attr('data-val');
                    var textField = $(elem).attr('data-text');
                    var primaryTextField = $(elem).attr('data-primarytext');

                    var descriptionField = $(elem).attr('description-field');
                    var addAll = $(elem).attr('data-all');
                    $(elem).find('select').attr('ng-model', $(elem).attr('ng-model'));
                    $(elem).attr('id', $(elem).attr('ng-id'));
                    $(elem).attr('name', $(elem).attr('ng-name'));

                    var multiple = $(elem).attr('ng-multiple');

                    if (multiple == 'true') {


                        $(elem).find('select').attr('multiple', true);
                    }

                    $(elem).attr('on-loaded', $(elem).attr('on-loaded'));
                    $(elem).attr('on-ItemSelected', $(elem).attr('on-ItemSelected'));
                    var blur = $(elem).attr('ng-blur');
                    if (blur) {

                        $(elem).find('select').attr('onblur', $(elem).attr('ng-blur'));
                    }
                    if (newval != undefined) {

                        $(elem).find('select').empty();
                        var firstVal = 0;
                        var option = '<option data-subtext="Please select"  value="0">';
                        option += '</option>';
                        //get the model value assigned to the selectpicker

                        $(elem).find('select').append(option);
                        // var selectedVal = (scope.$eval($(elem).attr('ng-model')));

                        for (var i = 0; i < newval.length; i++) {
                            if (i == 0) {
                                firstVal = newval[i][valueField];
                            }

                            var dataContent = '';
                            if (descriptionField) {
                                dataContent = newval[i][descriptionField];
                            }
                            if (dataContent == null) {
                                dataContent = '';
                            }
                            var text = newval[i][textField];
                            if (primaryTextField) {
                                var primaryText = newval[i][primaryTextField];
                                if (primaryText !== null && primaryText !== "") {
                                    text = primaryText;
                                }
                            }
                            if (textField == 'SiteAddress') {
                                if (newval[i]['Project'] && newval[i]['Project'] != '') {
                                    text = newval[i]['Project'] + ', ' + newval[i]['SiteAddress'];
                                }
                            }
                            var searchTokens = (text != null && text !== undefined) ? String(text) : '';
                            if (descriptionField && dataContent) {
                                searchTokens = searchTokens + ' ' + dataContent;
                            } else if (newval[i]['Code']) {
                                searchTokens = searchTokens + ' ' + (newval[i]['Code'] || '');
                            }

                            if (searchTokens != '') {
                                option = '<option data-subtext="' + dataContent + '" data-tokens="' + (searchTokens || '').replace(/"/g, '&quot;') + '" value="' + newval[i][valueField] + '">' + (text != null ? text : '');
                                option += '</option>';

                                $(elem).find('select').append(option);
                            }
                        }

                        //select the item to show the already selected item. This is used on the edit mode.
                        // $(elem).find('select').selectpicker();

                        /*
                        if (selectedVal == 0) {
                            selectedVal = firstVal;
                        }
                        var model = $parse($(elem).attr('ng-model'));
                        if (selectedVal != undefined) {
                         //   alert(selectedVal);
                            //sets the selected item.
                          //  model.assign(scope, selectedVal);
                            // var ItemSelected = $(elem).attr('on-ItemSelected');
                            // $(elem).find('select').selectpicker('val', selectedVal);
                            //scope[ItemSelected](selectedVal);
                            // alert(selectedVal);
                        } */
                        $(elem).find('select').selectpicker('refresh');
                    }
                    $(elem).on('loaded.bs.select', function (e) {
                        //$(elem).find('button').attr('tabIndex', $(elem).attr('tabindex'));
                        //$(elem).removeAttr('tabindex');
                        var onLoaded = $(elem).attr('on-loaded');

                        if (onLoaded != undefined && onLoaded != null) {
                            scope.$eval(onLoaded, {});
                        }
                    });

                    $(elem).find('select').on('changed.bs.select', function (e, clickedIndex, newValue, oldValue) {

                        scope.$apply(function () {

                            var model = $parse($(elem).attr('ng-model'));

                            var selectedValue = e.target.value;//parseInt(e.target.value);


                            if (model.assign) {
                                model.assign(scope, selectedValue);
                            }
                            var a = selectedValue;
                            var ret_type = $(elem).attr('ng-ret_type');
                            if (ret_type == 'index') {
                                a = e.target.selectedIndex;
                            }
                            var ItemSelected = $(elem).attr('on-ItemSelected');
                            //to be continue
                            if (ItemSelected != undefined && ItemSelected != null) {
                                //  scope[ItemSelected](a);
                                if (a < 0) return;
                                scope.$eval(ItemSelected, { itemId: a });

                            }
                            //debugger
                            var validation_elem = $('#' + $(elem).attr('id') + '-error');
                            if (validation_elem) {
                                $(validation_elem).remove();
                            }
                        });
                    });
                    scope.$watch($(elem).attr('ng-model'), function () {
                        var modelValue = scope.$eval($(elem).attr('ng-model'));
                        $(elem).find('select').selectpicker('val', modelValue);
                    });
                });
            //  $('.selectpicker').selectpicker();
        }
    };
});
app.directive('recnav', function ($parse) {

    return {
        restrict: 'E',
        templateUrl: 'directive/recnav.html?d=' + new Date().getTime(),
        link: function (scope, elem, attr) {
            var navClicked = $(elem).attr('ng-navClick');
            elem.find('div[navType!="undefined"]').on('click', function (event) {
                var idx = $(this).attr('navType');
                if (idx != undefined) {
                    scope.$eval(navClicked, { index: idx });
                }
                event.stopImmediatePropagation();
            });
        }
    };
});
app.directive('pageheader', function ($parse) {

    return {
        restrict: 'E',
        templateUrl: 'directive/pageheader.html?d=' + new Date().getTime(),
        link: function (scope, elem, attr) {
            elem.find('div[class="page-header-text"]').html(attr.heading);

        }
    };
});
app.directive('maskednumber', function () {
    return {
        restrict: 'C',
        require: 'ngModel',
        link: function ($scope, elem, attr, ngModelCtrl) {


            $(elem).val(0);
            $(elem).addClass('text-right');

            if (attr.phoneformat) {
                $(elem).removeClass('text-right');
                decimals = '';
            }
            $(elem).on('blur', function () {

                var decimals = '.00';
                var nodecimal = attr.nodecimal;

                var decimalplaces = 2;
                if (DECIMAL_PLACES) {
                    decimalplaces = DECIMAL_PLACES
                }
                if (!decimalplaces) {
                    decimalplaces = 2;
                }

                if (decimalplaces) {
                    decimals = '.' + '0'.repeat(decimalplaces);
                }
                if (nodecimal != undefined) {
                    decimals = '';
                }
                if (attr.phoneformat) {
                    $(elem).removeClass('text-right');
                    decimals = '';
                }
                ////for two decimals do not add decimal attribute
                //if (!decimals    ) {
                //    decimals = '.00';
                //}
                //else {
                //    decimals = '';
                //}

                //var val = $(this).val();
                //var fragment = $(this).val().split('.');
                //if (fragment.length == 1 && decimals) {
                //    val = (isNaN(parseInt(val)) ? 0 : val) + decimals;
                //}
                //else {
                //    fragment[0] = isNaN(parseInt(fragment[0])) ? 0 : fragment[0];
                //    val = fragment.join('.');
                //    //replicate trailling zeros(0) if not

                //    if (fragment.length > 1) {
                //        var dl = fragment[1].length;
                //        if (dl <= 2) {
                //            var z = Array((2 - dl) + 1).join('0');
                //            if (dl < 3) {
                //                val = val + z;
                //            }
                //        }
                //    }
                //}
                var val = $(this).val();
                var fragment = val.split('.');

                // Check if there's a decimal point but nothing after it
                if (fragment.length === 2 && fragment[1] === '') {
                    val = val + decimals;  // Add 0 after decimal
                }
                // Or if there's no decimal point but decimals should be shown
                else if (fragment.length === 1 && decimals) {
                    val = (isNaN(parseInt(val)) ? '0' : val) + decimals;
                }

                $(this).val(parseInt(val));
                var ngModel = this.getAttribute('ng-model');
                if (ngModel) {

                    ngModelCtrl.$setViewValue(val);
                    ngModelCtrl.$render();
                }

            });
            $(elem).on('keypress', function (e) {

                //restrict to only numbers and decimal

                //if ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 46) {
                //    //restrict to only one . character
                //    if (e.keyCode == 46) {
                //        if ($(elem).val().split('.').length > 1) {
                //            e.preventDefault();
                //        }
                //    }
                //    //var fraction = $(elem).val().split('.')[1].length;
                //    //if (fraction == 3) {
                //    //    e.preventDefault();
                //    //}
                //}
                //else {
                //    e.preventDefault();
                //}

                var currentValue = $(elem).val();
                var key = e.key;

                // Allow control keys
                if ([8, 9, 13, 27, 46].indexOf(e.which) !== -1) return true;
                var allowNegative = attr.allownegative !== undefined;
                // Allow minus sign only at the beginning
                if (e.which === 45) { // minus sign
                    // Allow minus if cursor is at position 0 and no minus sign exists
                    if (allowNegative && this.selectionStart === 0 && currentValue.indexOf('-') === -1) {
                        return true;
                    }
                    e.preventDefault();
                    return false;
                }

                // If there's already a decimal point
                if (currentValue.indexOf('.') !== -1) {
                    var decimalPart = currentValue.split('.')[1];
                    // Prevent typing if already have 3 decimal places
                    if (decimalPart.length >= DECIMAL_PLACES) {
                        e.preventDefault();
                        return false;
                    }
                }

                // Only allow numbers and single decimal point
                if ((e.which < 48 || e.which > 57) && e.which !== 46) {
                    e.preventDefault();
                    return false;

                }

                // Prevent multiple decimal points
                if (e.which === 46 && currentValue.indexOf('.') !== -1) {
                    e.preventDefault();
                    return false;
                }

                return true;

            });
            $(elem).on('focus', function () {
                //   $(this).select();
            });
        }

    };
});

app.directive('dateControl', function () {
    return {
        restrict: 'C',

        link: function (scope, elem, attr) {
            $(elem).attr('data-date-format', 'dd/mm/yyyy');
            $(elem).datepicker({});

        }
    };
});
app.directive('btn', function () {
    return {
        restrict: 'C',
        scope: {},
        link: function (scope, elem, attr) {
            var _scope = scope;

            $(elem).bind('click', function (e, _scope) {
                //debugger

            })
        }
    };
});
app.directive('input', function () {
    return {
        restrict: 'E',
        link: function (scope, elem, attr) {
            $(elem).attr('autocomplete', 'off');

            $(elem).on('focusout', function (e) {

                var maxDateAttr = $(elem).attr('maxDate');
                var minDateAttr = $(elem).attr('minDate');
                var val = e.target.value;
                var f = $(":focus")
                if (maxDateAttr) {

                    var maxDate = new Date(maxDateAttr);
                    maxDate.setHours(0, 0, 0, 0);
                    let dateString = val
                    let [day, month, year] = dateString.split('/')
                    const dateObj = new Date(+year, +month - 1, +day)
                    dateObj.setHours(0, 0, 0, 0);

                    //if (dateObj > maxDate) {
                    //    alert('Date can not be ahead of ' + maxDate.toDateString());
                    //    e.target.value = '';
                    //    $(elem).focus();
                    //}

                    // $('._720kb-datepicker-calendar').removeClass('_720kb-datepicker-open');
                }
                if (minDateAttr) {

                    var minDate = new Date(minDateAttr);
                    minDate.setHours(0, 0, 0, 0);
                    let dateString = val
                    let [day, month, year] = dateString.split('/')
                    const dateObj = new Date(+year, +month - 1, +day)
                    dateObj.setHours(0, 0, 0, 0);

                    //if (dateObj < minDate) {
                    //    alert('Date can not be less than ' + minDate.toDateString());
                    //    e.target.value = '';
                    //    $(elem).focus();
                    //}
                    // $('._720kb-datepicker-calendar').removeClass('_720kb-datepicker-open');
                }
            })
        }
    }
});
app.directive('noSpace', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            noSpace: '@' // Optional configuration
        },
        link: function (scope, element, attrs, ngModel) {

            var allowLeadingSpace = scope.noSpace === 'allow-leading';
            var allowTrailingSpace = scope.noSpace === 'allow-trailing';

            function removeSpaces(value) {
                if (!value) return value;

                var stringValue = value.toString();

                if (allowLeadingSpace && allowTrailingSpace) {
                    return stringValue.replace(/\s+/g, ' ').trim();
                } else if (allowLeadingSpace) {
                    return stringValue.replace(/\s+/g, ' ').replace(/\s+$/, '');
                } else if (allowTrailingSpace) {
                    return stringValue.replace(/\s+/g, ' ').replace(/^\s+/, '');
                } else {
                    return stringValue.replace(/\s/g, '');
                }
            }

            element.on('input keydown paste', function (event) {
                if (event.type === 'keydown' && event.keyCode === 32) {
                    event.preventDefault();
                    return false;
                }

                if (event.type === 'input' || event.type === 'paste') {
                    setTimeout(function () {
                        var cleanValue = removeSpaces(element.val());
                        if (element.val() !== cleanValue) {
                            ngModel.$setViewValue(cleanValue);
                            ngModel.$render();
                        }
                    }, 0);
                }
            });

            ngModel.$parsers.push(removeSpaces);
            ngModel.$formatters.push(removeSpaces);
        }
    };
});
app.directive('datepicker', function () {
    return {
        restrict: 'E',
        link: function (scope, elem, attr) {
            $(elem).on('keydown', function (e) {
                $('._720kb-datepicker-calendar').removeClass('_720kb-datepicker-open');
                if (isNaN(e.key) && e.keyCode != 8 && e.keyCode != 9 && e.keyCode != 13) {
                    e.preventDefault();
                    return;
                }


            });

            $(elem).on('keyup', function (e) {

                if (e.keyCode == 8) {

                    return;
                }
                var val = e.target.value;
                if (val == undefined) {
                    return;
                }
                if (val.length == 2 || val.length == 5) {
                    val = val + '/';
                }
                if (val.length >= 10) {
                    val = val.substring(0, 10);

                    var d = new Date(val);
                    if (!d) {
                        val = '';
                    }

                }
                e.target.value = val;

            });

            //    $(elem).attr('data-date-format', 'dd/mm/yyyy');

        }
    };
});

// angular.module('components', []).directive('category', function () {
app.directive('partyselection', function () {
    // debugger;

    return {
        restrict: 'E',
        templateUrl: 'directive/partySelection.html?d=' + new Date().getTime()//,
        //controller: function ($scope, $http, $attrs) {
        //    //$http({
        //    //    url: "api/FeaturedProducts/" + $attrs.catName,
        //    //    method: "get"
        //    //}).success(function (data, status, headers, config) {
        //    //    $scope.Cat = data;
        //    //}).error(function (data, status, headers, config) {
        //    //    $scope.data = data;
        //    //    $scope.status = status;
        //    //});

        //}

    };
});
// angular.module('components', []).directive('category', function () {
app.directive('partysites', function () {
    // debugger;

    return {
        restrict: 'E',
        scope: {
            accounts: '=',
            searchText: '@',
            onSelected: '&' // Function binding
        },
        templateUrl: 'directive/partywithSites.html?d=' + new Date().getTime(),

        controller: function ($scope) {
            $scope.onPartySelected = function (e) {
                $scope.onSelected({ e: e });
            }
            $scope.filterByAll = function (account) {

                if (!$scope.searchText) return true;

                var searchLower = $scope.searchText.toLowerCase();

                // Check if search text exists in any of the three fields
                var nameMatch = account.Name && account.Name.toLowerCase().indexOf(searchLower) !== -1;
                var projectMatch = account.Project && account.Project.toLowerCase().indexOf(searchLower) !== -1;
                var addressMatch = account.SiteAddress && account.SiteAddress.toLowerCase().indexOf(searchLower) !== -1;

                return nameMatch || projectMatch || addressMatch;
            };
            //$http({
            //    url: "api/FeaturedProducts/" + $attrs.catName,
            //    method: "get"
            //}).success(function (data, status, headers, config) {
            //    $scope.Cat = data;
            //}).error(function (data, status, headers, config) {
            //    $scope.data = data;
            //    $scope.status = status;
            //});

        },

    };
});
app.directive('multiselectcheckbox', function () {
    // debugger;

    return {
        restrict: 'E',
        replace: true,
        scope: {
            data: '=',
            text: '@',
            text2: '@',
            key: '@',
            selecteddata: '='
        },
        templateUrl: 'directive/multiselect.checked.html?d=' + new Date().getTime(),

        controller: function ($scope, $filter) {

            $scope.onPartySelected = function (e) {
                // $scope.onSelected({ e: e });
            }
            $scope.updateSelection = function () {

                $scope.selecteddata = $scope.data.filter(o => o.Selected == true);
            }
        },

    };
});
app.directive('month', function ($parse) {

    return {
        restrict: 'E',
        scope: {
            month: '=ngModel',
        },
        templateUrl: 'directive/months.html?d=' + new Date().getTime(),
        link: function (scope, elem, attr) {
            elem.bind('change', function (e) {

                var model = $parse($(elem).attr('ng-model'));

                var selectedValue = e.target.value;//parseInt(e.target.value);


                if (model.assign) {
                    model.assign(scope, selectedValue);
                }
            });
            // elem.find('div[class="page-header-text"]').html(attr.heading);

        }
    };
}, true);
app.directive('product', function () {
    // debugger;

    return {
        restrict: 'E',
        replace: true,
        scope: {
            Product: '=ngModel',
            boundAction: '&',
            setSelected: '@',
            id: '=id'
        },
        templateUrl: 'directive/products-search.html?d=' + new Date().getTime(),

        controller: function ($scope, $http, $attrs, $filter) {
            $scope.selectedElement;
            $scope.Search = { Name: '' };

            $scope.dialogId = $scope.id + 'dlgProductsSearch'
            $scope.productSearch = $scope.id + 'productSearch';
            $scope.dlgtxtkeyWordToFind = $scope.id + 'dlgtxtkeyWordToFind';
            $scope.tblProducts = $scope.id + 'tblProducts';

            // $scope.Product = $scope.model;
            $scope.$watch('Product', function () {
                var ele = $(':focus');
                if (!$scope.Product) return;

                if ($scope.Product.length >= 3 && $(ele).attr('id') == $scope.productSearch) {

                    $('#' + $scope.dialogId).modal('show');
                    $('#' + $scope.dialogId).on('shown.bs.modal', function (e) {
                        $('#' + $scope.dlgtxtkeyWordToFind).focus();
                        $scope.Search.Name = $scope.Product;
                    })
                    $scope.FindProducts();
                }
            });

            var Product = new $.Product();
            $scope.FindProducts = function () {
                Product.GetAll(function (e) {
                    $scope.Products = e.data;
                });
            }

            $scope.editRow = function (productId) {

                $('#' + $scope.dialogId).modal('hide');
                //var val = $scope.Products[y - 1];

                var val = $scope.Products.find(o => o.ProductId == productId);
                $scope.$apply(function () {
                    $scope.Product = val.Name;
                    $scope.Search.Name = '';
                });

                $scope.boundAction({ value: val });
            }
            $scope.onKeyDown = function ($event) {
                if ($event.keyCode == 40) {
                    $('#' + $scope.tblProducts).focus();
                }
            }

        }

    };
}, true);

app.directive('quotation', function () {
    // debugger;

    return {
        restrict: 'E',
        replace: true,
        scope: {
            Product: '=ngModel',
            boundAction: '&',
            setSelected: '@',
            id: '=id'
        },
        templateUrl: 'directive/quotation-search.html?d=' + new Date().getTime(),

        controller: function ($scope, $http, $attrs, $filter) {
            $scope.selectedElement;
            $scope.Search = { Name: '' };

            $scope.dialogId = $scope.id + 'dlgProductsSearch'
            $scope.productSearch = $scope.id + 'productSearch';
            $scope.dlgtxtkeyWordToFind = $scope.id + 'dlgtxtkeyWordToFind';
            $scope.tblProducts = $scope.id + 'tblProducts';

            // $scope.Product = $scope.model;
            $scope.$watch('Product', function () {
                var ele = $(':focus');
                if (!$scope.Product) return;

                if ($scope.Product.length >= 3 && $(ele).attr('id') == $scope.productSearch) {

                    $('#' + $scope.dialogId).modal('show');
                    $('#' + $scope.dialogId).on('shown.bs.modal', function (e) {
                        $('#' + $scope.dlgtxtkeyWordToFind).focus();
                        $scope.Search.Name = $scope.Product;
                    })
                    $scope.FindProducts();
                }
            });

            var Product = new $.Product();
            $scope.FindProducts = function () {
                Product.GetAll(function (e) {
                    $scope.Products = e.data;
                });
            }

            $scope.editRow = function (productId) {

                $('#' + $scope.dialogId).modal('hide');
                //var val = $scope.Products[y - 1];

                var val = $scope.Products.find(o => o.ProductId == productId);
                $scope.$apply(function () {
                    $scope.Product = val.Name;
                    $scope.Search.Name = '';
                });

                $scope.boundAction({ value: val });
            }
            $scope.onKeyDown = function ($event) {
                if ($event.keyCode == 40) {
                    $('#' + $scope.tblProducts).focus();
                }
            }

        }

    };
}, true);

app.directive('year', function ($parse) {
    return {
        restrict: 'E',
        require: 'ngModel',
        scope: {
            ngModel: '=',
            heading: '@'
        },
        templateUrl: 'directive/years.html?d=' + new Date().getTime(),
        link: function (scope, elem, attrs) {
            // Populate years
            scope.years = [];
            var currYear = new Date().getFullYear();
            for (var i = currYear; i > currYear - 2; i--) {
                scope.years.push(i);
            }

            // Update ngModel when selection changes
            scope.updateModel = function () {
                scope.ngModel = scope.selectedYear;
            };

            // Watch for external changes to ngModel
            scope.$watch('ngModel', function (newValue) {
                if (newValue !== undefined && newValue !== null) {
                    scope.selectedYear = newValue;
                }
            });

            // Set heading if provided
            scope.headingText = attrs.heading;
        }
    };
});
app.directive('state', function ($compile) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'directive/state.html?d=' + new Date().getTime(),
        link: function (scope, elem, attr) {

            var stateObj = new $.StateCity();
            scope.states = [];
            var model = $(elem).attr('ng-model');


            stateObj.GetStates(function (o) {

                scope.states = o.data;

            });

        }
    };
});
$('body').on('keypress', function (event) {
    var element = event.target;

    if (element && event.keyCode == 13) {
        //var tabIndex = element.tabIndex;

        //$('[tabIndex=' + (tabIndex + 1) + ']').focus();

        //  $(element).closest('input').nextAll().eq(1).focus()
    }
});

app.directive('inputTable', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attr) {
            elem.bind("keydown", function (event, scope) {


            });
        }
    };
});

app.directive('file', function () {
    return {
        restrict: 'EA',
        scope: false,
        link: function (scope, elem, attr) {
            elem.bind('change', function (event) {
                var parentScopeField = this.getAttribute('ng-model');
                var val = this.value;
                scope.$apply(function () {
                    scope[parentScopeField] = val;
                });
            });
        }
    }
});
app.directive('input', function () {
    return {
        restrict: 'E',
        link: function (scope, elem, attr) {
            //   $(elem).attr('autocomplete', 'off');
            //elem.bind('keypress', function (event) {
            //    var keyCode = event.keyCode;
            //    if (keyCode == 13) {
            //        var indx = elem[0].tabIndex;
            //        $('input[tabIndex=' + (indx + 1) + ']').focus();
            //        if (elem[0].nextElementByTabIndex)
            //            elem[0].nextElementByTabIndex.focus();
            //    }
            //});
            elem.bind('focus', function () {
                elem.select();
            });
        }
    }
});
app.directive('searchabletext', function ($compile) {

    var getTemplateUrl = function (type, attrs, descriptionFieldName) {
        var templateUrl = '<div angucomplete-alt id="ex5" placeholder="" initial-value="" field-entry="entry" local-data="" data-entry="0" focus-in="focusIn()" pause="10" selected-object="" remote-url="" remote-url-request-formatter="remoteUrlRequestFn" remote-url-data-field="items" title-field="Name" search-field="" minlength="2" input-class="form-control form-control-small autocomplete" match-class="highlight" field-required="true"></div>';
        var div = $(templateUrl);
        var URL = API_URL;
        var selectedObject = '';
        var onSelection = attrs.selectedObject;
        var name = attrs.name;
        var includeAll = attrs.all;
        var allParam = '';
        if (includeAll != undefined && includeAll != '') {
            allParam = 'all=' + includeAll + '&';
        }

        switch (type) {
            case "Ledger":
                URL = URL + 'Ledger/GetAllLedger?name=';
                selectedObject = 'SelectedLedger';
                break;
            case "Product":
                URL = URL + 'Product/Search?name=';
                break;
            case "Company":
                URL = URL + 'Company/SearchCompany?name=';
                selectedObject = 'selectedCompany'
                break;
            case "Client":
                URL = URL + 'Ledger/SearchClient?' + allParam + 'name=';
                break;
            default:
                break;
        }

        if (onSelection != null && onSelection != undefined) {
            selectedObject = onSelection;
        }
        $(div).attr('id', attrs.ngId);
        $(div).attr('field-required-class', attrs.ngId);
        if (attrs.fieldRequired != '' && attrs.fieldRequired != undefined) {
            $(div).attr('field-required', attrs.fieldRequired);
            $(div).attr('input-name', attrs.name);
        }

        if (attrs.searchField != undefined) {
            $(div).attr('search-field', attrs.searchField);
        }
        if (attrs.titleField != undefined) {
            $(div).attr('title-field', attrs.titleField);
        }
        if (attrs.searchFields != undefined) {
            $(div).attr('search-fields', attrs.searchFields);
        }

        // Set the description-field (now it will be a simple property name, e.g. '$$combinedDesc')
        if (descriptionFieldName) {
            $(div).attr('description-field', descriptionFieldName);
        }

        if (attrs.initialValue != undefined) {
            $(div).attr('initial-value', attrs.initialValue);
        }

        if (attrs.tabIndex != undefined) {
            $(div).attr('field-tabindex', attrs.tabIndex);
        }

        if (attrs.localData != undefined) {
            $(div).attr('local-data', attrs.localData);
        }
        else {
            $(div).attr('remote-url', URL);
        }

        $(div).attr('selected-object', selectedObject);
        return $(div)[0].outerHTML;
    };

    var linker = function (scope, elem, attrs) {
        var descriptionFieldName = attrs.descriptionField;
        var isMulti = attrs.descriptionField && attrs.descriptionField.indexOf(',') !== -1;

        // If multiple fields and using local data, pre-process the data array
        if (isMulti && attrs.localData) {
            var fields = attrs.descriptionField.split(',').map(function (f) { return f.trim(); });
            var combinedKey = '$$combinedDesc_' + fields.join('_');

            // Watch the local-data array and add the combined property
            scope.$watch(attrs.localData, function (dataArray) {
                if (dataArray && angular.isArray(dataArray)) {
                    angular.forEach(dataArray, function (item) {
                        if (!item[combinedKey]) {
                            // Build combined description string
                            var descParts = fields.map(function (field) {
                                return item[field] !== undefined ? item[field] : '';
                            });
                            item[combinedKey] = descParts.join(' - ');
                        }
                    });
                }
            }, true);

            // Tell angucomplete-alt to use this new property as description-field
            descriptionFieldName = combinedKey;
        }
        else if (isMulti && !attrs.localData) {
            console.warn('searchabletext: Multi-field description with remote URL not auto-supported. Please add a combined field in your remote data or use local-data.');
        }

        // Generate template with the correct description field name
        var templateUrl = getTemplateUrl(attrs.item, attrs, descriptionFieldName);
        elem.html(templateUrl);
        $compile(elem.contents())(scope);
    };

    return {
        restrict: 'E',
        template: '<div>{{data}}</div>',
        link: linker
    }
});
app.directive('showtab',
    function () {
        return {
            link: function (scope, element, attrs) {
                element.click(function (e) {
                    e.preventDefault();
                    $(element).tab('show');
                });
            }
        };
    });
//app.directive('preventDefault',
//    function () {
//        return {

//            link: function (scope, element, attrs) {
//                element.click(function (e) {
//                    e.preventDefault();
//                });
//            }
//        };
//    });
/*
//--end of select dropdown directive----------------
$(document).bind('keydown', function (event) {
    
    switch (event.keyCode) {
        case 113://F2 - NEW
            $('.fa-files-o').click();
            break;
        case 119: //F8- SAVE
            $('.fa-save').click();
            break;
        case 112: //F1 - HELP
            break;
        default:
            break;
    }


});
*/
/* Initialization UI Code */
app.filter('sumByKey', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined' || typeof (key) === 'undefined') {
            return 0;
        }

        var sum = 0.0;
        angular.forEach(data, function (obj, objKey) {
            sum += parseFloat(obj[key]);
        });

        return sum;
    };
});

app.filter("group", ["$parse", "$filter", function ($parse, $filter) {
    return function (array, groupByField) {
        var result = [];
        var prev_item = null;
        var groupKey = false;
        var filteredData = $filter('orderBy')(array, groupByField);
        if (!filteredData) {
            return [];
        }
        for (var i = 0; i < filteredData.length; i++) {
            groupKey = false;
            if (prev_item !== null) {
                if (prev_item[groupByField] !== filteredData[i][groupByField]) {
                    groupKey = true;
                }
            } else {
                groupKey = true;
            }
            if (groupKey) {
                filteredData[i]['group_by_key'] = true;
            } else {
                filteredData[i]['group_by_key'] = false;
            }
            result.push(filteredData[i]);
            prev_item = filteredData[i];
        }
        return result;
    }
}])
app.filter("sum", ["$parse", "$filter", function ($parse, $filter) {
    return function (array, sumByField) {

        var result = 0;
        var item = null;
        //  var groupKey = false;
        //    var filteredData = $filter('orderBy')(array, groupByField);
        if (array) {
            for (var i = 0; i < array.length; i++) {
                item = array[i];
                result += parseFloat(item[sumByField], 0);

            }
        }
        return result;
    }
}])

app.directive('defaltcompany', function ($parse, $rootScope) {

    return {
        restrict: 'E',
        templateUrl: 'templ/defaultCompany.html?d=' + new Date().getTime(),
        controller: 'ToolbarController',
        link: function (scope, elem, attr) {
            //  elem.find('div[class="page-header-text"]').html(attr.heading);

        }
    };
});
app.directive('export', function ($parse, $rootScope) {

    return {
        restrict: 'E',
        // replace: true,

        templateUrl: 'directive/export.html?d=' + new Date().getTime(),
        controller: 'ExportController',
        scope: {
            data: '=',
            url: '=',
            data2: '=',
            data3: '='
        },
        link: function (scope, elem, attr) {


        }
    };
});

app.directive('toolbar', function ($parse) {

    return {
        restrict: 'E',
        templateUrl: 'templ/toolbar.html?d=' + new Date().getTime(),
        controller: 'ToolbarController',
        link: function (scope, elem, attr) {

            elem.find('div[class="page-header-text"]').html(attr.heading);
            $(elem[0]).find('li').bind('click', function (e) {
                var cmd = $(this).attr('command');
                if (cmd) {
                    if (scope) {
                        scope.OnToolClick(cmd);
                    }

                }
            });

        }
    };
});
app.directive('a', function ($parse) {

    return {
        restrict: 'E',
        link: function (scope, elem, attr) {
            elem.bind('click', function (e) {

                if (attr.stop == "true") {
                    e.preventDefault();
                }
                else if (attr.toggle == "tab") {
                    e.preventDefault();
                }
            });
        }
    };
});
app.directive('blockheader', function ($parse) {

    return {
        restrict: 'E',
        templateUrl: 'directive/blockheader.html?d=' + new Date().getTime(),
        controller: function () {

        },
        link: function (scope, elem, attr) {
            //  elem.find('div[class="page-header-text"]').html(attr.heading);
            $($(elem).find('span')[1]).html(attr.heading)
            $(elem).bind('click', function () {
                var cls = $($(elem).find('span')[0]).find('i').hasClass('fa-caret-right');
                if (cls) {
                    $($(elem).find('span')[0]).find('i').removeClass('fa-caret-right');
                    $($(elem).find('span')[0]).find('i').addClass('fa-caret-down');
                }
                else {
                    $($(elem).find('span')[0]).find('i').addClass('fa-caret-right');
                    $($(elem).find('span')[0]).find('i').removeClass('fa-caret-down');
                }

            });
        }
    };
});

app.directive('downloadAsExcel', function ($compile, $sce, $templateRequest) {
    return {
        restrict: 'E',
        scope: {
            template: '@',
            object: '='
        },
        replace: true,
        template: '<a class="xls btn btn-success">Download as Excel</a>',
        //template:'<input type="button" class="xls" value="Export" />',
        link: function (scope, element, attrs) {
            var contentType = 'data:application/vnd.ms-excel;base64';
            var htmlS = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" ><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{sheetname}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body>{table}</body></html>';
            var format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };

            var blobbed = function (data) {
                var blob = new Blob([format(htmlS, data)], { type: contentType });
                var blobURL = window.URL.createObjectURL(blob);
                if (blobURL) {
                    element.attr('href', blobURL);
                    element.attr('download', data['name']);
                }
            };

            scope.$watch('object', function (nv, ov) {
                var tUrl = $sce.getTrustedResourceUrl(scope.template);
                $templateRequest(tUrl)
                    .then(function (tmpl) {
                        var t = $('<div/>').append($compile(tmpl)(scope));
                        setTimeout(function () {
                            scope.$apply();
                            blobbed({
                                sheetname: attrs.sheetname,
                                name: attrs.xlname,
                                table: t.html()
                            });
                        }, 100);
                    });
            }, true);
        }
    };
});

app.directive('fileReader', function () {
    return {
        scope: {
            fileReader: "="
        },
        link: function (scope, element) {
            $(element).on('change', function (changeEvent) {
                var files = changeEvent.target.files;
                if (files.length) {
                    var r = new FileReader();
                    r.onload = function (e) {
                        var contents = e.target.result;
                        scope.$apply(function () {
                            scope.fileReader = contents;
                        });
                    };

                    r.readAsText(files[0]);
                    if (scope.$parent.FileSelected != undefined) {
                        scope.$parent.FileSelected(files[0]);
                    }
                }
            });
        }
    };
});
app.directive('loadingcircle', function ($parse, $rootScope) {

    return {
        restrict: 'E',
        templateUrl: 'directive/loading-circle.html?d=' + new Date().getTime(),
        //controller: 'ToolbarController',
        //link: function (scope, elem, attr) {
        //    //  elem.find('div[class="page-header-text"]').html(attr.heading);

        //}
    };
});
app.directive('img', function () {
    return {
        restrict: 'E',
        link: function (scope, element, attrs) {
            element.bind('error', function () {

                element.attr('src', '/img/no-pictures.png'); // set default image
            });
        }
    }
});
app.directive('minValue', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('error', function () {

                element.attr('src', '/img/userPic.jpg'); // set default image
            });
        }
    }
});
app.directive('pdf', function ($compile) {

    return {
        restrict: 'A',

        scope: {
            printElement: "="
        },

        link: function (scope, elem, attr) {

            (elem).bind('click', function () {

                const { jsPDF } = window.jspdf;
                var html = $('#rpt').html();
                var compiled = $('<div/>').append($compile(html)(scope));
                var fileName = 'ewaybill.pdf';
                setTimeout((o) => {
                    scope.$apply();
                    html = compiled.html();
                    let pdf = new jsPDF({
                        orientation: 'p',
                        unit: 'pt',
                        format: 'letter',
                        putOnlyUsedFonts: true,
                        compress: true
                    });
                    pdf.html(html,
                        {
                            width: 580,
                            windowWidth: 580,
                            margin: 15,
                            callback: function (pdf) { pdf.save(fileName) }
                        });
                }, 100);
            });
            //  elem.find('div[class="page-header-text"]').html(attr.heading);

        }
    };
});
app.directive("phoneformat", function () {
    return {
        restrict: "A",
        require: "ngModel",
        link: function (scope, element, attr, ngModelCtrl) {
            var phoneParse = function (value) {
                var numbers = value && value.replace(/-/g, "");
                if (/^\d{10}$/.test(numbers)) {
                    return numbers;
                }

                return undefined;
            }
            var phoneFormat = function (value) {
                var numbers = value && value.replace(/-/g, "");
                var matches = numbers && numbers.match(/^(\d{3})(\d{3})(\d{4})$/);

                if (matches) {
                    // return matches[1] + "-" + matches[2] + "-" + matches[3];
                    return matches[1] + "" + matches[2] + "" + matches[3];
                }

                return undefined;
            }
            ngModelCtrl.$parsers.push(phoneParse);
            ngModelCtrl.$formatters.push(phoneFormat);

            element.bind("blur", function () {
                var value = phoneFormat(element.val());
                var isValid = !!value;
                if (isValid) {
                    ngModelCtrl.$setViewValue(value);
                    ngModelCtrl.$render();
                }

                ngModelCtrl.$setValidity("telephone", isValid);
                scope.$apply();
            });
        }
    };
});
app.directive('ckEditor', function ($timeout) {
    return {
        require: '?ngModel',
        link: function (scope, elm, attr, ngModel) {
            CKEDITOR.disableAutoInline = true;

            // Generate unique ID
            var elementId = elm[0].id || elm[0].name || 'cke_editor_' + Date.now();
            elm[0].id = elementId;

            // Cleanup existing instances
            function cleanupEditor() {
                // By element reference
                for (var key in CKEDITOR.instances) {
                    if (CKEDITOR.instances.hasOwnProperty(key)) {
                        var instance = CKEDITOR.instances[key];
                        try {
                            if (instance.element && instance.element.$ === elm[0]) {
                                instance.destroy(true);
                                delete CKEDITOR.instances[key];
                            }
                        } catch (e) { }
                    }
                }

                // By ID
                if (CKEDITOR.instances[elementId]) {
                    CKEDITOR.instances[elementId].destroy(true);
                    delete CKEDITOR.instances[elementId];
                }

                // Remove wrappers
                var wrapper = elm[0].nextSibling;
                while (wrapper && wrapper.className && wrapper.className.indexOf('cke_') !== -1) {
                    var toRemove = wrapper;
                    wrapper = wrapper.nextSibling;
                    if (toRemove.parentNode) {
                        toRemove.parentNode.removeChild(toRemove);
                    }
                }
            }

            cleanupEditor();

            var ck = null;

            $timeout(function () {
                try {
                    ck = CKEDITOR.replace(elm[0]);

                    if (!ngModel) return;

                    ck.on('instanceReady', function () {
                        ck.setData(ngModel.$viewValue || '');
                    });

                    ck.on('change', function () {
                        scope.$apply(function () {
                            ngModel.$setViewValue(ck.getData());
                        });
                    });

                    ck.on('key', function () {
                        scope.$apply(function () {
                            ngModel.$setViewValue(ck.getData());
                        });
                    });

                    ngModel.$render = function () {
                        if (ck && ck.setData) {
                            var data = ngModel.$viewValue || '';
                            if (ck.getData() !== data) {
                                ck.setData(data);
                            }
                        }
                    };

                    scope.$on('$destroy', function () {
                        if (ck) {
                            ck.destroy(true);
                            ck = null;
                        }
                        if (CKEDITOR.instances[elementId]) {
                            CKEDITOR.instances[elementId].destroy(true);
                            delete CKEDITOR.instances[elementId];
                        }
                    });

                } catch (e) {
                    console.error('Error creating CKEditor:', e);
                }
            }, 100);
        }
    };
});
//app.directive('ckEditor', function () {
//    return {
//        require: '?ngModel',
//        link: function (scope, elm, attr, ngModel) {
//            debugger
//            CKEDITOR.disableAutoInline = true;
//            //var elementId = elm[0].id || elm[0].name;
//            //if (!elementId || elementId == "") {
//            //    elm[0].id = elementId= 'cke_auto_id_' + Date.now();
//            //}
//            //if (elementId && CKEDITOR.instances[elementId]) {
//            //    // 3. Destroy the old instance safely
//            //    CKEDITOR.instances[elementId].destroy(true);
//            //}
//            var ck = CKEDITOR.replace(elm[0]);
//            if (!ngModel) return;
//            ck.on('instanceReady', function () {
//                ck.setData(ngModel.$viewValue);
//            });
//            function updateModel() {
//                scope.$apply(function () {
//                    ngModel.$setViewValue(ck.getData());
//                });
//            }
//            ck.on('change', updateModel);
//            ck.on('key', updateModel);
//            ck.on('dataReady', updateModel);

//            ngModel.$render = function (value) {
//                ck.setData(ngModel.$viewValue);
//            };
//        }
//    };
//});
app.directive('contenteditable', function () {
    return {
        require: '?ngModel', // Require ngModel if present
        link: function (scope, element, attrs, ngModel) {
            if (!ngModel) return; // Do nothing if ngModel is not present

            // View -> Model
            element.on('blur keyup change', function () {
                scope.$apply(function () {
                    ngModel.$setViewValue(element.html());
                });
            });

            // Model -> View
            ngModel.$render = function () {
                element.html(ngModel.$viewValue || '');
            };
        }
    };
});
app.directive('validateprefix', function () {
    return {
        require: '?ngModel', // Require ngModel if present
        restrict: "A",

        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) return; // Do nothing if ngModel is not present

            // View -> Model
            element.on('blur keyup change', function ($event) {


                const regex = /^[a-zA-Z0-9/-]+$/;
                var input = $(element).val();
                var _isValid = regex.test(input);
                if (!_isValid) {
                    input = input.replace(/[^a-zA-Z0-9\/-]/g, '');
                }

                scope.$apply(function () {
                    // $(element).val(_validVal)
                    ngModelCtrl.$setViewValue(input);
                    ngModelCtrl.$render();
                });
            });

            //// Model -> View
            //ngModel.$render = function () {
            //    element.html(ngModel.$viewValue || '');
            //};
        }
    };
});
//app.directive('validateprefix', function () {
//    return function (input) {


//    };
//});
app.component('buttonRenderer', {
    template: `
    <button class="btn btn-primary btn-sm" ng-click="$ctrl.onClick()">
      {{$ctrl.params.label || 'Action'}}
    </button>
  `,
    bindings: {
        params: '<'
    },
    controller: function () {
        var ctrl = this;

        ctrl.onClick = function () {
            if (ctrl.params.onClick) {
                ctrl.params.onClick(ctrl.params);
            }
        };

        ctrl.$onInit = function () {
            console.log('Button renderer initialized');
        };
    }
});


app.directive('adSort', function () {
    return {
        restrict: 'A',

        scope: {
            data: "="
        },
        link: function (scope, element) {
            alert(scope.data);
        }
    }

});

// app/directives/sort-header.js
app.directive('sortHeader', function () {
    return {
        restrict: 'A',
        scope: {
            sortHeader: '=',
            sortField: '@',
            sortDefault: '@?',
            onSortChange: '&?'
        },
        link: function (scope, element, attrs) {
            var currentOrder = 'none'; // none, asc, desc

            // Add sort header styles
            element.addClass('sort-header');
            element.css({
                'cursor': 'pointer',
                'user-select': 'none',
                'position': 'relative',
                'padding-right': '20px'
            });

            // Create sort indicator
            var indicator = angular.element('<span class="ml-1 sort-indicator"></span>');
            element.append(indicator);

            // Click handler
            element.on('click', function () {
                if (!scope.sortHeader) return;

                scope.$apply(function () {
                    // Cycle through sort states: none -> asc -> desc -> none
                    if (currentOrder === 'none') {
                        currentOrder = 'asc';
                    } else if (currentOrder === 'asc') {
                        currentOrder = 'desc';
                    } else {
                        currentOrder = 'asc';
                    }

                    applySort();
                    updateVisualState();

                    // Notify parent
                    if (scope.onSortChange) {
                        scope.onSortChange({
                            field: scope.sortField,
                            order: currentOrder,
                            array: scope.sortHeader
                        });
                    }
                });
            });

            function applySort() {
                if (currentOrder === 'none' || !scope.sortField) {
                    // Restore original order or don't sort
                    return;
                }

                scope.sortHeader.sort(function (a, b) {
                    var aValue = getPropertyValue(a, scope.sortField);
                    var bValue = getPropertyValue(b, scope.sortField);

                    // Type handling
                    if (typeof aValue === 'string' && typeof bValue === 'string') {
                        aValue = aValue.toLowerCase();
                        bValue = bValue.toLowerCase();
                    }

                    // Handle different types
                    var result = compareValues(aValue, bValue);

                    return currentOrder === 'desc' ? -result : result;
                });
            }

            function getPropertyValue(obj, path) {
                if (!path) return obj;
                return path.split('.').reduce(function (current, key) {
                    return current && current[key] !== undefined ? current[key] : null;
                }, obj);
            }

            function compareValues(a, b) {
                // Handle null/undefined
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                // Handle numbers
                if (typeof a === 'number' && typeof b === 'number') {
                    return a - b;
                }

                // Handle dates
                if (a instanceof Date && b instanceof Date) {
                    return a - b;
                }

                // Handle strings
                if (typeof a === 'string' && typeof b === 'string') {
                    return a.localeCompare(b);
                }

                // Fallback
                return a < b ? -1 : a > b ? 1 : 0;
            }

            function updateVisualState() {
                // Remove all state classes
                element.removeClass('sort-none sort-asc sort-desc active');

                // Add current state class
                //  element.addClass('sort-' + currentOrder);

                if (currentOrder !== 'none') {
                    //   element.addClass('active');
                }

                // Update indicator
                var indicatorText = '';
                if (currentOrder === 'asc') indicatorText = '↑';
                if (currentOrder === 'desc') indicatorText = '↓';
                //if (currentOrder === 'none') indicatorText = '↕';

                indicator.text(indicatorText);
            }

            // Set default sort if specified
            if (attrs.sortDefault !== undefined) {
                currentOrder = 'asc';
                applySort();
                updateVisualState();
            }

            // Watch for external sort changes
            attrs.$observe('sortField', function (newField) {
                if (newField && scope.sortHeader) {
                    updateVisualState();
                }
            });

            // Initialize
            updateVisualState();

            // Cleanup
            scope.$on('$destroy', function () {
                element.off('click');
            });
        }
    };
});

//-------------------
app.directive('multiSelectDropdown', function ($document, $timeout) {
    return {
        restrict: 'E',
        scope: {
            options: '=',           // Array of options to display
            selectedItems: '=',     // Array to store selected items
            displayProperty: '@',   // Property to display for each option
            valueProperty: '@',     // Property to use as unique identifier
            placeholder: '@',       // Placeholder text
            selectAllText: '@'      // Text for select all checkbox
        },
        template: `
            <div class="multi-select-dropdown">
                <!-- Dropdown Toggle Button -->
                <div class="multi-select-dropdown-toggle"
                     ng-click="toggleDropdown()"
                     tabindex="0"
                     ng-keydown="onToggleKeyDown($event)">
                    <span class="selected-items-preview" ng-if="selectedItems.length > 0">
                        {{ getSelectedPreview() }}
                    </span>
                    <span class="placeholder" ng-if="selectedItems.length === 0">
                        {{ placeholder || 'Select options...' }}
                    </span>
                    <span class="dropdown-arrow" ng-class="{ 'open': isOpen }"></span>
                </div>
                
                <!-- Dropdown Panel -->
                <div class="dropdown-panel" ng-class="{ 'open': isOpen }">
                    <!-- Search Box -->
                    <div class="search-section" ng-if="options && options.length > 3">
                        <input type="text" 
                               class="search-input"
                               ng-model="searchText" 
                               placeholder="Type to search..."
                               ng-keydown="onSearchKeyDown($event)"
                               ng-focus="onSearchFocus()">
                    </div>
                    
                    <!-- Select All Section -->
                    <div class="select-all-section" ng-if="filteredOptions.length > 0">
                        <label class="select-all-checkbox">
                            <input type="checkbox"  
                                   ng-model="selection.allSelected"
                                   ng-change="toggleSelectAll()"
                                   ng-indeterminate="isIndeterminate">
                            {{ selectAllText || 'Select All' }} ({{ filteredOptions.length }} visible)
                        </label>
                    </div>
                    
                    <!-- Options List -->
                    <div class="options-list">
                        <div class="option-item" 
                             ng-repeat="option in filteredOptions = (getFilteredOptions()) track by $index"
                             ng-click="handleOptionClick($event, option)">
                            <label>
                                <input type="checkbox" 
                                       ng-model="option.selected" 
                                       ng-change="updateSelection(option)">
                                {{ getDisplayValue(option) }}
                            </label>
                        </div>
                    </div>
                    
                    <!-- No Results Message -->
                    <div class="no-options" ng-if="filteredOptions.length === 0 && options && options.length > 0">
                        No options found for "{{ searchText }}"
                    </div>
                    
                    <!-- No Options Message -->
                    <div class="no-options" ng-if="!options || options.length === 0">
                        No options available
                    </div>
                    
                    <!-- Selected Count -->
                    <div class="selected-count" ng-if="options && options.length > 0">
                        {{ getSelectedCount() }} of {{ options.length }} selected
                    </div>
                </div>
            </div>
        `,
        link: function (scope, element, attrs) {
            scope.isOpen = false;
            scope.searchText = '';
            scope.selection = { allSelected: false };
            scope.isIndeterminate = false;

            // Get filtered options
            scope.getFilteredOptions = function () {
                if (!scope.options) return [];
                if (!scope.searchText) return scope.options;

                return scope.options.filter(function (option) {
                    var displayValue = scope.getDisplayValue(option).toString().toLowerCase();
                    return displayValue.indexOf(scope.searchText.toLowerCase()) !== -1;
                });
            };

            // Get display value for an option
            scope.getDisplayValue = function (option) {
                return scope.displayProperty ? option[scope.displayProperty] : option.toString();
            };

            // Update selection when checkbox changes
            scope.updateSelection = function (option) {
                //  console.log('Update selection:', option[scope.displayProperty], option.selected);

                var index = scope.findSelectedIndex(option);

                if (option.selected && index === -1) {
                    // Add to selected items
                    scope.selectedItems.push(angular.copy(option));
                } else if (!option.selected && index !== -1) {
                    // Remove from selected items
                    scope.selectedItems.splice(index, 1);
                }

                scope.updateSelectAllState();
                //  scope.$apply(); // Force digest cycle
            };

            // Find index of option in selected items
            scope.findSelectedIndex = function (option) {
                if (!scope.selectedItems) return -1;

                return scope.selectedItems.findIndex(function (item) {
                    return item[scope.valueProperty] === option[scope.valueProperty];
                });
            };

            // Toggle select all for VISIBLE options only
            scope.toggleSelectAll = function () {
                // console.log('Toggle select all:', scope.allSelected);
                var filteredOptions = scope.getFilteredOptions();

                if (scope.selection.allSelected) {
                    // Select all visible options
                    filteredOptions.forEach(function (option) {
                        if (!option.selected) {
                            option.selected = true;
                            var index = scope.findSelectedIndex(option);
                            if (index === -1) {
                                scope.selectedItems.push(angular.copy(option));
                            }
                        }
                    });
                } else {
                    // Deselect all visible options
                    filteredOptions.forEach(function (option) {
                        if (option.selected) {
                            option.selected = false;
                            var index = scope.findSelectedIndex(option);
                            if (index !== -1) {
                                scope.selectedItems.splice(index, 1);
                            }
                        }
                    });
                }

                // Update the state after toggling
                scope.updateSelectAllState();
            };

            // Update select all checkbox state based on VISIBLE options
            scope.updateSelectAllState = function () {
                var filteredOptions = scope.getFilteredOptions();
                //console.log('Updating select all state. Filtered options:', filteredOptions.length);

                if (filteredOptions.length === 0) {
                    scope.allSelected = false;
                    scope.isIndeterminate = false;
                    //console.log('No visible options');
                    return;
                }

                var selectedVisibleCount = 0;
                filteredOptions.forEach(function (option) {
                    if (option.selected) {
                        selectedVisibleCount++;
                    }
                });

                //  console.log('Selected visible count:', selectedVisibleCount, 'Total visible:', filteredOptions.length);

                // Update allSelected - true only if ALL visible options are selected
                scope.allSelected = selectedVisibleCount === filteredOptions.length;

                // Update indeterminate - true if SOME but not ALL visible options are selected
                scope.isIndeterminate = selectedVisibleCount > 0 && selectedVisibleCount < filteredOptions.length;

                // console.log('allSelected:', scope.allSelected, 'isIndeterminate:', scope.isIndeterminate);
            };

            // Get preview of selected items
            scope.getSelectedPreview = function () {
                if (!scope.selectedItems || scope.selectedItems.length === 0) {
                    return '';
                }

                var maxDisplay = 2;
                var displayItems = scope.selectedItems.slice(0, maxDisplay).map(function (item) {
                    return scope.getDisplayValue(item);
                });

                var preview = displayItems.join(', ');
                if (scope.selectedItems.length > maxDisplay) {
                    preview += ' +' + (scope.selectedItems.length - maxDisplay) + ' more';
                }

                return preview;
            };

            // Get count of selected items
            scope.getSelectedCount = function () {
                return scope.selectedItems ? scope.selectedItems.length : 0;
            };

            // Initialize options selection state
            scope.init = function () {
                if (scope.options && scope.selectedItems) {
                    //console.log('Initializing with', scope.options.length, 'options and', scope.selectedItems.length, 'selected items');

                    // Mark options as selected based on selectedItems
                    scope.options.forEach(function (option) {
                        var isSelected = scope.selectedItems.some(function (selected) {
                            return selected[scope.valueProperty] === option[scope.valueProperty];
                        });
                        option.selected = isSelected;
                        //  console.log('Option:', option[scope.displayProperty], 'selected:', option.selected);
                    });

                    scope.updateSelectAllState();
                }
            };

            // Toggle dropdown visibility
            scope.toggleDropdown = function () {
                scope.isOpen = !scope.isOpen;
                if (scope.isOpen) {
                    $timeout(function () {
                        element.find('input.search-input').focus();
                        scope.updateSelectAllState(); // Refresh state when opening
                    });
                } else {
                    scope.searchText = ''; // Clear search when closing
                }
            };

            // Close dropdown when clicking outside
            var onClickOutside = function (event) {
                if (!element[0].contains(event.target)) {
                    //  scope.$apply(function () {
                    scope.isOpen = false;
                    scope.searchText = '';
                    // });
                }
            };

            // Keyboard navigation for toggle button
            scope.onToggleKeyDown = function (event) {
                switch (event.key) {
                    case ' ':
                    case 'Enter':
                    case 'Spacebar':
                        event.preventDefault();
                        scope.toggleDropdown();
                        break;
                    case 'Escape':
                        scope.isOpen = false;
                        break;
                    case 'ArrowDown':
                        event.preventDefault();
                        if (!scope.isOpen) {
                            scope.toggleDropdown();
                        }
                        break;
                }
            };

            // Keyboard navigation for search input
            scope.onSearchKeyDown = function (event) {
                switch (event.key) {
                    case 'Escape':
                        event.preventDefault();
                        scope.isOpen = false;
                        scope.searchText = '';
                        break;
                    case 'ArrowDown':
                        event.preventDefault();
                        break;
                }
            };

            // Focus search input when dropdown opens
            scope.onSearchFocus = function () {
                scope.isOpen = true;
            };

            // Handle option click (prevent event bubbling)
            scope.handleOptionClick = function (event, option) {
                event.stopPropagation();
            };

            // Watch for changes and update select all state
            scope.$watch('options', function (newVal) {
                if (newVal) {
                    scope.init();
                }
            }, true);

            scope.$watch('selectedItems', function (newVal) {
                if (newVal) {
                    // console.log('Selected items changed:', newVal.length);
                    scope.updateSelectAllState();
                }
            }, true);

            scope.$watch('searchText', function (newVal) {
                //  console.log('Search text changed:', newVal);
                scope.updateSelectAllState();
            });

            // Manual watch for option selection changes
            scope.$watch('options', function (newVal, oldVal) {
                if (newVal && oldVal) {
                    // Check if any selection states changed
                    var selectionChanged = false;
                    for (var i = 0; i < newVal.length; i++) {
                        if (newVal[i].selected !== (oldVal[i] && oldVal[i].selected)) {
                            selectionChanged = true;
                            break;
                        }
                    }
                    if (selectionChanged) {
                        // console.log('Option selection states changed');
                        scope.updateSelectAllState();
                    }
                }
            }, true);

            // Initialize
            scope.init();

            // Add event listeners
            $document.on('click', onClickOutside);
            $document.on('keydown', function (event) {
                if (event.key === 'Escape' && scope.isOpen) {
                    scope.$apply(function () {
                        scope.isOpen = false;
                        scope.searchText = '';
                    });
                }
            });

            // Cleanup
            scope.$on('$destroy', function () {
                $document.off('click', onClickOutside);
            });

            // Force initial update
            $timeout(function () {
                scope.updateSelectAllState();
            });
        }
    };
});
app.directive('ngDrop', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element[0].addEventListener('dragover', function (e) {
                e.preventDefault();
                e.stopPropagation();
                scope.$apply(function () {
                    scope.vm.isDragging = true;
                });
            });

            element[0].addEventListener('dragleave', function (e) {
                e.preventDefault();
                e.stopPropagation();
                scope.$apply(function () {
                    scope.vm.isDragging = false;
                });
            });

            element[0].addEventListener('drop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                scope.$apply(function () {
                    scope.vm.isDragging = false;
                    var files = e.dataTransfer.files;
                    scope.vm.onDrop(files, e);
                });
            });
        }
    };
});
app.directive('ngDropSuccess', function () {
    return {
        restrict: 'A',
        scope: {
            ngDropSuccess: '&'
        },
        link: function (scope, element, attrs) {
            // This is handled in the ngDrop directive
        }
    };
})
app.directive('fileupload', function () {
    return {
        restrict: 'E',
        scope: {
            selectedFiles: '=',
            onFileSelected: '&',
            onFileDelete: '&',
            allowedTypes: '='
        },
        templateUrl: 'directive/file-upload.html?d=' + new Date().getTime(),
        //controller: 'ToolbarController',
        //link: function (scope, elem, attr) {
        //    //  elem.find('div[class="page-header-text"]').html(attr.heading);

        //}
    };
});
app.run(['tabSyncService', '$rootScope', '$log',
    function (tabSyncService, $rootScope, $log) {

        // Global tab sync configuration
        tabSyncService.init({
            autoRefresh: true, // Let controllers handle refresh
            showConfirmation: false,
            refreshMessage: 'Another instance of this application was opened. Refresh this page?',
            excludePaths: ['/login', '/signup'],
            debug: false
        });

        // Global event handlers
        $rootScope.$on('tabSync:refreshRequired', function () {
            $log.log('Global: Refresh required due to new tab');
        });

        $rootScope.$on('tabSync:newTabOpened', function (event, data) {
            $log.log('Global: New tab opened:', data.newTabId);
        });

        //// Clean up on route changes if needed
        //$rootScope.$on('$routeChangeStart', function () {
        //    // Optional: Add route-specific logic
        //});
    }]);
// Custom filter to count array items
app.filter('count', function () {
    return function (input, property, value) {
        if (!angular.isArray(input)) return 0;

        // Count all items if no criteria specified
        if (!property) return input.length;

        // Count items matching specific criteria
        return input.filter(function (item) {
            if (value !== undefined && item[property]) {
                return item[property].toLowerCase() === value.toLowerCase();
            }
            return item[property];
        }).length;
    };
});

app.directive('loader', function () {
    return {
        restrict: 'E',
        template: `
                    <div class="loader-overlay" ng-show="loaderService.isLoading">
                        <div class="loader-content">
                            <div class="spinner"></div>
                            <div>Loading... Please wait</div>
                            <div style="margin-top: 10px;display:none; font-size: 14px;">
                                Active requests: {{loaderService.requests}}
                            </div>
                        </div>
                    </div>
                `,
        controller: function ($scope, loaderService) {
            $scope.loaderService = loaderService;
        }
    };
});
app.directive('warehouse', function () {
    return {
        restrict: 'E',
        scope: {
            ngModel: '=',
            warehouses: '=',
            name: '@?',
            field: '@?',
            dataVal: '@?',
            dataText: '@?',
            placeholder: '@?',
            disabled: '@?',
            required: '@?'
        },
        template: `
            <searchableselect 
                style="max-width: 220px; display: block;" 
                name="{{name || 'warehouse'}}"
                id="warehouse_{{::$id}}"
                data-field="warehouses"
                ng-model="ngModel"
                data-val="{{dataVal || 'Id'}}"
                data-text="{{dataText || 'Name'}}"
                placeholder="{{placeholder || 'Select warehouse'}}"
                ng-disabled="{{disabled}}"
                ng-required="{{required}}"
                compare-with="compareFn">
            </searchableselect>
        `,
        controller: ['$scope', function ($scope) {
            $scope.dataVal = $scope.dataVal || 'Id';
            $scope.dataText = $scope.dataText || 'Name';
            $scope.placeholder = $scope.placeholder || 'Select warehouse';

            // Custom comparator function
            $scope.compareFn = function (item1, item2) {
                if (!item1 || !item2) return false;

                // Get the ID values
                var val1 = item1[$scope.dataVal];
                var val2 = item2[$scope.dataVal];

                // Compare as strings for type safety
                return String(val1) === String(val2);
            };

            // Watch warehouses and ensure ngModel matches an item
            $scope.$watchCollection('warehouses', function (newWarehouses) {
                if (!newWarehouses || !$scope.ngModel) return;

                // If ngModel doesn't match any warehouse, clear it
                var found = newWarehouses.find(function (w) {
                    return String(w[$scope.dataVal]) === String($scope.ngModel);
                });

                if (!found && newWarehouses.length > 0) {
                    $scope.ngModel = null;
                }
            });
        }],
        link: function (scope, element, attrs) {
            // Validation
            scope.$watch('warehouses', function (newVal) {
                if (newVal && !Array.isArray(newVal)) {
                    console.warn('Warehouses should be an array');
                }
            });
        }
    };
});
app.filter('abs', function () {
    return function (value) {
        return Math.abs(value);
    };
});