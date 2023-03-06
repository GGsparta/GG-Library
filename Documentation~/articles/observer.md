# Observateur

---

> **DÉFINITION** :  
> L’Observateur est un patron de conception comportemental qui permet à certains objets d’envoyer des notifications concernant leur état à d’autres objets.

La *GG-Library* n'inclut pas directement ce patron, mais plutôt un encapsuleur. Il est basé non pas sur l'état mais sur la référence ou valeur du type.

Vous pouvez l'utiliser en créant un champs [`Observable<T>`](/api/GGL.Observer.Observable.html) avec `T` le type que vous souhaitez observer. Cela vous permet de:
- attribuer la valeur observée avec `Set(T valeur)` ;
- supprimer la valeur observée avec `Clear()` ;
- signaler une modification de champs interne avec `Commit()`.

Chacune de ces méthodes lèvera l'évènement `OnChange`. Exemple :

```` csharp
// Contains a string and refresh the display if changed.
public class StringDisplay : MonoBehaviour
{
    // Display
    [SerializeField]
    private Text textDisplay;
    
    // Stored string
    public Observable<string> Text { get; private set; };

    // Setup listener
    private void Awake() => Text.OnChange += Refresh;

    // Destroy listener
    private void OnDestroy() => Text.OnChange -= Refresh;

    // Refresh the display of the text.
    public void Refresh(string oldValue, string newValue) => textDisplay.text = newValue;
}
````

Pour les listes, vous pouvez utiliser le type `ObservableList<T>` par lequel ce même évènement sera levé par l'ajout et la suppression de données.