app.directive('posAutocomplete', function ($timeout, $window) {
    return {
        restrict: 'E',
        scope: {
            items: '=',           // Array of items to search
            onSelect: '&',        // Callback when item is selected
            recentSearches: '=?', // Recent searches array
            displayProperty: '@', // Property to display in results
            placeholder: '@',     // Input placeholder
            showScanButton: '@',  // Show barcode scan button
            onScan: '&',          // Callback for scan button
            inputSize: '@',       // Input size (sm, lg)
            disabled: '=?',       // Disable the input
            autoFocus: '@'        // Auto-focus after selection (default: true)
        },
        template: `
                    <div class="pos-autocomplete" ng-class="{'disabled': disabled}">
                        <div class="input-group" ng-class="getInputGroupClass()">
                            <input type="text" 
                                   class="form-control search-input"
                                   ng-class="getInputClass()"
                                   ng-model="searchTerm"
                                   ng-keydown="handleKeydown($event)"
                                   ng-keyup="handleKeyup($event)"
                                   ng-focus="handleFocus()"
                                   placeholder="{{placeholder || 'Search items...'}}"
                                   autocomplete="off"
                                   ng-disabled="disabled"
                                   id="{{inputId}}">
                            <span class="input-group-btn" ng-if="showScanButton">
                                <button class="btn btn-info" 
                                        type="button" 
                                        ng-click="handleScan()"
                                        ng-class="getButtonClass()"
                                        ng-disabled="disabled">
                                    <i class="fa fa-barcode"></i> Scan
                                </button>
                            </span>
                        </div>
                        
                        <div class="autocomplete-results" ng-class="{active: showResults && searchTerm.length > 0}" id="{{resultsId}}">
                            <!-- Recent Searches -->
                            <div class="recent-searches" ng-if="searchTerm.length === 0 && recentSearches && recentSearches.length > 0">
                                <div class="search-header">
                                    <i class="fa fa-clock-o"></i> Recent Searches
                                    <span class="pull-right" style="font-size: 11px; color: #999;">
                                        ↑↓ Navigate • Enter Select • Esc Close
                                    </span>
                                </div>
                                <div class="autocomplete-item" 
                                     ng-repeat="item in recentSearches track by $index"
                                     ng-click="selectItem(item)"
                                     ng-class="{selected: $index === selectedIndex}"
                                     id="{{resultsId + '-item-' + $index}}">
                                    <div class="item-info">
                                        <div class="item-name">
                                            {{getDisplayValue(item)}}
                                            <span class="category-badge" ng-if="item.category">{{item.category}}</span>
                                        </div>
                                        <div class="item-details">
                                            Code: {{item.code}} <span ng-if="item.stock != null">| Stock: {{item.stock}}</span>
                                        </div>
                                    </div>
                                    <div class="item-price" ng-if="item.price">{{ item.price }}</div>
                                </div>
                            </div>
                            
                            <!-- Search Results -->
                            <div ng-if="searchTerm.length > 0">
                                <div class="search-header">
                                    <span ng-if="searchResults.length > 0">
                                        Found {{searchResults.length}} items
                                    </span>
                                    <span ng-if="searchResults.length === 0">
                                        No items found
                                    </span>
                                    <span class="pull-right" style="font-size: 11px; color: #999;">
                                        ↑↓ Navigate • Enter Select • Esc Close
                                    </span>
                                </div>
                                
                                <div ng-if="searchResults.length > 0">
                                    <div class="autocomplete-item" 
                                         ng-repeat="item in searchResults track by $index"
                                         ng-click="selectItem(item)"
                                         ng-class="{selected: $index === selectedIndex}"
                                         id="{{resultsId + '-item-' + $index}}">
                                        <div class="item-info">
                                            <div class="item-name">
                                                {{getDisplayValue(item)}}
                                                <span class="category-badge" ng-if="item.category">{{item.category}}</span>
                                                <span class="barcode-indicator" ng-if="item.code === searchTerm">
                                                    <i class="fa fa-barcode"></i> Barcode
                                                </span>
                                            </div>
                                            <div class="item-details">
                                                Code: {{item.code}} 
                                                <span ng-if="item.brand">| Brand: {{item.brand}}</span>
                                            </div>
                                        </div>
                                        <div style="display: flex; align-items: center;">
                                            <div class="item-price" ng-if="item.price">{{ item.price }}</div>
                                            <span class="item-stock" 
                                                  ng-class="getStockClass(item)"
                                                  ng-if="item.stock != null">
                                                {{item.stock > 0 ? (item.stock + ' in stock') : 'Out of stock'}}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="no-results" ng-if="searchResults.length === 0 && searchTerm.length > 0">
                                    <i class="fa fa-search"></i> No items found for "{{searchTerm}}"
                                    <br>
                                    <small>Press Enter to add as custom item</small>
                                </div>
                            </div>
                        </div>
                    </div>
                `,
        link: function (scope, element, attrs) {
            // Initialize variables
            scope.searchTerm = '';
            scope.searchResults = [];
            scope.showResults = false;
            scope.selectedIndex = -1;
            scope.inputId = 'pos-search-' + Math.random().toString(36).substr(2, 9);
            scope.resultsId = 'pos-results-' + Math.random().toString(36).substr(2, 9);
            scope.autoFocus = angular.isDefined(attrs.autoFocus) ? scope.$eval(attrs.autoFocus) : true;

            // Get the search input element
            var getSearchInput = function () {
                return element[0].querySelector('.search-input');
            };

            // Get the results container element
            var getResultsContainer = function () {
                return element[0].querySelector('.autocomplete-results');
            };

            // Get a specific result item element
            var getResultItem = function (index) {
                return document.getElementById(scope.resultsId + '-item-' + index);
            };

            // Ensure selected item is visible in scroll view
            scope.scrollToSelectedItem = function () {
                if (scope.selectedIndex < 0) return;

                $timeout(function () {
                    var resultsContainer = getResultsContainer();
                    var selectedItem = getResultItem(scope.selectedIndex);

                    if (resultsContainer && selectedItem) {
                        var container = resultsContainer;
                        var item = selectedItem;

                        var containerRect = container.getBoundingClientRect();
                        var itemRect = item.getBoundingClientRect();

                        // Check if item is outside visible area
                        if (itemRect.top < containerRect.top) {
                            // Item is above visible area - scroll up
                            container.scrollTop -= (containerRect.top - itemRect.top);
                        } else if (itemRect.bottom > containerRect.bottom) {
                            // Item is below visible area - scroll down
                            container.scrollTop += (itemRect.bottom - containerRect.bottom);
                        }
                    }
                }, 0);
            };

            // Focus on search input
            scope.focusSearch = function () {
                $timeout(function () {
                    var input = getSearchInput();
                    if (input) {
                        input.focus();
                        // Select all text for quick replacement
                        input.select();
                    }
                }, 0);
            };

            // Handle focus event
            scope.handleFocus = function () {
                scope.showResults = true;
            };

            // Get input group class
            scope.getInputGroupClass = function () {
                if (scope.inputSize === 'sm') return 'input-group-sm';
                if (scope.inputSize === 'lg') return 'input-group-lg';
                return '';
            };

            // Get input class
            scope.getInputClass = function () {
                if (scope.inputSize === 'sm') return 'input-sm';
                if (scope.inputSize === 'lg') return 'input-lg';
                return '';
            };

            // Get button class
            scope.getButtonClass = function () {
                if (scope.inputSize === 'sm') return 'btn-sm';
                if (scope.inputSize === 'lg') return 'btn-lg';
                return '';
            };

            // Get display value for item
            scope.getDisplayValue = function (item) {
                if (scope.displayProperty && item[scope.displayProperty]) {
                    return item[scope.displayProperty];
                }
                return item.name || item.text || item.label;
            };

            // Get stock status class
            scope.getStockClass = function (item) {
                if (item.stock == null) return '';
                if (item.stock > 10) return 'stock-available';
                if (item.stock > 0) return 'stock-low';
                return 'stock-out';
            };

            // Search function with debouncing
            var searchTimeout;
            scope.handleKeyup = function (event) {
                if (scope.disabled) return;

                // Ignore arrow keys, enter, esc
                if ([38, 40, 13, 27].includes(event.keyCode)) return;
                if (!scope.showResults) {
                    scope.showResults = true;
                }
                if (searchTimeout) $timeout.cancel(searchTimeout);

                searchTimeout = $timeout(function () {
                    if (scope.searchTerm.length === 0) {
                        scope.searchResults = [];
                        scope.selectedIndex = -1;
                        return;
                    }

                    // Perform search
                    var term = scope.searchTerm.toLowerCase();
                    scope.searchResults = scope.items.filter(function (item) {
                        var searchableText = scope.getDisplayValue(item).toLowerCase();
                        return searchableText.includes(term) ||
                            (item.code && item.code.toLowerCase().includes(term)) ||
                            (item.category && item.category.toLowerCase().includes(term)) ||
                            (item.brand && item.brand.toLowerCase().includes(term)) ||
                            (item.sku && item.sku.toLowerCase().includes(term));
                    });

                    // Sort by relevance
                    scope.searchResults.sort(function (a, b) {
                        var aNameMatch = scope.getDisplayValue(a).toLowerCase() === term;
                        var bNameMatch = scope.getDisplayValue(b).toLowerCase() === term;
                        var aCodeMatch = a.code && a.code.toLowerCase() === term;
                        var bCodeMatch = b.code && b.code.toLowerCase() === term;

                        if (aCodeMatch && !bCodeMatch) return -1;
                        if (!aCodeMatch && bCodeMatch) return 1;
                        if (aNameMatch && !bNameMatch) return -1;
                        if (!aNameMatch && bNameMatch) return 1;
                        return 0;
                    });

                    scope.selectedIndex = scope.searchResults.length > 0 ? 0 : -1;

                    // Scroll to selected item when search results change
                    if (scope.searchResults.length > 0) {
                        scope.scrollToSelectedItem();
                    }
                }, 300);
            };

            // Keyboard navigation
            scope.handleKeydown = function (event) {
                if (scope.disabled) return;

                switch (event.keyCode) {
                    case 38: // Up arrow
                        event.preventDefault();
                        if (scope.selectedIndex > 0) {
                            scope.selectedIndex--;
                            scope.scrollToSelectedItem();
                        }
                        break;
                    case 40: // Down arrow
                        event.preventDefault();
                        var maxIndex = scope.searchTerm.length === 0 && scope.recentSearches ?
                            scope.recentSearches.length - 1 : scope.searchResults.length - 1;

                        if (scope.selectedIndex < maxIndex) {
                            scope.selectedIndex++;
                            scope.scrollToSelectedItem();
                        }
                        break;
                    case 13: // Enter
                        event.preventDefault();
                        if (scope.selectedIndex >= 0 && scope.searchResults.length > 0) {
                            scope.selectItem(scope.searchResults[scope.selectedIndex]);
                        } else if (scope.searchTerm.length > 0) {
                            // Add custom item if no selection but has search term
                            scope.addCustomItem();
                        }
                        break;
                    case 27: // Escape
                        scope.showResults = false;
                        scope.selectedIndex = -1;
                        break;
                    case 9: // Tab
                        if (scope.showResults) {
                            event.preventDefault();
                            if (scope.selectedIndex >= 0 && scope.searchResults.length > 0) {
                                scope.selectItem(scope.searchResults[scope.selectedIndex]);
                            }
                        }
                        break;
                }
            };

            // Add custom item
            scope.addCustomItem = function () {
                var customItem = {
                    id: Date.now(),
                    name: scope.searchTerm,
                    code: 'CUSTOM',
                    category: 'Custom',
                    price: 0,
                    stock: 0,
                    isCustom: true
                };

                scope.onSelect({ item: customItem });
                scope.resetSearch();
            };

            // Reset search state
            scope.resetSearch = function () {
                scope.searchTerm = '';
                scope.searchResults = [];
                scope.showResults = false;
                scope.selectedIndex = -1;

                // Auto-focus after selection if enabled
                if (scope.autoFocus) {
                    scope.focusSearch();
                }
            };

            // Select item
            scope.selectItem = function (item) {
                scope.onSelect({ item: item });
                scope.resetSearch();
            };

            // Handle barcode scan
            scope.handleScan = function () {
                if (scope.disabled) return;

                if (scope.onScan) {
                    scope.onScan();
                } else {
                    // Default scan behavior - simulate barcode input
                    scope.searchTerm = 'SCAN_' + Date.now();
                    scope.handleKeyup({ keyCode: 0 });
                }
            };

            // Initial focus
            $timeout(function () {
                scope.focusSearch();
            }, 100);

            // Close results when clicking outside (but keep focus)
            angular.element(document).on('click', function (event) {
                var searchContainer = element[0].querySelector('.pos-autocomplete');
                if (!searchContainer.contains(event.target)) {
                    scope.$apply(function () {
                        scope.showResults = false;
                        scope.selectedIndex = -1;
                    });
                }
            });

            // Global keyboard shortcut for focus (Ctrl+1)
            angular.element($window).on('keydown', function (event) {
                if (event.ctrlKey && event.key === '1') {
                    event.preventDefault();
                    scope.$apply(function () {
                        scope.focusSearch();
                    });
                }
            });

            // Cleanup
            scope.$on('$destroy', function () {
                angular.element(document).off('click');
                angular.element($window).off('keydown');
            });
        }
    };
})