using UnityEngine;

namespace GGL.Singleton
{
    /// <summary>
    /// Scene-contextual alternative to <see cref="SingletonBehaviour{T}"/>.
    /// </summary>
    /// <typeparam name="T">Monobehaviour class</typeparam>
    /// <remarks><b>It won't be automatically instantiated</b>: add your script in scene.</remarks>
    [HelpURL(Settings.ARTICLE_URL + "singleton" + Settings.COMMON_EXT)]
    public class CachedBehaviour<T> : MonoBehaviour where T : CachedBehaviour<T>
    {
        private static T _instance;
        
        /// <value>
        /// Access the cached instance. If it does not exist, will try to find it in scene and cache it.
        /// </value>
        public static T Instance => Exists ? _instance : _instance = FindObjectOfType<T>();
        
        /// <value>
        /// Check if the Singleton has been initialized. Happens if it has not yet been accessed nor awaken in any scene.
        /// </value>
        public static bool Exists => _instance;
        
        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void Awake()
        {
            if (_instance && !Equals(_instance))
            {
                Destroy(gameObject);
                Debug.LogWarning($"Destroyed cached self {name} because an instance already exists.");
                return;
            }
            _instance = this as T;
        }
    }
}