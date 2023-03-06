using System;
using System.Collections.Generic;
using System.Linq;

namespace GGL.Extensions
{
    public static class EnumerableExtensions
    {
        private static Random _rng = new();

        /// <summary>
        /// Simulate the foreach keyword with a lambda expresion.
        /// </summary>
        /// <param name="collection">The emurable collection you want to explore.</param>
        /// <param name="callback">Code to execute for each item of the collection. (with the index in params)</param>
        /// <typeparam name="T">Collection items type.</typeparam>
        /// <returns>The same collection as a list.</returns>
        /// <remarks>A simple foreach will have slightly better performances than a lambda conversion. Please <b>do not use this method in a hot path</b>. Thank you.</remarks>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T, int> callback)
        {
            int i = 0;
            IEnumerable<T> items = collection.ToList();
            foreach (T item in items) callback(item, i++);
            return items;
        }
        
        /// <summary>
        /// Simulate the foreach keyword with a lambda expression.
        /// </summary>
        /// <param name="collection">The emurable collection you want to explore.</param>
        /// <param name="callback">Code to execute for each item of the collection. (with the index in params)</param>
        /// <typeparam name="T">Collection items type.</typeparam>
        /// <returns>The same collection as a list.</returns>
        /// <remarks>A simple foreach will have slightly better performances than a lambda conversion. Please <b>do not use this method in a hot path</b>. Thank you.</remarks>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> callback)
        {
            IEnumerable<T> items = collection.ToList();
            foreach (T item in items) callback(item);
            return items;
        }

        /// <summary>
        /// Randomly set a new order of items in a collection and returns it. 
        /// </summary>
        /// <param name="collection">The emurable collection you want to shuffle.</param>
        /// <typeparam name="T">Collection items type.</typeparam>
        /// <returns>The shuffled collection as a list.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            List<T> list = collection.ToList();
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = _rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }
    }
}