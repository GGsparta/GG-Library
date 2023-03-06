using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GGL.DB.Scriptable;
using GGL.Singleton;
using Newtonsoft.Json;
using UnityEngine;

namespace GGL.DB
{
    /// <summary>
    /// Database Singleton that allows to:
    /// - Store into <see cref="Table"/> and return any object extending <see cref="Data"/>;
    /// - Save them into local storage;
    /// - Supports abstract classes to select several types at once (e.g. inheritance).
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Singletons/Database")]
    [HelpURL(Settings.ARTICLE_URL + "database" + Settings.COMMON_EXT)]
    public class Database : SingletonBehaviour<Database>
    {
        /// <value>
        /// JSON Format used while serializing data.
        /// </value>
        public const Formatting FORMAT = Formatting.Indented;

        // Cached DB
        private Dictionary<System.Type, Table> _db = new();
        
        /// <inheritdoc />
        [ExcludeFromDocFx]
        protected override void Awake()
        {
            base.Awake();
            if (PlayerPrefs.HasKey(nameof(Database)))
            {
                List<System.Type> list = JsonConvert.DeserializeObject<List<System.Type>>(PlayerPrefs.GetString(nameof(Database)));
                foreach (System.Type type in list)
                    GetType()
                        .GetMethod(nameof(CheckTable), BindingFlags.NonPublic | BindingFlags.Instance)!
                        .MakeGenericMethod(type)
                        .Invoke(this, null);
                return;
            }

            ResetDB();
        }

        /// <summary>
        /// Reinitialize both cached and local database with default scriptable values.
        /// </summary>
        public static void ResetDB()
        {
            // Clear
            Instance._db.Clear();
            foreach (string file in Directory.GetFiles(Application.persistentDataPath, "*")) 
                File.Delete(file);


            // Recreate from template in Resources folder
            ScriptableData[] data = Resources.LoadAll<ScriptableData>(string.Empty);
            foreach (ScriptableData item in data)
            {
                // Extract data
                System.Type itemType = item.GetType().BaseType?.GetGenericArguments().First();
                object itemData = item.GetType()
                    .GetMethod("Load")?
                    .Invoke(item, null);

                // Encapsulate data
                Array array = Array.CreateInstance(itemType!, 1);
                array.SetValue(itemData, 0);
                
                // Add to DB
                Instance.GetType()
                    .GetMethod(nameof(Insert))?
                    .MakeGenericMethod(itemType)
                    .Invoke(Instance, new object[] {array});
            }
            
            SaveDB();
        }

        /// <summary>
        /// Write cached database in local storage.
        /// </summary>
        public static void SaveDB()
        {
            foreach ((System.Type _, Table table) in Instance._db) 
                table.Save();
            PlayerPrefs.SetString(nameof(Database), JsonConvert.SerializeObject(Instance._db.Keys.ToArray(), Database.FORMAT));
        }
        

        /// <summary>
        /// Insert a list of items in the concerned table.
        /// </summary>
        /// <param name="data">Data to insert in database.</param>
        /// <typeparam name="T">Type of the data to insert. Used as a table selector - which can be several tables if the type is abstract.</typeparam>
        public static void Insert<T>(params T[] data) where T : Data
        {
            // Check abstract type
            if (typeof(T).IsAbstract)
            {
                foreach (T item in data)
                {
                    if(item.GetType().IsAbstract)
                    {
                        Debug.LogError($"Abstract item {item} failed to be inserted. Abstract classes are not yet supported.");
                        continue;
                    }

                    Instance.GetType()
                        .GetMethod(nameof(Insert))?
                        .MakeGenericMethod(item.GetType())
                        .Invoke(Instance, new object[] {item});
                }
                
                return;
            }

            if(!Instance._db.ContainsKey(typeof(T)))
                Instance._db.Add(typeof(T), new Table<T>(data));
            else
                (Instance._db[typeof(T)] as Table<T>)?.Insert(data);
        }

        /// <summary>
        /// Select an item from the concerned table.
        /// </summary>
        /// <param name="id">ID of the data you're looking for.</param>
        /// <typeparam name="T">Type of the data to find. Used as a table selector - which can be several tables if the type is abstract.</typeparam>
        /// <returns></returns>
        public static T Select<T>(ulong id) where T : Data
        {
            // Check if is abstract class
            if (typeof(T).IsAbstract)
            {
                IEnumerable<System.Type> types = Instance._db.Keys.Where(t => t.IsSubclassOf(typeof(T)));
                
                // LINQ here would not be optimal as it would search every matching item before returning the first one
                // It is faster to return a value once it is not null
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (System.Type type in types)
                {
                    T result = (T)
                        Instance.GetType()
                            .GetMethod(nameof(Select), new []{typeof(ulong)})?
                            .MakeGenericMethod(type)
                            .Invoke(Instance, new object[] {id});
                    if (result != null)
                        return result;
                }

                return null;
            }
            
            // Check table existence
            Instance.CheckTable<T>();
            
            // Return result
            return (Instance._db[typeof(T)] as Table<T>)?.Select(id);
        }


        /// <summary>
        /// Select a list of items. No params will return all data concerned table.
        /// </summary>
        /// <param name="ids">IDs of the data you're looking for.</param>
        /// <typeparam name="T">Type of the data to find. Used as a table selector - which can be several tables if the type is abstract.</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(params ulong[] ids) where T : Data
        {
            // Check if is abstract class
            if (typeof(T).IsAbstract)
            {
                // Select each known subclass
                return Instance._db.Keys
                    .Where(type => type.IsSubclassOf(typeof(T)))
                    .SelectMany(type => new List<T>((Instance.GetType()
                        .GetMethod(nameof(Select), new[] { typeof(ulong[]) })!
                        .MakeGenericMethod(type)
                        .Invoke(Instance, new object[] { ids }) as IEnumerable<T>)!));
            }
            
            // Check table existence
            Instance.CheckTable<T>();
            
            // Return results
            return (Instance._db[typeof(T)] as Table<T>)?.Select(ids);
        }

        /// <summary>
        /// Clear a table corresponding to type if it exists in database.
        /// </summary>
        /// <typeparam name="T">Type of the data to clear.</typeparam>
        public static void Clear<T>() where T : Data
        {
            if (!Instance._db.ContainsKey(typeof(T))) return;
            Instance._db.Remove(typeof(T));
        }
        
        /// <summary>Create and load a table if it does not exist yet.</summary>
        private void CheckTable<T>() where T : Data
        {
            if (_db.ContainsKey(typeof(T))) return;
            Table table = (Table)Activator.CreateInstance(typeof(Table<>).MakeGenericType(typeof(T)), Array.Empty<object>());
            _db.Add(typeof(T), table);
            table.Load();
        }
    }
}