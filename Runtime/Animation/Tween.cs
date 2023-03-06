using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace GGL.Animation
{
    /// <summary>
    /// Base class for anything you want to tween via inspector.
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <remarks>To learn how to properly inherit this class, check the HelpURL attribute.</remarks>
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT + "#creer-votre-propre-composant-tweent")]
    public abstract class Tween<T> : TriggerBehaviour, ITriggerState where T : Component
    {
        #region Variables
        #region Editor
        /// <value>
        /// Target to animate. If not set, it will find one in GameObject or children.
        /// </value>
        [Header("Tween - Settings")]
        [SerializeField] [Tooltip("Target to animate. If not set, it will find one in GameObject or children.")]
        protected T target;

        /// <value>
        /// Duration of tween animation.
        /// </value>
        [SerializeField] [Tooltip("Duration of tween animation.")]
        protected float duration = 1f;

        /// <value>
        /// Delay before starting the animation once triggered. <b>Won't be used between loops</b>.
        /// </value>
        [SerializeField] [Tooltip("Delay before starting the animation once triggered. Won't be used between loops!")]
        protected float delay;

        /// <value>
        /// Define how many time the animation will be repeated once triggered.
        /// </value>
        /// <remarks>Set '-1' for infinite loops.</remarks>
        [Header("Tween - Behaviour")]
        [SerializeField] [Tooltip("Set '-1' for infinite loops")]
        protected int loopCount = 1;

        /// <value>
        /// Define how the tween animation will loop.
        /// </value>
        [SerializeField] [Tooltip("Define how the tween animation will loop.")]
        protected LoopType loopType;

        /// <value>
        /// Will use a custom curve instead of usual default curve.
        /// </value>
        [SerializeField] [Tooltip("Will use a custom curve instead of usual default curve.")]
        protected bool useCustomEase;
        
        /// <value>
        /// Ease of the animation.
        /// </value>
        [SerializeField] [HideIf(nameof(useCustomEase))]
        [Tooltip("Ease of the animation.")]
        protected Ease ease = Ease.Linear;
        
        /// <value>
        /// Curve followed to reach the final value. (e.g. Linear, Sine,...)
        /// </value>
        [SerializeField] [ShowIf(nameof(useCustomEase))]
        [Tooltip("Curve followed to reach the final value. (e.g. Linear, Sine,...)")]
        protected AnimationCurve easeCurve; // Do not set a default curve because of an intern Unity refreshing issue (2019.4)

        /// <value>
        /// A tween can be triggered if its method <see cref="Trigger"/> is called. Decide if it is permitted. 
        /// </value>
        [Header("Tween - Advanced")]
        [SerializeField] [Tooltip("A tween can be triggered if its method is called. Decide if it is permitted.")]
        protected bool enableTriggerWhileDisable;

        /// <value>
        /// Ignores the delay and set a random position on the curve, in range [0;1].
        /// </value>
        [SerializeField] [Tooltip("Ignores the delay and set a random position on the curve, in range [0;1].")]
        protected bool randomizeStartPoint;

        /// <value>
        /// Ignores the delay and syncs movement to the other similar tweens.
        /// You really should set <see cref="loopType"/> to <see cref="LoopType.Restart"/> when activating this.
        /// </value>
        [SerializeField]
        [Tooltip("Ignores the delay and sync movement to the other similar tweens.\n" +
                 "You really should have a 'restart' looptype when activating this.")]
        protected bool syncToTweensStart;

        /// <value>
        /// Resets object base state when disabled/killed (not on completion)
        /// </value>
        [SerializeField] [Tooltip("Resets object base state when disabled/killed (not on completion).")]
        protected bool resetOnDisable = true;

        /// <value>
        /// This event is triggered when the tween is started.
        /// </value>
        [Foldout("Events")]
        [SerializeField] [Tooltip("This event is triggered when the tween is started.")]
        protected UnityEvent onStart;
        
        /// <value>
        /// This event is triggered every frame while the tween is running.
        /// </value>
        [Foldout("Events")]
        [SerializeField] [Tooltip("This event is triggered every frame while the tween is running.")]
        protected UnityEvent onUpdate;
        
        /// <value>
        /// This event is triggered when the tween is fully completed.
        /// </value>
        [Foldout("Events")]
        [SerializeField] [Tooltip("This event is triggered when the tween is fully completed.")]
        protected UnityEvent onComplete;
        #endregion

        #region Public
        /// <summary>
        /// Target to animate. If not set, it will find one in GameObject or children.
        /// </summary>
        public T Target
        {
            get => target ? target : target = GetComponentInChildren<T>();
            set => target = value;
        }

        /// <summary>
        /// This event is triggered when the tween is fully completed.
        /// </summary>
        public UnityEvent OnStart => onStart;
        
        /// <summary>
        /// This event is triggered every frame while the tween is running.
        /// </summary>
        public UnityEvent OnUpdate => onUpdate;
        
        /// <summary>
        /// This event is triggered when the tween is fully completed.
        /// </summary>
        public UnityEvent OnComplete => onComplete;
        #endregion

        #region Protected
        protected float startTime;
        #endregion

        #region Private
        private TweenParams _config;
        private bool _saved;
        private Tween _tween;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc />
        [ExcludeFromDocFx]
        protected override void OnEnable()
        {
            if (!Target)
            {
                Debug.LogError($"Could not find any object to animate in {gameObject.name} or children");
                return;
            }

            SaveIfNeeded();

            if (triggerOnceEnabled && (!DOTween.IsTweening(this) || !resetOnDisable))
            {
                if (syncToTweensStart && Time.timeSinceLevelLoad < 1.5f)
                    Invoke(nameof(Trigger), .01f); // Avoid LevelLoading offsets
                else Trigger();
            }
            else
            {
                DOTween.Play(this);
            }
        }

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnDisable()
        {
            if (resetOnDisable) Kill();
            else DOTween.Pause(this);
        }

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnDestroy() => DOTween.Kill(this);

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnValidate()
        {
            if (!target) target = GetComponentInChildren<T>();

            easeCurve ??= AnimationCurve.Linear(0, 0, 1, 1);
        }
        #endregion

        #region Public
        /// <summary>
        /// Trigger the tween animation
        /// </summary>
        public override void Trigger()
        {
            if (!isActiveAndEnabled && !enableTriggerWhileDisable)
            {
                _config = null;
                throw new UnityException(
                    $"{name}'s tween trigger was denied because it is not allowed by user if the compoment is inactive.");
            }

            SaveIfNeeded();
            Kill();
            
            _config = new TweenParams()
                .SetId(this)
                .SetLoops(loopCount, loopType)
                .OnStart(() =>
                {
                    if (syncToTweensStart || randomizeStartPoint)
                        _tween.Goto(startTime % duration, true);
                    onStart.Invoke();
                })
                .OnUpdate(onUpdate.Invoke)
                .OnComplete(onComplete.Invoke)
                .SetDelay(delay);

            if (useCustomEase) _config.SetEase(easeCurve);
            else _config.SetEase(ease);
            
            startTime = 0;
            if (syncToTweensStart) startTime += Time.timeSinceLevelLoad;
            if (randomizeStartPoint) startTime += duration * Random.value;

            _tween = GenerateTween().SetAs(_config);
        }

        /// <summary>
        /// Kill the tween animation
        /// </summary>
        public override void Kill()
        {
            if (!DOTween.IsTweening(this))
                return;
            DOTween.Kill(this, true);
            if (resetOnDisable) ResetState();
        }

        /// <summary>
        /// Create the animation with DOTween. 
        /// </summary>
        /// <remarks>
        /// It can be generated by extension from the target type (check <a href="http://dotween.demigiant.com/documentation.php#shortcuts">shortcuts</a>) or with <see cref="DOTween"/> static methods.<br />
        /// If you need to add an OnStart/OnUpdate/OnCompleted event, please use the ones in <see cref="Tween{T}"/> class.
        /// </remarks>
        public abstract Tween GenerateTween();

        /// <summary>
        /// Buffer the values of the target concerned by the animation. Used when restoring on disable.
        /// </summary>
        public abstract void SaveState();


        /// <summary>
        /// Restore the values you buffered via <see cref="ITriggerState.SaveState"/>. Used when restoring on disable.
        /// </summary>
        public abstract void ResetState();
        #endregion

        #region Private
        /// <summary>
        /// Save state if not done yet once enbaled
        /// </summary>
        private void SaveIfNeeded()
        {
            if (_saved) return;
            SaveState();
            _saved = true;
        }
        #endregion
        #endregion
    }
}