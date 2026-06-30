app
    .directive('keyboardTable', function () {
        return {
            restrict: 'E',
            scope: {
                tableData: '=',
                tableConfig: '=',
                onRowSelect: '&',
                onEdit: '&',
                onDelete: '&'

            },
            template: `
            <div class="keyboard-table-directive">
                <!-- Search Filter -->
                <div class="search-container" ng-if="config.enableSearch">
                    <input type="text" id="{{tableConfig.inputId}}"
                           ng-model="local.searchText"
                           placeholder="{{ config.searchPlaceholder || 'Search...' }}"
                           class="search-input form-control"
                           aria-label="Search table data"
                           ng-keydown="handleSearchKeyPress($event)"
                           ng-focus="onSearchFocus()"
                           ng-blur="onSearchBlur()">
                </div>
                <!-- Instructions -->                          
                <!-- Data Table -->
                <div class="table-container" style="max-height: 400px; overflow-y: scroll;"  >
                    <table class="keyboard-table table table-condensed"
                           role="grid" 
                           aria-label="Data table"
                           ng-keydown="handleKeyPress($event)">
                      
                            <tr class=headerRow">
                                <th scope="col" 
                                    ng-repeat="header in config.headers"
                                    ng-if="header.visible !== false"
                                    ng-click="sortTable(header.key)"
                                    ng-class="{'sortable-header': header.sortable !== false}"
                                    class="sortable-header"
                                    tabindex="0"
                                    ng-keydown="handleHeaderKeyPress($event, header.key)"
                                    ng-attr-aria-sort="{{sortColumn === header.key ? (sortDirection === 'asc' ? 'ascending' : 'descending') : 'none'}}">
                                    {{ header.label }}
                                    <span class="sort-indicator" ng-if="sortColumn === header.key && header.sortable !== false">
                                        {{ sortDirection === 'asc' ? '↑' : '↓' }}
                                    </span>
                                </th>
                                <th scope="col" ng-if="config.enableActions">Actions</th>
                            </tr>
                     
                        <tbody>
                            <tr ng-repeat="item in filteredData = (tableData | filter: local.searchText) | orderBy: sortColumn : sortDirection === 'desc'"
                                ng-class="{
                                    'selected': selectedRow === $index, 
                                    'focused': focusedRow === $index, 
                                    'search-focused': searchFocusedRow === $index
                                }"
                                ng-click="selectRow($index)"
                                tabindex="0"
                                ng-focus="focusRow($index)"
                                ng-blur="blurRow()"
                                role="row"
                                aria-selected="{{selectedRow === $index}}">
                                
                                <td ng-repeat="header in config.headers" ng-if="header.visible !== false">
                                    {{ item[header.key] }}
                                </td>
                                <td class="actions" ng-if="config.enableActions">
                                    <button ng-click="editItem(item, $event)"
                                            class="btn btn-edit"
                                            aria-label="Edit {{ item[config.headers[0].key] }}">
                                        Edit
                                    </button>
                                    <button ng-click="deleteItem(item, $event)"
                                            class="btn btn-delete"
                                            aria-label="Delete {{ item[config.headers[0].key] }}">
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <!-- No results message -->
                    <div ng-if="filteredData.length === 0" class="no-results">
                        No results found for "{{ searchText }}"
                    </div>

                    <!-- Status information -->
                 <!--    <div class="status-info" aria-live="polite">
                        {{ filteredData.length }} items found
                        <span ng-if="selectedRow !== null">
                            • Row {{ selectedRow + 1 }} selected
                        </span>
                        <span ng-if="searchFocusedRow !== null && searchHasFocus">
                            • Row {{ searchFocusedRow + 1 }} focused (from search)
                        </span>
                    </div>-->
                </div>
            </div>
        `,
            link: function (scope, element, attrs) {

                // Initialize configuration with defaults
                scope.config = angular.extend({
                    headers: [],
                    enableSearch: true,
                    enableSort: true,
                    enableActions: true,
                    inputId: 'input' + crypto.randomUUID().replace(/-/g, ''),
                    defaultSort: 'ProductId',
                    defaultSortDirection: 'asc',
                    searchPlaceholder: 'Search...'
                }, scope.tableConfig);
                scope.local = { searchText: '' };
                // Initialize state
                scope.local.searchText = '';
                scope.selectedRow = null;
                scope.focusedRow = null;
                scope.searchFocusedRow = null;
                scope.selectedItem = null;
                scope.sortColumn = scope.config.defaultSort;
                scope.sortDirection = scope.config.defaultSortDirection;
                scope.searchHasFocus = false;
                scope.filteredData = [];

                // Navigation methods
                scope.selectRow = function (index) {
                    scope.selectedRow = index;
                    scope.selectedItem = scope.filteredData[index];
                    scope.focusedRow = index;
                    scope.searchFocusedRow = null;

                    // Call callback if provided
                    if (scope.onRowSelect) {
                        scope.onRowSelect({ item: scope.selectedItem });
                    }
                };

                scope.focusRow = function (index) {
                    scope.focusedRow = index;
                    scope.searchFocusedRow = null;
                };

                scope.blurRow = function () {
                    scope.focusedRow = null;
                };

                // Search input focus handlers
                scope.onSearchFocus = function () {
                    scope.searchHasFocus = true;
                    if (scope.searchFocusedRow === null && scope.filteredData.length > 0) {
                        scope.searchFocusedRow = 0;
                    }
                };

                scope.onSearchBlur = function () {
                    scope.searchHasFocus = false;
                    scope.searchFocusedRow = null;
                };

                // Keyboard navigation handler for search input
                scope.handleSearchKeyPress = function (event) {
                    var key = event.key;
                     
                    if (scope.filteredData.length === 0) return;

                    switch (key) {
                        case 'ArrowUp':
                            event.preventDefault();
                            if (scope.searchFocusedRow === null || scope.searchFocusedRow === 0) {
                                scope.searchFocusedRow = scope.filteredData.length - 1;
                            } else {
                                scope.searchFocusedRow--;
                            }
                            scope.scrollToSearchFocusedRow();
                            break;

                        case 'ArrowDown':
                            event.preventDefault();
                            if (scope.searchFocusedRow === null || scope.searchFocusedRow === scope.filteredData.length - 1) {
                                scope.searchFocusedRow = 0;
                            } else {
                                scope.searchFocusedRow++;
                            }
                            scope.scrollToSearchFocusedRow();
                            break;

                        case 'Enter':
                            if (scope.searchFocusedRow !== null) {
                                event.preventDefault();
                                scope.selectRowFromSearch(scope.searchFocusedRow);
                            }
                            break;

                        case 'Escape':
                            if (scope.searchFocusedRow !== null) {
                                event.preventDefault();
                                scope.searchFocusedRow = null;
                            }
                            break;

                        case 'Home':
                            if (scope.filteredData.length > 0) {
                                event.preventDefault();
                                scope.searchFocusedRow = 0;
                                scope.scrollToSearchFocusedRow();
                            }
                            break;

                        case 'End':
                            if (scope.filteredData.length > 0) {
                                event.preventDefault();
                                scope.searchFocusedRow = scope.filteredData.length - 1;
                                scope.scrollToSearchFocusedRow();
                            }
                            break;
                    }
                };

                // Select row when using search input navigation
                scope.selectRowFromSearch = function (index) {
                    scope.selectedRow = index;
                    scope.selectedItem = scope.filteredData[index];
                    scope.searchFocusedRow = null;

                    // Call callback if provided
                    if (scope.onRowSelect) {
                        scope.onRowSelect({ item: scope.selectedItem });
                    }
                };

                // Scroll to make the search-focused row visible
                scope.scrollToSearchFocusedRow = function () {
                    setTimeout(function () {
                        var rows = element[0].querySelectorAll('.keyboard-table tbody tr');
                        if (rows[scope.searchFocusedRow]) {
                            rows[scope.searchFocusedRow].scrollIntoView({
                                behavior: 'smooth',
                                block: 'nearest'
                            });
                        }
                    }, 0);
                };

                // Main table keyboard navigation handler
                scope.handleKeyPress = function (event) {
                    var key = event.key;
                    var currentIndex = scope.focusedRow;

                    if (currentIndex === null || currentIndex === undefined) return;

                    switch (key) {
                        case 'ArrowUp':
                            event.preventDefault();
                            if (currentIndex > 0) {
                                scope.navigateToRow(currentIndex - 1);
                            }
                            break;

                        case 'ArrowDown':
                            event.preventDefault();
                            if (currentIndex < scope.filteredData.length - 1) {
                                scope.navigateToRow(currentIndex + 1);
                            }
                            break;

                        case 'Home':
                            event.preventDefault();
                            if (scope.filteredData.length > 0) {
                                scope.navigateToRow(0);
                            }
                            break;

                        case 'End':
                            event.preventDefault();
                            if (scope.filteredData.length > 0) {
                                scope.navigateToRow(scope.filteredData.length - 1);
                            }
                            break;

                        case 'Enter':
                            event.preventDefault();
                            scope.selectRow(currentIndex);
                            break;

                        case 'Escape':
                            event.preventDefault();
                            scope.clearSelection();
                            break;
                    }
                };

                // Header keyboard navigation
                scope.handleHeaderKeyPress = function (event, columnKey) {
                    if ((event.key === 'Enter' || event.key === ' ') && scope.config.enableSort) {
                        event.preventDefault();
                        scope.sortTable(columnKey);
                    }
                };

                // Navigate to specific row
                scope.navigateToRow = function (index) {
                    scope.focusedRow = index;
                    scope.searchFocusedRow = null;
                    var rows = element[0].querySelectorAll('.keyboard-table tbody tr');
                    if (rows[index]) {
                        rows[index].focus();
                    }
                };

                // Clear selection
                scope.clearSelection = function () {
                    scope.selectedRow = null;
                    scope.selectedItem = null;
                    scope.focusedRow = null;
                    scope.searchFocusedRow = null;
                };

                // Sort table
                scope.sortTable = function (column) {
                    if (!scope.config.enableSort) return;

                    if (scope.sortColumn === column) {
                        scope.sortDirection = scope.sortDirection === 'asc' ? 'desc' : 'asc';
                    } else {
                        scope.sortColumn = column;
                        scope.sortDirection = 'asc';
                    }
                };

                // Edit item
                scope.editItem = function (item, event) {
                    event.stopPropagation();
                    if (scope.onEdit) {
                        scope.onEdit({ item: item });
                    }
                };

                // Delete item
                scope.deleteItem = function (item, event) {
                    event.stopPropagation();
                    if (scope.onDelete) {
                        scope.onDelete({ item: item });
                    }
                };

                // Watch for configuration changes
                scope.$watch('tableConfig', function (newConfig) {
                    if (newConfig) {
                        scope.config = angular.extend(scope.config, newConfig);
                    }
                }, true);

                // Initialize
                scope.clearSelection();
            }
        };
    });