using System.Collections.Generic;
using System.Linq;
using GGL.Singleton;
using UnityEditor;
using UnityEngine;

namespace GGL.Editor.SingletonLoader
{
    public class SingletonLoaderWindow : BaseWindow<SingletonLoaderWindow>
    {
        private Configuration _loader;
        private Dictionary<SingletonBehaviour, bool> _singletons;
        private Vector2 _scrollPostion;
        public static bool dirty;


        [MenuItem("Tools/GG-Library/Manage Singletons", false, 5)]
        public static void Open() => Open("Singleton Loader");
        
        private void OnEnable() => Refresh();

        private void Refresh()
        {
            if (!_loader)
            {
                _loader = Resources.Load<Configuration>(nameof(Configuration));
                if(!_loader)
                {
                    _loader = CreateInstance<Configuration>();
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.CreateAsset(_loader, $"Assets/Resources/{nameof(Configuration)}.asset");
                }
            }

            _loader.Clean();
            _singletons = SingletonLoader.SearchSingletonPrefabs().ToDictionary(s => s, s => _loader.Contains(s.gameObject));
        }

        protected override void Render(Vector2 pos, Vector2 size)
        {
            if (dirty || _singletons.Values.Any(script => !script))
            {
                Refresh();
                dirty = false;
            }
            
            if (_singletons.Keys.Count == 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Such empty :/\n\nAdd your Singleton script to a prefab\nand it will show up here !\n\n\n", new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState
                    {
                        textColor = Color.white
                    }
                });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if(GUILayout.Button(nameof(Refresh))) Refresh();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                return;
            }
            
            _scrollPostion = EditorGUILayout.BeginScrollView(_scrollPostion);

            foreach (KeyValuePair<SingletonBehaviour, bool> current in _singletons)
            {
                EditorGUILayout.BeginHorizontal(new GUIStyle
                {
                    padding = new RectOffset(5,5,5,0),
                    fixedWidth = size.x,
                    alignment = TextAnchor.MiddleRight
                });

                GUIStyle guiStyle = new(EditorStyles.wordWrappedLabel)
                {
                    padding = new RectOffset(0,10,0,0),
                    normal = new GUIStyleState
                    {
                        textColor = Color.white
                    }
                };
                EditorGUILayout.LabelField(current.Key.GetType().FullName, guiStyle, GUILayout.Width(size.x/2));
                
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(string.Empty, current.Key, typeof(SingletonBehaviour), false, GUILayout.Width(size.x/2 - 30));
                EditorGUI.EndDisabledGroup();
                
                if (current.Value != EditorGUILayout.Toggle(current.Value))
                {
                    _loader.Set(current.Key.gameObject, !current.Value);
                    dirty = true;
                }

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
    
    public class AssetModificationDetector : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        { 
            SingletonLoaderWindow.dirty = true;
            return paths;
        }

        private static void OnWillCreateAsset(string assetName) => SingletonLoaderWindow.dirty = true;
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            SingletonLoaderWindow.dirty = true;
            return AssetDeleteResult.DidNotDelete;
        }
    }
}