# Interface utilisateur

---

Aspect souvent négligé des Game Jams, il y a pourtant des éléments qui reviennent régulièrement. Les éléments ci-dessous devraient vous aider à ne pas passer trop de temps sur les interfaces.

## Window

Pour simplement fermer et ouvrir une entité graphique avec une animation, ajoutez le composant [`Window`](/api/GGL.UI.Window.Window.html). Les méthode `Open()` et `Close()` permettront de gérer au même endroit l'ouverture et la fermeture animées de votre fenêtre.

> [!NOTE]
> Vous pouvez créer des systèmes plus avancés en héritant de [`Window`](/api/GGL.UI.Window.Window.html). C'est notamment le cas de [`Popup`](/api/GGL.UI.Window.Popup.html) qui gèrera également les interactions avec un fond bloquant.

## Loading Screen

Créez un canvas avec une fenêtre de chargement. Ajoutez au canvas le Singleton [`LoadingScreen`](/api/GGL.UI.LoadingScreen.html). À chaque fois que vous demanderez à ce composant de charger un scène, il fera apparaître votre fenêtre le temps de charger la scène.

> [!NOTE]
> Pour rappel, la scène que vous chargez doit avoir été ajouté au préalable dans les paramètres de build.

## Afficher une donnée

Les composants graphiques en charge d'afficher une donnée, comme les éléments d'une liste renvoyée par une requête BDD, sont souvent très redondant dans leur structure.

Si il n'est pas possible de tout lier de manière générique, vous pouvez au moins hériter de [`MonoDisplayer<T>`](/api/GGL.UI.MonoDisplayer-1.html).
Cette classe contiendra votre donnée à afficher, et actualisera l'affichage en cas de mise à jour.