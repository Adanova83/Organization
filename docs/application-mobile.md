# Application mobile — cahier des charges

Version de travail du 18/10/2026, à relire avec le bureau. Les décisions déjà prises sont indiquées ; les points
encore ouverts sont regroupés à la fin.

## 1. Objectif et principes

Une application sur le téléphone des membres et des animateurs pour :

- consulter ses ateliers et événements (comme participant ou comme animateur) ;
- **valider les présences** (feuille d'émargement) par QR code ou par appel ;
- recevoir des rappels et être prévenu des changements ;
- retrouver son planning dans son agenda habituel.

Principes retenus :

| Décision | Conséquence |
|---|---|
| **Autonome et gratuite** : ni Firebase ni autre service tiers, aucun abonnement | Tout est servi par le serveur de l'association |
| **Application web installable** (PWA) plutôt qu'une application des stores | Gratuite, sans App Store ni Play Store, sur Android **et** iPhone ; installée depuis le site (« Ajouter à l'écran d'accueil ») |
| Même serveur, même connexion que l'intranet | Connexion par Authentik (compte de l'intranet), mêmes droits |
| Code en C# (Blazor exécuté dans le navigateur) | Réutilise les contrats de l'API et les compétences du projet |

Seul relais externe inévitable : pour qu'une notification arrive téléphone en veille, le navigateur passe par le service
de son éditeur (Google, Apple, Mozilla) selon le standard ouvert Web Push. Gratuit, sans compte ni bibliothèque
tierce ; le contenu est chiffré de bout en bout par notre serveur.

## 2. Utilisateurs

| Profil | Qui | Dans l'application |
|---|---|---|
| Membre | tout compte (groupe `membres`) | ses inscriptions, pointage par QR code, absences, rappels, agenda, cotisation |
| Animateur | groupe `animateurs` | en plus : ce qu'il anime, participants et contacts des familles, QR code de la séance, appel, messages |
| Responsable | groupe `responsables` | les présences et statistiques **sur le site** (pas d'écran dédié dans l'application) |

Les enfants n'ont pas de compte : ils ne pointent pas eux-mêmes. **L'animateur fait l'appel** pour eux (présent /
absent, sans gestion des retards). Les contacts des familles ne sont pas des utilisateurs, mais peuvent recevoir les
messages par e-mail (voir 3.3).

## 3. Fonctionnalités de la première version

Toutes les fonctionnalités ci-dessous font partie de la première version ; le découpage en lots (§ 9) fixe seulement
l'ordre de développement.

### 3.1 Membre

