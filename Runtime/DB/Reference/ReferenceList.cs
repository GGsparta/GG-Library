using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace GGL.DB.Reference
{
    /// <summary>
    /// List of <see cref="Reference{T}"/>.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    [Serializable]
    public sealed class ReferenceList<T> : ObservableCollection<Reference<T>> where T : Data
    {
        /// <value>
        /// Access the cached list of datas.
        /// </value>
        /// <remarks>If not cached yet, will cache it for you :) .</remarks>
        [JsonIgnore] public List<T> Values => _values ??= this.Select(e => (T)e).ToList();
        [NonSerialized] private List<T> _values;

        /// <inheritdoc/>
        public ReferenceList() 
            => CollectionChanged += (_,_) => _values = null;
        /// <inheritdoc/>
        public ReferenceList(params ulong[] ids) : base(ids.Select(id => new Reference<T>(id)))
            => CollectionChanged += (_,_) => _values = null;
        /// <inheritdoc/>
        public ReferenceList(params T[] items) : base(items.Select(item => new Reference<T>(item)))
            => CollectionChanged += (_,_) => _values = null;
        
        public static implicit operator List<T>(ReferenceList<T> current) => current.Values;
        public static implicit operator T[](ReferenceList<T> current) => current.Values.ToArray();

        /// <summary>
        /// Sorts the elements or a portion of the elements in the <see cref="ReferenceList{T}"/> using either the
        /// specified or default <see cref="IComparer{T}"/> implementation or a provided <see cref="Comparison{T}"/>
        /// delegate to compare list elements.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<Reference<T>> comparer)
        {
            List<Reference<T>> sorted = this.OrderBy(x => x, comparer).ToList();
            for (int i = 0; i < sorted.Count; i++)
                Move(IndexOf(sorted[i]), i);
        }

        /// <summary>
        /// Remove the first item corresponding to ID
        /// </summary>
        /// <param name="id"></param>
        public void Remove(ulong id) => base.Remove(this.First(e => e.ContraintID == id));
    }
}