using System;
using System.Collections.Generic;
using System.Linq;
using GGL.Audio;
using GGL.Events;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGL
{
    /// <summary>
    /// It will load every <see cref="SingletonBehaviour"/> defined by <i>Tools > GG-Library > Manage Singletons</i>.
    /// </summary>
    /// <remarks>Don't touch that.</remarks>
    public class Configuration : Singleton.ScriptableSingleton<Configuration>
    {
        [Header("Audio")] 
        [SerializeField] private AudioChannel[] channelsToInit;
        [Tooltip("Default channels will be used by their respective singleton player")]
        [SerializeField] private AudioMixerGroup defaultSoundsChannel, defaultMusicChannel;
        public static AudioMixerGroup SoundsChannel => Instance.defaultSoundsChannel;
        public static AudioMixerGroup MusicChannel => Instance.defaultMusicChannel;

        
        
        [Header("Singletons")] [SerializeField] [ReadOnly]
        private List<GameObject> singletonPrefabs = new();

#if UNITY_EDITOR
        [Button("Manage Singletons")]
        [ContextMenu(nameof(Manage))]
        [ExcludeFromDocFx]
        public void Manage() => EditorApplication.ExecuteMenuItem("Tools/GG-Library/Manage Singletons");
#endif
        
        
        
        #region Singletons
        [ExcludeFromDocFx]
        public void Clean() => singletonPrefabs.RemoveAll(s => !s);

        [ExcludeFromDocFx]
        public bool Contains(GameObject singleton) => singletonPrefabs.Contains(singleton);

        [ExcludeFromDocFx]
        public void Remove(GameObject singleton) => singletonPrefabs.Remove(singleton);

        [ExcludeFromDocFx]
        public void Add(GameObject singleton) => singletonPrefabs.Add(singleton);

        /// <summary>
        /// Set state of a Singleton
        /// </summary>
        /// <param name="singleton"></param>
        /// <param name="value"></param>
        public void Set(GameObject singleton, bool value)
        {
            if (value) Add(singleton);
            else Remove(singleton);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            // Initialize before everything
            Instance.Clean();
            foreach (GameObject prefab in Instance.singletonPrefabs)
                Instantiate(prefab).name = prefab.name;
            
            // Initialize on Start
            GameObject loader = new();
            loader.AddComponent<OnStartEvent>().OnInvoke.AddListener(() =>
            {
                foreach (AudioChannel channel in Instance.channelsToInit)
                    channel.Init();
                
                Destroy(loader);
            });
        }

        protected override void Init()
        {
#if UNITY_EDITOR
            defaultSoundsChannel = AssetDatabase
                .LoadAssetAtPath<AudioMixerGroup>(AssetDatabase.GUIDToAssetPath("74f231a9cd4352745b6f99c763b82416"))
                .audioMixer
                .FindMatchingGroups("Sounds")
                .First();

            defaultMusicChannel = AssetDatabase
                .LoadAssetAtPath<AudioMixerGroup>(AssetDatabase.GUIDToAssetPath("74f231a9cd4352745b6f99c763b82416"))
                .audioMixer
                .FindMatchingGroups("Music")
                .First();

            channelsToInit = new[]
            {
                new AudioChannel(defaultSoundsChannel),
                new AudioChannel(defaultMusicChannel)
            };
#endif
        }
        
        
#if UNITY_EDITOR
        public class AssetModificationDetector : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if(!Instance)
                    Debug.LogError($"{nameof(Configuration)} cannot be created!");
            }
        }
#endif
    }
}