# Animation

---

## Animez rapidement sans créer de script !

La *GG-Library* inclut plusieurs [composants d'animation](/api/GGL.Animation.Tweens.html) (appelés *tween*) simplistes pour animer sans rédiger de code ! 
Ajoutez le composant que vous désirez, ajustez les paramètres, et c'est dans la boîte !

### Exemple 1 : Écran de chargement

Dans un écran de chargement, il est important de montrer que le jeu n'a pas planté. Rien de mieux qu'une petite animation !
Faisons simplement tourner un cercle à l'infini:
- Créez une image à l'intérieur de votre Canvas ;
- Ajoutez-y le composant [`RotateByTween`](/api/GGL.Animation.Tweens.RotateByTween.html) ;
- Cochez la case `Trigger Once Enabled` pour que l'animation démarre directement ;
- Affectez la valeur *-1* à `Loop Count` : cela définit un nombre infini de répétitions ;
- Changez l'axe de rotation pour tourner autour de l'axe Z. 

En lançant le jeu, vous devriez avoir le résultat ci-dessous.

![Loading example](/images/tweens_0.gif)

### Exemple 2 : Aggrégation d'animations

Réalisons une animation plus complexe: une balle qui rebondit !
L'idée est la faire rebondire en roulant. Le rebond aplatira légèrement la balle.

Nous avons donc 3 transformations: translation, rotation et scale.
L'élément perturbateur sera ce scale que l'on ne veut que sur l'axe Y du monde (et non local).
Pour faciliter cela, nous allonsplacer la balle dans un conteneur.
La balle aura la rotation, et le containeur les autres transformations.
Adoptons la hiérarchie d'objets suivante :

```` text
- Canvas
  - Animation
    - ScaleTween
    - MoveTween
    - RotateTween
  - BallContainer
    - Ball
  - Ground [optionnnel]
````

Mettez une image à votre balle, et laissez le conteneur vide de composants. Pour chaque enfant de `Animation`, créez et configurez le composant d'animation comme ci-dessous :

[![Ball example config](/images/tweens_1.png)](/images/tweens_1.png)

Pour le [`ScaleTween`](/api/GGL.Animation.Tweens.ScaleTween.html), nous allons créer une courbe d'animation personnalisée.
Si le `MoveTween` utilise une boucle Yoyo pour le rebond, nous allons définir le `ScaleTween` sur la totalité de la boucle (durée 2x plus longue que `MoveTween`).
Après le rebond, la balle subira encore le choc, contrairement à avant le choc... Ce qui nous donne une courbe similaire à celle ci-dessous.

![Ball example curve](/images/tweens_2.jpg)

Sélectionnez maintenant l'objet `Animation` et ajoutez le composant [`AnimationComposer`](/api/GGL.Animation.AnimationComposer.html). Cochez la case `Trigger Once Enabled` et vous pouvez lancer !
Le résultat devrait ressembler à ça :

![Ball example final](/images/tweens_3.gif)

<ins>À retenir</ins> :
- Dans ce cas, les composant d'animation seront appelé par le [`AnimationComposer`](/api/GGL.Animation.AnimationComposer.html) alors qu'ils ne sont pas encore initialisés, ce pourquoi on coche la case `Enable Trigger While Disabled`.
- Lorsque ces composants sont synchronisés avec précision (c'est la cas ici), `Sync To Tweens Start` permet d'éviter tout décalage.
- Ces composants sont intéressant dans la mesure où ils évitent de créer une multitude de scripts pour des animations légèrement différentes. Si vous avez une successions ou aggrégation d'animation complexe et unique à réaliser, vous avez tout intérêt à passer par votre propre script. 😅 

## Créez votre propre composant [`Tween<T>`](/api/GGL.Animation.Tween-1.html) 

Envie de créer votre propre composant d'animation pour animer des lumières, matériaux ou autres composants Unity ?  
Alors voici un simple exemple de code pour déplacer un objet :


```` csharp
// This is a movement tween! It moves a transform, so the generic type is Transform...
public class MoveTween : Tween<Transform>
{
    // Put a little header to separate the generic params from your new one.
    [Header("Tween - Move")]
    
    // Then put your params - like the movement
    [SerializeField]
    private Vector3 movement = Vector3.right;
    // [...]

    // Now let's define the base state in private.
    // When resetting the animation, we want to reinitialise the target with its base state.
    private Vector3 _basePosition;
    // [...]

    // Alright! Time to define the animation: do whatever you want.
    // The field `Target` has the type you specified: that is Transform in our case.
    public override Tween GenerateTween() => 
            Target.DOLocalMove(Target.localPosition + movement, duration);

    // Save the base state.
    public override void SaveState() => _basePosition = Target.localPosition;
    
    // Restore the base state.
    public override void ResetState() => Target.localPosition = _basePosition;
}
````
Pour hériter de [`Tween<T>`](/api/GGL.Animation.Tween-1.html), le type de votre cible d'animation vous est demandé. Les [shortcuts DOTween](http://dotween.demigiant.com/documentation.php#shortcuts) devraient vous aider à le définir.