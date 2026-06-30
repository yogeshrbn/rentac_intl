app.factory('tabSyncService', ['$window', '$rootScope', '$timeout', '$log',
    function ($window, $rootScope, $timeout, $log) {
        var service = {};
        var tabId = generateTabId();
        var isInitialized = false;

        // Generate unique tab ID
        function generateTabId() {
            return 'tab_' + Math.random().toString(36).substr(2, 9) + '_' + Date.now();
        }

        // Initialize tab synchronization
        service.init = function (options) {
            if (isInitialized) {
                $log.warn('TabSyncService already initialized');
                return;
            }

            var config = angular.extend({
                autoRefresh: true,
                showConfirmation: true,
                refreshMessage: 'A new tab was opened. Refresh this page?',
                excludePaths: [],
                debug: false
            }, options);

            if (config.debug) {
                $log.log('TabSyncService initializing with tab ID:', tabId);
            }

            // Store current tab ID
            $window.localStorage.setItem('currentTabId', tabId);

            // Set up storage event listener
            setupStorageListener(config);

            // Set up visibility change listener
            setupVisibilityListener(config);

            // Set up beforeunload listener for cleanup
            setupCleanupListener();

            // Check if this is a new tab and notify others
            checkTabStatus(config);

            isInitialized = true;

            if (config.debug) {
                $log.log('TabSyncService initialized successfully');
            }
        };

        // Set up storage event listener
        function setupStorageListener(config) {
            angular.element($window).on('storage', function (event) {
                var e = event.originalEvent;

                if (config.debug) {
                    $log.log('Storage event:', e.key, e.newValue);
                }

                if (e.key === 'refreshRequired' && e.newValue) {
                    handleRefreshRequired(config);
                } else if (e.key === 'tabOpened' && e.newValue) {
                    handleTabOpened(e.newValue, config);
                }
            });
        }

        // Set up visibility change listener
        function setupVisibilityListener(config) {
            angular.element(document).on('visibilitychange', function () {
                if (!document.hidden) {
                    // Tab became active - check if refresh is needed
                    checkForRefresh(config);
                }
            });
        }

        // Set up cleanup listener
        function setupCleanupListener() {
            angular.element($window).on('beforeunload', function () {
                service.cleanup();
            });
        }

        // Handle refresh required event
        function handleRefreshRequired(config) {
            if (shouldRefresh(config)) {
                debugger
              //  if ($window.confirm(config.refreshMessage)) {
                    triggerRefresh();
               // }
                if (config.autoRefresh) {
                    //if (config.showConfirmation) {
                    //    if ($window.confirm(config.refreshMessage)) {
                    //        triggerRefresh();
                    //    }
                    //} else {
                    //    triggerRefresh();
                    //}
                } else {
                    $rootScope.$broadcast('tabSync:refreshRequired');
                }
            }
        }

        // Handle new tab opened event
        function handleTabOpened(newTabId, config) {
            if (newTabId !== tabId) {
                if (config.debug) {
                    $log.log('New tab detected:', newTabId);
                }
                $rootScope.$broadcast('tabSync:newTabOpened', { newTabId: newTabId });
            }
        }

        // Check if current tab should refresh
        function shouldRefresh(config) {
            var currentPath = $window.location.pathname;

            // Check if current path is excluded
            if (config.excludePaths.some(function (path) {
                return currentPath.indexOf(path) === 0;
            })) {
                return false;
            }

            return true;
        }

        // Trigger page refresh
        function triggerRefresh() {
            //$timeout(function () {
            //    $window.location.reload();
            //}, 100);
        }

        // Check for pending refresh
        function checkForRefresh(config) {
            var refreshRequired = $window.localStorage.getItem('refreshRequired');
            if (refreshRequired && shouldRefresh(config)) {
                handleRefreshRequired(config);
            }
        }

        // Check tab status and notify if this is a new tab
        function checkTabStatus(config) {
            var lastActiveTab = $window.sessionStorage.getItem('lastActiveTab');
            var currentTime = Date.now();

            if (!lastActiveTab) {
                // First tab in session
                $window.sessionStorage.setItem('lastActiveTab', tabId);
                $window.sessionStorage.setItem('lastActiveTime', currentTime);
            } else if (lastActiveTab !== tabId) {
                // New tab opened - check if it's within a short time frame
                var lastActiveTime = parseInt($window.sessionStorage.getItem('lastActiveTime') || '0');
                var timeDiff = currentTime - lastActiveTime;

                if (timeDiff < 5000) { // 5 second window
                    if (config.debug) {
                        $log.log('New tab opened within time window, notifying other tabs');
                    }
                    notifyTabOpened();
                }

                // Update session storage
                $window.sessionStorage.setItem('lastActiveTab', tabId);
                $window.sessionStorage.setItem('lastActiveTime', currentTime);
            }
        }

        // Notify other tabs that this tab was opened
        function notifyTabOpened() {
            // Set refresh flag for other tabs
            $window.localStorage.setItem('refreshRequired', 'true');

            // Also store tab ID for tracking
            $window.localStorage.setItem('tabOpened', tabId);

            // Clear flags after short delay
            $timeout(function () {
                $window.localStorage.removeItem('refreshRequired');
                $window.localStorage.removeItem('tabOpened');
            }, 500);
        }

        // Manually trigger refresh in other tabs
        service.triggerRefreshInOtherTabs = function () {
            $window.localStorage.setItem('refreshRequired', 'true');
            $timeout(function () {
                $window.localStorage.removeItem('refreshRequired');
            }, 500);

            $rootScope.$broadcast('tabSync:refreshTriggered');
        };

        // Get current tab ID
        service.getTabId = function () {
            return tabId;
        };

        // Check if this is the primary tab
        service.isPrimaryTab = function () {
            return $window.sessionStorage.getItem('lastActiveTab') === tabId;
        };

        // Clean up resources
        service.cleanup = function () {
            //angular.element($window).off('storage');
            //angular.element($document).off('visibilitychange');
            //angular.element($window).off('beforeunload');
            isInitialized = false;
        };

        // Destroy service
        service.destroy = function () {
            service.cleanup();
            $window.localStorage.removeItem('refreshRequired');
            $window.localStorage.removeItem('tabOpened');
        };

        return service;
    }]);