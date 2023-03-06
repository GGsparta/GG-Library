# Outils pour GameObject

---

Cette bibliothèque inclut encore plusieurs composants utiles qui ne suffisent pas à la rédaction d'un artcile. Vous trouverez un résumer de ce qu'il reste et qu'il est bon de connaître. 😁

## Évènements

Les composants [`Event`](/api/GGL.Events.html) permettent de gérer par l'inspecteur Unity les évènements de cycle de vie d'un objet, et différemment selon la plateforme. 

Il y a aussi des évènements qui en convertissent d'autres. 
Par exemple, [`BooleanEvent`](/api/GGL.Events.BooleanEvent.html) permet rediriger un évènement booléen selon sa valeur. Cela est très utile pour gérer un évènement de `Toggle` par exemple. 
De même, [`FormatEvent`](/api/GGL.Events.FormatEvent.html) permetter de formatter calculer et formatter la valeur transportée par un évènement en texte. 

> [!NOTE]
> Bien évidemment, vous pouvez créer vos propres évènements de plateforme en héritant de [`PlatformEvent`](/api/GGL.Events.PlatformEvent.html).

## Optimisation

Il existe des tonnes de conseil autour de l'optimisation à utiliser dans des cas très spécifiques.  
Si une optimisation générique est envisageable, c'est bien celle de la mise en commun d'objets ([Object Pooling](https://learn.unity.com/tutorial/introduction-to-object-pooling)).

Vous avez peut-être déjà créé une liste visuelle à partir d'une requête BDD, ou encore implémenté des ennemis nombreux qui spawn et meurent régulièrement.
Dans ce cas, vous êtes rapidement ralentit car vous instanciez et détruisez beaucoup d'objets Unity, une opération couteuse que vous pouvez éviter grâce au Singleton [`ObjectPooler`](/api/GGL.Pooling.ObjectPooler.html).

Au lieu de créer et détruire vos objets, appelez les méthodes `ObjectPooler.Instantiate` ou `ObjectPooler.Destroy` qui vont recycler vos objets sans les détruire.

> [!IMPORTANT]
> Afin de déterminé si l'objet a déjà référencé, il est identifié par son nom. Par conséquent, Il est important de ne pas renommer votre objet.

> [!NOTE]
> Le nettoyage du cache est à votre charge. Si vous utilisez le [`LoadingScreen`](/api/GGL.UI.LoadingScreen.html), il se chargera de ce vidage à chaque chargement de scène.

## Divers

La *GG-Library* inclut encore quelques composants utiles mais inclassables que vous pouvez explorer [ici](/api/GGL.Components.html). 
Jetez également un œil aux [extensions](/api/GGL.Extensions.html) et [utilitaires](/api/GGL.Utility.html) C# inclus 😁.