1. **Se connecter** avec son compte (Authentik) ; la session reste ouverte sur le téléphone.
2. **Mes ateliers et événements** : prochaines séances, lieu, horaires, statut (inscrit, liste d'attente et rang).
3. **S'inscrire / se désinscrire** depuis l'application, avec les mêmes règles que le site (âge, capacité, liste
   d'attente).
4. **Valider sa présence** en scannant le QR code présenté par l'animateur.
5. **Prévenir d'une absence** pour une séance à venir (motif facultatif) : l'animateur le voit dans sa liste d'appel.
6. **Rappels** : à 9 h 00 le jour de la séance, et 15 minutes avant.
7. **Être prévenu** d'une séance annulée ou modifiée (horaire, salle), et quand une place se libère sur une liste
   d'attente (passage automatiquement d'« en attente » à « inscrit »).
8. **Ma cotisation** : à jour, ou reste à payer pour la saison.
9. **S'abonner à son agenda personnel** (voir § 6).

### 3.2 Animateur

En plus des fonctionnalités du membre :

1. **Ce que j'anime** : prochaines séances des créneaux d'ateliers dont il est animateur (les événements n'ont pas
   d'animateurs).
2. **Participants** d'une séance : inscrits, liste d'attente, âge, absences signalées, et **contacts des familles** en un
   geste (appeler le contact à prévenir en urgence, voir qui est autorisé à venir chercher l'enfant).
3. **QR code de la séance** à afficher sur son téléphone (voir § 5) ; les présences validées s'affichent en direct.
4. **Appel manuel** : cocher présent / absent pour chaque participant (indispensable pour les enfants) ; corrige aussi
   un pointage par QR code.
5. **Hors connexion** : le planning, les participants et les contacts sont gardés sur le téléphone ; l'appel est
   enregistré sur place puis envoyé dès que le réseau revient.
6. **Message aux participants** d'une séance (par exemple une annulation de dernière minute) : notification aux
   membres qui ont l'application, et e-mail aux membres et aux contacts des familles (serveur mail de l'association).

### 3.3 Responsable (sur le site)

1. **Feuille d'émargement** par séance : liste des inscrits avec présent / absent / absence signalée, mode de pointage
   (QR code ou appel), animateur ; export **PDF** (pour les financeurs : CAF, subventions).
2. **Statistiques de fréquentation** : taux de présence par atelier, par séance et par membre, sur une période.
3. Correction d'une présence après coup (avec trace de qui l'a modifiée).

## 4. Écrans de l'application

| Écran | Contenu |
|---|---|
| Connexion | redirection vers Authentik, puis retour dans l'application |
| Accueil | prochaines séances (participant et animateur), alertes (annulation, place obtenue), cotisation |
| Séance (participant) | détails, « Je serai absent », « Scanner le QR code » le jour même |
| Scanner | appareil photo, confirmation « Présence validée » |
| Séance (animateur) | participants et absences signalées, contacts des familles, boutons « QR code », « Appel », « Message » |
| QR code | code plein écran qui change toutes les 30 secondes, compteur des présents |
| Appel | liste à cocher présent / absent, envoi immédiat ou différé hors connexion |
| Inscriptions | ateliers et événements ouverts, s'inscrire / se désinscrire |
| Réglages | notifications (activer, rappels de 9 h et de 15 min), adresse de l'agenda personnel, déconnexion |

## 5. Présences et QR code

### 5.1 Ce qu'on enregistre

Une présence par **séance datée** : un créneau hebdomadaire se répète, la présence porte donc sur
« créneau + date ». **Pas d'appel ni de QR code pour les événements** (décision du 18/10/2026) : les présences ne
concernent que les ateliers.

| Donnée | Valeurs |
|---|---|
| Séance | créneau d'atelier + date |
| Membre | le participant |
| Statut | présent, absent, absence signalée |
| Mode | QR code, appel, signalement du membre, correction d'un responsable |
| Qui et quand | auteur et date de l'enregistrement (audit habituel, suppression logique) |

Une seule présence par membre et par séance ; un nouveau pointage remplace le précédent (l'appel de l'animateur prime
sur le reste).

### 5.2 Sécurité du QR code

Le QR code ne doit pas pouvoir être photographié et envoyé à quelqu'un resté chez lui :

1. Au moment d'ouvrir la séance (en ligne), l'application de l'animateur reçoit du serveur une **clé secrète propre à
   cette séance**.
2. Le QR code affiché contient la séance et un code calculé à partir de cette clé et de l'heure, **renouvelé toutes les
   30 secondes** (même principe que les codes de double authentification). Le téléphone de l'animateur n'a pas besoin
   de réseau pour l'afficher.
3. Le membre scanne : son application envoie le code au serveur, qui vérifie qu'il est valide **maintenant** (à 30
   secondes près), que le membre est **inscrit** à cette séance, et qu'on est dans la **plage de pointage** (par défaut
   de 15 minutes avant le début à la fin de la séance).
4. Le membre doit être connecté au réseau au moment du scan ; sinon l'application le lui indique et l'animateur peut le
   cocher à l'appel.

Seuls les animateurs de la séance (et les responsables) peuvent obtenir la clé d'une séance.

## 6. Agenda personnel

- Chaque utilisateur a une **adresse d'abonnement personnelle** (flux ICS avec un jeton secret, renouvelable s'il
  fuite), affichée dans les réglages de l'application et sur le site.
- Le flux ne contient que **ses** séances : ateliers et événements où il est inscrit, et ceux qu'il anime (marqués
  comme tels), avec les règles de rythme existantes (vacances, jours fériés).
- Compatible Agenda Nextcloud, Google Agenda, Thunderbird, iPhone.
- Les rappels ne passent **pas** par l'agenda (Google les ignore dans un agenda abonné, l'iPhone les supprime par
  défaut) : ils sont envoyés par l'application (§ 7).

Aujourd'hui, le flux ICS de l'intranet utilise une seule clé pour tout le planning ; le flux personnel est à ajouter.

## 7. Notifications

