// Service worker de l'application installable (AdaNova 83).
// - Pages (HTML) : réseau d'abord ; copie gardée sur le téléphone pour être relue hors connexion.
// - Fichiers statiques (CSS, JS, icônes) : copie locale d'abord, mise à jour en arrière-plan.
// - Jamais en cache : l'API, la connexion / déconnexion, les envois de formulaires (POST).
// La déconnexion efface les pages gardées (voir pwa.js).

const VERSION = "v1";
const PAGES = `pages-${VERSION}`;
const ASSETS = `assets-${VERSION}`;
const OFFLINE = "/offline.html";

self.addEventListener("install", (event) => {
    event.waitUntil(caches.open(ASSETS).then((cache) => cache.add(OFFLINE)));
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil((async () => {
        for (const key of await caches.keys()) {
            if (key !== PAGES && key !== ASSETS) {
                await caches.delete(key);
            }
        }
        await self.clients.claim();
    })());
});

self.addEventListener("message", (event) => {
    if (event.data === "clear-pages") {
        event.waitUntil(caches.delete(PAGES));
    }
});

function isPage(request) {
    return request.mode === "navigate" || (request.headers.get("accept") || "").includes("text/html");
}

function isExcluded(url) {
    return url.origin !== self.location.origin
        || url.pathname.startsWith("/api/")
        || url.pathname.startsWith("/_blazor")
        || url.pathname === "/sw.js";
}

self.addEventListener("fetch", (event) => {
    const request = event.request;
    const url = new URL(request.url);

    if (request.method !== "GET" || isExcluded(url)) {
        return;
    }

    if (isPage(request)) {
        event.respondWith(pageFromNetwork(request));
    } else {
        event.respondWith(assetFromCache(request, event));
    }
});

async function pageFromNetwork(request) {
    const cache = await caches.open(PAGES);
    try {
        const response = await fetch(request);
        // Seules les vraies pages du site sont gardées (pas une redirection vers la connexion, pas une erreur).
        if (response.ok && !response.redirected && (response.headers.get("content-type") || "").includes("text/html")) {
            await cache.put(request.url, response.clone());
        }
        return response;
    } catch {
        return (await cache.match(request.url)) || (await caches.match(OFFLINE));
    }
}

async function assetFromCache(request, event) {
    const cache = await caches.open(ASSETS);
    const cached = await cache.match(request);
    const network = fetch(request).then(async (response) => {
        if (response.ok) {
            await cache.put(request, response.clone());
        }
        return response;
    }).catch(() => cached || Response.error());

    if (cached) {
        event.waitUntil(network);
        return cached;
    }
    return network;
}
