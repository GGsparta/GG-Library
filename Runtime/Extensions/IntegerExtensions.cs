using UnityEngine;

namespace GGL.Extensions
{ 
    public static class IntegerExtensions {
        
        /// <summary>
        /// Checks if value is within a range [a,b].
        /// </summary>
        public static bool IsWithin(this int value, int a, int b)
            => a <= value && value <= b;
        
        #region MATHF Overloads
        /// <inheritdoc cref="Mathf.Clamp(int,int,int)"/>
        public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
        /// <summary><see cref="Clamp(int,int,int)"/> but min=0</summary>
        public static int Clamp(this int value, int max) => Mathf.Clamp(value, 0, max);
        /// <inheritdoc cref="Mathf.Abs(int)"/>
        public static int Abs(this int value) => Mathf.Abs(value);
        #endregion
    }
}