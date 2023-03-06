using System.Collections.Generic;
using System.Linq;
using GGL.Singleton;
using UnityEditor;

namespace GGL.Editor.SingletonLoader
{
    public static class SingletonLoader
    {
        public static IEnumerable<SingletonBehaviour> SearchSingletonPrefabs() => AssetDatabase
            .FindAssets("a:assets t:prefab")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SingletonBehaviour>)
            .Where(script => script);
    }
}