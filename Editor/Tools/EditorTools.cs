using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GGL.Tools
{
    public static class EditorTools
    {
        /// <summary>
        /// Proper way to create/edit scene objects while not playing.
        /// </summary>
        /// <param name="code">Your 'editing' piece of code</param>
        /// <param name="concernedObjects">Concerned components or scene objects. The ones that triggers changes and the ones that are being edited.</param>
        public static void Execute(Action code, params Object[] concernedObjects)
        {
            void NextUpdate()
            {
                EditorApplication.update -= NextUpdate;


                if (concernedObjects.Any(c => !c))
                {
                    // One of your objects has been destroyed and I have nothing to do with it!
                    // Why? Might be another edit or another scene.
                    return;
                }

                if (concernedObjects.All(c => !EditorUtility.IsDirty(c)))
                {
                    // No edit detected : let's return to bed
                    // Why? Might be the first OnValidate call when loading a scene.
                    return;
                }

                code?.Invoke();

                foreach (UnityEngine.Object component in concernedObjects)
                    EditorUtility.SetDirty(component);
            }

            EditorApplication.update += NextUpdate;
        }

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string name = typeof(T).Name;
            if (typeof(T).BaseType is { IsGenericType: true } baseType)
                name = baseType.GetGenericArguments().First().Name;
            AssetDatabase.CreateAsset(asset, $"{GetSelectedPath()}/{name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        public static string GetSelectedPath()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }
    }
}