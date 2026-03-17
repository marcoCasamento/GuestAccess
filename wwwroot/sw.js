const CACHE_NAME = 'guestaccess-v1';

// Static assets to pre-cache on install
const PRECACHE_ASSETS = [
  '/',
  '/css/site.css',
  '/lib/bootstrap/dist/css/bootstrap.min.css',
  '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
  '/lib/jquery/dist/jquery.min.js',
  '/icons/icon-192x192.png',
  '/icons/icon-512x512.png',
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => cache.addAll(PRECACHE_ASSETS))
  );
  self.skipWaiting();
});

self.addEventListener('activate', event => {
  // Remove old caches from previous versions
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k)))
    )
  );
  self.clients.claim();
});

self.addEventListener('fetch', event => {
  const { request } = event;
  const url = new URL(request.url);

  // NEVER cache or intercept POST requests (gate actions must always reach the server)
  if (request.method !== 'GET') return;

  // NEVER cache cross-origin requests (CDN, external auth endpoints)
  if (url.origin !== self.location.origin) return;

  // NEVER cache Identity/auth pages — always fresh from server
  if (url.pathname.startsWith('/Identity/')) return;

  // Static assets (css, js, fonts, images): cache-first
  if (
    url.pathname.startsWith('/css/') ||
    url.pathname.startsWith('/js/') ||
    url.pathname.startsWith('/lib/') ||
    url.pathname.startsWith('/icons/') ||
    url.pathname.match(/\.(png|jpg|jpeg|svg|ico|woff2?|ttf)$/)
  ) {
    event.respondWith(
      caches.match(request).then(cached => cached || fetch(request).then(response => {
        const clone = response.clone();
        caches.open(CACHE_NAME).then(cache => cache.put(request, clone));
        return response;
      }))
    );
    return;
  }

  // Navigation (HTML pages): network-first so permissions are always current.
  // Fall back to cache only if completely offline.
  event.respondWith(
    fetch(request)
      .then(response => {
        const clone = response.clone();
        caches.open(CACHE_NAME).then(cache => cache.put(request, clone));
        return response;
      })
      .catch(() => caches.match(request))
  );
});
