using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GGL.DB
{
    /// <summary>
    /// Base type for any <see cref="Table{T}"/> used by <see cref="Database"/>
    /// </summary>
    public abstract class Table
    {
        /// <summary>
        /// JSON resolver for non-public properties
        /// </summary>
        private class NonPublicPropertiesResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty prop = base.CreateProperty(member, memberSerialization);
                if (member is PropertyInfo pi)
                {
                    prop.Readable = pi.GetMethod != null;
                    prop.Writable = pi.SetMethod != null;
                }

                return prop;
            }
        }

        /// <value>
        /// Table serialisation settings
        /// </value>
        protected static JsonSerializerSettings jsonSettings;

        protected Table()
        {
            if (jsonSettings != null) return;
            jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new NonPublicPropertiesResolver()
            };
        }
        
        /// <summary>
        /// Write the content of the table to local storage.
        /// </summary>
        public abstract void Save();
        
        /// <summary>
        /// Restore in cache the data from the local storage.
        /// </summary>
        public abstract void Load();
    }

    /// <summary>
    /// Typed table used by <see cref="Database"/>.
    /// </summary>
    /// <typeparam name="T">Type that extends <see cref="Data"/>. This will be the type of the data contained and serialized by the table.</typeparam>
    public class Table<T> : Table where T : Data
    {
        private HashSet<T> _data;
        private ulong _currentID;
        
        /// <value>
        /// Path of the saved table in local storage.
        /// </value>
        protected static string Path => $"{Application.persistentDataPath}/{typeof(T).Name}.json";

        public Table(params T[] baseData)
        {
            _data = new HashSet<T>();
            _currentID = 1;
            Insert(baseData);
        }

        /// <summary>
        /// Insert data in table.
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>IDs may change while inserting.</remarks>
        public void Insert(params T[] data)
        {
            foreach (T item in data)
            {
                // Check possible bad ID before adding (ID conflict may overwrite old values)
                if (item.Id != default)
                {
                    if (_data.All(e => e.Id != item.Id))
                        _data.Add(item);
                    continue;
                }

                // Setup fitting ID
                while (_data.Any(d => d.Id == _currentID)) ++_currentID;

                // Illegally set the ID of the Data
                System.Type t = typeof(Data);
                if (t.GetProperty(nameof(item.Id),
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                    throw new ArgumentOutOfRangeException(nameof(item.Id),
                        $"Property {nameof(item.Id)} was not found in Type {typeof(T).FullName}");
                t.GetMethod("Rebase_", BindingFlags.NonPublic | BindingFlags.Instance)?
                    .Invoke(item, new object[] { _currentID });

                // Add the object
                _data.Add(item);
            }
        }

        /// <summary>
        /// Select an item by ID.
        /// </summary>
        /// <param name="id">row ID</param>
        /// <returns>Typed data</returns>
        public T Select(ulong id) => _data.FirstOrDefault(d => d.Id == id);

        /// <summary>
        /// Select a list of items. No params will return all data.
        /// </summary>
        /// <param name="ids">row IDs</param>
        /// <returns>Typed data</returns>
        public IEnumerable<T> Select(params ulong[] ids)
            => ids.Length == 0 ? _data.ToList() : _data.Where(d => ids.Contains(d.Id));

        /// <summary>
        /// Write the content of the table to local storage.
        /// </summary>
        public override void Save() => File.WriteAllText(Path, JsonConvert.SerializeObject(_data, Database.FORMAT));

        /// <summary>
        /// Restore in cache the data from the local storage.
        /// </summary>
        public override void Load()
        {
            try
            {
                string content = File.ReadAllText(Path);
                HashSet<T> data = JsonConvert.DeserializeObject<HashSet<T>>(content, jsonSettings);
                _data = data;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogWarning("Tried to access an non-existing database: " + e);
                _data ??= new HashSet<T>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}