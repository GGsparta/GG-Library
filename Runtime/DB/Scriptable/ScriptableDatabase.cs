using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGL.DB.Scriptable
{
    /// <summary>
    /// Similar to the <see cref="Database"/> but it maintains cohesion within the `Resources` folder.
    /// </summary>
    [HelpURL(Settings.ARTICLE_URL + "database" + Settings.COMMON_EXT + "#définition-des-données-par-défaut")]
    public class ScriptableDatabase : Singleton.ScriptableSingleton<ScriptableDatabase>
    {
        [Serializable]
        private class Table
        {
            // ReSharper disable once NotAccessedField.Local
            public string table;
            public System.Type type;
            public List<ScriptableData> datas;
        }
        

        [SerializeField] [ReadOnly] private List<Table> db;

        
        private void InitDBIfNeeded()
        {
            if(db != null) return;
            db = new List<Table>();
            ScriptableData[] items = Resources.LoadAll<ScriptableData>(string.Empty);
            foreach (ScriptableData data in items)
            {
                if(!data.GetType().BaseType!.IsGenericType)
                {
                    Debug.LogError("Unexpected non generic type");
                    continue;
                }

                System.Type dataType = data.GetType().BaseType!.GetGenericArguments().First();
                Table table;
                if((table = db.FirstOrDefault(t => t.type == dataType)) == null) db.Add(table = new Table {table = dataType.Name, type = dataType, datas = new List<ScriptableData>()});
                if(!table.datas.Contains(data)) table.datas.Add(data);
            }
        }

        /// <summary>
        /// Access table by scriptable data <see cref="System.Type"/>
        /// </summary>
        /// <param name="type"></param>
        public IEnumerable<ScriptableData> this[System.Type type]
        {
            get
            {
                Table table = db.FirstOrDefault(t => t.type == type); 
                return table != null ? table.datas :  Array.Empty<ScriptableData>();
            }
        }

        public void RegisterIfNeeded<T>(ScriptableData<T> data) where T : class
        {
            // Create temp DB
            if (db.All(t => t.type != typeof(T))) db = null;
            InitDBIfNeeded();

            // Update if new item / removed item since DB creation
            Table table = db!.FirstOrDefault(t => t.type == typeof(T));
            if(table == null) return;
            table.datas.RemoveAll(item => item == null);
            if(!table.datas.Contains(data)) table.datas.Add(data);
            
            // Ensure unique id
            if(table.datas.Count(item => item.id == data.id) == 1) return;
            while (table.datas.Count(item => item.id == data.id) != 1) ++data.id;
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif
        }

        /// <summary>
        /// Delete a scriptable data from scriptable database
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void Delete<T>(ScriptableData<T> data) where T : class
        {
            Table table;
            if ((table = db.FirstOrDefault(t => t.type == typeof(T))) == null || !table.datas.Contains(data)) return;
            table.datas.Remove(data);
            if (table.datas.Count == 0) db.Remove(table);
        }

        /// <inheritdoc />
        protected override void Init() => InitDBIfNeeded();
    }
}