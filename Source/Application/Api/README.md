# API Organizations

## Authentification OIDC

L'API valide les jetons d'accès (JWT) émis par l'annuaire du domaine (Keycloak, Authentik, Nextcloud OIDC...).
Elle ne gère plus de mots de passe : `Member.HashedPassword` a été supprimé, `Member.ExternalId` mémorise le compte externe.

| Variable | Rôle |
| --- | --- |
| `OIDC_AUTHORITY` | URL de l'émetteur (ex. `https://auth.adanova83.org/realms/adanova`). Vide = désactivé. |
| `OIDC_AUDIENCE` | Audience attendue dans le jeton. Vide = non vérifiée. |

`GET /api/me` (en-tête `Authorization: Bearer <jeton>`) :

1. cherche le membre dont `ExternalId` vaut le claim `sub` ;
2. à la première connexion, rattache le compte au membre qui porte le même e-mail (claim `email`, ignoré si `email_verified` vaut `false`) et qui n'est lié à aucun autre compte ;
3. renvoie le membre, ses rôles et ses permissions, ou `403` si aucun membre ne correspond (les membres doivent être créés avant la première connexion : l'adresse est obligatoire dans le modèle).

Les endpoints d'administration (`/api/migrations`, `/api/model`, `/api/calendar-sync`) restent protégés par la clé d'API (`X-Api-Key`).

## Documentation Swagger

- Interface : `http://localhost:5180/swagger` ; description OpenAPI : `/openapi/v1.json`.
- Active en `Development`, et dans Docker via `API_ENABLE_SWAGGER` (`true` par défaut) ; réglage `Api:EnableSwagger`. **À mettre à `false` en production**, car elle décrit toute l'API.
- Bouton **Authorize** : `Bearer` pour coller un jeton OIDC, ou `ApiKey` pour la clé `X-Api-Key` (acceptée sur les endpoints métier seulement si `Api:AllowKeyOnBusinessEndpoints` est vrai). L'autorisation est mémorisée par le navigateur.

## Rôles et permissions

- **Rôles = groupes de l'annuaire** (Authentik/LDAP). L'appartenance d'un membre à un rôle est son appartenance au groupe ; elle n'est pas stockée dans la base. Les tables `RoleMember`, `Permission`, `PermissionMember` et `RolePermission` ont été supprimées.
- Les groupes arrivent dans le jeton, dans le claim `groups` (réglable : `Oidc:GroupsClaim`). Avec Authentik, ajouter le scope `groups` au provider OIDC de l'API.
- **Permissions = configuration** : correspondance groupe -> permissions, section `Authorization:RolePermissions` de `appsettings.json` (`*` = toutes les permissions) :

```json
"Authorization": {
  "RolePermissions": {
    "bureau": [ "*" ],
    "animateurs": [ "activity.edit", "event.edit" ]
  }
}
```

- Protéger un endpoint : `.RequireAuthorization().RequirePermission("activity.edit")` (401 sans jeton, 403 sans la permission).
- **Couleur et présentation** : la table `Role` est conservée uniquement pour ce que l'annuaire ne stocke pas (libellé, image, `RGBColor`). `Role.Code` doit être égal au nom du groupe (comparaison insensible à la casse). Un groupe sans ligne dans `Role` fonctionne, il n'a simplement pas de couleur.
- `GET /api/me` renvoie le membre, ses groupes (`Groups`), les rôles connus avec leur présentation (`Roles`) et ses permissions (`Permissions`).

## Endpoints métier

Tous les endpoints suivent le même modèle (`Crud/CrudEndpoints.cs` et `Crud/OwnedCrudEndpoints.cs`) : `GET /` (liste paginée : `?page=` et `?pageSize=`, 50 par page par défaut, 200 au maximum), `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}` (suppression logique). À la création, l'`id` et l'`audit` fournis sont ignorés ; `active` (période de validité) est facultatif et conservé à la modification s'il est omis.

| Domaine | Routes | Écriture (la lecture exige `*.read` du domaine) |
| --- | --- | --- |
| Membres | `/api/members` | `member.edit` |
| Contacts des familles | `/api/contacts` (liste : `?query=`), `/api/member-contacts` (liste : `?memberId=`, `?targetId=` = contact) : parent ou tuteur, lien (`Mother`, `Father`, `Guardian`, `Other`), à prévenir en urgence, autorisé à venir chercher ; un contact peut suivre plusieurs membres | `member.edit` |
| Participants | `GET /api/participants/{activity-slot|event|project-slot}/{id}` : inscrits et liste d'attente avec coordonnées, âge et contacts des familles | animateurs de cet élément seulement, ou `subscription.read` + `member.read` |
| Présences (feuilles d'émargement) | `GET /api/attendances/{activity-slot|event}/{id}` : séances datées (rythme, vacances, jours fériés) et décomptes ; `GET` / `PUT .../{id}/{yyyy-MM-dd}` : feuille d'émargement, appel `[{ memberId, status }]` (`Present`, `Absent`, `Excused`) ; `GET /api/attendances/stats?from=&to=` : fréquentation par atelier / événement et par membre | animateurs de la séance (appel), `subscription.read` (lecture, statistiques), `subscription.edit` (correction) |
| Espaces | `/api/spaces` | `space.edit` |
| Années scolaires | `/api/school-years` (+ `/{id}/organization-sessions`, `/{id}/activity-sessions`, `/{id}/events`, `/{id}/projects`, `/{id}/holidays`) | `organization.edit` |
| Pays, organisations, saisons, jours fériés | `/api/countries`, `/api/organizations` (+ `/{id}/sessions`), `/api/organization-sessions`, `/api/holidays` | `organization.edit` (pays : lisibles par tout compte connecté) |
| Ateliers | `/api/activities` (+ `/{id}/sessions`), `/api/activity-sessions` (+ `/{id}/slots`), `/api/activity-session-slots` | `activity.edit` |
| Événements | `/api/events` | `event.edit` |
| Projets | `/api/projects` (+ `/{id}/slots`), `/api/project-slots` | `project.edit` |
| Animateurs | `/api/activity-session-slot-animations`, `/api/event-animations`, `/api/project-slot-animations` (liste : `?targetId=` et `?memberId=`) | `activity.edit`, `event.edit`, `project.edit` |
| Inscriptions | `/api/activity-session-slot-subscriptions`, `/api/event-subscriptions`, `/api/project-slot-subscriptions` | le membre pour lui-même, ou `subscription.edit` |
| Cotisations | `/api/contributions` (liste : `?memberId=`), `amount` = montant payé ; `/api/contributions/seasons/{organizationSessionId}` : situation de la saison | `contribution.edit` (lecture : le membre pour les siennes ; saison : `contribution.read`) |
| Notes personnelles | `/api/notes`, `/api/note-links` (liste : `?targetId=<id de la note>`) | l'auteur seul |
| Contrôle d'accès | `/api/badges`, `/api/badge-assignations`, `/api/access-events` (lecture seule, `?targetId=<id du badge>`) | `access.edit` |

- **Lecture (GET)** : chaque domaine a une permission `*.read` (`activity.read`, `event.read`...) ; la permission d'écriture `*.edit` la donne aussi, inutile d'attribuer les deux. Pour les données qui appartiennent à un membre (inscriptions, cotisations, notes), un membre ordinaire voit toujours les siennes sans permission ; `subscription.read` et `contribution.read` donnent accès à celles de tous.
- **Données d'un membre** : le membre connecté est retrouvé par son compte de l'annuaire (`sub`), puis par e-mail à la première utilisation. Un membre ne peut s'inscrire que lui-même ; pour inscrire quelqu'un d'autre, il faut `subscription.edit`. Règles d'inscription (`SubscriptionGuard`, 409 « Inscription refusée. » avec un message en français) : un membre n'est jamais inscrit deux fois au même élément (inscrit ou en attente) ; âge minimum apprécié au début de l'activité (session, projet ou date de l'événement ; aujourd'hui si elle a commencé), date de naissance obligatoire dans ce cas ; `capacity` (créneaux d'ateliers, événements, créneaux de projets ; vide = illimitée) : quand l'élément est complet, l'inscription est créée en **liste d'attente** (`isWaiting`, `waitingRank` : 1 = le prochain), et à chaque place libérée (désinscription, membre supprimé, capacité augmentée) les premiers de la liste la prennent. Un responsable (`subscription.edit`, ou la clé d'API) peut déroger à la capacité et à l'âge (inscription directe), pas aux doublons. Places sans les noms : `GET /api/{activity-session-slot|event|project-slot}-subscriptions/availability?ids=…` (`capacity`, `subscribed`, `waiting`, `remaining`, `isFull`), avec la permission de lecture du domaine.
- **Ateliers** : `activity.edit` s'applique à tous les ateliers, sans règle de propriété.
- **Membres** : `ExternalId` (lien avec le compte de l'annuaire) n'est jamais modifiable par ce biais : il est posé à la première connexion, par rapprochement d'e-mail. Créer un membre avec l'e-mail du compte Authentik suffit donc à l'autoriser.
- **Son propre profil** : `PUT /api/me` (prénom, nom, photo, couleur, naissance, téléphone, adresse), sans permission ; l'e-mail et le lien avec l'annuaire ne changent pas.
- **Obtenir une permission** : soit par la correspondance groupe -> permissions (`Authorization:RolePermissions`), soit en appartenant à un groupe de l'annuaire portant exactement le nom de la permission (groupe `activity.edit`, `event.edit`...). Permissions existantes, chacune en `.read` et `.edit` : `member`, `space`, `activity`, `organization`, `event`, `project`, `subscription`, `contribution`, `access`.
- **Exemple** : donner la lecture du planning à tous les membres, et l'écriture des ateliers aux animateurs (`Authorization:RolePermissions`, groupes de l'annuaire) :

```json
"Authorization": {
  "RolePermissions": {
    "membres": [ "space.read", "activity.read", "event.read", "project.read", "organization.read" ],
    "animateurs": [ "activity.edit", "event.edit" ]
  }
}
```

  Sans cela, un membre sans permission reçoit 403 sur ces routes (le planning reste lisible par le flux ICS, protégé par son propre jeton).
- **Clé d'API** : l'en-tête `X-Api-Key` donne tous les droits, mais seulement si `Api:AllowKeyOnBusinessEndpoints` vaut `true` (développement, et par défaut dans le `docker-compose.yml` via `API_ALLOW_KEY_ON_BUSINESS`). **À mettre à `false` en production.** La clé doit être configurée (`API_KEY`).
- **Erreurs** : règle de validation non respectée = 400 ; clé étrangère inexistante, élément encore utilisé ou valeur unique en double = 409 ; élément absent (ou appartenant à un autre membre) = 404 ; droits insuffisants = 403 ; non authentifié = 401.
- **Années scolaires** : une année (`code` « 2026-2027 », rentrée, fin des cours) est commune à toutes les organisations. La saison d'une organisation (`organization-sessions` : année + cotisation), les sessions d'activité, les événements et les projets portent un `schoolYearId` et prennent les dates de l'année. Une session d'activité ou un projet peut restreindre sa période (`startDate` / `endDate` facultatives) ; l'API renvoie en lecture seule `schoolYearCode` et les dates effectives (`effectiveStartDate`, `effectiveEndDate`), utilisées aussi par le flux ICS. Un événement porte son organisation (`organizationId`) et son année. Une année encore utilisée ne peut pas être supprimée (409, voir « Suppression logique »). Les sessions, événements et projets d'une année demandent en plus la permission de lecture de leur domaine. Migration `SchoolYear` : les années sont déduites des saisons et des projets existants (août à juillet, heure de Paris).
- **Vacances scolaires** : chaque année scolaire a une zone (`zone` : A, B ou C ; B pour le Var). À sa création, l'API importe les vacances officielles de la zone depuis le calendrier de l'Éducation nationale (data.education.gouv.fr, jeu `fr-en-calendrier-scolaire`) et les rattache à l'année ; un échec (calendrier pas encore publié, service indisponible) n'empêche pas la création. `POST /api/school-years/{id}/import-holidays` relance l'import (les périodes déjà présentes sont ignorées ; 502 si le calendrier est injoignable). Dates : du premier jour de vacances à la veille de la reprise ; seules les dates « élèves » sont retenues. Réglage : `SchoolHolidays:DatasetUrl`.
- **Rythme des sessions d'activité** : `duringTermTime` (hors vacances scolaires, vrai par défaut) et `duringSchoolHolidays` (pendant les vacances scolaires) ; au moins l'un des deux. Les vacances scolaires sont les `Holiday` rattachées à une année (`schoolYearId`), les jours fériés celles sans année : aucun créneau n'a lieu un jour férié. Le flux ICS exclut les dates concernées (`EXDATE`).
- **Séries d'événements** : `POST /api/events/series` (`event.edit`) avec `event` (champs communs, dont `schoolYearId` ; ses dates sont ignorées), `frequency` (`Daily`, `Weekly`, `MonthlyByDate`, `MonthlyByWeekday`), `interval` (tous les N jours, semaines ou mois), selon la fréquence `dayOfWeek` (0 = dimanche), `dayOfMonth` (1 à 31, dernier jour pour les mois courts) ou `weekOfMonth` (1 à 4, -1 = dernier) + `dayOfWeek`, et `startTime` / `endTime` (heure de Paris ; fin avant début = lendemain). Rythme : `duringTermTime` (vrai par défaut) et `duringSchoolHolidays`, comme les sessions d'activité ; aucune date un jour férié. Période : celle de l'année scolaire, à partir d'aujourd'hui si elle a commencé (`from` / `until` facultatifs pour la restreindre). Un événement par date, codes suffixés par la date (`JEUX-20261106`), même `seriesId`. 200 dates et 2 ans au plus. `GET /api/events/series/{id}` liste les occurrences ; `PUT` (nom, description, âge, organisation, année, lieu, heure de début, durée) et `DELETE` s'appliquent à toute la série ou, avec `?from=yyyy-MM-dd`, à partir d'une date. Une occurrence seule se modifie ou se supprime par `/api/events/{id}`.
- **Jours fériés automatiques** : `POST /api/holidays/generate?year=2027` (`organization.edit`) crée les 11 jours fériés légaux de l'année (fêtes mobiles calculées à partir de Pâques, `Library.Domain.FrenchPublicHolidays`) ; `&alsaceMoselle=true` ajoute le Vendredi saint et la Saint-Étienne. Un jour déjà présent à la même date n'est pas recréé : l'appel peut être relancé. Les vacances scolaires, fixées par arrêté, restent à saisir.
- **Jours des créneaux** : `dayOfWeek` de 0 (dimanche) à 6 (samedi).
- **Fréquence des créneaux** (`frequency`, activités) : intervalle en semaines entre deux occurrences, 1 par défaut (chaque semaine), 2 pour une semaine sur deux. C'est une hypothèse sur le sens de ce champ ; elle est reprise dans le flux ICS (`INTERVAL`).
- **Cotisations d'une saison** : `GET /api/contributions/seasons/{organizationSessionId}` (`contribution.read`, noms des membres inclus) donne pour chaque membre actuel le montant dû (cotisation de la saison), payé (somme de ses paiements : paiement en plusieurs fois possible), le reste, la date du dernier paiement, l'état (`Unpaid`, `Partial`, `Paid`) et s'il **participe** (inscrit ou en attente à un atelier, un événement ou un projet de l'organisation cette année-là) ; totaux : participants, participants à jour / non à jour, encaissé, attendu des participants. Migration `ContributionAmount` : les cotisations déjà enregistrées prennent le tarif de leur saison.
- **Suppression logique** (`Audit.DeletedAt`), avec une politique centrale (`Library/Data/FullDbContextDeletion.cs`) appliquée à chaque enregistrement. **Propagée** aux éléments qui n'ont pas de sens seuls : activité → sessions → créneaux → inscriptions et animateurs ; événement → inscriptions et animateurs ; projet → créneaux → inscriptions et animateurs ; note → liens ; badge → attributions (l'historique des passages est conservé) ; membre → inscriptions, animations, notes et badges (ses cotisations sont conservées, historique comptable). **Refusée** (409 « Suppression impossible. » avec un message en français) pour un élément de référence encore utilisé : année scolaire, organisation (saisons, activités, événements, projets), saison avec des cotisations, salle (créneaux, événements), pays (adresses des membres). Les places libérées (membre supprimé) et une capacité augmentée (créneau, événement, créneau de projet) font passer les premiers de la liste d'attente. Les valeurs uniques (codes, e-mail des membres, codes pays, identifiant de badge) ne sont vérifiées que parmi les éléments non supprimés : un code libéré par une suppression peut être réutilisé.
- **Non couverts** : les applications du contrôle d'accès (`Application`, qui porte une clé d'API de lecteur) n'ont pas encore d'endpoints ; ils viendront avec l'authentification des lecteurs de badges, la clé devant alors être stockée hachée.

## Premier administrateur

- Variables du `.env` : `ADMIN_EMAIL`, `ADMIN_FIRSTNAME`, `ADMIN_LASTNAME`, `ADMIN_GROUP` (par défaut `authentik Admins`).
- Au démarrage (après les migrations), si aucun membre ne porte `ADMIN_EMAIL`, l'API le crée, ainsi que le pays France dont son adresse a besoin. L'adresse est provisoire (« À renseigner ») : la corriger avec `PUT /api/me`. Un membre existant n'est jamais modifié.
- À sa première connexion, ce membre est rattaché à son compte de l'annuaire par son e-mail (l'e-mail du compte Authentik doit être vérifié et identique).
- Les membres du groupe `ADMIN_GROUP` de l'annuaire ont **toutes** les permissions. Le compte initial d'Authentik appartient déjà au groupe `authentik Admins`.
- **Mot de passe** : `ADMIN_PASSWORD` figure dans le `.env` pour le compose global, qui l'utilisera pour créer le compte initial d'Authentik (`AUTHENTIK_BOOTSTRAP_EMAIL` et `AUTHENTIK_BOOTSTRAP_PASSWORD`). L'API ne le lit jamais et ne le reçoit pas.

