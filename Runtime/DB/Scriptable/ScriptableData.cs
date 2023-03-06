using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGL.DB.Scriptable
{
    /// <summary>
    /// Base class for every <see cref="ScriptableData{T}"/>.
    /// </summary>
    [HelpURL(Settings.ARTICLE_URL + "database" + Settings.COMMON_EXT + "#définition-des-données-par-défaut")]
    public abstract class ScriptableData : ScriptableObject
    {
        /// <value>
        /// Contextual menu base path suggested for the item generation.
        /// </value>
        protected const string MENU_CREATE = "Assets/Create/Database/";

        /// <value>
        /// ID of the future data.
        /// </value>
        /// <remarks>It is updated automatically by <see cref="ScriptableDatabase"/></remarks>
        [ReadOnly]
        public ulong id = 1;


        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnValidate() => UpdateInDB();

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void Awake() => UpdateInDB();

        /// <summary>
        /// Register itself in scriptable database.
        /// </summary>
        protected abstract void UpdateInDB();

        /// <summary>
        /// Delete itself in scriptable database.
        /// </summary>
        public abstract void DeleteFromDB();
    }
    
    
    /// <summary>
    /// Base <see cref="ScriptableObject"/> for every default <see cref="Data"/> you want to configure in <see cref="Database"/>.
    /// </summary>
    /// <remarks>Check <a href="https://ggl.yoannhaffner.com/articles/database.html#définition-des-données-par-défaut">this guide</a> to see how you can properly inherit this class.</remarks>
    public abstract class ScriptableData<T> : ScriptableData where T : class
    {
        /// <summary>
        /// Create a data from default configuration.
        /// </summary>
        /// <returns></returns>
        public abstract T Load();

        /// <inheritdoc />
        protected override void UpdateInDB() => ScriptableDatabase.Instance.RegisterIfNeeded(this);
        
        /// <inheritdoc />
        public override void DeleteFromDB() => ScriptableDatabase.Instance.Delete(this);
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Automatically delete any removed <see cref="ScriptableData"/> from the <see cref="ScriptableDatabase"/>.
    /// </summary>
    public class ScriptableDataDeleteDetector : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
        {
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (t != null && t.IsSubclassOf(typeof(ScriptableData)))
                AssetDatabase.LoadAssetAtPath<ScriptableData>(path).DeleteFromDB();
            return AssetDeleteResult.DidNotDelete;
        }
    }
#endif
}