| Notification | Quand | Destinataires |
|---|---|---|
| Rappel du jour | 9 h 00 le jour de la séance | participants et animateurs |
| Rappel avant la séance | 15 minutes avant | participants et animateurs |
| Séance annulée ou modifiée | aussitôt | participants et animateurs |
| Place obtenue (liste d'attente) | aussitôt | le membre concerné |
| Message de l'animateur | aussitôt | participants (et e-mail, § 3.2) |

Fonctionnement :

- Standard **Web Push** : notre serveur signe et envoie lui-même les messages (clés VAPID générées et gardées sur le
  serveur), sans Firebase ni compte chez un tiers.
- L'utilisateur active les notifications dans l'application (son accord est demandé par le téléphone) et peut choisir
  les rappels voulus.
- Une tâche planifiée de l'API calcule les séances du jour et envoie les rappels à l'heure.
- **iPhone** : notifications seulement si l'application a été **ajoutée à l'écran d'accueil** (iOS 16.4 ou plus
  récent). L'écran d'installation l'explique.

## 8. Technique

| Sujet | Choix |
|---|---|
| Type d'application | PWA : manifeste d'application, service worker (hors connexion, notifications) |
| Interface | Blazor WebAssembly (C#), servi par le site sous `https://intranet.adanova83.org/app` |
| Connexion | session de l'intranet (cookie sécurisé du même site), durée allongée pour le téléphone ; l'API ne change pas de modèle de sécurité |
| Scan | appareil photo du navigateur, décodage du QR code dans l'application |
| Hors connexion | planning, participants et contacts en cache chiffré par le navigateur ; file d'attente de l'appel |
| Notifications | Web Push (VAPID) envoyé par l'API ; bibliothèque libre côté serveur |
| E-mails | serveur mail de l'association (étape 04 de l'infrastructure) |
| Installation | depuis le site ; pas de store |

Nouveaux éléments côté API (indicatif) :

- présences : enregistrement, lecture par séance, correction, export PDF, statistiques ;
- séances datées : liste des prochaines séances d'un membre ou d'un animateur ;
- clé de QR code d'une séance, validation d'un scan ;
- absences signalées ;
- abonnements Web Push et préférences de notification ;
- jeton personnel du flux ICS ;
- messages aux participants.

## 9. Découpage en lots

| Lot | Contenu | Utile dès |
|---|---|---|
| 1 | Présences : modèle, appel manuel **sur le site**, feuille d'émargement PDF, statistiques | les animateurs peuvent faire l'appel sur le site du téléphone |
| 2 | Application installable : connexion, accueil, mes séances, inscriptions, cotisation, participants et contacts, appel, hors connexion | l'application remplace le site sur le téléphone |
| 3 | QR code : clé de séance, affichage tournant, scan et validation | pointage autonome des membres |
| 4 | Agenda personnel et notifications (rappels, changements, place obtenue) | |
| 5 | Absences signalées et messages aux participants (notification et e-mail) | |

Les lots 4 et 5 ont besoin du serveur en ligne (DNS) et du serveur mail.

## 10. Données personnelles (RGPD)

- Les présences de mineurs sont des données personnelles : visibles par les animateurs de la séance et les
  responsables uniquement.
- Les contacts des familles ne sont montrés qu'aux animateurs des séances où l'enfant est inscrit.
- Rien n'est collecté en dehors de ce qui est décrit ici (ni géolocalisation, ni statistiques d'usage).
- Durée de conservation des présences : à fixer (voir § 11).

## 11. Points ouverts

1. **Durée de conservation** des présences (par exemple 3 ans pour les justificatifs des financeurs, puis
   anonymisation des statistiques) ?
2. **Plage de pointage** par QR code : 15 minutes avant le début jusqu'à la fin de la séance, ou autre ?
3. **Feuille d'émargement PDF** : quelles informations les financeurs demandent-ils (logo, signature de l'animateur,
   numéro d'agrément...) ?
4. **Absence signalée** : jusqu'à quand avant la séance ? Le motif est-il visible des responsables seulement, ou aussi
   de l'animateur ?
5. **Messages aux participants** : les e-mails aux contacts des familles sont-ils envoyés systématiquement, ou au choix
   de l'animateur ?
6. **Rappels** : activés par défaut une fois les notifications autorisées, ou au choix de chacun ?
