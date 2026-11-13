// Application installable (AdaNova 83) : service worker, appel hors connexion, installation.
(function () {
    var QUEUE_KEY = "adanova.pending-forms";

    // ---- Service worker : pages consultées gardées pour le hors connexion (voir sw.js) ----
    if ("serviceWorker" in navigator) {
        window.addEventListener("load", function () {
            navigator.serviceWorker.register("/sw.js").catch(function () { /* navigateur sans service worker : le site marche quand même */ });
        });
    }

    // ---- Déconnexion : les pages gardées sur le téléphone (participants, contacts...) sont effacées ----
    document.addEventListener("click", function (event) {
        var link = event.target.closest && event.target.closest('a[href^="/api/auth/logout"]');
        if (link && navigator.serviceWorker && navigator.serviceWorker.controller) {
            navigator.serviceWorker.controller.postMessage("clear-pages");
        }
    }, true);

    // ---- Formulaires marqués data-offline-queue (l'appel) : sans réseau, gardés puis envoyés au retour ----
    function readQueue() {
        try { return JSON.parse(localStorage.getItem(QUEUE_KEY) || "[]"); } catch (e) { return []; }
    }

    function writeQueue(queue) {
        try { localStorage.setItem(QUEUE_KEY, JSON.stringify(queue)); } catch (e) { /* stockage indisponible */ }
        showBanner();
    }

    function showBanner() {
        var count = readQueue().length;
        var banner = document.getElementById("offline-queue-banner");
        if (count === 0) {
            if (banner) { banner.remove(); }
            return;
        }
        if (!banner) {
            banner = document.createElement("p");
            banner.id = "offline-queue-banner";
            banner.className = "notice offline-banner";
            var content = document.querySelector("main.content") || document.body;
            content.insertBefore(banner, content.firstChild);
        }
        banner.textContent = count === 1
            ? "Un appel est enregistré sur ce téléphone : il sera envoyé dès le retour du réseau."
            : count + " appels sont enregistrés sur ce téléphone : ils seront envoyés dès le retour du réseau.";
    }

    // Capture : passe avant le traitement des formulaires de Blazor.
    document.addEventListener("submit", function (event) {
        var form = event.target;
        if (!form.hasAttribute || !form.hasAttribute("data-offline-queue") || navigator.onLine) {
            return;
        }
        event.preventDefault();
        event.stopImmediatePropagation();

        var fields = [];
        new FormData(form).forEach(function (value, name) { fields.push([name, String(value)]); });
        var queue = readQueue();
        queue.push({ url: form.action || location.href, fields: fields, savedAt: new Date().toISOString() });
        writeQueue(queue);
    }, true);

    var sending = false;

    function sendQueue() {
        var queue = readQueue();
        if (sending || queue.length === 0 || !navigator.onLine) {
            showBanner();
            return;
        }
        sending = true;
        var item = queue[0];
        fetch(item.url, {
            method: "POST",
            credentials: "same-origin",
            body: new URLSearchParams(item.fields)
        }).then(function (response) {
            sending = false;
            if (response.ok) {
                writeQueue(readQueue().slice(1));
                sendQueue();
            } else {
                showBanner();
            }
        }).catch(function () {
            sending = false;
            showBanner();
        });
    }

    window.addEventListener("online", sendQueue);
    document.addEventListener("DOMContentLoaded", function () {
        sendQueue();
        prefetchRollCalls();
    });

    // ---- Appels du jour : pages ouvertes à l'avance, pour pouvoir faire l'appel même sans réseau ----
    function prefetchRollCalls() {
        if (!navigator.onLine || !navigator.serviceWorker || !navigator.serviceWorker.controller) {
            return;
        }
        document.querySelectorAll("a.agenda-action").forEach(function (link) {
            fetch(link.href, { credentials: "same-origin", headers: { "Accept": "text/html" } }).catch(function () { });
        });
    }

    // ---- Installation (Android, Chrome, Edge) : bouton « Installer » de la page d'installation ----
    var installPrompt = null;
    window.addEventListener("beforeinstallprompt", function (event) {
        event.preventDefault();
        installPrompt = event;
        document.querySelectorAll("[data-install-button]").forEach(function (button) { button.hidden = false; });
    });

    document.addEventListener("click", function (event) {
        var button = event.target.closest && event.target.closest("[data-install-button]");
        if (button && installPrompt) {
            installPrompt.prompt();
            installPrompt = null;
            button.hidden = true;
        }
    });

    if (window.Blazor) {
        Blazor.addEventListener("enhancedload", function () {
            showBanner();
            prefetchRollCalls();
            if (installPrompt) {
                document.querySelectorAll("[data-install-button]").forEach(function (button) { button.hidden = false; });
            }
        });
    }
})();
