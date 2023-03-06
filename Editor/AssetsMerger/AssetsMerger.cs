using System;
using System.Collections.Generic;
using System.Linq;
using GGL.Editor.Tools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GGL.Editor.AssetsMerger
{
    public static class AssetsMerger
    {
        // Methods to treat a single target asset
        public static List<Object> FindPotentialDuplicates(Object targetAsset,
            FolderPath[] searchFolders)
        {
            string targetName = targetAsset.name;
            string targetExtension = GetExtension(AssetDatabase.GetAssetPath(targetAsset));

            IEnumerable<Object> assets = FolderUtils.Find(searchFolders, targetAsset.GetType(), true);

            return (from asset in assets
                where asset.name == targetName && asset != targetAsset
                let path = AssetDatabase.GetAssetPath(asset)
                where !string.IsNullOrEmpty(path) && GetExtension(path) == targetExtension
                select asset).ToList();
        }

        private static bool ShouldForcePropertyOverrideFor(SerializedObject so)
        {
            switch (PrefabUtility.GetPrefabInstanceStatus(so.targetObject))
            {
                case PrefabInstanceStatus.Connected:
                    return false;
                default:
                    return true;
            }
        }

        private static bool ReplaceDuplicatesIn(SerializedObject so, ICollection<int> duplicateIds,
            Object targetAsset)
        {
            bool forceOverride = ShouldForcePropertyOverrideFor(so);
            bool changed = false;

            IEnumerator<SerializedProperty> propIt = EnumeratePropertiesOf(so);
            while (propIt.MoveNext())
            {
                SerializedProperty prop = propIt.Current;
                if (prop == null || prop.propertyType != SerializedPropertyType.ObjectReference ||
                    !duplicateIds.Contains(prop.objectReferenceInstanceIDValue) ||
                    !forceOverride && !prop.prefabOverride && prop.isInstantiatedPrefab) continue;
                prop.objectReferenceValue = targetAsset;
                changed = true;
            }

            if (changed)
            {
                so.ApplyModifiedProperties();
            }

            return changed;
        }

        public static bool ReplaceDuplicatesIn(IEnumerable<Object> objects, List<int> duplicateIds,
            Object targetAsset, bool replaceInChildrens)
        {
            return ReplaceDuplicatesIn(objects, so => ReplaceDuplicatesIn(so, duplicateIds, targetAsset),
                replaceInChildrens);
        }

        public static bool ReplaceDuplicatesIn(GameObject go, List<int> duplicateIds,
            Object targetAsset, bool replaceInChildrens)
        {
            return ReplaceDuplicatesIn(go, so => ReplaceDuplicatesIn(so, duplicateIds, targetAsset),
                replaceInChildrens);
        }

        public static bool ReplaceDuplicatesIn(SceneAsset scene, List<int> duplicateIds, Object targetAsset,
            bool saveSceneIfAlreadyLoaded)
        {
            return ReplaceDuplicatesIn(scene, so => ReplaceDuplicatesIn(so, duplicateIds, targetAsset),
                saveSceneIfAlreadyLoaded);
        }

        // Methods to treat a collection of assets

        public static Dictionary<T, List<T>> FindPotentialDuplicates<T>(IEnumerable<T> targetAssets,
            FolderPath[] searchFolders) where T : Object
        {
            Dictionary<string, List<T>> targetNames = new();
            Dictionary<T, List<T>> results = new();

            foreach (T targetAsset in targetAssets)
            {
                if (results.ContainsKey(targetAsset))
                {
                    continue;
                }

                results.Add(targetAsset, new List<T>(1));

                string targetName = targetAsset.name;
                if (!targetNames.TryGetValue(targetName, out List<T> list))
                {
                    targetNames.Add(targetName, list = new List<T>(1));
                }

                list.Add(targetAsset);
            }

            T[] assets = FolderUtils.Find<T>(searchFolders, true);

            foreach (T asset in assets)
            {
                if (!targetNames.TryGetValue(asset.name, out List<T> targets) || targets.Contains(asset)) continue;
                string assetExtension = GetExtension(asset);
                if (assetExtension == null)
                {
                    continue;
                }

                for (int i = targets.Count - 1; i >= 0; --i)
                {
                    T target = targets[i];
                    if (target.GetType() == asset.GetType() && GetExtension(target) == assetExtension)
                    {
                        results[target].Add(asset);
                    }
                }
            }

            return results;
        }

        public static Dictionary<T, T> CreateDuplicatesReplacementMap<T>(IEnumerable<T> targetAssets,
            FolderPath[] searchFolders, Func<T, T, T> getBestTarget) where T : Object
        {
            Dictionary<string, Dictionary<string, T>>
                targetNamesByExt = new();

            foreach (T targetAsset in targetAssets)
            {
                string targetName = targetAsset.name;
                string targetExt = GetExtension(targetAsset) ?? "";

                if (!targetNamesByExt.TryGetValue(targetExt, out Dictionary<string, T> targetNames))
                {
                    targetNames = new Dictionary<string, T>();
                    targetNamesByExt.Add(targetExt, targetNames);
                }

                if (targetNames.TryGetValue(targetName, out T otherTarget))
                {
                    targetNames[targetName] = getBestTarget(targetAsset, otherTarget);
                }
                else
                {
                    targetNames[targetName] = targetAsset;
                }
            }

            Dictionary<T, T> results = new();
            T[] assets = FolderUtils.Find<T>(searchFolders, true);

            foreach (T asset in assets)
            {
                string assetExt = GetExtension(asset);
                if (assetExt == null)
                {
                    continue;
                }

                if (!targetNamesByExt.TryGetValue(assetExt, out Dictionary<string, T> targetNames))
                {
                    continue;
                }

                if (targetNames.TryGetValue(asset.name, out T target) &&
                    target != asset &&
                    target.GetType() == asset.GetType())
                {
                    results[asset] = target;
                }
            }

            return results;
        }

        private static bool ReplaceDuplicatesIn(SerializedObject so, Dictionary<int, int> referencesToReplace)
        {
            bool forceOverride = ShouldForcePropertyOverrideFor(so);
            bool changed = false;

            IEnumerator<SerializedProperty>
                propIt = EnumeratePropertiesOf(so);
            while (propIt.MoveNext())
            {
                SerializedProperty prop = propIt.Current;
                if (prop == null || prop.propertyType != SerializedPropertyType.ObjectReference ||
                    !referencesToReplace.TryGetValue(prop.objectReferenceInstanceIDValue, out int newRef) ||
                    !forceOverride && !prop.prefabOverride && prop.isInstantiatedPrefab) continue;
                prop.objectReferenceInstanceIDValue = newRef;
                changed = true;
            }

            if (changed)
            {
                so.ApplyModifiedProperties();
            }

            return changed;
        }

        public static bool ReplaceDuplicatesIn(IEnumerable<Object> objects,
            Dictionary<int, int> referencesToReplace, bool replaceInChildrens)
        {
            return ReplaceDuplicatesIn(objects, so => ReplaceDuplicatesIn(so, referencesToReplace), replaceInChildrens);
        }

        public static bool ReplaceDuplicatesIn(GameObject go, Dictionary<int, int> referencesToReplace,
            bool replaceInChildrens)
        {
            return ReplaceDuplicatesIn(go, so => ReplaceDuplicatesIn(so, referencesToReplace), replaceInChildrens);
        }

        public static bool ReplaceDuplicatesIn(SceneAsset scene, Dictionary<int, int> referencesToReplace,
            bool saveSceneIfAlreadyLoaded)
        {
            return ReplaceDuplicatesIn(scene, so => ReplaceDuplicatesIn(so, referencesToReplace),
                saveSceneIfAlreadyLoaded);
        }

        // Private methods

        private static bool ReplaceDuplicatesIn(IEnumerable<Object> objects,
            Func<SerializedObject, bool> replaceInSerializedObject, bool replaceInChildrens)
        {
            bool changed = false;

            int count = 0;
            foreach (Object obj in objects)
            {
                ++count;

                if (obj is GameObject go)
                {
                    changed |= ReplaceDuplicatesIn(go, replaceInSerializedObject, replaceInChildrens);
                }
                else
                {
                    changed |= replaceInSerializedObject(new SerializedObject(obj));
                }

                if (count < 50) continue;

                GC.Collect();
                count = 0;
            }

            return changed;
        }

        private static bool ReplaceDuplicatesIn(GameObject go,
            Func<SerializedObject, bool> replaceInSerializedObject,
            bool replaceInChildrens)
        {
            bool changed = replaceInSerializedObject(new SerializedObject(go));

            foreach (Component component in go.GetComponents<Component>())
            {
                try
                {
                    if (component != null) // Surprinsigly, it sometimes happens that the returned component is null
                    {
                        changed |= replaceInSerializedObject(new SerializedObject(component));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "[AssetsMergingTools] Exception occured while serializing component '" + component +
                        "' of GameObject '" + go + "'", go);
                    Debug.LogException(e, go);
                }
            }

            if (!replaceInChildrens) return changed;
            Transform t = go.transform;
            for (int i = t.childCount - 1; i >= 0; --i)
            {
                changed |= ReplaceDuplicatesIn(t.GetChild(i).gameObject, replaceInSerializedObject, true);
            }

            return changed;
        }

        private static bool ReplaceDuplicatesIn(SceneAsset scene,
            Func<SerializedObject, bool> replaceInSerializedObject, bool saveSceneIfAlreadyLoaded)
        {
            return ExecuteOnSceneRootGameObjects(scene,
                rootGos => ReplaceDuplicatesIn(rootGos, replaceInSerializedObject, true), saveSceneIfAlreadyLoaded);
        }

        private static IEnumerator<SerializedProperty> EnumeratePropertiesOf(SerializedObject so)
        {
            SerializedProperty prop = so.GetIterator();
            while (prop.Next(true))
            {
                yield return prop;
            }
        }

        private static IEnumerator<SerializedProperty> EnumeratePropertiesOf(SerializedObject so,
            Predicate<SerializedProperty> enumerateChildrenCondition)
        {
            SerializedProperty prop = so.GetIterator();
            if (!prop.Next(true)) yield break;
            do
            {
                yield return prop;
            } while (prop.Next(prop.hasChildren && enumerateChildrenCondition(prop)));
        }

        private static string GetExtension(string path)
        {
            int dot = path.LastIndexOf('.');
            return dot > 0 ? path.Substring(dot) : "";
        }

        private static string GetExtension(Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return string.IsNullOrEmpty(path) ? GetExtension(path) : null;
        }

        private static bool CheckExtension(Object asset1, Object asset2)
        {
            string p1 = AssetDatabase.GetAssetPath(asset1);
            if (string.IsNullOrEmpty(p1))
            {
                return false;
            }

            string p2 = AssetDatabase.GetAssetPath(asset2);
            return !(string.IsNullOrEmpty(p2) || GetExtension(p1) != GetExtension(p2));
        }

        // action will receive the scene's root gameobjects and must return wether the scene changed or not
        public static bool ExecuteOnSceneRootGameObjects(SceneAsset sceneAsset,
            Func<GameObject[], bool> action, bool saveSceneIfAlreadyLoaded)
        {
            bool sceneWasLoaded = false;
            Scene scene = default(Scene);
            GameObject[] rootGameObjects = null;

            if (sceneAsset != null)
            {
                scene = SceneManager.GetSceneByName(sceneAsset.name);
                if (!scene.isLoaded)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset), OpenSceneMode.Additive);
                    scene = SceneManager.GetSceneByName(sceneAsset.name);
                    sceneWasLoaded = true;
                }

                if (scene.isLoaded)
                {
                    rootGameObjects = scene.GetRootGameObjects();
                }
                else
                {
                    Debug.LogError("Unable to load scene " + sceneAsset.name, sceneAsset);
                    return false;
                }
            }

            bool changed = action(rootGameObjects);

            if (changed && (sceneWasLoaded || saveSceneIfAlreadyLoaded))
            {
                EditorSceneManager.SaveScene(scene);
            }

            if (!sceneWasLoaded) return changed;

            EditorSceneManager.CloseScene(scene, true);
            EditorUtility.UnloadUnusedAssetsImmediate();
            GC.Collect();

            return changed;
        }
    }
}