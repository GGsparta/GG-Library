using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GGL.Editor.ReferencesTracker
{
    public static class ReferencesTracker
    {
        public static IEnumerable<Object> FindObjectsReferencing(Object target)
        {
            bool isAsset = !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target));

            int[] targetInstanceIds;

            if (target is GameObject gameObject)
            {
                Component[] components = gameObject.GetComponents<Component>().Where(c => c != null).ToArray();
                int count = components.Length;

                targetInstanceIds = new int[count + 1];
                for (int i = count - 1; i >= 0; --i)
                {
                    targetInstanceIds[i] = components[i].GetInstanceID();
                }
                targetInstanceIds[count] = gameObject.GetInstanceID();
            }
            else
            {
                targetInstanceIds = new[] { target.GetInstanceID() };
            }

            List<Object> results = new();
            FindObjectsReferencing(targetInstanceIds, Resources.FindObjectsOfTypeAll<Component>(), isAsset, ref results);
            if (isAsset)
            {
                FindObjectsReferencing(targetInstanceIds, Resources.FindObjectsOfTypeAll<ScriptableObject>(), true, ref results);
            }

            return results;
        }

        private static void FindObjectsReferencing<T>(
            int[] targetInstanceIds, 
            IEnumerable<T> candidates, 
            bool isAsset, 
            ref List<Object> results) where T : Object
        {
            foreach (T candidate in candidates)
            {
                if (candidate is Transform) { continue; }
                if (!isAsset && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(candidate))) { continue; }

                bool found = false;
                SerializedObject so;
                try
                {
                    so = new SerializedObject(candidate);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[{0}] Cannot create SerializedObject for '{1}'", nameof(ReferencesTracker), candidate);
                    Debug.LogException(e, candidate);
                    continue;
                }
                SerializedProperty prop = so.GetIterator();
                while (prop.Next(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                        targetInstanceIds.Contains(prop.objectReferenceInstanceIDValue) &&
                        !(candidate is Component && prop.propertyPath == "m_GameObject"))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    results.Add(candidate);
                }
            }
        }
    }
}