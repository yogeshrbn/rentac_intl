// firebase-messaging-sw.js
importScripts('https://www.gstatic.com/firebasejs/8.10.1/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/8.10.1/firebase-messaging.js');

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
debugger
// Initialize Firebase
firebase.initializeApp(firebaseConfig);
const messaging = firebase.messaging();

// Background message handler
messaging.onBackgroundMessage(function (payload) {
    console.log('[SW] Received background message:', payload);

    const notificationTitle = payload.notification?.title || 'New Notification';
    const notificationOptions = {
        body: payload.notification?.body || 'You have a new message',
        icon: payload.notification?.icon || '/assets/icon-192x192.png',
        badge: '/assets/badge-72x72.png',
        data: payload.data || {},
        tag: 'background-notification',
        actions: payload.data?.actions || []
    };

    return self.registration.showNotification(notificationTitle, notificationOptions);
});

// Notification click handler
self.addEventListener('notificationclick', function (event) {
    console.log('[SW] Notification clicked:', event.notification.tag);

    event.notification.close();

    const urlToOpen = event.notification.data?.url || self.location.origin;

    event.waitUntil(
        clients.matchAll({
            type: 'window',
            includeUncontrolled: true
        }).then(function (windowClients) {
            // Check for existing window
            for (let client of windowClients) {
                if (client.url === urlToOpen && 'focus' in client) {
                    return client.focus();
                }
            }

            // Open new window
            if (clients.openWindow) {
                return clients.openWindow(urlToOpen);
            }
        })
    );
});