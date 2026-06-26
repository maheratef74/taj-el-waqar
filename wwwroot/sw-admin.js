/* Taj El-Waqar Admin PWA Service Worker */

const CACHE_NAME = 'taj-waqar-admin-v1';
const urlsToCache = [
  '/Dashboard',
  '/image/app_logo.svg',
  '/image/logo_el_maher.svg'
];

self.addEventListener('install', event => {
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', event => {
    // Simple pass-through for stability
    event.respondWith(fetch(event.request).catch(() => caches.match(event.request)));
});
