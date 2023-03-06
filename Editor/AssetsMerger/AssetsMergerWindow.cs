using System;
using System.Collections.Generic;
using System.Linq;
using GGL.Editor.Tools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GGL.Editor.AssetsMerger
{
    public class AssetsMergerWindow : BaseWindow<AssetsMergerWindow>
    {
        [MenuItem("Tools/GG-Library/Merge duplicated assets", false, 50)]
        public static void Open() => Open("Assets Merger");

        public enum View
        {
            SINGLE_ASSET = 0,
            ASSETS_IN_DIRECTORIES = 1
        }

        public enum AssetType
        {
            MATERIALS = 2,
            TEXTURES = 3,
            SPRITES = 4,
            MESHES = 5,
        }

        private static string[] _viewNames = { "Single Asset", "Per Directory" };

        [SerializeField]
        private View selectedView = View.SINGLE_ASSET;

        [SerializeField]
        private Object targetAsset;

        [SerializeField]
        private FolderPath[] targetFolders = { };

        [SerializeField]
        private AssetType targetType = AssetType.MATERIALS;

        [SerializeField]
        private FolderPath[] searchFolders = { new("Assets") };

        [SerializeField]
        private List<Object> duplicates = new();

        [SerializeField]
        private bool deleteReplacedAssets;

        private Vector2 _scrollPosition = Vector2.zero;


        protected override void Render(Vector2 pos, Vector2 size)
        {
            selectedView = (View)GUILayout.Toolbar((int)selectedView, _viewNames);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            switch (selectedView)
            {
                case View.SINGLE_ASSET:
                    DrawSingleAssetView();
                    break;

                case View.ASSETS_IN_DIRECTORIES:
                    DrawAssetsInDirectoriesView();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSingleAssetView()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(this), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            Object newTargetAsset = EditorGUILayout.ObjectField("Target Asset", targetAsset, typeof(Object), false);
            if (newTargetAsset != targetAsset)
            {
                targetAsset = newTargetAsset;
                duplicates.Clear();
            }

            SerializedObject windowSo = new(this);

            EditorGUILayout.PropertyField(windowSo.FindProperty("duplicates"), true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(windowSo.FindProperty("searchFolders"), true);
            EditorGUILayout.Space();

            windowSo.ApplyModifiedPropertiesWithoutUndo();

            deleteReplacedAssets =
                EditorGUILayout.ToggleLeft("Delete Replaced Assets (Cannot be undone)", deleteReplacedAssets);
            EditorGUILayout.Space();

            if (targetAsset != null && GUILayout.Button("Search Duplicates"))
            {
                duplicates = AssetsMerger.FindPotentialDuplicates(targetAsset, searchFolders);
            }

            if (targetAsset != null && duplicates is { Count: > 0 } && GUILayout.Button("Merge"))
            {
                List<int> duplicateIds = new(duplicates.Count);
                for (int i = duplicates.Count - 1; i >= 0; --i)
                {
                    if (duplicates[i] != null)
                    {
                        duplicateIds.Add(duplicates[i].GetInstanceID());
                    }
                }

                FolderPath rootPath = new("Assets");
                foreach (SceneAsset scene in FolderUtils.Find<SceneAsset>(rootPath, true))
                {
                    AssetsMerger.ReplaceDuplicatesIn(scene, duplicateIds, targetAsset, true);
                }

                AssetsMerger.ReplaceDuplicatesIn(FolderUtils.Find<Object>(rootPath, true), duplicateIds, targetAsset,
                    true);

                if (deleteReplacedAssets)
                {
                    DeleteMainAssets(duplicates);
                    duplicates.Clear();
                }

                AssetDatabase.SaveAssets();
            }
        }

        private void DrawAssetsInDirectoriesView()
        {
            SerializedObject windowSo = new(this);

            SerializedProperty targetFoldersProp = windowSo.FindProperty("targetFolders");
            SerializedProperty targetTypeProp = windowSo.FindProperty("targetType");
            SerializedProperty searchFoldersProp = windowSo.FindProperty("searchFolders");

            EditorGUILayout.PropertyField(targetFoldersProp, true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(targetTypeProp, true);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(searchFoldersProp, true);
            EditorGUILayout.Space();

            windowSo.ApplyModifiedPropertiesWithoutUndo();

            deleteReplacedAssets =
                EditorGUILayout.ToggleLeft("Delete Replaced Assets (Cannot be undone)", deleteReplacedAssets);
            EditorGUILayout.Space();

            if (GUILayout.Button("Merge all duplicates"))
            {
                Dictionary<int, int> replacementMap = null;
                List<Object> foundDuplicates = null;

                switch (targetType)
                {
                    /*
			case AssetType.All:
				replacementMap = CreateReplacementMap<Object>(targetFolders, searchFolders, null);
				break;

			case AssetType.AllExceptPrefabs:
				replacementMap = CreateReplacementMap<Object>(targetFolders, searchFolders, asset => !(asset is GameObject));
				break;
			*/
                    case AssetType.MATERIALS:
                        replacementMap =
                            CreateReplacementMap<Material>(targetFolders, searchFolders, null, out foundDuplicates);
                        break;

                    case AssetType.MESHES:
                        replacementMap =
                            CreateReplacementMap<Mesh>(targetFolders, searchFolders, null, out foundDuplicates);
                        break;

                    case AssetType.SPRITES:
                        replacementMap =
                            CreateReplacementMap<Sprite>(targetFolders, searchFolders, null, out foundDuplicates);
                        break;

                    case AssetType.TEXTURES:
                        replacementMap =
                            CreateReplacementMap<Texture>(targetFolders, searchFolders, null, out foundDuplicates);
                        break;
                }


                FolderPath rootPath = new("Assets");

                // replace in scenes
                foreach (SceneAsset scene in FolderUtils.Find<SceneAsset>(rootPath, true))
                {
                    AssetsMerger.ReplaceDuplicatesIn(scene, replacementMap, true);
                }

                // replace in assets (must be done after the scene replacements to avoid conflicts on prefab instances overriding array elements)
                AssetsMerger.ReplaceDuplicatesIn(FolderUtils.Find<Object>(rootPath, true), replacementMap, true);

                if (deleteReplacedAssets)
                {
                    DeleteMainAssets(foundDuplicates);
                }

                AssetDatabase.SaveAssets();
            }
        }

        private void DeleteMainAssets<T>(List<T> assets) where T : Object
        {
            for (int i = assets.Count - 1; i >= 0; --i)
            {
                T asset = assets[i];

                string path = asset != null ? AssetDatabase.GetAssetPath(asset) : null;
                if (!string.IsNullOrEmpty(path) && AssetDatabase.IsMainAsset(asset))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }

        private T GetWithLowestDepth<T>(T t1, T t2) where T : Object
        {
            string p1 = AssetDatabase.GetAssetPath(t1);
            if (string.IsNullOrEmpty(p1))
            {
                return t2;
            }

            string p2 = AssetDatabase.GetAssetPath(t2);
            if (string.IsNullOrEmpty(p2))
            {
                return t1;
            }

            int d1 = GetPathDepth(p1);
            int d2 = GetPathDepth(p2);

            if (d1 == d2)
            {
                return p1.Length < p2.Length ? t1 : t2;
            }

            return d1 < d2 ? t1 : t2;
        }

        private int GetPathDepth(string path)
        {
            int depth = 0;
            for (int i = path.Length - 1; i >= 0; --i)
            {
                if (path[i] == '/')
                {
                    ++depth;
                }
            }

            return depth;
        }

        private Dictionary<int, int> CreateReplacementMap<T>(FolderPath[] targetDirs, FolderPath[] searchDirs,
            Predicate<T> targetFilter, out List<Object> duplicatedItems) where T : Object
        {
            T[] candidates = FolderUtils.Find<T>(targetDirs, true);

            IEnumerable<T> targets = candidates;

            if (targetFilter != null)
            {
                targets = candidates.Where(e => targetFilter(e));
            }

            Dictionary<T, T> replacements =
                AssetsMerger.CreateDuplicatesReplacementMap(targets, searchDirs, GetWithLowestDepth);
            duplicatedItems = new List<Object>(replacements.Keys);
            Dictionary<int, int> results = new(replacements.Count);

            foreach (KeyValuePair<T, T> entry in replacements)
                results[entry.Key.GetInstanceID()] = entry.Value.GetInstanceID();

            return results;
        }
    }
}