# Cahier des charges — Gestion d'une collection de mangas

## 1. Présentation du projet

L'application est un logiciel de bureau Windows destiné à gérer une collection personnelle de mangas japonais. Elle s'appuie sur WPF et suit le pattern MVC. L'utilisateur doit pouvoir consulter et maintenir sa collection, rechercher des mangas dans un catalogue public et exporter ses données.

## 2. Objectifs

- Centraliser les informations des mangas possédés par l'utilisateur.
- Permettre une consultation simple de la collection, sous forme de liste ou de jaquettes.
- Faciliter l'ajout de mangas, notamment à partir de résultats de recherche en ligne.
- Permettre la modification et la suppression des entrées.
- Fournir un export exploitable de la collection au format CSV.

## 3. Utilisateur cible

L'application s'adresse à un particulier souhaitant inventorier sa collection de mangas. Elle est conçue pour un utilisateur à la fois et ne nécessite ni compte ni service distant pour consulter et gérer sa collection existante.

## 4. Périmètre fonctionnel

### 4.1 Consultation de la collection

- À l'ouverture, l'application affiche la collection enregistrée.
- L'utilisateur peut basculer entre une vue en liste et une vue en grille de jaquettes.
- Une entrée présente au minimum le titre et le nombre de volumes possédés. La vue en liste peut afficher les autres informations disponibles.
- Une collection vide est accompagnée d'un état explicite et d'une invitation à ajouter un manga.
- Les mangas restent consultables si la recherche en ligne est indisponible.

### 4.2 Gestion des mangas

L'utilisateur peut ajouter, modifier et supprimer une entrée de sa collection.

Les informations gérées pour une entrée sont :

- le titre (obligatoire) ;
- l'auteur, si connu ;
- le nombre de volumes possédés (entier supérieur ou égal à zéro) ;
- le nombre total de volumes publiés ou prévus, si connu ;
- le genre ou les genres, si connus ;
- le statut de publication (en cours, terminé ou inconnu), si connu ;
- l'URL de la jaquette, si disponible ;
- une note personnelle facultative.

Lors d'un ajout depuis les résultats de recherche, les informations fournies par le catalogue sont préremplies ; l'utilisateur peut les compléter ou les corriger avant l'enregistrement. Les champs non fournis par le catalogue restent modifiables. Une confirmation est demandée avant une suppression définitive.

### 4.3 Recherche dans un catalogue public

- L'utilisateur peut rechercher des mangas par titre.
- L'application interroge une API publique de mangas (par exemple Jikan, API publique de MyAnimeList) et affiche les résultats disponibles, dont le titre et, lorsque fournis, l'auteur, le nombre de volumes, le statut et la jaquette.
- L'utilisateur peut sélectionner un résultat pour préremplir une nouvelle entrée dans sa collection.
- Une recherche sans résultat est signalée clairement.
- En cas d'absence de connexion, d'indisponibilité de l'API ou d'erreur de requête, l'application informe l'utilisateur et lui permet de réessayer ; les données locales restent accessibles.
- Les données renvoyées par l'API sont considérées comme des suggestions et peuvent être incomplètes ou corrigées par l'utilisateur.

### 4.4 Export CSV

- L'utilisateur peut exporter sa collection dans un fichier CSV via une boîte de dialogue de sélection de destination.
- Le fichier contient une ligne d'en-tête et une ligne par manga, avec au minimum le titre, l'auteur, le nombre de volumes possédés et le nombre total de volumes lorsqu'il est connu. Les autres champs enregistrés peuvent également être exportés.
- Les champs contenant des séparateurs, des guillemets ou des retours à la ligne sont échappés conformément au format CSV.
- Le fichier est encodé en UTF-8 afin de préserver les caractères accentués et japonais.
- L'application indique si l'export a réussi ou si une erreur empêche sa création.

## 5. Parcours principaux

### Ajouter un manga

1. L'utilisateur ouvre l'action d'ajout et saisit un titre à rechercher.
2. Il sélectionne un résultat du catalogue ou choisit de saisir les informations manuellement.
3. Il vérifie et complète les informations, puis enregistre l'entrée.
4. Le manga apparaît dans la collection.

### Modifier ou supprimer un manga

1. L'utilisateur sélectionne une entrée dans la collection.
2. Il modifie les champs souhaités et enregistre, ou demande la suppression.
3. En cas de suppression, il confirme son choix ; l'entrée est ensuite retirée de la collection.

### Exporter la collection

1. L'utilisateur déclenche l'export CSV.
2. Il choisit le nom et l'emplacement du fichier.
3. L'application crée le fichier et affiche le résultat de l'opération.

## 6. Exigences non fonctionnelles

- L'interface doit rester compréhensible et utilisable avec une collection vide comme avec une collection contenant de nombreuses entrées.
- Les opérations de consultation et de gestion locales ne doivent pas dépendre de l'accès à Internet.
- Les erreurs de saisie, d'accès réseau et d'écriture de fichier doivent être communiquées sans fermer inopinément l'application.
- Les informations personnelles de collection sont conservées localement ; aucune authentification ou synchronisation distante n'est requise.
- L'interface doit permettre de distinguer les données absentes des valeurs numériques égales à zéro.

## 7. Critères d'acceptation

- L'utilisateur peut afficher sa collection sous forme de liste et de jaquettes.
- L'utilisateur peut créer, modifier et supprimer une entrée, avec validation du titre et du nombre de volumes possédés.
- L'utilisateur peut rechercher un manga via une API publique et utiliser un résultat pour préremplir une entrée.
- Une erreur ou une absence de réseau n'empêche pas de consulter ni de gérer les données locales.
- L'utilisateur peut exporter la collection vers un CSV UTF-8 lisible, avec en-têtes et données correctement échappées.

## 8. Hors périmètre

- La création de comptes, le partage de collections et leur synchronisation en ligne.
- La gestion de plusieurs utilisateurs ou de plusieurs collections distinctes.
- L'achat de mangas, le suivi de prix ou la gestion de prêts.
- La garantie d'exhaustivité ou d'exactitude des informations provenant de services tiers.
