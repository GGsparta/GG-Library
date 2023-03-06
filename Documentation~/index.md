# GG-Library 1.0.0

La bibliothèque *GG-Library* propose des outils, extensions et patrons de programmation en C# afin de créer rapidement des jeux optimisés sur Unity suivant les principes SOLID.

_Note: compatible à partir de Unity 2022.2._

> [!NOTE]
> **Bibliothèque en cours de création** : Elle est en constante évolution et stabilisation. Si vous repérez un bug, [signalez-le](https://github.com/GGsparta/GG-Library/issues) ! Avec un peu de chance je pourrai m'y atteler rapidement.

## Installation

Dans le fichier ``Packages/manifest.json`` de votre projet Unity, ajouter les lignes suivantes:

```
{
  "dependencies": {
    [...]
    "com.dbrizov.naughtyattributes": "https://github.com/dbrizov/NaughtyAttributes.git#upm",
    "es.jacksparrot.dotween": "https://github.com/JackSParrot/dotween-pkg.git",
    "com.yoannhaffner.ggl": "https://github.com/GGsparta/GG-Library.git#1.0.0"
  }
}
```

> [!NOTE]
> Le numéro de version est optionnel. Si vous souhaitez toujours être à la dernière version, ne le spécifiez pas.
> Vous pouvez également profiter des dernières modifications en utilisant le tag ``#dev``.

> [!IMPORTANT]
> Si vous rencontrez des problèmes d'importation ou de script introuvable durant **l'installation** ou **la mise à jour**, allez sur la fenêtre `Project` de Unity, faites clic droit sur `Packages/GG-Library` puis `Reimport`.

Tout est bon ? Alors par [ici](/articles/intro.html) la suite ! 😁