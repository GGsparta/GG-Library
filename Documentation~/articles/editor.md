# Outils pour l'éditeur

---

Des utilitaires très pratiques sont disponibles dans `Tools > GG-Library` !

## Singleton Loader

Cet outil permet d'activer ou désactiver les Singletons associés à des prefabs dans le projet. Voir [l'article dédié](/articles/singleton.html#configuration-avancée).

## Assets Merger

Il arrive que certains fichers soient en double, et référencés par différentes parties du projet. Cet outil permet de localiser ces duplicatas et de les fusionner au besoin.

## Event Tracker

Sur Unity, beaucoup d'évènements sont gérés par `UnityEvent` et leurs callbacks sont souvent attribués depuis l'éditeur. L'envers du décor est que ces appels ne sont pas traçable depuis votre IDE (sauf par debug).
L'_Event Tracker_ permet de rentrer un nom de méthode et d'en visualiser les appels dans la scène.

## Empty Folder Cleaner

Pour diverses raisons (nettoyage, refactor, git, etc.), il arrive que des dossiers soient vidés sans être supprimés - et chaque retour sur Unity va leur attribuer un fichier `.meta`. Cet outil va supprimer tous les répertoires qui sont effectivement vides. 

## Missing References Tracker

Un renommage de script et c'est la catastrophe: ses instances sont marqués comme "_Missing Script_" dans toutes les scènes ! Cet utilitaire vous permet de localiser toutes les zones où des scripts sont manquants ou introuvables dans un ensemble de `GameObject`. 🎉

> [!NOTE]
> Votre script dans les assets possède un `Instance ID`. Affectez le sur votre "_Missing Script_" pour restaurer le component tout en conservant les valeurs précédemment affectées !  
> 👉 Pour rendre ce champ visible, il faudra passer l'inspecteur en mode Debug.  
> 👉 Si le script manquant est sur un prefab, éditez le prefab directement.

## References Tracker

Permet de voir toutes les références d'un objet/asset quelconque dans la scène sélectionnée. Utile pour des fins de nettoyage.

## Utilitaires C#

Côté code, la classe `EditorUtility` possède des outils pour l'exécution de code dans lié à l'éditeur. C'est utile pour modifier et sauvegarder un asset par exemple.