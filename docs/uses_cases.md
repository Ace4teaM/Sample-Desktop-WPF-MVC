# Cas d’utilisation

Ce document décrit les interactions les plus fréquentes entre l’utilisateur et l’application de gestion de collection de mangas. L’application gère une collection personnelle locale, sans compte utilisateur. Le catalogue public et le sélecteur de fichiers sont des services externes utilisés dans les cas concernés.

## Acteurs

- **Utilisateur** : propriétaire de la collection et seul utilisateur de l’application.
- **Catalogue public** : service interrogé pour rechercher des mangas et suggérer des informations.
- **Sélecteur de fichiers** : boîte de dialogue permettant de choisir la destination d’un export CSV.

## UC-01 — Consulter la collection

**Acteur principal :** Utilisateur  
**Déclencheur :** L’utilisateur ouvre l’application ou choisit d’afficher sa collection.

### Scénario nominal

1. L’application charge et affiche les mangas enregistrés.
2. L’utilisateur choisit une présentation en liste ou en grille de jaquettes.
3. L’application affiche au minimum le titre et le nombre de volumes possédés pour chaque manga.
4. En vue liste, l’application peut aussi afficher les autres informations disponibles.

### Variante — Collection vide

1. L’application indique explicitement que la collection est vide.
2. Elle propose à l’utilisateur d’ajouter un manga.

### Variante — Catalogue inaccessible

La consultation des données locales reste disponible, que le catalogue soit accessible ou non.

**Résultat :** L’utilisateur consulte sa collection dans la présentation choisie.

## UC-02 — Rechercher un manga dans le catalogue

**Acteur principal :** Utilisateur  
**Acteur secondaire :** Catalogue public  
**Déclencheur :** L’utilisateur souhaite trouver un manga à ajouter.

### Scénario nominal

1. L’utilisateur ouvre la recherche et saisit un titre.
2. L’application interroge le catalogue public.
3. Le catalogue renvoie les résultats disponibles.
4. L’application affiche les titres et, lorsque ces informations sont fournies, les auteurs, le nombre de volumes, le statut de publication et les jaquettes.
5. L’utilisateur peut sélectionner un résultat pour démarrer l’ajout d’un manga.

### Variantes et erreurs

- Si aucun résultat ne correspond à la recherche, l’application l’indique clairement.
- Si la connexion est absente, si le catalogue est indisponible ou si la requête échoue, l’application informe l’utilisateur et lui permet de réessayer.
- Les informations du catalogue sont des suggestions susceptibles d’être incomplètes ou inexactes ; l’utilisateur peut les corriger. Les données locales restent accessibles en cas d’erreur.

**Résultat :** L’utilisateur peut choisir un résultat à préremplir ou revenir à sa collection sans la modifier.

## UC-03 — Ajouter un manga

**Acteur principal :** Utilisateur  
**Acteur secondaire éventuel :** Catalogue public  
**Précondition :** L’application est ouverte.

### Scénario nominal — Depuis le catalogue

1. L’utilisateur lance l’ajout et recherche un manga (voir UC-02).
2. Il sélectionne un résultat.
3. L’application préremplit les informations fournies par le catalogue.
4. L’utilisateur vérifie et complète les champs, notamment le nombre de volumes possédés.
5. Il enregistre l’entrée.
6. L’application valide les informations et ajoute le manga à la collection.
7. Le manga apparaît dans la collection.

### Variante — Saisie manuelle

1. L’utilisateur choisit de saisir un manga manuellement plutôt que de sélectionner un résultat.
2. Il renseigne le titre obligatoire et les informations disponibles.
3. Il enregistre l’entrée ; l’application applique les mêmes validations que dans le scénario nominal.

### Erreur de saisie

Si le titre est vide ou si le nombre de volumes possédés n’est pas un entier supérieur ou égal à zéro, l’application signale l’erreur et permet à l’utilisateur de corriger les champs. L’entrée n’est pas enregistrée tant que les données requises ne sont pas valides.

**Résultat :** Une nouvelle entrée validée est ajoutée à la collection.

## UC-04 — Modifier un manga

**Acteur principal :** Utilisateur  
**Précondition :** La collection contient le manga à modifier.

### Scénario nominal

1. L’utilisateur sélectionne un manga dans la collection.
2. Il modifie les informations souhaitées : titre, auteur, volumes possédés ou total, genres, statut, jaquette ou note personnelle.
3. Il enregistre ses modifications.
4. L’application valide les champs obligatoires et met à jour l’entrée.

### Erreur de saisie

Si le titre est vide ou si le nombre de volumes possédés est invalide, l’application signale l’erreur et laisse l’utilisateur corriger les informations. Les modifications ne sont enregistrées qu’après validation.

**Résultat :** L’entrée mise à jour est visible dans la collection.

## UC-05 — Supprimer un manga

**Acteur principal :** Utilisateur  
**Précondition :** La collection contient le manga à supprimer.

### Scénario nominal

1. L’utilisateur sélectionne le manga et demande sa suppression.
2. L’application demande une confirmation avant la suppression définitive.
3. L’utilisateur confirme.
4. L’application retire le manga de la collection ainsi que ses associations aux genres.

### Variante — Annulation

Si l’utilisateur refuse la confirmation, le manga reste dans la collection.

**Résultat :** Le manga est supprimé uniquement si l’utilisateur confirme.

## UC-06 — Exporter la collection en CSV

**Acteur principal :** Utilisateur  
**Acteur secondaire :** Sélecteur de fichiers  
**Déclencheur :** L’utilisateur demande l’export de sa collection.

### Scénario nominal

1. L’utilisateur déclenche l’export CSV.
2. Le sélecteur de fichiers lui permet de choisir le nom et l’emplacement du fichier.
3. L’application génère un fichier CSV encodé en UTF-8, avec une ligne d’en-tête et une ligne par manga.
4. Le fichier contient au minimum le titre, l’auteur, le nombre de volumes possédés et, s’il est connu, le nombre total de volumes. Les autres champs disponibles peuvent également être inclus.
5. L’application échappe correctement les champs contenant des séparateurs, des guillemets ou des retours à la ligne.
6. L’application indique que l’export a réussi.

### Variantes et erreurs

- Si l’utilisateur annule le choix de destination, aucun export n’est créé.
- Si une erreur empêche l’écriture du fichier, l’application informe l’utilisateur que l’export a échoué.

**Résultat :** La collection est exportée dans le fichier choisi, ou l’utilisateur est informé de l’échec.
