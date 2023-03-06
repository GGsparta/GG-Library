using System;
using Newtonsoft.Json;

namespace GGL.DB.Reference
{
    /// <summary>
    /// A reference to another <see cref="Data"/> that might be in <see cref="Database"/>.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    [Serializable] [JsonConverter(typeof(ReferenceConverter))] 
    public class Reference<T> : Reference where T : Data
    {
        /// <value>
        /// Access the cached data value.
        /// </value>
        /// <remarks>If not cached yet, will cache it for you :) .</remarks>
        [JsonIgnore] public T Value => _value ??= TrySelect();
        [NonSerialized] private T _value;

        /// <summary>
        /// Create an empty reference.
        /// </summary>
        public Reference() {}

        /// <summary>
        /// Create a reference to the data by its ID.
        /// </summary>
        /// <param name="id">Id of the data in database.</param>
        /// <remarks>That means that your data was previously added in database. You did, right?</remarks>
        public Reference(ulong id)
        {
            ContraintID = id;
            _value = TrySelect();
        }

        /// <summary>
        /// Create a reference to a data
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>The data doesn't not need to be in database yet. This won't insert the data in database.</remarks>
        public Reference(T value)
        {
            _value = value;
            ContraintID = value.Id;
            value.OnRebase += () => ContraintID = value.Id;
        }

        /// <summary>
        /// Returns directly the cached value of the reference.
        /// </summary>
        /// <param name="current">this.</param>
        /// <returns>Cached value of the reference.</returns>
        /// <remarks>If not cached yet, will cache it for you :) .</remarks>
        public static implicit operator T(Reference<T> current) => current.Value;

        private T TrySelect()
        {
            T value = Database.Select<T>(ContraintID);
            if (value != null)
                value.OnRebase += () => ContraintID = value.Id;
            return value;
        }
    }
    
    /// <summary>
    /// Base type for any <see cref="Reference{T}"/>.
    /// </summary>
    public abstract class Reference
    {
        /// <summary>
        /// ID used when requesting data from database.
        /// </summary>
        public ulong ContraintID { get; protected set; }
    }
    
    /// <summary>
    /// Rules for reference serialization (keep id only)
    /// </summary>
    public class ReferenceConverter : JsonConverter<Reference>
    {
        public override void WriteJson(JsonWriter writer, Reference value, JsonSerializer serializer) 
            => writer.WriteValue(value.ContraintID);

        public override Reference ReadJson(JsonReader reader, System.Type objectType, Reference existingValue,
            bool hasExistingValue, JsonSerializer serializer) => 
            objectType.GetConstructor(new[] { typeof(ulong) }).Invoke(new object[] { Convert.ToUInt64(reader.Value) }) as Reference;
    }
}