using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace GGL.Editor.ReferencesTracker
{
    public class ReferencesTrackerWindow : BaseWindow<ReferencesTrackerWindow>
    {
        private Object _target;
        private bool _kill, _autoClean;
        private EditorCoroutine _searching;
        private List<Object> _references = new();
        private List<Object> _notUsedAssets = new();
        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("Tools/GG-Library/Track references", false, 30)]
        public static void Open() => Open("References Tracker");


        protected override void Render(Vector2 pos, Vector2 size)
        {
            _target = EditorGUILayout.ObjectField("Target reference :", _target, typeof(Object), true);

            if (_target == null)
            {
                return;
            }

            if (GUILayout.Button(_searching == null
                    ? $"Find {(_target is DefaultAsset ? "unused assets in folder" : "objects referencing asset")} '{_target.name}'"
                    : "Kill"))
            {
                if (_searching != null)
                {
                    _kill = true;
                }
                else if (_target is DefaultAsset)
                {
                    _kill = false;
                    _notUsedAssets.Clear();
                    _searching = EditorCoroutineUtility.StartCoroutine(FindUnusedAssets(), this);
                }
                else
                {
                    _references.Clear();
                    _notUsedAssets.Clear();
                    _references = ReferencesTracker.FindObjectsReferencing(_target).ToList();
                }
            }

            _autoClean = _target is DefaultAsset && GUILayout.Toggle(_autoClean, "Autoclean (discouraged)");

            if (_references is { Count: > 0 })
            {
                GUILayout.Label($"{_target.name} is referenced by:", EditorStyles.boldLabel);

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                EditorGUI.BeginDisabledGroup(true);
                foreach (Object reference in _references)
                {
                    EditorGUILayout.ObjectField(reference, typeof(Object), true);
                }

                EditorGUI.EndDisabledGroup();
                GUILayout.Space(5);
                EditorGUILayout.EndScrollView();
            }

            if (_notUsedAssets is not { Count: > 0 }) return;

            GUILayout.Label(
                "The following assets are referenced in the current scene(s) and project assets.\nBefore removing please check other code references.",
                EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            foreach (Object reference in _notUsedAssets)
                EditorGUILayout.ObjectField(reference, typeof(Object), true);

            EditorGUI.EndDisabledGroup();
        }


        public IEnumerator FindUnusedAssets()
        {
            DefaultAsset rootFolder = _target as DefaultAsset;
            string[] assets = AssetDatabase.FindAssets(string.Empty, new[] { AssetDatabase.GetAssetPath(rootFolder) });
            yield return null;
            if (_kill)
            {
                _searching = null;
                yield break;
            }

            Dictionary<Object, string> assetObjects = assets
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.Contains('.'))
                .ToDictionary(AssetDatabase.LoadMainAssetAtPath);


            foreach (Object obj in assetObjects.Keys)
            {
                yield return null;
                if (_kill)
                {
                    _searching = null;
                    yield break;
                }

                _references = ReferencesTracker.FindObjectsReferencing(obj).ToList();
                _references.Clear();

                if (_references.Count == 0) // Not used
                {
                    if (_autoClean)
                    {
                        Debug.LogWarning($"Removed {obj.name}");
                        AssetDatabase.DeleteAsset(assetObjects[obj]);
                        _notUsedAssets.Add(null);
                    }
                    else
                    {
                        _notUsedAssets.Add(obj);
                    }
                }
            }

            Debug.LogWarning($"Found {_notUsedAssets.Count} unused assets! They were {(_autoClean ? "ALL" : "not")} removed.");
        }
    }
}