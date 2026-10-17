# Travaux à faire

Liste des travaux restants, par ordre de priorité. Cocher une tâche une fois terminée.

## Priorité haute

- [x] **Tester le site connecté (administrateur)** : 57 pages parcourues sans erreur ; création d'une série et
      inscription par les formulaires ; corrigé : script Blazor absent de l'image Docker, pré-contrôle d'âge des
      responsables, mise en page de la colonne « Inscription ».
- [ ] **Tester avec un compte aux droits limités** : côté API, vérifié avec le profil « membre-test »
      (groupe « membres », lecture seule, `docker-compose.dev.yml`) ; reste à parcourir le site connecté avec ce profil
      (menus, pages refusées, boutons masqués) — membre `membre.test.org` à créer dans la base.
- [x] **Tests automatiques** : projet `Source/Tests/Organizations.Tests` (xUnit) : récurrence des séries, rythme
      vacances / jours fériés, Pâques et jours fériés, dates des vacances importées, heure de Paris, exclusions ICS.
- [ ] **Tests à compléter** : handlers (validations), endpoints de l'API (droits `*.read` / `*.edit`, données d'un
      membre), import des vacances avec une réponse simulée du calendrier national.
- [x] **`.vscode/launch.json`** : débogage de l'API, du site web, ou des deux ensemble.

## Fonctionnalités incomplètes

- [x] **Projets** : rythme « hors vacances / pendant les vacances » (jamais les jours fériés), couleur, et affichage
      dans le calendrier et le flux ICS.
- [x] **Inscriptions** : règles côté API (doublons, capacité, âge minimum au début de l'activité, date de naissance
      requise), dérogation des responsables (`subscription.edit`) pour la capacité et l'âge.
- [x] **Places restantes et liste d'attente** : places visibles par tous (sans les noms), liste d'attente automatique
      quand c'est complet, promotion automatique à chaque désinscription ; tableau de bord sur l'accueil (mes
      inscriptions, prochains événements, remplissage pour `subscription.read`).
- [ ] **Liste d'attente et capacité augmentée** : augmenter la capacité d'un créneau ou d'un événement ne fait pas encore
      passer les personnes en attente (seule une désinscription le fait).
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
