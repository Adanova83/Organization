// Tableaux sur téléphone et menu de l'utilisateur.
// Tableaux sur téléphone : chaque cellule reçoit l'intitulé de sa colonne (data-label), affiché par app.css quand le
// tableau passe en fiches. Relancé après chaque navigation de Blazor (« enhanced navigation »).
(function () {
    function label(root) {
        root.querySelectorAll("table").forEach(function (table) {
            var headers = Array.prototype.map.call(table.querySelectorAll("thead th"), function (th) {
                return th.textContent.trim();
            });
            table.querySelectorAll("tbody tr").forEach(function (row) {
                Array.prototype.forEach.call(row.children, function (cell, index) {
                    cell.setAttribute("data-label", headers[index] || "");
                });
            });
        });
    }

    // Menu de l'utilisateur : refermé en cliquant ailleurs, et après chaque changement de page.
    function closeUserMenu(event) {
        document.querySelectorAll("details.user-menu[open]").forEach(function (menu) {
            if (!event || !menu.contains(event.target)) {
                menu.removeAttribute("open");
            }
        });
    }

    document.addEventListener("click", closeUserMenu);
    document.addEventListener("DOMContentLoaded", function () { label(document); });
    if (window.Blazor) {
        Blazor.addEventListener("enhancedload", function () { label(document); closeUserMenu(); });
    }
})();
