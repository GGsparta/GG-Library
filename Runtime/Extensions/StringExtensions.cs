using System;
using System.Linq;

namespace GGL.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Return a new string with a specific part removed
        /// </summary>
        public static string Remove(this string str, string partToRemove) 
            => str.Replace(partToRemove, string.Empty);

        /// <summary>
        /// Reverse current string into a new one
        /// </summary>
        public static string Reverse(this string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        
        /// <summary>
        /// Return a new string excluding a set of characters
        /// </summary>
        public static string Subtract(this string str, params char[] excludedChars) 
            => excludedChars.Aggregate(str, (current, c) => current.Replace(c.ToString(), string.Empty));
    }
}
