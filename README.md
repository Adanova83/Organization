# Organizations

Application de gestion d'une association (Adanova 83) : membres, cotisations, ateliers, événements, projets,
inscriptions, animateurs, salles, années scolaires, vacances et contrôle d'accès par badge.

Ce dépôt contient une **API** (ASP.NET Core, .NET 10), un **site web** (Blazor, rendu côté serveur) et la
**bibliothèque** commune (Entity Framework Core, PostgreSQL). Les comptes, documents partagés et messageries ne sont
pas gérés ici : ils relèvent de l'annuaire (Authentik, OIDC) et de Nextcloud.

## Fonctionnalités

- **Membres** : fiches, adresse, cotisations par saison, profil modifiable par le membre lui-même.
- **Années scolaires** (ex. 2026-2027) communes à toutes les organisations, avec leur zone de vacances (A, B ou C) ;
  les vacances scolaires officielles sont importées automatiquement à la création de l'année.
- **Ateliers** : sessions par année scolaire (période restreinte possible), créneaux hebdomadaires, rythme
  « hors vacances scolaires » et/ou « pendant les vacances », jamais les jours fériés.
- **Événements** uniques ou **récurrents** (chaque jour, chaque semaine, chaque mois à la même date ou le même jour,
  par exemple le dernier vendredi), chaque date restant modifiable et ouverte aux inscriptions séparément.
- **Projets**, **inscriptions**, **animateurs**, **notes personnelles**, **salles**, **jours fériés** (calcul
  automatique des jours fériés légaux) et **badges** (attributions, passages).
- **Calendrier** hebdomadaire des ateliers et événements, hachuré aux couleurs de l'activité et de la salle.
- **Flux ICS** du planning, auxquels l'Agenda Nextcloud peut s'abonner.

## Architecture

```
Source/
├── Library/              Bibliothèque commune : modèle, règles métier, accès aux données
│   ├── <Domaine>/<Entité>/Contract    Modèle échangé avec l'API (partagé avec le site)
│   ├──                     Domain      Modèle métier et règles
│   ├──                     Entity      Modèle Entity Framework
│   ├──                     Mapper      Conversions entre les modèles
│   ├──                     Handler     Contrôles métier
│   ├──                     Repository  Requêtes
│   ├── Data/             DbContext (audit, suppression logique)
│   └── Migrations/       Migrations Entity Framework
└── Application/
    ├── Api/              API minimale : endpoints, OIDC, permissions, flux ICS (voir son README)
    └── Web/              Site web Blazor : appelle uniquement l'API
```

- Le navigateur ne connaît qu'une adresse, celle du site : le site transmet `/api/*` à l'API (YARP). L'API mène la
  connexion OIDC avec l'annuaire et remet un cookie de session ; le site ne voit jamais de jeton.
- La suppression est **logique** : une ligne supprimée est conservée et masquée (`Audit.DeletedAt`).

## Droits

Les **rôles** sont les groupes de l'annuaire ; les **permissions** sont associées aux groupes dans la configuration
de l'API (`Authorization:RolePermissions`). Pour chaque domaine (`member`, `space`, `activity`, `organization`,
`event`, `project`, `subscription`, `contribution`, `access`) :

- `*.read` permet l'**affichage** (listes et fiches) ;
- `*.edit` permet la **modification** (formulaires) et donne aussi la lecture.

Les membres du groupe administrateur (`ADMIN_GROUP`, par défaut `authentik Admins`) ont toutes les permissions.
Un membre voit et modifie toujours ses propres inscriptions, notes et son profil.

## Démarrage en développement

Prérequis : Docker Desktop, et le SDK .NET 10 pour compiler en dehors de Docker.

1. Copier `.env.example` en `.env` et renseigner au moins `DB_PASSWORD`, `API_KEY` et `ADMIN_EMAIL`.
   Pour le faux serveur d'identité, `ADMIN_EMAIL` doit être l'adresse du compte de test définie dans
   `dev/oidc-config.json`. Mettre `MIGRATIONS_APPLY_AT_STARTUP=true` pour créer les tables au démarrage.
2. Lancer l'ensemble avec le faux serveur d'identité (mock-oauth2-server, à la place d'Authentik) :

   ```bash
   docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d --build
   ```

3. Ouvrir le site sur <http://localhost:5200> et se connecter (compte de test, membre du groupe administrateur).
   L'API répond sur <http://localhost:5180> ; sa documentation Swagger est sur <http://localhost:5180/swagger>.

En développement, la base peut être recréée à chaque changement du modèle :

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml down -v
```

Ajouter une migration après une modification du modèle (aucune connexion à la base n'est nécessaire) :

```bash
cd Source/Library
ADANOVA_CONNECTION_STRING="Host=localhost;Database=dummy;Username=dummy;Password=dummy" dotnet ef migrations add NomDeLaMigration
```

## Production

- Renseigner l'annuaire OIDC (`OIDC_AUTHORITY`, `OIDC_CLIENT_ID`, `OIDC_CLIENT_SECRET`) et ne pas utiliser
  `docker-compose.dev.yml`.
- Mettre `API_ALLOW_KEY_ON_BUSINESS=false` et `API_ENABLE_SWAGGER=false`.
- Définir `CALENDAR_FEED_TOKEN` (secret long) pour activer les flux ICS.
- Placer le site derrière un reverse proxy (Traefik) qui gère le HTTPS.

Le détail des endpoints, des permissions, des flux ICS et des réglages est dans
[Source/Application/Api/README.md](Source/Application/Api/README.md). Les travaux restant à faire sont listés dans
[TASK.md](TASK.md).

## Licence

Ce logiciel est distribué sous la licence libre **CeCILL-B** (licence de droit français, proche des licences BSD) :
voir le fichier [LICENCE](LICENCE) ou <https://cecill.info>.
