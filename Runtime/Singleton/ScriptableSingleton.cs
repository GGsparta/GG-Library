using System;
using GGL.DB.Scriptable;
using UnityEditor;
using UnityEngine;

namespace GGL.Singleton
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        /// <value>
        /// Access the singleton instance
        /// </value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    _instance = Resources.Load<T>(ResourcesPath);
                    if (!_instance) 
                        AssetDatabase.CreateAsset(_instance = CreateInstance<T>(), GlobalPath);
#else
                    _instance = Resources.Load<T>(ResourcesPath);
#endif
                }

                return _instance;
            }
        }
        private static T _instance;
        
        
        protected static string GlobalPath => "Assets/Resources/" + ResourcesPath + ".asset";
        protected static string ResourcesPath => typeof(T).Name;

        private void Reset() => Init();

        /// <summary>
        /// Init the scriptable once created. Called on reset as well.
        /// </summary>
        protected abstract void Init();
    }
}