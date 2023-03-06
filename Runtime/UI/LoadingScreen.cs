using System;
using System.Collections;
using System.Diagnostics;
using GGL.Collections;
using GGL.Pooling;
using GGL.Singleton;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GGL.UI
{
    /// <summary>
    /// Singleton that load a scene while showing a window.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Singletons/Loading Screen")]
    [HelpURL(Settings.ARTICLE_URL + "ui" + Settings.COMMON_EXT + "#loading-screen")]
    public class LoadingScreen : SingletonBehaviour<LoadingScreen>
    {
        /// <value>
        /// Window to display when loading.
        /// </value>
        /// <remarks>Make it fullscreen.</remarks>
        [SerializeField] private Window.Window content;
        
        /// <value>
        /// If loading is too fast, animations might not be visible. Define how long it should last at least!
        /// </value>
        [SerializeField] private float minDuration;
        
        /// <value>
        /// Load duration can vary and be hard to tell to the user. Compense here so that te pourcentage can be reliable
        /// </value>
        [MinMaxSlider(0f, 1f)]
        [SerializeField] private Vector2 sceneLoadingProportion = new(0.1f, 0.9f);
        
        /// <value>
        /// Event raised each frame during load. Contains progression in range [0,1]
        /// </value>
        [SerializeField] private UnityEvent<float> onLoadProgress;
        
        /// <value>
        /// Event raised each when the window is finally closed.
        /// </value>
        [SerializeField] private UnityEvent onLoadCompleted;

        /// <inheritdoc cref="onLoadProgress"/>
        public static UnityEvent<float> OnLoadProgress => Instance.onLoadProgress;
        /// <inheritdoc cref="onLoadCompleted"/>
        public static event Action OnLoadCompleted;

        private PriorityQueue<IEnumerator, int> _processes = new();
        public static PriorityQueue<IEnumerator, int> Processes => Instance._processes;
        
        private Stopwatch _watch;  // Used in parallel to check minimal duration
        private string _sceneToLoad;


        /// <summary>Optimize event listening.</summary>
        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void OnEnable()
        {
            content.OnWindowOpen.AddListener(PrepareLoading);
            content.OnOpenAnimFinished.AddListener(LaunchLoading);
            content.OnCloseAnimFinished.AddListener(FinalizeLoading);
        }

        /// <summary>Optimize event listening.</summary>
        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void OnDisable()
        {
            content.OnWindowOpen.RemoveListener(PrepareLoading);
            content.OnOpenAnimFinished.RemoveListener(LaunchLoading);
            content.OnCloseAnimFinished.RemoveListener(FinalizeLoading);
        }

        
        /// Act like if another scene was being loaded.
        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void Start() => StartCoroutine(EPostLoadEvents());


        /// <summary>
        /// Loads a new scene after fading on the canvas
        /// </summary>
        /// <param name="sceneName">Scene to be loaded</param>
        public static void Load(string sceneName)
        {
            if (Instance._sceneToLoad != null) return;

            Instance._sceneToLoad = sceneName;
            Instance.onLoadProgress.Invoke(0);
            Instance._watch = Stopwatch.StartNew();
            Instance.content.Open();
        }

        private void PrepareLoading() => onLoadProgress.Invoke(0);
        private void LaunchLoading()
        {
            if (_sceneToLoad == null) return;
            StartCoroutine(ELoad(_sceneToLoad));
        }

        private void FinalizeLoading()
        {
            _sceneToLoad = null;
            onLoadCompleted.Invoke();
            OnLoadCompleted?.Invoke();
            OnLoadCompleted = null;
        }

        private IEnumerator ELoad(string sceneName)
        {
            if (ObjectPooler.Initialized) ObjectPooler.Clear();
            onLoadProgress.Invoke(sceneLoadingProportion.x);
            
            yield return null;
            
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone || _watch.Elapsed.TotalSeconds < minDuration)
            {
                onLoadProgress.Invoke(sceneLoadingProportion.x + (sceneLoadingProportion.y - sceneLoadingProportion.x) * op.progress);
                yield return null;
            }

            onLoadProgress.Invoke(sceneLoadingProportion.y);
            yield return null;
            yield return EPostLoadEvents();
        }

        private IEnumerator EPostLoadEvents()
        {
            float t = 0, tt = _processes.Count;

            while (_processes.Count > 0)
            {
                RaiseProgress(sceneLoadingProportion.y + (++t/tt) * (1 - sceneLoadingProportion.y));
                yield return _processes.Dequeue();
            }
            
            onLoadProgress.Invoke(1);
            yield return null;
            
            _watch?.Stop();
            if (content.IsOpen) content.Close();
            else content.InvokeCloseFinished();
        }

        private void RaiseProgress(float progress) =>
            onLoadProgress.Invoke(_watch != null
                ? Mathf.Min(progress, (float)_watch.Elapsed.TotalSeconds / minDuration)
                : progress);
    }
}