## Planning : flux ICS pour Nextcloud

Principe : ce qui existe dans Nextcloud (comptes, agendas personnels, fichiers, messagerie) n'est **pas** dupliqué ici. Seul le planning de l'organisation (événements, créneaux d'activités et de projets, jours fériés) est une donnée de cette application ; elle le publie en lecture seule sous forme de flux iCalendar auxquels Nextcloud s'abonne. Aucun compte de service ni mot de passe d'application n'est nécessaire.

| Flux | Contenu |
| --- | --- |
| `events` | `Event` |
| `activities` | créneaux hebdomadaires des sessions d'activités |
| `projects` | créneaux hebdomadaires des projets |
| `holidays` | `Holiday` (journées entières) |
| `all` | l'ensemble |

Adresse : `GET /api/calendar/{flux}.ics?token=<CALENDAR_FEED_TOKEN>`

Mise en place :

1. Définir `CALENDAR_FEED_TOKEN` (secret long, `openssl rand -hex 32`). Vide = flux désactivés (404).
2. Dans Nextcloud, application Agenda : « Nouvel abonnement », coller l'adresse complète (par exemple `https://api.adanova83.org/api/calendar/all.ics?token=...`).
3. Nextcloud réactualise l'abonnement environ toutes les heures (`REFRESH-INTERVAL`).

Points d'attention :

- Le jeton est dans l'adresse : à traiter comme un mot de passe (Nextcloud ne sait pas envoyer d'en-tête pour un abonnement). Il peut apparaître dans les journaux du reverse proxy ; le changer invalide les abonnements existants.
- Les créneaux sont publiés en heure de Paris (heure d'été comprise), les événements ponctuels en UTC. Les participants (inscriptions) ne sont pas publiés.
- Réglages dans `appsettings.json`, section `Calendar` : `UidDomain`, `DayOfWeekStartsOnMonday`.

Hypothèses à vérifier :

- **Numéro de jour des créneaux** (`DayOfWeek`, 0 à 6) : convention retenue, comme `System.DayOfWeek` (0 = dimanche, 1 = lundi… 6 = samedi). C'est le comportement par défaut ; `Calendar:DayOfWeekStartsOnMonday` ne sert que si cette convention change un jour.
- **`ActivitySessionSlot.Frequency`** existe en base mais n'est pas exposé par le domaine : tous les créneaux sont publiés chaque semaine.
- **Projets** : les dates ont été ajoutées au domaine ; les projets existants dont les dates valent `0001-01-01` sont ignorés (avertissement dans les journaux).

## Ce qui n'est plus géré ici

Pour éviter une double source de vérité, les fonctions suivantes relèvent uniquement de l'annuaire / de Nextcloud : mots de passe et comptes (OIDC), documents partagés (Nextcloud Fichiers), messagerie (Nextcloud Talk). Les tables `Document` et `Chat*` ont été supprimées par la migration `NextcloudIntegration`.
