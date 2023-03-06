using NaughtyAttributes;
using UnityEngine;

namespace GGL.Singleton
{
    /// <summary>
    /// Parent class of every <see cref="SingletonBehaviour{T}"/>. DO NOT INHERIT THIS CLASS DIRECTLY.
    /// </summary>
    public abstract class SingletonBehaviour : MonoBehaviour {}
    
    /// <summary>
    /// <see cref="Singleton{T}"/> adapted to extend <see cref="MonoBehaviour"/>.
    /// </summary>
    /// <remarks>When you can, try to setup static class to avoid using methods of a dead class.</remarks>
    /// <example><see cref="GGL.UI.LoadingScreen"/> ou <see cref="GGL.DB.Database"/></example>
    /// <typeparam name="T">Monobehaviour class</typeparam>
    [HelpURL(Settings.ARTICLE_URL + "singleton" + Settings.COMMON_EXT)]
    public abstract class SingletonBehaviour<T> : SingletonBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;

        /// <value>
        /// Access the cached instance. If it does not exist, it will be created on a new root GameObject.
        /// </value>
        public static T Instance => _instance ? _instance : _instance = FindObjectOfType<T>() ?? InstantiateSingleton();

        private static T InstantiateSingleton()
        {
            T itsAMe = new GameObject(typeof(T).Name).AddComponent<T>();
            Debug.Log($"{itsAMe.name} just has been created from scratch.", itsAMe);
            return itsAMe;
        }

        /// <value>
        /// Check if the Singleton has been initialized. Happens if it has not yet been accessed nor awaken in any scene.
        /// </value>
        public static bool Initialized => _instance;

        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void Awake()
        {
            if (_instance && !Equals(_instance))
            {
                Destroy(gameObject);
                Debug.LogWarning($"Destroyed Singleton self {name} because an instance already exists.");
                return;
            }

            if (transform.parent) 
                Debug.LogError($"{name} is a Singleton and should be a root scene GameObject!");
            
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

#if UNITY_EDITOR
        private bool ShowInfoBox => transform.parent;
        [InfoBox("This component is a Singleton and should be on a root scene GameObject!", EInfoBoxType.Error)]
        [ShowIf(nameof(ShowInfoBox))] [InspectorName("Error")] [SerializeField] [BoxGroup("Be careful!")] [ReadOnly]
        // ReSharper disable once NotAccessedField.Local
#pragma warning disable 0414
        private string error;
#pragma warning restore 0414
#endif
    }
}