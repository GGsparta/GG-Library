# Base de données

---

Le module `DB` inclus propose les fonctionnalités suivantes:
- **Définir des données par défaut** en tant que Scriptable ;
- **Persister les données** en les sauvegardant sur l'ordinateur.

Un simple accès au singleton [`Database`](/api/GGL.DB.Database.html) suffit à charger toutes les données !

> [!IMPORTANT]
> Dans une mise à jour future, il sera également possible de supprimer les données. Oui... c'est pas encore là.

## Déclaration des données

Créez votre classe de données en héritant de [`Data`](/api/GGL.DB.Data.html). Vous pouvez y inclure tout ce qui est sérialisable. Si vous souhaitez inclure des images, utilisez la classe [`ImageData`](/api/GGL.DB.Type.ImageData.html).

```` csharp
[Serializable]
public class Team : Data
{
    public string Name { get; protected set; }
    public Reference<League> League { get; protected set; }
    public Reference<ImageData> Icon { get; protected set; }

    public Team(): base(default) {}
    public Team(ulong id, ulong leagueId, string name, Sprite icon) : base(id)
    {
        // When creating a reference, specify either:
        // - the data ID:   the reference will request the data later
        // - the data:      it will be automatically added to database
        League = new Reference<League>(leagueId);
        Name = name;
        Icon = new Reference<ImageData>(new ImageData(ulong.Parse(icon.name), icon));
    }
}
````

Vous remarquerez que l'exemple ci-dessus fait usage de [`Reference<T>`](/api/GGL.DB.Reference.Reference-1.html) (pour les listes, voir [`ReferenceList<T>`](/api/GGL.DB.Reference.ReferenceList-1.html)). Lorsque vous essaierez d'accéder à leur valeur, ils vérifieront sa présence en cache avant de vous la rendre. Si ce n'est pas le cas, la référence cherchera la valeur dans la base de donnée.

> [!NOTE]
> Les références sont généralement propices à l'utilisation de données volatiles, mais ce n'est pas le cas ici. Cela viendra peut-être dans une prochaine version.

## Définition des données par défaut

Pour créer des données par défaut, il faudra générer des `ScriptableObject` dans le dossier `Assets/Resources`.

> [!IMPORTANT]
> Ce dossier propose un référencement de son contenu et est à utiliser **uniquement** en dernier recours car son utilisation peut provoquer des chutes de FPS. Dans notre cas, le contenu ne sera utilisé qu'en cas de réinitialisation de la base de données.

Afin d'inclure ces données par défaut à la base, il est nécessaire d'hériter de [`ScriptableData<T>`](/api/GGL.DB.Scriptable.ScriptableData-1.html). Au travers de cette classe, on peut définir les valeurs souhaitées, et définir comment la donnée sera créer et incluse à l'exécution.

```` csharp
public class ScriptableTeam : ScriptableData<Team>
{
    // Display a beautiful inspector for your data
    [Header("General")]
    public new string name;
    [Space]
    [ShowAssetPreview] public Sprite icon; 
    [Space]
    [Header("League")]
    public ScriptableLeague league;

#if UNITY EDITOR
    // To easily create your asset I advice you to use this piece of code :) 
    [MenuItem(MENU_CREATE + "Team")]
    public static void GenerateScriptableObject() => EditorUtils.CreateAsset<ScriptableTeam>(); 
#endif

    // Load you data in database cache
    public override Team Load() => new(id, league.id, name, icon);
}
````

Pour ensuite générer votre donnée par défaut:
- Créez un dossier avec le nom de votre classe de donnée (optionnel) puis allez-y ;
- Clic droit puis `Create > Database > [classe de donnée]` ;
- Renseignez les valeurs, et vous y êtes !

![Final result](/images/db_2.jpg)

Les éléments que vous venez de générer sont automatiquement géré par [`ScriptableDatabase<T>`](/api/GGL.DB.Scriptable.ScriptableDatabase.html).
Ces données vont être chargé au premier démarrage, et à chaque réinitialisation de la base.
Vous pouvez également réinitialiser en nettoyer les `PlayerPrefs`. (`Edit > Clear All PlayerPrefs`)

## Sauvegarder, restaurer et réinitialiser

Dès que vous accédez à [`Database`](/api/GGL.DB.Database.html), toutes les données sont mise en cache. Tous les scripts peuvent y accéder et modifier le contenu.
Ce même singleton possède les méthodes pour:
- Sauvegarder: enregistre les données sur la machine ;
- Restaurer: recharger les données depuis la machine ;
- Réinitialiser: écrase la sauvegarde machine et restaure les données par défaut.

> [!NOTE]
> La base de donnée est sauvegardée au format JSON dans le dossier référencé par [`Application.persistentDataPath`](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html). Sur Windows, il s'agit de `%appdata%\..\LocalLow\[CompanyName]\[ApplicationName]`.

