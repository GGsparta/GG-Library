using System;

namespace GGL.Collections
{
    /// <summary>
    /// Node that contains a read-only value and links to other nodes in a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ListNode<T>
    {
        /// <value>
        /// Read-only value of the node.
        /// </value>
        public T Value { get; private set; }
        
        /// <value>
        /// Link to the previous node in list.
        /// </value>
        public ListNode<T> Previous { get; private set; }
        
        /// <value>
        /// Link to the next node in list.
        /// </value>
        public ListNode<T> Next { get; private set; }
        
        /// <summary>
        /// Create a node with its read-only value.
        /// </summary>
        /// <param name="value">Value of the node</param>
        public ListNode(T value) => Value = value;

        /// <summary>
        /// Link this node to other nodes in a list.
        /// </summary>
        public void Link(ListNode<T> previous, ListNode<T> next)
        {
            Previous = previous;
            Next = next;
            previous.Next = next.Previous = this;
        }

        /// <summary>
        /// Delete the value and all links to other nodes.
        /// </summary>
        public void Purge()
        {
            Value = default;
            Previous.Next = Next.Previous = null;
            Previous = Next = null;
        }

        public static implicit operator T(ListNode<T> listNode) => listNode.Value;
    }
}