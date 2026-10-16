# Travaux à faire

Liste des travaux restants, par ordre de priorité. Cocher une tâche une fois terminée.

## Priorité haute

- [ ] **Tester le site connecté** : parcourir toutes les pages (profil, années scolaires, ateliers, sessions,
      événements et répétitions, calendrier, notes, badges, cotisations…) avec un compte administrateur, puis avec
      un compte aux droits limités (`*.read` seulement) pour vérifier les droits.
- [x] **Tests automatiques** : projet `Source/Tests/Organizations.Tests` (xUnit) : récurrence des séries, rythme
      vacances / jours fériés, Pâques et jours fériés, dates des vacances importées, heure de Paris, exclusions ICS.
- [ ] **Tests à compléter** : handlers (validations), endpoints de l'API (droits `*.read` / `*.edit`, données d'un
      membre), import des vacances avec une réponse simulée du calendrier national.
- [x] **`.vscode/launch.json`** : débogage de l'API, du site web, ou des deux ensemble.

## Fonctionnalités incomplètes

- [ ] **Projets** : les afficher dans le calendrier et ajouter à leurs créneaux le rythme « hors vacances / pendant
      les vacances » (comme les sessions d'ateliers).
- [ ] **Inscriptions** : appliquer les règles côté API (âge minimum, capacité maximale, doublons). Aujourd'hui,
      seul le site contrôle l'âge, et la capacité n'existe pas dans le modèle.
- [ ] **Suppression en cascade** : supprimer une activité laisse ses sessions, créneaux, inscriptions et animateurs ;
      décider ce qui doit suivre (suppression, blocage ou avertissement).
- [ ] **Cotisations** : vue « qui n'a pas payé cette saison », et enregistrement du montant payé.
- [ ] **Contrôle d'accès** : endpoints pour les lecteurs de badges (`Application`), enregistrement des passages,
      clé d'API des lecteurs stockée hachée.
- [ ] **Flux ICS** : page indiquant aux membres l'adresse d'abonnement pour l'Agenda Nextcloud.
- [ ] **Séries d'événements** : permettre de modifier le rythme (vacances, jours fériés) d'une série existante ; les
      dates sont aujourd'hui calculées une fois pour toutes à la création.
- [ ] **Profil** : la liste des pays demande `organization.read` ; un membre sans cette permission ne peut pas
      changer de pays.
- [ ] **Formulaires** : les unités affichées à côté de « Tous les N » suivent la fréquence grâce au CSS ; vérifier
      l'affichage sur les navigateurs anciens (sélecteur `:has()`).

## Avant la mise en production

- [ ] Désactiver dans `docker-compose.yml` la clé d'API sur les endpoints métier (`API_ALLOW_KEY_ON_BUSINESS`) et
      Swagger (`API_ENABLE_SWAGGER`).
- [ ] Fixer la version de l'image `mock-oauth2-server` dans `docker-compose.dev.yml` (aujourd'hui `latest`).
- [ ] Configurer Authentik (fournisseur OIDC, claim `groups`) et la correspondance groupes → permissions
      (`Authorization:RolePermissions`), par exemple la lecture du planning pour tous les membres.
- [ ] Données personnelles des membres (RGPD) : décider qui voit quoi, la durée de conservation et l'information
      des membres.
- [ ] Reverse proxy (Traefik) et HTTPS ; sauvegarde régulière de la base PostgreSQL.

## Connu, sans gravité

- [ ] Avertissement `AD0001` à la compilation complète de l'API : plantage de l'analyseur ASP.NET de Microsoft
      (`RouteHandlerAnalyzer`), indépendant du code ; à revérifier lors des mises à jour du SDK.
