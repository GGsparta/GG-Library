using System;
using DG.Tweening;
using GGL.Animation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GGL.UI.Window
{
    /// <summary>
    /// Basic window that open and close based on a given animation.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/UI/Window")]
    [HelpURL(Settings.ARTICLE_URL + "ui" + Settings.COMMON_EXT)]
    public class Window : MonoBehaviour
    {
        /// <summary>
        /// Define the animations defaultly implemented in <see cref="Window"/>
        /// </summary>
        protected enum Transition
        {
            /// <summary>
            /// Transition will scale the object to vector (1,1,1).
            /// </summary>
            SCALE,
            /// <summary>
            /// Transition will fade alpha to 1. (requires a CanvasGroup component)
            /// </summary>
            FADE
        }
        
        #region Variables
        #region Editor
        /// <value>
        /// You can use your own custom animation for each state open/close.
        /// </value>
        [Header("Transition")]
        [SerializeField] [Tooltip("You can use your own custom animation for each state open/close.")]
        protected bool customTransition;
        
        /// <value>
        /// Custom animation triggered when opening the window.
        /// </value>
        /// <remarks>For custom animations, you'll have to trigger the end of animation by calling <see cref="InvokeOpenFinished"/> once completed.</remarks>
        [SerializeField] [ShowIf(nameof(customTransition))]
        [Tooltip("Custom animation triggered when opening the window. Please trigger 'InvokeOpenFinished' at the end.")]
        protected TriggerBehaviour openTransition;

        /// <value>
        /// Custom animation triggered when closing the window.
        /// </value>
        /// <remarks>For custom animations, you'll have to trigger the end of animation by calling <see cref="InvokeCloseFinished"/> once completed.</remarks>
        [SerializeField] [ShowIf(nameof(customTransition))]
        [Tooltip("Custom animation triggered when closing the window. Please trigger 'InvokeCloseFinished' at the end.")]
        protected TriggerBehaviour closeTansition;
        
        /// <value>
        /// Default implemented animation when not using a custom one.
        /// </value>
        [SerializeField] [HideIf(nameof(customTransition))]
        [Tooltip("Default implemented animation when not using a custom one.")]
        protected Transition transition = Transition.SCALE;
        
        /// <value>
        /// Duration for default transition. Not used by custom ones.
        /// </value>
        [SerializeField] [HideIf(nameof(customTransition))]
        [Tooltip("Duration for default transition. Not used by custom ones.")]
        protected float transitionDuration = .3f;
        
        /// <value>
        /// Ease for default transition. Not used by custom ones.
        /// </value>
        [SerializeField] [HideIf(nameof(customTransition))]
        [Tooltip("Ease for default transition. Not used by custom ones.")]
        protected Ease transitionEase = Ease.OutSine;

        [Header("Events")]
        [SerializeField] [Foldout("Events")]
        private UnityEvent onWindowOpen;
        [SerializeField] [Foldout("Events")]
        private UnityEvent onWindowClose;
        [SerializeField] [Foldout("Events")]
        private UnityEvent onOpenAnimFinished;
        [SerializeField] [Foldout("Events")]
        private UnityEvent onCloseAnimFinished;
        #endregion

        #region Public
        /// <value>
        /// Event triggered after the method <see cref="Open"/> is called.
        /// </value>
        public UnityEvent OnWindowOpen => onWindowOpen;
        /// <value>
        /// Event triggered after the method <see cref="Close"/> is called.
        /// </value>
        public UnityEvent OnWindowClose => onWindowClose;
        
        /// <value>
        /// Event triggered after the opening animation is completed.
        /// </value>
        /// <remarks>For custom animations, you'll have to trigger the end of animation by calling <see cref="InvokeOpenFinished"/> once completed.</remarks>
        public UnityEvent OnOpenAnimFinished => onOpenAnimFinished;
        
        /// <value>
        /// Event triggered after the closing animation is completed.
        /// </value>
        /// <remarks>For custom animations, you'll have to trigger the end of animation by calling <see cref="InvokeCloseFinished"/> once completed.</remarks>
        public UnityEvent OnCloseAnimFinished => onCloseAnimFinished;

        /// <value>
        /// Returns True if the window is currently open.
        /// </value>
        public bool IsOpen { get; protected set; }
        #endregion

        #region Private
        private readonly Vector2 _scaleZero = Vector2.one * .00001f; // Almost but non-zero vector, to avoid some text issues.
        private Tweener _tweener;
        private CanvasGroup _canvas;
        private CanvasGroup Canvas => _canvas ? _canvas : _canvas = GetComponent<CanvasGroup>();
        #endregion
        #endregion


        #region Methods
        private void OnValidate()
        {
            if(transition == Transition.FADE && !Canvas)
                Debug.LogWarning($"{name} is set with transition {nameof(Transition.FADE)} but has no CanvasGroup attached.");
        }

        #region Public
        /// <summary>
        /// Start the opening animation of the window and triggers the different events.
        /// </summary>
        /// <remarks>Fast open/close will kill custom animations but not the default transitions that will wait unitl its completed to recall the method.</remarks>
        [ContextMenu("Open")]
        public void Open()
        {
            if (closeTansition)
            {
                closeTansition.Kill();
            }
            else if(_tweener != null && _tweener.IsPlaying())
            {
                _tweener.OnComplete(() =>
                {
                    OnCloseFinished();
                    Open();
                });
                return;
            }

            IsOpen = true;
            gameObject.SetActive(true);

            if (openTransition)
            {
                openTransition.Trigger();
            }
            else
            {
                switch (transition)
                {
                    case Transition.SCALE:
                        transform.localScale = _scaleZero;
                        _tweener = transform.DOScale(Vector3.one, transitionDuration);
                        break;
                    case Transition.FADE:
                        Canvas.alpha = 0;
                        _tweener = Canvas.DOFade(1, transitionDuration);
                        break;
                    default:
                        _tweener = null;
                        break;
                }

                _tweener?
                    .SetId(this)
                    .SetEase(transitionEase)
                    .OnStepComplete(() =>
                    {
                        if (IsOpen)
                        {
                            OnOpenFinished(); 
                            _tweener.Pause();
                        }
                        else
                        {
                            OnCloseFinished();
                        }
                    })
                    .SetLoops(2, LoopType.Yoyo);
            }

            OnOpen();
        }


        /// <summary>
        /// Start the closing animation of the window and triggers the different events.
        /// </summary>
        /// <remarks>Fast open/close will kill custom animations but not the default transitions that will wait unitl its completed to recall the method.</remarks>
        [ContextMenu("Close")]
        public void Close()
        {
            if (!gameObject.activeInHierarchy || !IsOpen)
            {
                if(IsOpen)
                {
                    gameObject.SetActive(false);
                }

                return;
            }

            if (openTransition)
            {
                openTransition.Kill();
            }
            else if(_tweener == null || _tweener.IsPlaying())
            {
                _tweener?.OnStepComplete(() =>
                {
                    OnCloseFinished();
                    _tweener.Pause();
                });
                return;
            }
            
            IsOpen = false;

            if (closeTansition)
                closeTansition.Trigger();
            else
                _tweener.Play();

            OnClose();
        }

        /// <summary>
        /// Will open or close the window depending on value.
        /// </summary>
        /// <param name="open"></param>
        public void SetOpen(bool open)
        {
            if (open) Open();
            else Close();
        }
        
        /// <summary>
        /// When using custom aniamtions, call this to complete your opening animation
        /// </summary>
        public void InvokeCloseFinished() => OnCloseFinished();
        /// <summary>
        /// When using custom aniamtions, call this to complete your closing animation
        /// </summary>
        public void InvokeOpenFinished() => OnOpenFinished();
        #endregion

        #region Private
        /// <summary>
        /// Define code to execute after the method <see cref="Open"/> is called.
        /// </summary>
        protected virtual void OnOpen() => onWindowOpen?.Invoke();
        
        /// <summary>
        /// Define code to execute after the method <see cref="Close"/> is called.
        /// </summary>
        protected virtual void OnClose() => onWindowClose?.Invoke();

        /// <summary>
        /// Define code to execute after the animation of opening is completed.
        /// </summary>
        protected virtual void OnOpenFinished() => onOpenAnimFinished.Invoke();

        /// <summary>
        /// Define code to execute after the animation of closing is completed.
        /// </summary>
        protected virtual void OnCloseFinished()
        {
            onCloseAnimFinished.Invoke();
            
            gameObject.SetActive(false);
            if(customTransition && openTransition is ITriggerState state) state.ResetState();
            else
            {
                switch (transition)
                {
                    case Transition.SCALE:
                        transform.localScale = Vector3.one;
                        break;
                    case Transition.FADE:
                        Canvas.alpha = 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            _tweener = null;
        }
        #endregion
        #endregion
    }
}