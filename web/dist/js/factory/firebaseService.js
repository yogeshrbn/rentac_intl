
// app/services/firebase-service.js
app.factory('FirebaseService', ['$q', '$rootScope', '$http', function ($q, $rootScope, $http) {

    // Firebase configuration
    const firebaseConfig = {
        apiKey: "AIzaSyDGOsIjfDbsg3aLEIN41PQ1rXiFIwqlIZQ",
        authDomain: "rentac-472610.firebaseapp.com",
        projectId: "rentac-472610",
        storageBucket: "rentac-472610.firebasestorage.app",
        messagingSenderId: "1050659470536",
        appId: "1:1050659470536:web:df4ecc931d616b752539fd",
        measurementId: "G-LC6BWWXJ9C"
    };

    // Initialize Firebase
    firebase.initializeApp(firebaseConfig);

    let messaging = null;
    if (firebase.messaging.isSupported()) {
        messaging = firebase.messaging();
    }

    const VAPID_KEY = 'BP7Lh-PSlYp5l-eNfdWzNDY7kh_6-icBzRtIGpoO6jBsXaOX75wUKOkDAxDRsbj6TRJ7JS2yEMaIo6mTOjRH1_g';

    const service = {
        isSupported: false,
        token: null,
        swRegistration: null,

        // Initialize the service
        initialize: function () {
            this.isSupported = this.checkBrowserSupport();

            if (!this.isSupported) {
                console.warn('Firebase Messaging not supported in this browser');
                $rootScope.$broadcast('fcmUnsupported');
                return $q.reject('Not supported');
            }

            return this.registerServiceWorker()
                .then(() => this.setupMessageHandlers())
                .then(() => this.checkExistingPermission())
                .catch(error => {
                    console.error('Initialization error:', error);
                    $rootScope.$broadcast('fcmError', error);
                    throw error;
                });
        },

        // Check browser support
        checkBrowserSupport: function () {
            return (
                typeof window !== 'undefined' &&
                'Notification' in window &&
                'serviceWorker' in navigator &&
                'PushManager' in window &&
                firebase.messaging.isSupported()
            );
        },

        // Register service worker
        registerServiceWorker: function () {
            const deferred = $q.defer();

            if (!this.isSupported) {
                deferred.reject('Not supported');
                return deferred.promise;
            }

            navigator.serviceWorker.register('/firebase-messaging-sw.js')
                .then(registration => {
                    console.log('Service Worker registered:', registration);
                    this.swRegistration = registration;
                    $rootScope.$broadcast('serviceWorkerRegistered', registration);
                    deferred.resolve(registration);
                })
                .catch(error => {
                    console.error('Service Worker registration failed:', error);
                    $rootScope.$broadcast('serviceWorkerError', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        },

        // Setup message handlers
        setupMessageHandlers: function () {
            if (!messaging) {
                return;
            }

            // Handle foreground messages
            messaging.onMessage(payload => {
                console.log('Foreground message received:', payload);
                $rootScope.$apply(() => {
                    $rootScope.$broadcast('fcmMessageReceived', payload);
                });
            });

            // Handle token refresh
            messaging.onTokenRefresh(() => {
                console.log('Token refresh triggered');
                this.getToken().then(token => {
                    $rootScope.$broadcast('fcmTokenRefreshed', token);
                });
            });
        },

        // Check existing permission state
        checkExistingPermission: function () {
            if (Notification.permission === 'granted') {
                return this.getToken();
            }
            return $q.resolve();
        },

        // Request notification permission
        requestPermission: function () {
            const deferred = $q.defer();

            if (!this.isSupported) {
                deferred.reject('Notifications not supported');
                return deferred.promise;
            }

            if (!this.swRegistration) {
                deferred.reject('Service Worker not registered');
                return deferred.promise;
            }

            Notification.requestPermission()
                .then(permission => {
                    if (permission === 'granted') {
                        console.log('Notification permission granted');
                        $rootScope.$broadcast('permissionGranted');
                        return this.getToken();
                    } else {
                        throw new Error('Permission not granted: ' + permission);
                    }
                })
                .then(token => {
                    deferred.resolve(token);
                })
                .catch(error => {
                    console.error('Permission request failed:', error);
                    $rootScope.$broadcast('permissionDenied', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        },

        // Get FCM token
        getToken: function () {
            const deferred = $q.defer();

            if (!messaging) {
                deferred.reject('Messaging not available');
                return deferred.promise;
            }

            messaging.getToken({
                vapidKey: VAPID_KEY,
                serviceWorkerRegistration: this.swRegistration
            })
                .then(token => {
                    if (token) {
                        this.token = token;
                        console.log('FCM token obtained:', token);
                        $rootScope.$broadcast('fcmTokenObtained', token);
                        this.sendTokenToServer(token);
                        deferred.resolve(token);
                    } else {
                        throw new Error('No token available');
                    }
                })
                .catch(error => {
                    console.error('Token retrieval failed:', error);
                    $rootScope.$broadcast('fcmTokenError', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        },

        // Delete token (disable notifications)
        deleteToken: function () {
            const deferred = $q.defer();

            if (!messaging || !this.token) {
                deferred.resolve();
                return deferred.promise;
            }

            messaging.deleteToken(this.token)
                .then(() => {
                    console.log('Token deleted');
                    this.token = null;
                    $rootScope.$broadcast('fcmTokenDeleted');
                    this.notifyServerTokenRemoval();
                    deferred.resolve();
                })
                .catch(error => {
                    console.error('Token deletion failed:', error);
                    deferred.reject(error);
                });

            return deferred.promise;
        },

        // Send token to server
        sendTokenToServer: function (token) {
            // Implement your server API call here
            console.log('Sending token to server:', token);

            // Example:
            // return $http.post('/api/tokens/save', {
            //     token: token,
            //     platform: 'web',
            //     userAgent: navigator.userAgent,
            //     timestamp: new Date().toISOString()
            // });
        },

        // Notify server about token removal
        notifyServerTokenRemoval: function () {
            // Implement your server API call here
            console.log('Notifying server about token removal');

            // Example:
            // return $http.post('/api/tokens/remove', {
            //     token: this.token
            // });
        },

        // Send test notification (client-side simulation)
        sendTestNotification: function () {
            const testPayload = {
                notification: {
                    title: 'Test Notification',
                    body: 'This is a test notification from your AngularJS app!',
                    icon: '/assets/icon-192x192.png',
                    click_action: window.location.href
                },
                data: {
                    type: 'test',
                    timestamp: new Date().toISOString(),
                    url: window.location.href
                }
            };

            $rootScope.$broadcast('fcmMessageReceived', testPayload);

            // Also show browser notification if permitted
            if (Notification.permission === 'granted') {
                this.showBrowserNotification(testPayload);
            }

            return $q.resolve(testPayload);
        },

        // Show browser notification
        showBrowserNotification: function (payload) {
            if (Notification.permission !== 'granted') {
                return;
            }

            const options = {
                body: payload.notification?.body,
                icon: payload.notification?.icon || '/assets/icon-192x192.png',
                badge: '/assets/badge-72x72.png',
                data: payload.data || {},
                tag: payload.notification?.tag || 'general',
                requireInteraction: true
            };

            const notification = new Notification(
                payload.notification?.title || 'Notification',
                options
            );

            notification.onclick = () => {
                window.focus();
                notification.close();

                if (payload.data?.url) {
                    window.open(payload.data.url, '_blank');
                }

                $rootScope.$apply(() => {
                    $rootScope.$broadcast('notificationClicked', payload);
                });
            };
        },

        // Get current token
        getCurrentToken: function () {
            return this.token;
        },

        // Check if notifications are enabled
        isNotificationsEnabled: function () {
            return !!this.token && Notification.permission === 'granted';
        }
    };

    return service;
}]);