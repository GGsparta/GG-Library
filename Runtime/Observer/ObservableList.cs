using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using GGL.Extensions;

namespace GGL.Observer
{
    /// <summary>
    /// <see cref="IList{T}"/> where collection (and values eventually) changes are listenable.
    /// </summary>
    /// <typeparam name="T">Class or basic type.</typeparam>
    public class ObservableList<T> : IList<Observable<T>>
    {
        /// <value>
        /// Event raised if collection is edited.
        /// </value>
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event Action OnChange;

        private List<Observable<T>> _value;
        private readonly bool _observeItem;
        
        /// <summary>
        /// Create an observable empty list.
        /// </summary>
        /// <param name="observeItem">Decide if values specific changes are listened too.</param>
        public ObservableList(bool observeItem = true)
        {
            _value = new List<Observable<T>>();
            _observeItem = observeItem;
        }
        
        /// <summary>
        /// Create an observable list from another.
        /// </summary>
        /// <param name="collection">Well... the starting list I guess?</param>
        /// <param name="observeItem">Decide if values specific changes are listened too.</param>
        public ObservableList([System.Diagnostics.CodeAnalysis.NotNull] IEnumerable<Observable<T>> collection, bool observeItem = true)
        {
            _value = new List<Observable<T>>(collection);
            _observeItem = observeItem;
        }
        
        /// <summary>
        /// Create an observable list with capacity.
        /// </summary>
        /// <param name="capacity">List capacity.</param>
        /// <param name="observeItem">Decide if values specific changes are listened too.</param>
        public ObservableList(int capacity, bool observeItem = true)
        {
            _value = new List<Observable<T>>(capacity);
            _observeItem = observeItem;
        }

        /// <inheritdoc />
        [ExcludeFromDocFx]
        ~ObservableList()
        {
            if(_observeItem)
                foreach (Observable<T> value in _value.Where(value => value != null))
                    value.OnChange -= Item_OnChange;
        }


        /// <inheritdoc />
        [ExcludeFromDocFx]
        public IEnumerator<Observable<T>> GetEnumerator() => _value.GetEnumerator();
        /// <inheritdoc />
        [ExcludeFromDocFx]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public bool Contains(Observable<T> item) => _value.Contains(item);
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public void CopyTo(Observable<T>[] array, int arrayIndex) => _value.CopyTo(array, arrayIndex);
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public int Count => _value.Count;
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public bool IsReadOnly => false;
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public int IndexOf(Observable<T> item) => _value.IndexOf(item);

        
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public void Add(Observable<T> item)
        {
            _value.Add(item);
            if (_observeItem && item != null) item.OnChange += Item_OnChange;
            OnChange?.Invoke();
        }

        /// <summary>
        /// Add raw item after setting it as an observable item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Created observable item</returns>
        public Observable<T> Add([CanBeNull] T item)
        {
            Observable<T> value = item != null ? new Observable<T>(item) : null;
            Add(value);
            return value;
        }

        /// <inheritdoc />
        [ExcludeFromDocFx]
        public void Clear()
        {
            if(_observeItem)
                foreach (Observable<T> value in _value.Where(value => value != null))
                    value.OnChange -= Item_OnChange;
            _value.Clear();
            OnChange?.Invoke();
        }
        
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public bool Remove(Observable<T> item)
        {
            if (_observeItem && item != null) item.OnChange -= Item_OnChange;
            bool success = _value.Remove(item);
            if(success) OnChange?.Invoke();
            return success;
        }

        /// <inheritdoc />
        [ExcludeFromDocFx]
        public void Insert(int index, Observable<T> item)
        {
            if (_observeItem && item != null) item.OnChange += Item_OnChange;
            _value.Insert(index, item);
            OnChange?.Invoke();
        }

        /// <inheritdoc />
        [ExcludeFromDocFx]
        public void RemoveAt(int index)
        {
            if (_observeItem && this[index] != null) this[index].OnChange -= Item_OnChange;
            _value.RemoveAt(index);
            OnChange?.Invoke();
        }

        
        /// <inheritdoc />
        [ExcludeFromDocFx]
        public Observable<T> this[int index]
        {
            get => _value[index];
            set
            {
                Observable<T> old = _value[index];
                if (_observeItem && old != null) old.OnChange -= Item_OnChange;
                if (_observeItem && value != null) value.OnChange += Item_OnChange;
                _value[index] = value;
                OnChange?.Invoke();
            }
        }

        /// <summary>
        /// Add several items after setting them as observable items.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(params T[] items)
        {
            AddRange(items.Select(item => item != null ? new Observable<T>(item) : null).ToArray());
            OnChange?.Invoke();
        }

        /// <summary>
        /// Add several items
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(params Observable<T>[] items)
        {
            items.Where(i => i != null).ForEach(i => i.OnChange += Item_OnChange);
            _value.AddRange(items);
            OnChange?.Invoke();
        }
        
        private void Item_OnChange(T newValue, T oldValue) => OnChange?.Invoke();
    }
}