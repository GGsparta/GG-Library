# Singleton

---

> **DÉFINITION** :  
> Le Singleton est un patron de conception de création qui s’assure de l’existence d’un seul objet de son genre et fournit un unique point d’accès vers cet objet.

Si vous souhaitez créer un Singleton indépendant de Unity, il vous suffit d'hériter de [`Singleton<T>`](/api/GGL.Singleton.Singleton-1.html).

## Singleton Unity

Dans la plupart des cas, vous souhaiterez faire interagir votre Singleton avec la scène et hériter de `MonoBehaviour`. 
Pour ce faire, héritez de [`SingletonBehaviour<T>`](/api/GGL.Singleton.SingletonBehaviour-1.html). Votre script se créera de lui-même dès son premier accès.

### Configuration avancée

Vous pouvez configurer votre Singleton via l'inspecteur :
- Créez un prefab dans votre dossier `Assets` ;
- Ajoutez-y votre Singleton et configurez-le ;
- Ouvrez l'outil `Tools > GG-Library > Manage Singletons` ;
- Cochez votre Singleton.

Votre Singleton sera automatiquement instancié au lancement du jeu. Exemple avec le Singleton [`LoadingScreen`](/api/GGL.UI.LoadingScreen.html) :

![SingletonBehaviour usage example](/images/singleton_0.jpg)

## Singleton contextuel

L'utilisation de Singleton est fantastique, mais parfois, on veut accéder à un objet uniquement dans une scène spécifique sans l'emmener sur d'autres... (exemple: HUD)

Un équivalent que vous pouvez hériter est [`CachedBehaviour<T>`](/api/GGL.Singleton.CachedBehaviour-1.html), dont l'existence se limite à la scène.
Il vous faudra donc ajouter votre script manuellement dans la scène.

> [!NOTE]
> Unity étant _thread-safe_, il n'est pas nécessaire d'utiliser de lock, sémaphore ou mutex comme cela est parfois conseillé.
> Sauf si... 😤 sauf si vous vous la jouez [ECS](https://unity.com/ecs) ??
> <a href="https://www.youtube.com/watch?v=_wMD3754-og"><img title="Tu serai pas en train de te la jouer Turbo ??" src="https://www.nicepng.com/png/full/264-2649047_turbo-tastic-wreck-it-ralph-turbo-pixel.png" alt="Ralph's gone Turbo? - Wreck It Ralph" height="18px"></a>