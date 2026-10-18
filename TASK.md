# Travaux à faire

Liste des travaux restants, par ordre de priorité. Cocher une tâche une fois terminée.

## Priorité haute

- [x] **Tester le site connecté (administrateur)** : 57 pages parcourues sans erreur ; création d'une série et
      inscription par les formulaires ; corrigé : script Blazor absent de l'image Docker, pré-contrôle d'âge des
      responsables, mise en page de la colonne « Inscription ».
- [ ] **Tester avec un compte aux droits limités** : côté API, vérifié avec le profil « membre-test »
      (groupe « membres », lecture seule, `docker-compose.dev.yml`) ; reste à parcourir le site connecté avec ce profil
      (menus, pages refusées, boutons masqués) avec le membre `membre.test@adanova83.org` (créé dans la base de dev).
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
- [x] **Liste d'attente et capacité augmentée** : augmenter la capacité d'un créneau, d'un événement ou d'un créneau de
      projet, ou supprimer un membre, fait passer les personnes en attente.
- [x] **Suppression en cascade** : suppression logique propagée (activité, session, créneau, événement, projet, note,
      badge, membre) ou refusée avec un message clair (année scolaire, organisation, saison avec cotisations, salle, pays
      encore utilisés) ; confirmations du site détaillées ; tests `DeletionPolicyTests`.
- [x] **Cotisations** : montant payé (paiement en plusieurs fois), page « Cotisations de la saison » (participants non à
      jour, reste à payer, paiement prérempli), bloc « Cotisations » des saisons en cours sur le tableau de bord.
- [ ] **Application mobile** (application web installable) : présences par QR code et appel, rappels, agenda
      personnel ; cahier des charges dans [docs/application-mobile.md](docs/application-mobile.md), en 5 lots.
      Lot 1 fait : présences par séance datée, appel sur le site (animateurs et responsables), feuille d'émargement
      imprimable ou en PDF (impression du navigateur), statistiques de fréquentation (page « Présences »).
- [ ] **Contrôle d'accès** : endpoints pour les lecteurs de badges (`Application`), enregistrement des passages,
      clé d'API des lecteurs stockée hachée.
- [ ] **Flux ICS** : page indiquant aux membres l'adresse d'abonnement pour l'Agenda Nextcloud.
- [ ] **Séries d'événements** : permettre de modifier le rythme (vacances, jours fériés) d'une série existante ; les
      dates sont aujourd'hui calculées une fois pour toutes à la création.
- [x] **Profil** : la liste des pays est lisible par tout compte connecté (adresse du profil).
- [x] **Profils de l'intranet** : groupes `membres` (planning, inscription), `animateurs` (en plus, participants de ce
      qu'ils animent, avec les contacts des familles), `responsables` (planning, inscriptions, membres, cotisations),
      administrateurs (groupe `Admin__Group`) ; contacts des familles (parents, tuteurs) gérés par les responsables.
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
