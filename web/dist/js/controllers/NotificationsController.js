app.controller('NotificationListController', function ($scope, $state, NotificationSerivice) {

    $scope.Alerts = [];
    $scope.Filter = {};
    var date = new Date();
    $scope.Filter.From = convertDate(date);
    $scope.Filter.To = convertDate(date);
    $scope.Filter.Status = 'all';
    $scope.getAll = function () {
        var model = cloneObj($scope.Filter);
        model.From = formatdate(model.From);
        model.To = formatdate(model.To);

        NotificationSerivice.getAlerts(model).then(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            $scope.Alerts = e.data.Data;
        });
    }
    $scope.getAll();

    $scope.markRead = function (item) {
        item.status = 'read';
        NotificationSerivice.updateStatus(item).then(function (e) {
            if (e.data.Code != 200) {
                alert(e.data.Message);
                return;
            }
            item.Status = 'read';
        });
    }

});
// controllers/notificationController.js
// app/controllers/notification-controller.js
app.controller('NotificationController', [
    '$scope', '$rootScope', 'FirebaseService', '$timeout',
    function ($scope, $rootScope, FirebaseService, $timeout) {
        const vm = this;

        // Controller state
        vm.isSupported = false;
        vm.permissionStatus = 'default';
        vm.fcmToken = null;
        vm.isLoading = false;
        vm.showCopyFeedback = false;

        vm.swStatus = {
            registered: false,
            registering: false
        };

        vm.notifications = [];
        vm.toasts = [];
        let notificationId = 0;
        let toastId = 0;

        // Initialize controller
        vm.initialize = function () {
            vm.isSupported = FirebaseService.isSupported;
            vm.permissionStatus = Notification.permission;

            setupEventListeners();

            if (vm.isSupported) {
                vm.swStatus.registering = true;
                FirebaseService.initialize().finally(() => {
                    vm.swStatus.registering = false;
                });
            }
        };

        // Setup event listeners
        function setupEventListeners() {
            // Service Worker events
            $rootScope.$on('serviceWorkerRegistered', function (event, registration) {
                vm.swStatus.registered = true;
                vm.swStatus.registering = false;
            });

            $rootScope.$on('serviceWorkerError', function (event, error) {
                vm.swStatus.registered = false;
                vm.swStatus.registering = false;
                vm.showToast('error', 'Service Worker Error', 'Failed to register service worker');
            });

            // Permission events
            $rootScope.$on('permissionGranted', function () {
                vm.permissionStatus = 'granted';
            });

            $rootScope.$on('permissionDenied', function (event, error) {
                vm.permissionStatus = 'denied';
                vm.showToast('error', 'Permission Denied', 'Notification permission was denied');
            });

            // FCM Token events
            $rootScope.$on('fcmTokenObtained', function (event, token) {
                vm.fcmToken = token;
                vm.showToast('success', 'Notifications Enabled', 'Push notifications are now active');
            });

            $rootScope.$on('fcmTokenRefreshed', function (event, token) {
                vm.fcmToken = token;
                vm.showToast('info', 'Token Refreshed', 'FCM token has been refreshed');
            });

            $rootScope.$on('fcmTokenDeleted', function () {
                vm.fcmToken = null;
                vm.showToast('info', 'Notifications Disabled', 'Push notifications have been disabled');
            });

            $rootScope.$on('fcmTokenError', function (event, error) {
                vm.showToast('error', 'Token Error', 'Failed to get FCM token: ' + error.message);
            });

            // FCM Message events
            $rootScope.$on('fcmMessageReceived', function (event, payload) {
                handleNewMessage(payload);
            });

            $rootScope.$on('notificationClicked', function (event, payload) {
                vm.addNotification({
                    title: payload.notification?.title + ' (Clicked)',
                    body: 'Notification was clicked by user',
                    type: 'interaction',
                    data: payload.data
                });
            });

            // Error events
            $rootScope.$on('fcmError', function (event, error) {
                vm.showToast('error', 'FCM Error', error.message || 'Unknown error occurred');
            });

            $rootScope.$on('fcmUnsupported', function () {
                vm.showToast('error', 'Not Supported', 'Push notifications are not supported in your browser');
            });
        }

        // Handle new FCM message
        function handleNewMessage(payload) {
            const notification = {
                id: notificationId++,
                title: payload.notification?.title || 'Notification',
                body: payload.notification?.body || 'New message received',
                type: payload.data?.type || 'message',
                data: payload.data,
                timestamp: new Date()
            };

            vm.addNotification(notification);

            // Show toast for foreground messages
            if (payload.notification) {
                vm.showToast('info', payload.notification.title, payload.notification.body);
            }
        }

        // Add notification to log
        vm.addNotification = function (notification) {
            vm.notifications.unshift(notification);

            // Limit to 100 notifications
            if (vm.notifications.length > 100) {
                vm.notifications.pop();
            }
        };

        // Remove specific notification
        vm.removeNotification = function (id) {
            vm.notifications = vm.notifications.filter(notif => notif.id !== id);
        };

        // Clear all notifications
        vm.clearNotifications = function () {
            vm.notifications = [];
        };

        // Show toast message
        vm.showToast = function (type, title, message) {
            const toast = {
                id: toastId++,
                type: type,
                title: title,
                message: message,
                timestamp: new Date()
            };

            vm.toasts.unshift(toast);

            // Auto-remove after 5 seconds
            $timeout(() => {
                vm.removeToast(toast.id);
            }, 5000);

            // Limit to 5 toasts
            if (vm.toasts.length > 5) {
                vm.toasts.pop();
            }
        };

        // Remove toast
        vm.removeToast = function (id) {
            vm.toasts = vm.toasts.filter(toast => toast.id !== id);
        };

        // Request notification permission
        vm.requestPermission = function () {
            vm.isLoading = true;

            FirebaseService.requestPermission()
                .then(token => {
                    vm.fcmToken = token;
                })
                .catch(error => {
                    console.error('Permission request failed:', error);
                    vm.showToast('error', 'Permission Failed', error.message);
                })
                .finally(() => {
                    vm.isLoading = false;
                });
        };

        // Disable notifications
        vm.disableNotifications = function () {
            vm.isLoading = true;

            FirebaseService.deleteToken()
                .then(() => {
                    vm.fcmToken = null;
                })
                .catch(error => {
                    console.error('Disable notifications failed:', error);
                    vm.showToast('error', 'Disable Failed', error.message);
                })
                .finally(() => {
                    vm.isLoading = false;
                });
        };

        // Send test notification
        vm.sendTestNotification = function () {
            FirebaseService.sendTestNotification();
        };

        // Copy token to clipboard
        vm.copyToken = function () {
            if (!vm.fcmToken) return;

            navigator.clipboard.writeText(vm.fcmToken).then(() => {
                vm.showCopyFeedback = true;
                $timeout(() => {
                    vm.showCopyFeedback = false;
                }, 2000);
            });
        };

        // Get permission status text
        vm.getPermissionText = function () {
            switch (vm.permissionStatus) {
                case 'granted': return 'Granted';
                case 'denied': return 'Denied';
                default: return 'Not Set';
            }
        };

        // Get service worker status text
        vm.getSWStatusText = function () {
            if (vm.swStatus.registered) return 'Registered';
            if (vm.swStatus.registering) return 'Registering...';
            return 'Not Registered';
        };

        // Initialize the controller
        vm.initialize();
    }]);