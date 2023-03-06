using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace GGL.Collections
{
    /// <summary>
    /// <see cref="ICollection{T}"/> where every item knows the previous and the next item in collection.
    /// </summary>
    /// <seealso cref="ICollection" />
    /// <remarks>The last item is bind to the first one.</remarks>
    [Serializable]
    public class CircularList<T> : ICollection<T>, ICollection
    {
        [SerializeReference] private HashSet<T> content;
        private Dictionary<T, ListNode<T>> _nodes;

        /// <summary>
        /// Create an empty list
        /// </summary>
        public CircularList()
        {
            content = new HashSet<T>();
            _nodes = new Dictionary<T, ListNode<T>>();
        }

        /// <summary>
        /// Create an empty list with an initial capacity
        /// </summary>
        public CircularList(int capacity)
        {
            content = new HashSet<T>(capacity);
            _nodes = new Dictionary<T, ListNode<T>>(capacity);
        }

        /// <summary>
        /// Create a circular list from a commnon one.
        /// </summary>
        /// <param name="data"></param>
        public CircularList([NotNull, InstantHandle] IEnumerable<T> data) : this()
        {
            foreach (T item in data) Add(item);
        }

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public void Add(T item)
        {
            if (item == null) return;
            Add(item, out _);
        }

        /// <summary>
        /// Add item in circular list.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        /// <param name="createdListNode">Created node in list.</param>
        public void Add(T item, out ListNode<T> createdListNode)
        {
            if (Count > 0)
            {
                createdListNode = AddAfter(LastNode(), item);
            }
            else
            {
                ListNode<T> listNode = new(item);
                createdListNode = AddBetween(listNode, listNode, listNode);
            }
        }
        
        private ListNode<T> AddBetween(ListNode<T> previous, ListNode<T> next, ListNode<T> item)
        {
            if (_nodes.ContainsKey(item))
                throw new Exception("You cannot add a node that is already in list.");
            if (!content.Contains(previous) || !content.Contains(next))
                throw new Exception("You cannot link a node with nodes that are not in list.");
            content.Add(item);
            _nodes.Add(item, item);
            item.Link(previous, next);
            return item;
        }

        /// <summary>
        /// Add item in list right before the specified node.
        /// </summary>
        public ListNode<T> AddBefore(ListNode<T> listNode, T item) =>
            AddBetween(listNode.Previous, listNode, new ListNode<T>(item));

        /// <summary>
        /// Add item in list right after the specified node.
        /// </summary>
        public ListNode<T> AddAfter(ListNode<T> listNode, T item) =>
            AddBetween(listNode, listNode.Next, new ListNode<T>(item));

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public void Clear()
        {
            content.Clear();
            _nodes.Clear();
        }

        /// <summary>
        /// Remove a node from list
        /// </summary>
        /// <returns>True if succeeded to remove.</returns>
        public bool Remove(ListNode<T> listNode)
        {
            if (listNode.Next == null || listNode.Previous == null) return false;
            listNode.Purge();
            return content.Remove(listNode) && _nodes.Remove(listNode);
        }

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public bool Remove(T item) => item != null && _nodes.ContainsKey(item) && Remove(_nodes[item]);
        
        /// <summary>
        /// Get the node of a contained item if it exists.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ListNode<T> FindNode(T item) => _nodes[item];
        
        /// <summary>
        /// Get the first node of the list.
        /// </summary>
        public ListNode<T> FirstNode() => _nodes.Count > 0 ? _nodes[content.First()] : null;
        
        /// <summary>
        /// Get the last node of the list.
        /// </summary>
        public ListNode<T> LastNode() => _nodes.Count > 0 ? _nodes[content.Last()] : null;


        /// <inheritdoc cref="ICollection.Count" />
        [ExcludeFromDocFx]
        public int Count => content.Count;

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public bool Contains(T item) => content.Contains(item);

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public void CopyTo(T[] array, int arrayIndex) => content.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public void CopyTo(Array array, int index) => ((ICollection)content.ToList()).CopyTo(array, index);

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public bool IsSynchronized => ((ICollection)content.ToList()).IsSynchronized;

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public object SyncRoot => ((ICollection)content.ToList()).SyncRoot;

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        public IEnumerator<T> GetEnumerator() => content.GetEnumerator();

        /// <inheritdoc/>
        [ExcludeFromDocFx]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)content).GetEnumerator();
    }
}